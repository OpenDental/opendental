using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	///<summary>Used in the accounting section in chart of accounts.  Not related to patient accounts in any way.</summary>
	[Serializable()]
	public class Account:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AccountNum;
		///<summary>.</summary>
		public string Description;
		///<summary>Enum:AccountType Asset, Liability, Equity,Revenue, Expense</summary>
		public AccountType AcctType;
		///<summary>For asset accounts, this would be the bank account number for deposit slips.</summary>
		public string BankNumber;
		///<summary>Set to true to not normally view this account in the list.</summary>
		public bool Inactive;
		///<summary>.</summary>
		[XmlIgnore]
		public Color AccountColor;

		///<summary></summary>
		public Account Clone() {
			return (Account)this.MemberwiseClone();
		}


		///<summary>Used only for serialization purposes</summary>
		[XmlElement("AccountColor",typeof(int))]
		public int AccountColorXml {
			get {
				return AccountColor.ToArgb();
			}
			set {
				AccountColor=Color.FromArgb(value);
			}
		}
	}

	
}




