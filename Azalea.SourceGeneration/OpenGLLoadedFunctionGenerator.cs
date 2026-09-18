using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Azalea.SourceGeneration;

[Generator]
internal class OpenGLLoadedFunctionGenerator : IIncrementalGenerator
{
	readonly struct OpenGLLoadedFunctionData(Details details, (string, string, RefKind)[] arguments, string? docs,
		string? overrideName, bool automaticPrefix)
	{
		public Details Details { get; } = details;
		public (string, string, RefKind)[] Arguments { get; } = arguments;
		public string? Docs { get; } = docs;
		public bool AutomaticPrefix { get; } = automaticPrefix;
		public string ShorthandName { get; } = overrideName is not null
			? overrideName
			: details.Name.Substring(0, details.Name.Length - 8);
	}

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.ForAllWithAttribute(
			"Azalea.Native.OpenGL.GL+OpenGLLoadedFunctionAttribute",
			node => node is DelegateDeclarationSyntax,
			"OpenGLLoadedFunctions.g.cs",
			serializeData,
			generateSource);
	}

	private OpenGLLoadedFunctionData serializeData(AttributeData attribute, ISymbol symbol)
	{
		var namedSymbol = (INamedTypeSymbol)symbol;

		return new OpenGLLoadedFunctionData(
					GenerationExtentions.GetDetails(symbol),
					arguments: [..namedSymbol.DelegateInvokeMethod!.Parameters
					.Select(p => (p.Type.ToString(), p.Name, p.RefKind))],
					docs: attribute.ConstructorArguments[0].Value?.ToString(),
					overrideName: attribute.ConstructorArguments[1].Value?.ToString(),
					automaticPrefix: bool.Parse(attribute.ConstructorArguments[2].Value!.ToString()));
	}

	private string generateSource(ImmutableArray<OpenGLLoadedFunctionData> data)
	{
		var builder = new SourceBuilder();

		foreach (var item in data)
		{
			builder.BeginNamespace(item.Details.Namespace);
			builder.BeginClass(item.Details.ContainingDetails);

			builder.AppendLine($"private static {item.Details.Name}? __{item.Details.Name};");

			if (item.Docs is not null)
			{
				builder.Append("/// <summary><see href=\"");
				builder.Append(item.Docs);
				builder.AppendLine("\">Official Documentation</see></summary>");
			}

			builder.Append($"public static {item.Details.ReturnType} {item.ShorthandName}(");
			appendAllParameters(builder, item);
			builder.Append($") => __{item.Details.Name}!(");
			appendAllParameters(builder, item, types: false);
			builder.AppendLine(");");

			builder.EndClass(item.Details.ContainingDetails);
			builder.EndScope();
			builder.NewLine();
		}

		builder.BeginNamespace(data[0].Details.Namespace);
		builder.BeginClass(data[0].Details.ContainingDetails[0]);

		builder.AppendLine("public static bool DynamicFunctionsLoaded { get; private set; } = false;");
		builder.AppendLine("/// <summary> A valid OpenGL context must be current before calling this method </summary>");
		builder.Append("public static void LoadDynamicFunctions(Func<string, nint> getProcAddressMethod)");
		builder.BeginScope();

		foreach (var item in data)
		{
			builder.Append($"__{item.Details.Name} = System.Runtime.InteropServices.Marshal.");
			builder.Append($"GetDelegateForFunctionPointer<{item.Details.Name}>(getProcAddressMethod(\"");
			if (item.AutomaticPrefix)
				builder.Append("gl");
			builder.AppendLine($"{item.ShorthandName}\"));");
		}

		builder.Append("DynamicFunctionsLoaded = true;");

		builder.EndScope();
		builder.EndScope();
		builder.EndScope();

		return builder.ToString();

		void appendAllParameters(SourceBuilder builder, OpenGLLoadedFunctionData command, bool types = true)
		{
			for (int i = 0; i < command.Arguments.Length; i++)
			{
				var type = command.Arguments[i].Item1;
				var name = command.Arguments[i].Item2;
				var refKind = command.Arguments[i].Item3;

				switch (refKind)
				{
					case RefKind.None: break;
					case RefKind.In:
						builder.Append("in "); break;
					case RefKind.Out:
						builder.Append("out "); break;
					case RefKind.Ref:
						builder.Append("ref "); break;
				}

				if (types)
				{
					builder.Append(type);
					builder.Append(' ');
				}

				builder.Append(name);

				if (i + 1 < command.Arguments.Length)
					builder.Append(", ");
			}
		}
	}
}
