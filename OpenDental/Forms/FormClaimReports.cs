using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Net;
using System.Collections.Generic;
using System.Globalization;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormClaimReports : FormODBase {
		///<summary>If true, then reports will be automatically retrieved for default clearinghouse.  Then this form will close.</summary>
		public bool AutomaticMode;
		private List<Clearinghouse> _listHqClearinghouses;

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormClaimReportsCanada";
			}
			return "FormClaimReports";
		}

		///<summary></summary>
		public FormClaimReports()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormClaimReports_Load(object sender, System.EventArgs e) {
			_listHqClearinghouses=Clearinghouses.GetDeepCopy();
			for(int i=0;i<_listHqClearinghouses.Count;i++){
				comboClearhouse.Items.Add(_listHqClearinghouses[i].Description);
				if(PrefC.GetLong(PrefName.ClearinghouseDefaultDent)==_listHqClearinghouses[i].ClearinghouseNum){
					comboClearhouse.SelectedIndex=i;
				}
			}
			if(comboClearhouse.Items.Count>0 && comboClearhouse.SelectedIndex==-1){
				comboClearhouse.SelectedIndex=0;
			}
		}

		private void FormClaimReports_Shown(object sender,EventArgs e) {
			if(AutomaticMode) {
				labelRetrieving.Visible=true;
				Cursor=Cursors.WaitCursor;
				Clearinghouse clearinghouseHq=_listHqClearinghouses[comboClearhouse.SelectedIndex];
				Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
				string errorMessage=Clearinghouses.RetrieveAndImport(clearinghouseClin,AutomaticMode);
				if(errorMessage!="") {
					MessageBox.Show(errorMessage);
				}
				Cursor=Cursors.Default;
				Close();
			}
		}

		private void butRetrieve_Click(object sender,EventArgs e) {
			if(comboClearhouse.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a clearinghouse first.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Connect to clearinghouse to retrieve reports?")) {
				return;
			}
			Clearinghouse clearhouseHq=_listHqClearinghouses[comboClearhouse.SelectedIndex];
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearhouseHq,Clinics.ClinicNum);
			if(!Directory.Exists(clearinghouseClin.ResponsePath)) {
				MsgBox.Show(this,"Clearinghouse does not have a valid Report Path set.");
				return;
			}
			ODProgressExtended progressbar=new ODProgressExtended(ODEventType.Clearinghouse,new ClearinghouseEvent(),this
				,new ProgressBarHelper((Lans.g(this,"Clearinghouse Progress")),progressBarEventType:ProgBarEventType.Header),lanThis: this.Name);
			//For Tesia, user wouldn't normally manually retrieve.
			if(clearhouseHq.ISA08=="113504607") {
				if((PrefC.RandomKeys && PrefC.HasClinicsEnabled)//See FormClaimsSend_Load
					|| PrefC.GetLong(PrefName.ClearinghouseDefaultDent)!=clearhouseHq.ClearinghouseNum) //default
				{
					//But they might need to in these situations.
					string errorMessage=Clearinghouses.RetrieveAndImport(clearinghouseClin,false,progressbar);
					progressbar.UpdateProgressDetailed("",tagString:"reports",percentVal:"100%",barVal:100);
					if(errorMessage=="") {
						progressbar.UpdateProgress(Lan.g(this,"Retrieval and import successful"));
					}
					else {
						progressbar.UpdateProgress(errorMessage);
					}
					progressbar.UpdateProgress(Lan.g(this,"Done"));
				}
				else{
					progressbar.UpdateProgress(Lan.g(this,"No need to retrieve. Available reports are automatically downloaded every three minutes."));
				}
				progressbar.OnProgressDone();
				return;
			}
			if(Plugins.HookMethod(this,"FormClaimReports.butRetrieve_Click_isRetrievingReportsSupported",clearhouseHq)) {
				goto HookSkipRetrievingNotSupported;
			}
			if(clearhouseHq.CommBridge==EclaimsCommBridge.None
				|| clearhouseHq.CommBridge==EclaimsCommBridge.Renaissance 
				|| clearhouseHq.CommBridge==EclaimsCommBridge.RECS) 
			{
				progressbar.UpdateProgress(Lan.g(this,"No built-in functionality for retrieving reports from this clearinghouse."));
				progressbar.OnProgressDone();
				return;
			}
			HookSkipRetrievingNotSupported: { }
			labelRetrieving.Visible=true;
			string errorMesssage=Clearinghouses.RetrieveAndImport(clearinghouseClin,false,progressbar);
			progressbar.UpdateProgressDetailed("",tagString:"reports",percentVal:"100%",barVal:100);
			if(clearhouseHq.CommBridge==EclaimsCommBridge.ClaimConnect && errorMesssage=="" && Directory.Exists(clearinghouseClin.ResponsePath)) {
				//Since the dentalxchange website was successfully launched in this scenario, the user does not need any further notification.
			}
			else if(errorMesssage=="") {
				progressbar.UpdateProgress(Lan.g(this,"Retrieve and import successful."));
			}
			else {
				progressbar.UpdateProgress(Lans.g(progressbar.LanThis,"Error Log:")+"\r\n"+errorMesssage);
			}
			labelRetrieving.Visible=false;
			progressbar.OnProgressDone();
			if(progressbar.IsCanceled) {//close
				progressbar.Close();
			}
		}


		/*private void listMain_DoubleClick(object sender, System.EventArgs e) {
			if(listMain.SelectedIndices.Count==0){
				return;
			}
			string messageText=File.ReadAllText((string)listMain.SelectedItem);
			if(X12object.IsX12(messageText)){
				X12object xobj=new X12object(messageText);
				if(X277U.Is277U(xobj)){
					MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(X277U.MakeHumanReadable(xobj));
					msgbox.ShowDialog();
				}
				else if(X997.Is997(xobj)) {
					//MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(X997.MakeHumanReadable(xobj));
					//msgbox.ShowDialog();
				}
			}
			else{
				MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(messageText);
				msgbox.ShowDialog();
			}
			
			//if the file is an X12 file (277 for now), then display it differently
			if(Path.GetExtension((string)listMain.SelectedItem)==".txt"){
				//List<string> messageLines=new List<string>();
				//X12object xObj=new X12object(File.ReadAllText(fileName));
				string firstLine="";
				using(StreamReader sr=new StreamReader((string)listMain.SelectedItem)){
					firstLine=sr.ReadLine();
				}
				if(firstLine!=null && firstLine.Length==106 && firstLine.Substring(0,3)=="ISA"){
					//try{
						string humanText=X277U.MakeHumanReadable((string)listMain.SelectedItem);
						ArchiveFile((string)listMain.SelectedItem);
						//now the file will be gone
						//create a new file from humanText with same name as original file
						StreamWriter sw=File.CreateText((string)listMain.SelectedItem);
						sw.Write(humanText);
						sw.Close();
						//now, it will try to launch the new text file
					//}
					//catch(Exception ex){
					//	MessageBox.Show(ex.Message);
					//	return;
					//}
				}
			}
			try{
				Process.Start((string)listMain.SelectedItem);
			}
			catch{
				MsgBox.Show(this,"Could not open the item. You could try open it directly from the folder where it is located.");
			}
			//FillGrid();
		}*/

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

	

		

		

		

		


	}
}





















