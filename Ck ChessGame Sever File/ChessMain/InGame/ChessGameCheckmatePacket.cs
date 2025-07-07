using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Packet;

namespace EndoAshu.Chess.InGame
{
    public abstract class ChessGameCheckmatePacket : BasePacket
    {
        public enum ResultCode
        {
            STALEMATE,
            CHECKMATE
        }

        public ResultCode Code { get; }
        public Pawn.Color RequestColor { get; }

        public static readonly int PacketId = 0x4000 | 0xc;

        public ChessGameCheckmatePacket(ResultCode code, Pawn.Color requestColor)
        {
            Code = code;
            RequestColor = requestColor;
        }

        public ChessGameCheckmatePacket(RunetideBuffer buffer) : base(buffer)
        {
            Code = buffer.ReadEnum<ResultCode>();
            RequestColor = buffer.ReadEnum<Pawn.Color>();
        }

        public override void Write(RunetideBuffer buffer)
        {
            buffer.WriteEnum(Code);
            buffer.WriteEnum(RequestColor);
        }
    }
}
