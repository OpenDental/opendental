using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormApptRules : FormODBase {
		private bool _changed;
		private List<AppointmentRule> _listAppointmentRules;

		///<summary></summary>
		public FormApptRules()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormApptRules_Load(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			AppointmentRules.RefreshCache();
			_listAppointmentRules=AppointmentRules.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Description",200);
			gridMain.Columns.Add(col);
			col=new GridColumn("Start Code",100);
			gridMain.Columns.Add(col);
			col=new GridColumn("End Code",100);
			gridMain.Columns.Add(col);
			col=new GridColumn("Enabled",50,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			UI.GridRow row;
			for(int i=0;i<_listAppointmentRules.Count;i++){
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(_listAppointmentRules[i].RuleDesc);
				row.Cells.Add(_listAppointmentRules[i].CodeStart);
				row.Cells.Add(_listAppointmentRules[i].CodeEnd);
				if(_listAppointmentRules[i].IsEnabled){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			FrmApptRuleEdit frmApptRuleEdit=new FrmApptRuleEdit(_listAppointmentRules[e.Row]);
			frmApptRuleEdit.ShowDialog();
			FillGrid();
			_changed=true;
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			AppointmentRule appointmentRule=new AppointmentRule();
			appointmentRule.IsEnabled=true;
			FrmApptRuleEdit frmApptRuleEdit=new FrmApptRuleEdit(appointmentRule);
			frmApptRuleEdit.IsNew=true;
			frmApptRuleEdit.ShowDialog();
			if(!frmApptRuleEdit.IsDialogOK){
				return;
			}
			FillGrid();
			_changed=true;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormPayPeriods_FormClosing(object sender,FormClosingEventArgs e) {
			if(_changed) {
				DataValid.SetInvalid(InvalidType.Views);
			}
		}

		

		

		



		
	}
}





















