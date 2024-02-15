using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// </summary>
	public partial class FormQuestionDefs:FormODBase {
		private QuestionDef[] _questionDefArray;

		///<summary></summary>
		public FormQuestionDefs()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormQuestionDefs_Load(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			_questionDefArray=QuestionDefs.Refresh();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableQuestionDefs","Type"),110);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableQuestionDefs","Question"),570);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_questionDefArray.Length;i++){
				row=new GridRow();
				row.Cells.Add(Lan.g("enumQuestionType",_questionDefArray[i].QuestType.ToString()));
				row.Cells.Add(_questionDefArray[i].Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormQuestionDefEdit formQuestionDefEdit=new FormQuestionDefEdit(_questionDefArray[e.Row]);
			formQuestionDefEdit.ShowDialog();
			if(formQuestionDefEdit.DialogResult!=DialogResult.OK){
				return;
			}
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			QuestionDef questionDef=new QuestionDef();
			questionDef.ItemOrder=_questionDefArray.Length;
			using FormQuestionDefEdit formQuestionDefEdit=new FormQuestionDefEdit(questionDef);
			formQuestionDefEdit.IsNew=true;
			formQuestionDefEdit.ShowDialog();
			FillGrid();
		}

		private void butUp_Click(object sender,EventArgs e) {
			int selected=gridMain.GetSelectedIndex();
			try{
				QuestionDefs.MoveUp(selected,_questionDefArray);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			FillGrid();
			if(selected==0) {
				gridMain.SetSelected(0,true);
				return;
			}
			gridMain.SetSelected(selected-1,true);
			
		}

		private void butDown_Click(object sender,EventArgs e) {
			int selected=gridMain.GetSelectedIndex();
			try {
				QuestionDefs.MoveDown(selected,_questionDefArray);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			FillGrid();
			if(selected==_questionDefArray.Length-1) {
				gridMain.SetSelected(selected,true);
				return;
			}
			gridMain.SetSelected(selected+1,true);
			
		}

	}
}