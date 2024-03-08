﻿namespace Supernova.Enum.Generators;

public static class EnumGeneratorAttributeHelper
{
    public const string Name = "EnumGenerator";

    public const string Content = @"// <auto-generated />

using System;
using System.CodeDom.Compiler;

namespace " + SourceGeneratorHelper.NameSpace + @"
{
    [AttributeUsage(AttributeTargets.Enum)]
    [GeneratedCodeAttribute(""Supernova.Enum.Generators"", null)]
    public sealed class " + Name + @"Attribute : Attribute
    {
    }
}
";
}
