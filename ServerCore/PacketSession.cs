namespace ServerCore;

public abstract class PacketSession : Session
{
    public static readonly int HeaderSize = 2;
    public sealed override int OnRecv(ArraySegment<byte> buffer)
    {
        int processLength = 0;

        while (true)
        {
            if (buffer.Count < HeaderSize)
                break;
            ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            if (buffer.Count < dataSize)
                break;

            OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
            processLength += dataSize;
            buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
        }
        
        return processLength;
    }

    public abstract void OnRecvPacket(ArraySegment<byte> buffer);
}