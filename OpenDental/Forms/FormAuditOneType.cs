using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>This form shows all of the security log entries for one fKey item. So far this only applies to a single appointment or a single procedure code.</summary>
	public partial class FormAuditOneType : FormODBase {
		private long PatNum;
		private List <Permissions> PermTypes;
		private long FKey;

		///<summary>LogList can be filled before loading the window with a custom log list or it will get automatically filled upon load if left emtpy.  Used for showing mixtures of generic audit entries and FK entries.  Viewing specific ortho chart visit audits need to always have patient field changes.</summary>
		public SecurityLog[] LogList;

		///<summary>Supply the patient, types, and title.</summary>
		public FormAuditOneType(long patNum,List<Permissions> permTypes,string title,long fKey) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			Text=title;
			PatNum=patNum;
			PermTypes=new List<Permissions>(permTypes);
			FKey=fKey;
		}

		private void FormAuditOneType_Load(object sender, System.EventArgs e) {
			//Default is "Changes made to this appointment before the update to 12.3 will not be reflected below, but can be found in the regular audit trail."
			if(PermTypes.Contains(Permissions.ProcFeeEdit)) {
				labelDisclaimer.Text=Lan.g(this,"Changes made to this procedure fee before the update to 13.2 were not tracked in the audit trail.");
			} 
			else if(PermTypes.Contains(Permissions.InsPlanChangeCarrierName)) {
				labelDisclaimer.Text=Lan.g(this,"Changes made to the carrier for this ins plan before the update to 13.2 were not tracked in the audit trail.");
			}
			else if(PermTypes.Contains(Permissions.RxEdit)) {
				labelDisclaimer.Text=Lan.g(this,"Changes made to the carrier for this Rx before the update to 14.2 were not tracked in the audit trail.");
			}
			else if(PermTypes.Contains(Permissions.OrthoChartEditFull)) {
				labelDisclaimer.Text=Lan.g(this,"Changes made to the ortho chart for this date before the update to 14.3 were not tracked in the audit trail.");
			}
			else if(PermTypes.Contains(Permissions.ImageEdit) 
				|| PermTypes.Contains(Permissions.ImageDelete)
				|| PermTypes.Contains(Permissions.ImageCreate) 
				|| PermTypes.Contains(Permissions.ImageExport))
			{
				labelDisclaimer.Text=Lan.g(this,"Edits and deletes before 15.1 and creates and exports before 21.1 for this document will not be reflected below.");
			}
			else if(PermTypes.Contains(Permissions.EhrMeasureEventEdit)) {
				labelDisclaimer.Text=Lan.g(this,"Changes made to this measure event before the update to 15.2 will not be reflected below.");
			}
			FillGrid();
		}

		private void FillGrid() {
			try {
				LogList=SecurityLogs.Refresh(PatNum,PermTypes,FKey);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"There was a problem refreshing the Audit Trail with the current filters."),ex);
				LogList=new SecurityLog[0];
			}
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableAudit","Date Time"),120);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableAudit","User"),70);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableAudit","Permission"),170);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableAudit","Log Text"),510);
			grid.ListGridColumns.Add(col);
			grid.ListGridRows.Clear();
			GridRow row;
			Userod user;
			foreach(SecurityLog logCur in LogList) {
				row=new GridRow();
				row.Cells.Add(logCur.LogDateTime.ToShortDateString()+" "+logCur.LogDateTime.ToShortTimeString());
				user=Userods.GetUser(logCur.UserNum);
				if(user==null) {//Will be null for audit trails made by outside entities that do not require users to be logged in.  E.g. Web Sched.
					row.Cells.Add("unknown");
				}
				else {
					row.Cells.Add(user.UserName);
				}
				row.Cells.Add(logCur.PermType.ToString());
				row.Cells.Add(logCur.LogText);
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			grid.ScrollToEnd();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			this.Close();
		}
	}
}





















