using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDentBusiness{
	///<summary></summary>
	public class CustReferences{
		///<summary>Gets one CustReference from the db.</summary>
		public static CustReference GetOne(long custReferenceNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<CustReference>(MethodBase.GetCurrentMethod(),custReferenceNum);
			}
			return Crud.CustReferenceCrud.SelectOne(custReferenceNum);
		}

		///<summary></summary>
		public static long Insert(CustReference custReference){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				custReference.CustReferenceNum=Meth.GetLong(MethodBase.GetCurrentMethod(),custReference);
				return custReference.CustReferenceNum;
			}
			return Crud.CustReferenceCrud.Insert(custReference);
		}

		///<summary></summary>
		public static void Update(CustReference custReference){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),custReference);
				return;
			}
			Crud.CustReferenceCrud.Update(custReference);
		}

		///<summary>Might not be used.  Might implement when a patient is deleted but doesn't happen often if ever.</summary>
		public static void Delete(long custReferenceNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),custReferenceNum);
				return;
			}
			string command= "DELETE FROM custreference WHERE CustReferenceNum = "+POut.Long(custReferenceNum);
			Db.NonQ(command);
		}

		///<summary>Used only from FormReferenceSelect to get the list of references.</summary>
		public static DataTable GetReferenceTable(bool limit,long[] billingTypes,bool showBadRefs,bool showUsed,bool showGuarOnly,string city,string state,string zip,
			string areaCode,string specialty,int superFam,string lname,string fname,string patnum,int age,string country) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),limit,billingTypes,showBadRefs,showUsed,showGuarOnly,city,state,zip,areaCode,specialty,superFam,lname,fname,patnum,age,country);
			}
			string billingSnippet="";
			if(billingTypes.Length!=0){
				for(int i=0;i<billingTypes.Length;i++) {
					if(i==0) {
						billingSnippet+="AND (";
					}
					else {
						billingSnippet+="OR ";
					}
					billingSnippet+="BillingType="+POut.Long(billingTypes[i])+" ";
					if(i==billingTypes.Length-1) {
						billingSnippet+=") ";
					}
				}
			}
			string phonedigits="";
			for(int i=0;i<areaCode.Length;i++) {
				if(Regex.IsMatch(areaCode[i].ToString(),"[0-9]")) {
					phonedigits=phonedigits+areaCode[i];
				}
			}
			string regexp="";
			for(int i=0;i<phonedigits.Length;i++) {
				if(i<1) {
					regexp="^[^0-9]?";//Allows phone to start with "("
				}
				regexp+=phonedigits[i]+"[^0-9]*";
			}
			DataTable table=new DataTable();
			DataRow row;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("CustReferenceNum");
			table.Columns.Add("PatNum");
			table.Columns.Add("FName");
			table.Columns.Add("LName");
			table.Columns.Add("HmPhone");
			table.Columns.Add("State");
			table.Columns.Add("City");
			table.Columns.Add("Zip");
			table.Columns.Add("Country");
			table.Columns.Add("Specialty");
			table.Columns.Add("age");
			table.Columns.Add("SuperFamily");
			table.Columns.Add("DateMostRecent");
			table.Columns.Add("TimesUsed");
			table.Columns.Add("IsBadRef");
			List<DataRow> rows=new List<DataRow>();
			string command=@"SELECT cr.*,p.LName,p.FName,p.HmPhone,p.State,p.City,p.Zip,p.Birthdate,pf.FieldValue,
					(SELECT COUNT(*) FROM patient tempp WHERE tempp.SuperFamily=p.SuperFamily AND tempp.SuperFamily<>0) AS SuperFamily,
					(SELECT COUNT(*) FROM custrefentry tempcre WHERE tempcre.PatNumRef=cr.PatNum) AS TimesUsed,p.Country
				FROM custreference cr
				INNER JOIN patient p ON cr.PatNum=p.PatNum
				LEFT JOIN patfield pf ON cr.PatNum=pf.PatNum AND pf.FieldName='Specialty' 
				WHERE TRUE ";//This just makes the following AND statements brainless.
				command+="AND (p.PatStatus="+POut.Int((int)PatientStatus.Patient)+" OR p.PatStatus="+POut.Int((int)PatientStatus.NonPatient)+") "//excludes deleted, etc.
					+billingSnippet;
			if(age > 0) {
				command+="AND p.Birthdate <"+POut.Date(DateTime.Now.AddYears(-age))+" ";
			}
			if(regexp!="") {
				command+="AND (p.HmPhone REGEXP '"+POut.String(regexp)+"' )";
			}
			command+=(lname.Length>0?"AND (p.LName LIKE '"+POut.String(lname)+"%' OR p.Preferred LIKE '"+POut.String(lname)+"%') ":"")
					+(fname.Length>0?"AND (p.FName LIKE '"+POut.String(fname)+"%' OR p.Preferred LIKE '"+POut.String(fname)+"%') ":"")
					+(city.Length>0?"AND p.City LIKE '"+POut.String(city)+"%' ":"")
					+(state.Length>0?"AND p.State LIKE '"+POut.String(state)+"%' ":"")
					+(zip.Length>0?"AND p.Zip LIKE '"+POut.String(zip)+"%' ":"")
					+(country.Length>0?"AND p.Country LIKE '"+POut.String(country)+"%' ":"")
					+(patnum.Length>0?"AND p.PatNum LIKE '"+POut.String(patnum)+"%' ":"")
					+(specialty.Length>0?"AND pf.FieldValue LIKE '"+POut.String(specialty)+"%' ":"")
					+(showBadRefs?"":"AND cr.IsBadRef=0 ")
					+(showGuarOnly?"AND p.Guarantor=p.PatNum ":"")
					+"HAVING TRUE ";//Once again just making AND statements brainless.
			if(superFam>0) {
				command+="AND SuperFamily>"+POut.Int(superFam)+" ";
			}
			if(showUsed) {
				command+="AND TimesUsed>0 ";
			}
			if(limit) {
				command=DbHelper.LimitOrderBy(command,40);
			}
			DataTable rawtable=Db.GetTable(command);
			for(int i=0;i<rawtable.Rows.Count;i++) {
				row=table.NewRow();
				row["CustReferenceNum"]=rawtable.Rows[i]["CustReferenceNum"].ToString();
				row["PatNum"]=rawtable.Rows[i]["PatNum"].ToString();
				row["FName"]=rawtable.Rows[i]["FName"].ToString();
				row["LName"]=rawtable.Rows[i]["LName"].ToString();
				row["HmPhone"]=rawtable.Rows[i]["HmPhone"].ToString();
				row["State"]=rawtable.Rows[i]["State"].ToString();
				row["City"]=rawtable.Rows[i]["City"].ToString();
				row["Zip"]=rawtable.Rows[i]["Zip"].ToString();
				row["Country"]=rawtable.Rows[i]["Country"].ToString();
				row["Specialty"]=rawtable.Rows[i]["FieldValue"].ToString();
				row["age"]=Patients.DateToAge(PIn.Date(rawtable.Rows[i]["Birthdate"].ToString())).ToString();
				row["SuperFamily"]=rawtable.Rows[i]["SuperFamily"].ToString();
				DateTime recentDate=PIn.DateT(rawtable.Rows[i]["DateMostRecent"].ToString());
				row["DateMostRecent"]="";
				if(recentDate.Year>1880) {
					row["DateMostRecent"]=recentDate.ToShortDateString();
				}
				row["TimesUsed"]=rawtable.Rows[i]["TimesUsed"].ToString();
				row["IsBadRef"]=rawtable.Rows[i]["IsBadRef"].ToString();
				rows.Add(row);
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		///<summary>Returns FName 'Preferred' M LName.  This is here because I get names by patnum a lot with references.</summary>
		public static string GetCustNameFL(long patNum) {
			//Calls to the db happen in the other s classes.
			Patient pat=Patients.GetLim(patNum);
			return Patients.GetNameFL(pat.LName,pat.FName,pat.Preferred,pat.MiddleI);
		}

		///<summary>Gets the most recent CustReference entry for that patient.  Returns null if none found.  There should be only one entry for each patient, but there was a bug before 14.3 that could have created multiple so we only get the more relevant entry.</summary>
		public static CustReference GetOneByPatNum(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<CustReference>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * "
						+"FROM custreference "
						+"WHERE PatNum="+POut.Long(patNum)+" "
						+"ORDER BY DateMostRecent DESC";
			List<CustReference> custRefList=Crud.CustReferenceCrud.SelectMany(command);
			if(custRefList.Count==0) {
				return null;
			}
			else {
				return custRefList[0];
			}
		}
	}
}