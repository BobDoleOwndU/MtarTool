using MtarTool.Core.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MtarTool.Core.Mtar
{
    [XmlType("MtarFile2")]
    public class MtarFile2 : ArchiveFile
    {
        [XmlAttribute("Signature")]
        public uint signature;

        [XmlIgnore]
        public uint fileCount;

        [XmlAttribute("BoneGroups")]
        public ulong boneGroups;

        [XmlAttribute("BoneGroups2")]
        public uint boneGroups2;

        [XmlIgnore]
        public uint trackOffset;

        [XmlArray("Entries")]
        public List<MtarGaniFile2> files = new List<MtarGaniFile2>();

        [XmlArrayItem("Entries")]
        public MtarTrack2 mtarTrack;

        public override void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            signature = reader.ReadUInt32();
            fileCount = reader.ReadUInt32();
            boneGroups = reader.ReadUInt64();
            boneGroups2 = reader.ReadUInt32();
            trackOffset = reader.ReadUInt32();

            mtarTrack = new MtarTrack2(trackOffset);

            reader.Skip(8);

            for (int i = 0; i < fileCount; i++)
            {
                MtarGaniFile2 mtarGaniFile2 = new MtarGaniFile2();
                mtarGaniFile2.Read(input);
                files.Add(mtarGaniFile2);
            } //for ends


        } //method Read ends

        public override void Export(Stream output, string path)
        {
            for (int i = 0; i < files.Count; i++)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path + files[i].name));
                File.WriteAllBytes(path + files[i].name, files[i].ReadData(output));
            } //for ends
        } //method Export ends

        public override void Import(Stream output, string path)
        {
            string inputPath = Path.GetFileNameWithoutExtension(path);

            Console.WriteLine(inputPath);
            uint offset = (uint)output.Position;
            BinaryWriter writer = new BinaryWriter(output, Encoding.Default, true);

            fileCount = (uint)files.Count;

            writer.Write(signature);
            writer.Write(fileCount);
            writer.Write(boneGroups);
            writer.Write(boneGroups2);
            writer.WriteZeros(16);

            for (int i = 0; i < files.Count; i++)
            {
                files[i].Write(output);
            } //for ends

            for (int i = 0; i < files.Count; i++)
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
    } //class MtarFile2 ends
}
