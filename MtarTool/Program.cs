using System;
using System.IO;
using MtarTool.Core.Mtar;

namespace MtarTool
{
    class Program
    {
        static void Main(string[] args)
        {
            using (FileStream input = new FileStream(@"C:\Program Files (x86)\Steam\steamapps\common\MGS_TPP\master\chunk0_dat\Assets\tpp\pack\player\motion\player2_facial_snake_fpk\Assets\tpp\motion\mtar\player2\TppPlayer2Facial.mtar", FileMode.Open))
            {
                MtarFile mtarFile = new MtarFile();

                mtarFile.Read(input);
                mtarFile.ListFiles();
            } //using ends
        } //function Main ends

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
