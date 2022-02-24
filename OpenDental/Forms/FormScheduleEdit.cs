using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public class FormScheduleEdit : FormODBase	{
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label labelStop;
		private System.Windows.Forms.Label labelStart;
		private OpenDental.ODtextBox textNote;
		private System.Windows.Forms.Label label4;
		private ComboBox comboStop;
		private ComboBox comboStart;
		private OpenDental.UI.ListBoxOD listBoxOps;
		private Label labelOps;
		public Schedule SchedCur;
		///<summary>Filters the list of operatories available to the clinic passed in.  Set to 0 to show all operatories.  Also the clinic selected by
		///default for holidays and provider notes.</summary>
		public long ClinicNum;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		///<summary>All ops if clinics not enabled, otherwise all ops for ClinicNum.</summary>
		private List<Operatory> _listOps;
		///<summary>List of schedules for the day set from FormScheduleDayEdit filled with the filtered list of schedules for the day.
		///Used to ensure there is only one holiday schedule item per day/clinic, since this list has not been synced to the db yet.</summary>
		public List<Schedule> ListScheds;
		private bool _isHolidayOrNote;
		public List<long> ListProvNums;

		///<summary></summary>
		public FormScheduleEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScheduleEdit));
			this.labelStop = new System.Windows.Forms.Label();
			this.labelStart = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboStop = new System.Windows.Forms.ComboBox();
			this.comboStart = new System.Windows.Forms.ComboBox();
			this.listBoxOps = new OpenDental.UI.ListBoxOD();
			this.labelOps = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.SuspendLayout();
			// 
			// labelStop
			// 
			this.labelStop.Location = new System.Drawing.Point(6, 39);
			this.labelStop.Name = "labelStop";
			this.labelStop.Size = new System.Drawing.Size(89, 16);
			this.labelStop.TabIndex = 9;
			this.labelStop.Text = "Stop Time";
			this.labelStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStart
			// 
			this.labelStart.Location = new System.Drawing.Point(6, 12);
			this.labelStart.Name = "labelStart";
			this.labelStart.Size = new System.Drawing.Size(89, 16);
			this.labelStart.TabIndex = 7;
			this.labelStart.Text = "Start Time";
			this.labelStart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(97, 92);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Schedule;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(220, 113);
			this.textNote.TabIndex = 15;
			this.textNote.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 93);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(89, 16);
			this.label4.TabIndex = 16;
			this.label4.Text = "Note";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStop
			// 
			this.comboStop.FormattingEnabled = true;
			this.comboStop.Location = new System.Drawing.Point(97, 38);
			this.comboStop.MaxDropDownItems = 48;
			this.comboStop.Name = "comboStop";
			this.comboStop.Size = new System.Drawing.Size(120, 21);
			this.comboStop.TabIndex = 25;
			// 
			// comboStart
			// 
			this.comboStart.FormattingEnabled = true;
			this.comboStart.Location = new System.Drawing.Point(97, 11);
			this.comboStart.MaxDropDownItems = 48;
			this.comboStart.Name = "comboStart";
			this.comboStart.Size = new System.Drawing.Size(120, 21);
			this.comboStart.TabIndex = 24;
			// 
			// listOps
			// 
			this.listBoxOps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxOps.IntegralHeight = false;
			this.listBoxOps.Location = new System.Drawing.Point(348, 34);
			this.listBoxOps.Name = "listOps";
			this.listBoxOps.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxOps.Size = new System.Drawing.Size(243, 349);
			this.listBoxOps.TabIndex = 27;
			// 
			// labelOps
			// 
			this.labelOps.Location = new System.Drawing.Point(348, 2);
			this.labelOps.Name = "labelOps";
			this.labelOps.Size = new System.Drawing.Size(243, 28);
			this.labelOps.TabIndex = 26;
			this.labelOps.Text = "Operatories. Usually, do not select operatories.  Only used to override default o" +
    "peratory provider.";
			this.labelOps.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(516, 393);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 14;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(428, 393);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 12;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(61, 65);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(234, 21);
			this.comboClinic.TabIndex = 94;
			this.comboClinic.Visible = false;
			// 
			// FormScheduleEdit
			// 
			this.ClientSize = new System.Drawing.Size(603, 431);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.listBoxOps);
			this.Controls.Add(this.labelOps);
			this.Controls.Add(this.comboStop);
			this.Controls.Add(this.comboStart);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelStop);
			this.Controls.Add(this.labelStart);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormScheduleEdit";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Edit Schedule";
			this.Load += new System.EventHandler(this.FormScheduleEdit_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormScheduleEdit_Load(object sender, System.EventArgs e) {
			_isHolidayOrNote=(SchedCur.StartTime==TimeSpan.Zero && SchedCur.StopTime==TimeSpan.Zero);
			if(PrefC.HasClinicsEnabled) {
				if(ClinicNum==0) {
					Text+=" - "+Lan.g(this,"Headquarters");
				}
				else {
					string abbr=Clinics.GetAbbr(ClinicNum);
					if(!string.IsNullOrWhiteSpace(abbr)) {
						Text+=" - "+abbr;
					}
				}
				//if clinics are enabled and this is a holiday or practice note, set visible and fill the clinic combobox and private list of clinics
				if(_isHolidayOrNote && SchedCur.SchedType==ScheduleType.Practice) {
					comboClinic.Visible=true;//only visible for holidays and practice notes and only if clinics are enabled
					comboClinic.SelectedClinicNum=Clinics.ClinicNum;
				}
				else {
					comboClinic.Visible=false;
				}
			}
			textNote.Text=SchedCur.Note;
			if(_isHolidayOrNote) {
				comboStart.Visible=false;
				labelStart.Visible=false;
				comboStop.Visible=false;
				labelStop.Visible=false;
				listBoxOps.Visible=false;
				labelOps.Visible=false;
				textNote.Select();
				return;
			}
			//from here on, NOT a practice note or holiday
			DateTime time;
			for(int i=0;i<24;i++) {
				time=DateTime.Today+TimeSpan.FromHours(7)+TimeSpan.FromMinutes(30*i);
				comboStart.Items.Add(time.ToShortTimeString());
				comboStop.Items.Add(time.ToShortTimeString());
			}
			comboStart.Text=SchedCur.StartTime.ToShortTimeString();
			comboStop.Text=SchedCur.StopTime.ToShortTimeString();
			listBoxOps.Items.Add(Lan.g(this,"not specified"));
			//filter list if using clinics and if a clinic filter was passed in to only ops assigned to the specified clinic, otherwise all non-hidden ops
			_listOps=Operatories.GetDeepCopy(true);
			if(PrefC.HasClinicsEnabled && ClinicNum>0) {
				_listOps.RemoveAll(x => x.ClinicNum!=ClinicNum);
			}
			for(int i=0;i<_listOps.Count;i++) {
				listBoxOps.Items.Add(_listOps[i].OpName);
				if(SchedCur.Ops.Contains(_listOps[i].OperatoryNum)) {
					listBoxOps.SetSelected(listBoxOps.Items.Count-1);
				}
			}
			listBoxOps.SetSelected(0,listBoxOps.SelectedIndices.Count==0);//select 'not specified' if no ops were selected in the loop
			comboStart.Select();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			#region Validation
			DateTime startDateT=DateTime.MinValue;
			DateTime stopDateT=DateTime.MinValue;
			if(!_isHolidayOrNote) {
				if(listBoxOps.SelectedIndices.Count==0){
					MsgBox.Show(this,"Please select ops first.");
					return;
				}
				if(listBoxOps.SelectedIndices.Count>1 && listBoxOps.SelectedIndices.Contains(0)){
					MsgBox.Show(this,"Invalid selection of ops.");
					return;
				}
				startDateT=PIn.DateT(comboStart.Text);
				stopDateT=PIn.DateT(comboStop.Text);
				if(startDateT==DateTime.MinValue || stopDateT==DateTime.MinValue) {
					MsgBox.Show(this,"Incorrect time format");
					return;
				}
				if(startDateT>stopDateT) {
					MsgBox.Show(this,"Stop time must be later than start time.");
					return;
				}
				List<Schedule> listProvSchedsOnly=ListScheds.FindAll(x=>x.SchedType==ScheduleType.Provider);
				List<long> listSelectedOps=new List<long>();
				if(listBoxOps.SelectedIndices.Contains(0)) {
					//not specified, so empty list
				}
				else {
					for(int i=0;i<listBoxOps.SelectedIndices.Count;i++) {
						listSelectedOps.Add(_listOps[listBoxOps.SelectedIndices[i]-1].OperatoryNum);
					}
				}
				SchedCur.Ops=listSelectedOps.ToList();//deep copy of list. (because it is value type.)
				SchedCur.StartTime=startDateT.TimeOfDay;
				SchedCur.StopTime=stopDateT.TimeOfDay;
				List<long> listProvsOverlap;
				//====================Pre-Emptive Overlaps====================
				//Because this window is explicitly designed for one start and one stop time we know that there will be overlapping if multiple providers are 
				//selected with at least one specific operatory selected.
				if(ListProvNums.Count > 1 && listSelectedOps.Count > 0) {
					listProvsOverlap=ListProvNums.Distinct().ToList();
				}
				else {//Go see if there is going to be overlapping issues with this new schedule.
					listProvsOverlap=Schedules.GetOverlappingSchedProvNums(ListProvNums,SchedCur,listProvSchedsOnly,listSelectedOps);
				}
				if(listProvsOverlap.Count>0 && MessageBox.Show(Lan.g(this,"Overlapping provider schedules detected, would you like to continue anyway?")+"\r\n"+Lan.g(this,"Providers affected")+":\r\n  "
					+string.Join("\r\n  ",listProvsOverlap.Select(x=>Providers.GetLongDesc(x))),"",MessageBoxButtons.YesNo)!=DialogResult.Yes) 
				{ 
					return;
				}
			}
			else if(SchedCur.Status!=SchedStatus.Holiday && textNote.Text=="") {//don't allow blank schedule notes
				MsgBox.Show(this,"Please enter a note first.");
				return;
			}
			long clinicNum=0;
			if(_isHolidayOrNote && SchedCur.SchedType==ScheduleType.Practice && PrefC.HasClinicsEnabled) {//prov notes do not have a clinic
				clinicNum=comboClinic.SelectedClinicNum;
				if(SchedCur.Status==SchedStatus.Holiday) {//duplicate holiday check
					List<Schedule> listScheds=ListScheds.FindAll(x => x.SchedType==ScheduleType.Practice && x.Status==SchedStatus.Holiday);//scheds in local list
					listScheds.AddRange(Schedules.GetAllForDateAndType(SchedCur.SchedDate,ScheduleType.Practice)
						.FindAll(x => x.ScheduleNum!=SchedCur.ScheduleNum
							&& x.Status==SchedStatus.Holiday
							&& listScheds.All(y => y.ScheduleNum!=x.ScheduleNum)));//add any in db that aren't in local list
					if(listScheds.Any(x => x.ClinicNum==0 || x.ClinicNum==clinicNum)//already a holiday for HQ in db or duplicate holiday for a clinic
						|| (clinicNum==0 && listScheds.Count>0))//OR trying to create a HQ holiday when a clinic already has one for this day
					{
						MsgBox.Show(this,"There is already a Holiday for the practice or clinic on this date.");
						return;
					}
				}
			}
			#endregion Validation
			#region Set Schedule Fields
      SchedCur.Note=textNote.Text;
			SchedCur.Ops=new List<long>();
			if(listBoxOps.SelectedIndices.Count>0 && !listBoxOps.SelectedIndices.Contains(0)) {
				for(int i=0;i<listBoxOps.SelectedIndices.Count;i++) {
					SchedCur.Ops.Add(_listOps[listBoxOps.SelectedIndices[i]-1].OperatoryNum);
				}
			}
			SchedCur.ClinicNum=clinicNum;//0 if HQ selected or clinics not enabled or not a holiday or practice note
			#endregion Set Schedule Fields
			DialogResult=DialogResult.OK;		  
    }

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}






