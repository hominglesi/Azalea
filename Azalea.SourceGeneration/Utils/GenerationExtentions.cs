using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Azalea.SourceGeneration;

internal static class GenerationExtentions
{
	public static void ForAllWithAttribute<T>(this IncrementalGeneratorInitializationContext context,
		string attributeName, Func<SyntaxNode, bool> nodePredicate, string resultingClass,
		Func<AttributeData, ISymbol, T> dataCreation,
		Func<ImmutableArray<T>, string> classCreation)
	{
		var provider = context.SyntaxProvider.ForAttributeWithMetadataName(attributeName,
			predicate: (node, _) => nodePredicate(node),
			transform: (ctx, _) =>
			{
				var attribute = ctx.Attributes.First(
					a => a.AttributeClass?.ToString() == attributeName);

				return dataCreation(attribute, ctx.TargetSymbol);
			});

		context.RegisterSourceOutput(provider.Collect(),
			(ctx, commands) => ctx.AddSource(resultingClass, classCreation(commands)));
	}

	public static Details GetDetails(ISymbol symbol)
	{
		var @namespace = symbol.ContainingNamespace.IsGlobalNamespace ?
						string.Empty
						: symbol.ContainingNamespace.ToDisplayString();

		var builder = ImmutableArray.CreateBuilder<Details>(4);
		var current = symbol.ContainingType;

		while(current is not null)
		{
			var typeParameters = ImmutableArray<string>.Empty;

			if(current.TypeParameters.Length > 0)
			{
				var parametersBuilder = ImmutableArray.CreateBuilder<string>();
				foreach (var typeParam in current.TypeParameters)
					parametersBuilder.Add(typeParam.Name);

				typeParameters = parametersBuilder.ToImmutable();
			}

			builder.Add(new Details(
				containingDetails: ImmutableArray<Details>.Empty,
				@namespace: @namespace,
				name: current.Name,
				accessModifier: current.DeclaredAccessibility,
				isAbstract: current.IsAbstract,
				isStatic: current.IsStatic,
				typeParameters: typeParameters,
				kind: current.TypeKind,
				returnType: null));

			current = current.ContainingType;
		}

		ITypeSymbol? returnType = null;

		if(symbol is IPropertySymbol propertySymbol)
			returnType = propertySymbol.Type;


		return new Details(
					containingDetails: builder.ToImmutable(),
					@namespace: @namespace,
					name: symbol.Name,
					accessModifier: symbol.DeclaredAccessibility,
					isAbstract: symbol.IsAbstract,
					isStatic: symbol.IsStatic,
					typeParameters: ImmutableArray<string>.Empty,
					kind: TypeKind.Unknown,
					returnType: returnType);
	}
}

public readonly struct Details(ImmutableArray<Details> containingDetails, string @namespace, string name,
	Accessibility accessModifier, bool isAbstract, bool isStatic, ImmutableArray<string> typeParameters,
	TypeKind kind, ITypeSymbol? returnType)
{
	public ImmutableArray<Details> ContainingDetails { get; } = containingDetails;

	public string Namespace { get; } = @namespace;
	public string Name { get; } = name;
	public Accessibility AccessModifier { get; } = accessModifier;
	public bool IsAbstract { get; } = isAbstract;
	public bool IsStatic { get; } = isStatic;
	public ImmutableArray<string> TypeParameters { get; } = typeParameters;
	public TypeKind Kind { get; } = kind;
	public ITypeSymbol? ReturnType { get; } = returnType;
}
