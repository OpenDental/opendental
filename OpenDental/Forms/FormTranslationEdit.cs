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
		private bool IsNew;
		//private bool IsNewCulTran;
		private Language LanCur;
		private LanguageForeign LanForeign;
		private string OtherTrans;

		///<summary>lanForeign might be null.</summary>
		public FormTranslationEdit(Language lanCur,LanguageForeign lanForeign,string otherTrans){
			InitializeComponent();
			InitializeLayoutManager();
			//no need to translate anything here
			LanCur=lanCur;
			LanForeign=lanForeign;
			OtherTrans=otherTrans;
		}

		private void FormTranslationEdit_Load(object sender, System.EventArgs e){
			textEnglish.Text=LanCur.English;
			textOtherTranslation.Text=OtherTrans;
			if(LanForeign==null){
				LanForeign=new LanguageForeign();
				LanForeign.ClassType=LanCur.ClassType;
				LanForeign.English=LanCur.English;
				LanForeign.Culture=CultureInfo.CurrentCulture.Name;
				Text="Add Translation";
				IsNew=true;
			}
			else{
				//LanguageForeigns.Cur=((LanguageForeign)LanguageForeigns.HList[Lan.Cur.ClassType+Lan.Cur.English]);
				textTranslation.Text=LanForeign.Translation;
				textComments.Text=LanForeign.Comments;
				Text="Edit Translation";
				IsNew=false;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e){
			if(textTranslation.Text=="" && textComments.Text==""){
				//If only the translation is "", then the Lan.g routine will simply ignore it and use English.
				if(!IsNew){
					if(MessageBox.Show("This translation is blank and will be deleted.  Continue?",""
						,MessageBoxButtons.OKCancel)!=DialogResult.OK)
					{
						return;
					}
					LanguageForeigns.Delete(LanForeign);
				}
				DialogResult=DialogResult.OK;
				return;
			}
			LanForeign.Translation=textTranslation.Text;
			LanForeign.Comments=textComments.Text;
			if(IsNew){
				LanguageForeigns.Insert(LanForeign);
			}
			else{
				LanguageForeigns.Update(LanForeign);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
