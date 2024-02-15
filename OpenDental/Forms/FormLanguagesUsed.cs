using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormLanguagesUsed:FormODBase {
		private List<CultureInfo> _listCultureInfo;
		private List<string> _listLangsUsed;

		///<summary></summary>
		public FormLanguagesUsed() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormLanguagesUsed_Load(object sender,EventArgs e) {
			_listCultureInfo=CultureInfo.GetCultures(CultureTypes.NeutralCultures).OrderBy(x=>x.DisplayName).ToList();
			listAvailable.Items.AddStrings(_listCultureInfo.Select(x=>x.DisplayName));
			if(PrefC.GetString(PrefName.LanguagesUsedByPatients)=="") {
				_listLangsUsed=new List<string>();
				FillListUsed();
				return;
			}
			_listLangsUsed=new List<string>(PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(','));
			FillListUsed();
		}

		///<summary>Also calls FillComboLanguagesIndicateNone().</summary>
		private void FillListUsed() {
			listUsed.Items.Clear();
			for(int i=0;i<_listLangsUsed.Count;i++) {
				if(_listLangsUsed[i]=="") {
					continue;
				}
				CultureInfo cultureInfo=CodeBase.MiscUtils.GetCultureFromThreeLetter(_listLangsUsed[i]);
				if(cultureInfo==null) {//custom language
					listUsed.Items.Add(_listLangsUsed[i]);
					continue;
				}
				listUsed.Items.Add(cultureInfo.DisplayName);
			}
			FillComboLanguagesIndicateNone();
		}

		private void FillComboLanguagesIndicateNone() {
			comboLanguagesIndicateNone.Items.Clear();
			for(int i=0;i<_listLangsUsed.Count;i++) {
				if(_listLangsUsed[i]=="") {
					continue;
				}
				CultureInfo cultureInfo=CodeBase.MiscUtils.GetCultureFromThreeLetter(_listLangsUsed[i]);
				if(cultureInfo!=null){ //not a custom language
					continue;
				}
				//custom language
				comboLanguagesIndicateNone.Items.Add(_listLangsUsed[i]);//Only add custom languages to this combobox.
				if(_listLangsUsed[i]==PrefC.GetString(PrefName.LanguagesIndicateNone)) {
					comboLanguagesIndicateNone.SelectedIndex=comboLanguagesIndicateNone.Items.Count-1;//Select the item we just added.
				}
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(listAvailable.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a language first");
				return;
			}
			string lang=_listCultureInfo[listAvailable.SelectedIndex].ThreeLetterISOLanguageName;//eng,spa etc
			if(_listLangsUsed.Contains(lang)) {
				MsgBox.Show(this,"Language already added.");
				return;
			}
			_listLangsUsed.Add(lang);
			FillListUsed();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(listUsed.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a language first");
				return;
			}
			List<string> listLangRules=ApptReminderRules.GetAll().FindAll(x => x.Language!=string.Empty).Select(x => x.Language).ToList();
			if(listLangRules.Contains(_listLangsUsed[listUsed.SelectedIndex])) {
				MsgBox.Show(this,"Language is in use by:\r\n - eService reminders or confirmations");
				return;
			}
			_listLangsUsed.RemoveAt(listUsed.SelectedIndex);
			FillListUsed();
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(listUsed.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a language first");
				return;
			}
			if(listUsed.SelectedIndex==0) {
				return;
			}
			int indexNew=listUsed.SelectedIndex-1;
			_listLangsUsed.Reverse(listUsed.SelectedIndex-1,2);
			FillListUsed();
			listUsed.SetSelected(indexNew);
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(listUsed.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a language first");
				return;
			}
			if(listUsed.SelectedIndex==listUsed.Items.Count-1) {
				return;
			}
			int indexNew=listUsed.SelectedIndex+1;
			_listLangsUsed.Reverse(listUsed.SelectedIndex,2);
			FillListUsed();
			listUsed.SetSelected(indexNew);
		}

		private void butAddCustom_Click(object sender,EventArgs e) {
			if(textCustom.Text=="") {
				MsgBox.Show(this,"Please enter a custom language first");
				return;
			}
			string lang=textCustom.Text;
			if(_listLangsUsed.Contains(lang)) {
				MsgBox.Show(this,"Language already added.");
				return;
			}
			_listLangsUsed.Add(lang);
			textCustom.Clear();
			FillListUsed();
		}

		private void butSave_Click(object sender,EventArgs e) {
			string str="";
			for(int i=0;i<_listLangsUsed.Count;i++) {
				if(i>0) {
					str+=",";
				}
				str+=_listLangsUsed[i];
			}
			Prefs.UpdateString(PrefName.LanguagesUsedByPatients,str);
			if(comboLanguagesIndicateNone.SelectedIndex==-1) {
				Prefs.UpdateString(PrefName.LanguagesIndicateNone,"");
			}
			else {
				Prefs.UpdateString(PrefName.LanguagesIndicateNone,comboLanguagesIndicateNone.SelectedItem.ToString());
			}
			//prefs refresh handled by the calling form.
			DialogResult=DialogResult.OK;
		}

		private void FormLanguagesUsed_FormClosing(object sender,FormClosingEventArgs e) {
			//if LanguagesUsedByPatients does not contain LanguagesIndicateNone clear LanguagesIndicateNone
			if(!PrefC.GetString(PrefName.LanguagesUsedByPatients).Contains(PrefC.GetString(PrefName.LanguagesIndicateNone))) {
				Prefs.UpdateString(PrefName.LanguagesIndicateNone,"");
			}
		}

	}
}