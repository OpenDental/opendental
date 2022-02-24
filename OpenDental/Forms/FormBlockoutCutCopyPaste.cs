using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormBlockoutCutCopyPaste:FormODBase {
		private static DateTime DateCopyStart=DateTime.MinValue;
		private static DateTime DateCopyEnd=DateTime.MinValue;
		public long ApptViewNumCur;
		private static long ApptViewNumPrevious;
		public DateTime DateSelected;

		private bool _isWeekend {
			get {
				return DateSelected.DayOfWeek==DayOfWeek.Saturday || DateSelected.DayOfWeek==DayOfWeek.Sunday;
			}
		}

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
			if(_isWeekend) {
				checkWeekend.Checked=true;
			}			
			if(ApptViewNumCur!=ApptViewNumPrevious){
				DateCopyStart=DateTime.MinValue;
				DateCopyEnd=DateTime.MinValue;
			}
			FillClipboard();
			ApptViewNumPrevious=ApptViewNumCur;//remember the appt view for next time.
		}

		private void butClearDay_Click(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				string clincAbbr=(Clinics.ClinicNum==0?Lan.g(this,"Headquarters"):Clinics.GetAbbr(Clinics.ClinicNum));
				if(MessageBox.Show(Lan.g(this,"Clear all blockouts for day for clinic: ")+clincAbbr+Lan.g(this,"?")+"\r\n"
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
			if(DateCopyStart.Year<1880){
				textClipboard.Text="";
			}
			else if(DateCopyStart==DateCopyEnd) {
				textClipboard.Text=DateCopyStart.ToShortDateString();
			}
			else {
				textClipboard.Text=DateCopyStart.ToShortDateString()+"-"+DateCopyEnd.ToShortDateString();
			}
		}

		private void butCopyDay_Click(object sender,EventArgs e) {
			DateCopyStart=DateSelected;
			DateCopyEnd=DateSelected;
			Close();
		}

		private void butCopyWeek_Click(object sender,EventArgs e) {
			//Always start week on Monday
			if(DateSelected.DayOfWeek==DayOfWeek.Sunday) {//if selecting Sunday, go back to the previous Monday.
				DateCopyStart=DateSelected.AddDays(-6);
			}
			else {//Any other day. eg Wed.AddDays(1-3)=Wed.AddDays(-2)=Monday
				DateCopyStart=DateSelected.AddDays(1-(int)DateSelected.DayOfWeek);//eg Wed.AddDays(1-3)=Wed.AddDays(-2)=Monday
			}
			if(checkWeekend.Checked){
				DateCopyEnd=DateCopyStart.AddDays(6);
			}
			else{
				DateCopyEnd=DateCopyStart.AddDays(4);
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
			if(DateCopyStart.Year < 1880) {
				MsgBox.Show(this,"Please copy a selection to the clipboard first.");
				return;
			}
			if(_isWeekend && !checkWeekend.Checked) {//user is trying to 'paste' onto a weekend date
				MsgBox.Show(this,"You must check 'Include Weekends' if you would like to paste into weekends.");
				return;
			}
			//calculate which day or week is currently selected.
			DateTime dateSelectedStart;
			DateTime dateSelectedEnd;
			bool isWeek=DateCopyStart!=DateCopyEnd;
			if(isWeek) {
				//Always start week on Monday
				if(DateSelected.DayOfWeek==DayOfWeek.Sunday) {//if selecting Sunday, go back to the previous Monday.
					dateSelectedStart=DateSelected.AddDays(-6);
				}
				else {//Any other day. eg Wed.AddDays(1-3)=Wed.AddDays(-2)=Monday
					dateSelectedStart=DateSelected.AddDays(1-(int)DateSelected.DayOfWeek);//eg Wed.AddDays(1-3)=Wed.AddDays(-2)=Monday
				}
				//DateCopyEnd is greater than DateCopyStart and is either 4 days greater or 6 days greater, so clear/paste the same number of days
				dateSelectedEnd=dateSelectedStart.AddDays((DateCopyEnd-DateCopyStart).Days);
			}
			else {
				dateSelectedStart=DateSelected;
				dateSelectedEnd=DateSelected;
			}
			//When pasting, it's not allowed to paste back over the same day or week.
			if(dateSelectedStart==DateCopyStart && numRepeat==1) {
				MsgBox.Show(this,"Not allowed to paste back onto the same date as is on the clipboard.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			string errors=Schedules.CopyBlockouts(ApptViewNumCur,isWeek,checkWeekend.Checked,checkReplace.Checked,DateCopyStart,DateCopyEnd,
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