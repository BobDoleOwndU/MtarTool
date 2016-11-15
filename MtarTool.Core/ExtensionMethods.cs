using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtarTool.Core
{
    internal static class ExtensionMethods
    {
        //Method Skip skips ahead in the stream by a specified amount.
        internal static void Skip(this Stream stream, int count)
        {
            stream.Seek(count, SeekOrigin.Current);
        } //method Skip ends

        //Method Skip uses the BaseStream skip to skip ahead in the stream by a specified amount.
        internal static void Skip(this BinaryReader reader, int count)
        {
            reader.BaseStream.Skip(count);
        } //method Skip ends

        //Method WriteZeros creates an empty array of a specified amount and writes it using BinaryWriter.
        internal static void WriteZeros(this BinaryWriter writer, int count)
        {
            byte[] zeros = new byte[count];
            writer.Write(zeros);
        } //method WriteZeros ends
    } //class ExtensionMethods ends
}
