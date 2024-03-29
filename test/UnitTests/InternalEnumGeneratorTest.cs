using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Supernova.Enum.Generators;

namespace UnitTests;

[EnumGenerator]
internal enum InternalUserTypeTest
{
    [Display(Name = "مرد", Description = "Descمرد")] Men = 3,

    [Display(Name = "زن", Description = "Descزن")] Women = 4,

    //[Display(Name = "نامشخص")]
    None
}

[TestClass]
public class InternalEnumGeneratorTest
{
    [TestMethod]
    public void TestEnumDefined()
    {
        var defined = InternalUserTypeTestEnumExtensions.IsDefinedFast("Men");

        Assert.IsTrue(defined);
    }

    [TestMethod]
    public void TestEnumDefined_Undefined()
    {
        var defined = InternalUserTypeTestEnumExtensions.IsDefinedFast("DoesNotExist");

        Assert.IsFalse(defined);
    }

    [TestMethod]
    public void TestEnumToString()
    {
        var menString = InternalUserTypeTest.Men.ToStringFast();

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
        var menString = InternalUserTypeTest.Men.ToDescriptionFast();

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
        var menString = InternalUserTypeTest.Men.ToDisplayFast();

        Assert.AreEqual("مرد", menString);
    }

    [TestMethod]
    public void TestEnumToDisplayNone()
    {
        var menString = InternalUserTypeTest.None.ToDisplayFast();

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
        var names = InternalUserTypeTestEnumExtensions.GetNamesFast();

        Assert.IsNotNull(names);
        names.Should().NotBeEmpty()
            .And.HaveCount(3)
            .And.ContainInOrder(nameof(InternalUserTypeTest.Men), nameof(InternalUserTypeTest.Women), nameof(InternalUserTypeTest.None))
            .And.ContainItemsAssignableTo<string>();
    }

    [TestMethod]
    public void TestEnumDisplayNamesDictionary()
    {
        var names = InternalUserTypeTestEnumExtensions.DisplayNamesDictionary;
        Assert.IsNotNull(names);
        names.Should().NotBeEmpty()
            .And.HaveCount(3)
            .And.ContainInOrder(
                new KeyValuePair<InternalUserTypeTest, string>(InternalUserTypeTest.Men, "مرد"),
                new KeyValuePair<InternalUserTypeTest, string>(InternalUserTypeTest.Women, "زن"),
                new KeyValuePair<InternalUserTypeTest, string>(InternalUserTypeTest.None, "None")
            );
    }
    
    [TestMethod]
    public void TestEnumDisplayDescriptionsDictionary()
    {
        var names = InternalUserTypeTestEnumExtensions.DisplayDescriptionsDictionary;
        Assert.IsNotNull(names);
        names.Should().NotBeEmpty()
            .And.HaveCount(3)
            .And.ContainInOrder(
                new KeyValuePair<InternalUserTypeTest, string>(InternalUserTypeTest.Men, "Descمرد"),
                new KeyValuePair<InternalUserTypeTest, string>(InternalUserTypeTest.Women, "Descزن"),
                new KeyValuePair<InternalUserTypeTest, string>(InternalUserTypeTest.None, "None")
            );
    }

    [TestMethod]
    public void TestEnumGetValues()
    {
        var values = InternalUserTypeTestEnumExtensions.GetValuesFast();

        Assert.IsNotNull(values);
        values.Should().NotBeEmpty()
            .And.HaveCount(3)
            .And.ContainInOrder(InternalUserTypeTest.Men, InternalUserTypeTest.Women, InternalUserTypeTest.None)
            .And.ContainItemsAssignableTo<InternalUserTypeTest>();
    }

    [TestMethod]
    public void TestEnumGetLength()
    {
        var length = InternalUserTypeTestEnumExtensions.GetLengthFast();

        Assert.AreEqual(Enum.GetValues<InternalUserTypeTest>().Length, length);
    }

    private InternalUserTypeTest GetUndefinedEnumValue() => (InternalUserTypeTest)(-1);
}
