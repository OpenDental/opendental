using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Cpts{
		public static List<Cpt> GetBySearchText(string searchText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Cpt>>(MethodBase.GetCurrentMethod(),searchText);
			}
			string[] searchTokens=searchText.Split(' ');
			string command=@"SELECT * FROM cpt ";
			for(int i=0;i<searchTokens.Length;i++) {
				command+=(i==0?"WHERE ":"AND ")+"(CptCode LIKE '%"+POut.String(searchTokens[i])+"%' OR Description LIKE '%"+POut.String(searchTokens[i])+"%') ";
			}
			return Crud.CptCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(Cpt cpt) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				cpt.CptNum=Meth.GetLong(MethodBase.GetCurrentMethod(),cpt);
				return cpt.CptNum;
			}
			return Crud.CptCrud.Insert(cpt);
		}

		public static List<Cpt> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Cpt>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM cpt";
			return Crud.CptCrud.SelectMany(command);
		}

		public static List<string> GetAllCodes() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			List<string> retVal=new List<string>();
			string command="SELECT CptCode FROM cpt";
			DataTable table=DataCore.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				retVal.Add(table.Rows[i][0].ToString());
			}
			return retVal;
		}

		///<summary>Gets one Cpt object directly from the database by CptCode.  If code does not exist, returns null.</summary>
		public static Cpt GetByCode(string cptCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Cpt>(MethodBase.GetCurrentMethod(),cptCode);
			}
			string command="SELECT * FROM cpt WHERE CptCode='"+POut.String(cptCode)+"'";
			return Crud.CptCrud.SelectOne(command);
		}

		///<summary>Directly from db.</summary>
		public static bool CodeExists(string cptCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),cptCode);
			}
			string command="SELECT COUNT(*) FROM cpt WHERE CptCode = '"+POut.String(cptCode)+"'";
			string count=Db.GetCount(command);
			if(count=="0") {
				return false;
			}
			return true;
		}

		///<summary>Returns the total count of CPT codes.  CPT codes cannot be hidden.</summary>
		public static long GetCodeCount() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM cpt";
			return PIn.Long(Db.GetCount(command));
		}

		///<summary>Updates an existing CPT code description if versionID is newer than current versionIDs.  If versionID is different than existing versionIDs, it will be added to the comma delimited list.</summary>
		public static void UpdateDescription(string cptCode, string description, string versionID) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cptCode, description, versionID);
				return;
			}
			Cpt cpt=Cpts.GetByCode(POut.String(cptCode));
			string[] versionIDs=cpt.VersionIDs.Split(',');
			bool versionIDFound=false;
			string maxVersionID="";
			for(int i=0;i<versionIDs.Length;i++) {
				if(string.Compare(versionIDs[i],maxVersionID)>0) {//Find max versionID in list
					maxVersionID=versionIDs[i];
				}
				if(versionIDs[i]==versionID) {//Find if versionID is already in list
					versionIDFound=true;
				}
			}
			if(!versionIDFound) {//If the current version isn't already in the list
				cpt.VersionIDs+=','+versionID;  //VersionID should never be blank for an existing code... should we check?
			}
			if(string.Compare(versionID,maxVersionID)>=0) { //If newest version
				cpt.Description=description;
			}
			Crud.CptCrud.Update(cpt);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<Cpt> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Cpt>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM cpt WHERE PatNum = "+POut.Long(patNum);
			return Crud.CptCrud.SelectMany(command);
		}

		///<summary>Gets one Cpt from the db.</summary>
		public static Cpt GetOne(long cptNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Cpt>(MethodBase.GetCurrentMethod(),cptNum);
			}
			return Crud.CptCrud.SelectOne(cptNum);
		}

		///<summary></summary>
		public static void Update(Cpt cpt){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cpt);
				return;
			}
			Crud.CptCrud.Update(cpt);
		}

		///<summary></summary>
		public static void Delete(long cptNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cptNum);
				return;
			}
			string command= "DELETE FROM cpt WHERE CptNum = "+POut.Long(cptNum);
			Db.NonQ(command);
		}
		*/
	}
}