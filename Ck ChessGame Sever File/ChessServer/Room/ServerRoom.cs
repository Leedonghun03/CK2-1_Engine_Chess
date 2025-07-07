using EndoAshu.Chess.InGame.Pieces;
using EndoAshu.Chess.Room;
using EndoAshu.Chess.Server.InGame;
using EndoAshu.Chess.Server.User;
using EndoAshu.Chess.User;
using Runetide.Net.Context;
using Runetide.Packet;
using Runetide.Util;
using Runetide.Util.Functions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EndoAshu.Chess.Server.Room
{
    public class ServerRoom : AbstractRoom
    {
        private RoomState _state { get; set; }
        public override RoomState State => _state;
        private readonly ConcurrentDictionary<UUID, Member> members = new ConcurrentDictionary<UUID, Member>();
        public bool Removed { get; private set; } = false;
        public RoomManager Manager { get; }

        public ServerRoom(RoomManager manager) : base()
        {
            Manager = manager;
            _state = RoomState.LOBBY;
        }

        public override int GetMemberCount()
        {
            members.Values.Where(e => !e.Ctx.IsConnected).ToList().ForEach(e => KickMember(e, false));
            int c = members.Count;
            if (c <= 0)
            {
                Remove();
            }
            return c;
        }

        public override Member? GetMember(UUID uuid)
        {
            return members.TryGetValue(uuid, out Member res) ? res : null;
        }

        protected override bool KickMember(Member member, bool sendPacket = true)
        {
            if (members.TryRemove(member.UUID, out Member _))
            {
                UserCache.GetIfPresent(member.Ctx, user =>
                {
                    user.CurrentRoom = null;
                });

                if (member.UUID == RoomMasterId)
                {
                    RoomMasterId = members.Count > 0 ? members.OrderBy(e => e.Value.JoinTime).First().Key : UUID.NULL;
                }

                if (sendPacket)
                {
                    member.Ctx.Send(new ServerSideRoomQuitPacket.Response(member.UUID, RoomQuitPacket.QuitStatus.KICKED));
                    SyncAll();
                } else
                    SyncAll(e => e.UUID != member.UUID);
                return true;
            }
            return false;
        }

        public void CheckMembers()
        {
            foreach(var i in members.Values)
            {
                if (!i.Ctx.IsConnected)
                    KickMember(i, false);
            }
        }

        public override IEnumerable<UUID> GetMembersUUID()
        {
            return members.Keys;
        }

        public override bool HasMember(UUID memberId)
        {
            return members.ContainsKey(memberId);
        }

        public JoinRoomStatus Add(NetworkContext ctx)
        {
            Member member = new Member(ctx);
            if (HasMember(member.UUID)) return JoinRoomStatus.ALREADY_INSIDE_ROOM;
            return UserCache.GetIfPresent(ctx, user =>
            {
                if (user.CurrentRoom == null)
                {
                    if (GetMaxPlayers() > members.Count)
                    {
                        if (PlayingData.IsPlaying)
                            member.Mode = PlayerMode.OBSERVER;

                        if (members.TryAdd(member.UUID, member))
                        {
                            user.CurrentRoom = this;
                            SyncAll();
                            return JoinRoomStatus.SUCCESS;
                        }
                        return JoinRoomStatus.FAILED;
                    }
                    return JoinRoomStatus.ROOM_IS_FULL;
                }
                return JoinRoomStatus.ALREADY_INSIDE_OTHER_ROOM;
            }, JoinRoomStatus.FAILED);
        }

        public void SyncTo(NetworkContext ctx, params IPacket[] postPacket)
        {
            List<IPacket> pks = new List<IPacket>();
            pks.Add(new ServerSideRoomSyncPacket(this));
            if (postPacket != null)
                pks.AddRange(postPacket);
            ctx.Send(pks.ToArray());
        }

        public void Broadcast(Predicate<Member> predicate, params IPacket[] pk)
        {
            foreach (var i in members.Values)
            {
                if (predicate.Invoke(i))
                    i.Ctx.Send(pk);
            }
        }

        public void SyncAll(params IPacket[] postPacket) => SyncAll(e => true, postPacket);

        public void SyncAll(Predicate<Member> predicate, params IPacket[] postPacket)
        {
            if (PlayingData.IsPlaying)
            {
                int t1c = members.Count(e => e.Value.Mode == PlayerMode.TEAM1);
                int t2c = members.Count(e => e.Value.Mode == PlayerMode.TEAM2);

                Manager.Server.Logger?.Info($"Syncing room {RoomId} with {members.Count} members. Team1: {t1c}, Team2: {t2c}");

                if (t2c == 0 && t1c != 0)
                {
                    OnWin(PlayerMode.TEAM1);
                } else if (t1c == 0 && t2c != 0)
                {
                    OnWin(PlayerMode.TEAM2);
                } else if (t1c + t2c <= 0)
                {
                    PlayingData.Reset();
                }
            }

            foreach (var i in members.Values)
            {
                if (predicate.Invoke(i))
                    SyncTo(i.Ctx, postPacket);
            }
        }

        private object removeLock = new object();

        public void Remove()
        {
            lock (removeLock)
            {
                if (Removed) return;
                Removed = true;
            }

            foreach(var member in members.Values)
            {
                member.Ctx.Send(new ServerSideRoomQuitPacket.Response(member.UUID, RoomQuitPacket.QuitStatus.ROOM_REMOVED));
            }
            members.Clear();
        }

        public override void BuildMembers(List<RoomSyncPacket.SyncMember> members)
        {
            CheckMembers();
            foreach(var member in this.members.Values)
            {
                string name = member.Ctx.GetAttribute(UserAccount.ACCOUNT_KEY).IfPresent(e => e.Username, "<none>");
                members.Add(new RoomSyncPacket.SyncMember(member.UUID, name, member.Mode));
            }
        }

        internal bool Start(bool autoSync = true)
        {
            if (IsCanStartAble())
            {
                PlayingData.Reset();
                PlayingData.IsPlaying = true;
                PlayingData.Team1UUID.AddRange(GetMembersUUID().Where(e => GetMember(e)?.Mode == PlayerMode.TEAM1));
                PlayingData.Team2UUID.AddRange(GetMembersUUID().Where(e => GetMember(e)?.Mode == PlayerMode.TEAM2));
                if (autoSync)
                    SyncAll();
                return true;
            }
            return false;
        }

        public void OnWin(PlayerMode mode)
        {
            if (mode != PlayerMode.TEAM1 && mode != PlayerMode.TEAM2 && mode != PlayerMode.DRAW)
                return;

            foreach (UUID id in PlayingData.Team1UUID)
            {
                var account = ServerServices.GetService<IUserAccountService>().Get(id);
                if (account != null)
                {
                    if (mode == PlayerMode.DRAW)
                        account.Draw += 1;
                    else if (mode == PlayerMode.TEAM1)
                        account.Win += 1;
                    else
                        account.Lose += 1;
                    account.Save();
                }
            }

            foreach (UUID id in PlayingData.Team2UUID)
            {
                var account = ServerServices.GetService<IUserAccountService>().Get(id);
                if (account != null)
                {
                    if (mode == PlayerMode.DRAW)
                        account.Draw += 1;
                    else if (mode == PlayerMode.TEAM2)
                        account.Win += 1;
                    else
                        account.Lose += 1;
                    account.Save();
                }
            }

            var team1 = PlayingData.Team1UUID
                .Select(id => GetMember(id))
                .Select(member => member != null ? new RoomSyncPacket.SyncMember(member.UUID, member.Username, member.Mode) : null)
                .Where(e => e != null)
                .ToList();

            var team2 = PlayingData.Team2UUID
                .Select(id => GetMember(id))
                .Select(member => member != null ? new RoomSyncPacket.SyncMember(member.UUID, member.Username, member.Mode) : null)
                .Where(e => e != null)
                .ToList();

            var pk = new ServerSideChessGameEndPacket(mode, team1!, team2!);

            Broadcast(e => true, pk);

            PlayingData.Reset();
        }

        public void OnTick()
        {
            if (PlayingData.IsPlaying && PlayingData.IsPromote)
            {
                System.DateTime now = System.DateTime.UtcNow;
                if (now - PlayingData.PromoteRequestTime > System.TimeSpan.FromSeconds(10))
                {
                    lock (PlayingData.lockObj)
                    {
                        if (PlayingData.IsPromote)
                        {
                            if (PlayingData.Board[PlayingData.PromotePos.Item1, PlayingData.PromotePos.Item2] is Pawn p)
                            {
                                p.IsPromoted = true;
                            }
                            PlayingData.ClearPromote();
                            PlayingData.NextTurn();
                        }
                    }
                    SyncAll();
                }
            }
        }
    }
}
