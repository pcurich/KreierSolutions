namespace Ks.Admin.Extensions
{
    public static class StringHelper
    {
        public static string GetLine(this string str , string type="_")
        {
            var large = str.Length;
            var result = "";
            for (var i = 0; i < large; i++)
            {
                result += type;
            }
            return result;
        }
    }
}