using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrPatListElementEdit:FormODBase {
		public EhrPatListElement Element;
		public bool IsNew; 
		public bool Delete;

		public FormEhrPatListElementEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormPatListElementEdit_Load(object sender,EventArgs e) {
			listRestriction.Items.Clear();
			listRestriction.Items.AddEnums<EhrRestrictionType>();
			listRestriction.SelectedIndex=(int)Element.Restriction;
			listOperand.SelectedIndex=(int)Element.Operand;
			textCompareString.Text=Element.CompareString;
			textLabValue.Text=Element.LabValue;
			checkOrderBy.Checked=Element.OrderBy;
			ChangeLayout();
		}

		private void listRestriction_SelectedIndexChanged(object sender,EventArgs e) {
			ChangeLayout();
		}

		private void ChangeLayout() {
			labelCompareString.Visible=true;
			textCompareString.Visible=true;
			labelExample.Visible=true;
			labelLabValue.Visible=false;
			textLabValue.Visible=false;
			labelOperand.Visible=false;
			listOperand.Visible=false;
			if(listRestriction.SelectedIndex==0) {//Birthdate
				labelCompareString.Text="Enter age";
				labelExample.Text="Ex: 22";
				labelOperand.Visible=true;
				listOperand.Visible=true;
			}
			if(listRestriction.SelectedIndex==1) {//Disease
				labelCompareString.Text="Enter ICD9 code";
				labelExample.Text="Ex: 414.0";
			}
			if(listRestriction.SelectedIndex==2) {//Medication
				labelCompareString.Text="Medication name";
				labelExample.Text="Ex: Coumadin";
			}
			if(listRestriction.SelectedIndex==3) {//LabResult
				labelCompareString.Text="Test name (exact)";
				labelExample.Text="Ex: HDL-cholesterol";
				labelLabValue.Visible=true;
				textLabValue.Visible=true;
				labelOperand.Visible=true;
				listOperand.Visible=true;
			}
			if(listRestriction.SelectedIndex==4) {//Gender
				labelCompareString.Text="For display and sorting";
				labelExample.Visible=false;
				textCompareString.Visible=false;
			}
		}

		private bool IsValid() {
			int index=listRestriction.SelectedIndex;
			if(textCompareString.Text.Trim()=="" && index!=4) {//4-Gender
				MessageBox.Show(Lans.g(this,"Please enter a value."));
				return false;
			}
			if(index==0) {//Birthdate
					try {
						System.Convert.ToInt32(textCompareString.Text);//Must be number.
					}
					catch {
						MessageBox.Show("Please enter a valid age.");
						return false;
					}
			}
			if(index==1) {//Disease
				try {
					System.Convert.ToDecimal(textCompareString.Text);//Must be number.
				}
				catch {
					MessageBox.Show("Please enter a valid number ICD9 code.");
					return false;
				}
			}
			if(index==3) {//LabResult
				try {
					System.Convert.ToDecimal(textLabValue.Text);//Must be number.
				}
				catch {
					MessageBox.Show("Please enter a valid number for Lab value.");
					return false;
				}
			}
			if(index==4) {//Gender
				textCompareString.Text="";
			}
			if(index!=3) {//Not LabResult
				textLabValue.Text="";
			}
			return true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!IsNew) {
				Delete=true;
			}
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			Element.Restriction=(EhrRestrictionType)listRestriction.SelectedIndex;
			Element.Operand=(EhrOperand)listOperand.SelectedIndex;
			Element.CompareString=textCompareString.Text;
			Element.LabValue=textLabValue.Text;
			Element.OrderBy=checkOrderBy.Checked;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
