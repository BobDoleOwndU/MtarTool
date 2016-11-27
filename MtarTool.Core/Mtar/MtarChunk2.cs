using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MtarTool.Core.Mtar
{
    [XmlType("Chunk", Namespace = "Mtar")]
    public class MtarChunk2
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

        public byte[] ReadData(Stream input)
        {
            input.Position = offset;
            byte[] data = new byte[size];
            input.Read(data, 0, size);

            return data;
        } //method ReadData ends

        public void GetSize(Stream input)
        {
            uint lineValue = 0x0;
            bool run = true;
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            reader.Skip(32);

            while(run)
            {
                lineValue = reader.ReadUInt32();

                if(lineValue <= 0xFF && lineValue > 0x0)
                {
                    run = false;
                } //if ends
                else
                {
                    reader.Skip(12);
                } //else ends
            } //while ends

            Console.WriteLine();

            size = (int)((input.Position - 0x4) - offset);
        } //function GetLength ends
    } //class MtarChunk2 ends
}
