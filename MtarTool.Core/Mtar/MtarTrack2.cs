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
        public uint unknown;

        [XmlIgnore]
        public int size;

        public void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            signature = reader.ReadUInt32();
            unknown = reader.ReadUInt32();
            size = reader.ReadInt32();
            reader.Skip(4);
        } //method Read ends

        public byte[] ReadData(Stream input)
        {
            input.Position = offset;
            byte[] data = new byte[size];
            input.Read(data, 0, size);

            return data;
        } //method ReadData ends
    } //class MtarTrack ends
}
