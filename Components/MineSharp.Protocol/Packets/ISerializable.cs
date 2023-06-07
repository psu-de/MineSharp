using MineSharp.Core.Common;

namespace MineSharp.Protocol.Packets;

public interface ISerializable<out T> where T : ISerializable<T>
{
    public void Write(PacketBuffer buffer);

    public abstract static T Read(PacketBuffer buffer);
}
