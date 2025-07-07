using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Packet;

namespace EndoAshu.Chess.InGame
{
    public abstract class ChessPawnPromotePacket
    {

        public abstract class Request : BasePacket
        {
            public static readonly int PacketId = 0x4000 | 0x8;
            public ChessPawn.TypeId PromoteType { get; }

            public Request(ChessPawn.TypeId type)
            {
                PromoteType = type;
            }

            public Request(RunetideBuffer buffer) : base(buffer)
            {
                PromoteType = buffer.ReadEnum<ChessPawn.TypeId>();
            }

            public override void Write(RunetideBuffer buffer)
            {
                buffer.WriteEnum(PromoteType);
            }
        }

        public enum ResponseType
        {
            SUCCESS,
            FAIL_COLOR_MISMATCH,
            FAIL_PROMOTE_TO_KING,
            FAILURE,
            TIMEOUT_REQUEST,
            TIMEOUT_PROMOTE
        }

        public abstract class Response : BasePacket
        {
            public static readonly int PacketId = 0x4000 | 0x9;
            public ChessPawn.TypeId PromoteType { get; }
            public ResponseType Result { get; }

            public Response(ChessPawn.TypeId type, ResponseType result)
            {
                PromoteType = type;
                Result = result;
            }

            public Response(RunetideBuffer buffer) : base(buffer)
            {
                PromoteType = buffer.ReadEnum<ChessPawn.TypeId>();
                Result = buffer.ReadEnum<ResponseType>();
            }

            public override void Write(RunetideBuffer buffer)
            {
                buffer.WriteEnum(PromoteType);
                buffer.WriteEnum(Result);
            }
        }
    }
}
