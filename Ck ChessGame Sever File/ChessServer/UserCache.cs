using EndoAshu.Chess.Server.Room;
using EndoAshu.Chess.User;
using Runetide.Attr;
using Runetide.Net.Context;
using Runetide.Util.Functions;

namespace EndoAshu.Chess.Server
{
    /// <summary>
    /// 로그인 해야 Attribute에 존재
    /// </summary>
    public class UserCache
    {
        //UserCache를 재사용하므로, 기존에 다른 Context에 등록된거 체크를 함수를 통해 구현할 예정이므로, key는 private
        private static readonly AttributeKey<UserCache> ATTRIBUTE_KEY = AttributeKey<UserCache>.Get("sse_user_cache");

        //Apply
        public static void Apply(NetworkContext ctx, UserCache cache)
        {
            ctx.GetAttribute(ATTRIBUTE_KEY).Set(cache);
        }

        /// <summary>
        /// Context에서 UserCache를 가져오는데, UserCache에 등록된 Context와 다를 경우 null 리턴
        /// </summary>
        /// <param name="ctx">가져올 Context</param>
        /// <returns></returns>
        public static UserCache? Get(NetworkContext ctx)
        {
            UserCache? cache = ctx.GetAttribute(ATTRIBUTE_KEY).Get();
            if (cache == null) return null;
            if (cache.Ctx != ctx)
            {
                ctx.GetAttribute(ATTRIBUTE_KEY).Remove();
                return null;
            }
            return cache;
        }

        internal static void GetIfPresent(NetworkContext ctx, Consumer<UserCache> consumer)
        {
            UserCache? cache = Get(ctx);
            if (cache != null)
                consumer.Invoke(cache);
        }

        internal static T GetIfPresent<T>(NetworkContext ctx, Function<UserCache, T> presentGetter, T notPresentValue)
        {
            UserCache? cache = Get(ctx);
            if (cache != null)
            {
                if (cache.CurrentRoom != null && cache.CurrentRoom.Removed)
                        cache.CurrentRoom = null;
                return presentGetter.Invoke(cache);
            }
            return notPresentValue;
        }

        private NetworkContext? context = null;
        public NetworkContext? Ctx
        {
            get
            {
                if (context != null && !context.IsConnected)
                {
                    context = null;
                }
                return context;
            }
            internal set => context = value;
        }

        public ServerRoom? CurrentRoom { get; internal set; } = null;

        public UserAccount? Account => Ctx?.GetAttribute(UserAccount.ACCOUNT_KEY).Get();

        internal UserCache(NetworkContext? context)
        {
            Ctx = context;
        }
    }
}