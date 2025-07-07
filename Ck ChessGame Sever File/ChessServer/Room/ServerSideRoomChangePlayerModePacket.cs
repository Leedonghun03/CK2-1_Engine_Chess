using EndoAshu.Chess.Room;
using EndoAshu.Chess.User;
using Runetide.Buffer;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;

namespace EndoAshu.Chess.Server.Room
{
    public sealed class ServerSideRoomChangePlayerModePacket : RoomChangePlayerModePacket
    {
        public ServerSideRoomChangePlayerModePacket(UUID targetUUID, PlayerMode targetMode) : base(targetUUID, targetMode)
        {
        }

        public ServerSideRoomChangePlayerModePacket(RunetideBuffer buf) : base(buf)
        {
        }

        public override void Handle(PacketContext<NetworkContext> ctx)
        {
            ctx.Get()?.GetAttribute(ChessServer.CHESS_SERVER).IfPresent(server =>
            {
                ctx.MarkHandle();
                UUID userUid = ctx.Get()!.GetAttribute(UserAccount.ACCOUNT_KEY).Get()?.UniqueId ?? UUID.NULL;
                UserCache.GetIfPresent(ctx.Get()!, cache =>
                {
                    if (cache.CurrentRoom != null)
                    {
                        if (userUid != TargetUUID && !cache.CurrentRoom.IsRoomMaster(userUid))
                        {
                            return;
                        }

                        if (cache.CurrentRoom.PlayingData.IsPlaying)
                        {
                            return;
                        }

                        if (cache.CurrentRoom.HasMember(TargetUUID))
                        {
                            cache.CurrentRoom.GetMember(TargetUUID)!.Mode = TargetMode;
                            cache.CurrentRoom.SyncAll();
                        }
                    }
                });
            });
        }
    }
}
