using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenDental.UI {
	#region EnumImageNodeType
	///<summary>Category,Document,Mount</summary>
	public enum EnumImageNodeType {
		///<summary>This isn't used for any items in list, but can be useful for comparisons.  For example, this can indicate that nothing is selected.</summary>
		None,
		///<summary>Uses Def.</summary>
		Category,
		///<summary>Uses DocNum, Document, and ImgType.</summary>
		Document,
		///<summary>Uses MountNum and Mount.</summary>
		Mount
	}
	#endregion EnumImageNodeType
	
	///<summary>Either DocNum or MountNum will be 0.</summary>
	public class DragEventArgsOD{
		public long DocNum;
		public long MountNum;
		///<summary>DefNum of the new Category.</summary>
		public long DefNumNew;
	}

	public class DragDropImportEventArgs{
		///<summary>DefNum of the new Category.</summary>
		public long DefNumNew;
		public List<string> ListFileNames;
	}

	///<summary>Allows comparison of items based on nodetype and key.  </summary>
	[Serializable]
	public class NodeTypeAndKey:ISerializable {
		public EnumImageNodeType NodeType;
		public long PriKey;

		private NodeTypeAndKey() {
		}

		///<summary></summary>
		public NodeTypeAndKey(EnumImageNodeType nodeType,long priKey){
			NodeType=nodeType;
			PriKey=priKey;
		}

		///<summary>The special constructor is used to deserialize values.</summary>
		public NodeTypeAndKey(SerializationInfo info, StreamingContext context){
			NodeType=(EnumImageNodeType)info.GetValue("NodeType", typeof(EnumImageNodeType));
			PriKey=(long)info.GetValue("PriKey",typeof(long));
		}

		///<summary>The method is called on serialization for placing on clipboard.</summary>
		public void GetObjectData(SerializationInfo info, StreamingContext context){
			info.AddValue("NodeType", NodeType, typeof(EnumImageNodeType));
			info.AddValue("PriKey", PriKey, typeof(long));
		}

		///<summary>Tests whether the two nodeTypeAndKeys match.</summary>
		public bool IsMatching(NodeTypeAndKey nodeTypeAndKey){
			if(nodeTypeAndKey is null){
				return false;
			}
			if(NodeType!=nodeTypeAndKey.NodeType){
				return false;
			}
			if(PriKey==nodeTypeAndKey.PriKey){
				return true;
			}
			return false;
		}
			

		public NodeTypeAndKey Copy(){
			NodeTypeAndKey nodeTypeAndKey=new NodeTypeAndKey();
			nodeTypeAndKey.NodeType=NodeType;
			nodeTypeAndKey.PriKey=PriKey;
			return nodeTypeAndKey;
		}
	}
}
