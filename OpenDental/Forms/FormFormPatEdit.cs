using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDental.ReportingComplex;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormFormPatEdit : FormODBase {
		//private Question[] QuestionList;
		private QuestionDef[] QuestionDefList;
		//private ContrMultInput multInput;
		public FormPat FormPatCur;
		//public bool IsNew;

		///<summary></summary>
		public FormFormPatEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			//PatNum=patNum;
			//FormPatCur=formPatCur.Clone();
			//multInput.IsQuestionnaire=true;
		}
		
		private void FormFormPatEdit_Load(object sender,EventArgs e) {
			QuestionDefList=QuestionDefs.Refresh();
			/*if(IsNew){
				gridMain.Visible=false;
				butDelete.Visible=false;
				//only gets filled once on startup, and not saved until OK.
				for(int i=0;i<QuestionDefList.Length;i++) {
					if(QuestionDefList[i].QuestType==QuestionType.FreeformText) {
						multInput.AddInputItem(QuestionDefList[i].Description,FieldValueType.String,"");
					}
					else if(QuestionDefList[i].QuestType==QuestionType.YesNoUnknown) {
						multInput.AddInputItem(QuestionDefList[i].Description,FieldValueType.YesNoUnknown,YN.Unknown);
					}
				}
			}
			else {*/
			butOK.Visible=false;
			butCancel.Text=Lan.g(this,"Close");
			//multInput.Visible=false;
			//Gets filled repeatedly.  Saved each time user double clicks on a row.  Only the answer can be edited.
			FillGrid();
			//}
			/*QuestionDefList=QuestionDefs.Refresh();
			QuestionList=Questions.Refresh(PatNum);
			if(QuestionList.Length==0){
				IsNew=true;
				gridMain.Visible=false;
				butDelete.Visible=false;
				//only gets filled once on startup, and not saved until OK.
				for(int i=0;i<QuestionDefList.Length;i++){
					if(QuestionDefList[i].QuestType==QuestionType.FreeformText){
						multInput.AddInputItem(QuestionDefList[i].Description,FieldValueType.String,"");
					}
					else if(QuestionDefList[i].QuestType==QuestionType.YesNoUnknown) {
						multInput.AddInputItem(QuestionDefList[i].Description,FieldValueType.YesNoUnknown,YN.Unknown);
					}
				}
			}
			else{
				IsNew=false;
				butOK.Visible=false;
				butCancel.Text=Lan.g(this,"Close");
				multInput.Visible=false;
				//Gets filled repeatedly.  Saved each time user double clicks on a row.  Only the answer can be edited.
				FillGrid();
			}*/
		}

		private void FormFormPatEdit_Shown(object sender,EventArgs e) {
			//if(IsNew){
			//	if(QuestionDefList.Length==0){
			//		MsgBox.Show(this,"Go to Setup | Obsolete | Questionnaire to setup questions for all patients.");
			//	}
			//}
		}

		private void FillGrid(){
			//QuestionList=Questions.Refresh(PatNum);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableQuestions","Question"),370);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableQuestions","Answer"),370);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<FormPatCur.QuestionList.Count;i++){
				row=new GridRow();
				row.Cells.Add(FormPatCur.QuestionList[i].Description);
				row.Cells.Add(FormPatCur.QuestionList[i].Answer);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//only visible if editing existing quesionnaire.
			using InputBox input=new InputBox(FormPatCur.QuestionList[e.Row].Description);
			input.textResult.Text=FormPatCur.QuestionList[e.Row].Answer;
			input.ShowDialog();
			if(input.DialogResult==DialogResult.OK){
				FormPatCur.QuestionList[e.Row].Answer=input.textResult.Text;
				Questions.Update(FormPatCur.QuestionList[e.Row]);
			}
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//only visible if editing existing quesionnaire.
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this questionnaire?")){
				return;
			}
			FormPats.Delete(FormPatCur.FormPatNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(QuestionDefList.Length==0){
				MsgBox.Show(this,"No questions to save.");
				return;
			}
			//only visible if IsNew
			/*
			FormPats.Insert(FormPatCur);
			Question quest;
			ArrayList ALval;
			for(int i=0;i<QuestionDefList.Length;i++){
				quest=new Question();
				quest.PatNum=FormPatCur.PatNum;
				quest.ItemOrder=QuestionDefList[i].ItemOrder;
				quest.Description=QuestionDefList[i].Description;
				if(QuestionDefList[i].QuestType==QuestionType.FreeformText){
					ALval=multInput.GetCurrentValues(i);
					if(ALval.Count>0){
						quest.Answer=ALval[0].ToString();
					}
					//else it will just be blank
				}
				else if(QuestionDefList[i].QuestType==QuestionType.YesNoUnknown){
					quest.Answer=Lan.g("enumYN",multInput.GetCurrentValues(i)[0].ToString());
				}
				quest.FormPatNum=FormPatCur.FormPatNum;
				Questions.Insert(quest);
			}*/
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			Close();
		}

		

		

	

		


	}
}





















