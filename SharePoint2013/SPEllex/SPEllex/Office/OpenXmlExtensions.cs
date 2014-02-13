using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SPEllex.Office
{
    public static class OpenXmlExtensions
    {
        public static XDocument GetXDocument(this XmlDocument document)
        {
            var document2 = new XDocument();
            using (XmlWriter writer = document2.CreateWriter())
            {
                document.WriteTo(writer);
            }
            var declaration = document.ChildNodes.OfType<XmlDeclaration>().FirstOrDefault<XmlDeclaration>();
            if (declaration != null)
            {
                document2.Declaration = new XDeclaration(declaration.Version, declaration.Encoding,
                    declaration.Standalone);
            }
            return document2;
        }

        public static XElement GetXElement(this XmlNode node)
        {
            var document = new XDocument();
            using (XmlWriter writer = document.CreateWriter())
            {
                node.WriteTo(writer);
            }
            return document.Root;
        }

        public static string ToStringAlignAttributes(this XContainer xContainer)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
                NewLineOnAttributes = true
            };
            var output = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(output, settings))
            {
                xContainer.WriteTo(writer);
            }
            return output.ToString();
        }
    }
}