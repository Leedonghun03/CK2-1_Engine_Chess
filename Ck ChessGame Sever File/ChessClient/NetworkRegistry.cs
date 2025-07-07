using EndoAshu.Chess.Client.InGame;
using EndoAshu.Chess.Client.Lobby;
using EndoAshu.Chess.Client.Room;
using EndoAshu.Chess.Client.User;
using EndoAshu.Chess.InGame;
using EndoAshu.Chess.Lobby;
using EndoAshu.Chess.Room;
using EndoAshu.Chess.User;
using Runetide.Packet;

namespace EndoAshu.Chess.Client
{
    public class NetworkRegistry
    {
        public static void Apply(IPacketDescriptor descriptor)
        {
            descriptor.Register<ClientSideLoginPacket>(LoginPacket.PacketId);
            descriptor.Register<ClientSideLoginPacket.Response>(LoginPacket.Response.PacketId);
            descriptor.Register<ClientSideUserAccountSyncPacket>(UserAccountSyncPacket.PacketId);


            descriptor.Register<ClientSideRoomSyncPacket>(ClientSideRoomSyncPacket.PacketId);
            descriptor.Register<ClientSideRoomQuitPacket.Request>(RoomQuitPacket.Request.PacketId);
            descriptor.Register<ClientSideRoomQuitPacket.Response>(RoomQuitPacket.Response.PacketId);
            descriptor.Register<ClientSideRoomJoinPacket.Request>(RoomJoinPacket.Request.PacketId);
            descriptor.Register<ClientSideRoomJoinPacket.Response>(RoomJoinPacket.Response.PacketId);
            descriptor.Register<ClientSideRoomListPacket.Request>(RoomListPacket.Request.PacketId);
            descriptor.Register<ClientSideRoomListPacket.Response>(RoomListPacket.Response.PacketId);
            descriptor.Register<ClientSideRoomCreatePacket.Request>(RoomCreatePacket.Request.PacketId);
            descriptor.Register<ClientSideRoomCreatePacket.Response>(RoomCreatePacket.Response.PacketId);
            descriptor.Register<ClientSideRoomRemovePacket.Request>(RoomRemovePacket.Request.PacketId);
            descriptor.Register<ClientSideRoomRemovePacket.Response>(RoomRemovePacket.Response.PacketId);
            descriptor.Register<ClientSideRoomChangePlayerModePacket>(RoomChangePlayerModePacket.PacketId);
            descriptor.Register<ClientSideRoomStartPacket.Request>(RoomStartPacket.Request.PacketId);
            descriptor.Register<ClientSideRoomStartPacket.Response>(RoomStartPacket.Response.PacketId);

            descriptor.Register<ClientSideChessPawnMovePacket.Request>(ChessPawnMovePacket.Request.PacketId);
            descriptor.Register<ClientSideChessPawnMovePacket.Response>(ChessPawnMovePacket.Response.PacketId);
            descriptor.Register<ClientSideChessPawnHeldPacket.Request>(ChessPawnHeldPacket.Request.PacketId);
            descriptor.Register<ClientSideChessPawnHeldPacket.Response>(ChessPawnHeldPacket.Response.PacketId);
            descriptor.Register<ClientSideChessPawnPlacePacket.Request>(ChessPawnPlacePacket.Request.PacketId);
            descriptor.Register<ClientSideChessPawnPlacePacket.Response>(ChessPawnPlacePacket.Response.PacketId);
            descriptor.Register<ClientSideChessGhostMovePacket>(ChessGhostMovePacket.PacketId);
            descriptor.Register<ClientSideChessPawnPromotePacket.Request>(ChessPawnPromotePacket.Request.PacketId);
            descriptor.Register<ClientSideChessPawnPromotePacket.Response>(ChessPawnPromotePacket.Response.PacketId);
            descriptor.Register<ClientSideChessPawnRemovedPacket>(ChessPawnRemovedPacket.PacketId);
            descriptor.Register<ClientSideChessGameEndPacket>(ChessGameEndPacket.PacketId);
            descriptor.Register<ClientSideChessGameCheckmatePacket>(ChessGameCheckmatePacket.PacketId);

            descriptor.Register<ClientSideLobbyChatPacket>(LobbyChatPacket.PacketId);
        }
    }
}