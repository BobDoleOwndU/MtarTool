using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MtarTool.Core.Mtar
{
    [XmlType("MtarFile")]
    public class MtarFile
    {
        [XmlAttribute("Name")]
        public string name;

        [XmlAttribute("Signature")]
        public uint signature;

        [XmlIgnore]
        public uint fileCount;

        [XmlAttribute("Unknown1")]
        public uint unknown1;

        [XmlAttribute("Unknown2")]
        public uint unknown2;

        [XmlArray("Entries")]
        public List<MtarGaniFile> files = new List<MtarGaniFile>();

        public void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            signature = reader.ReadUInt32();
            fileCount = reader.ReadUInt32();
            unknown1 = reader.ReadUInt32();
            unknown2 = reader.ReadUInt32();
            reader.Skip(16);

            for(int i = 0; i < fileCount; i++)
            {
                MtarGaniFile mtarGaniFile = new MtarGaniFile();
                mtarGaniFile.Read(input);
                files.Add(mtarGaniFile);
            } //for ends
        } //method Read ends

        public void Export(Stream output, string path)
        {
            for(int i = 0; i < files.Count; i++)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path + files[i].name));
                File.WriteAllBytes(path + files[i].name, files[i].ReadData(output));
            } //for ends
        } //method Export ends
    } //class MtarEntry ends
}
