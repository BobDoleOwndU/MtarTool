using MtarTool.Core.Utility;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MtarTool.Core.Mtar
{
    [XmlType("Gani", Namespace = "Mtar")]
    public class MtarGaniFile2
    {
        [XmlIgnore]
        public ulong hash;

        [XmlAttribute("FilePath")]
        public string name;

        [XmlIgnore]
        public uint offset;

        [XmlIgnore]
        public int size;

        [XmlIgnore]
        public int size2;

        [XmlIgnore]
        public int exChunkSize;

        [XmlIgnore]
        public uint endChunkOffset;

        public void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            hash = reader.ReadUInt64();
            name = NameResolver.TryFindName(NameResolver.GetHashFromULong(hash)) + ".gani";
            offset = reader.ReadUInt32();
            size = reader.ReadInt16();
            size2 = reader.ReadInt16();

            if(size == size2)
            {
                size *= 0x10;
            } //if ends

            exChunkSize = reader.ReadInt16() * 0x10;
            reader.Skip(6);
            endChunkOffset = reader.ReadUInt32();
            reader.Skip(4);
        } //method Read ends

        public void Write(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output, Encoding.Default, true);

            hash = NameResolver.GetHashFromName(name);

            writer.Write(hash);
            writer.WriteZeros(8);
        } //method Read ends

        public byte[] ReadData(Stream input)
        {
            input.Position = offset;
            byte[] data = new byte[size];
            input.Read(data, 0, size);

            return data;
        } //method ReadData ends
    } //class MtarGaniFile2 ends
}
