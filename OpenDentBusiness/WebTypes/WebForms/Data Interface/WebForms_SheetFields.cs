using DataConnectionBase;
using OpenDentBusiness.WebTypes.WebForms.Crud;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness.WebTypes.WebForms.Data_Interface {
	public class WebForms_SheetFields {

		///<summary></summary>
		public static void DeleteForSheetIDs(List<long> listSheetIds) {
			if(listSheetIds==null || listSheetIds.Count==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSheetIds);
				return;
			}
			DataAction.Run(() => {
				string command="DELETE FROM webforms_sheetfield "
					+"WHERE SheetID IN ("+string.Join(",",listSheetIds.Select(x => POut.Long(x)))+")";
				DataCore.NonQ(command);
			},ConnectionNames.WebForms);
		}

		///<summary></summary>
		public static List<WebForms_SheetField> GetSheetFieldsForSheetIds(List<long> listSheetIds) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebForms_SheetField>>(MethodBase.GetCurrentMethod(),listSheetIds);
			}
			if(listSheetIds==null || listSheetIds.Count==0) {
				return new List<WebForms_SheetField>();
			}
			return DataAction.GetT(() => {
				//Need to order by SheetFieldID so that the signature data is in the same order as when it was encrypted.
				string command="SELECT * FROM webforms_sheetfield WHERE SheetID IN ("+string.Join(",",listSheetIds)+") ORDER BY SheetID, SheetFieldID";
				return WebForms_SheetFieldCrud.SelectMany(command);
			},ConnectionNames.WebForms);
		}

	}
}
