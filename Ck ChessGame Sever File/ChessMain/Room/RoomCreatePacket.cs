using Runetide.Buffer;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Room
{
    public class RoomCreatePacket
    {
        public abstract class Request : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0x8;
            public RoomOptions Options { get; }

            public Request(RoomOptions options)
            {
                Options = options;
            }

            public Request(RunetideBuffer buf) : base(buf)
            {
                Options = buf.ReadJson<RoomOptions>()!;
            }

            public override void Write(RunetideBuffer buf)
            {
                buf.WriteJson(Options);
            }
        }

        public enum CreateStatus
        {
            SUCCESS,
            OPTION_NOT_ALLOWED,
            ALREADY_INSIDE,
            FAILED,
            TIMEOUT
        }

        public abstract class Response : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0x9;
            public CreateStatus Status { get; }
            public UUID RoomId { get; }

            public Response(CreateStatus status, UUID? roomId)
            {
                Status = status;
                RoomId = roomId ?? UUID.NULL;
            }

            public Response(RunetideBuffer buf)
            {
                Status = buf.ReadEnum<CreateStatus>();
                RoomId = buf.ReadUUID();
            }

            public override void Write(RunetideBuffer buf)
            {
                buf.WriteEnum(Status);
                buf.WriteUUID(RoomId);
            }
        }
    }
}