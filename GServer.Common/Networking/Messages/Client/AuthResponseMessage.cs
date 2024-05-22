using System.Text;
using GServer.Common.Networking.Enums;

namespace GServer.Common.Networking.Messages.Client;

public enum AuthResponseFailure : byte
{
    IncorrectLoginOrPassword,
    Unknown
}

public class AuthResponseMessage : BaseMessage, IMessage<AuthResponseMessage>
{
    public bool IsSuccessful { get; private set; }

    /// <summary>
    /// Used to authenticate the user. Only set if IsSuccessful is true.
    /// </summary>
    public string? SessionToken { get; private set; }

    /// <summary>
    /// Reason for auth failure. Only set is IsSuccessful is false.
    /// </summary>
    public AuthResponseFailure? FailureReason { get; private set; }

    public AuthResponseMessage(bool isSuccessful, string? sessionToken = null, AuthResponseFailure? failureReason = null) : base((byte)ClientPacketIn.AUTH_RESPONSE)
    {
        IsSuccessful = isSuccessful;
        SessionToken = sessionToken;
        FailureReason = failureReason;
    }

    public AuthResponseMessage(MessageNetworkStream stream) : base((byte)ClientPacketIn.AUTH_RESPONSE)
    {
        IsSuccessful = stream.ReadBoolean();

        if (IsSuccessful)
        {
            ushort sessionTokenLen = stream.ReadUInt16();
            SessionToken = stream.ReadUTF8String(sessionTokenLen);
        }
        else
        {
            FailureReason = (AuthResponseFailure)stream.ReadByte();
        }
    }

    public byte[] Serialize()
    {
        using MessageNetworkStream stream = new();

        stream.WriteByte(PacketId);
        stream.WriteBoolean(IsSuccessful);

        if (IsSuccessful)
        {
            short sessionTokenByteLen = (short)Encoding.UTF8.GetByteCount(SessionToken!);
            stream.WriteUInt16(sessionTokenByteLen);
            stream.WriteUTF8String(SessionToken!);
        }
        else
        {
            stream.WriteByte((byte)FailureReason!);
        }

        return stream.ToArray();
    }
}
