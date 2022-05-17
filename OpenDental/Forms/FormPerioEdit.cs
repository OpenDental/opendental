using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPerioEdit : FormODBase {
		public PerioExam PerioExamCur;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormPerioEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPerioEdit_Load(object sender, System.EventArgs e) {
			textDate.Text=PerioExamCur.ExamDate.ToShortDateString();
			textBoxNotes.Text=PerioExamCur.Note;
			listProv.Items.Clear();
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				listProv.Items.Add(_listProviders[i].Abbr);
				if(_listProviders[i].ProvNum==PerioExamCur.ProvNum){
					listProv.SelectedIndex=i;
				}
			}
			if(listProv.SelectedIndex==-1) {
				listProv.SelectedIndex=0;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(string.IsNullOrEmpty(textDate.Text) || !textDate.IsValid()){
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			PerioExamCur.ExamDate=PIn.Date(textDate.Text);
			PerioExamCur.Note=PIn.String(textBoxNotes.Text);
			PerioExamCur.ProvNum=_listProviders[listProv.SelectedIndex].ProvNum;
			PerioExams.Update(PerioExamCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	


	}
}





















