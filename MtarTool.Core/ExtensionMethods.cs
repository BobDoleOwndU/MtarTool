using System.IO;
using System.Linq;

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

        internal static void AlignWrite(this Stream output, int alignment, byte data)
        {
            long alignmentRequired = output.Position % alignment;
            if (alignmentRequired > 0)
            {
                byte[] alignmentBytes = Enumerable.Repeat(data, (int)(alignment - alignmentRequired)).ToArray();
                output.Write(alignmentBytes, 0, alignmentBytes.Length);
            }
        } //method AlignWrite ends
    } //class ExtensionMethods ends
}
