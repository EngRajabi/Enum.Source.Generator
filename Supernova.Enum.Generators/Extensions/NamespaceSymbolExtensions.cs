using System;
using Microsoft.CodeAnalysis;

namespace Supernova.Enum.Generators.Extensions;

/// <summary>
/// Provides extension methods for <see cref="INamespaceSymbol"/> objects.
/// </summary>
public static class NamespaceSymbolExtensions
{
    /// <summary>
    /// Gets the full name of the namespace, including parent namespaces.
    /// </summary>
    /// <param name="namespaceSymbol">The namespace symbol.</param>
    /// <param name="fullName">Optional. The initial full name to start with.</param>
    /// <returns>The full name of the namespace.</returns>
    public static string FullName(this INamespaceSymbol namespaceSymbol, string fullName = null)
    {
        fullName ??= string.Empty;

        if (namespaceSymbol == null)
        {
            return fullName;
        }
        
        if (namespaceSymbol.ContainingNamespace != null)
        {
            fullName = namespaceSymbol.ContainingNamespace.FullName(fullName);
        }

        if (!fullName.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
        {
            fullName += ".";
        }

        fullName += namespaceSymbol.Name;

        return fullName;
    }
}
