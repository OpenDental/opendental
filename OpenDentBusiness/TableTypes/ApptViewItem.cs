using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;
using System.Text;
using System.ComponentModel;
using CodeBase;
using System.Linq;

namespace OpenDentBusiness {
	///<summary>Each item is attached to a row in the apptview table.  Each item specifies ONE of: OpNum, ProvNum, ElementDesc, ApptFieldDefNum, or PatFieldDefNum.  The other 4 will be 0 or "".</summary>
	[Serializable]
	public class ApptViewItem:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ApptViewItemNum;//
		///<summary>FK to apptview.</summary>
		public long ApptViewNum;
		///<summary>FK to operatory.OperatoryNum.</summary>
		public long OpNum;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>Must be one of the hard coded strings picked from the available list.</summary>
		public string ElementDesc;
		///<summary>If this is a row Element, then this is the 0-based order within its area.  For example, UR starts over with 0 ordering.</summary>
		public byte ElementOrder;
		///<summary>If this is an element, then this is the color.</summary>
		[XmlIgnore]
		public Color ElementColor;
		///<summary>Enum:ApptViewAlignment If this is an element, then this is the alignment of the element within the appointment.</summary>
		public ApptViewAlignment ElementAlignment;
		///<summary>FK to apptfielddef.ApptFieldDefNum.  If this is an element, and the element is an appt field, then this tells us which one.</summary>
		public long ApptFieldDefNum;
		///<summary>FK to patfielddef.PatFieldDefNum.  If this is an element, and the element is an pat field, then this tells us which one.</summary>
		public long PatFieldDefNum;

		public ApptViewItem(){

		}

		///<summary>this constructor is just used in GetForCurView when no view selected.</summary>
		public ApptViewItem(string elementDesc,byte elementOrder,Color elementColor) {
			ApptViewItemNum=0;
			ApptViewNum=0;
			OpNum=0;
			ProvNum=0;
			ElementDesc=elementDesc;
			ElementOrder=elementOrder;
			ElementColor=elementColor;
			ElementAlignment=ApptViewAlignment.Main;
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ElementColor",typeof(int))]
		public int ElementColorXml {
			get {
				return ElementColor.ToArgb();
			}
			set {
				ElementColor = Color.FromArgb(value);
			}
		}

		public ApptViewItem Clone() {
			return (ApptViewItem)this.MemberwiseClone();
		}

		public EnumApptViewElement GetElement(){
			if(ElementDesc==""){
				return EnumApptViewElement.None;
			}
			try{
				return (EnumApptViewElement)Enum.Parse(typeof(EnumApptViewElement),ElementDesc);
			}
			catch{ }
			for(int i=0;i<Enum.GetValues(typeof(EnumApptViewElement)).Length;i++){
				if(((EnumApptViewElement)i).GetDescription()==ElementDesc){
					return (EnumApptViewElement)i;
				}
			}
			return EnumApptViewElement.None;
		}
	}

	///<summary>Main, UR, or LR</summary>
	public enum ApptViewAlignment {
		///<summary>0</summary>
		Main,
		///<summary>1</summary>
		UR,
		///<summary>2</summary>
		LR
	}

	///<summary>The string versions of these enum items are used in the db as ApptViewItem.ElementDesc.  New items can be added, but existing ones can't be changed.  Use enum.GetDescription and ApptViewItem.GetElement for going back and forth to the database.</summary>
	public enum EnumApptViewElement{
		///<summary>This represents empty string.</summary>
		[Description("")]
		None,
		Address,
		AddrNote,
		Age,
		ASAP,
		[Description("ASAP[A]")]
		ASAP_A,
		AssistantAbbr,
		Birthdate,
		ChartNumAndName,
		ChartNumber,
		ConfirmedColor,
		CreditType,
		DiscountPlan,
		EstPatientPortion,
		Guardians,
		[Description("HasDiscount[D]")]
		HasDiscount_D,
		[Description("HasIns[I]")]
		HasIns_I,
		HmPhone,
		[Description("InsToSend[!]")]
		InsToSend_excl,
		Insurance,
		InsuranceColor,
		[Description("IsLate[L]")]
		IsLate_L,
		Lab,
		LateColor,
		[Description("MedOrPremed[+]")]
		MedOrPremed_plus,
		MedUrgNote,
		NetProduction,
		Note,
		PatientName,
		PatientNameF,
		PatientNamePref,
		PatNum,
		PatNumAndName,
		PremedFlag,
		Procs,
		ProcsColored,
		Production,
		[Description("Prophy/PerioPastDue[P]")]
		ProphyPerioPastDue_P,
		Provider,
		[Description("RecallPastDue[R]")]
		RecallPastDue_R,
		TimeAskedToArrive,
		[Description("VerifyIns[V]")]
		VerifyIns_V,
		WirelessPhone,
		WkPhone
	}
	
}
