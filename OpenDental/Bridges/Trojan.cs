using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using OpenDentBusiness;
using System.Drawing;
using System.Collections.ObjectModel;
using CodeBase;
using System.Globalization;
using System.Linq;

namespace OpenDental.Bridges {
	class Trojan {
		private static Collection<string[]> deletePatientRecords;
		private static Collection<string[]> deleteTrojanRecords;
		private static DataTable pendingDeletionTable;
		private static DataTable pendingDeletionTableTrojan;

		public static void StartupCheck(){
			//Skip all if not using Trojan.
			Program ProgramCur=Programs.GetCur(ProgramName.Trojan);
			if(!Programs.IsEnabledByHq(ProgramCur,out _) || !ProgramCur.Enabled || ODBuild.IsWeb()) {
				return;
			}
			//Ensure that Trojan has a sane install.
			RegistryKey regKey=Registry.LocalMachine.OpenSubKey("Software\\TROJAN BENEFIT SERVICE");
			string file="";
			if(ODBuild.IsDebug()) {
				file=@"C:\Trojan\ETW\";
				ProcessDeletedPlans(file+@"DELETEDPLANS.TXT");
				ProcessTrojanPlanUpdates(file+@"ALLPLANS.TXT");
			}
			else {
				if(regKey==null || regKey.GetValue("INSTALLDIR")==null) {
					//jsparks: The below is wrong.  The user should create a registry key manually.
					return;
					//The old trojan registry key is missing. Try to locate the new Trojan registry key.
					//regKey=Registry.LocalMachine.OpenSubKey("Software\\Trojan Eligibility");
					//if(regKey==null||regKey.GetValue("INSTALLDIR")==null) {//Unix OS will exit here.
					//	return;
					//}
				}
				//Process DELETEDPLANS.TXT for recently deleted insurance plans.
				file=regKey.GetValue("INSTALLDIR").ToString()+@"\DELETEDPLANS.TXT";//C:\ETW\DELETEDPLANS.TXT

				ProcessDeletedPlans(file);
				//Process ALLPLANS.TXT for new insurance plan information.
				file=regKey.GetValue("INSTALLDIR").ToString()+@"\ALLPLANS.TXT";//C:\ETW\ALLPLANS.TXT
				ProcessTrojanPlanUpdates(file);
			}
		}

		///<summary>Process the deletion of existing insurance plans.</summary>
		private static void ProcessDeletedPlans(string file){
			if(!File.Exists(file)) {
				//Nothing to process.
				return;
			}
			string deleteplantext=File.ReadAllText(file);
			if(deleteplantext=="") {
				//Nothing to process. Don't delete the file in-case Trojan is filling the file right now.
				return;
			}
			deletePatientRecords=new Collection<string[]>();
			deleteTrojanRecords=new Collection<string[]>();
			string[] trojanplans=deleteplantext.Split(new string[] { "\n" },StringSplitOptions.RemoveEmptyEntries);
			Collection <string[]> records=new Collection<string[]>();
			for(int i=0;i<trojanplans.Length;i++) {
				string[] record=trojanplans[i].Split(new string[] {"\t"},StringSplitOptions.None);
				for(int j=0;j<record.Length;j++){
					//Remove any white space around the field and remove the surrounding quotes.
					record[j]=record[j].Trim().Substring(1);
					record[j]=record[j].Substring(0,record[j].Length-1);
				}
				records.Add(record);
				string whoToContact=record[3].ToUpper();
				if(whoToContact=="T"){
					deleteTrojanRecords.Add(record);
				}
				else{//whoToContact="P"
					deletePatientRecords.Add(record);
				}
			}
			if(deletePatientRecords.Count>0){
				pendingDeletionTable=TrojanQueries.GetPendingDeletionTable(deletePatientRecords);
				if(pendingDeletionTable.Rows.Count>0){
					using FormPrintReport fpr=new FormPrintReport();
					fpr.Text="Trojan Plans Pending Deletion: Contact Patients";
					fpr.ScrollAmount=10;
					fpr.printGenerator=ShowPendingDeletionReportForPatients;
					fpr.UsePageNumbers(new Font(FontFamily.GenericMonospace,8));
					fpr.MinimumTimesToPrint=1;
					fpr.ShowDialog();
				}
			}
			if(deleteTrojanRecords.Count>0) {
				pendingDeletionTableTrojan=TrojanQueries.GetPendingDeletionTableTrojan(deleteTrojanRecords);
				if(pendingDeletionTableTrojan.Rows.Count>0) {
					using FormPrintReport fpr=new FormPrintReport();
					fpr.Text="Trojan Plans Pending Deletion: Contact Trojan";
					fpr.ScrollAmount=10;
					fpr.printGenerator=ShowPendingDeletionReportForTrojan;
					fpr.UsePageNumbers(new Font(FontFamily.GenericMonospace,8));
					fpr.MinimumTimesToPrint=1;
					fpr.Landscape=true;
					fpr.ShowDialog();
				}
			}
			//Now that the plans have been reported, drop the plans that are marked finally deleted.
			for(int i=0;i<records.Count;i++){
				if(records[i][1]=="F") {
					try {
						InsPlan[] insplans=InsPlans.GetByTrojanID(records[i][0]);
						for(int j=0;j<insplans.Length;j++) {
							InsPlan planOld = insplans[j].Copy();
							insplans[j].PlanNote="PLAN DROPPED BY TROJAN"+Environment.NewLine+insplans[j].PlanNote;
							insplans[j].TrojanID="";
							InsPlans.Update(insplans[j],planOld);
							PatPlan[] patplans=PatPlans.GetByPlanNum(insplans[j].PlanNum);
							for(int k=0;k<patplans.Length;k++) {
								PatPlans.Delete(patplans[k].PatPlanNum);
							}
						}
					} 
					catch(ApplicationException ex) {
						MessageBox.Show(ex.Message);
						return;
					}
				}
			}
			File.Delete(file);
		}

