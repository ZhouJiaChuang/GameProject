using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
public static class XmlEditorUtil
{
    public static XmlNode LoadXml(string xmlPath)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(xmlPath);
        return doc;
    }
    public static XmlNode ReadXml(XmlNode node, string nodePath)
    {
        XmlNode n = node.SelectSingleNode(nodePath);
        return n;
    }

    public static XmlNodeList ReadXmlList(XmlNode node, string nodePath)
    {
        XmlNodeList list = node.SelectNodes(nodePath);
        return list;
    }

    public static void WriteXml(string xmlPath, string nodePath, string innerText)
    {
        WriteXmlTo(xmlPath, xmlPath, nodePath, innerText);
    }

    public static void WriteXmlTo(string xmlPath, string outXmlPath, string nodePath, string innerText)
    {
        //XmlDocument doc = new XmlDocument();
        //doc.Load(xmlPath);

        //XmlNode node = doc.SelectSingleNode(nodePath);
        //if(node != null)
        //{
        //    node.InnerText = innerText;
        //}
        //else
        //{
        //    node = doc.CreateNode()
        //}
        //doc.Save(outXmlPath);
    }
}
