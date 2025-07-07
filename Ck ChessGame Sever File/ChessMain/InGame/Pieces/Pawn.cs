using Runetide.Buffer;

namespace EndoAshu.Chess.InGame.Pieces
{
    public class Pawn : ChessPawn
    {
        public override TypeId PawnType => TypeId.PAWN;
        public bool IsPromoted { get; set; } = false;
        public bool IsTwiceMoved { get; set; } = false;

        public Pawn(Color color) : base(color)
        {

        }

        public Pawn(RunetideBuffer buffer) : base(buffer)
        {
            IsPromoted = buffer.ReadBool();
            IsTwiceMoved = buffer.ReadBool();
        }

        public override void Write(RunetideBuffer buffer)
        {
            base.Write(buffer);
            buffer.WriteBool(IsPromoted);
            buffer.WriteBool(IsTwiceMoved);
        }
    }
}