		private static void ShowPendingDeletionReportForPatients(FormPrintReport fpr){
			//Print the header on the report.
			Font font=new Font(FontFamily.GenericMonospace,12);
			string text=PrefC.GetString(PrefName.PracticeTitle);
			SizeF size=fpr.Graph.MeasureString(text,font);
			float y=20;
			fpr.Graph.DrawString(text,font,Brushes.Black,fpr.GraphWidth/2-size.Width/2,y);
			text=DateTime.Today.ToShortDateString();
			size=fpr.Graph.MeasureString(text,font);
			fpr.Graph.DrawString(text,font,Brushes.Black,fpr.GraphWidth-size.Width,y);
			y+=size.Height;
			text="PLANS PENDING DELETION WHICH REQUIRE YOUR ATTENTION";
			size=fpr.Graph.MeasureString(text,font);
			fpr.Graph.DrawString(text,font,Brushes.Black,fpr.GraphWidth/2-fpr.Graph.MeasureString(text,font).Width/2,y);
			y+=size.Height;
			y+=20;//Skip a line or so.
			text="INSTRUCTIONS: These plans no longer exist, please do not contact Trojan. Please contact your patient for current benefit information.";
			fpr.Graph.DrawString(text,new Font(font,FontStyle.Bold),Brushes.Black,new RectangleF(0,y,650,500));
			y+=70;//Skip a line or so.
			text="Patient&Insured";
			font=new Font(font.FontFamily,9);
			fpr.Graph.DrawString(text,font,Brushes.Black,20,y);
			text="TrojanID";
			fpr.Graph.DrawString(text,font,Brushes.Black,240,y);
			text="Employer";
			fpr.Graph.DrawString(text,font,Brushes.Black,330,y);
			text="Carrier";
			fpr.Graph.DrawString(text,font,Brushes.Black,500,y);
			y+=20;
			//Use a static height for the records, to keep the math simple.
			float recordHeight=140;
			float recordSpacing=10;
			//Calculate the total number of pages in the report the first time this function is called only.
			if(fpr.TotalPages==0){
				fpr.TotalPages=(int)Math.Ceiling((y+recordHeight*pendingDeletionTable.Rows.Count+
					((pendingDeletionTable.Rows.Count>1)?pendingDeletionTable.Rows.Count-1:0)*recordSpacing)/fpr.PageHeight);
			}
			float pageBoundry=fpr.PageHeight;
			for(int i=0;i<pendingDeletionTable.Rows.Count;i++){
				//Draw the outlines around this record.
				fpr.Graph.DrawLine(Pens.Black,new PointF(0,y),new PointF(fpr.GraphWidth-1,y));
				fpr.Graph.DrawLine(Pens.Black,new PointF(0,y+recordHeight),new PointF(fpr.GraphWidth-1,y+recordHeight));
				fpr.Graph.DrawLine(Pens.Black,new PointF(0,y),new PointF(0,y+recordHeight));
				fpr.Graph.DrawLine(Pens.Black,new PointF(fpr.GraphWidth-1,y),new PointF(fpr.GraphWidth-1,y+recordHeight));
				fpr.Graph.DrawLine(Pens.Black,new PointF(0,y+recordHeight-40),new PointF(fpr.GraphWidth-1,y+recordHeight-40));
				fpr.Graph.DrawLine(Pens.Black,new PointF(235,y),new PointF(235,y+recordHeight-40));
				fpr.Graph.DrawLine(Pens.Black,new PointF(325,y),new PointF(325,y+recordHeight-40));
				fpr.Graph.DrawLine(Pens.Black,new PointF(500,y),new PointF(500,y+recordHeight-40));
				//Install the information for the record into the outline box.
				//Patient name, Guarantor name, guarantor SSN, guarantor birthdate, insurance plan group number,
				//and reason for pending deletion.
				fpr.Graph.DrawString(
					PIn.String(pendingDeletionTable.Rows[i][0].ToString())+" "+PIn.String(pendingDeletionTable.Rows[i][1].ToString())+Environment.NewLine+
					PIn.String(pendingDeletionTable.Rows[i][2].ToString())+" "+PIn.String(pendingDeletionTable.Rows[i][3].ToString())+Environment.NewLine+
					" SSN: "+PIn.String(pendingDeletionTable.Rows[i][4].ToString())+Environment.NewLine+
					" Birth: "+PIn.Date(pendingDeletionTable.Rows[i][5].ToString()).ToShortDateString()+Environment.NewLine+
					" Group: "+PIn.String(pendingDeletionTable.Rows[i][6].ToString()),font,Brushes.Black,
					new RectangleF(20,y+5,215,95));
				//Pending deletion reason.
				for(int j=0;j<deletePatientRecords.Count;j++) {
					if(deletePatientRecords[j][0]==PIn.String(pendingDeletionTable.Rows[i][8].ToString())) {
						text="REASON FOR DELETION: "+deletePatientRecords[j][7];
						if(deletePatientRecords[j][1].ToUpper()=="F"){
							text="FINALLY DELETED"+Environment.NewLine+text;
						}
						fpr.Graph.DrawString(text,font,Brushes.Black,
							new RectangleF(20,y+100,fpr.GraphWidth-40,40));
						break;
					}
				}
				//Trojan ID.
				fpr.Graph.DrawString(PIn.String(pendingDeletionTable.Rows[i][8].ToString()),font,Brushes.Black,new RectangleF(240,y+5,85,95));
				//Employer Name and Phone.
				fpr.Graph.DrawString(PIn.String(pendingDeletionTable.Rows[i][9].ToString())+Environment.NewLine+
					PIn.String(pendingDeletionTable.Rows[i][10].ToString()),font,Brushes.Black,new RectangleF(330,y+5,170,95));
				//Carrier Name and Phone
				fpr.Graph.DrawString(PIn.String(pendingDeletionTable.Rows[i][11].ToString())+Environment.NewLine+
					PIn.String(pendingDeletionTable.Rows[i][12].ToString()),font,Brushes.Black,
					new RectangleF(500,y+5,150,95));
				//Leave space between records.
				y+=recordHeight+recordSpacing;
				//Watch out for the bottom of each page for the next record.
				if(y+recordHeight>pageBoundry) {
					y=pageBoundry+fpr.MarginBottom+20;
					pageBoundry+=fpr.PageHeight+fpr.MarginBottom;
					text="Patient&Insured";
					font=new Font(font.FontFamily,9);
					fpr.Graph.DrawString(text,font,Brushes.Black,20,y);
					text="TrojanID";
					fpr.Graph.DrawString(text,font,Brushes.Black,240,y);
					text="Employer";
					fpr.Graph.DrawString(text,font,Brushes.Black,330,y);
					text="Carrier";
					fpr.Graph.DrawString(text,font,Brushes.Black,500,y);
					y+=20;
				}
			}
		}

