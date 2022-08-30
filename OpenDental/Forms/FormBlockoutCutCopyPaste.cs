using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormBlockoutCutCopyPaste:FormODBase {
		private static DateTime _dateCopyStart=DateTime.MinValue;
		private static DateTime _dateCopyEnd=DateTime.MinValue;
		public long ApptViewNum;
		private static long _apptViewNumPrevious;
		public DateTime DateSelected;

		///<summary></summary>
		public FormBlockoutCutCopyPaste()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormBlockoutCutCopyPaste_Load(object sender,EventArgs e) {
			if(DateSelected.DayOfWeek==DayOfWeek.Saturday || DateSelected.DayOfWeek==DayOfWeek.Sunday) {
				checkWeekend.Checked=true;
			}			
			if(ApptViewNum!=_apptViewNumPrevious){
				_dateCopyStart=DateTime.MinValue;
				_dateCopyEnd=DateTime.MinValue;
			}
			FillClipboard();
			_apptViewNumPrevious=ApptViewNum;//remember the appt view for next time.
		}

		private void butClearDay_Click(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				string clinicAbbr=(Clinics.ClinicNum==0?Lan.g(this,"Headquarters"):Clinics.GetAbbr(Clinics.ClinicNum));
				if(MessageBox.Show(Lan.g(this,"Clear all blockouts for day for clinic: ")+clinicAbbr+Lan.g(this,"?")+"\r\n"
					+Lan.g(this,"(This may include blockouts not shown in the current appointment view)")
					,Lan.g(this,"Clear Blockouts"),MessageBoxButtons.OKCancel)!=DialogResult.OK) 
				{ 
					return;
				}
				Schedules.ClearBlockoutsForClinic(Clinics.ClinicNum,DateSelected);//currently selected clinic only, works for daily or weekly
				Schedules.BlockoutLogHelper(BlockoutAction.Clear,dateTime:DateSelected,clinicNum:Clinics.ClinicNum);
			}
			else {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Clear all blockouts for day? (This may include blockouts not shown in the current appointment view)")) {
					return;
				}
				Schedules.ClearBlockoutsForDay(DateSelected);//works for daily or weekly
				Schedules.BlockoutLogHelper(BlockoutAction.Clear,dateTime:DateSelected);
			}
			Close();
		}

		private void FillClipboard(){
			if(_dateCopyStart.Year<1880){
				textClipboard.Text="";
			}
			else if(_dateCopyStart==_dateCopyEnd) {
				textClipboard.Text=_dateCopyStart.ToShortDateString();
			}
			else {
				textClipboard.Text=_dateCopyStart.ToShortDateString()+"-"+_dateCopyEnd.ToShortDateString();
			}
		}

		private void butCopyDay_Click(object sender,EventArgs e) {
			_dateCopyStart=DateSelected;
			_dateCopyEnd=DateSelected;
			Close();
		}

		private void butCopyWeek_Click(object sender,EventArgs e) {
			//Always start week on Monday
			if(DateSelected.DayOfWeek==DayOfWeek.Sunday) {//if selecting Sunday, go back to the previous Monday.
				_dateCopyStart=DateSelected.AddDays(-6);
			}
			else {//Any other day. eg Wed.AddDays(1-3)=Wed.AddDays(-2)=Monday
				_dateCopyStart=DateSelected.AddDays(1-(int)DateSelected.DayOfWeek);//eg Wed.AddDays(1-3)=Wed.AddDays(-2)=Monday
			}
			if(checkWeekend.Checked){
				_dateCopyEnd=_dateCopyStart.AddDays(6);
			}
			else{
				_dateCopyEnd=_dateCopyStart.AddDays(4);
			}
			Close();
		}

		private void butPaste_Click(object sender,EventArgs e) {
			CopyOverBlockouts(1);
		}

		private void butRepeat_Click(object sender,EventArgs e) {
			try {
				int.Parse(textRepeat.Text);
			}
			catch {
				MsgBox.Show(this,"Please fix number box first.");
				return;
			}
			CopyOverBlockouts(PIn.Int(textRepeat.Text));
		}

		private void CopyOverBlockouts(int numRepeat) {
			if(_dateCopyStart.Year < 1880) {
				MsgBox.Show(this,"Please copy a selection to the clipboard first.");
				return;
			}
			if(DateSelected.DayOfWeek==DayOfWeek.Saturday || DateSelected.DayOfWeek==DayOfWeek.Sunday) {//copying from a weekend
				if(!checkWeekend.Checked) {//but they didn't indicate pasting onto weekends
					MsgBox.Show(this,"You must check 'Include Weekends' if you would like to paste into weekends.");
					return;
				}
			}
			//calculate which day or week is currently selected.
			DateTime dateSelectedStart;
			DateTime dateSelectedEnd;
			bool isWeek=_dateCopyStart!=_dateCopyEnd;
			if(isWeek) {
				//Always start week on Monday
				if(DateSelected.DayOfWeek==DayOfWeek.Sunday) {//if selecting Sunday, go back to the previous Monday.
					dateSelectedStart=DateSelected.AddDays(-6);
				}
				else {//Any other day. eg Wed.AddDays(1-3)=Wed.AddDays(-2)=Monday
					dateSelectedStart=DateSelected.AddDays(1-(int)DateSelected.DayOfWeek);//eg Wed.AddDays(1-3)=Wed.AddDays(-2)=Monday
				}
				//DateCopyEnd is greater than DateCopyStart and is either 4 days greater or 6 days greater, so clear/paste the same number of days
				dateSelectedEnd=dateSelectedStart.AddDays((_dateCopyEnd-_dateCopyStart).Days);
			}
			else {
				dateSelectedStart=DateSelected;
				dateSelectedEnd=DateSelected;
			}
			//When pasting, it's not allowed to paste back over the same day or week.
			if(dateSelectedStart==_dateCopyStart && numRepeat==1) {
				MsgBox.Show(this,"Not allowed to paste back onto the same date as is on the clipboard.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			string errors=Schedules.CopyBlockouts(ApptViewNum,isWeek,checkWeekend.Checked,checkReplace.Checked,_dateCopyStart,_dateCopyEnd,
				dateSelectedStart,dateSelectedEnd,numRepeat);
			Cursor=Cursors.Default;
			if(!string.IsNullOrEmpty(errors)) {
				MessageBox.Show(errors);//Error was translated inside of the S class method.
				return;
			}
			Close();
		}
	}
}