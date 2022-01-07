using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormSupplyOrderEdit:FormODBase {
		public SupplyOrder SupplyOrderCur;
		public List<Supplier> ListSuppliersAll;

		///<Summary>This form is only used to edit existing supplyOrders, not to add new ones.</Summary>
		public FormSupplyOrderEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSupplyOrderEdit_Load(object sender,EventArgs e) {
			textSupplier.Text=Suppliers.GetName(ListSuppliersAll,SupplyOrderCur.SupplierNum);
			if(SupplyOrderCur.DatePlaced.Year>2200){
				textDatePlaced.Text=DateTime.Today.ToShortDateString();
				SupplyOrderCur.UserNum=Security.CurUser.UserNum;
			}
			else{
				textDatePlaced.Text=SupplyOrderCur.DatePlaced.ToShortDateString();
			}
			textAmountTotal.Text=SupplyOrderCur.AmountTotal.ToString("n");
			textShippingCharge.Text=SupplyOrderCur.ShippingCharge.ToString("n");
			if(SupplyOrderCur.DateReceived.Year > 1880) {
				textDateReceived.Text=SupplyOrderCur.DateReceived.ToShortDateString();
			}
			textNote.Text=SupplyOrderCur.Note;
			comboUser.Items.AddNone<Userod>();
			comboUser.Items.AddList(Userods.GetUsers().FindAll(x => !x.IsHidden),x=>x.UserName);//the abbr parameter is usually skipped. <T> is inferred.
			comboUser.SetSelectedKey<Userod>(SupplyOrderCur.UserNum,x=>x.UserNum,x=>Userods.GetName(x)); 
		}
				
		private void butToday_Click(object sender,EventArgs e) {
			textDateReceived.Text=DateTime.Today.ToShortDateString();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete entire order?")){
				return;
			}
			SupplyOrders.DeleteObject(SupplyOrderCur);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textDatePlaced.IsValid()
				|| !textAmountTotal.IsValid()
				|| !textShippingCharge.IsValid()
				|| !textDateReceived.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDatePlaced.Text==""){
				SupplyOrderCur.DatePlaced=new DateTime(2500,1,1);
				SupplyOrderCur.UserNum=0;//even if they had set a user, set it back because the order hasn't been placed. 
			}
			else{
				SupplyOrderCur.DatePlaced=PIn.Date(textDatePlaced.Text);
				SupplyOrderCur.UserNum=comboUser.GetSelectedKey<Userod>(x=>x.UserNum);
			}
			SupplyOrderCur.AmountTotal=PIn.Double(textAmountTotal.Text);
			SupplyOrderCur.Note=textNote.Text;
			SupplyOrderCur.ShippingCharge=PIn.Double(textShippingCharge.Text);
			SupplyOrderCur.DateReceived=PIn.Date(textDateReceived.Text);
			SupplyOrders.Update(SupplyOrderCur);//never new
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}