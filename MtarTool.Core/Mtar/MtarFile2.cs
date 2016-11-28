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

        [XmlElement]
        public MtarTrack2 mtarTrack = new MtarTrack2();

        [XmlElement]
        public MtarChunk2 mtarChunk = new MtarChunk2();

        public override void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            signature = reader.ReadUInt32();
            fileCount = reader.ReadUInt32();
            boneGroups = reader.ReadUInt64();
            boneGroups2 = reader.ReadUInt32();
            trackOffset = reader.ReadUInt32();

            mtarTrack.offset = trackOffset;

            reader.Skip(8);

            for (int i = 0; i < fileCount; i++)
            {
                MtarGaniFile2 mtarGaniFile2 = new MtarGaniFile2();
                mtarGaniFile2.Read(input);
                files.Add(mtarGaniFile2);
            } //for ends

            mtarTrack.Read(input);
            input.Position = mtarTrack.offset + mtarTrack.size;
            mtarChunk.offset = (uint)input.Position;
            mtarChunk.GetSize(input);
        } //method Read ends

        public override void Export(Stream output, string path)
        {
            mtarTrack.name = "track.trk";
            mtarChunk.name = "chunk.chnk";

            Directory.CreateDirectory(Path.GetDirectoryName(path + "1.trk"));
            File.WriteAllBytes(path + mtarTrack.name, mtarTrack.ReadData(output));
            File.WriteAllBytes(path + mtarChunk.name, mtarChunk.ReadData(output));

            for (int i = 0; i < files.Count; i++)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path + files[i].name + ".gani"));
                File.WriteAllBytes(path + files[i].name + ".gani", files[i].ReadData(output));

                if(files[i].exChunkSize != 0x0)
                {
                    File.WriteAllBytes(path + files[i].name + ".exchnk", files[i].ReadExChunkData(output));
                } //if ends

                if(files[i].endChunkOffset != 0x0)
                {
                    File.WriteAllBytes(path + files[i].name + ".enchnk", files[i].ReadEndChunkData(output));
                } //if ends
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
            writer.WriteZeros(12);

            for (int i = 0; i < files.Count; i++)
            {
                files[i].Write(output);
            } //for ends

            offset = (uint)output.Position;
            byte[] track = File.ReadAllBytes(inputPath + @"_mtar\track.trk");
            output.Position = 0x14;
            writer.Write(offset);
            output.Position = offset;
            writer.Write(track);
            byte[] chunk = File.ReadAllBytes(inputPath + @"_mtar\chunk.chnk");
            writer.Write(chunk);

            for (int i = 0; i < files.Count; i++)
            {
                byte[] file = File.ReadAllBytes(inputPath + @"_mtar\" + files[i].name + ".gani");
                byte[] exFile;
                offset = (uint)output.Position;
                output.Position = (0x20 + ((0x20 * i) + 0x8));
                writer.Write(offset);
                writer.Write((ushort)(file.Length / 0x10));

                if(File.Exists(inputPath + @"_mtar\" + files[i].name + ".exchnk"))
                {
                    writer.Write((ushort)(file.Length / 0x10));
                    exFile = File.ReadAllBytes(inputPath + @"_mtar\" + files[i].name + ".exchnk");
                    writer.Write((ushort)(exFile.Length / 0x10));
                } //if ends
                else
                {
                    exFile = new byte[0];
                } //else ends

                output.Position = offset;
                writer.Write(file);
                
                if(exFile.Length > 0)
                {
                    writer.Write(exFile);
                } //if ends
            } //for ends

            for (int i = 0; i < files.Count; i++)
            {
                if(File.Exists(inputPath + @"_mtar\" + files[i].name + ".enchnk"))
                {
                    byte[] file = File.ReadAllBytes(inputPath + @"_mtar\" + files[i].name + ".enchnk");

                    offset = (uint)output.Position;
                    output.Position = (0x30 + ((0x20 * i) + 0x8));
                    writer.Write(offset);
                    output.Position = offset;
                    writer.Write(file);
                } //if ends
            } //for ends
        } //method Import ends
    } //class MtarFile2 ends
}
