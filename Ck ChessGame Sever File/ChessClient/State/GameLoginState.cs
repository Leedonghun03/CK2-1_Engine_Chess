using EndoAshu.Chess.Client.User;
using EndoAshu.Chess.User;
using System.Threading.Tasks;

namespace EndoAshu.Chess.Client.State
{
    public sealed class GameLoginState : GameState
    {
        public GameLoginState(ChessClient client) : base(client)
        {

        }

        #region Login
        internal TaskCompletionSource<ClientSideLoginPacket.Response>? LoginResponse;

        /// <summary>
        /// 계정을 사용해 로그인합니다.
        /// 서버 전송 시 자동으로 암호화됩니다.
        /// </summary>
        /// <param name="id">유저 ID</param>
        /// <param name="pw">유저 비밀번호</param>
        /// <param name="timeoutMS">timeout 시간</param>
        /// <returns></returns>
        public async Task<LoginPacket.LoginStatus> Login(string id, string pw, int timeoutMS = 5000)
        {
            if (LoginResponse != null)
                return LoginPacket.LoginStatus.FAILED;

            LoginResponse = new TaskCompletionSource<ClientSideLoginPacket.Response>();
            Client.Send(new ClientSideLoginPacket(id, pw));

            LoginPacket.LoginStatus result;
            try
            {
                var ack = await LoginResponse.Task.WaitAsync(timeoutMS);
                result = ack.Result;
            }
            catch
            {
                result = LoginPacket.LoginStatus.TIMEOUT;
            }

            LoginResponse = null;

            if (result == LoginPacket.LoginStatus.SUCCESS)
            {
                Client.UpdateState(new GameLobbyState(Client));
            }
            return result;
        }
        #endregion

        public override void Dispose()
        {
            LoginResponse?.SetCanceled();
        }

        public void __InternalResetLogin()
        {
            LoginResponse?.TrySetCanceled();
            LoginResponse = null;
        }
    }
}
