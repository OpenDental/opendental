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
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormLanguagesUsed:FormODBase {
		private CultureInfo[] AllCultures;
		private List<string> LangsUsed;

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
			AllCultures=CultureInfo.GetCultures(CultureTypes.NeutralCultures);
			string[] culturedescripts=new string[AllCultures.Length];
			for(int i=0;i<AllCultures.Length;i++) {
				culturedescripts[i]=AllCultures[i].DisplayName;
			}
			Array.Sort(culturedescripts,AllCultures);//sort based on descriptions
			for(int i=0;i<AllCultures.Length;i++) {
				listAvailable.Items.Add(AllCultures[i].DisplayName);
			}
			if(PrefC.GetString(PrefName.LanguagesUsedByPatients)=="") {
				LangsUsed=new List<string>();
			}
			else {
				LangsUsed=new List<string>(PrefC.GetString(PrefName.LanguagesUsedByPatients).Split(','));
			}
			FillListUsed();
		}

		///<summary>Also calls FillComboLanguagesIndicateNone().</summary>
		private void FillListUsed() {
			listUsed.Items.Clear();
			for(int i=0;i<LangsUsed.Count;i++) {
				if(LangsUsed[i]=="") {
					continue;
				}
				CultureInfo culture=CodeBase.MiscUtils.GetCultureFromThreeLetter(LangsUsed[i]);
				if(culture==null) {//custom language
					listUsed.Items.Add(LangsUsed[i]);
				}
				else {
					listUsed.Items.Add(culture.DisplayName);
				}
			}
			FillComboLanguagesIndicateNone();
		}

		private void FillComboLanguagesIndicateNone() {
			comboLanguagesIndicateNone.Items.Clear();
			for(int i=0;i<LangsUsed.Count;i++) {
				if(LangsUsed[i]=="") {
					continue;
				}
				CultureInfo culture=CodeBase.MiscUtils.GetCultureFromThreeLetter(LangsUsed[i]);
				if(culture==null) {//custom language
					comboLanguagesIndicateNone.Items.Add(LangsUsed[i]);//Only add custom languages to this combobox.
					if(LangsUsed[i]==PrefC.GetString(PrefName.LanguagesIndicateNone)) {
						comboLanguagesIndicateNone.SelectedIndex=comboLanguagesIndicateNone.Items.Count-1;//Select the item we just added.
					}
				}
			}
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(listAvailable.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a language first");
				return;
			}
			string lang=AllCultures[listAvailable.SelectedIndex].ThreeLetterISOLanguageName;//eng,spa etc
			if(LangsUsed.Contains(lang)) {
				MsgBox.Show(this,"Language already added.");
				return;
			}
			LangsUsed.Add(lang);
			FillListUsed();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(listUsed.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a language first");
				return;
			}
			List<string> listLangRules=ApptReminderRules.GetAll().FindAll(x => x.Language!=string.Empty).Select(x => x.Language).ToList();
			if(listLangRules.Contains(LangsUsed[listUsed.SelectedIndex])) {
				MsgBox.Show(this,"Language is in use by:\r\n - eService reminders or confirmations");
				return;
			}
			LangsUsed.RemoveAt(listUsed.SelectedIndex);
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
			int newIndex=listUsed.SelectedIndex-1;
			LangsUsed.Reverse(listUsed.SelectedIndex-1,2);
			FillListUsed();
			listUsed.SetSelected(newIndex);
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(listUsed.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a language first");
				return;
			}
			if(listUsed.SelectedIndex==listUsed.Items.Count-1) {
				return;
			}
			int newIndex=listUsed.SelectedIndex+1;
			LangsUsed.Reverse(listUsed.SelectedIndex,2);
			FillListUsed();
			listUsed.SetSelected(newIndex);
		}

		private void butAddCustom_Click(object sender,EventArgs e) {
			if(textCustom.Text=="") {
				MsgBox.Show(this,"Please enter a custom language first");
				return;
			}
			string lang=textCustom.Text;
			if(LangsUsed.Contains(lang)) {
				MsgBox.Show(this,"Language already added.");
				return;
			}
			LangsUsed.Add(lang);
			textCustom.Clear();
			FillListUsed();
		}

		private void butOK_Click(object sender,EventArgs e) {
			string str="";
			for(int i=0;i<LangsUsed.Count;i++) {
				if(i>0) {
					str+=",";
				}
				str+=LangsUsed[i];
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

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormLanguagesUsed_FormClosing(object sender,FormClosingEventArgs e) {
			//if LanguagesUsedByPatients does not contain LanguagesIndicateNone clear LanguagesIndicateNone
			if(!PrefC.GetString(PrefName.LanguagesUsedByPatients).Contains(PrefC.GetString(PrefName.LanguagesIndicateNone))) {
				Prefs.UpdateString(PrefName.LanguagesIndicateNone,"");
			}
		}

	}
}





















