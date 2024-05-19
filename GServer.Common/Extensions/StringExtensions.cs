using System.Text;

namespace GServer.Common;

public static class StringExtensions
{
  public static byte[] GetASCIIBytes(this string value)
  {
    return Encoding.ASCII.GetBytes(value);
  }
}
