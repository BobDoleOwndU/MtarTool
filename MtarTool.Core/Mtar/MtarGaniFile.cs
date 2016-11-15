using System.IO;
using System.Text;

namespace MtarTool.Core.Mtar
{
    class MtarGaniFile
    {
        public byte[] name;
        uint offset;
        int size;

        public void Read(Stream input)
        {
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            name = reader.ReadBytes(8);
            offset = reader.ReadUInt32();
            size = reader.ReadInt32();
        } //method Read ends

        public byte[] ReadData(Stream input)
        {
            input.Position = offset;
            byte[] data = new byte[size];
            input.Read(data, 0, size);

            return data;
        } //method ReadData ends
    } //class MtarGaniFile ends
}
