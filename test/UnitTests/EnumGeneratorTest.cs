using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumFastToStringGenerated;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests;

[EnumGenerator]
public enum UserTypeTest
{
    //[Display(Name = "مرد")]
    Men,

    //[Display(Name = "زن")]
    Women,

    //[Display(Name = "نامشخص")]
    None
}


[TestClass]
public class EnumGeneratorTest
{

    [TestMethod]
    public void TestEnumDefined()
    {
        var defined =UserTypeTestEnumExtensions.IsDefined("Men");

        Assert.IsTrue(defined);
    }

    [TestMethod]
    public void TestEnumToString()
    {
        var menString =UserTypeTest.Men.StringToFast();

        Assert.AreEqual("Men", menString);
    }
}
