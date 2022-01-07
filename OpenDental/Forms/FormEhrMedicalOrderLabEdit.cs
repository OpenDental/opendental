using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEhrMedicalOrderLabEdit:FormODBase {
		public MedicalOrder MedOrderCur;
		public bool IsNew;
		private List<LabPanel> listPanels;
		private List<Provider> _listProviders;

		public FormEhrMedicalOrderLabEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormMedicalOrderLabEdit_Load(object sender,EventArgs e) {
			textDateTime.Text=MedOrderCur.DateTimeOrder.ToString();
			checkIsDiscontinued.Checked=MedOrderCur.IsDiscontinued;
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProv.Items.Add(_listProviders[i].GetLongDesc());
				if(MedOrderCur.ProvNum==_listProviders[i].ProvNum) {
					comboProv.SelectedIndex=i;
				}
			}
			//if a provider was subsequently hidden, the combobox may now be -1.
			textDescription.Text=MedOrderCur.Description;
			FillGrid();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("Date Time",135);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Service",200);
			gridMain.ListGridColumns.Add(col);
			listPanels=LabPanels.GetPanelsForOrder(MedOrderCur.MedicalOrderNum);//for a new lab order, this will naturally return no results
			List<LabResult> listResults;
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listPanels.Count;i++) {
				row=new GridRow();
				listResults=LabResults.GetForPanel(listPanels[i].LabPanelNum);
				if(listResults.Count==0) {
					row.Cells.Add(" ");//to avoid a very short row
				}
				else {
					row.Cells.Add(listResults[0].DateTimeTest.ToString());
				}
				row.Cells.Add(listPanels[i].ServiceName);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			LabPanel panel=listPanels[e.Row];
			using FormEhrLabPanelEdit formPE=new FormEhrLabPanelEdit();
			formPE.PanelCur=panel;
			formPE.ShowDialog();
			FillGrid();
		}

		private void butAttach_Click(object sender,EventArgs e) {
			if(IsNew){
				MessageBox.Show("Cannot attach lab panels to a brand new order.  Please save order first.");
				return;
			}
			using FormEhrLabPanels formL=new FormEhrLabPanels();
			Patient pat=Patients.GetPat(MedOrderCur.PatNum);
			formL.PatCur=pat;
			formL.IsSelectionMode=true;
			formL.ShowDialog();
			if(formL.DialogResult!=DialogResult.OK) {
				return;
			}
			LabPanel panel=LabPanels.GetOne(formL.SelectedLabPanelNum);
			panel.MedicalOrderNum=MedOrderCur.MedicalOrderNum;
			LabPanels.Update(panel);
			FillGrid();
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MessageBox.Show("Please select a lab panel first.");
				return;
			}
			LabPanel panel=listPanels[gridMain.GetSelectedIndex()];
			panel.MedicalOrderNum=0;
			LabPanels.Update(panel);
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(MessageBox.Show("Delete?","Delete?",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
				return;
			}
			try {
				MedicalOrders.Delete(MedOrderCur.MedicalOrderNum);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text=="") {
				MessageBox.Show(this,"Please enter a description.");
				return;
			} 
			try {
				MedOrderCur.DateTimeOrder=PIn.DateT(textDateTime.Text);
			}
			catch {
				MessageBox.Show(this,"Please enter a Date Time with format DD/MM/YYYY HH:mm AM/PM");
			}
			MedOrderCur.Description=textDescription.Text;
			MedOrderCur.IsDiscontinued=checkIsDiscontinued.Checked;
			if(comboProv.SelectedIndex==-1) {
				//don't make any changes to provnum.  0 is ok, but should never happen.  ProvNum might also be for a hidden prov.
			}
			else {
				MedOrderCur.ProvNum=_listProviders[comboProv.SelectedIndex].ProvNum;
			}
			if(IsNew) {
				MedicalOrders.Insert(MedOrderCur);
				EhrMeasureEvent newMeasureEvent=new EhrMeasureEvent();
				newMeasureEvent.DateTEvent=DateTime.Now;
				newMeasureEvent.EventType=EhrMeasureEventType.CPOE_LabOrdered;
				newMeasureEvent.PatNum=MedOrderCur.PatNum;
				newMeasureEvent.MoreInfo="";
				newMeasureEvent.FKey=MedOrderCur.MedicalOrderNum;
				EhrMeasureEvents.Insert(newMeasureEvent);
			}
			else {
				MedicalOrders.Update(MedOrderCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		




	


	}
}
