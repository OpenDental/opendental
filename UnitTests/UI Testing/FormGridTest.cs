using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental;
using OpenDental.UI;
using OpenDentBusiness;

namespace UnitTests{
	public partial class FormGridTest : FormODBase{
		//private List<Patient> listPatients;
		private int _countLoop=200;
		private DataTable _table;

		public FormGridTest(){
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormGridTest_Load(object sender, EventArgs e){
			FillDataTable();
			labelRowsOld.Text="Rows: "+_table.Rows.Count.ToString();
			labelRowsNew.Text="Rows: "+_table.Rows.Count.ToString();
			FillGridOld();
			FillGridNew();
			//gridOld.PagingMode=GridOld.GridPagingMode.EnabledBottomUp;
			//gridOld.MaxPageRows=20;
			FillListBox();
			SetBorderColor(DefCatMiscColors.MainBorderOutline,Color.Yellow);
		}

		private void FillListBox(){
			for(int i=0;i<20;i++){
				listBoxOD.Items.Add("Item"+i.ToString());
			}
		}

		private void butFillOld_Click(object sender,EventArgs e) {
			DateTime dateTimeStart=DateTime.Now;
			Cursor=Cursors.WaitCursor;
			FillGridOld();
			Cursor=Cursors.Default;
			TimeSpan timeSpan=DateTime.Now-dateTimeStart;
			labelTimeOld.Text="Time: "+timeSpan.TotalMilliseconds.ToString("f0")+"ms";
		}

		private void butFillNew_Click(object sender,EventArgs e) {
			DateTime dateTimeStart=DateTime.Now;
			Cursor=Cursors.WaitCursor;
			FillGridNew();
			//gridMain.ScrollValue=0;
			Cursor=Cursors.Default;
			TimeSpan timeSpan=DateTime.Now-dateTimeStart;
			labelTimeNew.Text="Time: "+timeSpan.TotalMilliseconds.ToString("f0")+"ms";
		}

		private void FillDataTable(){
			List<Patient> listPatients=new List<Patient>();
			Patient patient;
			for(int i=0;i<_countLoop;i++){
				patient=new Patient();
				patient.LName="LName"+i.ToString();
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
				dataRow[1]=listPatients[i].FName;
				int randomRows=1;//random.Next(6)+1;//a number 1-6
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

		private void FillGridOld(){
			gridOld.BeginUpdate();
			gridOld.Columns.Clear();
			GridColumn col;
			col=new GridColumn("LNgamej",80);
			gridOld.Columns.Add(col);
			col=new GridColumn("FNamep",80);
			gridOld.Columns.Add(col);
			col=new GridColumn("Col2",80);
			//col.IsEditable=true;
			gridOld.Columns.Add(col);
			col=new GridColumn("Col3",80);
			gridOld.Columns.Add(col);
			col=new GridColumn("Col4",80);
			gridOld.Columns.Add(col);
			col=new GridColumn("Col5",80);
			gridOld.Columns.Add(col);
			col=new GridColumn("Col6",80);
			gridOld.Columns.Add(col);
			col=new GridColumn("Col7",80);
			gridOld.Columns.Add(col);
			gridOld.NoteSpanStop=1;
			//gridOld.FuncConstructGridRow=GridProgRowConstructionOld;//dataRow => GridProgRowConstruction(dataRow as DataRow);
			List<DataRow> listDataRows=_table.Select().ToList();
			//gridOld.SetPagingData(listDataRows);
			#region old
			for(int i=0;i<_table.Rows.Count;i++){
				GridRow row=new GridRow();
				for(int c=0;c<_table.Columns.Count;c++){
					row.Cells.Add(_table.Rows[i][c].ToString());
				}
				//System.Threading.Thread.Sleep(1);
				gridOld.ListGridRows.Add(row);
			}
			/*gridOld.ListGridRows.Clear();			
			GridRow row;
			for(int i=0;i<_listPatients.Count;i++){
				row=new GridRow();
				if(i==2){
					row.Cells.Add(new GridCell("click"){ColorBackG=Color.LightGray,ClickEvent=DeleteClick });
				}
				//else if(i==400){
				//	row.Cells.Add(textLong);
				//}
				else{
					row.Cells.Add(_listPatients[i].LName);//+" "+_listPatients[i].FName+" "+_listPatients[i].FName);
				}
				if(i==10){
					row.Cells.Add("10");//textLong+textLong+textLong+textLong+textLong+textLong+textLong+textLong);
				}
				else{
					row.Cells.Add(_listPatients[i].FName);
				}
				row.Cells.Add("Col2 Row"+i.ToString());
				row.Cells.Add("Col3 Row"+i.ToString());
				row.Cells.Add("Col4 Row"+i.ToString());
				row.Cells.Add("Col5 Row"+i.ToString());
				row.Cells.Add("Col6 Row"+i.ToString());
				row.Cells.Add("Col7 Row"+i.ToString());
				
				/*if(i==5){
					row.DropDownParent=gridOld.ListGridRows[4];
				}
				if(i==6){
					row.DropDownParent=gridOld.ListGridRows[4];
				}
				if(i==7){
					row.DropDownParent=gridOld.ListGridRows[4];
				}
				if(i==8){
					row.DropDownParent=gridOld.ListGridRows[7];
				}
				if(i==10){
					row.Note="Some note";
				}
				System.Threading.Thread.Sleep(1);
				gridOld.ListGridRows.Add(row);
			}*/
			#endregion old
			gridOld.EndUpdate();
			//gridOld.SetSelected(4,true);
		}

		private void FillGridNew(){
			gridNew.BeginUpdate();
			gridNew.Columns.Clear();
			GridColumn col;
			col=new GridColumn("LNgamej",80);
			gridNew.Columns.Add(col);
			col=new GridColumn("FNamep",80);
			gridNew.Columns.Add(col);
			col=new GridColumn("Col2",80);
			//col.IsEditable=true;
			gridNew.Columns.Add(col);
			col=new GridColumn("Col3",80);
			gridNew.Columns.Add(col);
			col=new GridColumn("Col4",80);
			gridNew.Columns.Add(col);
			col=new GridColumn("Col5",80);
			gridNew.Columns.Add(col);
			col=new GridColumn("Col6",80);
			gridNew.Columns.Add(col);
			col=new GridColumn("Col7",80);
			gridNew.Columns.Add(col);
			gridNew.NoteSpanStop=1;
			//gridNew.FuncConstructGridRow=GridProgRowConstruction;//dataRow => GridProgRowConstruction(dataRow as DataRow);
			//List<DataRow> listDataRows=_table.Select().ToList();//This step does take over a second with a million rows.  Slightly annoying.
			//gridNew.SetData(listDataRows);
			gridNew.ListGridRows.Clear();
			for(int i=0;i<_table.Rows.Count;i++){
				//GridRow gridRow=GridProgRowConstruction(_table.Rows[i]);
				GridRow gridRow=new GridRow();
				for(int c=0;c<_table.Rows[i].ItemArray.Length;c++){
					GridCell gridCell=new GridCell(_table.Rows[i][c].ToString());
					//if(i==1 && c==1){
					//	gridCell.ColorBackG=Color.Red;
					//}
					//if(i==1 && c==3){
					//	gridCell.ColorBackG=Color.Purple;
					//}
					gridRow.Cells.Add(gridCell);
				}
				//gridRow.ColorBackG=Color.FromArgb(215,230,235);
					//250,250,235);
					//light blue:235,249,255);//highlight is 205,220,235
				gridNew.ListGridRows.Add(gridRow);
			}
			#region old
			//gridNew.ListGridRows.Clear();
			//for(int i=0;i<_table.Rows.Count;i++){
				//GridRow row=new GridRow();
				//for(int c=0;c<_table.Columns.Count;c++){
				//	row.Cells.Add(_table.Rows[i][c].ToString());
				//}
				//GridCell cell;
				//if(i==2){
				//	row.Cells.Add(new GridCell("click"){ColorBackG=Color.LightGray,ClickEvent=DeleteClick });
				//}
				//else if(i==400){
				//	row.Cells.Add(textLong);
				//}
				//else{
				//row.Cells.Add(listPatients[i].LName);//+" "+_listPatients[i].FName+" "+_listPatients[i].FName);
				//}
				//if(i==10){
				//	row.Cells.Add("10");//textLong+textLong+textLong+textLong+textLong+textLong+textLong+textLong);
				//}
				//else{
				//row.Cells.Add(listPatients[i].FName);
				//}
				//cell=new GridCell("Col2 Row"+i.ToString());
				//if(i==5){
				//	cell.ColorBackG=Color.Red;
				//}
				//row.Cells.Add(cell);
				//cell=new GridCell("Col3 Row"+i.ToString());
				//if(i==5){
				//	cell.ColorBackG=Color.Gray;
				//}
				//row.Cells.Add(cell);
				//cell=new GridCell("Col4 Row"+i.ToString());
				//if(i==5){
				//	cell.ColorBackG=Color.Black;
				//}
				//row.Cells.Add(cell);
				//row.Cells.Add("Col5 Row"+i.ToString());
				//row.Cells.Add("Col6 Row"+i.ToString());
				//row.Cells.Add("Col7 Row"+i.ToString());
				/*if(i==5){
					row.DropDownParent=gridNew.ListGridRows[4];
				}
				if(i==6){
					row.DropDownParent=gridNew.ListGridRows[4];
				}
				if(i==7){
					row.DropDownParent=gridNew.ListGridRows[4];
				}
				if(i==8){
					row.DropDownParent=gridNew.ListGridRows[7];
				}*/
				//if(i==10){
				//	row.Note="Some note";
				//}
				//System.Threading.Thread.Sleep(1);
				//gridNew.ListGridRows.Add(row);
			//}
			#endregion old
			gridNew.EndUpdate();
			//gridNew.SetSelected(4,true);
			//gridNew.ScrollToEnd();
		}

		///<summary>Converts a DataRow to a GridRow.</summary>
		private GridRow GridProgRowConstruction(DataRow dataRow) {
			GridRow gridRow=new GridRow();
			for(int c=0;c<dataRow.ItemArray.Length;c++){
				gridRow.Cells.Add(dataRow[c].ToString());
			}
			//System.Threading.Thread.Sleep(1);
			return gridRow;
		}

		///<summary>Converts a DataRow to a GridRow.</summary>
		private GridRow GridProgRowConstructionOld(object objectDataRow) {
			DataRow dataRow=objectDataRow as DataRow;
			GridRow gridRow=new GridRow();
			for(int c=0;c<dataRow.ItemArray.Length;c++){
				gridRow.Cells.Add(dataRow[c].ToString());
			}
			//System.Threading.Thread.Sleep(1);
			return gridRow;
		}

		private void butWider_Click(object sender,EventArgs e) {
			
		}

		private void butPrint_Click(object sender, EventArgs e){
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,"");
		}

		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			text="Heading";
			g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
			yPos+=(int)g.MeasureString(text,headingFont).Height;
			yPos=gridNew.PrintPage(g,0,bounds,yPos);
			//if(yPos==-1) {
			//	e.HasMorePages=true;
			//}
			//else {
				e.HasMorePages=false;
			//}
		}

		private void butEnd_Click(object sender,EventArgs e) {
			gridNew.ScrollToEnd();
		}

		private void butScroll_Click(object sender, EventArgs e){
			int scrollVal=gridNew.ScrollValue;
			FillGridNew();
			gridNew.ScrollValue=scrollVal;
		}

		private void DeleteClick(object sender,EventArgs e) {
			MsgBox.Show("Clicked");
		}

		/*
		private void Button1_Click(object sender, EventArgs e){
			int width=gridMain.GetIdealWidth();
			MsgBox.Show(width.ToString());
			//_countLoop++;
			//FillPatients();
			//FillGrid();
			/*
			Font _fontCell=new Font(FontFamily.GenericSansSerif,8.5f);
			Font _fontCellBold=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Bold);
			int h1=_fontCell.Height;
			int h2=_fontCellBold.Height;
		}

		private void butMaximized_Click(object sender,EventArgs e) {
			FormGroupBoxTests formGroupBoxTests=new FormGroupBoxTests();
			formGroupBoxTests.Show();
		}*/

		/*private void ButObjects_Click(object sender, EventArgs e){
			//2M basic objects per second, 500k fonts per second
			DateTime dt=DateTime.Now;
			ODGridRow row;
			List<ODGridRow> listRows=new List<ODGridRow>();
			Font[] fonts=new Font[1000000];
			for(int i=0;i<1000000;i++){
				Font font=new Font("Arial",8);
				fonts[i]=font;
				row=new ODGridRow();
				row.RowNum=i;
				row.Tag="tag";
				row.Note="note";
				row.RowHeight=20;
				listRows.Add(row);
			}
			TimeSpan time=DateTime.Now-dt;
			MessageBox.Show(fonts[2].ToString()+"   "+time.ToString());
		}*/

		private string textLong=@"
Amoxicillin 250mg/5mL (>65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs)65lbs";

		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode==Keys.Up || e.KeyCode==Keys.Down) {
				//e.Handled=true;
			}
		}
	}
}
