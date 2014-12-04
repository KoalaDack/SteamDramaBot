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
        private static XmlTextWriter _file;
        public void load()
        {

            if ( !File.Exists("stats.xml") )
            {
                Debug.PrintError("stats.xml does not exist, creating!");
                createXML("stats.xml", "stats");
            }

            if (!File.Exists("comments.xml"))
            {
                Debug.PrintError("comments.xml does not exist, creating!");
                createXML("comments.xml", "stats");
            }
            Debug.PrintError("Please note that the XML feature is currently not finished.");
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

        public static void addXML(string file, string node, string value)
        {
            // TODO: Finish later ( taking way to fucking long )
        }
        private static XmlTextWriter saveXML(string file)
        {
           XmlTextWriter writer = new XmlTextWriter( file, System.Text.Encoding.UTF8);
           return writer;
        }
        private static void createXML(string file, string element)
        {
                _file = new XmlTextWriter(file, System.Text.Encoding.UTF8);
                _file.WriteStartDocument(true);
                _file.Formatting = Formatting.Indented;
                _file.Indentation = 2;
                _file.WriteStartElement(element, element);
                _file.WriteEndElement();
                _file.WriteEndDocument();
                _file.Close();
                //addXML("comments.xml", "comments", "test");
        }
        private static void createNode(XmlTextWriter _file, string node, string value)
        {
            _file.WriteStartElement("node");
            _file.WriteString( value );
        }
    }
}
