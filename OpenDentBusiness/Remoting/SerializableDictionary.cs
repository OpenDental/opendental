using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>Because Dictionaries are not serializable, this class should be used whenever a dictionary needs to be serialized.</summary>
	[XmlRoot("SerializableDictionary")]
	public class SerializableDictionary<TKey, TValue>:Dictionary<TKey,TValue>, IXmlSerializable {
		///<summary>Returns null.  Required when extending IXmlSerializable.</summary>
		public System.Xml.Schema.XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader) {
			XmlSerializer keySerializer=new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerializer=new XmlSerializer(typeof(TValue));
			bool wasEmpty=reader.IsEmptyElement;
			reader.Read();
			if(wasEmpty) {
				return;
			}
			while(reader.NodeType != System.Xml.XmlNodeType.EndElement) {
				reader.ReadStartElement("I");//Item
				reader.ReadStartElement("K");//Key
				TKey key=(TKey)keySerializer.Deserialize(reader);
				if(key!=null) {
					key=(TKey)XmlConverter.XmlUnescapeRecursion(typeof(TKey),key);
				}
				reader.ReadEndElement();
				reader.ReadStartElement("V");//Value
				TValue value=(TValue)valueSerializer.Deserialize(reader);
				if(value!=null) {
					value=(TValue)XmlConverter.XmlUnescapeRecursion(typeof(TValue),value);
				}
				this.Add(key,value);
				reader.ReadEndElement();
				reader.ReadEndElement();
				reader.MoveToContent();
			}
			reader.ReadEndElement();
		}

		public void WriteXml(System.Xml.XmlWriter writer) {
			XmlSerializer keySerializer=new XmlSerializer(typeof(TKey));
			XmlSerializer valueSerializer=new XmlSerializer(typeof(TValue));
			foreach(TKey key in this.Keys) {
				writer.WriteStartElement("I");//Item
				writer.WriteStartElement("K");//Key
				keySerializer.Serialize(writer,(TKey)XmlConverter.XmlEscapeRecursion(typeof(TKey),key));
				writer.WriteEndElement();
				writer.WriteStartElement("V");//Value
				valueSerializer.Serialize(writer,(TValue)XmlConverter.XmlEscapeRecursion(typeof(TValue),this[key]));
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}
	}

	public static class SerializableDictionaryExtensions {
		///<summary>Returns a SerializableDictionary similar to how ToDictionary() works.</summary>
		public static SerializableDictionary<TKey,TValue> ToSerializableDictionary<TSource,TKey,TValue>(this IEnumerable<TSource> source,
			Func<TSource,TKey> keySelector,Func<TSource,TValue> elementSelector) 
		{
			SerializableDictionary<TKey,TValue> dict=new SerializableDictionary<TKey,TValue>();
			foreach(TSource element in source) {
				dict.Add(keySelector(element),elementSelector(element));
			}
			return dict;
		}
	}
}