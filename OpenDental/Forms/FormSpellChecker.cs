using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSpellChecker:FormODBase {

		public string TextRtf {
			get {
				return textMain.Rtf;
			}
		}

		public string TextPlain {
			get {
				return textMain.Text;
			}
		}

		public void SetText(string textIn) {
			try {
				textMain.Rtf=textIn;
			}
			catch {
				MsgBox.Show(this,"Invalid RTF. Clicking OK in this window will result in loss of formatting.");
				textMain.Text=textIn;
			}
		}

		public FormSpellChecker() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSpellChecker_Load(object sender,EventArgs e) {
			textMain.timerSpellCheck.Start();//so the spell check will run when form opens.
			LayoutMenu();
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",setupToolStripMenuItem_Click));
			menuMain.EndUpdate();
		}

		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormSpellCheck FormSC = new FormSpellCheck();
			FormSC.ShowDialog();
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}