using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtarTool
{
    class Program
    {
        static void Main(string[] args)
        {
            using (FileStream input = new FileStream(@"C:\Program Files (x86)\Steam\steamapps\common\MGS_TPP\master\chunk0_dat\Assets\tpp\pack\player\motion\player2_facial_snake_fpk\Assets\tpp\motion\mtar\player2\TppPlayer2Facial.mtar", FileMode.Open))
            {
                Read(input);
            } //using ends
        } //function Main ends

        static void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            //line 1
            uint signature = reader.ReadUInt32();
            uint fileCount = reader.ReadUInt32();
            uint unknown1 = reader.ReadUInt32();
            uint unknown2 = reader.ReadUInt32();

            //line 2
            byte[] padding1 = reader.ReadBytes(16);

            for(int i = 0; i < fileCount; i++)
            {
                ReadEntry(input);
            } //if ends
        } //function Read ends

        static void ReadEntry(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            string fileName = getFileName(reader.ReadBytes(8));
            uint offset = reader.ReadUInt32();
            uint fileSize = reader.ReadUInt32();

            Console.WriteLine(fileName);
        } //function ReadEntry ends

        static string getFileName(byte[] arr)
        {
            byte[] nameByte = arr;
            Array.Reverse(nameByte);

            switch (nameByte[1])
            {
                case 0x50:
                    nameByte[1] = 0x00;
                    break;
                case 0x51:
                    nameByte[1] = 0x01;
                    break;
                case 0x52:
                    nameByte[1] = 0x02;
                    break;
                case 0x53:
                    nameByte[1] = 0x03;
                    break;
            } //switch ends

            string nameString = BitConverter.ToString(nameByte).Replace("-", "");

            if (nameString.Substring(2, 2) == "00")
            {
                nameString = nameString.Substring(4, 12);
                
                if (nameString.Substring(0, 1) == "0")
                {
                    nameString = nameString.Substring(1, 11);
                } //if ends
            } //if ends
            else if(nameString.Substring(2, 1) == "0")
            {
                nameString = nameString.Substring(3, 13);
            } //else if ends

            return nameString;
        } //function getFileName ends
    } //class Program ends
} //namespace MtarTool ends
