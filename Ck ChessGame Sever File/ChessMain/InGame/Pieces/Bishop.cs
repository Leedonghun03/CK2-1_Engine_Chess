using Runetide.Buffer;

namespace EndoAshu.Chess.InGame.Pieces
{
    public class Bishop : ChessPawn
    {
        public override TypeId PawnType => TypeId.BISHOP;

        public Bishop(Color color) : base(color)
        {

        }

        public Bishop(RunetideBuffer buffer) : base(buffer)
        {
        }

        public override void Write(RunetideBuffer buffer)
        {
            base.Write(buffer);
            // Additional Pawn-specific data can be written here
        }
    }
}