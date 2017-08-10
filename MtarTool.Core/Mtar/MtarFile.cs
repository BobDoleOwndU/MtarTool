using MtarTool.Core.Common;
using MtarTool.Core.Utility;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MtarTool.Core.Mtar
{
    [XmlType("MtarFile")]
    public class MtarFile : ArchiveFile
    {
        [XmlAttribute("Signature")]
        public uint signature;

        [XmlIgnore]
        public uint fileCount;

        [XmlAttribute("BoneGroups")]
        public ulong boneGroups;

        [XmlAttribute("BoneGroups2")]
        public ulong boneGroups2;

        [XmlArray("Entries")]
        public List<MtarGaniFile> files = new List<MtarGaniFile>();

        public override void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            signature = reader.ReadUInt32();
            fileCount = reader.ReadUInt32();
            boneGroups = reader.ReadUInt64();
            boneGroups2 = reader.ReadUInt64();
            reader.Skip(8);

            for(int i = 0; i < fileCount; i++)
            {
                MtarGaniFile mtarGaniFile = new MtarGaniFile();
                mtarGaniFile.Read(input);
                files.Add(mtarGaniFile);
            } //for ends
        } //method Read ends

        public override void Export(Stream output, string path)
        {
            files.Sort((x, y) => x.offset.CompareTo(y.offset));

            for (int i = 0; i < files.Count; i++)
            {
                if(numberNames)
                {
                    string ganiPath = Path.GetDirectoryName(files[i].name).Replace('\\', '/');
                    string ganiName = Path.GetFileName(files[i].name);
                    

                    if (ganiPath != "")
                    {
                        ganiPath += "/";
                    } //if ends

                    ganiPath += i.ToString("0000") + "_" + ganiName;
                    files[i].name = ganiPath;
                } //if ends

                Directory.CreateDirectory(Path.GetDirectoryName(path + files[i].name));
                File.WriteAllBytes(path + files[i].name, files[i].ReadData(output));
            } //for ends
        } //method Export ends

        public override void Import(Stream output, string path)
        {
            string inputPath = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path);

            uint offset = (uint)output.Position;
            BinaryWriter writer = new BinaryWriter(output, Encoding.Default, true);

            fileCount = (uint)files.Count;

            writer.Write(signature);
            writer.Write(fileCount);
            writer.Write(boneGroups);
            writer.Write(boneGroups2);
            writer.WriteZeros(8);

            for (int i = 0; i < files.Count; i++)
            {
                files[i].hash = NameResolver.GetHashFromName(files[i].name);
            } //for ends

            files.Sort((x, y) => x.hash.CompareTo(y.hash));

            for (int i = 0; i < files.Count; i++)
            {
                files[i].Write(output);
            } //for ends

            for(int i = 0; i < files.Count; i++)
            {
                byte[] file = File.ReadAllBytes(inputPath + @"_mtar\" + files[i].name);
                offset = (uint)writer.BaseStream.Position;
                writer.BaseStream.Position = (0x20 + ((0x10 * i) + 0x8));
                writer.Write(offset);
                writer.Write(file.Length);
                writer.BaseStream.Position = offset;
                writer.Write(file);
                output.AlignWrite(16, 0x00);
            } //for ends
        } //method Import ends
    } //class MtarEntry ends
}
