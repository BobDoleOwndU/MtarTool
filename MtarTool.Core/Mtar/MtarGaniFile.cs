using MtarTool.Core.Utility;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MtarTool.Core.Mtar
{
    [XmlType("Entry", Namespace = "Mtar")]
    public class MtarGaniFile
    {
        [XmlIgnore]
        public ulong hash;

        [XmlAttribute("FilePath")]
        public string name;

        [XmlIgnore]
        public uint offset;

        [XmlIgnore]
        public int size;

        public void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            hash = reader.ReadUInt64();
            name = NameResolver.TryFindName(NameResolver.GetHashFromULong(hash)) + ".gani";
            offset = reader.ReadUInt32();
            size = reader.ReadInt32();
        } //method Read ends

        public void Write(Stream output)
        {
            BinaryWriter writer = new BinaryWriter(output, Encoding.Default, true);

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
    } //class MtarGaniFile ends
}
