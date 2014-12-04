using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;

using System.Threading.Tasks;

namespace ComNet_SV
{
    public class XML
    {
        public static void load()
        {

            if ( !File.Exists("statistics.xml") )
            {
                createXML(1);
            }

            if (!File.Exists("comments.xml"))
            {
                createXML(2);
            }
        }

        private static List<string> getTree(string file, string tree)
        {
            List<string> _child = new List<string>(); 
            XDocument _file = XDocument.Load(file);
            var children = _file.Descendants(tree);
            foreach (var _children in children)
            {
                _child.Add(_children.Value);
            }

            return _child; 
        }

        private static void createXML(int lane)
        {
            XmlTextWriter _file = new XmlTextWriter("statistics.xml", System.Text.Encoding.UTF8);

            _file.WriteStartDocument(true);
            _file.Formatting = Formatting.Indented;
            _file.Indentation = 2;
                switch (lane)
                {
                    case 1:
                        _file.WriteStartElement("Stats");
                        break;
                    case 2:
                        _file.WriteStartElement("Comments");
                        break;
                }
            _file.WriteEndElement();
            _file.WriteEndDocument();
            _file.Close();
        }

        private static void createNode(XmlTextWriter _file, string node, string value)
        {
            _file.WriteStartElement("node");
            _file.WriteString( value );
        }
    }
}
