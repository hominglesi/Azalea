using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Azalea.SourceGeneration;

[Generator]
internal class ObservableGenerator : IIncrementalGenerator
{
	readonly struct ObservableData(Details details, Accessibility setAccessibility)
	{
		public Details Details { get; } = details;
		public Accessibility SetAccessibility { get; } = setAccessibility;
	}

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.ForAllWithAttribute(
			"Azalea.Utils.ObservableAttribute",
			node => node is PropertyDeclarationSyntax,
			"Observable.g.cs",
			serializeData,
			generateSource);
	}

	private ObservableData serializeData(AttributeData attribute, ISymbol symbol)
	{
		var propertySymbol = (IPropertySymbol)symbol;

		return new ObservableData(
			GenerationExtentions.GetDetails(symbol),
			propertySymbol.SetMethod is null ?
				Accessibility.NotApplicable :
				propertySymbol.SetMethod.DeclaredAccessibility);
	}

	private string generateSource(ImmutableArray<ObservableData> data)
	{
		var builder = new SourceBuilder();

		foreach (var item in data)
		{
			builder.BeginNamespace(item.Details.Namespace);
			builder.BeginClass(item.Details.ContainingDetails);

			if (item.Details.ContainingDetails[0].Kind == TypeKind.Class)
			{
				#region Property Implementation

				builder.AppendAccessibility(item.Details.AccessModifier);
				if (item.Details.IsStatic)
					builder.Append("static ");
				builder.Append($"partial {item.Details.ReturnType} {item.Details.Name}");
				builder.BeginScope();
				builder.AppendLine("get;");

				if (item.SetAccessibility != Accessibility.NotApplicable)
				{
					builder.AppendAccessibility(item.SetAccessibility);
					builder.Append("set");
					builder.BeginScope();
					builder.AppendLine("if (field == value)");
					builder.AppendLine("\t return;");
					builder.AppendLine("field = value;");
					builder.Append($"On{item.Details.Name}Changed?.Invoke(field);");
					builder.EndScope();
				}

				builder.EndScope();

				#endregion

				builder.NewLine();
			}

			#region Changed Event

			builder.Append("public ");
			if(item.Details.IsStatic)
				builder.Append("static ");
			builder.Append($"event System.Action<{item.Details.ReturnType}>? On{item.Details.Name}Changed;");
			#endregion

			builder.EndClass(item.Details.ContainingDetails);
			builder.EndScope();
			builder.NewLine();
		}

		return builder.ToString();
	}
}
