namespace WebApp.Extensions;

public static class StringExtensions
{
    public static string EncodePlusSign(this string str)
    {
        return str.Replace("+", "%2B");
    }
}
