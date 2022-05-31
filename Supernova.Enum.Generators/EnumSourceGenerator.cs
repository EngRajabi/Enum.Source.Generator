using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Supernova.Enum.Generators;

[Generator]
public class EnumSourceGenerator : IIncrementalGenerator
{
    private static readonly string AttributeName = "Supernova.Enum.Generators.EnumGeneratorAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
//#if DEBUG
//        if (!Debugger.IsAttached)
//        {
//            Debugger.Launch();
//        }
//#endif

        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "EnumGeneratorAttribute.g.cs", SourceText.From($@"using System;
namespace Supernova.Enum.Generators
{{
    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class EnumGeneratorAttribute : Attribute
    {{
    }}
}}
", Encoding.UTF8)));

        var mapperClassDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => IsSyntaxTargetForGeneration(s),
                static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(r => r != null);


        var compilationAndMappers = context.CompilationProvider.Combine(mapperClassDeclarations.Collect());
        context.RegisterSourceOutput(compilationAndMappers, static
            (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is EnumDeclarationSyntax { AttributeLists.Count: > 0 };
    }


    private static EnumDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var enumDeclarationSyntax = (EnumDeclarationSyntax)context.Node;
        foreach (var attributeListSyntax in enumDeclarationSyntax.AttributeLists)
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    continue;

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == AttributeName)
                    return enumDeclarationSyntax;
            }

        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<EnumDeclarationSyntax> enums,
        SourceProductionContext context)
    {
        if (enums.IsDefaultOrEmpty)
            return;

        var distinctEnums = enums.Distinct();

        var enumAttribute = compilation.GetTypeByMetadataName(AttributeName);
        if (enumAttribute == null)
            return;
        
        foreach (var enumDeclarationSyntax in enums)
        {
            var semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
                // report diagnostic, something went wrong
                continue;

            var symbolName = $"{enumSymbol.ContainingNamespace}.{enumSymbol.Name}";
            var memberAttribute = new Dictionary<string, string>();
            foreach (var member in enumSymbol.GetMembers())
            {
                if (member is not IFieldSymbol field
                    || field.ConstantValue is null)
                    continue;

                foreach (var attribute in member.GetAttributes())
                {
                    if (attribute.AttributeClass is null ||
                        attribute.AttributeClass.Name != "DisplayAttribute") continue;

                    foreach (var namedArgument in attribute.NamedArguments)
                        if (namedArgument.Key.Equals("Name", StringComparison.OrdinalIgnoreCase) &&
                            namedArgument.Value.Value?.ToString() is { } dn)
                        {
                            memberAttribute.Add(member.Name, dn);
                            break;
                        }
                }
            }


            var sourceBuilder = new StringBuilder($@"using System;
namespace EnumFastToStringGenerated
{{
    public static class {enumSymbol.Name}Extensions
    {{");

            //ToStringFast
            ToStringFast(sourceBuilder, symbolName, enumDeclarationSyntax);

            //IsDefined enum
            IsDefinedEnum(sourceBuilder, symbolName, enumDeclarationSyntax);

            //IsDefined string
            IsDefinedString(sourceBuilder, enumDeclarationSyntax, symbolName);

            //ToDisplay string
            ToDisplay(sourceBuilder, symbolName, enumDeclarationSyntax, memberAttribute);

            //GetValues
            GetValuesFast(sourceBuilder, symbolName, enumDeclarationSyntax);

            //GetNames
            GetNamesFast(sourceBuilder, symbolName, enumDeclarationSyntax);

            //GetLength
            GetLengthFast(sourceBuilder, symbolName, enumDeclarationSyntax);

            sourceBuilder.Append(@"
    }
}
");

            context.AddSource($"{enumSymbol.Name}_EnumGenerator.g.cs",
                SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }

    private static void ToDisplay(StringBuilder sourceBuilder, string symbolName, EnumDeclarationSyntax e,
        Dictionary<string, string> memberAttribute)
    {
        sourceBuilder.Append($@"
        public static string {SourceGeneratorHelper.ExtensionMethodNameToDisplay}(this {symbolName} states)
        {{
            return states switch
            {{
");
        foreach (var member in e.Members)
        {
            var display = memberAttribute
                              .FirstOrDefault(r =>
                                  r.Key.Equals(member.Identifier.ValueText, StringComparison.OrdinalIgnoreCase))
                              .Value
                          ?? member.Identifier.ValueText;

            sourceBuilder.AppendLine(
                $@"                {symbolName}.{member.Identifier.ValueText} => ""{display}"",");
        }

        sourceBuilder.Append(
            @"                _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }");
    }

    private static void IsDefinedString(StringBuilder sourceBuilder, EnumDeclarationSyntax e, string symbolName)
    {
        sourceBuilder.Append($@"
        public static bool {SourceGeneratorHelper.ExtensionMethodNameIsDefined}(string states)
        {{
            return states switch
            {{
");
        foreach (var member in e.Members.Select(x => x.Identifier.ValueText))
            sourceBuilder.AppendLine($@"                nameof({symbolName}.{member}) => true,");
        sourceBuilder.Append(
            @"                _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }");
    }

    private static void IsDefinedEnum(StringBuilder sourceBuilder, string symbolName, EnumDeclarationSyntax e)
    {
        sourceBuilder.Append($@"
        public static bool {SourceGeneratorHelper.ExtensionMethodNameIsDefined}({symbolName} states)
        {{
            return states switch
            {{
");
        foreach (var member in e.Members.Select(x => x.Identifier.ValueText))
            sourceBuilder.AppendLine($@"                {symbolName}.{member} => true,");
        sourceBuilder.Append(
            @"                _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }");
    }

    private static void ToStringFast(StringBuilder sourceBuilder, string symbolName, EnumDeclarationSyntax e)
    {
        sourceBuilder.Append($@"
        public static string {SourceGeneratorHelper.ExtensionMethodNameToString}(this {symbolName} states)
        {{
            return states switch
            {{
");
        foreach (var member in e.Members.Select(x => x.Identifier.ValueText))
            sourceBuilder.AppendLine($@"                {symbolName}.{member} => nameof({symbolName}.{member}),");
        sourceBuilder.Append(
            @"                _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }");
    }

    private static void GetValuesFast(StringBuilder sourceBuilder, string symbolName, EnumDeclarationSyntax e)
    {
        sourceBuilder.Append($@"
        public static {symbolName}[] {SourceGeneratorHelper.ExtensionMethodNameGetValues}()
        {{
            return new[]
            {{
");
        foreach (var member in e.Members.Select(x => x.Identifier.ValueText))
            sourceBuilder.AppendLine($@"                {symbolName}.{member},");

        sourceBuilder.Append(@"            };
        }");
    }

    private static void GetNamesFast(StringBuilder sourceBuilder, string symbolName, EnumDeclarationSyntax e)
    {
        sourceBuilder.Append($@"
        public static string[] {SourceGeneratorHelper.ExtensionMethodNameGetNames}()
        {{
            return new[]
            {{
");
        foreach (var member in e.Members.Select(x => x.Identifier.ValueText))
            sourceBuilder.AppendLine($@"                nameof({symbolName}.{member}),");

        sourceBuilder.Append(@"            };
        }");
    }

    private static void GetLengthFast(StringBuilder sourceBuilder, string symbolName, EnumDeclarationSyntax e)
    {
        sourceBuilder.Append($@"
        public static int {SourceGeneratorHelper.ExtensionMethodNameGetLength}()
        {{
            return {e.Members.Count};");

        sourceBuilder.Append(@"        }");
    }
}
