using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Azalea.SourceGeneration;

[Generator]
internal class ObservablePropertyGenerator : IIncrementalGenerator
{
	readonly struct ObservablePropertyData(Details details, Accessibility setAccessibility)
	{
		public Details Details { get; } = details;
		public Accessibility SetAccessibility { get; } = setAccessibility;
	}

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.ForAllWithAttribute(
			"Azalea.Utils.ObservablePropertyAttribute",
			node => node is PropertyDeclarationSyntax,
			"ObservableProperties.g.cs",
			serializeData,
			generateSource);
	}

	private ObservablePropertyData serializeData(AttributeData attribute, ISymbol symbol)
	{
		var propertySymbol = (IPropertySymbol)symbol;

		return new ObservablePropertyData(
			GenerationExtentions.GetDetails(symbol),
			propertySymbol.SetMethod is null ?
				Accessibility.NotApplicable :
				propertySymbol.SetMethod.DeclaredAccessibility);
	}

	private string generateSource(ImmutableArray<ObservablePropertyData> data)
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

			builder.Append($"public event System.Action<{item.Details.ReturnType}>? On{item.Details.Name}Changed;");

			#endregion

			builder.EndClass(item.Details.ContainingDetails);
			builder.EndScope();
			builder.NewLine();
		}

		return builder.ToString();
	}
}
