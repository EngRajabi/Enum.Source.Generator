[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/EngRajabi/Enum.Source.Generator/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/dt/Supernova.Enum.Generators?label=Nuget.org%20Downloads&style=flat-square&color=blue)](https://www.nuget.org/packages/Supernova.Enum.Generators)
[![Nuget](https://img.shields.io/nuget/vpre/Supernova.Enum.Generators.svg?label=NuGet)](https://www.nuget.org/packages/Supernova.Enum.Generators)

# Supernova.Enum.Generators
# The best Source Generator for working with enums in C#
A C# source generator to create an enumeration class from an enum type.
With this package, you can work on enums very, very fast without using reflection.

**Package** - [Supernova.Enum.Generators](https://www.nuget.org/packages/Supernova.Enum.Generators/)

Add the package to your application using

```bash
dotnet add package Supernova.Enum.Generators
```

Adding the package will automatically add a marker attribute, `[EnumGenerator]`, to your project.

To use the generator, add the `[EnumGenerator]` attribute to an enum. For example:

```csharp
[EnumGenerator]
public enum UserTypeTest
{
    [Display(Name = "مرد")]
    Men,

    [Display(Name = "زن")]
    Women,

    //[Display(Name = "نامشخص")]
    None
}
```

This will generate a class called `EnumNameEnumExtensions` (`UserTypeTest` + `EnumExtensions`), which contains a number of helper methods.
For example:

```csharp
    public static class UserTypeTestEnumExtensions
    {
        public static string ToStringFast(this UnitTests.UserTypeTest states)
        {
            return states switch
            {
                UnitTests.UserTypeTest.Men => nameof(UnitTests.UserTypeTest.Men),
                UnitTests.UserTypeTest.Women => nameof(UnitTests.UserTypeTest.Women),
                UnitTests.UserTypeTest.None => nameof(UnitTests.UserTypeTest.None),
                _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }
        public static bool IsDefinedFast(UnitTests.UserTypeTest states)
        {
            return states switch
            {
                UnitTests.UserTypeTest.Men => true,
                UnitTests.UserTypeTest.Women => true,
                UnitTests.UserTypeTest.None => true,
                _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }
        public static bool IsDefinedFast(string states)
        {
            return states switch
            {
                nameof(UnitTests.UserTypeTest.Men) => true,
                nameof(UnitTests.UserTypeTest.Women) => true,
                nameof(UnitTests.UserTypeTest.None) => true,
                _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }
        public static string ToDisplayFast(this UnitTests.UserTypeTest states)
        {
            return states switch
            {
                UnitTests.UserTypeTest.Men => "مرد",
                UnitTests.UserTypeTest.Women => "زن",
                UnitTests.UserTypeTest.None => "None",
                _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }
        public static UnitTests.UserTypeTest[] GetValuesFast()
        {
            return new[]
            {
                UnitTests.UserTypeTest.Men,
                UnitTests.UserTypeTest.Women,
                UnitTests.UserTypeTest.None,
            };
        }
        public static string[] GetNamesFast()
        {
            return new[]
            {
                nameof(UnitTests.UserTypeTest.Men),
                nameof(UnitTests.UserTypeTest.Women),
                nameof(UnitTests.UserTypeTest.None),
            };
        }
        public static int GetLengthFast()
        {
            return 3;

        }
    }
```

You do not see this file inside the project. But you can use it.

Usage
```csharp
var stringEnum = UserTypeTest.Men.ToStringFast(); //Men;

var isDefined = UserTypeTestEnumExtensions.IsDefinedFast(UserType.Men); //true;

var displayEnum = UserTypeTest.Men.ToDisplayFast(); //مرد

var names = UserTypeTestEnumExtensions.GetNamesFast(); //string[]

var values = UserTypeTestEnumExtensions.GetValuesFast(); //UserType[]

var length = UserTypeTestEnumExtensions.GetLengthFast(); //3
```

If you had trouble using UserTypeTestEnumExtensions and the IDE did not recognize it. This is an IDE problem and you need to restart the IDE once.

Benchmark

![Benchmark](https://raw.githubusercontent.com/EngRajabi/Enum.Source.Generator/master/Supernova.Enum.Generators.png?v=4)

## Contributing

Create an [issue](https://github.com/EngRajabi/Enum.Source.Generator/issues/new) if you find a BUG or have a Suggestion or Question. If you want to develop this project :

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request

## Give a Star! ⭐️

If you find this repository useful, please give it a star. Thanks!

## License

Supernova.Enum.Generators is Copyright © 2022 [Mohsen Rajabi](https://github.com/EngRajabi) under the MIT License.
