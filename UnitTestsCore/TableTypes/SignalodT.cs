using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using OpenDentBusiness;
using DataConnectionBase;

namespace UnitTestsCore {
	public class SignalodT {
		///<summary>Deletes everything from the Signalod table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearSignalodTable() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			try {
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DELETE FROM signalod";
					DataCore.NonQ(command);
				}
			}
			catch {
				
			}
		}

		///<summary>Gets all entries from the Signalod table.</summary>
		public static List<Signalod> GetAllSignalods() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Signalod>>(MethodBase.GetCurrentMethod());
			}
			List<Signalod> listSignals=null;
			try {
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT * FROM signalod";
					listSignals=OpenDentBusiness.Crud.SignalodCrud.TableToList(DataCore.GetTable(command));
				}
			}
			catch {
				listSignals=new List<Signalod>();
			}
			return listSignals;
		}
	}
}
