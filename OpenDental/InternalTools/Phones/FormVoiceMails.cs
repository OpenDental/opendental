using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormVoiceMails:FormODBase {

		public FormVoiceMails() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormVoiceMails_Load(object sender,EventArgs e) {
			FillGrid();
			gridVoiceMails.ContextMenu=menuVoiceMailsRightClick;
			axWindowsMediaPlayer.settings.volume=100;//Max volume. Voicemails are generally pretty quiet.
		}

		private void FillGrid() {
			labelError.Visible=false;
			List<VoiceMail> listVoiceMails=VoiceMails.GetAll(includeDeleted:checkShowDeleted.Checked).OrderBy(x => x.UserNum>0)//Show unclaimed VMs first
				.ThenBy(x => x.DateCreated).ToList();//Show oldest VMs on top
			VoiceMail voiceMailSelected=null;
			int selectedCellX=gridVoiceMails.SelectedCell.X;
			string changedNoteText=null;//If this value is null, then the note has not been changed by the user.
			if(gridVoiceMails.SelectedCell.Y > -1) {
				voiceMailSelected=(VoiceMail)gridVoiceMails.ListGridRows[gridVoiceMails.SelectedCell.Y].Tag;
				changedNoteText=gridVoiceMails.GetText(gridVoiceMails.SelectedCell.Y,gridVoiceMails.ListGridColumns.GetIndex(Lan.g(this,"Note")));
				if(changedNoteText==voiceMailSelected.Note) {
					changedNoteText=null;//To indicate that it has not been changed.
				}
			}
			gridVoiceMails.BeginUpdate();
			gridVoiceMails.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Date Time"),120);
			gridVoiceMails.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Phone #"),105);
			gridVoiceMails.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient"),200);
			gridVoiceMails.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"User"),90);
			gridVoiceMails.ListGridColumns.Add(col);
			if(checkShowDeleted.Checked) {
				col=new GridColumn(Lan.g(this,"Deleted"),60,HorizontalAlignment.Center);
				gridVoiceMails.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Note"),200,true);
			col.Tag=nameof(VoiceMail.Note);
			gridVoiceMails.ListGridColumns.Add(col);
			gridVoiceMails.ListGridRows.Clear();
			GridRow row;
			foreach(VoiceMail voiceMail in listVoiceMails) {
				row=new GridRow();
				row.Cells.Add(voiceMail.DateCreated.ToShortDateString()+" "+voiceMail.DateCreated.ToShortTimeString());
				string phoneField=TelephoneNumbers.AutoFormat(voiceMail.PhoneNumber);
				if(string.IsNullOrEmpty(phoneField)) {
					phoneField=Lan.g(this,"Unknown");
				}
				row.Cells.Add(phoneField);
				row.Cells.Add(voiceMail.PatientName);
				row.Cells.Add(voiceMail.UserName);
				if(checkShowDeleted.Checked) {
					row.Cells.Add(voiceMail.StatusVM==VoiceMailStatus.Deleted ? "X" : "");
				}
				row.Cells.Add(voiceMail.Note);
				row.Tag=voiceMail;
				gridVoiceMails.ListGridRows.Add(row);
			}
			gridVoiceMails.EndUpdate();
			//Reselect the selected row if necessary
			if(voiceMailSelected!=null) {
				for(int i=0;i<gridVoiceMails.ListGridRows.Count;i++) {
					if(((VoiceMail)gridVoiceMails.ListGridRows[i].Tag).VoiceMailNum==voiceMailSelected.VoiceMailNum) {
						gridVoiceMails.SetSelected(new Point(selectedCellX,i));
						if(changedNoteText != null) {//The user has changed the note.
							gridVoiceMails.ListGridRows[i].Cells[gridVoiceMails.ListGridColumns.GetIndex(Lan.g(this,"Note"))].Text=changedNoteText;
						}
					}
				}
			}
		}

		private void checkShowDeleted_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
		}

		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(listSignals.Any(x => x.IType==InvalidType.VoiceMails)) {
				FillGrid();
			}
		}

		private void menuVoiceMailsRightClick_Popup(object sender,EventArgs e) {
			if(gridVoiceMails.SelectedCell.Y==-1 
				|| (gridVoiceMails.ListGridColumns[gridVoiceMails.SelectedCell.X].Tag??"").ToString()==nameof(VoiceMail.Note)) 
			{
				gridVoiceMails.ContextMenu=menuVoiceMailsRightClick;
				return;
			}
			VoiceMail voiceMailCur=(VoiceMail)gridVoiceMails.ListGridRows[gridVoiceMails.SelectedCell.Y].Tag;
			if(voiceMailCur.PatNum==0) {
				menuGoToChart.Visible=false;
			}
			else {
				menuGoToChart.Visible=true;
			}
			if(voiceMailCur.PhoneNumber=="") {
				menuGoogleNum.Visible=false;
			}
			else {
				menuGoogleNum.Visible=true;
			}
		}

		private void menuSendToMe_Click(object sender,EventArgs e) {
			if(!SendToMe()) {
				return;
			}
			GoToChart();
		}

		private void menuSendToMeCreateTask_Click(object sender,EventArgs e) {
			if(!SendToMe()) {
				return;
			}
			FormTaskEdit formTaskEdit;
			if(!TryCreateTaskAndForm(out formTaskEdit)) {
				return;
			}
			GoToChart();
			formTaskEdit.Show();
		}

		private void menuGoToChart_Click(object sender,EventArgs e) {
			GoToChart();
		}

		private void menuGoogleNum_Click(object sender,EventArgs e) {
			if(gridVoiceMails.SelectedCell.Y==-1 || gridVoiceMails.ListGridRows.Count<=gridVoiceMails.SelectedCell.Y) {
				return;
			}
			VoiceMail voiceMailCur=(VoiceMail)gridVoiceMails.ListGridRows[gridVoiceMails.SelectedCell.Y].Tag;
			string number=voiceMailCur.PhoneNumber;
			//The numbers are often represented with a leading 1, ex: 15033635432. Google searches generally yield better results without the leading 1.
			//If the number is a standard length (11 digits) and it starts with a 1, strip the leading 1.
			if(number.Length==11 && number[0]=='1') {
				number=StringTools.TruncateBeginning(number,10);
			}
			string url=@"https://www.google.com/search?q="+number;
			try {
				Process.Start(url);
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
		}

		private void gridVoiceMails_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Row==-1 || gridVoiceMails.ListGridRows.Count<=e.Row) {
				return;
			}
			labelError.Visible=false;
			VoiceMail voiceMailCur=(VoiceMail)gridVoiceMails.ListGridRows[e.Row].Tag;
			labelDuration.Text=Lan.g(this,"Duration:");
			if(voiceMailCur.Duration>=60) {//At least one minute long
				labelDuration.Text+=" "+voiceMailCur.Duration/60+" "+Lan.g(this,"min");
			}
			if(voiceMailCur.Duration>=0) {
				labelDuration.Text+=" "+voiceMailCur.Duration%60+" "+Lan.g(this,"sec");
			}
			try {
				if(PrefC.GetBool(PrefName.VoiceMailSMB2Enabled)) {
					string remoteName=Path.GetDirectoryName(voiceMailCur.FileName);
					using(new ODNetworkConnection(remoteName,PrefC.VoiceMailNetworkCredentialsSMB2)) {
						PlayVM(voiceMailCur.FileName);
					}
				}
				else {
					PlayVM(voiceMailCur.FileName);
				}
			}
			catch(ODException ex) {
				labelError.Text=ex.Message+" - Error Code: "+ex.ErrorCode;
				labelError.Visible=true;
			}
			catch(Exception ex) {
				labelError.Text=ex.Message;
				labelError.Visible=true;
			}
		}

		///<summary>Plays the voice mail at the given file.</summary>
		private void PlayVM(string fileName) {
			if(!File.Exists(fileName)) {
				throw new ApplicationException(Lan.g(this,"File not found"));
			}
			if(axWindowsMediaPlayer.playState!=WMPLib.WMPPlayState.wmppsPlaying//Don't start playing it again if the selected VM is currently playing
				|| axWindowsMediaPlayer.URL!=fileName) {
				axWindowsMediaPlayer.URL=fileName;//Automatically starts playing the VM
			}
		}

		private void gridVoiceMails_CellLeave(object sender,ODGridClickEventArgs e) {
			if(gridVoiceMails.ListGridColumns[e.Col].Tag.ToString() != nameof(VoiceMail.Note)) {
				return;
			}
			VoiceMail voiceMailCur=(VoiceMail)gridVoiceMails.ListGridRows[e.Row].Tag;
			VoiceMail voiceMailOld=voiceMailCur.Copy();
			voiceMailCur.Note=gridVoiceMails.GetText(e.Row,e.Col);
			VoiceMails.Update(voiceMailCur,voiceMailOld);
			if(voiceMailCur.Note != voiceMailOld.Note) {
				Signalods.Insert(new Signalod { IType=InvalidType.VoiceMails });
			}
		}

		///<summary>Goes to the chart module of the patient of the selected voicemail.</summary>
		private void GoToChart() {
			if(gridVoiceMails.SelectedCell.Y==-1 || gridVoiceMails.ListGridRows.Count<=gridVoiceMails.SelectedCell.Y
				|| !Security.IsAuthorized(Permissions.ChartModule)) {
				return;
			}
			VoiceMail voiceMailCur=(VoiceMail)gridVoiceMails.ListGridRows[gridVoiceMails.SelectedCell.Y].Tag;
			GotoModule.GotoChart(voiceMailCur.PatNum);
		}

		///<summary>Returns true if the VM was successfully sent to self.</summary>
		private bool SendToMe() {
			if(gridVoiceMails.SelectedCell.Y==-1 || gridVoiceMails.ListGridRows.Count<=gridVoiceMails.SelectedCell.Y) {
				return false;
			}
			VoiceMail voiceMailCur=(VoiceMail)gridVoiceMails.ListGridRows[gridVoiceMails.SelectedCell.Y].Tag;
			VoiceMail voiceMailOld=voiceMailCur.Copy();
			if(voiceMailCur.UserNum==Security.CurUser.UserNum) {//Trying to send a VM to themselves that is already theirs
				return true;//Save a couple trips to the database
			}
			VoiceMail voiceMailUpdated=VoiceMails.GetOne(voiceMailCur.VoiceMailNum);
			if(voiceMailUpdated==null || voiceMailCur.StatusVM==VoiceMailStatus.Deleted) {
				MsgBox.Show(this,"This voice mail has been deleted.");
				FillGrid();
				return false;
			}
			DateTime dateTimeNow=MiscData.GetNowDateTime();
			if((voiceMailCur.UserNum==0 //The tech is trying to send an unclaimed VM to themselves.
				&& voiceMailUpdated.UserNum!=0) //Someone else has already claimed it.
				|| voiceMailUpdated.DateClaimed > dateTimeNow.AddSeconds(-8))//Someone has claimed it in the last 8 seconds
			{
				MsgBox.Show(this,"This voice mail has just been claimed by someone else.");
				FillGrid();
				return false;
			}
			if(voiceMailUpdated.UserNum!=0
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"This voice mail has been claimed by someone else. Are you sure you want to send it to yourself?")) 
			{
				return false;
			}
			voiceMailCur.UserNum=Security.CurUser.UserNum;
			voiceMailCur.DateClaimed=dateTimeNow;
			VoiceMails.Update(voiceMailCur,voiceMailOld);
			Signalods.Insert(new Signalod { IType=InvalidType.VoiceMails });
			FillGrid();
			return true;
		}

		///<summary>Returns true if a task was inserted into the DB, when true formTaskEdit is set. Otherwise null.</summary>
		private bool TryCreateTaskAndForm(out FormTaskEdit formTaskEdit) {
			formTaskEdit=null;
			if(gridVoiceMails.SelectedCell.Y==-1 || gridVoiceMails.ListGridRows.Count<=gridVoiceMails.SelectedCell.Y) {
				MsgBox.Show(this,"No voice mail selected");
				return false;
			}
			VoiceMail voiceMailCur=(VoiceMail)gridVoiceMails.ListGridRows[gridVoiceMails.SelectedCell.Y].Tag;
			VoiceMail voiceMailOld=voiceMailCur.Copy();
			if(voiceMailCur.PatNum==0) {//Multiple patients had a match for the phone number
				using FormPatientSelect FormPS=new FormPatientSelect(new Patient { HmPhone=voiceMailCur.PhoneNumber });
				if(FormPS.ShowDialog()!=DialogResult.OK) {
					return false;
				}
				voiceMailCur.PatNum=FormPS.SelectedPatNum;
				VoiceMails.Update(voiceMailCur,voiceMailOld);
				FillGrid();
			}
			Task task=new Task() { TaskListNum=-1 };//don't show it in any list yet.
			Tasks.Insert(task);
			Task taskOld=task.Copy();
			task.UserNum=Security.CurUser.UserNum;
			task.TaskListNum=Tasks.TriageTaskListNum;
			task.DateTimeEntry=voiceMailCur.DateCreated;
			task.PriorityDefNum=Tasks.TriageBlueNum;
			task.ObjectType=TaskObjectType.Patient;
			task.KeyNum=voiceMailCur.PatNum;
			task.Descript="VM "+TelephoneNumbers.AutoFormat(voiceMailCur.PhoneNumber)+" ";
			FormTaskEdit FormTE=new FormTaskEdit(task,taskOld);
			FormTE.IsNew=true;
			formTaskEdit=FormTE;
			return true;
		}

		private void butCreateTask_Click(object sender,EventArgs e) {
			FormTaskEdit formTaskEdit;
			if(!TryCreateTaskAndForm(out formTaskEdit)) {
				return;
			}
			formTaskEdit.Show();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridVoiceMails.SelectedCell.Y==-1 || gridVoiceMails.ListGridRows.Count<=gridVoiceMails.SelectedCell.Y) {
				MsgBox.Show(this,"No voice mail selected");
				return;
			}
			VoiceMail voiceMailCur=(VoiceMail)gridVoiceMails.ListGridRows[gridVoiceMails.SelectedCell.Y].Tag;
			if(voiceMailCur.StatusVM==VoiceMailStatus.Deleted) {
				MsgBox.Show(this,"Voice mail is already deleted.");
				return;
			}
			//Check if the voice mail has been altered by another user before we delete it.
			VoiceMail voiceMailDb=VoiceMails.GetOne(voiceMailCur.VoiceMailNum);
			if(voiceMailCur.UserNum != voiceMailDb.UserNum
				|| voiceMailCur.PatNum != voiceMailDb.PatNum
				|| voiceMailCur.StatusVM != voiceMailDb.StatusVM
				|| voiceMailCur.FileName != voiceMailDb.FileName) 
			{
				MsgBox.Show(this,"Another user has modified this voice mail.");
				FillGrid();
				return;
			}
			try {
				VoiceMails.Archive(voiceMailCur);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to archive deleted voice mail:")
					+"\r\n"+ex.Message);
				return;
			}
			axWindowsMediaPlayer.close();//No need to keep playing a deleted message.
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormVoiceMails_FormClosing(object sender,FormClosingEventArgs e) {
			//Change the ClockStatus to Available if the logged on user is clocked in and the same user as the extension.
			if(FormOpenDental.PhoneTile.PhoneCur!=null
				&& ClockEvents.IsClockedIn(Security.CurUser.EmployeeNum)
				&& Security.CurUser.EmployeeNum==FormOpenDental.PhoneTile.PhoneCur.EmployeeNum)
			{
				FormOpenDental.S_SetPhoneStatusAvailable();
			}
		}
	}
}