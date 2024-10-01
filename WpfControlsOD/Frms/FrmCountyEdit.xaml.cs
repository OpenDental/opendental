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
	public partial class FrmCountyEdit : FrmODBase {
		///<summary></summary>
		public bool IsNew;
		public County CountyCur;

		///<summary></summary>
		public FrmCountyEdit()
		{
			InitializeComponent();
			Load+=FrmCountyEdit_Load;
			textCountyName.TextChanged+=textCountyName_TextChanged;
			PreviewKeyDown+=FrmCountyEdit_PreviewKeyDown;
		}

		private void FrmCountyEdit_Load(object sender,EventArgs e) {
			Lang.F(this);
			textCountyName.Text=CountyCur.CountyName;
			textCountyCode.Text=CountyCur.CountyCode;
		}

		private void textCountyName_TextChanged(object sender, System.EventArgs e) {
			if(textCountyName.Text.Length==1){
				textCountyName.Text=textCountyName.Text.ToUpper();
				textCountyName.SelectionStart=1;
			}
		}

		private void FrmCountyEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			CountyCur.CountyName=textCountyName.Text;
			CountyCur.CountyCode=textCountyCode.Text;
			if(IsNew){
				if(Counties.DoesExist(CountyCur.CountyName)){
					MsgBox.Show(this,"County name already exists. Duplicate not allowed.");
					return;
				}
				Counties.Insert(CountyCur);
			}
			else{//existing County
				if(CountyCur.CountyName!=CountyCur.CountyNameOld){//County name was changed
					if(Counties.DoesExist(CountyCur.CountyName)){//changed to a name that already exists.
						MsgBox.Show(this,"County name already exists. Duplicate not allowed.");
						return;
					}
				}
				Counties.Update(CountyCur);
			}
			IsDialogOK=true;
		}

	}
}