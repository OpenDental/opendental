using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	[Serializable]
	public class SplitCollection:ICollection<PaySplit>,IXmlSerializable {
		private Dictionary<string,PaySplit> _dictPaySplits=new Dictionary<string,PaySplit>();

		public int Count {
			get {
				return _dictPaySplits.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}

		private string GetUniqueKeyFromPaySplit(PaySplit paySplit) {
			if(paySplit.SplitNum > 0) {
				return paySplit.SplitNum.ToString();
			}
			if(paySplit.TagOD is string && ((string)paySplit.TagOD)!="") {
				return (string)paySplit.TagOD;
			}
			throw new ODException("Invalid PaySplit with no SplitNum or invalid TagOD");
		}

		public void Add(PaySplit paySplit) {
			string uniqueKey=GetUniqueKeyFromPaySplit(paySplit);
			if(_dictPaySplits.ContainsKey(uniqueKey)) {
				return;
			}
			_dictPaySplits.Add(uniqueKey,paySplit);
		}

		public void AddRange(List<PaySplit> listPaySplits) {
			foreach(PaySplit split in listPaySplits) {
				Add(split);
			}
		}

		public void Clear() {
			_dictPaySplits.Clear();
		}

		public bool Contains(PaySplit paySplit) {
			return _dictPaySplits.ContainsKey(GetUniqueKeyFromPaySplit(paySplit));
		}

		public void CopyTo(PaySplit[] array,int arrayIndex) {
			for(int i=arrayIndex;i<_dictPaySplits.Values.Count;i++) {
				array[i]=_dictPaySplits.Values.ToList()[i];
			}
		}

		public bool Remove(PaySplit paySplit) {
			return _dictPaySplits.Remove(GetUniqueKeyFromPaySplit(paySplit));
		}

		public IEnumerator<PaySplit> GetEnumerator() {
			return _dictPaySplits.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<PaySplit>)_dictPaySplits.Values).GetEnumerator();
		}

		///<summary>Returns null.  Required when extending IXmlSerializable.</summary>
		public XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader) {
			XmlSerializer serializer=new XmlSerializer(typeof(List<PaySplit>));
			bool wasEmpty=reader.IsEmptyElement;
			reader.Read();
			if(wasEmpty) {
				return;
			}
			Clear();
			AddRange((List<PaySplit>)serializer.Deserialize(reader));
			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer) {
			XmlSerializer serializer=new XmlSerializer(typeof(List<PaySplit>));
			serializer.Serialize(writer,_dictPaySplits.Values.ToList());
		}
	}

	
}
