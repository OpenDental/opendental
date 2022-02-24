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

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormAudit : FormODBase {
		///<summary>The selected patNum.  Can be 0 to include all.</summary>
		private long PatNum;
		///<summary>This gets set externally beforehand.  Lets user quickly select audit trail for current patient.</summary>
		public long CurPatNum;
		///<summary>This alphabetizes the permissions, except for "none", which is always first.  If using a foreign language, the order will be according to the English version, not the foreign translated text.</summary>
		private List<string> permissionsAlphabetic;
		private bool headingPrinted;
		private int pagesPrinted;
		private int headingPrintH;
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
			Permissions[] permArray=(Permissions[])Enum.GetValues(typeof(Permissions));
			permissionsAlphabetic=new List<string>();
			for(int i=1;i<permArray.Length;i++){
				if(GroupPermissions.HasAuditTrail(permArray[i])) {
					permissionsAlphabetic.Add(permArray[i].ToString());
				}
			}
			permissionsAlphabetic.Sort();
			permissionsAlphabetic.Insert(0,Permissions.None.ToString());
		}

		private void FormAudit_Load(object sender, System.EventArgs e) {
			textDateFrom.Text=DateTime.Today.AddDays(-10).ToShortDateString();
			textDateTo.Text=DateTime.Today.ToShortDateString();
			for(int i=0;i<permissionsAlphabetic.Count;i++){
				if(i==0){
					comboPermission.Items.Add(Lan.g(this,"All"));//None
				}
				else{
					comboPermission.Items.Add(Lan.g("enumPermissions",permissionsAlphabetic[i]));
				}
			}
			comboPermission.SelectedIndex=0;
			_listUserods=Userods.GetDeepCopy();
			comboUser.Items.Add(Lan.g(this,"All"));
			comboUser.SelectedIndex=0;
			for(int i=0;i<_listUserods.Count;i++){
				comboUser.Items.Add(_listUserods[i].UserName);
			}
			PatNum=CurPatNum;
			if(PatNum==0) {
				textPatient.Text="";
			}
			else {
				textPatient.Text=Patients.GetLim(PatNum).GetNameLF();
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

		private void butCurrent_Click(object sender,EventArgs e) {
			PatNum=CurPatNum;
			if(PatNum==0){
				textPatient.Text="";
			}
			else{
				textPatient.Text=Patients.GetLim(PatNum).GetNameLF();
			}
			FillGrid();
		}

		private void butFind_Click(object sender,EventArgs e) {
			using FormPatientSelect FormP=new FormPatientSelect();
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK){
				return;
			}
			PatNum=FormP.SelectedPatNum;
			textPatient.Text=Patients.GetLim(PatNum).GetNameLF();
			FillGrid();
		}

		private void butAll_Click(object sender,EventArgs e) {
			PatNum=0;
			textPatient.Text="";
			FillGrid();
		}

		private void FillGrid() {
			if(!textRows.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			long userNum=0;
			if(comboUser.SelectedIndex>0) {
				userNum=_listUserods[comboUser.SelectedIndex-1].UserNum;
			}
			SecurityLog[] logList=null;
			DateTime datePreviousFrom=PIn.Date(textDateEditedFrom.Text);
			DateTime datePreviousTo=DateTime.Today;
			if(textDateEditedTo.Text!="") { 
				datePreviousTo=PIn.Date(textDateEditedTo.Text);
			}
			try {
				if(comboPermission.SelectedIndex==0) {
					logList=SecurityLogs.Refresh(PIn.Date(textDateFrom.Text),PIn.Date(textDateTo.Text),
						Permissions.None,PatNum,userNum,datePreviousFrom,datePreviousTo,PIn.Int(textRows.Text));
				}
				else {
					logList=SecurityLogs.Refresh(PIn.Date(textDateFrom.Text),PIn.Date(textDateTo.Text),
						(Permissions)Enum.Parse(typeof(Permissions),comboPermission.SelectedItem.ToString()),PatNum,userNum,
						datePreviousFrom,datePreviousTo,PIn.Int(textRows.Text));
				}
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"There was a problem refreshing the Audit Trail with the current filters."),ex);
				logList=new SecurityLog[0];
			}
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			grid.ListGridColumns.Add(new GridColumn(Lan.g("TableAudit","Date"),70,GridSortingStrategy.DateParse));
			grid.ListGridColumns.Add(new GridColumn(Lan.g("TableAudit","Time"),60,GridSortingStrategy.DateParse));
			grid.ListGridColumns.Add(new GridColumn(Lan.g("TableAudit","Patient"),100));
			grid.ListGridColumns.Add(new GridColumn(Lan.g("TableAudit","User"),70));
			grid.ListGridColumns.Add(new GridColumn(Lan.g("TableAudit","Permission"),190));
			grid.ListGridColumns.Add(new GridColumn(Lan.g("TableAudit","Computer"),70));
			grid.ListGridColumns.Add(new GridColumn(Lan.g("TableAudit","Log Text"),279));
			grid.ListGridColumns.Add(new GridColumn(Lan.g("TableAudit","Last Edit"),100));
			grid.ListGridRows.Clear();
			GridRow row;
			foreach(SecurityLog logCur in logList) {
				row=new GridRow();
				row.Cells.Add(logCur.LogDateTime.ToShortDateString());
				row.Cells.Add(logCur.LogDateTime.ToShortTimeString());
				row.Cells.Add(logCur.PatientName);
				//user might be null due to old bugs.
				row.Cells.Add(Userods.GetUser(logCur.UserNum)?.UserName??(Lan.g(this,"Unknown")+"("+POut.Long(logCur.UserNum)+")"));
				if(logCur.PermType==Permissions.ChartModule) {
					row.Cells.Add("ChartModuleViewed");
				}
				else if(logCur.PermType==Permissions.FamilyModule) {
					row.Cells.Add("FamilyModuleViewed");
				}
				else if(logCur.PermType==Permissions.AccountModule) {
					row.Cells.Add("AccountModuleViewed");
				}
				else if(logCur.PermType==Permissions.ImagingModule) {
					row.Cells.Add("ImagesModuleViewed");
				}
				else if(logCur.PermType==Permissions.TPModule) {
					row.Cells.Add("TreatmentPlanModuleViewed");
				}
				else {
					row.Cells.Add(logCur.PermType.ToString());
				}
				row.Cells.Add(logCur.CompName);
				if(logCur.PermType!=Permissions.UserQuery) {
					row.Cells.Add(logCur.LogText);
				}
				else {
					//Only display the first snipet of very long queries. User can double click to view.
					row.Cells.Add(StringTools.Truncate(logCur.LogText,200,true));
					row.Tag=(Action)(()=> {
						MsgBoxCopyPaste formText = new MsgBoxCopyPaste(logCur.LogText);
						formText.NormalizeContent();
						formText.Show();
					});
				}
				if(logCur.DateTPrevious.Year < 1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(logCur.DateTPrevious.ToString());
				}
				//Get the hash for the audit log entry from the database and rehash to compare
				if(logCur.LogHash!=SecurityLogHashes.GetHashString(logCur)) {
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
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugClassicPreview(pd_PrintPage,Lan.g(this,"Audit trail printed"),printoutOrientation:PrintoutOrientation.Landscape);
		}

		///<summary>Raised for each page to be printed.</summary>
		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
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
				text=Lan.g(this,"Audit Trail");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
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
				pagesPrinted=0;
			}
			g.Dispose();
		}
	}
}





















