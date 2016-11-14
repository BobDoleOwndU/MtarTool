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
        struct file
        {
            string name;
            int offset;
            int length;
        } //struct file ends

        static int fileNameOffset = 0x20 * 0x2;
        static int fileOffset = fileNameOffset;

        List<file> fileList = new List<file>(0);

        static void Main(string[] args)
        {
            GetFileInfo();
        } //function Main ends

        static void GetFileInfo()
        {
            string path = @"C:\Program Files (x86)\Steam\steamapps\common\MGS_TPP\master\chunk0_dat\Assets\tpp\pack\player\motion\player2_facial_snake_fpk\Assets\tpp\motion\mtar\player2\TppPlayer2Facial.mtar";
            bool run = true;
            
            var file = File.ReadAllBytes(path);
            string hex = BitConverter.ToString(file).Replace("-", String.Empty);

            while(run)
            {
                file File1;

                if(hex.Substring((fileOffset + 0x7) * 0x2, 0x1 * 0x2) != "FC")
                {
                    Console.WriteLine(hex.Substring((fileOffset + 0x7) * 0x2, 0x1 * 0x2));
                    Console.WriteLine("End of file section");
                    Console.WriteLine(fileOffset);
                    run = false;
                } //if ends
                else
                {
                    Console.WriteLine(hex.Substring(fileOffset, 0x7 * 0x2));
                    fileOffset = fileOffset + 0x10 * 0x2;
                }
            } //while ends
        } //function GetFileNames ends
    } //class Program ends
} //namespace MtarTool ends
