using System.Text;
using GServer.Common.Networking.Core;
using GServer.Common.Networking.Enums;

namespace GServer.Common.Networking.Messages.Client;

public enum AuthResponseFailure : byte
{
    IncorrectLoginOrPassword,
    Unknown
}

public class AuthResponseMessage : BaseMessage, IMessage<AuthResponseMessage>
{
    public bool IsSuccessful { get; }

    /// <summary>
    /// Used to authenticate the user. Only set if IsSuccessful is true.
    /// </summary>
    public string? SessionToken { get; }

    /// <summary>
    /// Reason for auth failure. Only set is IsSuccessful is false.
    /// </summary>
    public AuthResponseFailure? FailureReason { get; }

    public AuthResponseMessage(bool isSuccessful, string? sessionToken = null, AuthResponseFailure? failureReason = null) : base((byte)ClientPacketIn.AuthResponse)
    {
        IsSuccessful = isSuccessful;
        SessionToken = sessionToken;
        FailureReason = failureReason;
    }

    public AuthResponseMessage(MessageMemoryStream stream) : base((byte)ClientPacketIn.AuthResponse)
    {
        IsSuccessful = stream.ReadBoolean();

        if (IsSuccessful)
        {
            ushort sessionTokenLen = stream.ReadUInt16();
            SessionToken = stream.ReadUtf8String(sessionTokenLen);
        }
        else
        {
            FailureReason = (AuthResponseFailure)stream.ReadByte();
        }
    }

    public byte[] Serialize()
    {
        using MessageMemoryStream stream = new();

        stream.WriteByte(PacketId);
        stream.WriteBoolean(IsSuccessful);

        if (IsSuccessful)
        {
            short sessionTokenByteLen = (short)Encoding.UTF8.GetByteCount(SessionToken!);
            stream.WriteInt16(sessionTokenByteLen);
            stream.WriteUtf8String(SessionToken!);
        }
        else
        {
            stream.WriteByte((byte)FailureReason!);
        }

        return stream.ToArray();
    }
}
