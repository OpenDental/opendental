using System;
using System.Collections;

namespace OpenDental.ReportingComplex
{
	///<summary>Strongly typed collection of type ParameterDef.</summary>
	public class ParameterFieldCollection:CollectionBase{

		///<summary>Returns the ParameterField with the given index.</summary>
		public ParameterField this[int index]{
      get{
				return((ParameterField)List[index]);
      }
      set{
				List[index]=value;
      }
		}

		///<summary>Returns the ParameterDefinition with the given name.</summary>
		public ParameterField this[string name]{
			get{
				foreach(ParameterField pf in List){
					if(pf.Name==name)
						return pf;
				}
				return null;
      }
		}

		///<summary></summary>
		public int Add(ParameterField value){
			return(List.Add(value));
		}

		///<summary></summary>
		public int IndexOf(ParameterField value){
			return(List.IndexOf(value));
		}

		///<summary></summary>
		public void Insert(int index,ParameterField value){
			List.Insert(index,value);
		}

		
		//public int GetIndexOfType(SectionType sectType){
			
		//	return -1;
		//}


	}

}
