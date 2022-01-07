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
	public partial class FormCountyEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public County CountyCur;

		///<summary></summary>
		public FormCountyEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCountyEdit_Load(object sender, System.EventArgs e) {
			textCountyName.Text=CountyCur.CountyName;
			textCountyCode.Text=CountyCur.CountyCode;
		}

		private void textCountyName_TextChanged(object sender, System.EventArgs e) {
			if(textCountyName.Text.Length==1){
				textCountyName.Text=textCountyName.Text.ToUpper();
				textCountyName.SelectionStart=1;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			CountyCur.CountyName=textCountyName.Text;
			CountyCur.CountyCode=textCountyCode.Text;
			if(IsNew){
				if(Counties.DoesExist(CountyCur.CountyName)){
					MessageBox.Show(Lan.g(this,"County name already exists. Duplicate not allowed."));
					return;
				}
				Counties.Insert(CountyCur);
			}
			else{//existing County
				if(CountyCur.CountyName!=CountyCur.OldCountyName){//County name was changed
					if(Counties.DoesExist(CountyCur.CountyName)){//changed to a name that already exists.
						MessageBox.Show(Lan.g(this,"County name already exists. Duplicate not allowed."));
						return;
					}
				}
				Counties.Update(CountyCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


	}
}





















