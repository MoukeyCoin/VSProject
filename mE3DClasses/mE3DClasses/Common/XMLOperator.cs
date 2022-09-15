using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Aveva.Core.PMLNet;

namespace Aveva.Core.mE3DClasses.Common
{
    [PMLNetCallable()]
    public class XMLOperator
    {
        [PMLNetCallable()]
        public XMLOperator()
        {

        }

        [PMLNetCallable()]
        public void Assign(XMLOperator that)
        {
            //No state
        }
        //readNodeHierachy是pml的节点路径 用空格隔开
        //如'all node1 node2'表示all节点下的node1节点下的所有node2节点
        //返回二维hashtable,一维hashtable表示有多少个节点，
        //二维hashtable下是该节点的readAttributes对应的属性的值
        [PMLNetCallable()]       
        public Hashtable ReadxmlFile(string xmlfilepath,string readNodeHierachy,Hashtable readAttributes)
        {
            Hashtable returnArray = new Hashtable();
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlfilepath);
                //readNodeHierachy存储xml的node路径，找到最后一个节点
                string[] nodedirectory = readNodeHierachy.Split();
                XmlNode xmlnode = xmlDocument.SelectSingleNode(nodedirectory[0]);
                for (int i = 1; i < nodedirectory.Length - 1; i++)
                {
                    xmlnode = xmlnode.SelectSingleNode(nodedirectory[i]);
                }
                XmlNodeList nodeList = xmlnode.SelectNodes(nodedirectory.Last());
               
                foreach (XmlNode node in nodeList)
                {
                    Hashtable nodeattributes = new Hashtable();
                    string attributevalue = "";
                    int i = 1;
                    List<object> hashtableIndex = new List<object>();
                    //hashtable本来的顺序是反的。用list做升序排列
                    foreach (object key in readAttributes.Keys)
                    {
                        hashtableIndex.Add(key);
                    }
                    hashtableIndex = hashtableIndex.OrderBy(a=>a.ToString()).ToList();
                    foreach (object index in hashtableIndex)
                    {
                        //在attribute和innertext都找该属性
                        try
                        {
                            attributevalue = (node as XmlElement).GetAttribute(readAttributes[index].ToString());
                        }
                        catch (Exception ex)
                        {
                        }
                        if (string.IsNullOrEmpty(attributevalue))
                        {
                            try
                            {
                                attributevalue = node.SelectSingleNode(readAttributes[index].ToString().ToString()).InnerText;
                            }
                            catch (Exception ex)
                            { }
                        }
                        nodeattributes.Add(i, attributevalue);
                        i++;
                    }
                    returnArray.Add(returnArray.Count + 1, nodeattributes);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("所给的xml节点不存在.");
                MessageBox.Show("所给的xml节点不存在.");
                MessageBox.Show(ex.ToString());
            }
            return returnArray;
        }
        [PMLNetCallable()]
        public void WritexmlFile(string xmlfilepath, string addNodeHierachy, string addAttributeName,string addAttributeValue)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                if (File.Exists(xmlfilepath))
                {
                    xmlDocument.Load(xmlfilepath);
                }              
                //readNodeHierachy存储xml的node路径，找到最后一个节点
                string[] nodedirectory = addNodeHierachy.Split();
                XmlNode xmlnode = null;
                try
                {
                    xmlnode = xmlDocument.SelectSingleNode(nodedirectory[0]);
                }
                catch(Exception ex)
                {
                    XmlElement xmlRootElement = xmlDocument.CreateElement(nodedirectory[0]);
                    xmlnode = xmlDocument.AppendChild(xmlRootElement);
                }
                
                if (xmlnode == null)
                {
                    XmlElement xmlRootElement = xmlDocument.CreateElement(nodedirectory[0]);
                    xmlnode = xmlDocument.AppendChild(xmlRootElement);
                }
                XmlNode prevNode = xmlnode;
                for (int i = 1; i < nodedirectory.Length; i++)
                {
                    try
                    {
                        xmlnode = xmlnode.SelectSingleNode(nodedirectory[i]);
                    }
                    catch (Exception ex)
                    {
                        XmlElement xmlNewElement = xmlDocument.CreateElement(nodedirectory[i]);
                        xmlnode = xmlnode.AppendChild(xmlNewElement);
                    }
                    if (xmlnode == null)
                    {
                        XmlElement xmlNewElement = xmlDocument.CreateElement(nodedirectory[i]);
                        xmlnode = prevNode.AppendChild(xmlNewElement);
                        prevNode = xmlnode;
                    }

                }
                //没有属性名就添加到innertext中
                if (string.IsNullOrEmpty(addAttributeName))
                {
                    xmlnode.InnerText = addAttributeValue;
                }
                else
                {
                    //如果不存在该属性，添加
                    string attvalue = (xmlnode as XmlElement).GetAttribute(addAttributeName);
                    if (string.IsNullOrEmpty(attvalue))
                    {
                        XmlNode xmlAttribute = xmlDocument.CreateNode(XmlNodeType.Attribute, addAttributeName, null);
                        xmlAttribute.Value = addAttributeValue;
                        xmlnode.Attributes.SetNamedItem(xmlAttribute);
                        
                    }
                    else
                    {
                        XmlNode xmlAttribute = xmlnode.Attributes.GetNamedItem(addAttributeName);
                        xmlAttribute.Value = addAttributeValue;
                    }
                }
                /*XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = new UTF8Encoding(false);
                xmlWriterSettings.Indent = true;
                XmlWriter xmlWriter = XmlWriter.Create(xmlfilepath, xmlWriterSettings);
               
                xmlDocument.Save(xmlWriter);
                xmlWriter.Close();*/
                //直接保存打不开
                xmlDocument.Save(xmlfilepath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); 
            }
            
        }

    }
}
