using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;

namespace EndoAshu.Chess.InGame
{
    public abstract class ChessPawnPlacePacket
    {
        public abstract class Request : ChessPawnPacket
        {
            public static readonly int PacketId = 0x4000 | 0x5;

            protected Request(ChessPawn.Color commander, int targetX, int targetY) : base(commander, targetX, targetY)
            {
            }

            public Request(RunetideBuffer buf) : base(buf)
            {
            }
        }

        public abstract class Response : ChessPawnPacket
        {
            public static readonly int PacketId = 0x4000 | 0x6;

            protected Response(ChessPawn.Color commander, int targetX, int targetY) : base(commander, targetX, targetY)
            {
            }

            public Response(RunetideBuffer buf) : base(buf)
            {
            }
        }
    }
}
