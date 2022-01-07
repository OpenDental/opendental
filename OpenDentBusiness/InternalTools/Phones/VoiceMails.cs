using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness{
	///<summary></summary>
	public class VoiceMails{

		///<summary>The PatNum for 'Misc.' which is the patient we attach voice mails to if the phone number is not found.
		///Needs to be public because other projects (e.g. PhoneTrackingServer) use this constant.</summary>
		public const long MiscPatNum=77682;

		///<summary>Gets all voice mails.</summary>
		public static List<VoiceMail> GetAll(bool includeNonDbColumns=true,bool includeDeleted=false) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<VoiceMail>>(MethodBase.GetCurrentMethod(),includeNonDbColumns,includeDeleted);
			}
			string command;
			if(!includeNonDbColumns) {
				command="SELECT * FROM voicemail ";
				if(!includeDeleted) {
					command+="WHERE StatusVM != "+POut.Int((int)VoiceMailStatus.Deleted);
				}
				return Crud.VoiceMailCrud.SelectMany(command);
			}
			command=@"SELECT voicemail.*,
				COALESCE(userod.UserName,'') UserName,
				COALESCE(patient.LName,'') LName,
				COALESCE(patient.FName,'') FName,
				COALESCE(patient.MiddleI,'') MiddleI,
				COALESCE(patient.Preferred,'') Preferred
				FROM voicemail
				LEFT JOIN userod ON userod.UserNum=voicemail.UserNum
				LEFT JOIN patient ON patient.PatNum=voicemail.PatNum ";
			if(!includeDeleted) {
				command+="WHERE StatusVM != "+POut.Int((int)VoiceMailStatus.Deleted);
			}
			DataTable table=Db.GetTable(command);
			List<VoiceMail> listVoiceMails=Crud.VoiceMailCrud.TableToList(table);
			for(int i=0;i<table.Rows.Count;i++) {
				listVoiceMails[i].UserName=PIn.String(table.Rows[i]["UserName"].ToString());
				Patient patient=new Patient() {
					LName=PIn.String(table.Rows[i]["LName"].ToString()),
					FName=PIn.String(table.Rows[i]["FName"].ToString()),
					Preferred=PIn.String(table.Rows[i]["Preferred"].ToString()),
					MiddleI=PIn.String(table.Rows[i]["MiddleI"].ToString())
				};
				patient.LName=string.IsNullOrEmpty(patient.LName) ? Lans.g("VoiceMails","(Multiple)") : patient.LName;
				listVoiceMails[i].PatientName=patient.GetNameLF();
			}
			return listVoiceMails;
		}

		///<summary>Gets one VoiceMail from the db.</summary>
		public static VoiceMail GetOne(long voiceMailNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<VoiceMail>(MethodBase.GetCurrentMethod(),voiceMailNum);
			}
			return Crud.VoiceMailCrud.SelectOne(voiceMailNum);
		}

		///<summary>Returns the PatNum for the patient with a matching phone number. If no patient is found, returns the PatNum for 'Misc.'
		///If more than one patient is found, returns 0. Called from PhoneTrackingServer.</summary>
		public static long GetPatNumForPhone(string phoneNumber) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),phoneNumber);
			}
			if(phoneNumber.Count(x => char.IsDigit(x))==0) {
				return MiscPatNum;
			}
			string phoneRegex=string.Join("[^0-9]*",phoneNumber.Where(x=>char.IsDigit(x)));//So that any intervening non-digit characters will still match
			if(phoneNumber.Count(x => char.IsDigit(x))>3) {//If there is more than three digits 
				if(phoneRegex.StartsWith("1")) {//and the first digit is 1, make it optional.
					phoneRegex="1?"+phoneRegex.Substring(1);
				}
				else {
					phoneRegex="1?[^0-9]*"+phoneRegex;//add a leading 1 so that 1-800 numbers can show up simply by typing in 800 followed by the number.
				}
			}
			string command=@"SELECT DISTINCT patient.PatNum 
				FROM patient 
				LEFT JOIN phonenumber ON phonenumber.PatNum=patient.PatNum
				WHERE (patient.HmPhone REGEXP '"+POut.String(phoneRegex)+@"' 
				OR patient.WkPhone REGEXP '"+POut.String(phoneRegex)+@"' 
				OR patient.WirelessPhone REGEXP '"+POut.String(phoneRegex)+@"' 
				OR phonenumber.PhoneNumberVal REGEXP '"+POut.String(phoneRegex)+@"')
				AND patient.PatStatus!="+POut.Int((int)PatientStatus.Deleted);
			List<long> listPatNums=Db.GetListLong(command);
			if(listPatNums.Count==0) {
				return MiscPatNum;
			}
			if(listPatNums.Count>1) {
				return 0;
			}
			return listPatNums[0];
		}

		///<summary>Gets the Voice Mail Origination path for this current computer.</summary>
		public static string GetVoiceMailOriginationPath() {
			try {
				//The VoiceMailOriginationPath preference stores a list of KeyValuePairs where the key is the name of the computer and the value
				//is the path. When JSON-serialized it looks like this:
				//[{"Key":"SERVER153","Value":"\\\\110.10.6.249\\Voicemail\\default\\998\\INBOX"},{"Key":"CHRISM","Value":"C:\\development"}]
				List<KeyValuePair<string,string>> listPaths=JsonConvert.DeserializeObject<List<KeyValuePair<string,string>>>
					(PrefC.GetString(PrefName.VoiceMailOriginationPath));
				foreach(KeyValuePair<string,string> kvp in listPaths) {
					if(kvp.Key==Environment.MachineName) {
						return kvp.Value;
					}
				}
				return listPaths[0].Value;//Return the first one in the list.
			}
			catch(Exception ex) {
				ex.DoNothing();
				return Phones.PathPhoneMsg;//If all else fails, return a hard-coded path.
			}
		}

		///<summary>Updates the Voice Mail Origination path for this current computer. Returns true if the preference was changed.
		///This is the ONLY PLACE where the VoiceMailOriginationPath preference should be modified.</summary>
		public static bool UpdateVoiceMailOriginationPath(string path) {
			List<KeyValuePair<string,string>> listPaths;
			try {
				//The VoiceMailOriginationPath preference stores a list of KeyValuePairs where the key is the name of the computer and the value
				//is the path. When JSON-serialized it looks like this:
				//[{"Key":"SERVER153","Value":"\\\\110.10.6.249\\Voicemail\\default\\998\\INBOX"},{"Key":"CHRISM","Value":"C:\\development"}]
				listPaths=JsonConvert.DeserializeObject<List<KeyValuePair<string,string>>>
					(PrefC.GetString(PrefName.VoiceMailOriginationPath));
			}
			catch(Exception ex) {//If the preference value was not a well formed JSON list, start over with a new list.
				ex.DoNothing();
				listPaths=new List<KeyValuePair<string,string>>();
			}
			bool didUpdatePath=false;
			for(int i=0;i<listPaths.Count;i++) {
				if(listPaths[i].Key==Environment.MachineName) {
					listPaths[i]=new KeyValuePair<string,string>(listPaths[i].Key,path);
					didUpdatePath=true;
				}
			}
			if(!didUpdatePath) {
				listPaths.Add(new KeyValuePair<string,string>(Environment.MachineName,path));
			}
			return Prefs.UpdateString(PrefName.VoiceMailOriginationPath,JsonConvert.SerializeObject(listPaths));
		}

		///<summary>Deletes all the voice mails and their corresponding files that have a status of Deleted and a DateCreated that is before the passed 
		///in date. Called from PhoneTrackingServer.</summary>
		public static void DeleteBefore(DateTime dateBefore) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dateBefore);
				return;
			}
			string command=@"SELECT * FROM voicemail 
				WHERE StatusVM="+POut.Int((int)VoiceMailStatus.Deleted)+@"
				AND "+DbHelper.DtimeToDate("DateCreated")+"<"+POut.Date(dateBefore);
			List<VoiceMail> listVoiceMails=Crud.VoiceMailCrud.SelectMany(command);
			Exception firstEx=null;
			foreach(VoiceMail voiceMail in listVoiceMails) {
				try {
					Delete(voiceMail);//Also deletes the files
				}
				catch(Exception ex) {
					firstEx=firstEx??ex;
				}
			}
			if(firstEx!=null) {
				throw firstEx;
			}
		}

		///<summary>Called from PhoneTrackingServer.</summary>
		public static long Insert(VoiceMail voiceMail) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				voiceMail.VoiceMailNum=Meth.GetLong(MethodBase.GetCurrentMethod(),voiceMail);
				return voiceMail.VoiceMailNum;
			}
			return Crud.VoiceMailCrud.Insert(voiceMail);
		}

		///<summary></summary>
		public static void Update(VoiceMail voiceMail) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),voiceMail);
				return;
			}
			Crud.VoiceMailCrud.Update(voiceMail);
		}

		///<summary></summary>
		public static void Update(VoiceMail voiceMail,VoiceMail voiceMailOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),voiceMail,voiceMailOld);
				return;
			}
			Crud.VoiceMailCrud.Update(voiceMail,voiceMailOld);
		}

		///<summary>Changes the voice mail's status to Deleted and moves the .wav file into the Archived folder. Permanently deletes the .txt and .ogg
		///files.  Throws exceptions purposefully.</summary>
		public static void Archive(VoiceMail voiceMail) {
			//No need to check RemotingRole; no call to db.
			voiceMail.StatusVM=VoiceMailStatus.Deleted;
			string oldFileName=voiceMail.FileName;
			voiceMail.FileName="";//So that we don't end up with an incorrect file name if the stuff below fails.
			Update(voiceMail);//Updating now in case the file I/O stuff fails.
			Signalods.Insert(new Signalod { IType=InvalidType.VoiceMails });
			//Move the .wav file to the Archived folder.
			string archivePath=PrefC.GetString(PrefName.VoiceMailArchivePath);
			string newFileName;
			if(PrefC.GetBool(PrefName.VoiceMailSMB2Enabled)) {
				//This is brutal because we need to create a network connection to the archive path but the path might not exist.
				//We cannot check if the directory exists and we cannot create the directory without first making a network connection to it (or parent dir).
				//Therefore, instead of coding some sort of directory waterfall loop, I'm simply going to let the exception throw and we'll manually create it.
				using(new ODNetworkConnection(archivePath,PrefC.VoiceMailNetworkCredentialsSMB2)) {
					//Now that we have a network connection to the "parent" archive path we can do file IO like usual (assuming we have permission to).
					newFileName=ArchiveFile(oldFileName);
				}
			}
			else {
				newFileName=ArchiveFile(oldFileName);
			}
			voiceMail.FileName=newFileName;
			Update(voiceMail);
		}

		///<summary>Moves the file to the archive location.</summary>
		private static string ArchiveFile(string oldFileName) {
			string archivePath=PrefC.GetString(PrefName.VoiceMailArchivePath);
			string archivePathToday=Path.Combine(archivePath,DateTime.Now.ToString("MM-dd-yy"));
			if(!Directory.Exists(archivePathToday)) {
				Directory.CreateDirectory(archivePathToday);
			}
			string newFileName=Path.Combine(archivePathToday,"msg"+Security.CurUser.UserNum+"_"+DateTime.Now.ToString("MM_dd_yy_H_mm_ss_fff")+".wav");
			//Delete the .txt file.
			if(File.Exists(oldFileName.Replace(".wav",".txt"))) {
				File.Delete(oldFileName.Replace(".wav",".txt"));
			}
			File.Move(oldFileName,newFileName);
			return newFileName;
		}

		///<summary>Also deletes the voice mail file. Throws exceptions.</summary>
		public static void Delete(VoiceMail voiceMail) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),voiceMail);
				return;
			}
			if(PrefC.GetBool(PrefName.VoiceMailSMB2Enabled)) {
				string remoteName=PrefC.GetString(PrefName.VoiceMailArchivePath);
				if(!voiceMail.FileName.StartsWith(remoteName) && voiceMail.FileName!="") {
					remoteName=Path.GetDirectoryName(voiceMail.FileName);
				}
				//This is brutal because we need to create a network connection to the archive path but the path might not exist.
				//We cannot check if the directory exists without first making a network connection to it (or a parent dir).
				//Therefore, instead of coding some sort of directory waterfall loop, I'm simply going to let the exception throw and we'll manually create it.
				using(new ODNetworkConnection(remoteName,PrefC.VoiceMailNetworkCredentialsSMB2)) {
					//Now that we have a network connection to the directory that the file supposedly resides we can check to see if the file in fact exists.
					if(File.Exists(voiceMail.FileName)) {
						File.Delete(voiceMail.FileName);
					}
				}
			}
			else {
				if(File.Exists(voiceMail.FileName)) {
					File.Delete(voiceMail.FileName);
				}
			}
			Crud.VoiceMailCrud.Delete(voiceMail.VoiceMailNum);
		}


		///<summary>Returns true if the heartbeat is less than 7 seconds old. The VoiceMailMonitorHeartBeat gets updated every 3 seconds. Also returns the date time of the heartbeat.</summary>
		public static ODTuple<bool,DateTime> IsVoicemailMonitorHeartbeatValid(DateTime dateTimeLastHeartbeat) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ODTuple<bool,DateTime>>(MethodBase.GetCurrentMethod(),dateTimeLastHeartbeat);
			}
			//Default to using our local time just in case we can't query MySQL every second (lessens false positives due to query / network failure).
			DateTime dateTimeNow=DateTime.Now;
			DateTime dateTimeRecentHeartbeat=dateTimeLastHeartbeat;
			DataTable table=null;
			//Check to make sure the voicemail monitor is still up
			ODException.SwallowAnyException(() => {
				table=DataCore.GetTable("SELECT ValueString,NOW() DateTNow FROM preference WHERE PrefName='VoiceMailMonitorHeartBeat'");
			});
			if(table!=null && table.Rows.Count>=1 && table.Columns.Count>=2) {
				dateTimeRecentHeartbeat=PIn.DateT(table.Rows[0]["ValueString"].ToString());
				dateTimeNow=PIn.DateT(table.Rows[0]["DateTNow"].ToString());
			}
			//Check to see if the voicemail monitor heartbeat has stopped beating for the last 7 seconds.
			if((dateTimeNow-dateTimeRecentHeartbeat).TotalSeconds > 7) {
				return new ODTuple<bool,DateTime>(false,dateTimeRecentHeartbeat);
			}
			return new ODTuple<bool,DateTime>(true,dateTimeRecentHeartbeat);
		}
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<VoiceMail> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<VoiceMail>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM voicemail WHERE PatNum = "+POut.Long(patNum);
			return Crud.VoiceMailCrud.SelectMany(command);
		}

		

		
		*/



	}
}