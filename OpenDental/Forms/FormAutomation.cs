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
		private bool changed;
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
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Description",200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Trigger",150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Action",150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Details",200);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			UI.GridRow row;
			string detail;
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
				detail="";
				if(_listAutomations[i].AutoAction==AutomationAction.CreateCommlog) {
					detail+=Defs.GetName(DefCat.CommLogTypes,_listAutomations[i].CommType)
						+".  "+_listAutomations[i].MessageContent;
				}
				else if(_listAutomations[i].AutoAction==AutomationAction.PrintPatientLetter) {
					detail+=SheetDefs.GetDescription(_listAutomations[i].SheetDefNum);
				}
				else if(_listAutomations[i].AutoAction==AutomationAction.PrintReferralLetter) {
					detail+=SheetDefs.GetDescription(_listAutomations[i].SheetDefNum);
				}
				else if(_listAutomations[i].AutoAction==AutomationAction.ChangePatStatus) {
					detail+=Lans.g("enum"+nameof(PatientStatus),_listAutomations[i].PatStatus.GetDescription());
				}
				row.Cells.Add(detail);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			using FormAutomationEdit FormA=new FormAutomationEdit(_listAutomations[e.Row]);
			FormA.ShowDialog();
			FillGrid();
			changed=true;
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			Automation auto=new Automation();
			Automations.Insert(auto);//so that we can attach conditions
			using FormAutomationEdit FormA=new FormAutomationEdit(auto);
			FormA.IsNew=true;
			FormA.ShowDialog();
			if(FormA.DialogResult!=DialogResult.OK){
				return;
			}
			FillGrid();
			changed=true;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormAutomation_FormClosing(object sender,FormClosingEventArgs e) {
			if(changed) {
				DataValid.SetInvalid(InvalidType.Automation);
			}
		}

		

		

		



		
	}
}





















