using MtarTool.Core.Common;
using MtarTool.Core.Mtar;
using MtarTool.Core.Utility;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MtarTool
{
    class Program
    {
        private static bool numberNames;
        private static XmlSerializer xmlSerializer = new XmlSerializer(typeof(ArchiveFile), new[] { typeof(MtarFile), typeof(MtarFile2) });

        static void Main(string[] args)
        {
            if(args.Length != 0)
            {
                string path = Path.GetFullPath(args[0]);

                if(args.Length > 1)
                {
                    if(args[1] == "-n")
                    {
                        numberNames = true;
                    } //if ends
                } //if ends

                if(Path.GetExtension(path) == ".mtar")
                {
                    if(GetMtarType(path) == 1)
                    {
                        ReadArchive<MtarFile>(path);
                    } //if ends
                    else
                    {
                        ReadArchive<MtarFile2>(path);
                    } //else ends
                } //if ends
                else if(Path.GetExtension(path) == ".xml")
                {
                    WriteArchive(path);
                } //else if ends
            } //if ends
        } //method Main ends

        static int GetMtarType(string path)
        {
            using (FileStream input = new FileStream(path, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

                input.Position = 0x10;
                uint type = reader.ReadUInt32();

                if(type > 0)
                {
                    return 2;
                } //if ends

                return 1;
            } //using ends
        } //method GetMtarType ends

        static void ReadArchive<T>(string path) where T : ArchiveFile, new()
        {
            string directory = Path.GetDirectoryName(path);
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path).Substring(1);
            string outputPath = directory + @"\" + nameWithoutExtension + "_" + extension + @"\";
            string xmlOutputPath = path + ".xml";

            using (FileStream input = new FileStream(path, FileMode.Open))
            using (FileStream xmlOutput = new FileStream(xmlOutputPath, FileMode.Create))
            {
                T file = new T();

                file.numberNames = numberNames;

                file.name = Path.GetFileName(path);
                file.Read(input);
                file.Export(input, outputPath);

                xmlSerializer.Serialize(xmlOutput, file);
            } //using ends

            NameResolver.WriteOutputList();
        } //method ReadArchive ends

        static void WriteArchive(string path)
        {
            string outputPath = path.Replace(".xml", "");

            using (FileStream xmlInput = new FileStream(path, FileMode.Open))
            using (FileStream output = new FileStream(outputPath, FileMode.Create))
            {
                ArchiveFile archiveFile = xmlSerializer.Deserialize(xmlInput) as ArchiveFile;

                archiveFile.Import(output, outputPath);
            } //using ends
        } //method WriteArchive ends
    } //class Program ends
} //namespace MtarTool ends
