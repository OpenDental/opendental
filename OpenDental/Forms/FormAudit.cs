using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Data;
using CodeBase;
using System.Linq;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormAudit : FormODBase {
		///<summary>The selected patNum.  Can be 0 to include all.</summary>
		private long _patNum;
		///<summary>This gets set externally beforehand.  Lets user quickly select audit trail for current patient.</summary>
		public long CurPatNum;
		///<summary>This alphabetizes the permissions, except for "none", which is always first.  If using a foreign language, the order will be according to the English version, not the foreign translated text.</summary>
		private List<string> _listPermissionsAlphabetic;
		private bool _hasHeadingPrinted;
		private int _pagesPrinted;
		private int _headingPrintH;
		private List<Userod> _listUserods;

		///<summary></summary>
		public FormAudit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			//Permissions
			Permissions[] permissionsArray=(Permissions[])Enum.GetValues(typeof(Permissions));
			_listPermissionsAlphabetic=new List<string>();
			for(int i=1;i<permissionsArray.Length;i++){
				if(GroupPermissions.HasAuditTrail(permissionsArray[i])) {
					_listPermissionsAlphabetic.Add(permissionsArray[i].ToString());
				}
			}
			_listPermissionsAlphabetic.Sort();
			_listPermissionsAlphabetic.Insert(0,Permissions.None.ToString());
			//LogSources
			comboLogSource.IncludeAll=true;
			List<LogSources> listLogSourcesAlphabetic=Enum.GetValues(typeof(LogSources)).Cast<LogSources>()
				.OrderByDescending(x => x==LogSources.None)//All and None should be the first items suggested to the user.
				.ThenBy(x => x.GetDescription())
				.ToList();
			comboLogSource.Items.AddListEnum(listLogSourcesAlphabetic);
		}

		private void FormAudit_Load(object sender, System.EventArgs e) {
			textDateFrom.Text=DateTime.Today.AddDays(-10).ToShortDateString();
			textDateTo.Text=DateTime.Today.ToShortDateString();
			//Permission
			for(int i=0;i<_listPermissionsAlphabetic.Count;i++){
				if(i==0){
					comboPermission.Items.Add(Lan.g(this,"All"));//None
				}
				else{
					comboPermission.Items.Add(Lan.g("enumPermissions",_listPermissionsAlphabetic[i]));
				}
			}
			comboPermission.SelectedIndex=0;
			//LogSource
			comboLogSource.IsAllSelected=true;
			//User
			_listUserods=Userods.GetDeepCopy();
			comboUser.Items.Add(Lan.g(this,"All"));
			comboUser.Items.Add(Lan.g(this,"None"));
			comboUser.SelectedIndex=0; //To start with "All" selected.
			for(int i=0;i<_listUserods.Count;i++){
				comboUser.Items.Add(_listUserods[i].UserName);
			}
			_patNum=CurPatNum;
			if(_patNum==0) {
				textPatient.Text="";
			}
			else {
				textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
			}
			textRows.Text=PrefC.GetString(PrefName.AuditTrailEntriesDisplayed);
			FillGrid();
		}

		private void comboUser_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboPermission_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboLogSource_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butCurrent_Click(object sender,EventArgs e) {
			_patNum=CurPatNum;
			if(_patNum==0){
				textPatient.Text="";
			}
			else{
				textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
			}
			FillGrid();
		}

		private void butFind_Click(object sender,EventArgs e) {
			using FormPatientSelect formPatientSelect=new FormPatientSelect();
			formPatientSelect.ShowDialog();
			if(formPatientSelect.DialogResult!=DialogResult.OK){
				return;
			}
			_patNum=formPatientSelect.PatNumSelected;
			textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
			FillGrid();
		}

		private void butAll_Click(object sender,EventArgs e) {
			_patNum=0;
			textPatient.Text="";
			FillGrid();
		}

		private void FillGrid() {
			if(!textRows.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			//User filter
			long userNum=0; //In securitylog, UserNum of 0 is "None". This will be changed below to a valid userNum or -1 if "None" is not selected.
			if(comboUser.SelectedIndex==0) { //"All" is currently manually added the combobox, so we can't use IsAllSelected.
				userNum=-1; //We don't want to filter by userNum at all, so set it to -1.
			}else if(comboUser.SelectedIndex>1) {
				userNum=_listUserods[comboUser.SelectedIndex-2].UserNum; //Subtract 2 to accomodate for "None" and "All", since they dont exist in _listUserods.
			}
			SecurityLog[] securityLogArray=null;
			DateTime datePreviousFrom=PIn.Date(textDateEditedFrom.Text);
			DateTime datePreviousTo=DateTime.Today;
			if(textDateEditedTo.Text!="") { 
				datePreviousTo=PIn.Date(textDateEditedTo.Text);
			}
			//LogSource filter
			int logSource=-1;
			if(!comboLogSource.IsAllSelected) {
				logSource=(int)comboLogSource.GetSelected<LogSources>();
			}
			try {
				//Permission filter
				if(comboPermission.SelectedIndex==0) {
					securityLogArray=ReportsComplex.RunFuncOnReportServer(() => SecurityLogs.Refresh(PIn.Date(textDateFrom.Text),PIn.Date(textDateTo.Text),
						Permissions.None,_patNum,datePreviousFrom,datePreviousTo,PIn.Int(textRows.Text),userNum,logSource), Prefs.GetBoolNoCache(PrefName.AuditTrailUseReportingServer));
				}
				else {
					securityLogArray=ReportsComplex.RunFuncOnReportServer(() => SecurityLogs.Refresh(PIn.Date(textDateFrom.Text),PIn.Date(textDateTo.Text),
						(Permissions)Enum.Parse(typeof(Permissions),comboPermission.SelectedItem.ToString()),_patNum,
						datePreviousFrom,datePreviousTo,PIn.Int(textRows.Text),userNum,logSource), Prefs.GetBoolNoCache(PrefName.AuditTrailUseReportingServer));
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"There was a problem refreshing the Audit Trail with the current filters."),ex);
				securityLogArray=new SecurityLog[0];
			}
			grid.BeginUpdate();
			grid.Columns.Clear();
			grid.Columns.Add(new GridColumn(Lan.g("TableAudit","Date"),70,GridSortingStrategy.DateParse));
			grid.Columns.Add(new GridColumn(Lan.g("TableAudit","Time"),60,GridSortingStrategy.DateParse));
			grid.Columns.Add(new GridColumn(Lan.g("TableAudit","Patient"),100));
			grid.Columns.Add(new GridColumn(Lan.g("TableAudit","User"),70));
			grid.Columns.Add(new GridColumn(Lan.g("TableAudit","Permission"),190));
			grid.Columns.Add(new GridColumn(Lan.g("TableAudit","Computer"),70));
			grid.Columns.Add(new GridColumn(Lan.g("TableAudit","Log Text"),279));
			grid.Columns.Add(new GridColumn(Lan.g("TableAudit","Log Source"),140));
			grid.Columns.Add(new GridColumn(Lan.g("TableAudit","Last Edit"),100));
			grid.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<securityLogArray.Length;i++) {
				row=new GridRow();
				row.Cells.Add(securityLogArray[i].LogDateTime.ToShortDateString());
				row.Cells.Add(securityLogArray[i].LogDateTime.ToShortTimeString());
				row.Cells.Add(securityLogArray[i].PatientName);
				//user might be null due to old bugs.
				row.Cells.Add(Userods.GetUser(securityLogArray[i].UserNum)?.UserName??(Lan.g(this,"Unknown")+"("+POut.Long(securityLogArray[i].UserNum)+")"));
				if(securityLogArray[i].PermType==Permissions.ChartModule) {
					row.Cells.Add("ChartModuleViewed");
				}
				else if(securityLogArray[i].PermType==Permissions.FamilyModule) {
					row.Cells.Add("FamilyModuleViewed");
				}
				else if(securityLogArray[i].PermType==Permissions.AccountModule) {
					row.Cells.Add("AccountModuleViewed");
				}
				else if(securityLogArray[i].PermType==Permissions.ImagingModule) {
					row.Cells.Add("ImagesModuleViewed");
				}
				else if(securityLogArray[i].PermType==Permissions.TPModule) {
					row.Cells.Add("TreatmentPlanModuleViewed");
				}
				else {
					row.Cells.Add(securityLogArray[i].PermType.ToString());
				}
				row.Cells.Add(securityLogArray[i].CompName);
				string logText=securityLogArray[i].LogText;
				if(securityLogArray[i].PermType!=Permissions.UserQuery) {
					row.Cells.Add(logText);
				}
				else {
					//Only display the first snipet of very long queries. User can double click to view.
					row.Cells.Add(StringTools.Truncate(logText,200,hasElipsis:true));
					row.Tag=(Action)(()=> {
						MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(logText);
						msgBoxCopyPaste.NormalizeContent();
						msgBoxCopyPaste.Show();
					});
				}
				string logSrc=securityLogArray[i].LogSource.ToString();
				if(logSrc=="None") {
					logSrc="";
				}
				row.Cells.Add(logSrc);
				if(securityLogArray[i].DateTPrevious.Year < 1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(securityLogArray[i].DateTPrevious.ToString());
				}
				//Get the hash for the audit log entry from the database and rehash to compare
				if(securityLogArray[i].LogHash!=SecurityLogHashes.GetHashString(securityLogArray[i])) {
					row.ColorText=Color.Red; //Bad hash or no hash entry at all.  This prevents users from deleting the entire hash table to make the audit trail look valid and encrypted.
					//historical entries will show as red.
				}
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			grid.ScrollToEnd();
		}

		private void grid_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			(grid.ListGridRows[e.Row].Tag as Action)?.Invoke();
		}

		private void butRefresh_Click(object sender, System.EventArgs e) {
			if( textDateFrom.Text=="" 
				|| textDateTo.Text==""
				|| !textDateFrom.IsValid()
				|| !textDateTo.IsValid()
				|| !textRows.IsValid()
				|| !textDateEditedFrom.IsValid()
				|| !textDateEditedTo.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_hasHeadingPrinted=false;
			PrinterL.TryPrintOrDebugClassicPreview(pd_PrintPage,Lan.g(this,"Audit trail printed"),printoutOrientation:PrintoutOrientation.Landscape);
		}

		///<summary>Raised for each page to be printed.</summary>
		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
				//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			using Font headingFont=new Font("Arial",13,FontStyle.Bold);
			using Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_hasHeadingPrinted) {
				text=Lan.g(this,"Audit Trail");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				_hasHeadingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=grid.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
				_pagesPrinted=0;
			}
		}

	}
}





















