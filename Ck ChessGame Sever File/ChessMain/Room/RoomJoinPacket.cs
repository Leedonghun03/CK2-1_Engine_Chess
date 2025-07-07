using Runetide.Buffer;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Room
{
    public class RoomJoinPacket
    {
        public abstract class Request : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0x4;
            public UUID RoomId { get; }
            public string Password { get; }

            protected Request(UUID roomId, string password)
            {
                RoomId = roomId;
                Password = password;
            }

            protected Request(RunetideBuffer buf) : base(buf)
            {
                RoomId = buf.ReadUUID();
                Password = buf.ReadString();
            }

            public override void Write(RunetideBuffer buf)
            {
                buf.WriteUUID(RoomId);
                buf.WriteString(Password);
            }
        }

        public abstract class Response : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0x5;
            public JoinRoomStatus Status { get; }

            protected Response(JoinRoomStatus status)
            {
                Status = status;
            }

            protected Response(RunetideBuffer buf) : base(buf)
            {
                Status = buf.ReadEnum<JoinRoomStatus>();
            }

            public override void Write(RunetideBuffer buf)
            {
                buf.WriteEnum(Status);
            }
        }
    }
}