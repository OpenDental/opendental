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
		public CultureInfo OldCulture;
		///<summary></summary>
		public string NewName;
		private CultureInfo[] ciList;

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
			textOldCode.Text=OldCulture.DisplayName;
			string suggestedName=string.Format("{0}-{0}",OldCulture.Name);
			ciList=CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			for(int i=0;i<ciList.Length;i++){
				string item=ciList[i].DisplayName;
				listCulture.Items.Add(item);
				if(ciList[i].Name.ToLowerInvariant()==suggestedName) {
					listCulture.SetSelected(i);
				}
			}
		}

		private void UpdateLanguageCode(object sender,System.EventArgs e) {
			if(listCulture.SelectedIndex==-1){
				MessageBox.Show("Please select a new culture first.");
				return;
			}
			NewName=ciList[listCulture.SelectedIndex].Name;
			DialogResult=DialogResult.OK;
		}
	}
}





















