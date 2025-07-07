using Runetide.Buffer;

namespace EndoAshu.Chess.InGame.Pieces
{
    public class King : ChessPawn
    {
        public override TypeId PawnType => TypeId.KING;

        public King(Color color) : base(color)
        {

        }

        public King(RunetideBuffer buffer) : base(buffer)
        {
        }

        public override void Write(RunetideBuffer buffer)
        {
            base.Write(buffer);
            // Additional Pawn-specific data can be written here
        }
    }
}