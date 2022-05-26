using EnumFastToStringGenerated;
namespace Console.Test;


[EnumGenerator]
public enum UserType
{
    //[Display(Name = "مرد")]
    Men,

    //[Display(Name = "زن")]
    Women,

    //[Display(Name = "نامشخص")]
    None
}

public class Program
{
    static void Main(string[] args)
    {
        var state = UserType.Men;
        var ff = state.StringToFast();
    }
}
