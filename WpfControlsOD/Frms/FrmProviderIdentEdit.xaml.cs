using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FrmProviderIdentEdit : FrmODBase {
		///<summary>Set this field externally before using this window.</summary>
		public ProviderIdent ProviderIdentCur;
		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FrmProviderIdentEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//Lan.F(this);
			Load+=FrmProviderIdentEdit_Load;
		}

		private void FrmProviderIdentEdit_Load(object sender,EventArgs e) {
			textPayorID.Text=ProviderIdentCur.PayorID;
			listType.Items.AddEnums<ProviderSupplementalID>();
			listType.SetSelectedEnum(ProviderIdentCur.SuppIDType);
			textIDNumber.Text=ProviderIdentCur.IDNumber;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			ProviderIdentCur.PayorID=textPayorID.Text;
			ProviderIdentCur.SuppIDType=listType.GetSelected<ProviderSupplementalID>();
			ProviderIdentCur.IDNumber=textIDNumber.Text;
			if(IsNew){
				ProviderIdents.Insert(ProviderIdentCur);
			}
			else{
				ProviderIdents.Update(ProviderIdentCur);
			}
			IsDialogOK=true;
		}

		


	}
}





















