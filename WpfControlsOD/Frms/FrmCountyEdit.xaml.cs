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
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//Lan.F(this);
		}

		private void FrmCountyEdit_Loaded(object sender,RoutedEventArgs e) {
			textCountyName.Text=CountyCur.CountyName;
			textCountyCode.Text=CountyCur.CountyCode;
		}

		private void textCountyName_TextChanged(object sender, System.EventArgs e) {
			if(textCountyName.Text.Length==1){
				textCountyName.Text=textCountyName.Text.ToUpper();
				textCountyName.SelectionStart=1;
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
				if(CountyCur.CountyName!=CountyCur.OldCountyName){//County name was changed
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





















