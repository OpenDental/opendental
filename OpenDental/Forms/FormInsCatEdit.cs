using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Globalization;

namespace OpenDental {
	///<summary></summary>
	public partial class FormInsCatEdit:FormODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary>A list of CovSpans just for this category</summary>
		//private CovSpan[] CovSpanList;
		private CovCat _covCatCur;

		///<summary></summary>
		public FormInsCatEdit(CovCat covCatCur) {
			InitializeComponent();
			InitializeLayoutManager();
			_covCatCur=covCatCur.Copy();
			Lan.F(this);
		}

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormInsCatEditCanada";
			}
			return "FormInsCatEdit";
		}

		private void FormInsCatEdit_Load(object sender,System.EventArgs e) {
			textDescription.Text=_covCatCur.Description;
			if(_covCatCur.DefaultPercent==-1)
				textPercent.Text="";
			else
				textPercent.Text=_covCatCur.DefaultPercent.ToString();
			textPercent.MaxVal=100;
			checkHidden.Checked=_covCatCur.IsHidden;
			for(int i=0;i<Enum.GetNames(typeof(EbenefitCategory)).Length;i++){
				comboCat.Items.Add(Enum.GetNames(typeof(EbenefitCategory))[i]);
				if(Enum.GetNames(typeof(EbenefitCategory))[i]==_covCatCur.EbenefitCat.ToString()){
					comboCat.SelectedIndex=i;
				}
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textPercent.IsValid()
			//|| !textPriBasicPercent.IsValid()
			){
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			_covCatCur.Description=textDescription.Text;
			if(textPercent.Text=="") {
				_covCatCur.DefaultPercent=-1;
			}
			else {
				_covCatCur.DefaultPercent=PIn.Int(textPercent.Text);
			}
			_covCatCur.IsHidden=checkHidden.Checked;
			_covCatCur.EbenefitCat=(EbenefitCategory)comboCat.SelectedIndex;
			if(IsNew){
				CovCats.Insert(_covCatCur);
			}
			else{
				CovCats.Update(_covCatCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}
