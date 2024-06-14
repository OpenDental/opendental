using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormTasksForAppt:FormODBase {
		private long _aptNum;

		public FormTasksForAppt(long aptNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_aptNum=aptNum;
		}

		private void FormTasksForAppt_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.ListGridRows.Clear();
			GridColumn gridColumn=new GridColumn("Created",70,HorizontalAlignment.Left);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Completed",70,HorizontalAlignment.Left);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Description",70);
			gridColumn.IsWidthDynamic=true;
			gridMain.Columns.Add(gridColumn);
			List<long> listTaskNums=Tasks.GetMany(_aptNum).Select(x => x.TaskNum).ToList();
			DataTable table=Tasks.GetDataSet(0,new List<long>(),listTaskNums,"","","","","","",0,0,
				doIncludeTaskNote:false,doIncludeCompleted:true,doIncludeAttachments:false,reachedLimit:false,doRunOnReportServer:false);//GetDataSet orders in descending order by DateTimeOriginal if it exists, DateTimeEntry if not
			if(table==null) {
				gridMain.EndUpdate();
				return;
			}
			GridRow gridRow;
			for(int i=0;i<table.Rows.Count;i++) {
				gridRow=new GridRow();
				gridRow.Cells.Add(table.Rows[i]["dateCreate"].ToString());
				gridRow.Cells.Add(table.Rows[i]["dateComplete"].ToString());
				gridRow.Cells.Add(table.Rows[i]["description"].ToString());
				gridRow.Note=table.Rows[i]["note"].ToString();
				gridRow.ColorLborder=Color.Black;
				gridRow.ColorText=Color.FromArgb(PIn.Int(table.Rows[i]["color"].ToString()));
				gridMain.ListGridRows.Add(gridRow);
				gridRow.Tag=table.Rows[i]["TaskNum"].ToString();
			}
			gridMain.EndUpdate();
		}

		private void gridTasks_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			long taskNum=PIn.Long(gridMain.ListGridRows[e.Row].Tag.ToString());
			Task task=Tasks.GetOne(taskNum);
			if(task!=null) {
				using FormTaskEdit formTaskEdit=new FormTaskEdit(task);
				formTaskEdit.ShowDialog();
				if(formTaskEdit.DialogResult==DialogResult.OK){
					FillGrid();
					return;
				}
			}
			else {
				MsgBox.Show(this,"The task no longer exists.");
			}
		}

	}
}