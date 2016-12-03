using System.IO;
using System.Xml.Serialization;

namespace MtarTool.Core.Common
{
    [XmlType]
    public abstract class ArchiveFile
    {
        [XmlAttribute("Name")]
        public string name;

        public bool numberNames = false;

        public abstract void Read(Stream input);

        public abstract void Export(Stream output, string path);

        public abstract void Import(Stream output, string path);
    } //class ArchiveFile ends
}
