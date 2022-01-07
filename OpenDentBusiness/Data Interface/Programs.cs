using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CodeBase;

namespace OpenDentBusiness {

	///<summary></summary>
	public class Programs{
		#region Cache Pattern
		private class ProgramCache : CacheListAbs<Program> {
			protected override List<Program> GetCacheFromDb() {
				string command="SELECT * FROM program ORDER BY ProgDesc";
				return Crud.ProgramCrud.SelectMany(command);
			}
			protected override List<Program> TableToList(DataTable table) {
				return Crud.ProgramCrud.TableToList(table);
			}
			protected override Program Copy(Program program) {
				return program.Copy();
			}
			protected override DataTable ListToTable(List<Program> listPrograms) {
				return Crud.ProgramCrud.ListToTable(listPrograms,"Program");
			}
			protected override void FillCacheIfNeeded() {
				Programs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ProgramCache _programCache=new ProgramCache();

		public static List<Program> GetListDeep(bool isShort=false) {
			return _programCache.GetDeepCopy(isShort);
		}

		public static Program GetFirstOrDefault(Func<Program,bool> match,bool isShort=false) {
			return _programCache.GetFirstOrDefault(match,isShort);
		}

		public static List<Program> GetWhere(Predicate<Program> match,bool isShort=false) {
			return _programCache.GetWhere(match,isShort);
		}

		public static bool HListIsNull()
		{
			return _programCache.ListIsNull();
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_programCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_programCache.FillCacheFromTable(table);
				return table;
			}
			return _programCache.GetTableFromCache(doRefreshCache);
		}

		#endregion Cache Pattern

		///<summary>Checks to see if we have disabled the current program at HQ. Handles null Sets and translates the out parameter where possible.</summary>
		public static bool IsEnabledByHq(Program progCur,out string err) {
			err="";
			bool retVal=false;
			if(progCur==null) {
				err=Lans.g("Programs","The currently selected program could not be found.");
				return retVal;
			}
			if(DoUseCacheValues(progCur)) {
				retVal=!progCur.IsDisabledByHq;
				err=progCur.CustErr;
			}
			else {
				HqProgram prog=HqProgram.GetAll().First(x => x.ProgramNameAsString.Trim()==progCur.ProgName.Trim());
				if(prog==null) {
					retVal=false;
					err=Lans.g("Programs","The currently selected program could not be found.");
				}
				else {
					retVal=prog.IsEnabled;
					if(!retVal) {
						err=prog.CustErr;
					}
				}
			}
			if(!retVal && string.IsNullOrWhiteSpace(err)) {//if the CustErr wasn't set at HQ then we assume a customer is not able to use this program because they are not on support
				err=Lans.g("Program","You must be on support to use this program.");
			}
			return retVal;
		}


		///<summary>Checks to see if we have disabled the current program at HQ. Handles null Sets and translates the out parameter where possible.</summary>
		public static bool IsEnabledByHq(ProgramName progName,out string err) {
			Program progCur=GetCur(progName);
			return IsEnabledByHq(progCur,out err);
		}

		private static bool DoUseCacheValues(Program prog) {
			//Is not an OD defined program name or is not a program HQ is concerned with enabling/disabling.
			return !Enum.TryParse(prog.ProgName,out ProgramName progName) 
				|| !(HqProgram.IsInitialized() && HqProgram.GetAll().Any(x => x.ProgramNameAsString.Trim()==progName.ToString()));
		}


