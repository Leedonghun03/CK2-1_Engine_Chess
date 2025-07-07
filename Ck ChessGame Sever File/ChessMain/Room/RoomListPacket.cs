using Runetide.Buffer;
using Runetide.Packet;
using Runetide.Util;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EndoAshu.Chess.Room
{
    public class RoomListPacket
    {
        public class Item
        {
            public UUID RoomId { get; }
            public string RoomName { get; }
            public string MasterName { get; }

            public Item(UUID id, string name, string masterName)
            {
                RoomId = id;
                RoomName = name;
                MasterName = masterName;
            }

            public Item(RunetideBuffer buf)
            {
                RoomId = buf.ReadUUID();
                RoomName = buf.ReadString();
                MasterName = buf.ReadString();
            }

            public void Write(RunetideBuffer buf)
            {
                buf.WriteUUID(RoomId);
                buf.WriteString(RoomName);
                buf.WriteString(MasterName);
            }
        }

        public enum DataStatus
        {
            SUCCESS,
            FAILED,
            TIMEOUT
        }

        public class Data
        {
            public ReadOnlyCollection<Item> Items { get; }
            public int Page { get; }
            public DataStatus Status { get; }
            public int TotalPages { get; }

            public Data(int page, ReadOnlyCollection<Item> items, int totalPages)
            {
                Status = DataStatus.SUCCESS;
                Page = page;
                Items = items;
                TotalPages = totalPages;
            }

            public Data(int page, DataStatus status)
            {
                Status = status;
                Page = page;
                Items = (new List<Item>()).AsReadOnly();
                TotalPages = 1;
            }

            public Data(RunetideBuffer buf)
            {
                Status = buf.ReadEnum<DataStatus>();
                Page = buf.ReadInt32();
                List<Item> items = new List<Item>();
                int count = buf.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    items.Add(new Item(buf));
                }
                Items = items.AsReadOnly();
                TotalPages = buf.ReadInt32();
            }

            public void Write(RunetideBuffer buf)
            {
                buf.WriteEnum(Status);
                buf.WriteInt32(Page);
                buf.WriteInt32(Items.Count);
                foreach (var item in Items)
                {
                    item.Write(buf);
                }
                buf.WriteInt32(TotalPages);
            }
        }

        public abstract class Request : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0x6;
            public int Page { get; }
            public int ItemPerPage { get; }

            public Request(int page, int itemPerPage) : base()
            {
                Page = page;
                ItemPerPage = itemPerPage;
            }

            public Request(RunetideBuffer buf) : base(buf)
            {
                Page = buf.ReadInt32();
                ItemPerPage = buf.ReadInt32();
            }

            public override void Write(RunetideBuffer buf)
            {
                buf.WriteInt32(Page);
                buf.WriteInt32(ItemPerPage);
            }
        }

        public abstract class Response : BasePacket
        {
            public static readonly int PacketId = 0x2000 | 0x7;
            public Data Data { get; }

            public Response(RoomListPacket.Data data)
            {
                Data = data;
            }

            public Response(RunetideBuffer buf) : base()
            {
                Data = new Data(buf);
            }

            public override void Write(RunetideBuffer buf)
            {
                Data.Write(buf);
            }
        }
    }
}