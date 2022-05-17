using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSupplyEdit:FormODBase {
		public Supply SupplyCur;
		public List<Supplier> ListSuppliers;
		//private bool isHiddenInitialVal;
		//private long categoryInitialVal;
		//private Supply SuppOriginal;
		//private List<Def> _listSupplyCatDefs;

		public FormSupplyEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSupplyEdit_Load(object sender,EventArgs e) {
			textSupplier.Text=Suppliers.GetName(ListSuppliers,SupplyCur.SupplierNum);
			comboCategory.Items.AddDefs(Defs.GetDefsForCategory(DefCat.SupplyCats,true));
			comboCategory.SetSelectedDefNum(SupplyCur.Category); 
			textCatalogNumber.Text=SupplyCur.CatalogNumber;
			textDescript.Text=SupplyCur.Descript;
			if(SupplyCur.LevelDesired!=0){
				textLevelDesired.Text=SupplyCur.LevelDesired.ToString();
			}
			if(SupplyCur.OrderQty!=0){
				textOrderQty.Text=SupplyCur.OrderQty.ToString();
			}
			if(SupplyCur.Price!=0){
				textPrice.Text=SupplyCur.Price.ToString("n");
			}
			if(SupplyCur.LevelOnHand!=0) {
				textOnHand.Text=SupplyCur.LevelOnHand.ToString();
			}
			checkIsHidden.Checked=SupplyCur.IsHidden;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(SupplyCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			try{
				Supplies.DeleteObject(SupplyCur);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textLevelDesired.IsValid()
				|| !textPrice.IsValid()
				|| !textOrderQty.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDescript.Text==""){
				MsgBox.Show(this,"Please enter a description.");
				return;
			}
			SupplyCur.Category=comboCategory.GetSelectedDefNum();
			SupplyCur.CatalogNumber=textCatalogNumber.Text;
			SupplyCur.Descript=textDescript.Text;
			SupplyCur.LevelDesired=PIn.Float(textLevelDesired.Text);
			SupplyCur.OrderQty=PIn.Int(textOrderQty.Text);
			SupplyCur.Price=PIn.Double(textPrice.Text);
			SupplyCur.LevelOnHand=PIn.Float(textOnHand.Text);
			//the logic below handles some of the basics.  This is supplemented in some cases by the automatic order fixing.
			if(!SupplyCur.IsHidden && checkIsHidden.Checked){//hiding
				SupplyCur.ItemOrder=0;//not perfect.  Hidden get intermingled with the first item.
			}
			if(SupplyCur.IsHidden && !checkIsHidden.Checked){//unhiding
				SupplyCur.ItemOrder=0;//keeps it at top of list
			}
			SupplyCur.IsHidden=checkIsHidden.Checked;
			if(SupplyCur.IsNew){
				SupplyCur.SupplyNum=Supplies.Insert(SupplyCur);
			}
			else{
				Supplies.Update(SupplyCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
	}
}