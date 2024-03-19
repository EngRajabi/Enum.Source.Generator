using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Supernova.Enum.Generators.Extensions;

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
        context.AddSource($"{SourceGeneratorHelper.AttributeName}Attribute.g.cs", SourceText.From($@"
using System;
using System.CodeDom.Compiler;
namespace Supernova.Enum.Generators
{{
    /// <summary>
    /// An attribute that marks enums for which extension methods are to be generated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    [GeneratedCodeAttribute(""Supernova.Enum.Generators"", null)]
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

namespace {symbol.ContainingNamespace.FullNamespace()}
{{
    /// <summary>
    /// Provides extension methods for operations related to the {symbol.Name} enumeration.
    /// </summary>
    {symbol.DeclaredAccessibility.ToString("G").ToLower()} static class {symbol.Name}EnumExtensions
    {{");

            //DisplayNames Dictionary
            DisplayNamesDictionary(sourceBuilder, symbol, e, enumDisplayNames);

            //DisplayDescriptions Dictionary
            DisplayDescriptionsDictionary(sourceBuilder, symbol, e, enumDescriptions);

            //ToStringFast
            ToStringFast(sourceBuilder, symbol, e);

            //IsDefined enum
            IsDefinedEnum(sourceBuilder, symbol, e);

            //IsDefined string
            IsDefinedString(sourceBuilder, e, symbol);

            //ToDisplay string
            ToDisplay(sourceBuilder, symbol, e, enumDisplayNames);

            //ToDisplay string
            ToDescription(sourceBuilder, symbol, e, enumDescriptions);

            //GetValues
            GetValuesFast(sourceBuilder, symbol, e);

            //GetNames
            GetNamesFast(sourceBuilder, symbol, e);

            //GetLength
            GetLengthFast(sourceBuilder, symbol, e);

            sourceBuilder.Append(@"
    }
}
");

            context.AddSource($"{symbol.Name}_EnumGenerator.g.cs",
                SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }

    private static void ToDisplay(StringBuilder sourceBuilder, ISymbol symbol, EnumDeclarationSyntax e,
        Dictionary<string, string> enumDisplayNames)
    {
        sourceBuilder.Append($@"
        /// <summary>
        /// Converts the <see cref=""{symbol.Name}"" /> enumeration value to its display string.
        /// </summary>
        /// <param name=""states"">The <see cref=""{symbol.Name}"" /> enumeration value.</param>
        /// <param name=""defaultValue"">The default value to return if the enumeration value is not recognized.</param>
        /// <returns>The display string of the <see cref=""{symbol.Name}"" /> value.</returns>
        public static string {SourceGeneratorHelper.ExtensionMethodNameToDisplay}(this {symbol.FullName()} states, string defaultValue = null)
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
                $@"                {symbol}.{member.Identifier.ValueText} => ""{enumDisplayName ?? key}"",");
        }

        sourceBuilder.Append(
            @"                _ => defaultValue ?? throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }
");
    }

    private static void ToDescription(StringBuilder sourceBuilder, ISymbol symbol, EnumDeclarationSyntax e,
        Dictionary<string, string> enumDescriptions)
    {
        sourceBuilder.Append($@"
        /// <summary>
        /// Gets the description of the <see cref=""{symbol.Name}"" /> enumeration value.
        /// </summary>
        /// <param name=""states"">The <see cref=""{symbol.Name}"" /> enumeration value.</param>
        /// <param name=""defaultValue"">The default value to return if the enumeration value is not recognized.</param>
        /// <returns>The description of the <see cref=""{symbol.Name}"" /> value.</returns>
        public static string {SourceGeneratorHelper.ExtensionMethodNameToDescription}(this {symbol.FullName()} states, string defaultValue = null)
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
                $@"                {symbol}.{member.Identifier.ValueText} => ""{enumDescription ?? key}"",");
        }

        sourceBuilder.Append(
            @"                _ => defaultValue ?? throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }
");
    }

    private static void IsDefinedString(StringBuilder sourceBuilder, EnumDeclarationSyntax e, ISymbol symbol)
    {
        sourceBuilder.Append($@"
        /// <summary>
        /// Checks if the specified string represents a defined <see cref=""{symbol.Name}"" /> value.
        /// </summary>
        /// <param name=""states"">The string representing a <see cref=""{symbol.Name}"" /> value.</param>
        /// <returns>True if the string represents a defined <see cref=""{symbol.Name}"" /> value; otherwise, false.</returns>
        public static bool {SourceGeneratorHelper.ExtensionMethodNameIsDefined}(string states)
        {{
            return states switch
            {{
");
        foreach (var member in e.Members.Select(x => x.Identifier.ValueText))
            sourceBuilder.AppendLine($@"                nameof({symbol.FullName()}.{member}) => true,");
        sourceBuilder.Append(
            @"                _ => false
            };
        }
");
    }

    private static void IsDefinedEnum(StringBuilder sourceBuilder, ISymbol symbol, EnumDeclarationSyntax e)
    {
        sourceBuilder.Append($@"
        /// <summary>
        /// Checks if the specified <see cref=""{symbol.Name}"" /> value is defined.
        /// </summary>
        /// <param name=""states"">The <see cref=""{symbol.Name}"" /> value to check.</param>
        /// <returns>True if the <see cref=""{symbol.Name}"" /> value is defined; otherwise, false.</returns>
        public static bool {SourceGeneratorHelper.ExtensionMethodNameIsDefined}({symbol.FullName()} states)
        {{
            return states switch
            {{
");
        foreach (var member in e.Members.Select(x => x.Identifier.ValueText))
            sourceBuilder.AppendLine($@"                {symbol.FullName()}.{member} => true,");
        sourceBuilder.Append(
            @"                _ => false
            };
        }
");
    }

    private static void ToStringFast(StringBuilder sourceBuilder, ISymbol symbol, EnumDeclarationSyntax e)
    {
        sourceBuilder.Append($@"
        /// <summary>
        /// Converts the <see cref=""{symbol.Name}"" /> enumeration value to its string representation.
        /// </summary>
        /// <param name=""states"">The <see cref=""{symbol.Name}"" /> enumeration value.</param>
        /// <param name=""defaultValue"">The default value to return if the enumeration value is not recognized.</param>
        /// <returns>The string representation of the <see cref=""{symbol.Name}"" /> value.</returns>
        public static string {SourceGeneratorHelper.ExtensionMethodNameToString}(this {symbol.FullName()} states, string defaultValue = null)
        {{
            return states switch
            {{
");
        foreach (var member in e.Members.Select(x => x.Identifier.ValueText))
            sourceBuilder.AppendLine($@"                {symbol.FullName()}.{member} => nameof({symbol.FullName()}.{member}),");
        sourceBuilder.Append(
            @"                _ => defaultValue ?? throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }
");
    }

    private static void DisplayNamesDictionary(StringBuilder sourceBuilder, ISymbol symbol, EnumDeclarationSyntax e,
        Dictionary<string, string> enumDisplayNames)
    {
        sourceBuilder.Append($@"
        /// <summary>
        /// Provides a dictionary that maps <see cref=""{symbol.Name}"" /> values to their corresponding display names.
        /// </summary>
        public static readonly ImmutableDictionary<{symbol.FullName()}, string> {SourceGeneratorHelper.PropertyDisplayNamesDictionary} = new Dictionary<{symbol.FullName()}, string>
        {{
");
        foreach (var member in e.Members)
        {
            var key = member.Identifier.ValueText;
            var enumDisplayName = enumDisplayNames.TryGetValue(key, out var found)
                ? found
                : key;
            sourceBuilder.AppendLine(
                $@"                {{{symbol}.{member.Identifier.ValueText}, ""{enumDisplayName ?? key}""}},");
        }
        sourceBuilder.Append(
            @"
        }.ToImmutableDictionary();
");
    }

    private static void DisplayDescriptionsDictionary(StringBuilder sourceBuilder, ISymbol symbol, EnumDeclarationSyntax e,
        Dictionary<string, string> enumDescriptionNames)
    {
        sourceBuilder.Append($@"
        /// <summary>
        /// Provides a dictionary that maps <see cref=""{symbol.Name}"" /> values to their corresponding descriptions.
        /// </summary>
        public static readonly ImmutableDictionary<{symbol.FullName()}, string> {SourceGeneratorHelper.PropertyDisplayDescriptionsDictionary} = new Dictionary<{symbol.FullName()}, string>
        {{
");
        foreach (var member in e.Members)
        {
            var key = member.Identifier.ValueText;
            var enumDescription = enumDescriptionNames.TryGetValue(key, out var found)
                ? found
                : key;
            sourceBuilder.AppendLine(
                $@"                {{{symbol.FullName()}.{member.Identifier.ValueText}, ""{enumDescription ?? key}""}},");
        }
        sourceBuilder.Append(
            @"
        }.ToImmutableDictionary();
");
    }

    private static void GetValuesFast(StringBuilder sourceBuilder, ISymbol symbol, EnumDeclarationSyntax e)
    {
        sourceBuilder.Append($@"
        /// <summary>
        /// Retrieves an array of all <see cref=""{symbol.Name}"" /> enumeration values.
        /// </summary>
        /// <returns>An array containing all <see cref=""{symbol.Name}"" /> enumeration values.</returns>
        public static {symbol.FullName()}[] {SourceGeneratorHelper.ExtensionMethodNameGetValues}()
        {{
            return new[]
            {{
");
        foreach (var member in e.Members.Select(x => x.Identifier.ValueText))
            sourceBuilder.AppendLine($@"                {symbol.FullName()}.{member},");

        sourceBuilder.Append(@"            };
        }
");
    }

    private static void GetNamesFast(StringBuilder sourceBuilder, ISymbol symbol, EnumDeclarationSyntax e)
    {
        sourceBuilder.Append($@"
        /// <summary>
        /// Retrieves an array of strings containing the names of all <see cref=""{symbol.Name}"" /> enumeration values.
        /// </summary>
        /// <returns>An array of strings containing the names of all <see cref=""{symbol.Name}"" /> enumeration values.</returns>
        public static string[] {SourceGeneratorHelper.ExtensionMethodNameGetNames}()
        {{
            return new[]
            {{
");
        foreach (var member in e.Members.Select(x => x.Identifier.ValueText))
            sourceBuilder.AppendLine($@"                nameof({symbol.FullName()}.{member}),");

        sourceBuilder.Append(@"            };
        }
");
    }

    private static void GetLengthFast(StringBuilder sourceBuilder, ISymbol symbol, EnumDeclarationSyntax e)
    {
        sourceBuilder.Append($@"
        /// <summary>
        /// Gets the length of the <see cref=""{symbol.Name}"" /> enumeration.
        /// </summary>
        /// <returns>The length of the <see cref=""{symbol.Name}"" /> enumeration.</returns>
        public static int {SourceGeneratorHelper.ExtensionMethodNameGetLength}()
        {{
            return {e.Members.Count};
");

        sourceBuilder.Append(@"
        }
");
    }
}
