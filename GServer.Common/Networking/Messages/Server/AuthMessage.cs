using System.Text;
using GServer.Common.Networking.Enums;

namespace GServer.Common.Networking.Messages.Server;

public class AuthMessage : BaseMessage, IMessage<AuthMessage>
{
    public string Username { get; private set; }
    public string Password { get; private set; }

    public AuthMessage(string username, string password) : base((byte)ServerPacketIn.AUTH)
    {
        Username = username;
        Password = password;
    }

    public AuthMessage(MessageMemoryStream stream) : base((byte)ServerPacketIn.AUTH)
    {
        // byte usernameLength = (byte)stream.ReadByte();
        // byte[] usernameBytes = new byte[usernameLength];
        // _ = stream.Read(usernameBytes, 0, usernameLength);
        // string username = Encoding.UTF8.GetString(usernameBytes);

        // byte passwordLength = (byte)stream.ReadByte();
        // byte[] passwordBytes = new byte[passwordLength];
        // _ = stream.Read(passwordBytes, 0, passwordLength);
        // string password = Encoding.UTF8.GetString(passwordBytes);

        byte usernameLen = (byte)stream.ReadByte();
        string username = stream.ReadUTF8String(usernameLen);

        byte passwordLen = (byte)stream.ReadByte();
        string password = stream.ReadUTF8String(passwordLen);

        Username = username;
        Password = password;
    }

    public byte[] Serialize()
    {
        using MemoryStream stream = new();

        stream.WriteByte(PacketId);

        byte[] usernameBytes = Encoding.UTF8.GetBytes(Username);
        stream.WriteByte((byte)usernameBytes.Length);
        stream.Write(usernameBytes, 0, usernameBytes.Length);

        byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);
        stream.WriteByte((byte)passwordBytes.Length);
        stream.Write(passwordBytes, 0, passwordBytes.Length);

        return stream.ToArray();
    }
}