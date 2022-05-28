﻿namespace Supernova.Enum.Generators;

public static class SourceGeneratorHelper
{
    private const string Header = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Supernova source generator
//     Do not change this file !!
// </auto-generated>
//------------------------------------------------------------------------------
using System;
";

    public const string NameSpace = "EnumFastToStringGenerated";
    public const string AttributeName = "EnumGenerator";
    public const string ExtensionMethodNameToString = "StringToFast";
    public const string ExtensionMethodNameIsDefined = "IsDefinedFast";
    public const string ExtensionMethodNameToDisplay = "ToDisplayFast";
    public const string ExtensionMethodNameGetValues = "GetValuesFast";
    public const string ExtensionMethodNameGetNames = "GetNamesFast";
    public const string ExtensionMethodNameGetLength = "GetLengthFast";

    public const string Attribute = Header + $@"

namespace {NameSpace};

[AttributeUsage(AttributeTargets.Enum)]
public sealed class {AttributeName}Attribute : System.Attribute
{{
}}";
}
