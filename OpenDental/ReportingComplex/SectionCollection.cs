using System;
using System.Collections;

namespace OpenDental.ReportingComplex
{
	///<summary>Strongly typed collection of Sections.</summary>
	public class SectionCollection:CollectionBase{

		/*
		///<summary>Returns the Section with the given index.</summary>
		public Section this[int index]{
      get{
				return((Section)List[index]);
      }
      //set{
			//	List[index]=value;
      //}
		}*/

		///<summary>Returns the Section with the given type.</summary>
		public Section this[AreaSectionType kind]{
      get{
				foreach(Section section in List){
					if(section.SectionType==kind)
						return section;
				}
				return null;
      }
      //set{
			//	List[index]=value;
      //}
		}

		///<summary>Adds the specified section to the collection, but not to the end.  Instead, it inserts it exactly where it belongs based on the type.  The order cannot be changed by the user. Returns the index at which the section has been added, or -1 if not allowed because it already exists.</summary>
		public int Add(Section value){
			//if(value.Kind==AreaSectionKind.GroupHeader);
			//if(value.Kind==AreaSectionKind.GroupFooter);
			if(List.Count==0){
				List.Add(value);
				return 0;
			}
			for(int i=0;i<List.Count;i++){
				//we are trying to find the item to insert before
				if(i==List.Count-1){//if last item in list, then only option is to add to end of list
					List.Insert(i,value);
					return i;
				}
				if((int)value.SectionType < (int)((Section)List[i]).SectionType){
					List.Insert(i,value);
					return i;
				}
			}
			return -1;
		}

		///<summary></summary>
		public int IndexOf(Section value){
			return(List.IndexOf(value));
		}

		///<summary></summary>
		public int IndexOf(AreaSectionType kind) {
			foreach(Section section in List){
				if(section.SectionType==kind)
					return IndexOf(section);
			}
			return -1;
		}

		///<summary></summary>
		public bool Contains(AreaSectionType kind) {
			foreach(Section section in List){
				if(section.SectionType==kind)
					return true;
			}
			return false;
		}

		//<summary></summary>
		//public void Insert(int index,Section value){
		//	List.Insert(index,value);
		//}

		/*
		///<summary>Returns the first Section index of the given kind. Since only one of each kind is allowed, this will reliably return the section of interest.</summary>
		public int GetIndexOfKind(AreaSectionKind kind){
			foreach(Section section in List){
				if(section.Kind==kind)
					return IndexOf(section);
			}
			return -1;
		}*/

		/*
		///<summary>Returns the first Section of the given kind. Since only one of each kind is allowed, this will reliably return the section of interest.</summary>
		public Section GetOfKind(AreaSectionKind kind){
			foreach(Section section in List){
				if(section.Kind==kind)
					return section;
			}
			return null;
		}*/


	}

}
