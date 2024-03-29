using System;
using Microsoft.CodeAnalysis;

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
        => $"{symbol.ContainingNamespace.FullNamespace()}.{symbol.Name}";
}
