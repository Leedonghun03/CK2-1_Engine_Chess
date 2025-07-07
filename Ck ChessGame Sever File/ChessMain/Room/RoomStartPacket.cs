using Runetide.Buffer;
using Runetide.Packet;

namespace EndoAshu.Chess.Room
{
    public sealed class RoomStartPacket
    {
        public abstract class Request : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0xd;

            public Request()
            {

            }

            public Request(RunetideBuffer buf)
            {
            }

            public override void Write(RunetideBuffer buffer)
            {

            }
        }

        public enum ResultCode
        {
            SUCCESS,
            NOT_HOST,
            FAILED,
            TIMEOUT
        }

        public abstract class Response : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0xe;

            public ResultCode Code { get; }

            public Response(ResultCode code)
            {
                Code = code;
            }

            public Response(RunetideBuffer buf)
            {
                Code = buf.ReadEnum<ResultCode>();
            }

            public override void Write(RunetideBuffer buffer)
            {
                buffer.WriteEnum(Code);
            }
        }
    }
}
