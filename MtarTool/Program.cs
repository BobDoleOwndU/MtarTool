using MtarTool.Core.Mtar;
using System.IO;
using System.Xml.Serialization;

namespace MtarTool
{
    class Program
    {
        private static XmlSerializer xmlSerializer = new XmlSerializer(typeof(MtarFile));

        static void Main(string[] args)
        {
            if(args.Length != 0)
            {
                string path = args[0];

                if(Path.GetExtension(path) == ".mtar")
                {
                    ReadArchive(path);
                } //if ends
                else if(Path.GetExtension(path) == ".xml")
                {
                    WriteArchive(path);
                } //else if ends
            } //if ends
        } //method Main ends

        static void ReadArchive(string path)
        {
            string outputPath = path.Replace(".", "_") + @"\";
            string xmlOutputPath = path + ".xml";

            using (FileStream input = new FileStream(path, FileMode.Open))
            using (FileStream xmlOutput = new FileStream(xmlOutputPath, FileMode.Create))
            {
                MtarFile mtarFile = new MtarFile();

                mtarFile.name = Path.GetFileName(path);
                mtarFile.Read(input);
                mtarFile.Export(input, outputPath);

                xmlSerializer.Serialize(xmlOutput, mtarFile);
            } //using ends
        } //method ReadArchive ends

        static void WriteArchive(string path)
        {
            string outputPath = path.Replace(".xml", "");

            using (FileStream xmlInput = new FileStream(path, FileMode.Open))
            using (FileStream output = new FileStream(outputPath, FileMode.Create))
            {
                MtarFile mtarFile = xmlSerializer.Deserialize(xmlInput) as MtarFile;

                mtarFile.Import(output, outputPath);
            } //using ends
        } //method WriteArchive ends
    } //class Program ends
} //namespace MtarTool ends
