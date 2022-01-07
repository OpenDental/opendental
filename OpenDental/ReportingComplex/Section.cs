using System;

namespace OpenDental.ReportingComplex {
	///<summary>Every ReportObject in an ODReport must be attached to a Section.</summary>
	public class Section{
		///<summary></summary>
		private int _height;
		///<summary>Width is usually the entire page unless set differently here.</summary>
		private int _width;
		///<summary>Specifies which kind, like ReportHeader, or GroupFooter.</summary>
		private AreaSectionType _sectionType;

		///<summary></summary>
		public Section(AreaSectionType type,int height){
			_sectionType=type;
			_height=height;
		}

#region Properties
		///<summary></summary>
		public int Height{
			get{
				return _height;
			}
			set{
				_height=value;
			}
		}
		///<summary></summary>
		public int Width{
			get{
				return _width;
			}
			set{
				_width=value;
			}
		}
		///<summary></summary>
		public AreaSectionType SectionType{
			get{
				return _sectionType;
			}
			set{
				_sectionType=value;
			}
		}
#endregion


	}


	///<summary>The type of section is used in the Section class.  Only ONE of each type is allowed except for the GroupHeader and GroupFooter which are optional and can have one pair for each group.  The order of the sections is locked and user cannot change.</summary>
	public enum AreaSectionType{
		///<summary>0- None</summary>
		None,
		///<summary>1- Printed at the top of the report.</summary>
		ReportHeader,
		///<summary>2- Printed at the top of each page.</summary>
		PageHeader,
		///<summary>3- Title of a specific group</summary>
		GroupTitle,
		///<summary>4- Will print at the top of a specific group.</summary>
		GroupHeader,
		///<summary>5- This is the data of the report and represents one row of data.  This section gets printed once for each record in the datatable.</summary>
		Detail,
		///<summary>6- Contains a buffer and/or a total of a column</summary>
		GroupFooter,
		///<summary>7- Prints at the bottom of each page, including after the reportFooter</summary>
		PageFooter,
		///<summary>8- Prints at the bottom of the report, but before the page footer for the last page.</summary>
		ReportFooter,
		///<summary>9- Query Section, contains groups of queries.</summary>
		Query
	}



}












