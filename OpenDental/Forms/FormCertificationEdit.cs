using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormCertificationEdit:FormODBase {
		public Cert CertCur;

		public FormCertificationEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCertificationEdit_Load(object sender, EventArgs e){
			//Description, Wiki page
			textDescription.Text=CertCur.Description;
			textWikiPage.Text=CertCur.WikiPageLink;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.CertificationCategories);
			listBoxCategories.Items.AddList(listDefs,x => x.ItemName);
			listBoxCategories.SetSelected(listDefs.FindIndex(x=> x.DefNum==CertCur.CertCategoryNum));
			checkIsHidden.Checked=CertCur.IsHidden;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(CertCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			//Check to see if Certification has employees assigned to it before we allow deletion
			List<CertEmployee> listCertEmployees=CertEmployees.GetAll();
			bool isCertInUse=false;
			for(int i=0;i<listCertEmployees.Count;i++) {
				if(listCertEmployees[i].CertNum==CertCur.CertNum) {
					isCertInUse=true;
					break;
					}
				}
			if(isCertInUse) {
				MsgBox.Show(this,"Certificiation is still in use, remove Certification from all applicable employees first.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you want to delete this certification?")) {
				return;
			}
			//ItemOrder will be automatically fixed on next fillgrid
			//Update the other Certs ItemOrder information based on the category that it belongs to
			/*
			List<Cert> listCerts=Certs.GetAll(true).Where(x=> x.CertCategoryNum==listBoxCategories.GetSelected<Def>().DefNum).ToList();
			if(CertCur.ItemOrder!=listCerts[listCerts.Count-1].ItemOrder) {
				for(int i=CertCur.ItemOrder;i<listCerts.Count;i++) {
					if(listCerts[i].ItemOrder>CertCur.ItemOrder) {
						listCerts[i].ItemOrder--;
						Certs.Update(listCerts[i]);
					}
				}
			}*/
			Certs.Delete(CertCur.CertNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Description cannot be blank.");
				return;
			}
			CertCur.Description=PIn.String(textDescription.Text);
			CertCur.WikiPageLink=PIn.String(textWikiPage.Text);
			CertCur.IsHidden=checkIsHidden.Checked;
			Def def=new Def();
			def=(Def)listBoxCategories.Items.GetObjectAt(listBoxCategories.SelectedIndex);
			long categoryNumOld=CertCur.CertCategoryNum;
			CertCur.CertCategoryNum=def.DefNum;
			//If user changes category, update the itemOrder based on the last cert's itemOrder in the new category
			if(CertCur.CertCategoryNum!=categoryNumOld) {//Only run if they switch
				CertCur.ItemOrder=Certs.GetAll(true).FindAll(x => x.CertCategoryNum==listBoxCategories.GetSelected<Def>().DefNum).Count();
				//This doesn't fix the hole in old category, but that will get fixed they next time they view that category.
			}
			if(CertCur.IsNew) {
				Certs.Insert(CertCur);
			}
			else {
				Certs.Update(CertCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}