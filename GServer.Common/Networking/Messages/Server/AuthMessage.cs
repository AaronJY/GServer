using System.Text;
using GServer.Common.Networking.Core;
using GServer.Common.Networking.Enums;

namespace GServer.Common.Networking.Messages.Server;

public class AuthMessage : BaseMessage, IMessage<AuthMessage>
{
    public string Username { get; }
    public string Password { get; }

    public AuthMessage(string username, string password) : base((byte)ServerPacketIn.Auth)
    {
        Username = username;
        Password = password;
    }

    public AuthMessage(MessageMemoryStream stream) : base((byte)ServerPacketIn.Auth)
    {
        byte usernameLen = (byte)stream.ReadByte();
        string username = stream.ReadUtf8String(usernameLen);

        byte passwordLen = (byte)stream.ReadByte();
        string password = stream.ReadUtf8String(passwordLen);

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