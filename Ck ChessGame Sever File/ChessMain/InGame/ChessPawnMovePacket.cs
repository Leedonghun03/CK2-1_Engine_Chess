using Runetide.Buffer;
using Runetide.Packet;

namespace EndoAshu.Chess.InGame
{
    public abstract class ChessPawnMovePacket
    {
        public enum ResultCode
        {
            SUCCESS,
            INVALID_TURN,
            INVALID_MOVE,
            INVALID_POSITION,
            INVALID_DESTINATION,
            INVALID_PIECE,
            INVALID_COLOR,
            TIMEOUT,
            FAILED
        }

        public abstract class Response : BasePacket
        {
            public static readonly int PacketId = 0x4000 | 0x1;
            public ResultCode Code { get; }

            public Response(ResultCode code)
            {
                Code = code;
            }

            public Response(RunetideBuffer buffer) : base(buffer)
            {
                Code = buffer.ReadEnum<ResultCode>();
            }

            public override void Write(RunetideBuffer buffer)
            {
                buffer.WriteEnum(Code);
            }
        }

        public abstract class Request : BasePacket
        {
            public static readonly int PacketId = 0x4000 | 0x2;

            public int CurrentX { get; }
            public int CurrentY { get; }
            public int DestinationX { get; }
            public int DestinationY { get; }

            protected Request(int x, int y, int destX, int destY)
            {
                CurrentX = x;
                CurrentY = y;
                DestinationX = destX;
                DestinationY = destY;
            }

            protected Request(RunetideBuffer buffer) : base(buffer)
            {
                CurrentX = buffer.ReadInt32();
                CurrentY = buffer.ReadInt32();
                DestinationX = buffer.ReadInt32();
                DestinationY = buffer.ReadInt32();
            }

            public override void Write(RunetideBuffer buffer)
            {
                buffer.WriteInt32(CurrentX);
                buffer.WriteInt32(CurrentY);
                buffer.WriteInt32(DestinationX);
                buffer.WriteInt32(DestinationY);
            }
        }
    }
}
