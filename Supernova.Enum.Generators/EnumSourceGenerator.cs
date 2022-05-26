using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Supernova.Enum.Generators;

[Generator]
public class EnumSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {

        context.AddSource($"{SourceGeneratorHelper.AttributeName}Attribute.g.cs", SourceText.From($@"using System;
namespace {SourceGeneratorHelper.NameSpace}
{{
    [AttributeUsage(AttributeTargets.Enum)]
    public class {SourceGeneratorHelper.AttributeName}Attribute : Attribute
    {{
    }}
}}
", Encoding.UTF8));

        //context.AddSource(
        //    "EnumGeneratorAttribute.g.cs", SourceText.From(SourceGeneratorHelper.Attribute, Encoding.UTF8));

        var enums = new List<EnumDeclarationSyntax>();

        foreach (var syntaxTree in context.Compilation.SyntaxTrees)
        {
            var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);

            enums.AddRange(syntaxTree.GetRoot().DescendantNodesAndSelf()
                .OfType<EnumDeclarationSyntax>()
                .Where(x => semanticModel.GetDeclaredSymbol(x).GetAttributes()
                    .Any(x => string.Equals(x.AttributeClass.Name, SourceGeneratorHelper.AttributeName,
                        StringComparison.OrdinalIgnoreCase))));
        }


        var sourceBuilder = new StringBuilder($@"using System;
namespace {SourceGeneratorHelper.NameSpace}
{{
    public static class EnumExtensions
    {{");
        foreach (var e in enums)
        {
            var semanticModel = context.Compilation.GetSemanticModel(e.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(e);
            var symbolName = $"{symbol.ContainingNamespace}.{symbol.Name}";

            var attribute = symbol.GetAttributes()
                .FirstOrDefault(x => string.Equals(x.AttributeClass.Name, SourceGeneratorHelper.AttributeName,
                    StringComparison.OrdinalIgnoreCase));
            var argumentList = ((AttributeSyntax)attribute.ApplicationSyntaxReference.GetSyntax()).ArgumentList;
            var methodName = argumentList != null
                ? argumentList.Arguments
                    .Where(x => string.Equals(x.NameEquals.Name.Identifier.Text, "MethodName",
                        StringComparison.OrdinalIgnoreCase))
                    .Select(x => semanticModel.GetConstantValue(x.Expression).ToString())
                    .DefaultIfEmpty(SourceGeneratorHelper.ExtensionMethodName).First()
                : SourceGeneratorHelper.ExtensionMethodName;

            sourceBuilder.Append($@"
        public static string {methodName}(this {symbolName} states)
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

        sourceBuilder.Append(@"
    }
}
");
        context.AddSource("FastToStringGenerated", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }
}
