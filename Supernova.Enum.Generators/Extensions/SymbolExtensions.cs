using Microsoft.CodeAnalysis;
using System;

namespace Supernova.Enum.Generators.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ISymbol"/> objects.
/// </summary>
public static class SymbolExtensions
{
    /// <summary>
    /// Gets the full name of the symbol, including namespaces.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The full name of the symbol.</returns>
    public static string FullName(this ISymbol symbol)
    {
        // TODO: Use NamespaceSymbolExtensions.FullName after Merge of #70
        return $"{symbol.ContainingNamespace.FullNamespace()}.{symbol.Name}";
    }

    /// <summary>
    /// Gets the full name of the namespace, including parent namespaces.
    /// </summary>
    /// <param name="namespaceSymbol">The namespace symbol.</param>
    /// <param name="fullNamespace">Optional. The initial full name to start with.</param>
    /// <returns>The full name of the namespace.</returns>
    public static string FullNamespace(this INamespaceSymbol namespaceSymbol, string fullNamespace = null)
    {
        fullNamespace ??= string.Empty;

        if (namespaceSymbol == null)
        {
            return fullNamespace;
        }

        if (namespaceSymbol.ContainingNamespace != null)
        {
            fullNamespace = namespaceSymbol.ContainingNamespace.FullNamespace(fullNamespace);
        }

        if (!fullNamespace.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
        {
            fullNamespace += ".";
        }

        fullNamespace += namespaceSymbol.Name;

        return fullNamespace;
    }
}
