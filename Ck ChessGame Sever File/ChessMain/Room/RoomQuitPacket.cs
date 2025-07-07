using Runetide.Buffer;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Room
{
    public abstract class RoomQuitPacket
    {
        public enum QuitStatus
        {
            TARGET_KICKED,
            SELF_QUIT,
            KICKED,
            ROOM_REMOVED,
            NOT_HOST,
            FAILED,
            TIMEOUT
        }

        public abstract class Response : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0x2;
            public QuitStatus Status { get; }
            public UUID TargetId { get; }

            public Response(UUID target, QuitStatus status)
            {
                TargetId = target;
                Status = status;
            }

            public Response(RunetideBuffer buf) : base(buf)
            {
                TargetId = buf.ReadUUID();
                Status = buf.ReadEnum<QuitStatus>();
            }

            public override void Write(RunetideBuffer buf)
            {
                buf.WriteUUID(TargetId);
                buf.WriteEnum(Status);
            }
        }

        public abstract class Request : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0x3;
            public UUID RoomId { get; }
            public UUID TargetId { get; }

            public Request(UUID roomId, UUID targetId)
            {
                RoomId = roomId;
                TargetId = targetId;
            }

            public Request(RunetideBuffer buf) : base(buf)
            {
                RoomId = buf.ReadUUID();
                TargetId = buf.ReadUUID();
            }

            public override void Write(RunetideBuffer buf)
            {
                buf.WriteUUID(RoomId);
                buf.WriteUUID(TargetId);
            }
        }
    }
}
