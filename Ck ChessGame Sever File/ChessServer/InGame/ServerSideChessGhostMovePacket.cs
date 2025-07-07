using EndoAshu.Chess.InGame;
using EndoAshu.Chess.User;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Server.InGame
{
    public sealed class ServerSideChessGhostMovePacket : ChessGhostMovePacket
    {
        public ServerSideChessGhostMovePacket(RunetideBuffer buffer) : base(buffer)
        {
        }
        public ServerSideChessGhostMovePacket(UUID id, (float, float, float) position, (float, float, float) velocity) : base(id, position, velocity)
        {
        }
        public ServerSideChessGhostMovePacket(UUID id, (float, float, float) position, (float, float, float) velocity, long timestamp) : base(id, position, velocity, timestamp)
        {
        }
        public override void Handle(PacketContext<NetworkContext> ctx)
        {
            NetworkContext? net = ctx.Get();
            int? res = net?.GetAttribute(ChessServer.CHESS_SERVER).IfPresent(server =>
            {
                UUID userUid = ctx.Get()!.GetAttribute(UserAccount.ACCOUNT_KEY).Get()?.UniqueId ?? UUID.NULL;
                return UserCache.GetIfPresent(ctx.Get()!, (cache) =>
                {
                    if (cache.CurrentRoom == null)
                        return -1;

                    var member = cache.CurrentRoom.GetMember(userUid);
                    if (member == null)
                        return -1;

                    var data = cache.CurrentRoom.PlayingData;
                    if (!data.IsPlaying)
                        return -1;

                    var mode = cache.CurrentRoom.GetMember(userUid)?.Mode;
                    if (mode != Chess.Room.PlayerMode.TEAM1 &&
                        mode != Chess.Room.PlayerMode.TEAM2)
                    {
                        return -4;
                    }

                    ctx.MarkHandle();
                    var pk = new ServerSideChessGhostMovePacket(userUid, Position, Velocity);
                    cache.CurrentRoom.Broadcast((member) => true, pk);

                    return 0;
                }, -2);
            }, -3);
        }
    }
}
