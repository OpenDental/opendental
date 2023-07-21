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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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

		///<summary>Updates the SigDateTime on a given Signalod to match the given DateTime. Unable to set the SigDateTime to anything but DateTime.Now in the CRUD Layer.</summary>
		public static void SetSigDateTimeForSignal(DateTime sigDateTime,long signalNum) {
			string command="UPDATE signalod SET SigDateTime = "+POut.DateT(sigDateTime)
				+" WHERE SignalNum = "+POut.Long(signalNum);
			DataCore.NonQ(command);
    }
	}
}
