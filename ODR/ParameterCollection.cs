using System;
using System.Collections;

namespace ODR
{
	///<summary>Strongly typed collection of type Parameter.</summary>
	public class ParameterCollection:CollectionBase{

		///<summary>Returns the Parameter with the given index.</summary>
		public Parameter this[int index]{
      get{
				return((Parameter)List[index]);
      }
      set{
				List[index]=value;
      }
		}

		///<summary>Returns the Parameter with the given name.</summary>
		public Parameter this[string name]{
			get{
				foreach(Parameter p in List){
					if(p.Name==name)
						return p;
				}
				return null;
      }
		}

		///<summary></summary>
		public int Add(Parameter value){
			return(List.Add(value));
		}

		///<summary></summary>
		public int IndexOf(Parameter value){
			return(List.IndexOf(value));
		}

		///<summary></summary>
		public void Insert(int index,Parameter value){
			List.Insert(index,value);
		}

		
		//public int GetIndexOfType(SectionType sectType){
			
		//	return -1;
		//}


	}

}
























