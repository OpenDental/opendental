using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormWikiTableHeaders:FormODBase {
		public List<string> ColNames;
		///<summary>Widths must always be specified.  Not optional.</summary>
		public List<int> ColWidths;

		public FormWikiTableHeaders() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWikiTableHeaders_Load(object sender,EventArgs e) {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			for(int i=1;i<ColNames.Count+1;i++) {
				col=new GridColumn("",75,true);
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row0=new GridRow();
			GridRow row1=new GridRow();
			for(int i=0;i<ColNames.Count;i++) {
				row0.Cells.Add(ColNames[i]);
				row1.Cells.Add(ColWidths[i].ToString());
			}
			gridMain.ListGridRows.Add(row0);
			gridMain.ListGridRows.Add(row1);
			gridMain.EndUpdate();
		}

		private void butOK_Click(object sender,EventArgs e) {
			for(int i=0;i<ColNames.Count;i++) {
				ColNames[i]=gridMain.ListGridRows[0].Cells[i].Text;
				try {
					ColWidths[i]=PIn.Int(gridMain.ListGridRows[1].Cells[i].Text);
				}
				catch {
					MsgBox.Show(this,"Please enter only positive integer widths in the 2nd row");
					return;
				}
				if(ColWidths[i]<1) {
					MsgBox.Show(this,"Please enter only positive integer widths in the 2nd row");
					return;
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		

		
	

	

		

		

		

	

	}
}