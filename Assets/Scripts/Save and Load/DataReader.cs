using System.IO;
using UnityEngine;

namespace Game.IO
{
    public class DataReader
    {
        private readonly BinaryReader reader;

        public DataReader(BinaryReader reader)
        {
            this.reader = reader;
        }

        public bool ReadBool()
        {
            return reader.ReadBoolean();
        }

        public float ReadFloat()
        {
            return reader.ReadSingle();
        }

        public int ReadInt()
        {
            return reader.ReadInt32();
        }

        public Quaternion ReadQuaternion()
        {
            Quaternion value;
            value.x = reader.ReadSingle();
            value.y = reader.ReadSingle();
            value.z = reader.ReadSingle();
            value.w = reader.ReadSingle();
            return value;
        }

        public Vector3 ReadVector3()
        {
            Vector3 value;
            value.x = reader.ReadSingle();
            value.y = reader.ReadSingle();
            value.z = reader.ReadSingle();
            return value;
        }

        public Color ReadColor()
        {
            Color value;
            value.r = reader.ReadSingle();
            value.g = reader.ReadSingle();
            value.b = reader.ReadSingle();
            value.a = reader.ReadSingle();
            return value;
        }
    }
}
