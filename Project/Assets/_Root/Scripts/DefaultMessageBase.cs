using System;
using Ceras;
using Mirror;
using Newtonsoft.Json;

public abstract class DefaultMessageBase : IMessageBase
{
    protected abstract void CopyFrom(object obj);

    //Json
    //public void Deserialize(NetworkReader reader)
    //{
    //    CopyFrom(JsonConvert.DeserializeObject(reader.ReadString(), GetType()));
    //}
    //public void Serialize(NetworkWriter writer)
    //{
    //    writer.WriteString(JsonConvert.SerializeObject(this));
    //}

    //Ceras
    public void Deserialize(NetworkReader reader)
    {
        CopyFrom(StaticDeserialize(reader));
    }
    public void Serialize(NetworkWriter writer)
    {
        StaticSerialize(writer, this);
    }

    [Exclude, ThreadStatic]
    static CerasSerializer _ceras;
    [ThreadStatic]
    static byte[] _lengthPrefixBuffer;
    [ThreadStatic]
    static byte[] _streamBuffer;

    static DefaultMessageBase()
    {
        var config = new SerializerConfig();
        CerasUnityFormatters.ApplyToConfig(config);
        _ceras = new CerasSerializer(config);
    }

    static object StaticDeserialize(NetworkReader reader)
    {
        var length = (int)ReadVarIntFromStream(reader);
        var recvBuffer = new byte[length];
        reader.ReadBytes(recvBuffer, length);
        return _ceras.Deserialize<object>(recvBuffer);
    }

    static void StaticSerialize(NetworkWriter writer, object obj)
    {
        if (_lengthPrefixBuffer == null) {
            _lengthPrefixBuffer = new byte[5];
        }

        var size = _ceras.Serialize(obj, ref _streamBuffer);
        var sizeBytesLength = 0;
        SerializerBinary.WriteUInt32(ref _lengthPrefixBuffer, ref sizeBytesLength, (uint)size);

        writer.WriteBytes(_lengthPrefixBuffer, 0, sizeBytesLength);
        writer.WriteBytes(_streamBuffer, 0, size);
    }

    static uint ReadVarIntFromStream(NetworkReader reader)
    {
        var shift = 0;
        ulong result = 0;
        const int bits = 32;

        while (true) {
            ulong byteValue = reader.ReadByte();

            var tmp = byteValue & 0x7f;
            result |= tmp << shift;

            if (shift > bits) {
                throw new Exception("Malformed VarInt");
            }

            if ((byteValue & 0x80) != 0x80) {
                return (uint)result;
            }

            shift += 7;
        }
    }
}