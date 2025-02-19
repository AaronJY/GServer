using System.Text;

namespace GServer.Common.Networking.Core;

public class MessageMemoryStream : MemoryStream
{
    public MessageMemoryStream()
    {
    }

    public MessageMemoryStream(byte[] buffer) : base(buffer)
    {
    }

    public bool ReadBoolean()
    {
        return ReadByte() == 1;
    }

    public ushort ReadUInt16()
    {
        byte[] buffer = new byte[2];
        _ = Read(buffer, 0, 2);
        return BitConverter.ToUInt16(buffer);
    }

    public short ReadInt16()
    {
        byte[] buffer = new byte[2];
        _ = Read(buffer, 0, 2);
        return BitConverter.ToInt16(buffer);
    }

    public string ReadUtf8String(int length)
    {
        byte[] bytes = new byte[length];
        _ = Read(bytes, 0, length);
        return Encoding.UTF8.GetString(bytes);
    }

    public void WriteBoolean(bool value)
    {
        WriteByte((byte)(value ? 1 : 0));
    }

    public void WriteInt16(short value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        Write(bytes, 0, 2);
    }

    public void WriteUInt16(ushort value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        Write(bytes, 0, 2);
    }

    public void WriteUtf8String(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        Write(bytes, 0, bytes.Length);
    }

    public void WriteUtf8StringOfSize(uint byteLength, string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        Write(bytes, 0, (int)byteLength);
    }

    public void WriteInt64(long value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        Write(bytes, 0, 8);
    }

    public void WriteBytes(byte[] data)
    {
        long offset = 0;
        int chunkSize = Int32.MaxValue; // Maximum size for one write operation

        while (offset < data.Length)
        {
            int remaining = (int)Math.Min(chunkSize, data.Length - offset);
            Write(data, (int)offset, remaining);
            offset += remaining;
        }
    }
}
