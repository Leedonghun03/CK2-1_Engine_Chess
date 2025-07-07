using Runetide.Buffer;

namespace EndoAshu.Chess.InGame.Pieces
{
    public class Rook : ChessPawn
    {
        public override TypeId PawnType => TypeId.ROOK;

        public Rook(Color color) : base(color)
        {

        }

        public Rook(RunetideBuffer buffer) : base(buffer)
        {
        }

        public override void Write(RunetideBuffer buffer)
        {
            base.Write(buffer);
            // Additional Pawn-specific data can be written here
        }
    }
}