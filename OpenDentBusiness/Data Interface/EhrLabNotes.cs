using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrLabNotes {
		///<summary></summary>
		public static List<EhrLabNote> GetForLab(long ehrLabNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabNote>>(MethodBase.GetCurrentMethod(),ehrLabNum);
			}
			string command="SELECT * FROM ehrlabnote WHERE EhrLabNum = "+POut.Long(ehrLabNum)+" AND EhrLabResultNum=0";
			return Crud.EhrLabNoteCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<EhrLabNote> GetForLabResult(long ehrLabResultNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabNote>>(MethodBase.GetCurrentMethod(),ehrLabResultNum);
			}
			string command="SELECT * FROM ehrlabnote WHERE EhrLabResultNum="+POut.Long(ehrLabResultNum);
			return Crud.EhrLabNoteCrud.SelectMany(command);
		}

		///<summary>Deletes notes for lab results too.</summary>
		public static void DeleteForLab(long ehrLabNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabNum);
				return;
			}
			string command="DELETE FROM ehrlabnote WHERE EhrLabNum = "+POut.Long(ehrLabNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(EhrLabNote ehrLabNote) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				ehrLabNote.EhrLabNoteNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrLabNote);
				return ehrLabNote.EhrLabNoteNum;
			}
			return Crud.EhrLabNoteCrud.Insert(ehrLabNote);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<EhrLabNote> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EhrLabNote>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrlabnote WHERE PatNum = "+POut.Long(patNum);
			return Crud.EhrLabNoteCrud.SelectMany(command);
		}

		///<summary>Gets one EhrLabNote from the db.</summary>
		public static EhrLabNote GetOne(long ehrLabNoteNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<EhrLabNote>(MethodBase.GetCurrentMethod(),ehrLabNoteNum);
			}
			return Crud.EhrLabNoteCrud.SelectOne(ehrLabNoteNum);
		}

		///<summary></summary>
		public static void Update(EhrLabNote ehrLabNote){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabNote);
				return;
			}
			Crud.EhrLabNoteCrud.Update(ehrLabNote);
		}

		///<summary></summary>
		public static void Delete(long ehrLabNoteNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrLabNoteNum);
				return;
			}
			string command= "DELETE FROM ehrlabnote WHERE EhrLabNoteNum = "+POut.Long(ehrLabNoteNum);
			Db.NonQ(command);
		}
		*/
	}
}