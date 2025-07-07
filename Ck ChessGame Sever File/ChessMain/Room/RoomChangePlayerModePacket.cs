using Runetide.Buffer;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Room
{
    public abstract class RoomChangePlayerModePacket : BasePacket
    {
        public static readonly int PacketId = 0x2000 | 0xc;

        public UUID TargetUUID { get; }
        public PlayerMode TargetMode { get; }

        public RoomChangePlayerModePacket(UUID targetUUID, PlayerMode targetMode)
        {
            TargetUUID = targetUUID;
            TargetMode = targetMode;
        }
        public RoomChangePlayerModePacket(RunetideBuffer buf)
        {
            TargetUUID = buf.ReadUUID();
            TargetMode = buf.ReadEnum<PlayerMode>();
        }

        public override void Write(RunetideBuffer buf)
        {
            buf.WriteUUID(TargetUUID);
            buf.WriteEnum(TargetMode);
        }
    }
}
