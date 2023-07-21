using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
	public partial class FrmGridTest:FrmODBase {
		private int _countLoop=60;
		private DataTable _table;

		///<summary></summary>
		public FrmGridTest(){
			InitializeComponent();
		}

		private void FrmODBase_Loaded(object sender,RoutedEventArgs e) {
			FillDataTable();
			//FillGrid();
		}

		private void FillDataTable(){
			List<Patient> listPatients=new List<Patient>();
			Patient patient;
			for(int i=0;i<_countLoop;i++){
				patient=new Patient();
				patient.LName="LNamey"+i.ToString();
				patient.FName="FName"+i.ToString();
				listPatients.Add(patient);
			}
			_table=new DataTable();
			for(int i=0;i<8;i++){
				_table.Columns.Add(i.ToString());
			}
			Random random=new Random();
			for(int i=0;i<listPatients.Count;i++){
				DataRow dataRow=_table.NewRow();
				dataRow[0]=listPatients[i].LName;
				dataRow[1]=listPatients[i].FName;//+"longertextthatspillsoff";
				int randomRows=random.Next(4)+1;//a number 1-4
				string rowVal="";
				for(int r=1;r<=randomRows;r++){
					if(r>1){
						rowVal+="\r\n";
					}
					rowVal+="Line "+r.ToString();
				}
				dataRow[2]=rowVal;
				dataRow[3]="Col3 Row"+i.ToString();
				dataRow[4]="Col4 Row"+i.ToString();
				dataRow[5]="Col5 Row"+i.ToString();
				dataRow[6]="Col6 Row"+i.ToString();
				dataRow[7]="Col7 Row"+i.ToString();
				_table.Rows.Add(dataRow);
			}
		}

		private void butClear_Click(object sender,EventArgs e) {
			gridMain.BeginUpdate();
			gridMain.ListGridRows.Clear();
			gridMain.EndUpdate();
		}
		
		private void Button_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn("LNgamej",80);
			col.IsEditable=true;
			gridMain.Columns.Add(col);
			col=new GridColumn("FNamep",80);
			col.IsEditable=true;
			gridMain.Columns.Add(col);
			col=new GridColumn("Col2",80);
			col.IsEditable=true;
			gridMain.Columns.Add(col);
			col=new GridColumn("Col3",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Col4",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Col5",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Col6",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Col7",80);
			gridMain.Columns.Add(col);
			//gridMain.NoteSpanStop=1;
			//gridNew.FuncConstructGridRow=GridProgRowConstruction;//dataRow => GridProgRowConstruction(dataRow as DataRow);
			//List<DataRow> listDataRows=_table.Select().ToList();//This step does take over a second with a million rows.  Slightly annoying.
			//gridNew.SetData(listDataRows);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_table.Rows.Count;i++){
				//GridRow gridRow=GridProgRowConstruction(_table.Rows[i]);
				GridRow gridRow=new GridRow();
				for(int c=0;c<_table.Rows[i].ItemArray.Length;c++){
					GridCell gridCell=new GridCell(_table.Rows[i][c].ToString());
					if(i==1 && c==1) {
						gridCell.ColorBackG=Colors.Red;
					}
					if(i==1 && c==3) {
						gridCell.ColorBackG=Colors.Purple;
						gridCell.ColorText=Colors.White;
					}
					if(i==5 && c==2){
						gridCell.Bold=false;
					}
					if(i==6 && c==4) {
						gridCell.ColorBackG=Colors.Orange;
					}
					gridRow.Cells.Add(gridCell);
				}
				if(i==1){
					gridRow.ColorText=Colors.DarkGreen;
				}
				if(i==5){
					gridRow.Bold=true;
				}
				if(i==6){
					gridRow.ColorBackG=Colors.Blue;
				}
					//250,250,235);
					//light blue:235,249,255);//highlight is 205,220,235
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(4,true);
			//gridMain.ScrollToEnd();
		}

		private void gridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			MsgBox.Show("Double click Row:"+e.Row.ToString()+", Col:"+e.Col.ToString());
		}

		private void gridMain_CellClick(object sender,GridClickEventArgs e) {
			//MsgBox.Show("Click Row:"+e.Row.ToString()+", Col:"+e.Col.ToString());
		}
	}
}
