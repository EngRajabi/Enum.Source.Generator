# Supernova.Enum.Generators
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
public enum UserType
{
    Men,

    Women,
}
```

This will generate a class called `MyEnumExtensions` (by default), which contains a number of helper methods. For example:

```csharp
    public static class UserTypeTestEnumExtensions
    {
        public static string StringToFast(this UnitTests.UserTypeTest states)
        {
            return states switch
            {
                UnitTests.UserTypeTest.Men => nameof(UnitTests.UserTypeTest.Men),
                UnitTests.UserTypeTest.Women => nameof(UnitTests.UserTypeTest.Women),
                UnitTests.UserTypeTest.None => nameof(UnitTests.UserTypeTest.None),
                _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }
        public static bool IsDefined(UnitTests.UserTypeTest states)
        {
            return states switch
            {
                UnitTests.UserTypeTest.Men => true,
                UnitTests.UserTypeTest.Women => true,
                UnitTests.UserTypeTest.None => true,
                _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }
        public static bool IsDefined(string states)
        {
            return states switch
            {
                nameof(UnitTests.UserTypeTest.Men) => true,
                nameof(UnitTests.UserTypeTest.Women) => true,
                nameof(UnitTests.UserTypeTest.None) => true,
                _ => throw new ArgumentOutOfRangeException(nameof(states), states, null)
            };
        }
    }
```
