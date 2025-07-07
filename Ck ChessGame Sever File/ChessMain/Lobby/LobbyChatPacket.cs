using Runetide.Buffer;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Lobby
{
    public abstract class LobbyChatPacket : BasePacket
    {
        public static readonly int PacketId = 0x3000 | 0x1;

        public UUID SenderId { get; }
        public string SenderName { get; }
        public string Message { get; }

        public LobbyChatPacket(UUID userUniqueId, string userName, string message)
        {
            SenderId = userUniqueId;
            SenderName = userName;
            Message = message;
        }

        public LobbyChatPacket(RunetideBuffer buffer) : base(buffer)
        {
            SenderId = buffer.ReadUUID();
            SenderName = buffer.ReadString();
            Message = buffer.ReadString();
        }

        public override void Write(RunetideBuffer buffer)
        {
            buffer.WriteUUID(SenderId);
            buffer.WriteString(SenderName);
            buffer.WriteString(Message);
        }
    }
}
