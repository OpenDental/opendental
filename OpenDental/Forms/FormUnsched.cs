using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public class FormUnsched:FormODBase {
		private IContainer components;
		private OpenDental.UI.Button butClose;
		///<summary></summary>
		public static string procsForCur;
		private OpenDental.UI.GridOD grid;
		private OpenDental.UI.Button butPrint;
		private List<Appointment> _listUnschedApt;
		private bool headingPrinted;
		private int headingPrintH;
		private int pagesPrinted;
		private ComboBox comboOrder;
		private Label label1;
		private ComboBox comboProv;
		private Label label4;
		private OpenDental.UI.Button butRefresh;
		private ComboBox comboSite;
		private Label labelSite;
		private CheckBox checkBrokenAppts;
		private ContextMenuStrip menuRightClick;
		private ComboBoxClinicPicker comboClinic;
		private Label label6;
		private ToolStripMenuItem toolStripMenuItemSelectPatient;
		private ToolStripMenuItem toolStripMenuItemSeeChart;
		private ToolStripMenuItem toolStripMenuItemSendToPinboard;
		private ToolStripMenuItem toolStripMenuItemDelete;
		private ODDateRangePicker dateRangePicker;
		private List<long> _listAptSelected;
		private List<Provider> _listProviders;
		private ODCodeRangeFilter codeRangeFilter;
		private MenuOD menuMain;
		private List<Site> _listSites;

		///<summary>PatientGoTo must be set before calling Show() or ShowDialog().</summary>
		public FormUnsched(){
			InitializeComponent();// Required for Windows Form Designer support
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUnsched));
			this.butClose = new OpenDental.UI.Button();
			this.grid = new OpenDental.UI.GridOD();
			this.menuRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemSelectPatient = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSeeChart = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSendToPinboard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.butPrint = new OpenDental.UI.Button();
			this.comboOrder = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.comboSite = new System.Windows.Forms.ComboBox();
			this.labelSite = new System.Windows.Forms.Label();
			this.checkBrokenAppts = new System.Windows.Forms.CheckBox();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.label6 = new System.Windows.Forms.Label();
			this.dateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.codeRangeFilter = new OpenDental.UI.ODCodeRangeFilter();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.menuRightClick.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(810, 655);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(87, 24);
			this.butClose.TabIndex = 7;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// grid
			// 
			this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grid.ContextMenuStrip = this.menuRightClick;
			this.grid.Location = new System.Drawing.Point(10, 102);
			this.grid.Name = "grid";
			this.grid.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.grid.Size = new System.Drawing.Size(775, 577);
			this.grid.TabIndex = 8;
			this.grid.Title = "Unscheduled List";
			this.grid.TranslationName = "TableUnsched";
			this.grid.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellDoubleClick);
			this.grid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.grid_MouseUp);
			// 
			// menuRightClick
			// 
			this.menuRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSelectPatient,
            this.toolStripMenuItemSeeChart,
            this.toolStripMenuItemSendToPinboard,
            this.toolStripMenuItemDelete});
			this.menuRightClick.Name = "menuRightClick";
			this.menuRightClick.Size = new System.Drawing.Size(166, 92);
			// 
			// toolStripMenuItemSelectPatient
			// 
			this.toolStripMenuItemSelectPatient.Name = "toolStripMenuItemSelectPatient";
			this.toolStripMenuItemSelectPatient.Size = new System.Drawing.Size(165, 22);
			this.toolStripMenuItemSelectPatient.Text = "Select Patient";
			this.toolStripMenuItemSelectPatient.Click += new System.EventHandler(this.menuRight_click);
			// 
			// toolStripMenuItemSeeChart
			// 
			this.toolStripMenuItemSeeChart.Name = "toolStripMenuItemSeeChart";
			this.toolStripMenuItemSeeChart.Size = new System.Drawing.Size(165, 22);
			this.toolStripMenuItemSeeChart.Text = "See Chart";
			this.toolStripMenuItemSeeChart.Click += new System.EventHandler(this.menuRight_click);
			// 
			// toolStripMenuItemSendToPinboard
			// 
			this.toolStripMenuItemSendToPinboard.Name = "toolStripMenuItemSendToPinboard";
			this.toolStripMenuItemSendToPinboard.Size = new System.Drawing.Size(165, 22);
			this.toolStripMenuItemSendToPinboard.Text = "Send to Pinboard";
			this.toolStripMenuItemSendToPinboard.Click += new System.EventHandler(this.menuRight_click);
			// 
			// toolStripMenuItemDelete
			// 
			this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
			this.toolStripMenuItemDelete.Size = new System.Drawing.Size(165, 22);
			this.toolStripMenuItemDelete.Text = "Delete";
			this.toolStripMenuItemDelete.Click += new System.EventHandler(this.menuRight_click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(812, 607);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(87, 24);
			this.butPrint.TabIndex = 21;
			this.butPrint.Text = "Print List";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// comboOrder
			// 
			this.comboOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboOrder.Location = new System.Drawing.Point(97, 30);
			this.comboOrder.MaxDropDownItems = 40;
			this.comboOrder.Name = "comboOrder";
			this.comboOrder.Size = new System.Drawing.Size(133, 21);
			this.comboOrder.TabIndex = 35;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(23, 34);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 14);
			this.label1.TabIndex = 34;
			this.label1.Text = "Order by";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProv
			// 
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.Location = new System.Drawing.Point(319, 30);
			this.comboProv.MaxDropDownItems = 40;
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(181, 21);
			this.comboProv.TabIndex = 33;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(248, 34);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(69, 14);
			this.label4.TabIndex = 32;
			this.label4.Text = "Provider";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(813, 30);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(86, 24);
			this.butRefresh.TabIndex = 31;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// comboSite
			// 
			this.comboSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSite.Location = new System.Drawing.Point(319, 55);
			this.comboSite.MaxDropDownItems = 40;
			this.comboSite.Name = "comboSite";
			this.comboSite.Size = new System.Drawing.Size(181, 21);
			this.comboSite.TabIndex = 37;
			// 
			// labelSite
			// 
			this.labelSite.Location = new System.Drawing.Point(241, 58);
			this.labelSite.Name = "labelSite";
			this.labelSite.Size = new System.Drawing.Size(77, 14);
			this.labelSite.TabIndex = 36;
			this.labelSite.Text = "Site";
			this.labelSite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBrokenAppts
			// 
			this.checkBrokenAppts.AutoSize = true;
			this.checkBrokenAppts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBrokenAppts.Location = new System.Drawing.Point(65, 57);
			this.checkBrokenAppts.Name = "checkBrokenAppts";
			this.checkBrokenAppts.Size = new System.Drawing.Size(165, 17);
			this.checkBrokenAppts.TabIndex = 38;
			this.checkBrokenAppts.Text = "Include Broken Appointments";
			this.checkBrokenAppts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBrokenAppts.UseVisualStyleBackColor = true;
			// 
			// comboClinic
			// 
			this.comboClinic.IncludeAll = true;
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(547, 30);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(217, 21);
			this.comboClinic.TabIndex = 40;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(505, 58);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(77, 17);
			this.label6.TabIndex = 43;
			this.label6.Text = "Code Range";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// dateRangePicker
			// 
			this.dateRangePicker.BackColor = System.Drawing.SystemColors.Control;
			this.dateRangePicker.EnableWeekButtons = false;
			this.dateRangePicker.Location = new System.Drawing.Point(57, 79);
			this.dateRangePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.dateRangePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangePicker.Name = "dateRangePicker";
			this.dateRangePicker.Size = new System.Drawing.Size(453, 22);
			this.dateRangePicker.TabIndex = 47;
			// 
			// codeRangeFilter
			// 
			this.codeRangeFilter.Location = new System.Drawing.Point(584, 57);
			this.codeRangeFilter.Name = "codeRangeFilter";
			this.codeRangeFilter.Size = new System.Drawing.Size(160, 37);
			this.codeRangeFilter.TabIndex = 48;
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(909, 24);
			this.menuMain.TabIndex = 49;
			// 
			// FormUnsched
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(909, 696);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.codeRangeFilter);
			this.Controls.Add(this.dateRangePicker);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.checkBrokenAppts);
			this.Controls.Add(this.comboOrder);
			this.Controls.Add(this.comboSite);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelSite);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.menuMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormUnsched";
			this.Text = "Unscheduled List";
			this.Load += new System.EventHandler(this.FormUnsched_Load);
			this.Shown += new System.EventHandler(this.FormUnsched_Shown);
			this.menuRightClick.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormUnsched_Load(object sender, System.EventArgs e) {
			Cursor=Cursors.WaitCursor;
			comboOrder.Items.Add(Lan.g(this,"UnschedStatus"));
			comboOrder.Items.Add(Lan.g(this,"Alphabetical"));
			comboOrder.Items.Add(Lan.g(this,"Date"));
			comboOrder.SelectedIndex=0;
			comboProv.Items.Add(Lan.g(this,"All"));
			comboProv.SelectedIndex=0;
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProv.Items.Add(_listProviders[i].GetLongDesc());
			}
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				comboSite.Visible=false;
				labelSite.Visible=false;
			}
			else{
				comboSite.Items.Add(Lan.g(this,"All"));
				comboSite.SelectedIndex=0;
				_listSites=Sites.GetDeepCopy();
				for(int i=0;i<_listSites.Count;i++) {
					comboSite.Items.Add(_listSites[i].Description);
				}
			}
			if(PrefC.GetBool(PrefName.EnterpriseApptList)){
				comboClinic.IncludeAll=false;
			}
			_listAptSelected=new List<long>();
			LayoutMenu();
			InitDateRange();
			Cursor=Cursors.Default;
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",setupToolStripMenuItem_Click));
			menuMain.EndUpdate();
		}

		///<summary>There is a bug in ODProgress.cs that forces windows that use a progress bar on load to go behind other applications. 
		///This is a temporary workaround until we decide how to address the issue.</summary>
		private void FormUnsched_Shown(object sender,EventArgs e) {
			FillGrid();
		}

		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormUnschedListSetup formSetup=new FormUnschedListSetup();
			if(formSetup.ShowDialog()==DialogResult.OK) {
				InitDateRange();
			}
		}

		private void InitDateRange() {
			int dayCount=PrefC.GetInt(PrefName.UnschedDaysPast);
			if(dayCount==-1) {
				//Set the text to blank
				dateRangePicker.SetDateTimeFrom(DateTime.MinValue);
			}
			else {
				dateRangePicker.SetDateTimeFrom(DateTime.Today.AddDays(-dayCount));
			}
			dayCount=PrefC.GetInt(PrefName.UnschedDaysFuture);
			if(dayCount==-1) {
				//Set the text to blank. We check for DateTime.MinValue in Appointments.RefreshUnsched() and modify the query to not have an end date.
				dateRangePicker.SetDateTimeTo(DateTime.MinValue);
			}
			else {
				dateRangePicker.SetDateTimeTo(DateTime.Today.AddDays(dayCount));
			}
		}

		private void menuRight_click(object sender,System.EventArgs e) {
			if(grid.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			switch(menuRightClick.Items.IndexOf((ToolStripMenuItem)sender)) {
				case 0:
					SelectPatient_Click();
					break;
				case 1:
					SeeChart_Click();
					break;
				case 2:
					SendPinboard_Click();
					break;
				case 3:
					Delete_Click();
					break;
			}
		}

		private void grid_MouseUp(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right && grid.SelectedIndices.Length>0) {
				//To maintain legacy behavior we will use the last selected index if multiple are selected.
				Patient pat=Patients.GetLim(_listUnschedApt[grid.SelectedIndices[grid.SelectedIndices.Length-1]].PatNum);
				toolStripMenuItemSelectPatient.Text=Lan.g(grid.TranslationName,"Select Patient")+" ("+pat.GetNameFL()+")";
			}
		}

		private void SelectPatient_Click() {
			//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			long patNum=_listUnschedApt[grid.SelectedIndices[grid.SelectedIndices.Length-1]].PatNum;
			Patient pat=Patients.GetPat(patNum);
			FormOpenDental.S_Contr_PatientSelected(pat,true);
		}

		///<summary>If multiple patients are selected in UnchedList, will select the last patient to remain consistent with sending to pinboard behavior.</summary>
		private void SeeChart_Click() {
			if(grid.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			Patient pat=Patients.GetPat(_listUnschedApt[grid.SelectedIndices[grid.SelectedIndices.Length-1]].PatNum);//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			FormOpenDental.S_Contr_PatientSelected(pat,false);
			GotoModule.GotoChart(pat.PatNum);
		}

		private void SendPinboard_Click() {
			if(grid.SelectedIndices.Length==0) {
				MsgBox.Show("Please select an appointment first.");
				return;
			}
			_listAptSelected.Clear();
			int patsRestricted=0;
			for(int i=0;i<grid.SelectedIndices.Length;i++) {
				if(PatRestrictionL.IsRestricted(_listUnschedApt[grid.SelectedIndices[i]].PatNum,PatRestrict.ApptSchedule,true)) {
					patsRestricted++;
					continue;
				}
				_listAptSelected.Add(_listUnschedApt[grid.SelectedIndices[i]].AptNum);
			}
			if(patsRestricted>0) {
				if(_listAptSelected.Count==0) {
					MsgBox.Show("All selected appointments have been skipped due to patient restriction "
						+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)+".");
					return;
				}
				MessageBox.Show("Appointments skipped due to patient restriction "+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)
					+": "+patsRestricted+".");
			}
			GotoModule.PinToAppt(_listAptSelected,0);//This will send all appointments in _listAptSelected to the pinboard, and will select the patient attached to the last appointment in _listAptSelected.
		}

		private void Delete_Click() {
			if(!Security.IsAuthorized(Permissions.AppointmentEdit)) {
				return;
			}
			if(grid.SelectedIndices.Length>1) {
				if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Delete all selected appointments permanently?")) {
					return;
				}
			}
			List<Appointment> listApptsWithNote=new List<Appointment>();
			List<long> listSelectedAptNums=new List<long>();
			foreach(int i in grid.SelectedIndices) {
				listSelectedAptNums.Add(_listUnschedApt[i].AptNum);
				if(!string.IsNullOrEmpty(_listUnschedApt[i].Note)) {
					listApptsWithNote.Add(_listUnschedApt[i]);
				}
			}
			if(listApptsWithNote.Count>0) {//There were notes in the appointment(s) we are about to delete and we must ask if they want to save them in a commlog.
				string commlogMsg="";
				if(grid.SelectedIndices.Length==1) {
					commlogMsg=Commlogs.GetDeleteApptCommlogMessage(listApptsWithNote[0].Note,listApptsWithNote[0].AptStatus);
				}
				else {
					commlogMsg="One or more appointments have notes.  Save appointment notes in CommLogs?";
				}
				DialogResult result=MessageBox.Show(commlogMsg,"Question...",MessageBoxButtons.YesNoCancel);
				if(result==DialogResult.Cancel) {
					return;
				}
				else if(result==DialogResult.Yes) {
					foreach(Appointment apptCur in listApptsWithNote) {
						Commlog commlogCur=new Commlog();
						commlogCur.PatNum=apptCur.PatNum;
						commlogCur.CommDateTime=DateTime.Now;
						commlogCur.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
						commlogCur.Note=Lan.g(this,"Deleted Appt. & saved note")+": ";
						if(apptCur.ProcDescript!="") {
							commlogCur.Note+=apptCur.ProcDescript+": ";
						}
						commlogCur.Note+=apptCur.Note;
						commlogCur.UserNum=Security.CurUser.UserNum;
						//there is no dialog here because it is just a simple entry
						Commlogs.Insert(commlogCur);
					}
				}
			}
			Appointments.Delete(listSelectedAptNums);
			foreach(int i in grid.SelectedIndices) {
				SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,_listUnschedApt[i].PatNum,
					Lan.g(this,"Appointment deleted from the Unscheduled list."),_listUnschedApt[i].AptNum,_listUnschedApt[i].DateTStamp);
			}
			FillGrid();
		}

		private void FillGrid() {
			string order="";
			switch(comboOrder.SelectedIndex) {
				case 0:
					order="status";
					break;
				case 1:
					order="alph";
					break;
				case 2:
					order="date";
					break;
			}
			long provNum=0;
			if(comboProv.SelectedIndex!=0) {
				provNum=_listProviders[comboProv.SelectedIndex-1].ProvNum;
			}
			long siteNum=0;
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && comboSite.SelectedIndex!=0) {
				siteNum=_listSites[comboSite.SelectedIndex-1].SiteNum;
			}
			bool showBrokenAppts;
			showBrokenAppts=checkBrokenAppts.Checked;
			long clinicNum=PrefC.HasClinicsEnabled ? comboClinic.SelectedClinicNum : -1;
			Dictionary<long,string> dictPatNames=null;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				_listUnschedApt=Appointments.RefreshUnsched(order,provNum,siteNum,showBrokenAppts,clinicNum,
					codeRangeFilter.StartRange,codeRangeFilter.EndRange,dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo());
				dictPatNames=Patients.GetPatientNames(_listUnschedApt.Select(x => x.PatNum).ToList());
				};
			progressOD.ShowDialogProgress();
			int scrollVal=grid.ScrollValue;
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableUnsched","Patient"),140);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Date"),65);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","AptStatus"),90);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","UnschedStatus"),110);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Prov"),50);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Procedures"),150);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Notes"),200);
			grid.ListGridColumns.Add(col);
			grid.ListGridRows.Clear();
			GridRow row;
			foreach(Appointment apt in _listUnschedApt) {
				row=new GridRow();
				string patName=Lan.g(this,"UNKNOWN");
				dictPatNames.TryGetValue(apt.PatNum,out patName);
				row.Cells.Add(patName);
				if(apt.AptDateTime.Year < 1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(apt.AptDateTime.ToShortDateString());
				}
				if(apt.AptStatus == ApptStatus.Broken) {
					row.Cells.Add(Lan.g(this,"Broken"));
				}
				else {
					row.Cells.Add(Lan.g(this,"Unscheduled"));
				}
				row.Cells.Add(Defs.GetName(DefCat.RecallUnschedStatus,apt.UnschedStatus));
				row.Cells.Add(Providers.GetAbbr(apt.ProvNum));
				row.Cells.Add(apt.ProcDescript);
				row.Cells.Add(apt.Note);
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			grid.ScrollValue=scrollVal;
		}

		private void grid_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int currentSelection=e.Row;//tbApts.SelectedRow;
			int currentScroll=grid.ScrollValue;//tbApts.ScrollValue;
			Patient pat=Patients.GetPat(_listUnschedApt[e.Row].PatNum);//If multiple selected, just take the one that was clicked on.
			FormOpenDental.S_Contr_PatientSelected(pat,true);
			using FormApptEdit FormAE=new FormApptEdit(_listUnschedApt[e.Row].AptNum);
			FormAE.PinIsVisible=true;
			FormAE.ShowDialog();
			if(FormAE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormAE.PinClicked) {
				SendPinboard_Click();//Whatever they double clicked on will still be selected, just fire the event.
				DialogResult=DialogResult.OK;//this is an obsolete line. Window stays open.
			}
			else {
				FillGrid();
				grid.SetSelected(currentSelection,true);
				grid.ScrollValue=currentScroll;
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Unscheduled appointment list printed"));
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,"Unscheduled List");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				//yPos+=(int)g.MeasureString(text,headingFont).Height;
				//text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				//g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=25;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=grid.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

	}
}
