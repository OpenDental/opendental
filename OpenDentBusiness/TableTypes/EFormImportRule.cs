using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Each row is a rule that controls how form import logic works. It currently also applies to sheets. Clearly, if there's no row for a specific field, then a global rule handles it. Since there are 4 different situations, we might need 4 global rules just to start. But since users might forget to add them, if any of the 4 is missing, then that situation behaves the old way: action is Review.</summary>
	[Serializable]
	public class EFormImportRule:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EFormImportRuleNum;
		///<summary>If empty, this is a global rule. if the field name matches a DbLink value, then this applies to the Db field. Otherwise, it must be a Non-db field. It will first try to match a reportable name and then a label.</summary>
		public string FieldName;
		///<summary>Enum:EnumEFormImportSituation </summary>
		public EnumEFormImportSituation Situation;
		///<summary>Enum:EnumEFormImportAction </summary>
		public EnumEFormImportAction Action;

		///<summary>During setup, we need to keep track of this because we don't take action on it until the user clicks Save.</summary>
		[CrudColumn(IsNotDbColumn=true)]
		public bool IsDeleted;

		public EFormImportRule Copy(){
			return (EFormImportRule)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum EnumEFormImportSituation{
		///<summary>0-Original blank and entered new value</summary>
		New,
		///<summary>1-</summary>
		Changed,
		///<summary>2-</summary>
		Deleted,
		///<summary>3-Setup UI explains the fields that this can apply to.</summary>
		Invalid,
	}

	///<summary></summary>
	public enum EnumEFormImportAction{
		///<summary>0-</summary>
		Overwrite,
		///<summary>1-</summary>
		Review,
		///<summary>2-</summary>
		Ignore,
		///<summary>3-Setup UI explains the fixes that can be made.</summary>
		Fix
	}

}
