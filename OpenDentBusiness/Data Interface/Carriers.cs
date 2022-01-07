using CodeBase;
using DataConnectionBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Carriers{
		#region Cache Pattern

		private class CarrierCache : CacheDictAbs<Carrier,long,Carrier> {
			protected override List<Carrier> GetCacheFromDb() {
				string command="SELECT * FROM carrier ORDER BY CarrierName";
				return Crud.CarrierCrud.SelectMany(command);
			}
			protected override List<Carrier> TableToList(DataTable table) {
				return Crud.CarrierCrud.TableToList(table);
			}
			protected override Carrier Copy(Carrier carrier) {
				return carrier.Copy();
			}
			protected override DataTable DictToTable(Dictionary<long,Carrier> dictCarriers) {
				return Crud.CarrierCrud.ListToTable(dictCarriers.Values.Cast<Carrier>().ToList(),"Carrier");
			}
			protected override void FillCacheIfNeeded() {
				Carriers.GetTableFromCache(false);
			}
			protected override bool IsInDictShort(Carrier carrier) {
				return !carrier.IsHidden;
			}
			protected override long GetDictKey(Carrier carrier) {
				return carrier.CarrierNum;
			}
			protected override Carrier GetDictValue(Carrier carrier) {
				return carrier;
			}
			protected override Carrier CopyDictValue(Carrier carrier) {
				return carrier.Copy();
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static CarrierCache _carrierCache=new CarrierCache();

		public static bool GetContainsKey(long key,bool isShort=false) {
			return _carrierCache.GetContainsKey(key,isShort);
		}

		public static Carrier GetOne(long codeNum) {
			return _carrierCache.GetOne(codeNum);
		}

		public static Carrier GetFirstOrDefault(Func<Carrier,bool> match,bool isShort=false) {
			return _carrierCache.GetFirstOrDefault(match,isShort);
		}

		public static List<Carrier> GetWhere(Func<Carrier,bool> match,bool isShort=false) {
			return _carrierCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_carrierCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_carrierCache.FillCacheFromTable(table);
				return table;
			}
			return _carrierCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary>Used to get a list of carriers to display in the FormCarriers window.</summary>
		public static DataTable GetBigList(bool isCanadian,bool showHidden,string carrierName,string carrierPhone,string carrierElectId) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),isCanadian,showHidden,carrierName,carrierPhone,carrierElectId);
			}
			DataTable tableRaw;
			DataTable table;
			string command;
			//if(isCanadian){
			//Strip out the digits from the phone number.
			string phonedigits="";
			for(int i=0;i<carrierPhone.Length;i++) {
				if(Regex.IsMatch(carrierPhone[i].ToString(),"[0-9]")) {
					phonedigits=phonedigits+carrierPhone[i];
				}
			}
			//Create a regular expression so that the phone search uses only numbers.
			string regexp="";
			for(int i=0;i<phonedigits.Length;i++) {
				if(i!=0) {
					regexp+="[^0-9]*";//zero or more intervening digits that are not numbers
				}
				regexp+=phonedigits[i];
			}
			command="SELECT Address,Address2,canadiannetwork.Abbrev,carrier.CarrierNum,"
				+"CarrierName,CDAnetVersion,City,ElectID,"
				+"COUNT(insplan.PlanNum) insPlanCount,IsCDA,"
				+"carrier.IsHidden,Phone,State,Zip "
				+"FROM carrier "
				+"LEFT JOIN canadiannetwork ON canadiannetwork.CanadianNetworkNum=carrier.CanadianNetworkNum "
				+"LEFT JOIN insplan ON insplan.CarrierNum=carrier.CarrierNum "
				+"WHERE "
				+"CarrierName LIKE '%"+POut.String(carrierName)+"%' "
				+"AND ElectID LIKE '%"+POut.String(carrierElectId)+"%'";
			if(regexp!="") {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command+="AND Phone REGEXP '"+POut.String(regexp)+"' ";
				}
				else {//oracle
					command+="AND (SELECT REGEXP_INSTR(Phone,'"+POut.String(regexp)+"') FROM dual)<>0";
				}
			}
			if(isCanadian){
				command+="AND IsCDA=1 ";
			}
			if(!showHidden){
				command+="AND carrier.IsHidden=0 ";
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+="GROUP BY carrier.CarrierNum ";
			}
			else {//Oracle
				command+="GROUP BY Address,Address2,canadiannetwork.Abbrev,carrier.CarrierNum,"
				+"CarrierName,CDAnetVersion,City,ElectID,IsCDA,"
				+"carrier.IsHidden,Phone,State,Zip ";
			}
			command+="ORDER BY CarrierName";
			tableRaw=Db.GetTable(command);
			table=new DataTable();
			table.Columns.Add("Address");
			table.Columns.Add("Address2");
			table.Columns.Add("CarrierNum");
			table.Columns.Add("CarrierName");
			table.Columns.Add("City");
			table.Columns.Add("ElectID");
			table.Columns.Add("insPlanCount");
			table.Columns.Add("isCDA");
			table.Columns.Add("isHidden");
			table.Columns.Add("Phone");
			//table.Columns.Add("pMP");
			//table.Columns.Add("network");
			table.Columns.Add("State");
			//table.Columns.Add("version");
			table.Columns.Add("Zip");
			DataRow row;
			for(int i=0;i<tableRaw.Rows.Count;i++){
				row=table.NewRow();
				row["Address"]=tableRaw.Rows[i]["Address"].ToString();
				row["Address2"]=tableRaw.Rows[i]["Address2"].ToString();
				row["CarrierNum"]=tableRaw.Rows[i]["CarrierNum"].ToString();
				row["CarrierName"]=tableRaw.Rows[i]["CarrierName"].ToString();
				row["City"]=tableRaw.Rows[i]["City"].ToString();
				row["ElectID"]=tableRaw.Rows[i]["ElectID"].ToString();
				if(PIn.Bool(tableRaw.Rows[i]["IsCDA"].ToString())) {
					row["isCDA"]="X";
				}
				else {
					row["isCDA"]="";
				}
				if(PIn.Bool(tableRaw.Rows[i]["IsHidden"].ToString())){
					row["isHidden"]="X";
				}
				else{
					row["isHidden"]="";
				}
				row["insPlanCount"]=tableRaw.Rows[i]["insPlanCount"].ToString();
				row["Phone"]=tableRaw.Rows[i]["Phone"].ToString();
				//if(PIn.Bool(tableRaw.Rows[i]["IsPMP"].ToString())){
				//	row["pMP"]="X";
				//}
				//else{
				//	row["pMP"]="";
				//}
				//row["network"]=tableRaw.Rows[i]["Abbrev"].ToString();
				row["State"]=tableRaw.Rows[i]["State"].ToString();
				//row["version"]=tableRaw.Rows[i]["CDAnetVersion"].ToString();
				row["Zip"]=tableRaw.Rows[i]["Zip"].ToString();
				table.Rows.Add(row);
			}
			return table;
		}

		///<summary>Surround with try/catch.</summary>
		public static void Update(Carrier carrier,Carrier oldCarrier) {
			Update(carrier,oldCarrier,Security.CurUser.UserNum);
		}

		///<summary>Surround with try/catch.
		///No need to pass in usernum, it is set before the remoting role and passed in for logging.</summary>
		public static void Update(Carrier carrier,Carrier oldCarrier,long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),carrier,oldCarrier,userNum);
				return;
			}
			string command;
			DataTable table;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(carrier.IsCDA) {
					if(carrier.ElectID=="") {
						throw new ApplicationException(Lans.g("Carriers","Carrier Identification Number required."));
					}
					if(!Regex.IsMatch(carrier.ElectID,"^[0-9]{6}$")) {
						throw new ApplicationException(Lans.g("Carriers","Carrier Identification Number must be exactly 6 numbers."));
					}
				}
				//so the edited carrier looks good, but now we need to make sure that the original was allowed to be changed.
				command="SELECT ElectID,IsCDA FROM carrier WHERE CarrierNum = '"+POut.Long(carrier.CarrierNum)+"'";
				table=Db.GetTable(command);
				if(PIn.Bool(table.Rows[0]["IsCDA"].ToString())//if original carrier IsCDA
					&& PIn.String(table.Rows[0]["ElectID"].ToString()).Trim()!="" //and the ElectID was already set
					&& PIn.String(table.Rows[0]["ElectID"].ToString())!=carrier.ElectID)//and the ElectID was changed
				{
					command="SELECT COUNT(*) FROM etrans WHERE CarrierNum= "+POut.Long(carrier.CarrierNum)
						+" OR CarrierNum2="+POut.Long(carrier.CarrierNum);
					if(Db.GetCount(command)!="0"){
						throw new ApplicationException(Lans.g("Carriers","Not allowed to change Carrier Identification Number because it's in use in the claim history."));
					}
				}
			}
			Crud.CarrierCrud.Update(carrier,oldCarrier);
			InsEditLogs.MakeLogEntry(carrier,oldCarrier,InsEditLogType.Carrier,userNum);
		}

		///<summary>Surround with try/catch if possibly adding a Canadian carrier.</summary>
		public static long Insert(Carrier carrier, Carrier carrierOld=null,bool useExistingPriKey=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				carrier.CarrierNum=Meth.GetLong(MethodBase.GetCurrentMethod(),carrier,carrierOld,useExistingPriKey);
				return carrier.CarrierNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			carrier.SecUserNumEntry=Security.CurUser.UserNum;
			//string command;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(carrier.IsCDA){
					if(carrier.ElectID==""){
						throw new ApplicationException(Lans.g("Carriers","Carrier Identification Number required."));
					}
					if(!Regex.IsMatch(carrier.ElectID,"^[0-9]{6}$")) {
						throw new ApplicationException(Lans.g("Carriers","Carrier Identification Number must be exactly 6 numbers."));
					}
				}
			}
			if(carrierOld==null) {
				carrierOld=carrier.Copy();
			}
			Crud.CarrierCrud.Insert(carrier,useExistingPriKey);
			if(carrierOld.CarrierNum != 0) {
				InsEditLogs.MakeLogEntry(carrier,carrierOld,InsEditLogType.Carrier,carrier.SecUserNumEntry);
			}
			else {
				InsEditLogs.MakeLogEntry(carrier,null,InsEditLogType.Carrier,carrier.SecUserNumEntry);
			}
			return carrier.CarrierNum;
		}

		///<summary>Surround with try/catch.  If there are any dependencies, then this will throw an exception.  
		///This is currently only called from FormCarrierEdit.</summary>
		public static void Delete(Carrier Cur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			//look for dependencies in insplan table.
			string command="SELECT insplan.PlanNum,CONCAT(CONCAT(LName,', '),FName) FROM insplan "
				+"LEFT JOIN inssub ON insplan.PlanNum=inssub.PlanNum "
				+"LEFT JOIN patient ON inssub.Subscriber=patient.PatNum " 
				+"WHERE insplan.CarrierNum = "+POut.Long(Cur.CarrierNum)+" "
				+"ORDER BY LName,FName";
			DataTable table=Db.GetTable(command);
			string strInUse;
			if(table.Rows.Count>0){
				strInUse="";//new string[table.Rows.Count];
				for(int i=0;i<table.Rows.Count;i++) {
					if(i>0){
						strInUse+="; ";
					}
					strInUse+=PIn.String(table.Rows[i][1].ToString());
				}
				throw new ApplicationException(Lans.g("Carriers","Not allowed to delete carrier because it is in use.  Subscribers using this carrier include ")+strInUse);
			}
			//look for dependencies in etrans table.
			command="SELECT DateTimeTrans FROM etrans WHERE CarrierNum="+POut.Long(Cur.CarrierNum)
				+" OR CarrierNum2="+POut.Long(Cur.CarrierNum);
			table=Db.GetTable(command);
			if(table.Rows.Count>0){
				strInUse="";
				for(int i=0;i<table.Rows.Count;i++) {
					if(i>0) {
						strInUse+=", ";
					}
					strInUse+=PIn.DateT(table.Rows[i][0].ToString()).ToShortDateString();
				}
				throw new ApplicationException(Lans.g("Carriers","Not allowed to delete carrier because it is in use in the etrans table.  Dates of claim sent history include ")+strInUse);
			}
			command="DELETE from carrier WHERE CarrierNum = "+POut.Long(Cur.CarrierNum);
			Db.NonQ(command);
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			InsEditLogs.MakeLogEntry(null,Cur,InsEditLogType.Carrier,Security.CurUser.UserNum);
		}

		///<summary>Returns a list of insplans that are dependent on the Cur carrier. Used to display in carrier edit.</summary>
		public static List<string> DependentPlans(Carrier Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod(),Cur);
			}
			string command="SELECT CONCAT(CONCAT(LName,', '),FName) FROM patient,insplan,inssub" 
				+" WHERE patient.PatNum=inssub.Subscriber"
				+" AND insplan.PlanNum=inssub.PlanNum"
				+" AND insplan.CarrierNum = '"+POut.Long(Cur.CarrierNum)+"'"
				+" ORDER BY LName,FName";
			DataTable table=Db.GetTable(command);
			List<string> retStr=new List<string>();
			for(int i=0;i<table.Rows.Count;i++) {
				retStr.Add(PIn.String(table.Rows[i][0].ToString()));
			}
			return retStr;
		}

		///<summary>Gets the name of a carrier based on the carrierNum.  
		///This also refreshes the list if necessary, so it will work even if the list has not been refreshed recently.</summary>
		public static string GetName(long carrierNum) {
			//No need to check RemotingRole; no call to db.
			string carrierName="";
			//This is an uncommon pre-check because places throughout the program explicitly did not correctly send out a cache refresh signal.
			if(!GetContainsKey(carrierNum)) {
				RefreshCache();
			}
			ODException.SwallowAnyException(() => {
				carrierName=GetOne(carrierNum).CarrierName;
			});
			//Empty string can only happen if corrupted:
			return carrierName;
		}

		public static Carrier GetCarrierDB(long carrierNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Carrier>(MethodBase.GetCurrentMethod(),carrierNum);
			}
			string command="SELECT * FROM carrier WHERE CarrierNum="+POut.Long(carrierNum);
			return Crud.CarrierCrud.SelectOne(command);
		}

		///<summary>Gets the specified carrier from Cache. 
		///This also refreshes the list if necessary, so it will work even if the list has not been refreshed recently.</summary>
		public static Carrier GetCarrier(long carrierNum) {
			//No need to check RemotingRole; no call to db.
			Carrier retVal=new Carrier() { CarrierName="" };
			//This is an uncommon pre-check because places throughout the program explicitly did not correctly send out a cache refresh signal.
			if(!GetContainsKey(carrierNum)) {
				RefreshCache();
			}
			ODException.SwallowAnyException(() => {
				retVal=GetOne(carrierNum);
			});
			//New and empty carrier can only happen if corrupted.
			return retVal;
		}

		///<summary>Primarily used when user clicks OK from the InsPlan window.  Gets a carrierNum from the database based on the other supplied carrier
		///data.  Sets the CarrierNum accordingly. If there is no matching carrier, then a new carrier is created.  The end result is a valid carrierNum
		///to use.</summary>
		public static Carrier GetIdentical(Carrier carrier,Carrier carrierOld=null) {
			if(carrier.CarrierName=="") {
				return new Carrier();//should probably be null instead
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Carrier>(MethodBase.GetCurrentMethod(),carrier,carrierOld);
			}
			Carrier retVal=carrier.Copy();
			string command="SELECT CarrierNum,Phone FROM carrier WHERE " 
				+"CarrierName = '"    +POut.String(carrier.CarrierName)+"' "
				+"AND Address = '"    +POut.String(carrier.Address)+"' "
				+"AND Address2 = '"   +POut.String(carrier.Address2)+"' "
				+"AND City = '"       +POut.String(carrier.City)+"' "
				+"AND State LIKE '"   +POut.String(carrier.State)+"' "//This allows user to remove trailing spaces from the FormInsPlan interface.
				+"AND Zip = '"        +POut.String(carrier.Zip)+"' "
				+"AND ElectID = '"    +POut.String(carrier.ElectID)+"' "
				+"AND NoSendElect = " +POut.Int((int)carrier.NoSendElect)+" ";
			if(carrierOld!=null) {
				//When "Change Plan for all subscribers is selected on FormInsPlan, the user can be prompted to change the carrier for the plan's received
				//claims if any carrier information is edited. This prompt also could incorrectly appear when a plan was picked from the list
				//as a new plan for the patient, but no carrier info was changed. To prevent that, we first try to choose carrierOld if it is in our table.
				command+="ORDER BY (CASE WHEN CarrierNum="+POut.Long(carrierOld.CarrierNum)+" THEN 0 ELSE 1 END)";
			}
			DataTable table=Db.GetTable(command);
			//Previously carrier.Phone has been given to us after being formatted by ValidPhone in the UI (FormInsPlan).
			//Strip all formatting from the given phone number and the DB phone numbers to compare.
			//The phone in the database could be in a different format if it was imported in an old version.
			string carrierPhoneStripped=StringTools.StripNonDigits(carrier.Phone);
			for(int i=0;i<table.Rows.Count;i++) {
				string phone=PIn.String(table.Rows[i]["Phone"].ToString());
				if(StringTools.StripNonDigits(phone)==carrierPhoneStripped){
					//A matching carrier was found in the database, so we will use it.
					retVal.CarrierNum=PIn.Long(table.Rows[i][0].ToString());
					return retVal;
				}
			}
			//No match found.  Decide what to do.  Usually add carrier.--------------------------------------------------------------
			//Canada:
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				throw new ApplicationException(Lans.g("Carriers","Carrier not found."));//gives user a chance to add manually.
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			carrier.SecUserNumEntry=Security.CurUser.UserNum;
			Insert(carrier,carrierOld); //insert function takes care of logging.
			retVal.CarrierNum=carrier.CarrierNum;
			return retVal;
		}

		///<summary>Returns true if all fields for one carrier match all fields for another carrier.  
		///Returns false if one of the carriers is null or any of the fields don't match.</summary>
		public static bool Compare(Carrier carrierOne,Carrier carrierTwo) {
			if(carrierOne==null || carrierTwo==null) {
				return false;
			}
			if(carrierOne.Address!=carrierTwo.Address
				|| carrierOne.Address2!=carrierTwo.Address2
				|| carrierOne.CarrierName!=carrierTwo.CarrierName
				|| carrierOne.City!=carrierTwo.City
				|| carrierOne.ElectID!=carrierTwo.ElectID
				|| carrierOne.NoSendElect!=carrierTwo.NoSendElect
				|| carrierOne.Phone!=carrierTwo.Phone
				|| carrierOne.State!=carrierTwo.State
				|| carrierOne.Zip!=carrierTwo.Zip) 
			{
				return false;
			}
			return true;
		}

		///<summary>Returns an arraylist of Carriers with names similar to the supplied string.  Used in dropdown list from carrier field for faster entry.  There is a small chance that the list will not be completely refreshed when this is run, but it won't really matter if one carrier doesn't show in dropdown.</summary>
		public static List<Carrier> GetSimilarNames(string carrierName){
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.CarrierName.ToUpper().IndexOf(carrierName.ToUpper())==0,true);
		}

		///<summary>Excludes hidden Carriers. Not case sensitive. Returns a list of Carriers with the passed in name.</summary>
		public static List<Carrier> GetExactNames(string carrierName) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.CarrierName.ToUpper().Equals(carrierName.ToUpper()),true);
		}

		///<summary>Surround with try/catch. Combines all the given carriers into one. 
		///The carrier that will be used as the basis of the combination is specified in the pickedCarrier argument. 
		///Updates insplan and etrans, then deletes all the other carriers.</summary>
		public static void Combine(List<long> listCarrierNums,long pickedCarrierNum) {
			if(listCarrierNums==null || listCarrierNums.Count<=1) {
				return;//Nothing to do.
			}
			//Remove the CarrierNum that was picked as the superior carrier in order to get the list of carriers that will be combined into the picked carrier.
			List<long> listCarrierNumsToCombine=listCarrierNums.FindAll(x => x!=pickedCarrierNum);
			if(listCarrierNumsToCombine.IsNullOrEmpty()) {
				return;//No carriers to combine.
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listCarrierNums,pickedCarrierNum);
				return;
			}
			string strCarrierNums=string.Join(",",listCarrierNumsToCombine.Select(x => POut.Long(x)));
			//Create InsEditLogs============================================================================================================
			//Get all of the related insplan objects from the database.
			List<InsPlan> listInsPlan=InsPlans.GetAllByCarrierNums(listCarrierNumsToCombine.ToArray());
			//Create an InsEditLog out of each InsPlan returned that will be inserted into the database later.
			List<InsEditLog> listInsEditLogs=listInsPlan.Select(x => 
				InsEditLogs.MakeLogEntry("CarrierNum",
					Security.CurUser.UserNum,//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
					POut.Long(x.CarrierNum),
					POut.Long(pickedCarrierNum),
					InsEditLogType.InsPlan,
					x.PlanNum,
					0,
					x.GroupNum+" - "+x.GroupName,
					doInsert:false)
				).ToList();
			//Update insplan.CarrierNum=====================================================================================================
			string command=$"UPDATE insplan SET CarrierNum = {POut.Long(pickedCarrierNum)} WHERE CarrierNum IN({strCarrierNums})";
			Db.NonQ(command);
			//Update insbluebook.CarrierNum=================================================================================================
			command=$"UPDATE insbluebook SET insbluebook.CarrierNum = {POut.Long(pickedCarrierNum)} WHERE insbluebook.CarrierNum IN({strCarrierNums})";
			Db.NonQ(command);
			//Update etrans.CarrierNum======================================================================================================
			command=$"UPDATE etrans SET CarrierNum = {POut.Long(pickedCarrierNum)} WHERE CarrierNum IN({strCarrierNums})";
			Db.NonQ(command);
			//Update etrans.CarrierNum2=====================================================================================================
			command=$"UPDATE etrans SET CarrierNum2 = {POut.Long(pickedCarrierNum)} WHERE CarrierNum2 IN({strCarrierNums})";
			Db.NonQ(command);
			//Insert InsEditLogs============================================================================================================
			InsEditLogs.InsertMany(listInsEditLogs);
			//Delete Carriers===============================================================================================================
			//Get all of the related carrier objects from the cache before they are deleted.
			List<Carrier> listCarriersToCombine=GetCarriers(listCarrierNumsToCombine);
			//Delete all of the carriers from the database that were just combined into the picked carrier.
			command=$"DELETE FROM carrier WHERE CarrierNum IN({strCarrierNums})";
			Db.NonQ(command);
			//Make an InsEditLog for each carrier that was just deleted.
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			listCarriersToCombine.ForEach(x => InsEditLogs.MakeLogEntry(null,x,InsEditLogType.Carrier,Security.CurUser.UserNum));
		}

		///<summary>Used in the FormCarrierCombine window.</summary>
		public static List<Carrier> GetCarriers(List<long> carrierNums) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => ListTools.In(x.CarrierNum,carrierNums));
		}

		///<summary>If listInsPlans is empty, returns an empty list. Gets all carriers for InsPlans.</summary>
		public static List<Carrier> GetForInsPlans(List<InsPlan> listInsPlans) {
			//No need to check RemotingRole; no call to db.
			if(listInsPlans.Count==0) {
				return new List<Carrier>();
			}
			List<long> listCarrierNumsForClaims=listInsPlans.Select(x => x.CarrierNum).Distinct().ToList();
			return Carriers.GetCarriers(listCarrierNumsForClaims);
		}

		///<summary>Queries the database for all carriers that are flagged as IsCDA that have at least one etrans request message present.</summary>
		public static List<Carrier> GetCdaCarriersInUse() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Carrier>>(MethodBase.GetCurrentMethod());
			}
			//Use reflection to get all the values from the EtransType enumeration where the field is flagged as a request type via the EtransTypeAttr.
			List<EtransType> listEtransRequestTypes=typeof(EtransType).GetFields()
				.Select(x => 
					new {
						fieldInfo=x,
						etransTypeAttr=x.GetCustomAttributes(false).OfType<EtransTypeAttr>().FirstOrDefault(),
					})
				.Where(x => x.etransTypeAttr!=null && x.etransTypeAttr.IsRequestType)
				.Select(x => x.fieldInfo.GetValue(null))
				.Cast<EtransType>()
				.ToList();
			//Get all of the carriers flagged as IsCDA that have had at least one etrans request in the past.
			string command=$@"SELECT carrier.* 
				FROM carrier
				INNER JOIN etrans ON carrier.CarrierNum=etrans.CarrierNum 
					AND etrans.Etype IN({string.Join(",",listEtransRequestTypes.Select(x => POut.Enum(x)))})
				WHERE carrier.IsCDA=1
				GROUP BY carrier.CarrierNum";
			return Crud.CarrierCrud.SelectMany(command);
		}

		///<summary>Used in FormInsPlan to check whether another carrier is already using this id.
		///That way, it won't tell the user that this might be an invalid id.</summary>
		public static bool ElectIdInUse(string electID){
			//No need to check RemotingRole; no call to db.
			if(string.IsNullOrEmpty(electID)) {
				return true;
			}
			return (_carrierCache.GetFirstOrDefault(x => x.ElectID==electID)!=null);
		}

		///<summary>Used from insplan window when requesting benefits.  Gets carrier based on electID.  Returns empty list if no match found.</summary>
		public static List<Carrier> GetAllByElectId(string electID){
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.ElectID==electID);
		}

		///<summary>If carrierName is blank (empty string) this will throw an ApplicationException.  If a carrier is not found with the exact name,
		///including capitalization, a new carrier is created, inserted in the database, and returned.</summary>
		public static Carrier GetByNameAndPhone(string carrierName,string phone,bool updateCacheIfNew=false){
			//No need to check RemotingRole; no call to db.
			if(string.IsNullOrEmpty(carrierName)) {
				throw new ApplicationException("Carrier cannot be blank");
			}
			Carrier carrier=GetFirstOrDefault(x => x.CarrierName==carrierName && x.Phone==phone);
			if(carrier==null) {
				carrier=new Carrier();
				carrier.CarrierName=carrierName;
				carrier.Phone=phone;
				Insert(carrier); //Insert function takes care of logging.
				if(updateCacheIfNew) {
					Signalods.SetInvalid(InvalidType.Carriers);
					RefreshCache();
				}
			}
			return carrier;
		}

		///<summary>Returns null if no match is found. You are allowed to pass in nulls.</summary>
		public static Carrier GetByNameAndPhoneNoInsert(string carrierName,string phone) {
			//No need to check RemotingRole; no call to db.
			if(string.IsNullOrEmpty(carrierName) || string.IsNullOrEmpty(phone)) {
				return null;
			}
			return GetFirstOrDefault(x => x.CarrierName==carrierName && x.Phone==phone);
		}

		/*
		///<summary>Gets a dictionary of carrier names for the supplied patient list.</summary>
		public static Dictionary<long,string> GetCarrierNames(List<Patient> patients){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Dictionary<long,string>>(MethodBase.GetCurrentMethod(),patients);
			}
			if(patients.Count==0){
				return new Dictionary<long,string>();
			}
			string command="SELECT patient.PatNum,carrier.CarrierName "
				+"FROM patient "
				+"LEFT JOIN patplan ON patient.PatNum=patplan.PatNum "
				+"LEFT JOIN insplan ON patplan.PlanNum=insplan.PlanNum "
				+"LEFT JOIN carrier ON carrier.CarrierNum=insplan.CarrierNum "
				+"WHERE";
			for(int i=0;i<patients.Count;i++){
				if(i>0){
					command+=" OR";
				}
				command+=" patient.PatNum="+POut.Long(patients[i].PatNum);
			}
			command+=" GROUP BY patient.PatNum,carrier.CarrierName";
			DataTable table=Db.GetTable(command);
			Dictionary<long,string> retVal=new Dictionary<long,string>();
			for(int i=0;i<table.Rows.Count;i++){
				retVal.Add(PIn.Long(table.Rows[i]["PatNum"].ToString()),table.Rows[i]["CarrierName"].ToString());
			}
			return retVal;
		}*/

		///<summary>The carrierName is case insensitive.</summary>
		public static List<Carrier> GetByNameAndTin(string carrierName,string tin){
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.CarrierName.Trim().ToLower()==carrierName.Trim().ToLower() && x.TIN==tin);
		}

		///<summary>Will return null if carrier does not exist with that name.</summary>
		public static Carrier GetCarrierByName(string carrierName) {
			return GetFirstOrDefault(x => x.CarrierName==carrierName);
		}

		///<summary>Will return null if carrier does not exist with that name.</summary>
		public static Carrier GetCarrierByNameNoCache(string carrierName) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Carrier>(MethodBase.GetCurrentMethod(),carrierName);
			}
			string command="SELECT * FROM carrier WHERE CarrierName='"+POut.String(carrierName)+"'";
			return Crud.CarrierCrud.SelectOne(command);
		}

		public static bool IsMedicaid(Carrier carrier) {
			//No need to check RemotingRole; no call to db.
			ElectID electId=ElectIDs.GetID(carrier.ElectID);
			if(electId!=null && electId.IsMedicaid) {//Emdeon Medical requires loop 2420E when the claim is sent to DMERC (Medicaid) carriers.
				return true;
			}
			return false;
		}

		///<summary>Returns true if the carrier is set to block users from entering ortho payments on claims created by the Auto Ortho Tool. Otherwise, returns false if the carrier allows entering payments on claims created by the Auto Ortho Tool.</summary>
		public static bool DoConsolidateOrthoPayments(InsPlan insPlan) {
			//No need to check RemotingRole; no call to db.
			if(insPlan==null) {
				return false;//Invalid params, return false.
			}
			Carrier carrier=GetCarrier(insPlan.CarrierNum);//Returns a blank carrier object if not found which will use Global.
			if(carrier.OrthoInsPayConsolidate==EnumOrthoInsPayConsolidate.Global) {
				return PrefC.GetBool(PrefName.OrthoInsPayConsolidated);
			}
			return carrier.OrthoInsPayConsolidate==EnumOrthoInsPayConsolidate.ForceConsolidateOn;
		}
	}

	
	

}