		private static void ShowPendingDeletionReportForTrojan(FormPrintReport fpr) {
			//Print the header on the report.
			Font font=new Font(FontFamily.GenericMonospace,12);
			string text=PrefC.GetString(PrefName.PracticeTitle);
			SizeF size=fpr.Graph.MeasureString(text,font);
			float y=20;
			fpr.Graph.DrawString(text,font,Brushes.Black,fpr.GraphWidth/2-size.Width/2,y);
			text=DateTime.Today.ToShortDateString();
			size=fpr.Graph.MeasureString(text,font);
			fpr.Graph.DrawString(text,font,Brushes.Black,fpr.GraphWidth-size.Width,y);
			y+=size.Height;
			text="PLANS PENDING DELETION: Please Fax or Mail to Trojan";
			size=fpr.Graph.MeasureString(text,font);
			fpr.Graph.DrawString(text,font,Brushes.Black,fpr.GraphWidth/2-fpr.Graph.MeasureString(text,font).Width/2,y);
			y+=size.Height;
			text="Fax: 800-232-9788";
			size=fpr.Graph.MeasureString(text,font);
			fpr.Graph.DrawString(text,font,Brushes.Black,fpr.GraphWidth/2-fpr.Graph.MeasureString(text,font).Width/2,y);
			y+=size.Height;
			y+=20;//Skip a line or so.
			text="INSTRUCTIONS: Please complete the information requested below to help Trojan research these plans.\n"+
				"Active Patient: \"Yes\" means the patient has been in the office within the past 6 to 8 months.\n"+
				"Correct Employer: \"Yes\" means the insured currently is insured through this employer.\n"+
				"Correct Carrier: \"Yes\" means the insured currently has coverage with this carrier.";
			fpr.Graph.DrawString(text,new Font(new Font(font.FontFamily,10),FontStyle.Bold),Brushes.Black,new RectangleF(0,y,900,500));
			y+=85;//Skip a line or so.
			font=new Font(font.FontFamily,9);
			text="Active\nPatient?";
			fpr.Graph.DrawString(text,font,Brushes.Black,5,y);
			text="\nPatient&Insured";
			fpr.Graph.DrawString(text,font,Brushes.Black,80,y);
			text="\nTrojanID";
			fpr.Graph.DrawString(text,font,Brushes.Black,265,y);
			text="Correct\nEmployer?";
			fpr.Graph.DrawString(text,font,Brushes.Black,345,y);
			text="\nEmployer";
			fpr.Graph.DrawString(text,font,Brushes.Black,420,y);
			text="Correct\nCarrier?";
			fpr.Graph.DrawString(text,font,Brushes.Black,600,y);
			text="\nCarrier";
			fpr.Graph.DrawString(text,font,Brushes.Black,670,y);
			y+=30;
			//Use a static height for the records, to keep the math simple.
			float recordHeight=200;
			float recordSpacing=10;
			//Calculate the total number of pages in the report the first time this function is called only.
			if(fpr.TotalPages==0) {
				fpr.TotalPages=(int)Math.Ceiling((y+recordHeight*pendingDeletionTableTrojan.Rows.Count+
					((pendingDeletionTableTrojan.Rows.Count>1)?pendingDeletionTableTrojan.Rows.Count-1:0)*recordSpacing)/fpr.PageHeight);
			}
			float pageBoundry=fpr.PageHeight;
			for(int i=0;i<pendingDeletionTableTrojan.Rows.Count;i++) {
				//Draw the outlines around this record.
				fpr.Graph.DrawLine(Pens.Black,new PointF(0,y),new PointF(fpr.GraphWidth-1,y));
				fpr.Graph.DrawLine(Pens.Black,new PointF(0,y+recordHeight),new PointF(fpr.GraphWidth-1,y+recordHeight));
				fpr.Graph.DrawLine(Pens.Black,new PointF(0,y),new PointF(0,y+recordHeight));
				fpr.Graph.DrawLine(Pens.Black,new PointF(fpr.GraphWidth-1,y),new PointF(fpr.GraphWidth-1,y+recordHeight));
				fpr.Graph.DrawLine(Pens.Black,new PointF(0,y+recordHeight-40),new PointF(fpr.GraphWidth-1,y+recordHeight-40));
				fpr.Graph.DrawLine(Pens.Black,new PointF(260,y),new PointF(260,y+recordHeight-40));
				fpr.Graph.DrawLine(Pens.Black,new PointF(340,y),new PointF(340,y+recordHeight-40));
				fpr.Graph.DrawLine(Pens.Black,new PointF(595,y),new PointF(595,y+recordHeight-40));
				//Patient active boxes.
				text="Yes No";
				fpr.Graph.DrawString(text,font,Brushes.Black,10,y);
				fpr.Graph.DrawRectangle(Pens.Black,new Rectangle(15,(int)(y+15),15,15));
				fpr.Graph.DrawRectangle(Pens.Black,new Rectangle(40,(int)(y+15),15,15));
				//Install the information for the record into the outline box.
				//Patient name, Guarantor name, guarantor SSN, guarantor birthdate, insurance plan group number,
				//and reason for pending deletion.
				fpr.Graph.DrawString(
					PIn.String(pendingDeletionTableTrojan.Rows[i][0].ToString())+" "+PIn.String(pendingDeletionTableTrojan.Rows[i][1].ToString())+Environment.NewLine+
					PIn.String(pendingDeletionTableTrojan.Rows[i][2].ToString())+" "+PIn.String(pendingDeletionTableTrojan.Rows[i][3].ToString())+Environment.NewLine+
					" SSN: "+PIn.String(pendingDeletionTableTrojan.Rows[i][4].ToString())+Environment.NewLine+
					" Birth: "+PIn.Date(pendingDeletionTableTrojan.Rows[i][5].ToString()).ToShortDateString()+Environment.NewLine+
					" Group: "+PIn.String(pendingDeletionTableTrojan.Rows[i][6].ToString()),font,Brushes.Black,
					new RectangleF(80,y+5,185,95));
				//Pending deletion reason.
				for(int j=0;j<deleteTrojanRecords.Count;j++) {
					if(deleteTrojanRecords[j][0]==PIn.String(pendingDeletionTableTrojan.Rows[i][8].ToString())) {
						text="REASON FOR DELETION: "+deleteTrojanRecords[j][7];
						if(deleteTrojanRecords[j][1].ToUpper()=="F"){
							text="FINALLY DELETED"+Environment.NewLine+text;
						}
						fpr.Graph.DrawString(text,font,Brushes.Black,
							new RectangleF(5,y+recordHeight-40,fpr.GraphWidth-10,40));
						break;
					}
				}
				//Trojan ID.
				fpr.Graph.DrawString(PIn.String(pendingDeletionTableTrojan.Rows[i][8].ToString()),font,Brushes.Black,new RectangleF(265,y+5,85,95));
				//Correct Employer boxes and arrow.
				text="Yes No";
				fpr.Graph.DrawString(text,font,Brushes.Black,345,y);
				fpr.Graph.DrawRectangle(Pens.Black,new Rectangle(350,(int)(y+15),15,15));
				fpr.Graph.DrawRectangle(Pens.Black,new Rectangle(375,(int)(y+15),15,15));
				//Employer Name and Phone.
				fpr.Graph.DrawString(PIn.String(pendingDeletionTableTrojan.Rows[i][9].ToString())+Environment.NewLine+
					PIn.String(pendingDeletionTableTrojan.Rows[i][10].ToString()),font,Brushes.Black,new RectangleF(420,y+5,175,95));
				//New employer information if necessary.
				text="New\nEmployer:";
				fpr.Graph.DrawString(text,font,Brushes.Black,345,y+85);
				fpr.Graph.DrawLine(Pens.Black,415,y+110,590,y+110);
				fpr.Graph.DrawLine(Pens.Black,415,y+125,590,y+125);
				text="Phone:";
				fpr.Graph.DrawString(text,font,Brushes.Black,345,y+130);
				fpr.Graph.DrawLine(Pens.Black,415,y+140,590,y+140);
				//Correct Carrier boxes and arrow.
				text="Yes No";
				fpr.Graph.DrawString(text,font,Brushes.Black,600,y);
				fpr.Graph.DrawRectangle(Pens.Black,new Rectangle(605,(int)(y+15),15,15));
				fpr.Graph.DrawRectangle(Pens.Black,new Rectangle(630,(int)(y+15),15,15));
				//Carrier Name and Phone
				fpr.Graph.DrawString(PIn.String(pendingDeletionTableTrojan.Rows[i][11].ToString())+Environment.NewLine+
					PIn.String(pendingDeletionTableTrojan.Rows[i][12].ToString()),font,Brushes.Black,
					new RectangleF(670,y+5,225,95));
				//New carrier information if necessary.
				text="New\nCarrier:";
				fpr.Graph.DrawString(text,font,Brushes.Black,600,y+85);
				fpr.Graph.DrawLine(Pens.Black,670,y+110,895,y+110);
				fpr.Graph.DrawLine(Pens.Black,670,y+125,895,y+125);
				text="Phone:";
				fpr.Graph.DrawString(text,font,Brushes.Black,600,y+130);
				fpr.Graph.DrawLine(Pens.Black,670,y+140,895,y+140);
				//Leave space between records.
				y+=recordHeight+recordSpacing;
				//Watch out for the bottom of each page for the next record.
				if(y+recordHeight>pageBoundry) {
					y=pageBoundry+fpr.MarginBottom+20;
					pageBoundry+=fpr.PageHeight+fpr.MarginBottom;
					text="Active\nPatient?";
					fpr.Graph.DrawString(text,font,Brushes.Black,5,y);
					text="\nPatient&Insured";
					fpr.Graph.DrawString(text,font,Brushes.Black,80,y);
					text="\nTrojanID";
					fpr.Graph.DrawString(text,font,Brushes.Black,265,y);
					text="Correct\nEmployer?";
					fpr.Graph.DrawString(text,font,Brushes.Black,345,y);
					text="\nEmployer";
					fpr.Graph.DrawString(text,font,Brushes.Black,420,y);
					text="Correct\nCarrier?";
					fpr.Graph.DrawString(text,font,Brushes.Black,600,y);
					text="\nCarrier";
					fpr.Graph.DrawString(text,font,Brushes.Black,670,y);
					y+=30;
				}
			}
		}

