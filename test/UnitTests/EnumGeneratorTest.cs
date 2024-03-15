using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Supernova.Enum.Generators;

namespace UnitTests;

[EnumGenerator]
public enum UserTypeTest
{
    [Display(Name = "مرد", Description = "Descمرد")] Men = 3,

    [Display(Name = "زن", Description = "Descزن")] Women = 4,

    //[Display(Name = "نامشخص")]
    None
}

[TestClass]
public class EnumGeneratorTest
{
    [TestMethod]
    public void TestEnumDefined()
    {
        var defined = UserTypeTestEnumExtensions.IsDefinedFast("Men");

        Assert.IsTrue(defined);
    }

    [TestMethod]
    public void TestEnumDefined_Undefined()
    {
        var defined = UserTypeTestEnumExtensions.IsDefinedFast("DoesNotExist");

        Assert.IsFalse(defined);
    }

    [TestMethod]
    public void TestEnumToString()
    {
        var menString = UserTypeTest.Men.ToStringFast();

        Assert.AreEqual("Men", menString);
    }

    [TestMethod]
    public void TestEnumToString_Undefined()
    {
        var action = () => GetUndefinedEnumValue().ToStringFast();

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void TestEnumToString_Undefined_DefaultValue()
    {
        var value = GetUndefinedEnumValue().ToStringFast("DefaultValue");

        value.Should().Be("DefaultValue");
    }
    
    [TestMethod]
    public void TestEnumToDescription()
    {
        var menString = UserTypeTest.Men.ToDescriptionFast();

        menString.Should().Be("Descمرد");
    }

    [TestMethod]
    public void TestEnumToDescription_Undefined()
    {
        var action = () => GetUndefinedEnumValue().ToDescriptionFast();

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void TestEnumToDescription_Undefined_DefaultValue()
    {
        var value = GetUndefinedEnumValue().ToDescriptionFast("DefaultValue");

        value.Should().Be("DefaultValue");
    }

    [TestMethod]
    public void TestEnumToDisplay()
    {
        var menString = UserTypeTest.Men.ToDisplayFast();

        Assert.AreEqual("مرد", menString);
    }

    [TestMethod]
    public void TestEnumToDisplayNone()
    {
        var menString = UserTypeTest.None.ToDisplayFast();

        Assert.AreEqual("None", menString);
    }

    [TestMethod]
    public void TestEnumToDisplay_Undefined()
    {
        var action = () => GetUndefinedEnumValue().ToDisplayFast();

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [TestMethod]
    public void TestEnumToDisplay_Undefined_DefaultValue()
    {
        var value = GetUndefinedEnumValue().ToDisplayFast("DefaultValue");

        value.Should().Be("DefaultValue");
    }

    [TestMethod]
    public void TestEnumGetNames()
    {
        var names = UserTypeTestEnumExtensions.GetNamesFast();

        Assert.IsNotNull(names);
        names.Should().NotBeEmpty()
            .And.HaveCount(3)
            .And.ContainInOrder(nameof(UserTypeTest.Men), nameof(UserTypeTest.Women), nameof(UserTypeTest.None))
            .And.ContainItemsAssignableTo<string>();
    }

    [TestMethod]
    public void TestEnumDisplayNamesDictionary()
    {
        var names = UserTypeTestEnumExtensions.DisplayNamesDictionary;
        Assert.IsNotNull(names);
        names.Should().NotBeEmpty()
            .And.HaveCount(3)
            .And.ContainInOrder(
                new KeyValuePair<UserTypeTest, string>(UserTypeTest.Men, "مرد"),
                new KeyValuePair<UserTypeTest, string>(UserTypeTest.Women, "زن"),
                new KeyValuePair<UserTypeTest, string>(UserTypeTest.None, "None")
            );
    }
    
    [TestMethod]
    public void TestEnumDisplayDescriptionsDictionary()
    {
        var names = UserTypeTestEnumExtensions.DisplayDescriptionsDictionary;
        Assert.IsNotNull(names);
        names.Should().NotBeEmpty()
            .And.HaveCount(3)
            .And.ContainInOrder(
                new KeyValuePair<UserTypeTest, string>(UserTypeTest.Men, "Descمرد"),
                new KeyValuePair<UserTypeTest, string>(UserTypeTest.Women, "Descزن"),
                new KeyValuePair<UserTypeTest, string>(UserTypeTest.None, "None")
            );
    }

    [TestMethod]
    public void TestEnumGetValues()
    {
        var values = UserTypeTestEnumExtensions.GetValuesFast();

        Assert.IsNotNull(values);
        values.Should().NotBeEmpty()
            .And.HaveCount(3)
            .And.ContainInOrder(UserTypeTest.Men, UserTypeTest.Women, UserTypeTest.None)
            .And.ContainItemsAssignableTo<UserTypeTest>();
    }

    [TestMethod]
    public void TestEnumGetLength()
    {
        var length = UserTypeTestEnumExtensions.GetLengthFast();

        Assert.AreEqual(Enum.GetValues<UserTypeTest>().Length, length);
    }

    private UserTypeTest GetUndefinedEnumValue() => (UserTypeTest)(-1);
}
