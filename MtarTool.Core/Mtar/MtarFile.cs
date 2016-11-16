using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MtarTool.Core.Utility;

namespace MtarTool.Core.Mtar
{
    public class MtarFile
    {
        uint signature;
        uint fileCount;
        uint unknown1;
        uint unknown2;

        List<MtarGaniFile> files = new List<MtarGaniFile>();

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
                Directory.CreateDirectory(Path.GetDirectoryName(path + NameResolver.TryFindName(NameResolver.GetHashFromULong(files[i].name)) + ".gani"));
                File.WriteAllBytes(path + NameResolver.GetHashFromULong(files[i].name) + ".gani", files[i].ReadData(output));
            } //for ends
        } //method Export ends
    } //class MtarEntry ends
}
