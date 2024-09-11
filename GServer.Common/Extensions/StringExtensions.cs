using System.Text;

namespace GServer.Common.Extensions;

public static class StringExtensions
{
    public static byte[] GetAsciiBytes(this string value)
    {
        return Encoding.ASCII.GetBytes(value);
    }
}
