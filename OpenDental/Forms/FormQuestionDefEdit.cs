using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormQuestionDefEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private QuestionDef QuestionDefCur;

		///<summary></summary>
		public FormQuestionDefEdit(QuestionDef questionDefCur)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			QuestionDefCur=questionDefCur;
		}

		private void FormQuestionDefEdit_Load(object sender, System.EventArgs e) {
			listType.Items.AddEnums<QuestionType>();
			listType.SetSelectedEnum(QuestionDefCur.QuestType);
			textQuestion.Text=QuestionDefCur.Description;
		}

		private void buttonDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			QuestionDefs.Delete(QuestionDefCur);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			QuestionDefCur.QuestType=listType.GetSelected<QuestionType>();
			QuestionDefCur.Description=textQuestion.Text;
			if(IsNew){
				QuestionDefs.Insert(QuestionDefCur);
			}
			else{
				QuestionDefs.Update(QuestionDefCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	

		


	}
}





















