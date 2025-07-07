using EndoAshu.Chess.InGame;
using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using System;
using System.Collections.Generic;

namespace EndoAshu.Chess.Client.InGame
{
    public class ClientSideChessGameEndPacket : ChessGameEndPacket
    {
        public ClientSideChessGameEndPacket(RunetideBuffer buffer) : base(buffer)
        {
        }

        public ClientSideChessGameEndPacket(PlayerMode winnerTeam, List<RoomSyncPacket.SyncMember> team1, List<RoomSyncPacket.SyncMember> team2) : base(winnerTeam, team1, team2)
        {
        }

        public override void Handle(PacketContext<NetworkContext> context)
        {
            context.MarkHandle();
        }
    }
}
