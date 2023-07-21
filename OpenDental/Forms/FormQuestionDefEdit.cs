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
		private QuestionDef _questionDef;

		///<summary></summary>
		public FormQuestionDefEdit(QuestionDef questionDef)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_questionDef=questionDef;
		}

		private void FormQuestionDefEdit_Load(object sender, System.EventArgs e) {
			listType.Items.AddEnums<QuestionType>();
			listType.SetSelectedEnum(_questionDef.QuestType);
			textQuestion.Text=_questionDef.Description;
		}

		private void buttonDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			QuestionDefs.Delete(_questionDef);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			_questionDef.QuestType=listType.GetSelected<QuestionType>();
			_questionDef.Description=textQuestion.Text;
			if(IsNew){
				QuestionDefs.Insert(_questionDef);
			}
			else{
				QuestionDefs.Update(_questionDef);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	

		


	}
}





















