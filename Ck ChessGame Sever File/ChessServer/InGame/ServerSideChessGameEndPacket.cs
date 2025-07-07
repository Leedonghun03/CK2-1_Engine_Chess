using EndoAshu.Chess.InGame;
using EndoAshu.Chess.Room;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using System.Collections.Generic;

namespace EndoAshu.Chess.Server.InGame
{
    public class ServerSideChessGameEndPacket : ChessGameEndPacket
    {
        public ServerSideChessGameEndPacket(RunetideBuffer buffer) : base(buffer)
        {
        }

        public ServerSideChessGameEndPacket(PlayerMode winnerTeam, List<RoomSyncPacket.SyncMember> team1, List<RoomSyncPacket.SyncMember> team2) : base(winnerTeam, team1, team2)
        {
        }

        public override void Handle(PacketContext<NetworkContext> context)
        {

        }
    }
}
