using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness{
	///<summary>A room full of cubicles.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class MapAreaContainer:TableBase{
		///<summary></summary>
		[CrudColumn(IsPriKey=true)]
		public long MapAreaContainerNum;
		///<summary></summary>
		public int FloorWidthFeet;
		///<summary></summary>
		public int FloorHeightFeet;
		///<summary>Deprecated</summary>
		public int PixelsPerFoot;
		///<summary>Deprecated</summary>
		public bool ShowGrid;
		///<summary>Deprecated</summary>
		public bool ShowOutline;
		///<summary></summary>
		public string Description;
		///<summary>FK to site.SiteNum.  Represents the site or location which this room belongs to.  Used with escalation and conf rooms. Can be zero.</summary>
		public long SiteNum;
		///<summary>0 based</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public int ItemOrder;
	}
}
