using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using DataConnectionBase;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenDental{

	///<summary></summary>
	public partial class ClassConvertDatabase {
		private System.Version FromVersion;
		private System.Version ToVersion;

		///<summary>Return false to indicate exit app.  Only called when program first starts up at the beginning of FormOpenDental.PrefsStartup.</summary>
		public bool Convert(string fromVersion,string toVersion,bool isSilent,Form currentForm,bool useDynamicMode) {
			FromVersion=new Version(fromVersion);
			ToVersion=new Version(toVersion);
			if(FromVersion>=new Version("3.4.0") && PrefC.GetBool(PrefName.CorruptedDatabase)) {
				FormOpenDental.ExitCode=201;//Database was corrupted due to an update failure
				if(!isSilent) {
					MsgBox.Show(this,"Your database is corrupted because an update failed.  Please contact us.  This database is unusable and you will need to restore from a backup.");
				}
				return false;//shuts program down.
			}
			//There was a 19.3.0 convert method released in the 19.2.3 version.
			//We have to treat 19.3.0 as 19.2.3 so newer convert methods will run.
			if(!ODBuild.IsDebug() && FromVersion.ToString()=="19.3.0.0") {
				FromVersion=new Version("19.2.3.0");
				ODException.SwallowAnyException(() => Prefs.UpdateString(PrefName.DataBaseVersion,"19.2.3.0"));
			}
			if(FromVersion==ToVersion) {
				return true;//no conversion necessary
			}
			if(FromVersion.CompareTo(ToVersion)>0){//"Cannot convert database to an older version."
				//no longer necessary to catch it here.  It will be handled soon enough in CheckProgramVersion
				return true;
			}
			if(FromVersion < new Version("2.8.0")) {
				FormOpenDental.ExitCode=130;//Database must be upgraded to 2.8 to continue
				if(!isSilent) {
					MsgBox.Show(this,"This database is too old to easily convert in one step. Please upgrade to 2.1 if necessary, then to 2.8.  Then you will be able to upgrade to this version. We apologize for the inconvenience.");
				}
				return false;
			}
			if(FromVersion < new Version("6.6.2")) {
				FormOpenDental.ExitCode=131;//Database must be upgraded to 11.1 to continue
				if(!isSilent) {
					MsgBox.Show(this,"This database is too old to easily convert in one step. Please upgrade to 11.1 first.  Then you will be able to upgrade to this version. We apologize for the inconvenience.");
				}
				return false;
			}
			if(FromVersion < new Version("3.0.1")) {
				if(!isSilent) {
					MsgBox.Show(this,"This is an old database.  The conversion must be done using MySQL 4.1 (not MySQL 5.0) or it will fail.");
				}
			}
			if(FromVersion.ToString()=="2.9.0.0" || FromVersion.ToString()=="3.0.0.0" || FromVersion.ToString()=="4.7.0.0") {
				FormOpenDental.ExitCode=190;//Cannot convert this database version which was only for development purposes
				if(!isSilent) {
					MsgBox.Show(this,"Cannot convert this database version which was only for development purposes.");
				}
				return false;
			}
			if(FromVersion > new Version("4.7.0") && FromVersion.Build==0) {
				FormOpenDental.ExitCode=190;//Cannot convert this database version which was only for development purposes
				if(!isSilent) {
					MsgBox.Show(this,"Cannot convert this database version which was only for development purposes.");
				}
				return false;
			}
			if(FromVersion >= ConvertDatabases.LatestVersion) {
				return true;//no conversion necessary
			}
			//Trial users should never be able to update a database.
			if(ODBuild.IsTrial() && PrefC.GetString(PrefName.RegistrationKey)!="") {//Allow databases with no reg key to update.  Needed by our conversion department.
				FormOpenDental.ExitCode=191;//Trial versions cannot connect to live databases
				if(!isSilent) {
					MsgBox.Show(this,"Trial versions cannot connect to live databases.  Please run the Setup.exe in the AtoZ folder to reinstall your original version.");
				}
				return false;
			}
			string webServiceServerName=PrefC.GetString(PrefName.WebServiceServerName);
			//If the WebServiceServerName name is not set and they are using dynamic mode, continue on to download the binaries in CheckProgramVersion.
			//Or, if they do have the WebServiceServerName set and this is not the computer, continue on to CheckProgramVersion.
			if((webServiceServerName=="" && useDynamicMode)
				|| (webServiceServerName!="" && !ODEnvironment.IdIsThisComputer(webServiceServerName.ToLower())))
			{
				if(isSilent) {
					FormOpenDental.ExitCode=141;//Updates are only allowed from a designated web server
					return false;//if you are in debug mode and you really need to update the DB, you can manually clear the WebServiceServerName preference.
				}
				//This will be handled in CheckProgramVersion, giving the user option to downgrade or exit program.
				return true;
			}
			//At this point, the current instance of the program has the ability to execute an upgrade.
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				FormOpenDental.ExitCode=140;//Web client cannot convert database
				if(!isSilent) {
					MsgBox.Show(this,"Web client cannot convert database.  Must be using a direct connection.");
				}
				return false;
			}
			if(ReplicationServers.ServerIsBlocked()) {
				FormOpenDental.ExitCode=150;//Replication server is blocked from performing updates
				if(!isSilent) {
					MsgBox.Show(this,"This replication server is blocked from performing updates.");
				}
				return false;
			}
			//if database tables are in different storage engine formats, try to fix it by converting all tables to the default storage engine format
			string defaultStorageEngine=DatabaseMaintenances.GetStorageEngineDefaultName();
			bool hasBackedUp=false;
			if(DataConnection.DBtype==DatabaseType.MySql) {//not for Oracle
				Dictionary<string,List<string>> dictEngineTableNames=DatabaseMaintenances.GetTableEngineTableNames().Select()
					.GroupBy(x => PIn.String(x["ENGINE"].ToString()),x => PIn.String(x["TABLE_NAME"].ToString()))
					.ToDictionary(x => x.Key,x => x.ToList());
				if(dictEngineTableNames.Keys.Count>1) {
					if(!isSilent) {
						string engineNames=dictEngineTableNames.Keys.Count==2?string.Join(" and ",dictEngineTableNames.Keys)
							:(string.Join(", ",dictEngineTableNames.Keys.Take(dictEngineTableNames.Keys.Count-1))+", and "+dictEngineTableNames.Keys.Last());
						List<string> listNonDefaultEngineTableNames=dictEngineTableNames
							.Where(x => x.Key.ToUpper()!=defaultStorageEngine.ToUpper())
							.SelectMany(x => x.Value).ToList();
						string msgText=Lan.g(this,"A mixture of database tables in")+" "+engineNames+" "
							+Lan.g(this,"format were found. A database backup will now be made and then the following tables will be converted to")+" "
							+defaultStorageEngine+" "+Lan.g(this,"format")+":\r\n"+string.Join(", ",listNonDefaultEngineTableNames);
						if(MessageBox.Show(msgText,"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
							return false;
						}
					}
					hasBackedUp=Shared.MakeABackup(isSilent,BackupLocation.ConvertScript,false);
					if(!hasBackedUp) {
						Cursor.Current=Cursors.Default;
						FormOpenDental.ExitCode=101;//Database Backup failed
						return false;
					}
					if(!DatabaseMaintenances.ConvertToDefaultEngine()) {
						FormOpenDental.ExitCode=102;//Failed to convert tables to the default storage engine format
						if(!isSilent) {
							MessageBox.Show(Lan.g(this,"Failed to convert tables to")+" "+defaultStorageEngine+" "+Lan.g(this,"format. Please contact support."));
						}
						return false;
					}
					if(!isSilent) {
						MessageBox.Show(Lan.g(this,"All tables converted to")+" "+defaultStorageEngine+" "+Lan.g(this,"format successfully."));
					}
				}
				else if(dictEngineTableNames.Keys.Count==1) {
					if(dictEngineTableNames.First().Key.ToUpper()!=defaultStorageEngine.ToUpper()) {
						FormOpenDental.ExitCode=103;//Default database .ini setting is different than all tables
						if(!isSilent) {
							MessageBox.Show(Lan.g(this,"The database tables are in")+" "+dictEngineTableNames.First().Key+" "
								+Lan.g(this,"format, but the default storage engine format is")+" "+defaultStorageEngine+". "
								+Lan.g(this,"You must change the default storage engine within the my.ini (or my.cnf) file on the database server and restart MySQL "
									+"in order to fix this problem. Exiting."));
						}
						return false;
					}
				}
			}
			if(ODBuild.IsDebug()) {
				if(!isSilent && MessageBox.Show("You are in Debug mode.  Your database can now be converted"+"\r"
					+"from version"+" "+FromVersion.ToString()+"\r"
					+"to version"+" "+ToVersion.ToString()+"\r"
					+"You can click Cancel to skip conversion and attempt to run the newer code against the older database."
					,"",MessageBoxButtons.OKCancel)!=DialogResult.OK)
				{
					return true;//If user clicks cancel, then do nothing
				}
			}
			else {//release
				if(!isSilent && MessageBox.Show(Lan.g(this,"Your database will now be converted")+"\r"
					+Lan.g(this,"from version")+" "+FromVersion.ToString()+"\r"
					+Lan.g(this,"to version")+" "+ToVersion.ToString()+"\r"
					+Lan.g(this,"The conversion works best if you are on the server.  Depending on the speed of your computer, it can be as fast as a "
						+"few seconds, or it can take as long as 10 minutes.")
					+(!hasBackedUp && defaultStorageEngine.ToUpper()!="MYISAM"?("\r"+Lan.g(this,"The backup tables will be MyISAM format instead of")+" "+defaultStorageEngine+"."):"")
					,"",MessageBoxButtons.OKCancel)!=DialogResult.OK)
				{
					return false;//If user clicks cancel, then close the program
				}
			}
#if !DEBUG
			if(!isSilent) {
				if(DataConnection.DBtype!=DatabaseType.MySql
				&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,"If you have not made a backup, please Cancel and backup before continuing.  Continue?")) {
					return false;
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql && !hasBackedUp) {
				if(!Shared.MakeABackup(isSilent,BackupLocation.ConvertScript,false)) {
					Cursor.Current=Cursors.Default;
					FormOpenDental.ExitCode=101;//Database Backup failed
					return false;
				}
			}
			//We've been getting an increasing number of phone calls with databases that have duplicate preferences which is impossible
			//unless a user has gotten this far and another computer in the office is in the middle of an update as well.
			//The issue is most likely due to the blocking messageboxes above which wait indefinitely for user input right before upgrading the database.
			//This means that the cache for this computer could be stale and we need to manually refresh our cache to double check 
			//that the database isn't flagged as corrupt, an update isn't in progress, or that the database version hasn't changed (someone successfully updated already).
			Prefs.RefreshCache();
			//Now check the preferences that should stop this computer from executing an update.
			if(PrefC.GetBool(PrefName.CorruptedDatabase) 
				|| (PrefC.GetString(PrefName.UpdateInProgressOnComputerName)!="" && PrefC.GetString(PrefName.UpdateInProgressOnComputerName)!=ODEnvironment.MachineName))
			{
				//At this point, the pref "corrupted database" being true means that a computer is in the middle of running the upgrade script.
				//There will be another corrupted database check on start up which will take care of the scenario where this is truly a corrupted database.
				//Also, we need to make sure that the update in progress preference is set to this computer because we JUST set it to that value before entering this method.
				//If it has changed, we absolutely know without a doubt that another computer is trying to update at the same time.
				FormOpenDental.ExitCode=142;//Update is already in progress from another computer
				if(!isSilent) {
					MsgBox.Show(this,"An update is already in progress from another computer.");
				}
				return false;
			}
			//Double check that the database version has not changed.  This check is here just in case another computer has successfully updated the database already.
			Version versionDatabase=new Version(PrefC.GetString(PrefName.DataBaseVersion));
			if(FromVersion!=versionDatabase) {
				FormOpenDental.ExitCode=143;//Database has already been updated from another computer
				if(!isSilent) {
					MsgBox.Show(this,"The database has already been updated from another computer.");
				}
				return false;
			}
			try {
#endif
				if(FromVersion < new Version("7.5.17")) {//Insurance Plan schema conversion
					if(isSilent) {
						FormOpenDental.ExitCode=139;//Update must be done manually to fix Insurance Plan Schema
						Application.Exit();
						return false;
					}
					Cursor.Current=Cursors.Default;
					YN InsPlanConverstion_7_5_17_AutoMergeYN=YN.Unknown;
					if(FromVersion < new Version("7.5.1")) {
						using FormInsPlanConvert_7_5_17 form=new FormInsPlanConvert_7_5_17();
						if(PrefC.GetBoolSilent(PrefName.InsurancePlansShared,true)) {
							form.InsPlanConverstion_7_5_17_AutoMergeYN=YN.Yes;
						}
						else {
							form.InsPlanConverstion_7_5_17_AutoMergeYN=YN.No;
						}
						form.ShowDialog();
						if(form.DialogResult==DialogResult.Cancel) {
							MessageBox.Show("Your database has not been altered.");
							return false;
						}
						InsPlanConverstion_7_5_17_AutoMergeYN=form.InsPlanConverstion_7_5_17_AutoMergeYN;
					}
					ConvertDatabases.Set_7_5_17_AutoMerge(InsPlanConverstion_7_5_17_AutoMergeYN);//does nothing if this pref is already present for some reason.
					Cursor.Current=Cursors.WaitCursor;
				}
				if(!isSilent && FromVersion>new Version("16.3.0") && FromVersion<new Version("16.3.29") && ApptReminderRules.UsesApptReminders()) {
					//16.3.29 is more strict about reminder rule setup. Prompt the user and allow them to exit the update if desired.
					//Get all currently enabled reminder rules.
					List<bool> listReminderFlags=ApptReminderRules.Get_16_3_29_ConversionFlags();
					if(listReminderFlags?[0]??false) { //2 reminders scheduled for same day of appointment. 1 will be converted to future day reminder.
						MsgBox.Show(this,"You have multiple appointment reminders set to send on the same day of the appointment. One of these will be converted to send 1 day prior to the appointment.  Please review automated reminder rule setup after update has finished.");
					}
					if(listReminderFlags?[1]??false) { //2 reminders scheduled for future day of appointment. 1 will be converted to same day reminder.
						MsgBox.Show(this,"You have multiple appointment reminders set to send 1 or more days prior to the day of the appointment. One of these will be converted to send 1 hour prior to the appointment.  Please review automated reminder rule setup after update has finished.");
					}
				}
				if(FromVersion>=new Version("17.3.1") && FromVersion<new Version("17.3.23") && DataConnection.DBtype==DatabaseType.MySql
					&& (Tasks.HasAnyLongDescripts() || TaskNotes.HasAnyLongNotes() || Commlogs.HasAnyLongNotes())) 
				{
					if(isSilent) {
						FormOpenDental.ExitCode=138;//Update must be done manually in order to get data loss notification(s).
						Application.Exit();
						return false;
					}
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Data will be lost during this update."
						+"\r\nContact support in order to retrieve the data from a backup after the update."
						+"\r\n\r\nContinue?"))
					{
						MessageBox.Show("Your database has not been altered.");
						return false;
					}
				}
				if(FromVersion>=new Version("3.4.0")) {
					Prefs.UpdateBool(PrefName.CorruptedDatabase,true);
				}
				ConvertDatabases.FromVersion=FromVersion;
				if(!ODBuild.IsDebug()) {
					//Typically the UpdateInProgressOnComputerName preference will have already been set within FormUpdate.
					//However, the user could have cancelled out of FormUpdate after successfully downloading the Setup.exe
					//OR the Setup.exe could have been manually sent to our customer (during troubleshooting with HQ).
					//For those scenarios, the preference will be empty at this point and we need to let other computers know that an update going to start.
					//Updating the string (again) here will guarantee that all computers know an update is in fact in progress from this machine.
					Prefs.UpdateString(PrefName.UpdateInProgressOnComputerName,ODEnvironment.MachineName);
				}
				//Currently okay to show progress during isSilent.
				UI.ProgressOD progressOD=new UI.ProgressOD();
				progressOD.ActionMain=() => ConvertDatabases.InvokeConvertMethods();
				progressOD.ShowCancelButton=false;
				progressOD.TypeEvent=typeof(ODEvent);
				progressOD.ODEventType=ODEventType.ConvertDatabases;
				progressOD.ShowDialogProgress();
				if(FromVersion>=new Version("3.4.0")) {
					//CacheL.Refresh(InvalidType.Prefs);//or it won't know it has to update in the next line.
					Prefs.UpdateBool(PrefName.CorruptedDatabase,false,true);//more forceful refresh in order to properly change flag
				}
				//Specific caches should be invalid after convert script update.
				Cache.Refresh(InvalidType.Prefs,InvalidType.Programs);
				return true;
#if !DEBUG
			}
			catch(System.IO.FileNotFoundException e) {
				FormOpenDental.ExitCode=160;//File not found exception
				if(!isSilent) {
					MessageBox.Show(e.FileName+" "+Lan.g(this,"could not be found. Your database has not been altered and is still usable if you uninstall this version, then reinstall the previous version."));
				}
				if(FromVersion>=new Version("3.4.0")) {
					Prefs.UpdateBool(PrefName.CorruptedDatabase,false);
				}
				return false;
			}
			catch(System.IO.DirectoryNotFoundException) {
				FormOpenDental.ExitCode=160;//ConversionFiles folder could not be found
				if(!isSilent) {
					MessageBox.Show(Lan.g(this,"ConversionFiles folder could not be found. Your database has not been altered and is still usable if you uninstall this version, then reinstall the previous version."));
				}
				if(FromVersion>=new Version("3.4.0")) {
					Prefs.UpdateBool(PrefName.CorruptedDatabase,false);
				}
				return false;
			}
			catch(Exception ex) {
				FormOpenDental.ExitCode=201;//Database was corrupted due to an update failure
				if(!isSilent) {
					MessageBox.Show(ex.Message+"\r\n\r\n"
						+Lan.g(this,"Conversion unsuccessful. Your database is now corrupted and you cannot use it.  Please contact us."));
				}
				//Then, application will exit, and database will remain tagged as corrupted.
				return false;
			}
#endif
		}

	}

}