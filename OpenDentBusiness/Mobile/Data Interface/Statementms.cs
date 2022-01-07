using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;

namespace OpenDentBusiness.Mobile{
	///<summary></summary>
	public class Statementms{
		#region Only used for webserver for mobile.
		///<summary>Gets all Statementm for a single patient </summary>
		public static List<Statementm> GetStatementms(long customerNum,long patNum) {
			string command=
					"SELECT * from statementm "
					+"WHERE CustomerNum = "+POut.Long(customerNum)
					+" AND PatNum = "+POut.Long(patNum);
			return Crud.StatementmCrud.SelectMany(command);
		}

		///<summary>Limits the number of statements and documents in the database for a single patient </summary>
		public static void LimitStatementmsPerPatient(List<long> patList,long customerNum,int limitPerPatient) {
			int upperlimit=500+limitPerPatient;// The figure 500 is somewhat arbitrary.
			string limitStr="";
			if(limitPerPatient>0) {
				limitStr="LIMIT "+ limitPerPatient+","+upperlimit;
			}
			else {
				return;
			}
			for(int i=0;i<patList.Count;i++) {
				string command="SELECT StatementNum FROM statementm WHERE CustomerNum = "+POut.Long(customerNum)+" AND PatNum = "+POut.Long(patList[i])
					+" ORDER BY DateSent DESC, StatementNum DESC " + limitStr;
				DataTable table=Db.GetTable(command);
				if(table.Rows.Count>0) {
					string strStatementNums=" AND ( ";
					for(int j=0;j<table.Rows.Count;j++) {
						if(j>0) {
							strStatementNums+="OR ";
						}
						strStatementNums+="StatementNum='"+PIn.Long(table.Rows[j]["StatementNum"].ToString())+"' ";
					}
					strStatementNums+=" )";
					command="DELETE FROM statementm WHERE CustomerNum = "+POut.Long(customerNum)+" AND PatNum = "+POut.Long(patList[i])
						+strStatementNums;
					Db.NonQ(command);
				}
			}
			//Note: this statement does not work: error =This version of MySQL doesn't yet support 'LIMIT & IN/ALL/ANY/SOME subquery'
			//DELETE FROM statementm where StatementNum in (SELECT StatementNum FROM statementm WHERE CustomerNum=6566 AND 
			//PatNum=7 ORDER BY DateSent DESC, StatementNum DESC LIMIT 5,100)
		}
		#endregion

		#region Used only on OD
		///<summary>Fetches StatementNums restricted by the DateTStamp, PatNums and a limit of records per patient. If limitPerPatient is zero all StatementNums of a patient are fetched</summary>
		public static List<long> GetChangedSinceStatementNums(DateTime changedSince,List<long> eligibleForUploadPatNumList,int limitPerPatient) {
			return Statements.GetChangedSinceStatementNums(changedSince,eligibleForUploadPatNumList,limitPerPatient);
		}

		///<summary>The values returned are sent to the webserver.</summary>
		public static List<Statementm> GetMultStatementms(List<long> statementNums) {
			List<Statement> statementList=Statements.GetMultStatements(statementNums);
			List<Statementm> statementmList=ConvertListToM(statementList);
			return statementmList;
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<Statementm> ConvertListToM(List<Statement> list) {
			List<Statementm> retVal=new List<Statementm>();
			for(int i=0;i<list.Count;i++) {
				retVal.Add(Crud.StatementmCrud.ConvertToM(list[i]));
			}
			return retVal;
		}
		#endregion

		#region Used only on the Mobile webservice server for  synching.
		///<summary>Only run on server for mobile.  Takes the list of changes from the dental office and makes updates to those items in the mobile server db.  Also, make sure to run DeletedObjects.DeleteForMobile().</summary>
		public static void UpdateFromChangeList(List<Statementm> list,long customerNum) {
			for(int i=0;i<list.Count;i++) {
				list[i].CustomerNum=customerNum;
				Statementm statementm=Crud.StatementmCrud.SelectOne(customerNum,list[i].StatementNum);
				if(statementm==null) {//not in db
					Crud.StatementmCrud.Insert(list[i],true);
				}
				else {
					Crud.StatementmCrud.Update(list[i]);
				}
			}
		}

		///<summary>used in tandem with Full synch</summary>
		public static void DeleteAll(long customerNum) {
			string command= "DELETE FROM statementm WHERE CustomerNum = "+POut.Long(customerNum); ;
			Db.NonQ(command);
		}

		///<summary>Delete all statements of a particular patient</summary>
		public static void Delete(long customerNum,long PatNum) {
			string command= "DELETE FROM statementm WHERE CustomerNum = "+POut.Long(customerNum)+" AND PatNum = "+POut.Long(PatNum);
			Db.NonQ(command);
		}
		#endregion
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<Statementm> Refresh(long patNum){
			string command="SELECT * FROM statementm WHERE PatNum = "+POut.Long(patNum);
			return Crud.StatementmCrud.SelectMany(command);
		}

		///<summary>Gets one Statementm from the db.</summary>
		public static Statementm GetOne(long customerNum,long statementNum){
			return Crud.StatementmCrud.SelectOne(customerNum,statementNum);
		}

		///<summary></summary>
		public static long Insert(Statementm statementm){
			return Crud.StatementmCrud.Insert(statementm,true);
		}

		///<summary></summary>
		public static void Update(Statementm statementm){
			Crud.StatementmCrud.Update(statementm);
		}

		///<summary></summary>
		public static void Delete(long customerNum,long statementNum) {
			string command= "DELETE FROM statementm WHERE CustomerNum = "+POut.Long(customerNum)+" AND StatementNum = "+POut.Long(statementNum);
			Db.NonQ(command);
		}

		///<summary>First use GetChangedSince.  Then, use this to convert the list a list of 'm' objects.</summary>
		public static List<Statementm> ConvertListToM(List<Statement> list) {
			List<Statementm> retVal=new List<Statementm>();
			for(int i=0;i<list.Count;i++){
				retVal.Add(Crud.StatementmCrud.ConvertToM(list[i]));
			}
			return retVal;
		}

		///<summary>Only run on server for mobile.  Takes the list of changes from the dental office and makes updates to those items in the mobile server db.  Also, make sure to run DeletedObjects.DeleteForMobile().</summary>
		public static void UpdateFromChangeList(List<Statementm> list,long customerNum) {
			for(int i=0;i<list.Count;i++){
				list[i].CustomerNum=customerNum;
				Statementm statementm=Crud.StatementmCrud.SelectOne(customerNum,list[i].StatementNum);
				if(statementm==null){//not in db
					Crud.StatementmCrud.Insert(list[i],true);
				}
				else{
					Crud.StatementmCrud.Update(list[i]);
				}
			}
		}
		*/



	}
}