using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormProviderIdentEdit : FormODBase {
		///<summary>Set this field externally before using this window.</summary>
		public ProviderIdent ProvIdentCur;
		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FormProviderIdentEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProviderIdentEdit_Load(object sender, System.EventArgs e) {
			textPayorID.Text=ProvIdentCur.PayorID;
			listType.Items.AddEnums<ProviderSupplementalID>();
			listType.SetSelectedEnum(ProvIdentCur.SuppIDType);
			textIDNumber.Text=ProvIdentCur.IDNumber;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			ProvIdentCur.PayorID=textPayorID.Text;
			ProvIdentCur.SuppIDType=listType.GetSelected<ProviderSupplementalID>();
			ProvIdentCur.IDNumber=textIDNumber.Text;
			if(IsNew){
				ProviderIdents.Insert(ProvIdentCur);
			}
			else{
				ProviderIdents.Update(ProvIdentCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















