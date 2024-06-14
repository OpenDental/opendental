using DataConnectionBase;
using OpenDentBusiness.WebTypes.WebForms.Crud;
using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness.WebTypes.WebForms {
	public class WebForms_SheetFieldDefs {

		///<summary></summary>
		public static void DeleteForWebSheetDefID(long WebSheetDefID) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),WebSheetDefID);
				return;
			}
			DataAction.Run(() => {
				string command="DELETE FROM webforms_sheetfielddef "
					+"WHERE WebSheetDefID ="+POut.Long(WebSheetDefID);
				DataCore.NonQ(command);
			},ConnectionNames.WebForms);
		}

		///<summary></summary>
		public static void InsertMany(List<WebForms_SheetFieldDef> listWebForms_SheetFieldDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listWebForms_SheetFieldDef);
				return;
			}
			DataAction.Run(() => WebForms_SheetFieldDefCrud.InsertMany(listWebForms_SheetFieldDef),ConnectionNames.WebForms);
		}

	}
}
