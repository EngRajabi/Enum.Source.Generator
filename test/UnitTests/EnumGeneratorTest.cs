using EnumFastToStringGenerated;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;

namespace UnitTests;

[EnumGenerator]
public enum UserTypeTest
{
    [Display(Name = "مرد")] Men,

    [Display(Name = "زن")] Women,

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
}
