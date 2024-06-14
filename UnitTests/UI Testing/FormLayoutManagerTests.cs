using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental;
using OpenDental.UI;

namespace UnitTests{
	public partial class FormLayoutManagerTests:FormODBase{
		public FormLayoutManagerTests(){
			InitializeComponent();
			InitializeLayoutManager();
			//InitializeUIManager(isDpiSystem:true);
			//InitializeUIManager();
		}

		private void FormLayoutManagerTests_Load(object sender,EventArgs e){
			//bool isHandleCreated=butDelete.IsHandleCreated;//true
			//butDelete.Text="New text";//verified that this will fail as it should
			listBox1.Items.Clear();
			for(int i=0;i<30;i++){
				listBox1.Items.Add("Item"+i.ToString());
			}
			comboBox1.Items.Clear();
			for(int i=0;i<300;i++){
				comboBox1.Items.Add("Item"+i.ToString());
			}
			//FillGrid();
			/*
			System.Windows.Controls.Canvas canvas=new System.Windows.Controls.Canvas();
			canvas.Background=System.Windows.Media.Brushes.Red;
			elementHost1.Child = canvas;
			canvas.KeyDown+=Canvas_KeyDown;
			System.Windows.Controls.TextBox textBox=new System.Windows.Controls.TextBox();
			textBox.Width=100;
			canvas.Children.Add(textBox);*/
		}

		private void FormLayoutManagerTests_KeyDown(object sender,KeyEventArgs e) {
			MsgBox.Show("Form");
		}

		private void Canvas_KeyDown(object sender,System.Windows.Input.KeyEventArgs e) {
			//MsgBox.Show("Canvas");
		}

		private void FillGrid(){
			/*
			DataTable table=new DataTable();
			table.Columns.Add("LName");
			table.Columns.Add("FName");
			for(int i=0;i<300;i++){
				DataRow dataRow=table.NewRow();
				if(i%3==0){
					dataRow[0]="LName\r\nsecond row\r\nthird row"+i.ToString();
				}
				else{
					dataRow[0]="LName"+i.ToString();
				}
				dataRow[1]="FName"+i.ToString();
				table.Rows.Add(dataRow);
			}
			_um.Grid_ColumnsClear("gridMain");
			GridColumn col=new GridColumn("LastName",100);
			_um.Grid_ColumnAdd("gridMain",col);
			col=new GridColumn("FirstName",100,HorizontalAlignment.Center);
			col.IsWidthDynamic=true;
			_um.Grid_ColumnAdd("gridMain",col);
			List<GridRow> listGridRows=new List<GridRow>();
			for(int i=0;i<table.Rows.Count;i++){
				GridRow row=new GridRow();
				row.Cells.Add(table.Rows[i]["LName"].ToString());
				row.Cells.Add(table.Rows[i]["FName"].ToString());
				listGridRows.Add(row);
			}
			_um.Grid_SetData("gridMain",listGridRows);*/
		}

		private void butDelete_Click(object sender,EventArgs e) {
			//float fontSize=textBox1.Font.Size;
			//OpenDental.MessageBox.Show(fontSize.ToString());
		}

		private void textBox1_FontChanged(object sender,EventArgs e) {
			//float fontSize=textBox1.Font.Size;
			return;
		}

		private void button1_Click(object sender,EventArgs e) {
			//FormTextBoxTests formTextBoxTests=new FormTextBoxTests();
			//formTextBoxTests.ShowDialog();
			//button1.Text="NewText";
			//_um.SetText("button1","NewText");
			//string text=_um.GetText("button1");
		}

		private void butDelete_MouseUp(object sender,MouseEventArgs e) {
			return;
		}

		private void button1_MouseUp(object sender,MouseEventArgs e) {
			return;
		}

		private void button2_Click(object sender,EventArgs e) {
			//_um.ComboBox_SetSelectedIndex("comboBox1",3);
		}

		private void button1_Click_1(object sender,EventArgs e) {
			//_um.SetIsChecked("checkBox6",false);
		}

		private void button3_Click(object sender,EventArgs e) {
			//int selectedIndex=_um.ComboBox_GetSelectedIndex("comboBox1");
			//MsgBox.Show(selectedIndex.ToString());
			MsgBox.Show("Cancel");
			//DialogResult=DialogResult.Cancel;
			//Close();
		}

		private void checkBox6_Click(object sender,EventArgs e) {
			//bool? isChecked=_um.GetIsChecked("checkBox6");
			//MsgBox.Show(isChecked.ToString());
		}

		private void button1_Click_2(object sender,EventArgs e) {
			//FillGrid();
			MsgBox.Show("OK");
		}

		
	}
}
