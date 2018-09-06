using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MtarTool.Core.Mtar
{
    [XmlType("Track", Namespace = "Mtar")]
    public class MtarTrack2
    {
        [XmlAttribute("FilePath")]
        public string name;

        [XmlIgnore]
        public uint offset;

        [XmlIgnore]
        public uint signature;

        [XmlIgnore]
        public uint length;

        [XmlIgnore]
        public int chunkOffset;

        public void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            signature = reader.ReadUInt32();
            length = reader.ReadUInt32();
            chunkOffset = reader.ReadInt32();
            reader.Skip(4);
        } //method Read ends

        public byte[] ReadData(Stream input)
        {
            input.Position = offset;
            byte[] data = new byte[length + 0x10];
            input.Read(data, 0, (int)length + 0x10);

            return data;
        } //method ReadData ends
    } //class MtarTrack ends
}
