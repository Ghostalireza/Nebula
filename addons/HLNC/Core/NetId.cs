using Godot;
using HLNC.Serialization;
using HLNC.Utils.Bson;
using MongoDB.Bson;

namespace HLNC
{
    [SerialTypeIdentifier("NetId")]
    public partial class NetId : RefCounted, INetSerializable<NetId>, IBsonSerializable<NetId>
    {
        public static int NONE = -1;
        public NetNodeWrapper Node { get; private set; }
        public long Value { get; private set; }
        internal NetId(long value)
        {
            Value = value;
        }
        public static NetId BsonDeserialize(Variant context, byte[] bson, NetId initialObject)
        {
            var bsonValue = DataTransformer.Instance.DeserializeBsonValue<BsonInt64>(bson);
            return new NetId(bsonValue.Value);
        }
        public BsonValue BsonSerialize(Variant context)
        {
            return new BsonInt64(Value);
        }
        public static HLBuffer NetworkSerialize(WorldRunner currentWorld, NetPeer peer, NetId obj)
        {
            var buffer = new HLBuffer();
            if (NetRunner.Instance.IsServer) {
                HLBytes.Pack(buffer, currentWorld.GetPeerWorldState(peer).Value.WorldToPeerNodeMap[obj]);
            } else {
                HLBytes.Pack(buffer, (byte)obj.Value);
            }
            return buffer;
        }
        public static NetId NetworkDeserialize(WorldRunner currentWorld, NetPeer peer, HLBuffer buffer, NetId initialObject)
        {
            if (NetRunner.Instance.IsServer) {
                var id = HLBytes.UnpackInt8(buffer);
                return currentWorld.GetNetIdFromPeerId(peer, id);
            } else {
                var id = HLBytes.UnpackByte(buffer);
                return currentWorld.GetNetId(id);
            }
        }
    }
}