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
	public partial class FrmProviderIdentEdit : FrmODBase {
		///<summary>Set this field externally before using this window.</summary>
		public ProviderIdent ProviderIdentCur;
		///<summary></summary>
		public bool IsNew;

		///<summary></summary>
		public FrmProviderIdentEdit()
		{
			InitializeComponent();
			Load+=FrmProviderIdentEdit_Load;
			PreviewKeyDown+=FrmProviderIdentEdit_PreviewKeyDown;
		}

		private void FrmProviderIdentEdit_Load(object sender,EventArgs e) {
			Lang.F(this);
			textPayorID.Text=ProviderIdentCur.PayorID;
			listType.Items.AddEnums<ProviderSupplementalID>();
			listType.SetSelectedEnum(ProviderIdentCur.SuppIDType);
			textIDNumber.Text=ProviderIdentCur.IDNumber;
		}

		private void FrmProviderIdentEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
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