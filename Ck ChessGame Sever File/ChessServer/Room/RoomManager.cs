using EndoAshu.Chess.Room;
using Runetide.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EndoAshu.Chess.Server.Room
{
    public class RoomManager
    {
        private readonly ConcurrentDictionary<UUID, ServerRoom> sessions = new ConcurrentDictionary<UUID, ServerRoom>();
        public ChessServer Server { get; }

        public int Count => sessions.Count;

        internal RoomManager(ChessServer server)
        {
            Server = server;
        }

        public ServerRoom? GetRoom(UUID roomId)
        {
            return sessions.TryGetValue(roomId, out var room) ? room : null;
        }

        public ICollection<ServerRoom> GetRooms(int page, int itemsPerPage)
        {
            return sessions.OrderBy(kv => kv.Key)
            .Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .Select(e => e.Value)
            .ToList();
        }

        /// <summary>
        /// 룸을 생성합니다.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal ServerRoom? Create(UUID userId, RoomOptions options)
        {
            var list = sessions.Values.Where(e => e.HasMember(userId));
            if (list.Count() > 0) return null;
            ServerRoom room = new ServerRoom(this);
            room.Options = options;
            room.RoomId = UUID.Create();
            room.RoomMasterId = userId;
            var result = sessions.TryAdd(room.RoomId, room) ? room : null;
            Server.Logger?.Debug($"Create Room : {userId}, ");
            return result;
        }

        internal bool Remove(UUID roomId)
        {
            if (sessions.TryRemove(roomId, out var room))
            {
                room.Remove();
                return true;
            }
            return false;
        }

        public bool CheckIfEmpty(ServerRoom room)
        {
            if (room.GetMemberCount() > 0) return false;
            return sessions.TryRemove(room.RoomId, out var _);
        }

        public void OnTick()
        {
            foreach (var room in sessions.Values)
            {
                if (!CheckIfEmpty(room))
                {
                    room.OnTick();
                }
            }
        }
    }
}
