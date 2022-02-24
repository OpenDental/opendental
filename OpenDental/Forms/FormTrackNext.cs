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
	/// <summary>The Next appoinment tracking tool.</summary>
	public class FormTrackNext : FormODBase {
		private OpenDental.UI.Button butClose;
		private List<Appointment> _listPlannedAppts;
		private OpenDental.UI.GridOD gridMain;
		private ComboBox comboProv;
		private Label label4;
		private OpenDental.UI.Button butRefresh;
		private ComboBox comboOrder;
		private Label label1;
		private OpenDental.UI.Button butPrint;
		private int pagesPrinted;
		private bool headingPrinted;
		private ComboBox comboSite;
		private Label labelSite;
		private int headingPrintH;
		private ComboBoxClinicPicker comboClinic;
		private IContainer components;
		private ToolStripMenuItem toolStripMenuItemSelectPatient;
		private ToolStripMenuItem toolStripMenuItemSeeChart;
		private ToolStripMenuItem toolStripMenuItemSendToPinboard;
		private ODDateRangePicker dateRangePicker;
		private Label label6;
		private ContextMenuStrip menuRightClick;
		private List<Provider> _listProviders;
		private ODCodeRangeFilter codeRangeFilter;
		private MenuOD menuMain;
		private List<Site> _listSites;

		///<summary>PatientGoTo must be set before calling Show() or ShowDialog().</summary>
		public FormTrackNext(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTrackNext));
			this.butClose = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.menuRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItemSelectPatient = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSeeChart = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemSendToPinboard = new System.Windows.Forms.ToolStripMenuItem();
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.comboOrder = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butPrint = new OpenDental.UI.Button();
			this.comboSite = new System.Windows.Forms.ComboBox();
			this.labelSite = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.dateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.label6 = new System.Windows.Forms.Label();
			this.codeRangeFilter = new OpenDental.UI.ODCodeRangeFilter();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.menuRightClick.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(758, 624);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(87, 24);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ContextMenuStrip = this.menuRightClick;
			this.gridMain.Location = new System.Drawing.Point(12, 107);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(737, 541);
			this.gridMain.TabIndex = 2;
			this.gridMain.Title = "Planned Appointments";
			this.gridMain.TranslationName = "FormTrackNext";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.grid_MouseUp);
			// 
			// menuRightClick
			// 
			this.menuRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSelectPatient,
            this.toolStripMenuItemSeeChart,
            this.toolStripMenuItemSendToPinboard});
			this.menuRightClick.Name = "menuRightClick";
			this.menuRightClick.Size = new System.Drawing.Size(166, 70);
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
			// comboProv
			// 
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.Location = new System.Drawing.Point(300, 29);
			this.comboProv.MaxDropDownItems = 40;
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(181, 21);
			this.comboProv.TabIndex = 26;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(229, 33);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(70, 14);
			this.label4.TabIndex = 25;
			this.label4.Text = "Provider";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(758, 68);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(87, 24);
			this.butRefresh.TabIndex = 24;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// comboOrder
			// 
			this.comboOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboOrder.Location = new System.Drawing.Point(83, 29);
			this.comboOrder.MaxDropDownItems = 40;
			this.comboOrder.Name = "comboOrder";
			this.comboOrder.Size = new System.Drawing.Size(133, 21);
			this.comboOrder.TabIndex = 30;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 33);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 14);
			this.label1.TabIndex = 29;
			this.label1.Text = "Order by";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(758, 576);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(87, 24);
			this.butPrint.TabIndex = 31;
			this.butPrint.Text = "Print List";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// comboSite
			// 
			this.comboSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSite.Location = new System.Drawing.Point(300, 55);
			this.comboSite.MaxDropDownItems = 40;
			this.comboSite.Name = "comboSite";
			this.comboSite.Size = new System.Drawing.Size(181, 21);
			this.comboSite.TabIndex = 33;
			// 
			// labelSite
			// 
			this.labelSite.Location = new System.Drawing.Point(228, 57);
			this.labelSite.Name = "labelSite";
			this.labelSite.Size = new System.Drawing.Size(70, 14);
			this.labelSite.TabIndex = 32;
			this.labelSite.Text = "Site";
			this.labelSite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.IncludeAll = true;
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(536, 29);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(211, 21);
			this.comboClinic.TabIndex = 35;
			// 
			// dateRangePicker
			// 
			this.dateRangePicker.BackColor = System.Drawing.SystemColors.Control;
			this.dateRangePicker.EnableWeekButtons = false;
			this.dateRangePicker.Location = new System.Drawing.Point(20, 79);
			this.dateRangePicker.MaximumSize = new System.Drawing.Size(0, 185);
			this.dateRangePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.dateRangePicker.Name = "dateRangePicker";
			this.dateRangePicker.Size = new System.Drawing.Size(453, 22);
			this.dateRangePicker.TabIndex = 36;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(495, 57);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(77, 17);
			this.label6.TabIndex = 46;
			this.label6.Text = "Code Range";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// codeRangeFilter
			// 
			this.codeRangeFilter.Location = new System.Drawing.Point(573, 55);
			this.codeRangeFilter.Name = "codeRangeFilter";
			this.codeRangeFilter.Size = new System.Drawing.Size(133, 37);
			this.codeRangeFilter.TabIndex = 49;
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(855, 24);
			this.menuMain.TabIndex = 50;
			// 
			// FormTrackNext
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(855, 664);
			this.Controls.Add(this.codeRangeFilter);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.dateRangePicker);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.comboSite);
			this.Controls.Add(this.labelSite);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.comboOrder);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.menuMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormTrackNext";
			this.Text = "Track Planned Appointments";
			this.Load += new System.EventHandler(this.FormTrackNext_Load);
			this.Shown += new System.EventHandler(this.FormTrackNext_Shown);
			this.menuRightClick.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormTrackNext_Load(object sender, System.EventArgs e) {
			Cursor=Cursors.WaitCursor;
			comboOrder.Items.Add(Lan.g(this,"Status"));
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
			LayoutMenu();
			InitDateRange();
			RefreshAptList();
			Cursor=Cursors.Default;
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",setupToolStripMenuItem_Click));
			menuMain.EndUpdate();
		}

		///<summary>There is a bug in ODProgress.cs that forces windows that use a progress bar on load to go behind other applications. 
		///This is a temporary workaround until we decide how to address the issue.</summary>
		private void FormTrackNext_Shown(object sender,EventArgs e) {
			FillGrid();
		}

		private void InitDateRange() {
			int dayCount=PrefC.GetInt(PrefName.PlannedApptDaysPast);
			dateRangePicker.SetDateTimeFrom(DateTime.Today.AddDays(-dayCount));
			dayCount=PrefC.GetInt(PrefName.PlannedApptDaysFuture);
			dateRangePicker.SetDateTimeTo(DateTime.Today.AddDays(dayCount));
		}
		
		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormTrackNextSetup formTNS=new FormTrackNextSetup();
			if(formTNS.ShowDialog()==DialogResult.OK) {
				InitDateRange();
			}
		}

		private void menuRight_click(object sender,System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
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
			}
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Patient"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Date"),65);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Status"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Prov"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Procedures"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Notes"),200);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			Dictionary<long,string> dictPatNames=null;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				dictPatNames=Patients.GetLimForPats(_listPlannedAppts.Select(x => x.PatNum).ToList())
					.ToDictionary(x => x.PatNum,x => x.GetNameLF());
				};
			progressOD.ShowDialogProgress();
			foreach(Appointment apt in _listPlannedAppts){
				row=new GridRow();
				string patName=Lan.g(this,"UNKNOWN");
				dictPatNames.TryGetValue(apt.PatNum,out patName);
				row.Cells.Add(patName);
				if(apt.AptDateTime.Year<1880){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(apt.AptDateTime.ToShortDateString());
				}
				row.Cells.Add(Defs.GetName(DefCat.RecallUnschedStatus,apt.UnschedStatus));
				if(apt.IsHygiene) {
					Provider provHyg=Providers.GetFirstOrDefault(x => x.ProvNum==apt.ProvHyg);
					row.Cells.Add(provHyg==null?Lan.g(this,"INVALID"):provHyg.Abbr);
				}
				else {
					Provider prov=Providers.GetFirstOrDefault(x => x.ProvNum==apt.ProvNum);
					row.Cells.Add(prov==null?Lan.g(this,"INVALID"):prov.Abbr);
				}
				row.Cells.Add(apt.ProcDescript);
				row.Cells.Add(apt.Note);
			  
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void RefreshAptList() {
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
			long clinicNum=PrefC.HasClinicsEnabled ? comboClinic.SelectedClinicNum : -1;
			_listPlannedAppts=Appointments.RefreshPlannedTracker(order,provNum,siteNum,clinicNum,codeRangeFilter.StartRange,
				codeRangeFilter.EndRange,dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo());
		}

		private void grid_MouseUp(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right && gridMain.SelectedIndices.Length>0) {
				Patient pat=Patients.GetLim(_listPlannedAppts[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]].PatNum);
				toolStripMenuItemSelectPatient.Text=Lan.g(gridMain.TranslationName,"Select Patient")+" ("+pat.GetNameFL()+")";
			}
		}

		private void SelectPatient_Click() {
			//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			long patNum=_listPlannedAppts[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]].PatNum;
			Patient pat=Patients.GetPat(patNum);
			FormOpenDental.S_Contr_PatientSelected(pat,true);
		}

		private void SeeChart_Click() {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			//Only one can be selected at a time in this grid, but just in case we change it in the future it will select the last one in the list to be consistent with other patient selections.
			Patient pat=Patients.GetPat(_listPlannedAppts[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]].PatNum);
			FormOpenDental.S_Contr_PatientSelected(pat,false);
			GotoModule.GotoChart(pat.PatNum);
		}

		private void SendPinboard_Click() {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			List<long> listAppts=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listAppts.Add(_listPlannedAppts[gridMain.SelectedIndices[i]].AptNum);//Will only be one unless multiselect is enabled in the future
				_listPlannedAppts.RemoveAt(gridMain.SelectedIndices[i]);
			}
			GotoModule.PinToAppt(listAppts,0);//This will send all appointments in _listAptSelected to the pinboard, and will select the patient attached to the last appointment in _listAptSelected.
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int currentSelection=gridMain.GetSelectedIndex();
			int currentScroll=gridMain.ScrollValue;
			Patient pat=Patients.GetPat(_listPlannedAppts[e.Row].PatNum);//Only one can be selected at a time in this grid.
			FormOpenDental.S_Contr_PatientSelected(pat,true);
			using FormApptEdit FormAE=new FormApptEdit(_listPlannedAppts[e.Row].AptNum);
			FormAE.PinIsVisible=true;
			FormAE.ShowDialog();
			if(FormAE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormAE.PinClicked) {
				SendPinboard_Click();
				DialogResult=DialogResult.OK;
			}
			else {
				RefreshAptList();
				FillGrid();
				gridMain.SetSelected(currentSelection,true);
				gridMain.ScrollValue=currentScroll;
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshAptList();
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Planned appointment tracker list printed"));
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
				text=Lan.g(this,"Planned Appointment Tracker");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				//yPos+=(int)g.MeasureString(text,headingFont).Height;
				//text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				//g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=25;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,pagesPrinted,bounds,headingPrintH);
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