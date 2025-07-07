using EndoAshu.Chess.InGame;
using EndoAshu.Chess.Lobby;
using EndoAshu.Chess.Room;
using EndoAshu.Chess.Server.InGame;
using EndoAshu.Chess.Server.Lobby;
using EndoAshu.Chess.Server.Room;
using EndoAshu.Chess.Server.User;
using EndoAshu.Chess.User;
using Runetide.Packet;

namespace EndoAshu.Chess.Server
{
    public class NetworkRegistry
    {
        public static void Apply(IPacketDescriptor descriptor)
        {
            descriptor.Register<ServerSideLoginPacket>(LoginPacket.PacketId);
            descriptor.Register<ServerSideLoginPacket.Response>(LoginPacket.Response.PacketId);
            descriptor.Register<ServerSideUserAccountSyncPacket>(UserAccountSyncPacket.PacketId);

            descriptor.Register<ServerSideRoomSyncPacket>(ServerSideRoomSyncPacket.PacketId);
            descriptor.Register<ServerSideRoomQuitPacket.Request>(RoomQuitPacket.Request.PacketId);
            descriptor.Register<ServerSideRoomQuitPacket.Response>(RoomQuitPacket.Response.PacketId);
            descriptor.Register<ServerSideRoomJoinPacket.Request>(RoomJoinPacket.Request.PacketId);
            descriptor.Register<ServerSideRoomJoinPacket.Response>(RoomJoinPacket.Response.PacketId);
            descriptor.Register<ServerSideRoomListPacket.Request>(RoomListPacket.Request.PacketId);
            descriptor.Register<ServerSideRoomListPacket.Response>(RoomListPacket.Response.PacketId);
            descriptor.Register<ServerSideRoomCreatePacket.Request>(RoomCreatePacket.Request.PacketId);
            descriptor.Register<ServerSideRoomCreatePacket.Response>(RoomCreatePacket.Response.PacketId);
            descriptor.Register<ServerSideRoomRemovePacket.Request>(RoomRemovePacket.Request.PacketId);
            descriptor.Register<ServerSideRoomRemovePacket.Response>(RoomRemovePacket.Response.PacketId);
            descriptor.Register<ServerSideRoomChangePlayerModePacket>(RoomChangePlayerModePacket.PacketId);
            descriptor.Register<ServerSideRoomStartPacket.Request>(RoomStartPacket.Request.PacketId);
            descriptor.Register<ServerSideRoomStartPacket.Response>(RoomStartPacket.Response.PacketId);

            descriptor.Register<ServerSideChessPawnMovePacket.Request>(ChessPawnMovePacket.Request.PacketId);
            descriptor.Register<ServerSideChessPawnMovePacket.Response>(ChessPawnMovePacket.Response.PacketId);
            descriptor.Register<ServerSideChessPawnHeldPacket.Request>(ChessPawnHeldPacket.Request.PacketId);
            descriptor.Register<ServerSideChessPawnHeldPacket.Response>(ChessPawnHeldPacket.Response.PacketId);
            descriptor.Register<ServerSideChessPawnPlacePacket.Request>(ChessPawnPlacePacket.Request.PacketId);
            descriptor.Register<ServerSideChessPawnPlacePacket.Response>(ChessPawnPlacePacket.Response.PacketId);
            descriptor.Register<ServerSideChessGhostMovePacket>(ChessGhostMovePacket.PacketId);
            descriptor.Register<ServerSideChessPawnPromotePacket.Request>(ChessPawnPromotePacket.Request.PacketId);
            descriptor.Register<ServerSideChessPawnPromotePacket.Response>(ChessPawnPromotePacket.Response.PacketId);
            descriptor.Register<ServerSideChessPawnRemovedPacket>(ChessPawnRemovedPacket.PacketId);
            descriptor.Register<ServerSideChessGameEndPacket>(ChessGameEndPacket.PacketId);
            descriptor.Register<ServerSideChessGameCheckmatePacket>(ChessGameCheckmatePacket.PacketId);

            descriptor.Register<ServerSideLobbyChatPacket>(LobbyChatPacket.PacketId);
        }
    }
}
