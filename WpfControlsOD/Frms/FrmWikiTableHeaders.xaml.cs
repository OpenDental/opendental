using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmWikiTableHeaders:FrmODBase {

		public List<string> ListColNames;
		///<summary>Widths must always be specified.  Not optional.</summary>
		public List<int> ListColWidths;

		public FrmWikiTableHeaders() {
			InitializeComponent();
			//Lan.F(this);
		}

		private void FrmWikiTableHeaders_Loaded(object sender,RoutedEventArgs e) {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn;
			for(int i=1;i<ListColNames.Count+1;i++) {
				gridColumn=new GridColumn("",75);//needs to be editable
				gridMain.Columns.Add(gridColumn);
			}
			gridMain.ListGridRows.Clear();
			GridRow gridRow0=new GridRow();
			GridRow gridRow1=new GridRow();
			for(int i=0;i<ListColNames.Count;i++) {
				gridRow0.Cells.Add(ListColNames[i]);
				gridRow1.Cells.Add(ListColWidths[i].ToString());
				
			}
			gridMain.ListGridRows.Add(gridRow0);
			gridMain.ListGridRows.Add(gridRow1);
			gridMain.EndUpdate();
		}

		private void butSave_Click(object sender,EventArgs e) {
			for(int i=0;i<ListColNames.Count;i++) {
				ListColNames[i]=gridMain.ListGridRows[0].Cells[i].Text;
				try {
					ListColWidths[i]=PIn.Int(gridMain.ListGridRows[1].Cells[i].Text);
				}
				catch {
					MsgBox.Show(this,"Please enter only positive integer widths in the 2nd row");
					return;
				}
				if(ListColWidths[i]<1) {
					MsgBox.Show(this,"Please enter only positive integer widths in the 2nd row");
					return;
				}
			}
			IsDialogOK=true;
		}

		

		

		

		

		
	

	

		

		

		

	

	}
}