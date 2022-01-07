using System;
using System.Collections;

namespace OpenDentBusiness{

	///<summary>Attached to procbuttons.  These tell the program what to do when a user clicks on a button.  There are two types: proccodes or autocodes.</summary>
	[Serializable]
	public class ProcButtonItem:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ProcButtonItemNum;
		///<summary>FK to procbutton.ProcButtonNum.</summary>
		public long ProcButtonNum;
		///<summary>Do not use.</summary>
		public string OldCode;
		///<summary>FK to autocode.AutoCodeNum.  0 if this is a procedure code.</summary>
		public long AutoCodeNum;
		///<summary>FK to procedurecode.CodeNum.  0 if this is an autocode.</summary>
		public long CodeNum;
		///<summary>Unusual ItemOrder column. Set implicitly based on the order procedures were added to the procedure button. This should prevent "random"
		///ordered procedures on buttons with multiple procedures.</summary>
		public long ItemOrder;

		///<summary></summary>
		public ProcButtonItem Copy() {
			ProcButtonItem p=new ProcButtonItem();
			p.ProcButtonItemNum=ProcButtonItemNum;
			p.ProcButtonNum=ProcButtonNum;
			//p.OldCode=OldCode;
			p.AutoCodeNum=AutoCodeNum;
			p.CodeNum=CodeNum;
			return p;
		}


	}

	




}










