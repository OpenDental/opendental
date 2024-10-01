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
		private CovCat _covCat;

		///<summary></summary>
		public FormInsCatEdit(CovCat covCat) {
			InitializeComponent();
			InitializeLayoutManager();
			_covCat=covCat.Copy();
			Lan.F(this);
		}

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormInsCatEditCanada";
			}
			return "FormInsCatEdit";
		}

		private void FormInsCatEdit_Load(object sender,System.EventArgs e) {
			textDescription.Text=_covCat.Description;
			if(_covCat.DefaultPercent==-1)
				textPercent.Text="";
			else
				textPercent.Text=_covCat.DefaultPercent.ToString();
			textPercent.MaxVal=100;
			checkHidden.Checked=_covCat.IsHidden;
			for(int i=0;i<Enum.GetNames(typeof(EbenefitCategory)).Length;i++){
				comboCat.Items.Add(Enum.GetNames(typeof(EbenefitCategory))[i]);
				if(Enum.GetNames(typeof(EbenefitCategory))[i]==_covCat.EbenefitCat.ToString()){
					comboCat.SelectedIndex=i;
				}
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(!textPercent.IsValid())
			//|| !textPriBasicPercent.IsValid()
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			_covCat.Description=textDescription.Text;
			if(textPercent.Text=="") {
				_covCat.DefaultPercent=-1;
			}
			else {
				_covCat.DefaultPercent=PIn.Int(textPercent.Text);
			}
			_covCat.IsHidden=checkHidden.Checked;
			_covCat.EbenefitCat=(EbenefitCategory)comboCat.SelectedIndex;
			if(IsNew){
				CovCats.Insert(_covCat);
			}
			else{
				CovCats.Update(_covCat);
			}
			DialogResult=DialogResult.OK;
		}

	}
}