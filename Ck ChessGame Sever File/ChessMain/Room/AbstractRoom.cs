using EndoAshu.Chess.User;
using Runetide.Net.Context;
using Runetide.Util;
using System;
using System.Collections.Generic;
using EndoAshu.Chess.InGame;

namespace EndoAshu.Chess.Room
{
    public enum PlayerMode
    {
        WAIT,
        TEAM1,//(WHITE)
        TEAM2,//(BLACK)
        OBSERVER,
        DRAW//only winner 
    }

    public enum RoomState
    {
        LOBBY,
        IN_GAME
    }

    public enum JoinRoomStatus
    {
        SUCCESS,
        ROOM_IS_FULL,
        ALREADY_INSIDE_OTHER_ROOM,
        ALREADY_INSIDE_ROOM,
        FAILED,
        ROOM_NOT_FOUND,
        INCORRECT_PASSWORD,
        TIMEOUT
    }

    public abstract class AbstractRoom
    {
        public class Member
        {
            public NetworkContext Ctx { get; private set; }
            public UUID UUID { get; }
            public PlayerMode Mode { get; set; } = PlayerMode.WAIT;
            public string Username => Ctx.GetAttribute(UserAccount.ACCOUNT_KEY).Get()?.Username ?? "<none>";
            public DateTime JoinTime { get; }

            public Member(NetworkContext ctx)
            {
                Ctx = ctx;
                UUID = ctx.GetAttribute(UserAccount.ACCOUNT_KEY).Get()?.UniqueId ?? UUID.NULL;
                JoinTime = DateTime.Now;
            }

            /// <summary>
            /// NetworkContext를 새로 업데이트합니다.
            /// 단, 기존 Context와 계정 정보가 다르면 실패합니다.
            /// </summary>
            /// <param name="ctx">업데이트 할 Context</param>
            /// <returns>성공 여부</returns>
            public bool UpdateContext(NetworkContext ctx)
            {
                if (UUID != ctx.GetAttribute(UserAccount.ACCOUNT_KEY).Get()?.UniqueId)
                    return false;
                Ctx = ctx;
                return true;
            }
        }

        public UUID RoomId { get; set; } = UUID.NULL;
        public RoomOptions Options { get; set; }
        public abstract RoomState State { get; }
        public UUID RoomMasterId { get; set; } = UUID.NULL;

        public ChessGamePlayingData PlayingData { get; set; } = new ChessGamePlayingData();

        public AbstractRoom()
        {
            Options = new RoomOptions();
        }

        public abstract bool HasMember(UUID memberId);

        public bool KickMember(UUID uuid, bool sendPacket = true)
        {
            Member? member = GetMember(uuid);
            if (member != null)
                return KickMember(member, sendPacket);
            return false;
        }

        protected abstract bool KickMember(Member member, bool sendPacket = true);
        public abstract Member? GetMember(UUID uuid);
        public abstract int GetMemberCount();

        public abstract IEnumerable<UUID> GetMembersUUID();

        public abstract void BuildMembers(List<RoomSyncPacket.SyncMember> members);

        public bool IsRoomMaster(UUID uuid) => uuid == RoomMasterId;
        protected bool IsRoomMaster(Member member) => member.UUID == RoomMasterId;

        //TODO : 나중에 룸 최대 인원 구현
        public int GetMaxPlayers()
        {
            return 10;
        }

        /// <summary>
        /// 시작 가능한지 여부
        /// 한명도 WAIT상태가 아니면서
        /// TEAM1, TEAM2는 인원수가 맞을 때
        /// </summary>
        /// <returns>시작 가능여부</returns>
        public bool IsCanStartAble()
        {
            if (PlayingData.IsPlaying) return false;
            int t1c = 0, t2c = 0;
            foreach(var i in GetMembersUUID())
            {
                var member = GetMember(i)!;
                if (member.Mode == PlayerMode.WAIT) return false;
                if (member.Mode == PlayerMode.TEAM1) ++t1c;
                if (member.Mode == PlayerMode.TEAM2) ++t2c;
            }

            //TODO : 나중에 삭제하기
            return t1c + t2c > 0;

            if (t1c != t2c) return false;
            if (Options.Mode == GameMode.ONE_VS_ONE && t1c == 1) return true;
            if (Options.Mode == GameMode.TWO_VS_TWO && t1c == 2) return true;
            return false;
        }
    }
}