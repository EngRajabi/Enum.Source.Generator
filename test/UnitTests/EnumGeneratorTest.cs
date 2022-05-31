using System.ComponentModel.DataAnnotations;
using EnumFastToStringGenerated;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Supernova.Enum.Generators;

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
    public EnumGeneratorTest()
    {
    }
    [TestMethod]
    public void TestEnumDefined()
    {
        var defined = UserTypeTestExtensions.IsDefinedFast("Men");

        Assert.IsTrue(defined);
    }

    [TestMethod]
    public void TestEnumToString()
    {
        var menString = UserTypeTest.Men.StringToFast();

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
        var names = UserTypeTestExtensions.GetNamesFast();

        Assert.IsNotNull(names);
        names.Should().NotBeEmpty()
            .And.HaveCount(3)
            .And.ContainInOrder(nameof(UserTypeTest.Men), nameof(UserTypeTest.Women), nameof(UserTypeTest.None))
            .And.ContainItemsAssignableTo<string>();
    }

    [TestMethod]
    public void TestEnumGetValues()
    {
        var values = UserTypeTestExtensions.GetValuesFast();

        Assert.IsNotNull(values);
        values.Should().NotBeEmpty()
            .And.HaveCount(3)
            .And.ContainInOrder(UserTypeTest.Men, UserTypeTest.Women, UserTypeTest.None)
            .And.ContainItemsAssignableTo<UserTypeTest>();
    }

    [TestMethod]
    public void TestEnumGetLength()
    {
        var length = UserTypeTestExtensions.GetLengthFast();

        Assert.AreEqual(3, length);
    }
}
