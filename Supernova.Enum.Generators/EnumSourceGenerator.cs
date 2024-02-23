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
            if (semanticModel.GetDeclaredSymbol(e) is not INamedTypeSymbol enumSymbol)
                // report diagnostic, something went wrong
                continue;

            var symbol = semanticModel.GetDeclaredSymbol(e);
            var symbolName = $"{symbol.ContainingNamespace}.{symbol.Name}";

            //var attribute = symbol.GetAttributes()
            //    .FirstOrDefault(x => string.Equals(x.AttributeClass.Name, SourceGeneratorHelper.AttributeName,
            //        StringComparison.OrdinalIgnoreCase));
            //var argumentList = ((AttributeSyntax)attribute.ApplicationSyntaxReference.GetSyntax()).ArgumentList;
            //var methodName = argumentList != null
            //    ? argumentList.Arguments
            //        .Where(x => string.Equals(x.NameEquals.Name.Identifier.Text, "MethodName",
            //            StringComparison.OrdinalIgnoreCase))
            //        .Select(x => semanticModel.GetConstantValue(x.Expression).ToString())
            //        .DefaultIfEmpty(SourceGeneratorHelper.ExtensionMethodName).First()
            //    : SourceGeneratorHelper.ExtensionMethodName;


            /**********************/
            var enumDisplayNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var enumDescriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
                    {
                        if (namedArgument.Key.Equals("Name", StringComparison.OrdinalIgnoreCase) &&
                         namedArgument.Value.Value?.ToString() is { } displayName)
                        {
                            enumDisplayNames.Add(member.Name, displayName);
                        } 
                        if (namedArgument.Key.Equals("Description", StringComparison.OrdinalIgnoreCase) &&
                         namedArgument.Value.Value?.ToString() is { } description)
                        {
                            enumDescriptions.Add(member.Name, description);
                        }
                        
                    }
                }
            }

            var sourceBuilder = new StringBuilder($@"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
namespace {SourceGeneratorHelper.NameSpace}
{{
    public static class {symbol.Name}EnumExtensions
    {{");

            //DisplayNames Dictionary
            DisplayNamesDictionary(sourceBuilder, symbolName, e, enumDisplayNames);
            
            //DisplayDescriptions Dictionary
            DisplayDescriptionsDictionary(sourceBuilder, symbolName, e, enumDescriptions);
            
            //ToStringFast
            ToStringFast(sourceBuilder, symbolName, e);

            //IsDefined enum
            IsDefinedEnum(sourceBuilder, symbolName, e);

            //IsDefined string
            IsDefinedString(sourceBuilder, e, symbolName);

            //ToDisplay string
            ToDisplay(sourceBuilder, symbolName, e, enumDisplayNames);

            //ToDisplay string
            ToDescription(sourceBuilder, symbolName, e, enumDescriptions);

            //GetValues
            GetValuesFast(sourceBuilder, symbolName, e);

            //GetNames
            GetNamesFast(sourceBuilder, symbolName, e);

            //GetLength
            GetLengthFast(sourceBuilder, symbolName, e);

            sourceBuilder.Append(@"
    }
}
");

            context.AddSource($"{symbol.Name}_EnumGenerator.g.cs",
                SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }

    private static void ToDisplay(StringBuilder sourceBuilder, string symbolName, EnumDeclarationSyntax e,
        Dictionary<string, string> enumDisplayNames)
    {
        sourceBuilder.Append($@"
        public static string {SourceGeneratorHelper.ExtensionMethodNameToDisplay}(this {symbolName} states, string defaultValue = null)
        {{
            return states switch
            {{
");
        foreach (var member in e.Members)
        {
            var key = member.Identifier.ValueText;
            var enumDisplayName = enumDisplayNames.TryGetValue(key, out var found)
                ? found
                : key;
            sourceBuilder.AppendLine(
                $@"                {symbolName}.{member.Identifier.ValueText} => ""{enumDisplayName ?? key}"",");
        }

        sourceBuilder.Append(
            @"                _ => defaultValue ?? throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }");
    }

    private static void ToDescription(StringBuilder sourceBuilder, string symbolName, EnumDeclarationSyntax e,
        Dictionary<string, string> enumDescriptions)
    {
        sourceBuilder.Append($@"
        public static string {SourceGeneratorHelper.ExtensionMethodNameToDescription}(this {symbolName} states, string defaultValue = null)
        {{
            return states switch
            {{
");
        foreach (var member in e.Members)
        {
            var key = member.Identifier.ValueText;
            var enumDescription = enumDescriptions.TryGetValue(key, out var found)
                ? found
                : key;
            sourceBuilder.AppendLine(
                $@"                {symbolName}.{member.Identifier.ValueText} => ""{enumDescription ?? key}"",");
        }

        sourceBuilder.Append(
            @"                _ => defaultValue ?? throw new ArgumentOutOfRangeException(nameof(states), states, null)
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
            @"                _ => false
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
            @"                _ => false
            };
        }");
    }

    private static void ToStringFast(StringBuilder sourceBuilder, string symbolName, EnumDeclarationSyntax e)
    {
        sourceBuilder.Append($@"
        public static string {SourceGeneratorHelper.ExtensionMethodNameToString}(this {symbolName} states, string defaultValue = null)
        {{
            return states switch
            {{
");
        foreach (var member in e.Members.Select(x => x.Identifier.ValueText))
            sourceBuilder.AppendLine($@"                {symbolName}.{member} => nameof({symbolName}.{member}),");
        sourceBuilder.Append(
            @"                _ => defaultValue ?? throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }");
    }

    private static void DisplayNamesDictionary(StringBuilder sourceBuilder, string symbolName, EnumDeclarationSyntax e,
        Dictionary<string, string> enumDisplayNames)
    {
        sourceBuilder.Append($@"
        public static readonly ImmutableDictionary<{symbolName}, string> {SourceGeneratorHelper.PropertyDisplayNamesDictionary} = new Dictionary<{symbolName}, string>
        {{
");
        foreach (var member in e.Members)
        {
            var key = member.Identifier.ValueText;
            var enumDisplayName = enumDisplayNames.TryGetValue(key, out var found)
                ? found
                : key;
            sourceBuilder.AppendLine(
                $@"                {{{symbolName}.{member.Identifier.ValueText}, ""{enumDisplayName ?? key}""}},");
        }
        sourceBuilder.Append(
            @"
        }.ToImmutableDictionary();
");
    }

    private static void DisplayDescriptionsDictionary(StringBuilder sourceBuilder, string symbolName, EnumDeclarationSyntax e,
        Dictionary<string, string> enumDescriptionNames)
    {
        sourceBuilder.Append($@"
        public static readonly ImmutableDictionary<{symbolName}, string> {SourceGeneratorHelper.PropertyDisplayDescriptionsDictionary} = new Dictionary<{symbolName}, string>
        {{
");
        foreach (var member in e.Members)
        {
            var key = member.Identifier.ValueText;
            var enumDescription = enumDescriptionNames.TryGetValue(key, out var found)
                ? found
                : key;
            sourceBuilder.AppendLine(
                $@"                {{{symbolName}.{member.Identifier.ValueText}, ""{enumDescription ?? key}""}},");
        }
        sourceBuilder.Append(
            @"
        }.ToImmutableDictionary();
");
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
            return {e.Members.Count};
");

        sourceBuilder.Append(@"
        }");
    }
}
