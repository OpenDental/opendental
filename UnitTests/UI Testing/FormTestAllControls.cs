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
	public partial class FormTestAllControls:FormODBase{
		public FormTestAllControls(){
			InitializeComponent();
			//comboClinic.IsTestModeNoDb=true;
			InitializeLayoutManager();
			//InitializeUIManager(isDpiSystem:true);
			//InitializeUIManager();
		}

		private void FormTestAllControls_Load(object sender,EventArgs e){
			//bool isHandleCreated=butDelete.IsHandleCreated;//true
			//butDelete.Text="New text";//verified that this will fail as it should
			//_um.ListBox_Items_Clear("listBox1");
			//for(int i=0;i<40;i++){
			//	_um.ListBox_Items_Add("listBox1","Item"+i.ToString());
			//}
			//List<OpenDental.Wui.ListBoxItem> listListBoxItems=Lan.EnumsGetForListBox<DayOfWeek>();
			//_um.ListBox_Items_AddList("listBox1",listListBoxItems);
			//_um.ComboBox_Items_Clear("comboBox1");
			for(int i=0;i<30;i++){
				comboBox1.Items.Add(i.ToString());
			}
			//FillGrid();
			//LayoutMenu();
			//LayoutToolBar();
			//_um.ContextMenuStrip_SetEventOpening("gridMain",contextMenu_Opening);
			//_um.LinkLabel_SetEvent_LinkClicked("linkLabel1",linkLabel1_LinkClicked);
			bool canFocus=groupBox3.CanFocus;
			/*
			string myText=//"Line 1\r\nLine2";
				@"{\rtf1\ansi\deff0\nouicompat{\fonttbl{\f0\fnil Microsoft Sans Serif;}}
{\*\generator Riched20 10.0.22621}\viewkind4\uc1 
\pard\f0\fs17\lang1033 Line 1 \b Bold\b0  text.\par
Line2\par
}
";*/
			//odTextBox.Rtf=myText;
			//textRich.Rtf="";
		}

		

		private void LayoutMenu(){
			//_um.Menu_AddToMain("menuMain","File",menuItemFile_Click);
			//_um.Menu_AddToMain("menuMain","Top2",menuItemTop2_Click);
			//MenuItemOD menuItemReports=new MenuItemOD("Reports");//Use a local variable if there are children
			//menuItemReports.Name="menuItemReports";
			//menuItemReports.Add("Daily",menuItemDaily_Click);//Normal simple pattern when there are no children
			//MenuItemOD menuItemWeekly=new MenuItemOD("Weekly",menuItemWeekly_Click);//Also use a local variable if you want to set more properties.
			//menuItemWeekly.Checked=true;
			//menuItemReports.Add(menuItemWeekly);
			//menuItemReports.AddSeparator();
			//MenuItemOD menuItemMonthly=new MenuItemOD("Monthly",menuItemMonthly_Click);//Use a named MenuItem can be referenced later if it has a name. for example to set Available=false
			////menuItemReports.Add(menuItemMonthly);
			//_um.Menu_AddTreeToMain("menuMain",menuItemReports);
			//_um.Menu_AddToMain("menuMain","Top3",menuItemTop3_Click);
			////Adding later:
			//_um.Menu_AddTreeToItem("menuMain","menuItemReports",menuItemMonthly);//Just one item in this example tree, but children are allowed.
			////2. Simpler unnamed.
			//_um.Menu_AddToItem("menuMain","menuItemReports","Quarterly",menuItemQuarterly_Click);
		}

		private void LayoutToolBar() {
			//_um.ToolBar_Clear("toolBarMain");
			//_um.ToolBar_Add("toolBarMain",new ODToolBarButton("Add",Add_Click,EnumIcons.Add));
			////_um.ToolBar_AddSeparator("toolBarMain");
			//ODToolBarButton toolBarButton=new ODToolBarButton(Lan.g(this,"Edit"),Edit_Click);
			//toolBarButton.Bitmap=(Bitmap)imageList.Images[0];//We are moving away from these, but sometimes we don't have an icon yet.
			//toolBarButton.ToolTipText=Lan.g(this,"Edit Selected Account");
			////toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
			//_um.ToolBar_Add("toolBarMain",toolBarButton);
		}

		private void Add_Click(object sender,EventArgs e) {
			MsgBox.Show("Add");
		}

		private void Edit_Click(object sender,EventArgs e) {
			MsgBox.Show("Edit");
		}

		private void menuItemFile_Click(object sender,EventArgs e) {
			MsgBox.Show("File");
		}

		private void menuItemTop2_Click(object sender,EventArgs e) {
			MsgBox.Show("Top2");
		}

		private void menuItemTop3_Click(object sender,EventArgs e) {
			MsgBox.Show("Top3");
		}

		private void menuItemDaily_Click(object sender,EventArgs e) {
			MsgBox.Show("Daily");
		}

		private void menuItemWeekly_Click(object sender,EventArgs e) {
			MsgBox.Show("Weekly");
		}

		private void menuItemMonthly_Click(object sender,EventArgs e) {
			MsgBox.Show("Monthly");
		}

		private void menuItemQuarterly_Click(object sender,EventArgs e) {
			MsgBox.Show("Quarterly");
		}

		private void FormTestAllControls_KeyDown(object sender,KeyEventArgs e) {
			MsgBox.Show("Form");
		}

		private void Canvas_KeyDown(object sender,System.Windows.Input.KeyEventArgs e) {
			//MsgBox.Show("Canvas");
		}

		//private void FillGrid(){
		//	DataTable table = new DataTable();
		//	table.Columns.Add("LName");
		//	table.Columns.Add("FName");
		//	for(int i = 0;i<300;i++) {
		//		DataRow dataRow = table.NewRow();
		//		if(i%3==0) {
		//			dataRow[0]="LName\r\nsecond row\r\nthird row"+i.ToString();
		//		}
		//		else {
		//			dataRow[0]="LName"+i.ToString();
		//		}
		//		dataRow[1]="FName"+i.ToString();
		//		table.Rows.Add(dataRow);
		//	}
		//	_um.Grid_BeginUpdate("gridMain");
		//	_um.Grid_ColumnsClear("gridMain");
		//	OpenDental.Wui.GridColumn gridColumn = new OpenDental.Wui.GridColumn("LastName",100);
		//	_um.Grid_ColumnAdd("gridMain",gridColumn);
		//	gridColumn=new OpenDental.Wui.GridColumn("FirstName",100,System.Windows.HorizontalAlignment.Center);
		//	gridColumn.IsWidthDynamic=true;
		//	_um.Grid_ColumnAdd("gridMain",gridColumn);
		//	List<OpenDental.Wui.GridRow> listGridRows = new List<OpenDental.Wui.GridRow>();
		//	for(int i = 0;i<table.Rows.Count;i++) {
		//		OpenDental.Wui.GridRow gridRow = new OpenDental.Wui.GridRow();
		//		OpenDental.Wui.GridCell gridCell;
		//		gridCell=new OpenDental.Wui.GridCell(table.Rows[i]["LName"].ToString());
		//		if(i==2) {
		//			gridCell.Bold=true;
		//		}
		//		gridRow.Cells.Add(gridCell);
		//		gridCell=new OpenDental.Wui.GridCell(table.Rows[i]["FName"].ToString());
		//		if(i%5==0) {
		//			gridRow.ColorBackG=System.Windows.Media.Colors.Pink;
		//			gridCell.ColorBackG=System.Windows.Media.Colors.Green;
		//			gridCell.ColorText=System.Windows.Media.Colors.White;
		//		}
		//		gridRow.Cells.Add(gridCell);
		//		if(i%3==0) {
		//			gridRow.ColorLborder=System.Windows.Media.Colors.Black;
		//		}
		//		if(i==11) {
		//			gridRow.Bold=true;
		//		}
		//		gridRow.Tag=table.Rows[i];
		//		listGridRows.Add(gridRow);
		//	}
		//	_um.Grid_SetListGridRows("gridMain",listGridRows);
		//	_um.Grid_EndUpdate("gridMain");
		//}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			string str="Double clicked on row: "+e.Row.ToString()+", col: "+e.Col.ToString();
			MsgBox.Show(str);
		}

		private void butGetTag_Click(object sender,EventArgs e) {
			//DataRow dataRow=_um.Grid_GetSelectedTag<DataRow>("gridMain");
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
			//_um.SetChecked("checkBox6",false);
		}

		private void button3_Click(object sender,EventArgs e) {
			//int selectedIndex=_um.ComboBox_GetSelectedIndex("comboBox1");
			//MsgBox.Show(selectedIndex.ToString());
			//MsgBox.Show("Cancel");
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void checkBox6_Click(object sender,EventArgs e) {
			//bool? isChecked=_um.GetIsChecked("checkBox6");
			//MsgBox.Show(isChecked.ToString());
		}

		private void button1_Click_2(object sender,EventArgs e) {
			//FillGrid();
			MsgBox.Show("OK");
		}

		private void contextMenu_Opening(object sender,CancelEventArgs e) {
			//e.Cancel=true;
			//MsgBox.Show("Opening");
			//_um.ContextMenuItem_SetVisible("gridMain","contextMenu","voidPaymentToolStripMenuItem",false);
		}

		private void openPaymentToolStripMenuItem_Click(object sender,EventArgs e) {
			MsgBox.Show("Open Payment");
		}

		private void menuItemChild_Click(object sender,EventArgs e) {
			MsgBox.Show("Child");
		}

		private void voidPaymentToolStripMenuItem_Click(object sender,EventArgs e) {
			MsgBox.Show("Void Payment");
		}

		private void linkLabel1_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e) {
			MsgBox.Show("Link clicked");
		}

		private void butLaunchWin_Click(object sender,EventArgs e) {
			//OpenDental.Wui.WindowTest windowTest=new OpenDental.Wui.WindowTest();
			//windowTest.ShowDialog();
		}

		private void textBox1_TextChanged(object sender,EventArgs e) {
			MsgBox.Show("changed");
		}

		private void panel4_Click(object sender,EventArgs e) {
			MsgBox.Show("panelClicked");
		}

		private void butToggleVis_Click(object sender,EventArgs e) {
			label3.Visible=!label3.Visible;
		}
		private void butSetText_Click(object sender,EventArgs e) {
			/*
			string myText=//"Line 1\r\nLine2";
				@"{\rtf1\ansi\deff0\nouicompat{\fonttbl{\f0\fnil Microsoft Sans Serif;}}
{\*\generator Riched20 10.0.22621}\viewkind4\uc1 
\pard\f0\fs17\lang1033 Line 1 \b Bold\b0  text.\par
Line2\par
}
";
			odTextBox.Rtf=myText;*/
			textRich.SelectionStart=3;
			textRich.Focus();
		}

		private void butGetText_Click(object sender,EventArgs e) {
			//string myText=odTextBox.Rtf;
			int start=textRich.SelectionStart;
		}

		private void odTextBox_TextChanged(object sender,EventArgs e) {

		}
	}
}
