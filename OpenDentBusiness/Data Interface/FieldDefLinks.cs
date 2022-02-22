using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class FieldDefLinks{
		///<summary></summary>
		public static List<FieldDefLink> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<FieldDefLink>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM fielddeflink";
			return Crud.FieldDefLinkCrud.SelectMany(command);
		}

		///<summary>Gets a list of FieldDefLinks for a specified location.</summary>
		public static List<FieldDefLink> GetForLocation(FieldLocations fieldLocation) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<FieldDefLink>>(MethodBase.GetCurrentMethod(),fieldLocation);
			}
			string command="SELECT * FROM fielddeflink WHERE FieldLocation="+POut.Int((int)fieldLocation);
			return Crud.FieldDefLinkCrud.SelectMany(command);
		}

		public static bool Sync(List<FieldDefLink> listNew) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),listNew);
			}
			string command="SELECT * FROM fielddeflink";
			List<FieldDefLink> listDB=Crud.FieldDefLinkCrud.SelectMany(command);
			return Crud.FieldDefLinkCrud.Sync(listNew,listDB);
		}

		///<summary>Deletes all fieldDefLink rows that are associated to the given fieldDefNum and fieldDefType.</summary>
		public static void DeleteForFieldDefNum(long fieldDefNum,FieldDefTypes fieldDefType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),fieldDefNum,fieldDefType);
				return;
			}
			if(fieldDefNum==0) {
					return;
			}
			//Only delete records of the correct fieldDefType (Pat vs Appt)
			Db.NonQ("DELETE FROM fieldDefLink WHERE FieldDefNum="+POut.Long(fieldDefNum)+" AND FieldDefType="+POut.Int((int)fieldDefType));
		}
	}
}