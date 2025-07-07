using EndoAshu.Chess.InGame;
using EndoAshu.Chess.User;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;
using Runetide.Util.Functions;

namespace EndoAshu.Chess.Server.InGame
{
    public static class ServerSideChessPawnHelper
    {
        public static int GetBoard(PacketContext<NetworkContext> ctx, BiConsumer<(NetworkContext, UserCache, UUID), ChessGamePlayingData> callback)
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

                    callback.Invoke((net, cache, userUid), data);
                    return 0;
                }, 0);
            }, 0);

            return res ?? -3;
        }

        public static int GetAccessBoard(PacketContext<NetworkContext> ctx, int x, int y, BiConsumer<(NetworkContext, UserCache, UUID), ChessGamePlayingData> callback)
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

                    //TODO : Ignore Can Move
                    /*if (member.Mode == Chess.Room.PlayerMode.TEAM1)
                    {
                        if (data.CurrentTurn != ChessGamePlayingData.Turn.WHITE)
                            return -2;
                    }
                    else if (member.Mode == Chess.Room.PlayerMode.TEAM2)
                    {
                        if (data.CurrentTurn != ChessGamePlayingData.Turn.BLACK)
                            return -2;
                    }*/

                    callback.Invoke((net, cache, userUid), data);
                    return 0;
                }, 0);
            }, 0);

            return res ?? -3;
        }
    }
}
