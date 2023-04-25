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
		public ProviderIdent ProviderIdentCur;
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
			textPayorID.Text=ProviderIdentCur.PayorID;
			listType.Items.AddEnums<ProviderSupplementalID>();
			listType.SetSelectedEnum(ProviderIdentCur.SuppIDType);
			textIDNumber.Text=ProviderIdentCur.IDNumber;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			ProviderIdentCur.PayorID=textPayorID.Text;
			ProviderIdentCur.SuppIDType=listType.GetSelected<ProviderSupplementalID>();
			ProviderIdentCur.IDNumber=textIDNumber.Text;
			if(IsNew){
				ProviderIdents.Insert(ProviderIdentCur);
			}
			else{
				ProviderIdents.Update(ProviderIdentCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















