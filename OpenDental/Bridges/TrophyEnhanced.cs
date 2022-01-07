using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental.Bridges{
	/// <summary></summary>
	public class TrophyEnhanced{

		/// <summary></summary>
		public TrophyEnhanced(){
			
		}

		///<summary>Launches the program using command line.  It is confirmed that there is no space after the -P or -N</summary>
		public static void SendData(Program ProgramCur, Patient pat){
			string path=Programs.GetProgramPath(ProgramCur);
			//ArrayList ForProgram=ProgramProperties.GetForProgram(ProgramCur.ProgramNum);;
			if(pat==null){
				try{
					ODFileUtils.ProcessStart(path);//should start Trophy without bringing up at pt.
				}
				catch{
					MessageBox.Show(path+" is not available.");
				}
				return;
			}
			string storagePath=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Storage Path");
			if(!Directory.Exists(storagePath)){
				MessageBox.Show("Invalid storage path: "+storagePath);
				return;
			}
			string patFolder="";
			if(pat.TrophyFolder=="") {//no trophy folder assigned yet
				bool isNumberedMode=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Enter 1 to enable Numbered Mode")=="1";
				try{
					if(isNumberedMode){
						patFolder=AutomaticallyGetTrophyFolderNumbered(pat,storagePath);
					}
					else{
						patFolder=AutomaticallyGetTrophyFolder(pat,storagePath);
					}
				}
				catch(Exception ex){
					MessageBox.Show(ex.Message);
					return;
				}
				if(patFolder=="") {//exit without displaying any further message.
					return;
				}
				patFolder=ODFileUtils.CombinePaths(storagePath,patFolder);
			}
			else {//pat.TrophyFolder was already previously entered.
				patFolder=ODFileUtils.CombinePaths(storagePath,pat.TrophyFolder);
			}
			//can't do this because the folder won't exist yet for new patients.
			//if(!Directory.Exists(patFolder)) {
			//	MessageBox.Show("Invalid patient folder: "+patFolder);
			//	return;
			//}
			string comline="-P"+patFolder
				//We are adding a space between the last and first name of the patient because CareStream
				//was being given the information as a last name instead of a first and last name.
				//Quotes are not necessary around these command line parameters.
				+" -N"+Tidy(pat.LName)+", "+Tidy(pat.FName);
			//MessageBox.Show(comline);
			try{
				ODFileUtils.ProcessStart(path,comline);
			}
			catch{
				MessageBox.Show(path+" is not available.");
			}
		}

		///<summary>Guaranteed to always return a valid foldername unless major error or user chooses to exit.  This also saves the TrophyFolder value to this patient in the db and creates the new folder.</summary>
		private static string AutomaticallyGetTrophyFolderNumbered(Patient pat,string trophyPath) {
			//if this a patient with existing images in a trophy folder, find that folder
			//Different Trophy might? be organized differently.
			//But our logic will only cover the one situation that we are aware of.
			//In numbered mode, the folder numbering scheme is [trophyImageDir]\XX\PatNum.  The two digits XX are obtained by retrieving the 5th and 6th (indexes 4 and 5) digits of the patient's PatNum, left padded with 0's to ensure the PatNum is at least 6 digits long.  Examples: PatNum=103, left pad to 000103, substring to 03, patient's folder location is [trophyDirectory]\03\103.  PatNum=1003457, no need to left pad, substring to 45, pat folder is [trophyDirectory]\45\1003457.
			string retVal=ODFileUtils.CombinePaths(pat.PatNum.ToString().PadLeft(6,'0').Substring(4,2),pat.PatNum.ToString());//this is our default return value
			string fullPath=ODFileUtils.CombinePaths(trophyPath,retVal);
			if(!Directory.Exists(fullPath)) {
				try {
					Directory.CreateDirectory(fullPath);
				}
				catch {
					throw new Exception("Error.  Could not create folder: "+fullPath);
				}
			}
			//folder either existed before we got here, or we successfully created it
			Patient PatOld=pat.Copy();
			pat.TrophyFolder=retVal;
			Patients.Update(pat,PatOld);
			return retVal;
		}
		
		///<summary>Guaranteed to always return a valid foldername unless major error or user chooses to exit.  This also saves the TrophyFolder value to this patient in the db.</summary>
		private static string AutomaticallyGetTrophyFolder(Patient pat,string storagePath) {
			string retVal="";
			//try to find the correct trophy folder
			string rvgPortion=pat.LName.Substring(0,1)+".rvg";
			string alphaPath=ODFileUtils.CombinePaths(storagePath,rvgPortion);
			if(!Directory.Exists(alphaPath)) {
				throw new ApplicationException("Could not find expected path: "+alphaPath+".  The enhanced Trophy bridge assumes that folders already exist with that naming convention.");
			}
			DirectoryInfo dirInfo=new DirectoryInfo(alphaPath);
			DirectoryInfo[] dirArray=dirInfo.GetDirectories();
			List<TrophyFolder> listMatchesNot=new List<TrophyFolder>();//list of all patients found, all with same first letter of last name.
			List<TrophyFolder> listMatchesName=new List<TrophyFolder>();//list of all perfect matches for name but not birthdate.
			TrophyFolder folder;
			string maxFolderName="";
			string datafilePath;
			string[] datafileLines;
			string date;
			//loop through each folder.
			for(int i=0;i<dirArray.Length;i++) {
				if(String.Compare(dirArray[i].Name,maxFolderName) > 0) {//eg, if G0000035 > G0000024
					maxFolderName=dirArray[i].Name;
				}
				datafilePath=ODFileUtils.CombinePaths(dirArray[i].FullName,"FILEDATA.txt");
				if(!File.Exists(datafilePath)){
					continue;//fail silently.
				}
				//if this folder is already in use by some other patient, then skip
				if(Patients.IsTrophyFolderInUse(dirArray[i].Name)) {
					continue;
				}
				folder=new TrophyFolder();
				folder.FolderName=dirArray[i].Name;
				datafileLines=File.ReadAllLines(datafilePath);
				if(datafileLines.Length<2) {
					continue;
				}
				folder.FName=GetValueFromLines("PRENOM",datafileLines);
				folder.LName=GetValueFromLines("NOM",datafileLines);
				date=GetValueFromLines("DATE",datafileLines);
				try{
					folder.BirthDate=DateTime.ParseExact(date,"yyyyMMdd",CultureInfo.CurrentCulture.DateTimeFormat);
				}
				catch{}
				if(pat.LName.ToUpper()==folder.LName.ToUpper() && pat.FName.ToUpper()==folder.FName.ToUpper()) {
					if(pat.Birthdate==folder.BirthDate) {
						//We found a perfect match here, so do not display any dialog to user.
						retVal=rvgPortion+@"\"+dirArray[i].Name;
					}
					else{//name is perfect match, but not birthdate.  Maybe birthdate was not entered in one system or the other.
						listMatchesName.Add(folder);
					}
				}
				listMatchesNot.Add(folder);
			}
			if(retVal=="") {//perfect match not found
				if(listMatchesName.Count==1) {//exactly one name matched even though birthdays did not
					retVal=rvgPortion+@"\"+listMatchesName[0].FolderName;
				}
				else{//no or multiple matches
					using FormTrophyNamePick formPick=new FormTrophyNamePick();
					formPick.ListMatches=listMatchesNot;
					formPick.ShowDialog();
					if(formPick.DialogResult!=DialogResult.OK) {
						return "";//triggers total exit
					}
					if(formPick.PickedName=="") {//Need to generate new folder name
						int maxInt=0;
						if(maxFolderName!="") {
							maxInt=PIn.Int(maxFolderName.Substring(1));//It will crash here if can't parse the int.
						}
						maxInt++;
						string paddedInt=maxInt.ToString().PadLeft(7,'0');
						retVal=rvgPortion+@"\"+pat.LName.Substring(0,1).ToUpper()+paddedInt;
					}
					else {
						retVal=rvgPortion+@"\"+formPick.PickedName;
					}
				}
			}
			Patient patOld=pat.Copy();
			pat.TrophyFolder=retVal;
			Patients.Update(pat,patOld);
			return retVal;
		}

		private static string GetValueFromLines(string keyName,string[] lines) {
			for(int i=0;i<lines.Length;i++) {
				if(lines[i].StartsWith(keyName)) {
					int equalsPos=lines[i].IndexOf('=');
					if(equalsPos==-1) {
						return "";
					}
					if(lines[i].Length <= equalsPos+1){//eg, L=4, equalsPos=3 (last char). Nothing comes after =.
						return "";
					}
					string retVal=lines[i].Substring(equalsPos+1);
					retVal=retVal.TrimStart(' ');
					return retVal;
				}
			}
			return "";
		}

		private static string Tidy(string str) {
			string retVal=str.Replace("\"","");//gets rid of any quotes
			retVal=retVal.Replace("'","");//gets rid of any single quotes
			return retVal;
		}

	}

	public class TrophyFolder {
		///<summary>The simple folder name, like G000002, without the .rvg\</summary>
		public string FolderName;
		public string LName;
		public string FName;
		public DateTime BirthDate;
	}
}