		///<summary></summary>
		public static bool Update(Program cur,Program old=null){
			bool isRefreshNeeded=false;
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),cur,old);
			}
			if(old is null) {
				Crud.ProgramCrud.Update(cur);
				isRefreshNeeded=true;
			}
			else {
				isRefreshNeeded=Crud.ProgramCrud.Update(cur,old);
			}
			return isRefreshNeeded;
		}

		///<summary></summary>
		public static long Insert(Program Cur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Cur.ProgramNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.ProgramNum;
			}
			return Crud.ProgramCrud.Insert(Cur);
		}

		///<summary>This can only be called by the user if it is a program link that they created.  Included program links cannot be deleted.  If doing something similar from ClassConversion, must delete any dependent ProgramProperties first.  It will delete ToolButItems for you.</summary>
		public static void Delete(Program prog){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),prog);
				return;
			}
			string command = "DELETE from toolbutitem WHERE ProgramNum = "+POut.Long(prog.ProgramNum);
			Db.NonQ(command);
			command = "DELETE from program WHERE ProgramNum = '"+prog.ProgramNum.ToString()+"'";
			Db.NonQ(command);
		}

		///<summary>Returns true if a Program link with the given name or number exists and is enabled. Handles null.</summary>
		public static bool IsEnabled(ProgramName progName) {
			//No need to check RemotingRole; no call to db.
			Program program=GetFirstOrDefault(x => x.ProgName==progName.ToString());
			if(program==null){
				return false;
			}
			return program.Enabled;
		}

		///<summary></summary>
		public static bool IsEnabled(long programNum) {
			//No need to check RemotingRole; no call to db.
			Program program=GetFirstOrDefault(x => x.ProgramNum==programNum);
			return (program==null ? false : program.Enabled);
		}

		///<summary>Returns the Program of the passed in ProgramNum.  Will be null if a Program is not found.</summary>
		public static Program GetProgram(long programNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ProgramNum==programNum);
		}

		///<summary>Supply a valid program Name, and this will set Cur to be the corresponding Program object.</summary>
		public static Program GetCur(ProgramName progName) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ProgName==progName.ToString());
		}

		///<summary>Supply a valid program Name.  Will return 0 if not found.</summary>
		public static long GetProgramNum(ProgramName progName) {
			//No need to check RemotingRole; no call to db.
			Program program=GetCur(progName);
			return (program==null ? 0 : program.ProgramNum);
		}

		///<summary>These programs do not work in WEB mode for various reasons. We will restore them as our WEB customers request them.</summary>
		public static List<ProgramName> GetListDisabledForWeb() {
			return new List<ProgramName> {
				ProgramName.Apixia,
				ProgramName.AudaxCeph,
				ProgramName.CADI,
				ProgramName.DBSWin,
				ProgramName.DemandForce,
				ProgramName.DentalEye,
				ProgramName.DentalTekSmartOfficePhone,
				ProgramName.DentX,
				ProgramName.Dolphin,
				ProgramName.DrCeph,
				ProgramName.EvaSoft,
				ProgramName.iCat,
				ProgramName.IAP,
				ProgramName.Guru,
				ProgramName.HouseCalls,
				ProgramName.MediaDent,
				ProgramName.Owandy,
				ProgramName.PandaPerioAdvanced,
				ProgramName.Patterson,
				ProgramName.PT,
				ProgramName.PTupdate,
				ProgramName.Schick,
				ProgramName.Trojan,
				ProgramName.TigerView,
				ProgramName.TrophyEnhanced,
				ProgramName.UAppoint,
				ProgramName.Vipersoft,
				ProgramName.VixWinOld,
				ProgramName.Xcharge,
				ProgramName.PreXionAcquire,
				ProgramName.PreXionViewer,
			};
		}

		/// <summary>Using eClinicalWorks tight integration.</summary>
		public static bool UsingEcwTightMode() {
			//No need to check RemotingRole; no call to db.
			if(Programs.IsEnabled(ProgramName.eClinicalWorks)	&& ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"eClinicalWorksMode")=="0") {
				return true;
			}
			return false;
		}

		/// <summary>Using eClinicalWorks full mode.</summary>
		public static bool UsingEcwFullMode() {
			//No need to check RemotingRole; no call to db.
			if(Programs.IsEnabled(ProgramName.eClinicalWorks)	&& ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"eClinicalWorksMode")=="2") {
				return true;
			}
			return false;
		}

		/// <summary>Returns true if using eCW in tight or full mode.  In these modes, appointments ARE allowed to overlap because we block users from seeing them.</summary>
		public static bool UsingEcwTightOrFullMode() {
			//No need to check RemotingRole; no call to db.
			if(UsingEcwTightMode() || UsingEcwFullMode()) {
				return true;
			}
			return false;
		}

		/// <summary>Orion is completely obsolete.  All sections of code may be removed as you run across them.</summary>
		public static bool UsingOrion {
			//No need to check RemotingRole; no call to db.
			get {
				return Programs.IsEnabled(ProgramName.Orion);
			}
		}

		///<summary>Returns the local override path if available or returns original program path.  Always returns a valid path.</summary>
		public static string GetProgramPath(Program program) {
			//No need to check RemotingRole; no call to db.
			string overridePath=ProgramProperties.GetLocalPathOverrideForProgram(program.ProgramNum);
			if(overridePath!="") {
				return overridePath;
			}
			return program.Path;
		}

		/// <summary>Returns true if input program is a static program. Static programs are ones we do not want the user to be able to modify in some way.</summary>
		public bool IsStatic(Program prog) {
			//Currently there is just one static program. As more are created they will need to be added to this check.
			if(prog.ProgName == ProgramName.RapidCall.ToString()) {
				return true;
			}
			return false;
		}

		///<summary>For each enabled bridge, if the bridge uses a file to transmit patient data to the other software, then we need to remove the files or clear the files when OD is exiting.
		///Required for EHR 2014 module d.7 (as stated by proctor).</summary>
		public static void ScrubExportedPatientData() {
			//List all program links here. If there is nothing to do for that link, then create a comment stating so.
			string path="";
			//Adstra: Has no file paths containing outgoing pateint data from Open Dental.
			//Apixia:
			ScrubFileForProperty(ProgramName.Apixia,"System path to Apixia Digital Imaging ini file","",true);//C:\Program Files\Digirex\Switch.ini
			//Apteryx: Has no file paths containing outgoing patient data from Open Dental.
			//BioPAK: Has no file paths containing outgoing patient data from Open Dental.
			//CADI has no file paths containing outgoing patient data from Open Dental.
			//CallFire: Has no file paths containing outgoing patient data from Open Dental.
			//Camsight: Has no file paths containing outgoing patient data from Open Dental.
			//CaptureLink: Has no file paths containing outgoing patient data from Open Dental.
			//Carestream:
			ScrubFileForProperty(ProgramName.Carestream,"Patient.ini path","",true);//C:\Carestream\Patient.ini
			//Cerec: Has no file paths containing outgoing patient data from Open Dental.
			//CliniView: Has no file paths containing outgoing patient data from Open Dental.
			//ClioSoft: Has no file paths containing outgoing patient data from Open Dental.
			//DBSWin:
			ScrubFileForProperty(ProgramName.DBSWin,"Text file path","",true);//C:\patdata.txt
			//DentalEye: Has no file paths containing outgoing patient data from Open Dental.
			//DentalStudio: Has no file paths containing outgoing patient data from Open Dental.
			//DentForms: Has no file paths containing outgoing patient data from Open Dental.
			//DentX: Has no file paths containing outgoing patient data from Open Dental.
			//Dexis:
			ScrubFileForProperty(ProgramName.Dexis,"InfoFile path","",true);//InfoFile.txt
			//Digora: Has no file paths containing outgoing patient data from Open Dental.
			//Divvy: Has no file paths containing outgoing patient data from Open Dental.
			//Dolphin:
			ScrubFileForProperty(ProgramName.Dolphin,"Filename","",true);//C:\Dolphin\Import\Import.txt
			//DrCeph: Has no file paths containing outgoing patient data from Open Dental.
			//Dxis: Has no file paths containing outgoing patient data from Open Dental.
			//EasyNotesPro: Has no file paths containing outgoing patient data from Open Dental.
			//eClinicalWorks: HL7 files are created, but eCW is supposed to consume and delete them.
			//EvaSoft: Has no file paths containing outgoing patient data from Open Dental.
			//EwooEZDent:
			Program program=Programs.GetCur(ProgramName.EwooEZDent);
			if(program.Enabled) {
				path=Programs.GetProgramPath(program);
				if(File.Exists(path)) {
					string dir=Path.GetDirectoryName(path);
					string linkage=CodeBase.ODFileUtils.CombinePaths(dir,"linkage.xml");
					if(File.Exists(linkage)) {
						try {
							File.Delete(linkage);
						}
						catch {
							//Another instance of OD might be closing at the same time, in which case the delete will fail. Could also be a permission issue or a concurrency issue. Ignore.
						}
					}
				}
			}
			//FloridaProbe: Has no file paths containing outgoing patient data from Open Dental.
			//Guru: Has no file paths containing outgoing patient data from Open Dental.
			//HandyDentist: Has no file paths containing outgoing patient data from Open Dental.
			//HouseCalls:
			//Per Nathan(TaskNum:3517423), disable deleting Appt.txt on close for HouseCalls bridge.
			//ScrubFileForProperty(ProgramName.HouseCalls,"Export Path","Appt.txt",true);//C:\HouseCalls\Appt.txt
			//IAP: Has no file paths containing outgoing patient data from Open Dental.
			//iCat:
			ScrubFileForProperty(ProgramName.iCat,"XML output file path","",true);//C:\iCat\Out\pm.xml
			//ImageFX: Has no file paths containing outgoing patient data from Open Dental.
			//Lightyear: Has no file paths containing outgoing patient data from Open Dental.
			//MediaDent:
			ScrubFileForProperty(ProgramName.MediaDent,"Text file path","",true);//C:\MediadentInfo.txt
			//MiPACS: Has no file paths containing outgoing patient data from Open Dental.
			//Mountainside: Has no file paths containing outgoing patient data from Open Dental.
			//NewCrop: Has no file paths containing outgoing patient data from Open Dental.
			//Orion: Has no file paths containing outgoing patient data from Open Dental.
			//OrthoPlex: Has no file paths containing outgoing patient data from Open Dental.
			//Owandy: Has no file paths containing outgoing patient data from Open Dental.
			//PayConnect: Has no file paths containing outgoing patient data from Open Dental.
			//Patterson:
			ScrubFileForProperty(ProgramName.Patterson,"System path to Patterson Imaging ini","",true);//C:\Program Files\PDI\Shared files\Imaging.ini
			//PerioPal: Has no file paths containing outgoing patient data from Open Dental.
			//Planmeca: Has no file paths containing outgoing patient data from Open Dental.
			//PracticeWebReports: Has no file paths containing outgoing patient data from Open Dental.
			//PreXionAcquire: Has no file paths containing outgoing patient data from Open Dental.
			//PreXionViewer: Has no file paths containing outgoing patient data from Open Dental.
			//Progeny: Has no file paths containing outgoing patient data from Open Dental.
			//PT: Per our website "The files involved get deleted immediately after they are consumed."
			//PTupdate: Per our website "The files involved get deleted immediately after they are consumed."
			//RayMage: Has no file paths containing outgoing patient data from Open Dental.
			//Schick: Has no file paths containing outgoing patient data from Open Dental.
			//Sirona:
			program=Programs.GetCur(ProgramName.Sirona);
			if(program.Enabled) {
				path=Programs.GetProgramPath(program);
				//read file C:\sidexis\sifiledb.ini
				string iniFile=Path.GetDirectoryName(path)+"\\sifiledb.ini";
				if(File.Exists(iniFile)) {
					string sendBox=ReadValueFromIni("FromStation0","File",iniFile);
					if(File.Exists(sendBox)) {
						File.WriteAllText(sendBox,"");//Clear the sendbox instead of deleting.
					}
				}
			}
			//Sopro: Has no file paths containing outgoing patient data from Open Dental.
			//ThreeShape: Has no file paths containing outgoing patient data from Open Dental.
			//TigerView:
			ScrubFileForProperty(ProgramName.TigerView,"Tiger1.ini path","",false);//C:\Program Files\PDI\Shared files\Imaging.ini.  TigerView complains if the file is not present.
			//Trojan: Has no file paths containing outgoing patient data from Open Dental.
			//Trophy: Has no file paths containing outgoing patient data from Open Dental.
			//TrophyEnhanced: Has no file paths containing outgoing patient data from Open Dental.
			//Tscan: Has no file paths containing outgoing patient data from Open Dental.
			//UAppoint: Has no file paths containing outgoing patient data from Open Dental.
			//Vipersoft: Has no file paths containing outgoing patient data from Open Dental.
			//VixWin: Has no file paths containing outgoing patient data from Open Dental.
			//VixWinBase41: Has no file paths containing outgoing patient data from Open Dental.
			//VixWinOld: Has no file paths containing outgoing patient data from Open Dental.
			//Xcharge: Has no file paths containing outgoing patient data from Open Dental.
			//XVWeb: Has no file paths containing outgoing patient data from Open Dental.
			ScrubFileForProperty(ProgramName.XDR,"InfoFile path","",true);//C:\XDRClient\Bin\infofile.txt
		}

		///<summary>Needed for Sirona bridge data scrub in ScrubExportedPatientData().</summary>
		[DllImport("kernel32")]//this is the Windows function for reading from ini files.
		private static extern int GetPrivateProfileStringFromIni(string section,string key,string def
			,StringBuilder retVal,int size,string filePath);

		///<summary>Needed for Sirona bridge data scrub in ScrubExportedPatientData().</summary>
		private static string ReadValueFromIni(string section,string key,string iniFile) {
			StringBuilder strBuild=new StringBuilder(255);
			int i=GetPrivateProfileStringFromIni(section,key,"",strBuild,255,iniFile);
			return strBuild.ToString();
		}

		///<summary>If isRemovable is false, then the file referenced in the program property will be cleared.
		///If isRemovable is true, then the file referenced in the program property will be deleted.</summary>
		private static void ScrubFileForProperty(ProgramName programName,string strFileProperty,string strFilePropertySuffix,bool isRemovable) {
			Program program=Programs.GetCur(programName);
			if(!program.Enabled) {
				return;
			}
			string strFileToScrub=CodeBase.ODFileUtils.CombinePaths(ProgramProperties.GetPropVal(program.ProgramNum,strFileProperty),strFilePropertySuffix);
			if(!File.Exists(strFileToScrub)) {
				return;
			}
			try {
				File.WriteAllText(strFileToScrub,"");//Always clear the file contents, in case deleting fails below.
			}
			catch {
				//Another instance of OD might be closing at the same time, in which case the delete will fail. Could also be a permission issue or a concurrency issue. Ignore.
			}
			if(!isRemovable) {
				return;
			}
			try {
				File.Delete(strFileToScrub);
			}
			catch {
				//Another instance of OD might be closing at the same time, in which case the delete will fail. Could also be a permission issue or a concurrency issue. Ignore.
			}
		}

		///<summary>Returns true if more than 1 credit card processing program is enabled. EdgeExpress and XCharge count as one processor.</summary>
		public static bool HasMultipleCreditCardProgramsEnabled() {
			//No need to check RemotingRole; no call to db.
			return new List<bool> {
				(IsEnabled(ProgramName.EdgeExpress) || IsEnabled(ProgramName.Xcharge)),IsEnabled(ProgramName.PayConnect),IsEnabled(ProgramName.PaySimple)
			}.Count(x => x==true) >= 2;
		}

		///<summary>Called when we want to inform HQ about changes maded to enabled programs.</summary>
		public static void SendEnabledProgramsToHQ() {
			CustomerUpdatesProxy.SendAndReceiveUpdateRequestXml();//Piggy back on this, we don't do anything with result just want to trigger some code.
		}
	}

}










