using EndoAshu.Chess.InGame;
using EndoAshu.Chess.InGame.Pieces;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using System.Collections.Generic;

namespace EndoAshu.Chess.Client.InGame
{
    public sealed class ClientSideChessPawnRemovedPacket : ChessPawnRemovedPacket
    {
        public ClientSideChessPawnRemovedPacket(List<(int, int, ChessPawn.Color, ChessPawn.TypeId)> removed) : base(removed)
        {
        }

        public ClientSideChessPawnRemovedPacket(RunetideBuffer buffer) : base(buffer)
        {
        }

        public override void Handle(PacketContext<NetworkContext> context)
        {
            context.MarkHandle();
        }
    }
}