		///<summary>Process existing insurance plan updates from the ALLPLANS.TXT file.</summary>
		private static void ProcessTrojanPlanUpdates(string file){
			if(!File.Exists(file)) {
				//Nothing to process.
				return;
			}
			MessageBox.Show("Trojan update found.  Please print or save the text file when it opens, then close it.  You will be given a chance to cancel the update after that.");
			ODFileUtils.ProcessStart(file);
			if(!MsgBox.Show("Trojan",MsgBoxButtons.OKCancel,"Trojan plans will now be updated.")) {
				return;
			}
			Cursor.Current=Cursors.WaitCursor;
			string allplantext="";
			using(StreamReader sr=new StreamReader(file)) {
				allplantext=sr.ReadToEnd();
			}
			if(allplantext=="") {
				Cursor.Current=Cursors.Default;
				MessageBox.Show("Could not read file contents: "+file);
				return;
			}
			bool updateBenefits=MsgBox.Show("Trojan",MsgBoxButtons.YesNo,"Also update benefits?  Any customized benefits will be overwritten.");
			bool updateNote=MsgBox.Show("Trojan",MsgBoxButtons.YesNo,"Automatically update plan notes?  Any existing notes will be overwritten.  If you click No, you will be presented with each change for each plan so that you can edit the notes as needed.");
			string[] trojanplans=allplantext.Split(new string[] { "TROJANID" },StringSplitOptions.RemoveEmptyEntries);
			int plansAffected=0;
			try {
				for(int i=0;i<trojanplans.Length;i++) {
					trojanplans[i]="TROJANID"+trojanplans[i];
					plansAffected+=ProcessTrojanPlan(trojanplans[i],updateBenefits,updateNote);
				}
			}
			catch(Exception e) {//this will happen if user clicks cancel in a note box.
				MessageBox.Show("Error: "+e.Message+"\r\n\r\nWe will need a copy of the ALLPLANS.txt.");
				return;
			}
			Cursor.Current=Cursors.Default;
			MessageBox.Show(plansAffected.ToString()+" plans updated.");
			try{
				File.Delete(file);
			}
			catch{
				MessageBox.Show(file+" could not be deleted.  Please delete manually.");
			}
		}

