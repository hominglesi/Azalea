using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Azalea.SourceGeneration;

[Generator]
internal class ThreadCommandGenerator : IIncrementalGenerator
{
	readonly struct ThreadCommandData(Details details, string parentType, bool isAwaitable, bool generateHandler, string? displayName,
		List<(string, string)> properties)
	{
		public string ParentType { get; } = parentType;
		public Details Details { get; } = details;
		public bool IsAwaitable { get; } = isAwaitable;
		public bool GenerateHandler { get; } = generateHandler;
		public string DisplayName { get; } = displayName is not null ? displayName :
			(details.Name.EndsWith("Command") ? details.Name.Substring(0, details.Name.Length - 7) : details.Name);
		public List<(string, string)> Properties { get; } = properties;
	}

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.ForAllWithAttribute(
			"Azalea.Threading.ThreadCommandAttribute",
			node => node is ClassDeclarationSyntax,
			"ThreadCommands.g.cs",
			serializeData,
			generateSource);
	}

	private ThreadCommandData serializeData(AttributeData attribute, ISymbol symbol)
	{
		var namedSymbol = (INamedTypeSymbol)symbol;

		return new ThreadCommandData(
			GenerationExtentions.GetDetails(symbol),
			parentType: namedSymbol.BaseType!.Name,
			isAwaitable: (bool)attribute.ConstructorArguments[0].Value!,
			generateHandler: (bool)attribute.ConstructorArguments[1].Value!,
			displayName: (string?)attribute.ConstructorArguments[2].Value!,
			properties: [.. namedSymbol.GetMembers()
					.OfType<IFieldSymbol>()
					.Select(p => (p.Type.ToDisplayString(), p.Name))]);
	}

	private string generateSource(ImmutableArray<ThreadCommandData> data)
	{
		var builder = new SourceBuilder();

		foreach (var item in data)
		{
			builder.BeginNamespace(item.Details.Namespace);

			builder.AppendAccessibility(item.Details.AccessModifier);

			builder.Append($"partial class {item.Details.Name}");
			if (item.IsAwaitable)
				builder.Append(" : Azalea.Threading.ICommandAwaitable");
			builder.BeginScope();

			#region Constructor

			builder.Append($"private {item.Details.Name}(");
			appendAllParameters(builder, item);
			builder.Append(')');
			builder.BeginScope();


			builder.AppendLine("TotalCreated++;");
			for (int i = 0; i < item.Properties.Count; i++)
				builder.AppendLine($"{item.Properties[i].Item2} = __{item.Properties[i].Item2};");
			builder.Append("OnCommandCreated?.Invoke(this);");
			builder.EndScope();
			builder.NewLine();

			#endregion

			// CommandPool
			builder.Append($"private static readonly System.Collections.Concurrent.ConcurrentBag");
			builder.AppendLine($"<{item.Details.Name}> __commandPool = new();");

			// TotalCreated
			builder.AppendLine("internal static volatile new int TotalCreated = 0;");
			
			#region BorrowMethod

			builder.Append($"public static {item.Details.Name} Borrow(");
			appendAllParameters(builder, item);
			builder.Append(')');
			builder.BeginScope();

			builder.Append("if (__commandPool.TryTake(out var command))");
			builder.BeginScope();

			foreach (var property in item.Properties)
				builder.AppendLine($"command.{property.Item2} = __{property.Item2};");

			builder.Append("return command;");
			builder.EndScope();
			builder.NewLine();

			builder.Append($"return new {item.Details.Name} (");
			appendAllParameters(builder, item, types: false);
			builder.Append(");");
			builder.EndScope();
			builder.NewLine();

			#endregion

			#region Return

			builder.Append("public override void Return()");
			builder.BeginScope();
			builder.AppendLine("Cleanup();");
			if (item.IsAwaitable)
			{
				builder.AppendLine("_completedEvent.Set();");
				builder.AppendLine("_completedEvent.Reset();");
			}
			builder.Append("__commandPool.Add(this);");
			builder.EndScope();
			builder.NewLine();

			#endregion

			#region Deconstruct

			builder.Append("public void Deconstruct(");
			appendAllParameters(builder, item, @out: true);
			builder.Append(")");
			builder.BeginScope();

			for (int i = 0; i < item.Properties.Count; i++)
			{
				var name = item.Properties[i].Item2;

				builder.Append($"__{name} = {name};");

				if (i + 1 < item.Properties.Count)
					builder.NewLine();
			}

			builder.EndScope();

			#endregion

			#region Awaitable

			if (item.IsAwaitable)
			{
				builder.NewLine();

				builder.AppendLine("private readonly System.Threading.ManualResetEvent _completedEvent = new(false);");
				builder.Append("public void Await() => _completedEvent.WaitOne();");
			}

			#endregion

			builder.EndScope();

			#region GenerateHandler

			if (item.GenerateHandler)
			{
				builder.NewLine();

				builder.Append($"public static class {item.Details.Name}_Handler");
				builder.BeginScope();
				builder.Append($"public static {(item.IsAwaitable ? "Azalea.Threading.ICommandAwaitable" : "void")} {item.DisplayName}(this Azalea.Threading.ICommandHandler<{item.ParentType}> handler");

				if (item.Properties.Count > 0)
				{
					builder.Append(", ");
					appendAllParameters(builder, item);
				}

				builder.AppendLine(")");
				builder.Append($"\t=> handler.Enqueue({item.Details.Name}.Borrow(");
				appendAllParameters(builder, item, types: false);
				builder.Append("));");
				builder.EndScope();
			}

			#endregion

			builder.EndScope();
			builder.NewLine();
		}

		return builder.ToString();

		void appendAllParameters(SourceBuilder builder, ThreadCommandData command, bool types = true, bool @out = false)
		{
			for (int i = 0; i < command.Properties.Count; i++)
			{
				var type = command.Properties[i].Item1;
				var name = command.Properties[i].Item2;

				if (@out)
					builder.Append("out ");

				if (types)
				{
					builder.Append(type);
					builder.Append(' ');
				}

				builder.Append("__");
				builder.Append(name);

				if (i + 1 < command.Properties.Count)
					builder.Append(", ");
			}
		}
	}
}
