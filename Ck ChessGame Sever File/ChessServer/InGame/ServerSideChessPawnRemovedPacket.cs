using EndoAshu.Chess.InGame;
using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using System.Collections.Generic;

namespace EndoAshu.Chess.Server.InGame
{
    public class ServerSideChessPawnRemovedPacket : ChessPawnRemovedPacket
    {
        public ServerSideChessPawnRemovedPacket(List<(int, int, ChessPawn.Color, ChessPawn.TypeId)> removed) : base(removed)
        {
        }

        public ServerSideChessPawnRemovedPacket(RunetideBuffer buffer) : base(buffer)
        {
        }

        public override void Handle(PacketContext<NetworkContext> context)
        {
        }
    }
}