		///<summary>Returns number of subscribers affected.  Can throw an exception if user clicks cancel in a note box.</summary>
		private static int ProcessTrojanPlan(string trojanPlan,bool updateBenefits,bool updateNoteAutomatic){
			TrojanObject troj=ProcessTextToObject(trojanPlan);
			Carrier carrier=new Carrier();
			carrier.Phone=troj.ELIGPHONE;
			carrier.ElectID=troj.PAYERID;
			carrier.CarrierName=troj.MAILTO;
			carrier.Address=troj.MAILTOST;
			carrier.City=troj.MAILCITYONLY;
			carrier.State=troj.MAILSTATEONLY;
			carrier.Zip=troj.MAILZIPONLY;
			carrier.NoSendElect=NoSendElectType.SendElect;//regardless of what Trojan says.  Nobody sends paper anymore.
			if(carrier.CarrierName==null || carrier.CarrierName=="") {
				//if, for some reason, carrier is absent from the file, we can't do a thing with it.
				return 0;
			}
			carrier=Carriers.GetIdentical(carrier);
			//now, save this all to the database.
			troj.CarrierNum=carrier.CarrierNum;
			InsPlan plan=TrojanQueries.GetPlanWithTrojanID(troj.TROJANID);
			if(plan==null) {
				return 0;
			}
			TrojanQueries.UpdatePlan(troj,plan.PlanNum,updateBenefits);
			plan=InsPlans.RefreshOne(plan.PlanNum);
			InsPlan planOld = plan.Copy();
			if(updateNoteAutomatic) {
				if(plan.PlanNote!=troj.PlanNote) {
					plan.PlanNote=troj.PlanNote;
					InsPlans.Update(plan,planOld);
				}
			}
			else {
				//let user pick note
				if(plan.PlanNote!=troj.PlanNote) {
					string[] notes=new string[2];
					notes[0]=plan.PlanNote;
					notes[1]=troj.PlanNote;
					using FormNotePick FormN=new FormNotePick(notes);
					FormN.ShowDialog();
					if(FormN.DialogResult==DialogResult.OK) {
						if(plan.PlanNote!=FormN.SelectedNote) {
							plan.PlanNote=FormN.SelectedNote;
							InsPlans.Update(plan,planOld);
						}
					}
				}
			}
			return 1;
		}

