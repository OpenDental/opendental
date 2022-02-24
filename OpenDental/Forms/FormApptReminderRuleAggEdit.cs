using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormApptReminderRuleAggEdit:FormODBase {
		public ApptReminderRule ApptReminderRuleCur;
		public List<ApptReminderRule> ListRulesNonDefault;
		///<summary>Langauge of the tab that was selected in the parent form. Used for picking the tab index of this form. </summary>
		private string _selectedLanguageLoading;

		public FormApptReminderRuleAggEdit(ApptReminderRule apptReminderCur,List<ApptReminderRule> listRulesNonDefault,string selectedLanguage) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			//This needs to remain a shallow copy because FormEServicesECR is expecting shallow copy changes only. Making a new instance would break that.
			ApptReminderRuleCur=apptReminderCur;
			ListRulesNonDefault=listRulesNonDefault;
			_selectedLanguageLoading=selectedLanguage;
		}

		private void FormApptReminderRuleEdit_Load(object sender,EventArgs e) {
			UserControlReminderAgg userControlReminderAgg=new UserControlReminderAgg(ApptReminderRuleCur);
			if(ListRulesNonDefault.Count==0) {
				tabControl1.Visible=false;
				userControlReminderAgg.Bounds=tabControl1.Bounds;
				userControlReminderAgg.Anchor=System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left 
					| System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
				LayoutManager.AddUnscaled(userControlReminderAgg,PanelClient);
			}
			else {
				userControlReminderAgg.Dock=DockStyle.Fill;
				LayoutManager.AddUnscaled(userControlReminderAgg,tabPageDefault);
			}
			foreach(ApptReminderRule languageRule in ListRulesNonDefault) {
				TabPage tabPageLanguage=new TabPage();
				CultureInfo culture=MiscUtils.GetCultureFromThreeLetter(languageRule.Language);
				if(culture==null) {
					tabPageLanguage.Text=languageRule.Language;
				}
				else {
					tabPageLanguage.Text=culture.DisplayName;
				}
				LayoutManager.Add(tabPageLanguage,tabControl1);
				UserControlReminderAgg userControlReminderAggLang=new UserControlReminderAgg(languageRule);
				//languageAggControl.Anchor=defaultAggControl.Anchor;
				userControlReminderAggLang.Dock=DockStyle.Fill;
				LayoutManager.AddUnscaled(userControlReminderAggLang,tabPageLanguage);
				if(languageRule.Language==_selectedLanguageLoading) {
					tabControl1.SelectedTab=tabPageLanguage;
				}
			}
			LayoutManager.LayoutFormBoundsAndFonts(this);
		}

		private void butClose_Click(object sender,EventArgs e) {
			if(ListRulesNonDefault.Count==0) {
				List<string> listErrors=UIHelper.GetAllControls(this).OfType<UserControlReminderAgg>().First().ValidateTemplates();
				if(listErrors.Count!=0) {
					MessageBox.Show(Lan.g(this,"You must fix the following errors before continuing.")+"\r\n\r\n-"+string.Join("\r\n-",listErrors));
					return;
				}
				UIHelper.GetAllControls(this).OfType<UserControlReminderAgg>().First().SaveControlTemplates();
			}
			else {
				foreach(TabPage page in tabControl1.TabPages) {
					UserControlReminderAgg aggControl=(UserControlReminderAgg)page.Controls[0];
					List<string> listErrors=aggControl.ValidateTemplates();
					if(listErrors.Count!=0) {
						MessageBox.Show(Lan.g(this,"You must fix the following errors before continuing.")+"\r\n\r\n-"+string.Join("\r\n-",listErrors));
						return;
					}
					aggControl.SaveControlTemplates();
				}
			}
			DialogResult=DialogResult.OK;
		}
	}
}