using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormConvertLang39 : FormODBase {
		///<summary></summary>
		public CultureInfo CultureInfoOld;
		///<summary></summary>
		public string NewName;
		private CultureInfo[] _cultureInfoArray;

		///<summary></summary>
		public FormConvertLang39()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			//Lan.F(this);
		}

		private void FormConvertLang39_Load(object sender,System.EventArgs e) {
			textOldCode.Text=CultureInfoOld.DisplayName;
			string suggestedName=string.Format("{0}-{0}",CultureInfoOld.Name);
			_cultureInfoArray=CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			for(int i=0;i<_cultureInfoArray.Length;i++){
				string name=_cultureInfoArray[i].DisplayName;
				listCulture.Items.Add(name);
				if(_cultureInfoArray[i].Name.ToLowerInvariant()==suggestedName) {
					listCulture.SetSelected(i);
				}
			}
		}

		private void UpdateLanguageCode(object sender,System.EventArgs e) {
			if(listCulture.SelectedIndex==-1){
				MessageBox.Show("Please select a new culture first.");
				return;
			}
			NewName=_cultureInfoArray[listCulture.SelectedIndex].Name;
			DialogResult=DialogResult.OK;
		}
	}
}





















