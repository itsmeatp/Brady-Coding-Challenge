using BradyChallenge.Model;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BradyChallenge.Utilities
{
    public class XmlOperations : IXmlOperations
    {
        /// <summary>
        /// Convert from XML to Object
        /// </summary>
        /// <param name="Xml"> xml string</param>
        /// <param name="ObjType"> Type of the object to which the xml should be converted</param>
        /// <returns></returns>
        public object FromXml(string Xml, Type ObjType)
        {
            XmlSerializer serializer = new XmlSerializer(ObjType);
            StringReader stringReader = new StringReader(Xml);
            XmlTextReader xmlReader = new XmlTextReader(stringReader);
            object obj = serializer.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();
            return obj;
        }

        /// <summary>
        /// Convert from object to XML
        /// </summary>
        /// <param name="Obj">The object from which XML needs to be generated</param>
        /// <param name="ObjType">Type of object to be converted to XML</param>
        /// <returns></returns>
        public string ToXml(object Obj, Type ObjType)
        {
            XmlSerializer ser = new XmlSerializer(ObjType);
            MemoryStream memStream = new MemoryStream();
            XmlTextWriter xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
            xmlWriter.Namespaces = true;
            ser.Serialize(xmlWriter, Obj, GetNamespaces());
            xmlWriter.Close();
            memStream.Close();
            string xml = Encoding.UTF8.GetString(memStream.GetBuffer());
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            return xml;
        }
        /// <summary>
        /// Extract input data from the file in the filePath specified
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns> xml data as string</returns>
        public string ExtractInputData(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            string xmlcontents = doc.InnerXml;
            return xmlcontents;
        }
        
        /// <summary>
		/// Returns the set of included namespaces for the serializer.
		/// </summary>		
		public static XmlSerializerNamespaces GetNamespaces()
        {
            XmlSerializerNamespaces ns;
            ns = new XmlSerializerNamespaces();
            ns.Add("xsd", "http://www.w3.org/2001/XMLSchema");
            ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            return ns;
        }
    }
}