		///<summary>Converts the text for one plan into an object which will then be processed as needed.</summary>
		public static TrojanObject ProcessTextToObject(string text){
			string[] lines=text.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries);
			string line;
			string[] fields;
			int percent;
			double amt;
			string rowVal;
			TrojanObject troj=new TrojanObject();
			troj.BenefitList=new List<Benefit>();
			troj.BenefitNotes="";
			bool usesAnnivers=false;
			Benefit ben;
			Benefit benCrownMajor=null;
			Benefit benCrownOnly=null;
      for(int i=0;i<lines.Length;i++){
				line=lines[i];
				fields=line.Split(new char[] {'\t'});
				if(fields.Length!=3){
					continue;
				}
				//remove any trailing or leading spaces:
				fields[0]=fields[0].Trim();
				fields[1]=fields[1].Trim();
				fields[2]=fields[2].Trim();
				rowVal=fields[2].Trim();
				if(fields[2]==""){
					continue;
				}
				else{//as long as there is data, add it to the notes
					if(troj.BenefitNotes!="") {
						troj.BenefitNotes+="\r\n";
					}
					troj.BenefitNotes+=fields[1]+": "+fields[2];
					if(fields.Length==4) {
						troj.BenefitNotes+=" "+fields[3];
					}
				}
				switch(fields[0]){
					//default://for all rows that are not handled below
					case "TROJANID":
						troj.TROJANID=fields[2];
						break;
					case "ENAME":
						troj.ENAME=fields[2];
						break;
					case "PLANDESC":
						troj.PLANDESC=fields[2];
						break;
					case "ELIGPHONE":
						troj.ELIGPHONE=fields[2];
						break;
					case "POLICYNO":
						troj.POLICYNO=fields[2];
						break;
					case "ECLAIMS":
						if(fields[2]=="YES") {//accepts eclaims
							troj.ECLAIMS=true;
						}
						else {
							troj.ECLAIMS=false;
						}
						break;
					case "PAYERID":
						troj.PAYERID=fields[2];
						break;
					case "MAILTO":
						troj.MAILTO=fields[2];
						break;
					case "MAILTOST":
						troj.MAILTOST=fields[2];
						break;
					case "MAILCITYONLY":
						troj.MAILCITYONLY=fields[2];
						break;
					case "MAILSTATEONLY":
						troj.MAILSTATEONLY=fields[2];
						break;
					case "MAILZIPONLY":
						troj.MAILZIPONLY=fields[2];
						break;
					case "PLANMAX"://eg $3000 per person per year
						if(!fields[2].StartsWith("$"))
							break;
						fields[2]=fields[2].Remove(0,1);
						fields[2]=fields[2].Split(new char[] { ' ' })[0];
						if(CovCats.GetCount(true) > 0) {
							ben=new Benefit();
							ben.BenefitType=InsBenefitType.Limitations;
							ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum;
							ben.MonetaryAmt=PIn.Double(fields[2]);
							ben.TimePeriod=BenefitTimePeriod.CalendarYear;
							ben.CoverageLevel=BenefitCoverageLevel.Individual;
							troj.BenefitList.Add(ben.Copy());
						}
						break;
					case "PLANYR"://eg Calendar year or Anniversary year or month renewal
						string monthName=fields[2].Split(new char[] {' '})[0];
						usesAnnivers=true;
						if(fields[2]=="Calendar year" || ListTools.In(monthName,DateTimeFormatInfo.CurrentInfo.MonthNames)) {
							usesAnnivers=false;
							troj.MonthRenewal=DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().IndexOf(monthName)+1;
						}
						//MessageBox.Show("Warning.  Plan uses Anniversary year rather than Calendar year.  Please verify the Plan Start Date.");
						break;
					case "DEDUCT"://eg There is no deductible
						if(!fields[2].StartsWith("$")) {
							amt=0;
						}
						else {
							fields[2]=fields[2].Remove(0,1);
							fields[2]=fields[2].Split(new char[] { ' ' })[0];
							amt=PIn.Double(fields[2]);
						}
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.Deductible;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						ben.MonetaryAmt=amt;
						ben.CoverageLevel=BenefitCoverageLevel.Individual;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.Deductible;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						ben.MonetaryAmt=0;//amt;
						ben.CoverageLevel=BenefitCoverageLevel.Individual;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.Deductible;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						ben.MonetaryAmt=0;//amt;
						ben.CoverageLevel=BenefitCoverageLevel.Individual;
						troj.BenefitList.Add(ben.Copy());
						break;
					case "PREV"://eg 100% or 'Incentive begins at 70%' or '80% Endo Major see notes'
						if(rowVal.ToLower()=="not covered") {
							percent=0;
						}
						else {
							percent=ConvertPercentToInt(rowVal);//remove %
						}
						if(percent<0 || percent>100) {
							break;
						}
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						break;
					case "BASIC":
						if(rowVal.ToLower()=="not covered") {
							percent=0;
						}
						else {
							percent=ConvertPercentToInt(rowVal);//remove %
						}
						if(percent<0 || percent>100) {
							break;
						}
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Restorative).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						break;
					case "MAJOR":
						if(rowVal.ToLower()=="not covered") {
							percent=0;
						}
						else {
							percent=ConvertPercentToInt(rowVal);//remove %
						}
						if(percent<0 || percent>100) {
							break;
						}
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						benCrownMajor=new Benefit();
						benCrownMajor.BenefitType=InsBenefitType.CoInsurance;
						benCrownMajor.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Crowns).CovCatNum;
						benCrownMajor.Percent=percent;
						benCrownMajor.TimePeriod=BenefitTimePeriod.CalendarYear;
						//troj.BenefitList.Add(ben.Copy());//later
						break;
					case "CROWNS"://Examples: Paid Major, or 80%.  We will only process percentages.
						if(rowVal.ToLower()=="not covered") {
							percent=0;
						}
						else {
							percent=ConvertPercentToInt(rowVal);//remove %
						}
						if(percent<0 || percent>100) {
							break;
						}
						benCrownOnly=new Benefit();
						benCrownOnly.BenefitType=InsBenefitType.CoInsurance;
						benCrownOnly.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Crowns).CovCatNum;
						benCrownOnly.Percent=percent;
						benCrownOnly.TimePeriod=BenefitTimePeriod.CalendarYear;
						//troj.BenefitList.Add(ben.Copy());
						break;
					case "ORMAX"://eg $3500 lifetime
						if(!fields[2].StartsWith("$")) {
							break;
						}
						fields[2]=fields[2].Remove(0,1);
						fields[2]=fields[2].Split(new char[] { ' ' })[0];
						if(CovCats.GetCount(true) > 0) {
							ben=new Benefit();
							ben.BenefitType=InsBenefitType.Limitations;
							ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
							ben.MonetaryAmt=PIn.Double(fields[2]);
							ben.TimePeriod=BenefitTimePeriod.CalendarYear;
							troj.BenefitList.Add(ben.Copy());
						}
						break;
					case "ORPCT":
						if(rowVal.ToLower()=="not covered") {
							percent=0;
						}
						else {
							percent=ConvertPercentToInt(rowVal);//remove %
						}
						if(percent<0 || percent>100) {
							break;
						}
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						break;
					/*case "FEE":
						if(!ProcedureCodes.IsValidCode(fields[1])) {
							break;//skip
						}
						if(textTrojanID.Text==""){
							break;
						}
						feeSchedNum=Fees.ImportTrojan(fields[1],PIn.PDouble(fields[3]),textTrojanID.Text);
						//the step above probably created a new feeschedule, requiring a reset of the three listboxes.
						resetFeeSched=true;
						break;*/
					case "NOTES"://typically multiple instances
						if(troj.PlanNote!=null && troj.PlanNote!="") {
							troj.PlanNote+="\r\n";
						}
						troj.PlanNote+=fields[2];
						break;
				}//switch
			}//for
			//Set crowns
			if(benCrownOnly!=null){
				troj.BenefitList.Add(benCrownOnly.Copy());
			}
			else if(benCrownMajor!=null){
				troj.BenefitList.Add(benCrownMajor.Copy());
			}
			//set calendar vs serviceyear
			if(usesAnnivers) {
				for(int i=0;i<troj.BenefitList.Count;i++) {
					troj.BenefitList[i].TimePeriod=BenefitTimePeriod.ServiceYear;
				}
			}
			return troj;
		}

		///<summary>Takes a string percentage and returns the integer value.  Returns -1 if no match.</summary>
		private static int ConvertPercentToInt(string percent) {
			Match regMatch=Regex.Match(percent,@"([0-9]+)\%");
			if(regMatch.Success) {
				return PIn.Int(regMatch.Groups[1].Value);
			}
			return -1;
		}



	}

	
}
