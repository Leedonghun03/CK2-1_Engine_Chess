using EndoAshu.Chess.InGame;
using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using System;

namespace EndoAshu.Chess.Client.InGame
{
    public sealed class ClientSideChessGameCheckmatePacket : ChessGameCheckmatePacket
    {
        public ClientSideChessGameCheckmatePacket(RunetideBuffer buffer) : base(buffer)
        {
        }

        public ClientSideChessGameCheckmatePacket(ResultCode code, ChessPawn.Color requestColor) : base(code, requestColor)
        {
        }

        public override void Handle(PacketContext<NetworkContext> context)
        {
            throw new NotImplementedException();
        }
    }
}
