using System;
using System.Collections;

namespace OpenDental.ReportingComplex
{
	///<summary>Contains the ReportObject objects for every report object in the report.</summary>
	public class ReportObjectCollection:CollectionBase {

		///<summary>Returns the ReportObject with the given index.</summary>
		public ReportObject this[int index]{
      get{
				return((ReportObject)List[index]);
      }
      set{
				List[index]=value;
      }
		}

		///<summary>Returns the ReportObject with the given name.</summary>
		public ReportObject this[string name]{
			get{
				foreach(ReportObject ro in List){
					if(ro.Name==name)
						return ro;
				}
				return null;
      }
		}

		///<summary></summary>
		public int Add(ReportObject value){
			return(List.Add(value));
		}

		///<summary></summary>
		public int IndexOf(ReportObject value){
			return(List.IndexOf(value));
		}

		///<summary></summary>
		public void Insert(int index,ReportObject value){
			List.Insert(index,value);
		}
	}

}
