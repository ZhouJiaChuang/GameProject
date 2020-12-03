using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

/// <summary>
/// 可用于.net的xml序列化的字典结构
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class XMLSerializableDic<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
    XmlSchema IXmlSerializable.GetSchema()
    {
        return null;
    }

    void IXmlSerializable.ReadXml(XmlReader reader)
    {
        Dictionary<Type, XmlSerializer> serializersForDifferentType = new Dictionary<Type, XmlSerializer>();
        //读取键值对数量
        int dicCount = int.Parse(reader["Count"]);
        if (dicCount > 0)
        {
            //开始读取内容
            reader.ReadStartElement();
            for (int i = 0; i < dicCount; i++)
            {
                //读取本键值对的属性,键类型,值类型,以及值是否为null
                string keyTypeStr = reader.GetAttribute("KeyType");
                string valueTypeStr = null;
                bool valueIsNull = reader.GetAttribute("ValueIsNull") == "true";
                if (valueIsNull == false)
                {
                    valueTypeStr = reader.GetAttribute("ValueType");
                }
                //读取键值对内部
                reader.ReadStartElement("KeyValuePair");
                //读取键内容
                reader.ReadStartElement("Key");
                Type keyType = Type.GetType(keyTypeStr);
                if (serializersForDifferentType.ContainsKey(keyType) == false)
                {
                    serializersForDifferentType[keyType] = new XmlSerializer(keyType);
                }
                //反序列化键内容为键对象
                TKey keyObj = (TKey)serializersForDifferentType[keyType].Deserialize(reader);
                //键内容读取完毕
                reader.ReadEndElement();
                //读取值内容
                if (valueIsNull)
                {
                    //若值内容为空,则使用类型的默认量填值
                    this[keyObj] = default(TValue);
                }
                else
                {
                    //值内容不为空,则读取值内容
                    reader.ReadStartElement("Value");
                    Type valueType = Type.GetType(valueTypeStr);
                    if (serializersForDifferentType.ContainsKey(valueType) == false)
                    {
                        serializersForDifferentType[valueType] = new XmlSerializer(valueType);
                    }
                    //反序列化值内容为值对象
                    TValue valueObj = (TValue)serializersForDifferentType[valueType].Deserialize(reader);
                    this[keyObj] = valueObj;
                    //值内容读取完毕
                    reader.ReadEndElement();
                }
                //键值对内容读取完毕
                reader.ReadEndElement();
            }
            //循环内容读取完毕
            reader.ReadEndElement();
        }
    }

    void IXmlSerializable.WriteXml(XmlWriter writer)
    {
        Dictionary<Type, XmlSerializer> serializersForDifferentType = new Dictionary<Type, XmlSerializer>();
        //写入字典类型
        Type typeTemp = GetType();
        writer.WriteStartAttribute("Dictionary");
        writer.WriteValue(typeTemp.ToString());
        writer.WriteEndAttribute();
        //写入字典大小
        writer.WriteStartAttribute("Count");
        writer.WriteValue(Count);
        writer.WriteEndAttribute();
        //遍历字典,将字典键值写入
        foreach (var item in this)
        {
            bool isValueNull = item.Value == null || item.Value.Equals(default(TValue));
            Type keyType = null, valueType = null;
            writer.WriteStartElement("KeyValuePair");
            //写入键的类型
            writer.WriteStartAttribute("KeyType");
            keyType = item.Key.GetType();
            writer.WriteValue(keyType.ToString());
            writer.WriteEndAttribute();
            //写入值的类型是否为空
            writer.WriteStartAttribute("ValueIsNull");
            writer.WriteValue(isValueNull);
            writer.WriteEndAttribute();
            if (isValueNull == false)
            {
                //值类型不为空时,写入值的类型
                writer.WriteStartAttribute("ValueType");
                valueType = item.Value.GetType();
                writer.WriteValue(valueType.ToString());
                writer.WriteEndAttribute();
            }
            //写入键的内容
            writer.WriteStartElement("Key");
            if (serializersForDifferentType.ContainsKey(keyType) == false)
            {
                serializersForDifferentType[keyType] = new XmlSerializer(keyType);
            }
            serializersForDifferentType[keyType].Serialize(writer, item.Key);
            writer.WriteEndElement();
            //写入值的内容
            if (isValueNull == false)
            {
                //若值不为空,则写入值的内容
                writer.WriteStartElement("Value");
                if (serializersForDifferentType.ContainsKey(valueType) == false)
                {
                    serializersForDifferentType[valueType] = new XmlSerializer(valueType);
                }
                serializersForDifferentType[valueType].Serialize(writer, item.Value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
