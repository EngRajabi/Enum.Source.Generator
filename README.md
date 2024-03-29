[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/EngRajabi/Enum.Source.Generator/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/dt/Supernova.Enum.Generators?label=Nuget.org%20Downloads&style=flat-square&color=blue)](https://www.nuget.org/packages/Supernova.Enum.Generators)
[![Nuget](https://img.shields.io/nuget/vpre/Supernova.Enum.Generators.svg?label=NuGet)](https://www.nuget.org/packages/Supernova.Enum.Generators)

<p align="center">
 <a href="https://www.buymeacoffee.com/mohsenrajabi" target="_blank">
  <img src="https://cdn.buymeacoffee.com/buttons/v2/default-orange.png" height="61" width="194" />
 </a>
</p>


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
    [Display(Name = "مرد", Description = "Descمرد")] Men = 3,

    [Display(Name = "زن", Description = "Descزن")] Women = 4,

    //[Display(Name = "نامشخص")]
    None
}
```

This will generate a class called `EnumNameEnumExtensions` (`UserTypeTest` + `EnumExtensions`), which contains a number of helper methods.
For example:

```csharp

    /// <summary>
    /// Provides extension methods for operations related to the <see cref="global::UnitTests.UserTypeTest" /> enumeration.
    /// </summary>    
    [GeneratedCodeAttribute("Supernova.Enum.Generators", null)]
    public static class UserTypeTestEnumExtensions
    {
        /// <summary>
        /// Provides a dictionary that maps <see cref="global::UnitTests.UserTypeTest" /> values to their corresponding display names.
        /// </summary>
        public static readonly ImmutableDictionary<UnitTests.UserTypeTest, string> DisplayNamesDictionary = new Dictionary<UnitTests.UserTypeTest, string>
        {
                {UnitTests.UserTypeTest.Men, "مرد"},
                {UnitTests.UserTypeTest.Women, "زن"},
                {UnitTests.UserTypeTest.None, "None"},

        }.ToImmutableDictionary();

        /// <summary>
        /// Provides a dictionary that maps <see cref="global::UnitTests.UserTypeTest" /> values to their corresponding descriptions.
        /// </summary>
        public static readonly ImmutableDictionary<UnitTests.UserTypeTest, string> DisplayDescriptionsDictionary = new Dictionary<UnitTests.UserTypeTest, string>
        {
                {UnitTests.UserTypeTest.Men, "Descمرد"},
                {UnitTests.UserTypeTest.Women, "Descزن"},
                {UnitTests.UserTypeTest.None, "None"},

        }.ToImmutableDictionary();

        /// <summary>
        /// Converts the <see cref="global::UnitTests.UserTypeTest" /> enumeration value to its string representation.
        /// </summary>
        /// <param name="states">The <see cref="global::UnitTests.UserTypeTest" /> enumeration value.</param>
        /// <param name="defaultValue">The default value to return if the enumeration value is not recognized.</param>
        /// <returns>The string representation of the <see cref="global::UnitTests.UserTypeTest" /> value.</returns>
        public static string ToStringFast(this UnitTests.UserTypeTest states, string defaultValue = null)
        {
            return states switch
            {
                UnitTests.UserTypeTest.Men => nameof(UnitTests.UserTypeTest.Men),
                UnitTests.UserTypeTest.Women => nameof(UnitTests.UserTypeTest.Women),
                UnitTests.UserTypeTest.None => nameof(UnitTests.UserTypeTest.None),
                _ => defaultValue ?? throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }

        /// <summary>
        /// Checks if the specified <see cref="global::UnitTests.UserTypeTest" /> value is defined.
        /// </summary>
        /// <param name="states">The <see cref="global::UnitTests.UserTypeTest" /> value to check.</param>
        /// <returns>True if the <see cref="global::UnitTests.UserTypeTest" /> value is defined; otherwise, false.</returns>
        public static bool IsDefinedFast(UnitTests.UserTypeTest states)
        {
            return states switch
            {
                UnitTests.UserTypeTest.Men => true,
                UnitTests.UserTypeTest.Women => true,
                UnitTests.UserTypeTest.None => true,
                _ => false
            };
        }

        /// <summary>
        /// Checks if the specified string represents a defined <see cref="global::UnitTests.UserTypeTest" /> value.
        /// </summary>
        /// <param name="states">The string representing a <see cref="global::UnitTests.UserTypeTest" /> value.</param>
        /// <returns>True if the string represents a defined <see cref="global::UnitTests.UserTypeTest" /> value; otherwise, false.</returns>
        public static bool IsDefinedFast(string states)
        {
            return states switch
            {
                nameof(UnitTests.UserTypeTest.Men) => true,
                nameof(UnitTests.UserTypeTest.Women) => true,
                nameof(UnitTests.UserTypeTest.None) => true,
                _ => false
            };
        }

        /// <summary>
        /// Converts the <see cref="global::UnitTests.UserTypeTest" /> enumeration value to its display string.
        /// </summary>
        /// <param name="states">The <see cref="global::UnitTests.UserTypeTest" /> enumeration value.</param>
        /// <param name="defaultValue">The default value to return if the enumeration value is not recognized.</param>
        /// <returns>The display string of the <see cref="global::UnitTests.UserTypeTest" /> value.</returns>
        public static string ToDisplayFast(this UnitTests.UserTypeTest states, string defaultValue = null)
        {
            return states switch
            {
                UnitTests.UserTypeTest.Men => "مرد",
                UnitTests.UserTypeTest.Women => "زن",
                UnitTests.UserTypeTest.None => "None",
                _ => defaultValue ?? throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }

        /// <summary>
        /// Gets the description of the <see cref="global::UnitTests.UserTypeTest" /> enumeration value.
        /// </summary>
        /// <param name="states">The <see cref="global::UnitTests.UserTypeTest" /> enumeration value.</param>
        /// <param name="defaultValue">The default value to return if the enumeration value is not recognized.</param>
        /// <returns>The description of the <see cref="global::UnitTests.UserTypeTest" /> value.</returns>
        public static string ToDescriptionFast(this UnitTests.UserTypeTest states, string defaultValue = null)
        {
            return states switch
            {
                UnitTests.UserTypeTest.Men => "Descمرد",
                UnitTests.UserTypeTest.Women => "Descزن",
                UnitTests.UserTypeTest.None => "None",
                _ => defaultValue ?? throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }

        /// <summary>
        /// Retrieves an array of all <see cref="global::UnitTests.UserTypeTest" /> enumeration values.
        /// </summary>
        /// <returns>An array containing all <see cref="global::UnitTests.UserTypeTest" /> enumeration values.</returns>
        public static UnitTests.UserTypeTest[] GetValuesFast()
        {
            return new[]
            {
                UnitTests.UserTypeTest.Men,
                UnitTests.UserTypeTest.Women,
                UnitTests.UserTypeTest.None,
            };
        }

        /// <summary>
        /// Retrieves an array of strings containing the names of all <see cref="global::UnitTests.UserTypeTest" /> enumeration values.
        /// </summary>
        /// <returns>An array of strings containing the names of all <see cref="global::UnitTests.UserTypeTest" /> enumeration values.</returns>
        public static string[] GetNamesFast()
        {
            return new[]
            {
                nameof(UnitTests.UserTypeTest.Men),
                nameof(UnitTests.UserTypeTest.Women),
                nameof(UnitTests.UserTypeTest.None),
            };
        }

        /// <summary>
        /// Gets the length of the <see cref="global::UnitTests.UserTypeTest" /> enumeration.
        /// </summary>
        /// <returns>The length of the <see cref="global::UnitTests.UserTypeTest" /> enumeration.</returns>
        public static int GetLengthFast()
        {
            return 3;

        }

        /// <summary>
        /// Try parse a string to <see cref="global::UnitTests.UserTypeTest" /> value.
        /// </summary>
        /// <param name="states">The string representing a <see cref="global::UnitTests.UserTypeTest" /> value.</param>
        /// <param name="result">The enum <see cref="global::UnitTests.UserTypeTest" /> parse result.</param>
        /// <returns>True if the string is parsed successfully; otherwise, false.</returns>
        public static bool TryParseFast(string states, out UnitTests.UserTypeTest result)
        {
            switch (states)
            {
                case "Men": 
                {
                    result = UnitTests.UserTypeTest.Men;
                    return true;
                }
                case "Women": 
                {
                    result = UnitTests.UserTypeTest.Women;
                    return true;
                }
                case "None": 
                {
                    result = UnitTests.UserTypeTest.None;
                    return true;
                }
                default: {
                    result = default;
                    return false;
                }
            }
        }
    }
```

You do not see this file inside the project. But you can use it.

Usage
```csharp
var stringEnum = UserTypeTest.Men.ToStringFast(); //Men;

var isDefined = UserTypeTestEnumExtensions.IsDefinedFast(UserType.Men); //true;

var displayEnum = UserTypeTest.Men.ToDisplayFast(); //مرد

var descriptionEnum = UserTypeTest.Men.ToDescriptionFast(); //Descمرد

var names = UserTypeTestEnumExtensions.GetNamesFast(); //string[]

var values = UserTypeTestEnumExtensions.GetValuesFast(); //UserType[]

var length = UserTypeTestEnumExtensions.GetLengthFast(); //3

var menString = "Men";
var result = UserTypeTestEnumExtensions.TryParseFast(menString, out var enumValue);
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
