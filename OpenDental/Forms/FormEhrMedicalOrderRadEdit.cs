using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrMedicalOrderRadEdit:FormODBase {
		public MedicalOrder MedOrderCur;
		public bool IsNew;
		private List<Provider> _listProviders;

		public FormEhrMedicalOrderRadEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormMedicalOrderRadEdit_Load(object sender,EventArgs e) {
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
			catch (Exception ex){
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
				newMeasureEvent.EventType=EhrMeasureEventType.CPOE_RadOrdered;
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
