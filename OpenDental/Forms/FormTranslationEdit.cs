using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Globalization;
using System.IO;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormTranslationEdit : FormODBase {
		private bool _isNew;
		//private bool IsNewCulTran;
		private Language _language;
		private LanguageForeign _languageForeign;
		private string _otherTrans;

		///<summary>lanForeign might be null.</summary>
		public FormTranslationEdit(Language language,LanguageForeign languageForeign,string otherTrans){
			InitializeComponent();
			InitializeLayoutManager();
			//no need to translate anything here
			_language=language;
			_languageForeign=languageForeign;
			_otherTrans=otherTrans;
		}

		private void FormTranslationEdit_Load(object sender, System.EventArgs e){
			textEnglish.Text=_language.English;
			textOtherTranslation.Text=_otherTrans;
			if(_languageForeign==null){
				_languageForeign=new LanguageForeign();
				_languageForeign.ClassType=_language.ClassType;
				_languageForeign.English=_language.English;
				_languageForeign.Culture=CultureInfo.CurrentCulture.Name;
				Text="Add Translation";
				_isNew=true;
				return;
			}
			//LanguageForeigns.Cur=((LanguageForeign)LanguageForeigns.HList[Lan.Cur.ClassType+Lan.Cur.English]);
			textTranslation.Text=_languageForeign.Translation;
			textComments.Text=_languageForeign.Comments;
			Text="Edit Translation";
			_isNew=false;
		}

		private void butOK_Click(object sender, System.EventArgs e){
			if(textTranslation.Text=="" && textComments.Text==""){
				//If only the translation is "", then the Lan.g routine will simply ignore it and use English.
				if(!_isNew){
					if(MessageBox.Show("This translation is blank and will be deleted.  Continue?",""
						,MessageBoxButtons.OKCancel)!=DialogResult.OK)
					{
						return;
					}
					LanguageForeigns.Delete(_languageForeign);
				}
				DialogResult=DialogResult.OK;
				return;
			}
			_languageForeign.Translation=textTranslation.Text;
			_languageForeign.Comments=textComments.Text;
			if(_isNew){
				LanguageForeigns.Insert(_languageForeign);
			}
			else{
				LanguageForeigns.Update(_languageForeign);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}