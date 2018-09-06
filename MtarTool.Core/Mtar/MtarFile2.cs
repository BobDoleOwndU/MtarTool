using MtarTool.Core.Common;
using MtarTool.Core.Utility;
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

        [XmlAttribute("Unknown0")]
        public ushort unknown0;

        [XmlAttribute("Unknown1")]
        public ushort unknown1;

        [XmlAttribute("Unknown2")]
        public ushort unknown2;

        [XmlAttribute("Unknown3")]
        public ushort unknown3;

        [XmlAttribute("Unknown4")]
        public ushort unknown4;

        [XmlAttribute("Unknown5")]
        public ushort unknown5;

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
            unknown0 = reader.ReadUInt16();
            unknown1 = reader.ReadUInt16();
            unknown2 = reader.ReadUInt16();
            unknown3 = reader.ReadUInt16();
            unknown4 = reader.ReadUInt16();
            unknown5 = reader.ReadUInt16();
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

            input.Position = trackOffset + mtarTrack.chunkOffset;

            if (mtarTrack.chunkOffset > 0)
            {
                mtarChunk.offset = (uint)input.Position;
                mtarChunk.GetSize(input);
            } //if
        } //method Read ends

        public override void Export(Stream output, string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(path).Replace("_mtar", ".mtar"));

            Directory.CreateDirectory(Path.GetDirectoryName(path + "1.trk"));

            mtarTrack.name = fileName;
            File.WriteAllBytes(path + mtarTrack.name + ".trk", mtarTrack.ReadData(output));

            if (mtarTrack.chunkOffset > 0)
            {
                mtarChunk.name = fileName;
                File.WriteAllBytes(path + mtarChunk.name + ".chnk", mtarChunk.ReadData(output));
            } //if

            files.Sort((x, y) => x.offset.CompareTo(y.offset));

            for (int i = 0; i < files.Count; i++)
            {
                if (numberNames)
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
            string inputPath = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path);

            Console.WriteLine(inputPath);
            uint offset;
            BinaryWriter writer = new BinaryWriter(output, Encoding.Default, true);

            fileCount = (uint)files.Count;

            writer.Write(signature);
            writer.Write(fileCount);
            writer.Write(unknown0);
            writer.Write(unknown1);
            writer.Write(unknown2);
            writer.Write(unknown3);
            writer.Write(unknown4);
            writer.Write(unknown5);
            writer.WriteZeros(0xC);

            for (int i = 0; i < files.Count; i++)
            {
                files[i].hash = NameResolver.GetHashFromName(files[i].name);
            } //for ends

            files.Sort((x, y) => x.hash.CompareTo(y.hash));

            for (int i = 0; i < files.Count; i++)
            {
                files[i].Write(output);
            } //for ends

            offset = (uint)output.Position;
            byte[] track = File.ReadAllBytes(inputPath + @"_mtar\" + mtarTrack.name + ".trk");
            writer.BaseStream.Position = 0x14;
            writer.Write(offset);
            writer.BaseStream.Position = offset;
            writer.Write(track);

            if (output.Position % 0x10 != 0)
                writer.WriteZeros(0x10 - (int)output.Position % 0x10);

            if (File.Exists(inputPath + @"_mtar\" + mtarChunk.name + ".chnk"))
            {
                byte[] chunk = File.ReadAllBytes(inputPath + @"_mtar\" + mtarChunk.name + ".chnk");
                writer.Write(chunk);
            } //if

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
