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
		private QuestionDef[] QuestionList;

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
			QuestionList=QuestionDefs.Refresh();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableQuestionDefs","Type"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableQuestionDefs","Question"),570);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<QuestionList.Length;i++){
				row=new GridRow();
				row.Cells.Add(Lan.g("enumQuestionType",QuestionList[i].QuestType.ToString()));
				row.Cells.Add(QuestionList[i].Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormQuestionDefEdit FormQ=new FormQuestionDefEdit(QuestionList[e.Row]);
			FormQ.ShowDialog();
			if(FormQ.DialogResult!=DialogResult.OK)
				return;
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			QuestionDef def=new QuestionDef();
			def.ItemOrder=QuestionList.Length;
			using FormQuestionDefEdit FormQ=new FormQuestionDefEdit(def);
			FormQ.IsNew=true;
			FormQ.ShowDialog();
			FillGrid();
		}

		private void butUp_Click(object sender,EventArgs e) {
			int selected=gridMain.GetSelectedIndex();
			try{
				QuestionDefs.MoveUp(selected,QuestionList);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			FillGrid();
			if(selected==0) {
				gridMain.SetSelected(0,true);
			}
			else{
				gridMain.SetSelected(selected-1,true);
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			int selected=gridMain.GetSelectedIndex();
			try {
				QuestionDefs.MoveDown(selected,QuestionList);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			FillGrid();
			if(selected==QuestionList.Length-1) {
				gridMain.SetSelected(selected,true);
			}
			else{
				gridMain.SetSelected(selected+1,true);
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

	

		

		

		

		

		

		

		

		


	}
}



























