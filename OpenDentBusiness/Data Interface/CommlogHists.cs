using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class CommlogHists {
		#region Methods - Get
		///<summary></summary>
		public static List<CommlogHist> GetAllForCommlog(long commlogNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CommlogHist>>(MethodBase.GetCurrentMethod(),commlogNum);
			}
			string command= "SELECT * FROM commloghist WHERE CommlogNum = "+POut.Long(commlogNum);
			return Crud.CommlogHistCrud.SelectMany(command);
		}
		
		///<summary>Gets one CommlogHist from the db.</summary>
		public static CommlogHist GetOne(long commlogHistNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<CommlogHist>(MethodBase.GetCurrentMethod(),commlogHistNum);
			}
			return Crud.CommlogHistCrud.SelectOne(commlogHistNum);
		}
		#endregion Methods - Get
		#region Methods - Modify
		///<summary></summary>
		public static long Insert(CommlogHist commlogHist) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				commlogHist.CommlogHistNum=Meth.GetLong(MethodBase.GetCurrentMethod(),commlogHist);
				return commlogHist.CommlogHistNum;
			}
			return Crud.CommlogHistCrud.Insert(commlogHist);
		}

		///<summary></summary>
		public static void Delete(long commlogHistNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),commlogHistNum);
				return;
			}
			Crud.CommlogHistCrud.Delete(commlogHistNum);
		}

		public static void DeleteForCommlog(long commlogNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),commlogNum);
				return;
			}
			string command="DELETE FROM commloghist WHERE CommlogNum = "+POut.Long(commlogNum);
			Db.NonQ(command);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		///<summary>Returns a CommlogHist object for the commlog passed in. Does not validate the commlog provided.</summary>
		public static CommlogHist CreateFromCommlog(Commlog commlog) {
			//No need for remoting role check, no call to Db.
			return new CommlogHist() {
				CommlogNum=commlog.CommlogNum,
				PatNum=commlog.PatNum,
				CommDateTime=commlog.CommDateTime,
				CommType=commlog.CommType,
				Note=commlog.Note,
				Mode_=commlog.Mode_,
				SentOrReceived=commlog.SentOrReceived,
				UserNum=commlog.UserNum,
				Signature=commlog.Signature,
				SigIsTopaz=commlog.SigIsTopaz,
				//DateTStamp=commlog.DateTStamp, DB timestamp, set automatically when this CommlogHist row is inserted into the db. Use cast operators if you want this pulled from the commlog.
				DateTimeEnd=commlog.DateTimeEnd,
				CommSource=commlog.CommSource,
				ProgramNum=commlog.ProgramNum,
				//DateTEntry=commlog.DateTEntry, DB entry date, set automatically when this CommlogHist row is inserted into the db. Use cast operators if you want this pulled from the commlog.
			};
		}
		#endregion Methods - Misc
	}
}