using System;
using System.Collections.Generic;
using System.Diagnostics;
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
//#if DEBUG
//        if (!Debugger.IsAttached)
//        {
//            Debugger.Launch();
//        }
//#endif 
        context.AddSource($"{SourceGeneratorHelper.AttributeName}Attribute.g.cs", SourceText.From($@"using System;
namespace {SourceGeneratorHelper.NameSpace}
{{
    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class {SourceGeneratorHelper.AttributeName}Attribute : Attribute
    {{
    }}
}}
", Encoding.UTF8));

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


        foreach (var e in enums)
        {
            var semanticModel = context.Compilation.GetSemanticModel(e.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(e);
            var symbolName = $"{symbol.ContainingNamespace}.{symbol.Name}";

            var attribute = symbol.GetAttributes()
                .FirstOrDefault(x => string.Equals(x.AttributeClass.Name, SourceGeneratorHelper.AttributeName,
                    StringComparison.OrdinalIgnoreCase));
            var argumentList = ((AttributeSyntax)attribute.ApplicationSyntaxReference.GetSyntax()).ArgumentList;
            //var methodName = argumentList != null
            //    ? argumentList.Arguments
            //        .Where(x => string.Equals(x.NameEquals.Name.Identifier.Text, "MethodName",
            //            StringComparison.OrdinalIgnoreCase))
            //        .Select(x => semanticModel.GetConstantValue(x.Expression).ToString())
            //        .DefaultIfEmpty(SourceGeneratorHelper.ExtensionMethodName).First()
            //    : SourceGeneratorHelper.ExtensionMethodName;

            var sourceBuilder = new StringBuilder($@"using System;
namespace {SourceGeneratorHelper.NameSpace}
{{
    public static class {symbol.Name}EnumExtensions
    {{");

            //ToStringFast
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


            //IsDefined enum
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


            //IsDefined string
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

            sourceBuilder.Append(@"
    }
}
");
            context.AddSource($"{symbol.Name}_EnumGenerator.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));

        }

    }
}
