namespace GServer.Common.Networking.Messages;

public interface IMessage<T>
{
    byte[] Serialize();
}