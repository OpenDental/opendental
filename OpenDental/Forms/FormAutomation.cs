using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormAutomation:FormODBase {
		private bool _isChanged;
		private List<Automation> _listAutomations;

		///<summary></summary>
		public FormAutomation()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutomation_Load(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			Automations.RefreshCache();
			_listAutomations=Automations.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Description",200);
			gridMain.Columns.Add(col);
			col=new GridColumn("Trigger",150);
			gridMain.Columns.Add(col);
			col=new GridColumn("Action",150);
			gridMain.Columns.Add(col);
			col=new GridColumn("Details",200);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			UI.GridRow row;
			string strDetail;
			for(int i=0;i<_listAutomations.Count;i++){
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(_listAutomations[i].Description);
				if(_listAutomations[i].Autotrigger==AutomationTrigger.CompleteProcedure) {
					row.Cells.Add(_listAutomations[i].ProcCodes);
				}
				else {
					row.Cells.Add(_listAutomations[i].Autotrigger.ToString());
				}
				row.Cells.Add(_listAutomations[i].AutoAction.ToString());
				//details: 
				strDetail="";
				if(_listAutomations[i].AutoAction==AutomationAction.CreateCommlog) {
					strDetail+=Defs.GetName(DefCat.CommLogTypes,_listAutomations[i].CommType)
						+".  "+_listAutomations[i].MessageContent;
				}
				else if(_listAutomations[i].AutoAction==AutomationAction.PrintPatientLetter) {
					strDetail+=SheetDefs.GetDescription(_listAutomations[i].SheetDefNum);
				}
				else if(_listAutomations[i].AutoAction==AutomationAction.PrintReferralLetter) {
					strDetail+=SheetDefs.GetDescription(_listAutomations[i].SheetDefNum);
				}
				else if(_listAutomations[i].AutoAction==AutomationAction.ChangePatStatus) {
					strDetail+=Lans.g("enum"+nameof(PatientStatus),_listAutomations[i].PatStatus.GetDescription());
				}
				row.Cells.Add(strDetail);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			using FormAutomationEdit formAutomationEdit=new FormAutomationEdit(_listAutomations[e.Row]);
			formAutomationEdit.ShowDialog();
			FillGrid();
			_isChanged=true;
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			Automation automation=new Automation();
			Automations.Insert(automation);//so that we can attach conditions
			using FormAutomationEdit formAutomationEdit=new FormAutomationEdit(automation);
			formAutomationEdit.IsNew=true;
			formAutomationEdit.ShowDialog();
			if(formAutomationEdit.DialogResult!=DialogResult.OK){
				return;
			}
			FillGrid();
			_isChanged=true;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormAutomation_FormClosing(object sender,FormClosingEventArgs e) {
			if(_isChanged) {
				DataValid.SetInvalid(InvalidType.Automation);
			}
		}

		

		

		



		
	}
}





















