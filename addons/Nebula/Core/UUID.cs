using System;
using Godot;
using Nebula.Serialization;
using Nebula.Utility.Tools;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Nebula {

    /**
    <summary>
    A UUID implementation for Nebula. Serializes into 16 bytes.
    </summary>
    */
    public partial class UUID : RefCounted, INetSerializable<UUID>, IBsonSerializable<UUID> {
        public Guid Guid => new(_bytes);
        public static UUID Empty { get; } = new UUID("00000000-0000-0000-0000-000000000000");
        private byte[] _bytes = Guid.Empty.ToByteArray();

        public UUID() {
            _bytes = Guid.NewGuid().ToByteArray();
        }

        public UUID(string value) {
            _bytes = Guid.Parse(value).ToByteArray();
        }

        public UUID(byte[] value) {
            _bytes = value;
        }

        public override string ToString() {
            return Guid.ToString();
        }

        public byte[] ToByteArray() {
            return _bytes;
        }

        public static HLBuffer NetworkSerialize(WorldRunner currentWorld, NetPeer peer, UUID obj)
        {
            var buffer = new HLBuffer();
            HLBytes.Pack(buffer, obj.ToByteArray());
            return buffer;
        }

        public static UUID NetworkDeserialize(WorldRunner currentWorld, NetPeer peer, HLBuffer buffer, UUID initialObject)
        {
            return new UUID(HLBytes.UnpackByteArray(buffer, 16));
        }
        public static UUID BsonDeserialize(Variant context, byte[] bson, UUID initialObject)
        {
            var bsonValue = BsonTransformer.Instance.DeserializeBsonValue<BsonBinaryData>(bson);
            var guid = GuidConverter.FromBytes(bsonValue.Bytes, GuidRepresentation.Standard);
            var result = new UUID(guid.ToByteArray());
            return result;
        }
        public BsonValue BsonSerialize(Variant context)
        {
            return new BsonBinaryData(Guid, GuidRepresentation.Standard);
        }
    }
}