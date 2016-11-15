using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MtarTool.Core.Mtar
{
    public class MtarFile
    {
        uint signature;
        uint signature2;
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

        public void ListFiles()
        {
            for(int i = 0; i < files.Count; i++)
            {
                string nameString = BitConverter.ToString(files[i].name).Replace("-", "");
                Console.WriteLine(nameString);
            } //for ends
        } //method ListFiles ends
    } //class MtarEntry ends
}
