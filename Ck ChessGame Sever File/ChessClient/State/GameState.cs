using System;

namespace EndoAshu.Chess.Client.State
{
    public class GameState : IDisposable
    {
        public ChessClient Client { get; }

        public GameState(ChessClient client)
        {
            Client = client;
        }

        public virtual void Dispose()
        {

        }
    }
}
