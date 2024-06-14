using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;
using Microsoft.Win32;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmTestAllControls:FrmODBase {

		public FrmTestAllControls() {
			InitializeComponent();
			Loaded+=FrmTestAllControls_Loaded;
			Shown+=FrmTestAllControls_Shown;
			PreviewKeyDown+=FrmTestAllControls_PreviewKeyDown;
			KeyDown+=FrmTestAllControls_KeyDown;
			//monthCalendar.AllowClickingTopText=false;
			warningIntegrity1.SetTypeAndVisibility(EnumWarningIntegrityType.Patient,false);
			FormClosing+=FrmODBase_FormClosing_1;
		}

		private void FrmTestAllControls_Shown(object sender,EventArgs e) {
			/*
			Thread.Sleep(500);
			progressBar.Value=30;
			DoEvents();
			Thread.Sleep(500);
			progressBar.Value=60;
			DoEvents();
			Thread.Sleep(500);
			progressBar.Value=80;*/
		}

		private void FrmTestAllControls_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter) {
				button1_Click_2(this,new EventArgs());
			}
		}

		private void FrmTestAllControls_Loaded(object sender,RoutedEventArgs e) {
			//DisableAllExcept(textBox1);
			//MessageBox.Show("Font Family: "+FontFamily.ToString());
			listBox1.Items.Clear();
			for(int i = 0;i<200;i++) {
				listBox1.Items.Add("Item"+i.ToString()+" plus some long text spilling off the side");
			}
			//listBox1.Items.AddEnums<DayOfWeek>();
			//listBox1.SetSelected(2);
			for(int i = 0;i<300;i++) {
				comboBox1.Items.Add("Item "+i.ToString());
			}
			FillGrid();
			//FillGrid2();
			LayoutMenu();
			LayoutToolBar();
			_menuItemMonthly.Visibility=Visibility.Collapsed;
			//textBox1.Focus();
			//FocusManager.SetFocusedElement(this,textBox1);
			//textBox1.SelectionStart=2;
			/*
			string txt=@"{\rtf1\ansi\ansicpg1252\uc1\htmautsp\deff2{\fonttbl{\f0\fcharset0 Times New Roman;}{\f2\fcharset0 Segoe UI;}{\f3\fcharset0 Microsoft Sans Serif;}}{\colortbl\red0\green0\blue0;\red255\green255\blue255;}\loch\hich\dbch\pard\plain\ltrpar\itap0{\lang1033\fs17\f3\cf0 \cf0\ql{\f3 {\ltrch Line 1 }{\b\ltrch Bold}{\ltrch  text.}\li0\ri0\sa0\sb0\fi0\ql\par}
{\f3 {\ltrch Line2}\li0\ri0\sa0\sb0\fi0\ql\par}
}
}";*/
			/*/string txt=@"{\rtf1\ansi\ansicpg1252\uc1\htmautsp\deff2{\fonttbl{\f0\fcharset0 Times New Roman;}{\f2\fcharset0 Segoe UI;}{\f3\fcharset0 Microsoft Sans Serif;}}{\colortbl\red0\green0\blue0;\red255\green255\blue255;}\loch\hich\dbch\pard\plain\ltrpar\itap0{\lang1033\fs17\f3\cf0 \cf0\ql{\f3 {\ltrch Line 1 }{\b\ltrch Bold}{\ltrch  text.}\li0\ri0\sa0\sb0\fi0\ql\par}
{\f3 {\ltrch Line2. This is a bunch, of additional (text with line) bre-aks and some mispel'd words. so tht I can text spellcheking and highliting. I need to add enough text for it to trigger the vertical scrollbar.}\li0\ri0\sa0\sb0\fi0\ql\par}
}
}";*/
			//textRich.Rtf=txt;
			textRich.Text="Line1\r\n Line2\r\nLine3, of additional (text with line) bre-aks and some mispel'd words.\r\n so tht I can text spellcheking and highliting. I need\r\nto add enough text for it to trigger the vertical scrollbar.";
			FillTree();
			//textBox5_Copy.Focus();
			webBrowser.Navigate("https://www.opendental.com/");
			//InitializeWebView2Async();
			Size sizeCanvas=new Size(1000,1000);
			Size sizeImage=new Size(500,800);
			//zoomSlider.SetValueInitialFit(sizeCanvas,sizeImage,0);
			textBox3.Focus();
		}

		//private async void InitializeWebView2Async() {
		//	try {
		//		await webView2.Init();
		//	}
		//	catch(Exception ex) {
		//		FriendlyException.Show("Error loading window. Run in x86 for debug.",ex);
		//		Close();
		//		return;
		//	}
		//	webView2.Navigate("https://www.opendental.com/");
		//}

		private void FillTree(){
			//TreeViewItem treeViewItem1=new TreeViewItem();
			//treeViewItem1.Text="root1";
			//treeViewItem1.Icon=EnumIcons.ImageSelectorFolder;
			//TreeViewItem treeViewItem2=new TreeViewItem();
			//treeViewItem2.Text="child2";
			//treeViewItem2.Icon=EnumIcons.ImageSelectorDoc;
			//treeViewItem1.Items.Add(treeViewItem2);
			//treeView.Items.Add(treeViewItem1);
		}

		private void FrmTestAllControls_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(e.SystemKey==Key.S && Keyboard.Modifiers==ModifierKeys.Alt) {
				button2_Click(this,new EventArgs());
			}
		}

		private void LayoutToolBar() {
			//toolBarMain.Clear();
			//toolBarMain.Add(Lans.g(this,"Add"),EnumIcons.Patient,Add_Click);
			//toolBarMain.AddSeparator();
			//ToolBarButton toolBarButton=new ToolBarButton();
			//toolBarButton.Text=Lans.g(this,"Edit");
			//toolBarButton.ToolTipText=Lans.g(this,"Edit Selected Account");
			////todo: toolBarButton.Style=ToolBarButtonStyle.DropDownButton;
			//toolBarButton.Click+=Edit_Click;
			//toolBarMain.Add(toolBarButton);
			//toolBarMain.Add(Lans.g(this,"Info"),"editPencil.gif",Info_Click);
			toolBarMain.Add(Lans.g(this,"Print"),Info_Click,EnumIcons.Print);
			toolBarMain.Add(Lans.g(this,"Delete"),Info_Click,EnumIcons.DeleteX);
			toolBarMain.Add(Lans.g(this,"Info"),Info_Click,EnumIcons.Info);
			toolBarMain.Add(Lans.g(this,"Sign"),Info_Click);
			toolBarMain.AddSeparator();
			toolBarMain.Add(Lans.g(this,"Scan:"),Info_Click);
			toolBarMain.Add(Lans.g(this,""),Info_Click,EnumIcons.ImageSelectorDoc);
			toolBarMain.Add(Lans.g(this,""),Info_Click,EnumIcons.ScanMulti);
			toolBarMain.Add(Lans.g(this,""),Info_Click,EnumIcons.ScanXray);
			toolBarMain.Add(Lans.g(this,""),Info_Click,EnumIcons.ScanPhoto);
			toolBarMain.AddSeparator();
			toolBarMain.Add(Lans.g(this,"Mount/Acquire"),Info_Click,EnumIcons.Acquire);
			toolBarMain.Add(Lans.g(this,"Video"),Info_Click,EnumIcons.Video);
			toolBarMain.AddSeparator();
			ContextMenu contextMenu = new ContextMenu(this);
			contextMenu.Add(new MenuItem("Item1",menuItem1_Click));
			contextMenu.Add(new MenuItem("Item2",menuItem2_Click));
			contextMenu.Add(new MenuItem("Item3",menuItem3_Click));
			toolBarMain.Add(Lans.g(this,"Pick File"),toolBarButtonStyle:ToolBarButtonStyle.DropDownButton,contextMenuDropDown:contextMenu);
			//ToolBarButton toolBarButton = new ToolBarButton();
			//toolBarButton.Text="Import";
			//toolBarButton.Click+=Info_Click;
			//toolBarButton.Icon=EnumIcons.Import;
			//toolBarButton.ToolBarButtonStyle=ToolBarButtonStyle.DropDownButton;
			//ContextMenu contextMenu = new ContextMenu(this);
			//contextMenu.Add(new MenuItem("Item1",menuItem1_Click));
			//contextMenu.Add(new MenuItem("Item2",menuItem2_Click));
			//contextMenu.Add(new MenuItem("Item3",menuItem3_Click));
			//toolBarButton.ContextMenuDropDown=contextMenu;
			//toolBarButton.IsEnabled=false;
			//toolBarMain.Add(toolBarButton);
			//toolBarButton=new ToolBarButton();
			//toolBarButton.Text="Export";
			//toolBarButton.Click+=Info_Click;
			//toolBarButton.Icon=EnumIcons.Export;
			//toolBarButton.ToolBarButtonStyle=ToolBarButtonStyle.DropDownButton;
			//toolBarButton.ContextMenuDropDown=contextMenu;
			//toolBarMain.Add(toolBarButton);
			//toolBarMain.Add("Copy",Info_Click,EnumIcons.Copy);
			//toolBarMain.Add("Paste",Info_Click,EnumIcons.Paste);
			toolBarMain.Add("Crop",Info_Click,EnumIcons.Crop,ToolBarButtonStyle.ToggleButton);
			toolBarMain.Add("Pan",Info_Click,EnumIcons.Hand,ToolBarButtonStyle.ToggleButton);
			toolBarMain.Add("Adj",Info_Click,EnumIcons.ArrowsAll,ToolBarButtonStyle.ToggleButton);
			toolBarMain.Add("FlipH",Info_Click,EnumIcons.Flip);
			toolBarMain.Add("-90",Info_Click,EnumIcons.RotateL);
			toolBarMain.Add("+90",Info_Click,EnumIcons.RotateR);

		}

		private void Add_Click(object sender,EventArgs e) {
			MsgBox.Show("Add");
		}

		private void menuItem1_Click(object sender,EventArgs e) {
			MsgBox.Show("1");
		}

		private void menuItem2_Click(object sender,EventArgs e) {
			MsgBox.Show("2");
		}

		private void menuItem3_Click(object sender,EventArgs e) {
			MsgBox.Show("3");
		}

		private void Edit_Click(object sender,EventArgs e) {
			MsgBox.Show("Edit");
		}

		private void Info_Click(object sender,EventArgs e) {
			//MsgBox.Show("Info");
		}

		private MenuItem _menuItemMonthly;
		
		private void LayoutMenu(){
			//File-----------------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItem("File",menuItemFile_Click));
			//Reports--------------------------------------------------------------------------------------------------------
			MenuItem menuItemReports=new MenuItem("Reports");//Use a local variable if there are children
			menuMain.Add(menuItemReports);
			menuItemReports.Add("Daily",menuItemDaily_Click);//Normal simple pattern when there are no children
			MenuItem menuItemWeekly=new MenuItem("Weekly",menuItemWeekly_Click);//Also use a local variable if you want to set more properties.
			menuItemWeekly.IsChecked=true;
			menuItemWeekly.Shortcut="W";
			menuItemReports.Add(menuItemWeekly);
			menuItemReports.AddSeparator();
			_menuItemMonthly=new MenuItem("Monthly",menuItemMonthly_Click);//Use class field when you need access later, for example to set Visibility to Collapsed.
			menuItemReports.Add(_menuItemMonthly);
			//Help-----------------------------------------------------------------------------------------------------------
			menuMain.Add("Help",menuItemHelp_Click);
		}

		private void FillGrid(){

		}

		/*
		private void FillGridOld(){
			DataTable table = new DataTable();
			table.Columns.Add("LName");
			table.Columns.Add("FName");
			for(int i = 0;i<300;i++) {
				DataRow dataRow = table.NewRow();
				if(i%3==0) {
					dataRow[0]="LName\r\nsecond row\r\nthird row"+i.ToString();
				}
				else {
					dataRow[0]="LName"+i.ToString();
				}
				dataRow[1]="FName"+i.ToString();
				table.Rows.Add(dataRow);
			}
			gridMain.ColumnsClear();
			GridColumn gridColumn = new GridColumn(Lans.g("Table...","colName"),100);
			gridMain.ColumnAdd(gridColumn);
			gridColumn=new GridColumn(Lans.g("Table...","colName"),100,HorizontalAlignment.Center);
			gridColumn.IsWidthDynamic=true;//For the last column. This is new. This was not required in WinForms.
			gridMain.ColumnAdd(gridColumn);
			List<GridRow> listGridRows = new List<GridRow>();
			//for(int i=0;i<_list.Count;i++){
			//	GridRow gridRow=new GridRow();
			//	gridRow.Cells.Add(_list[i].);
			//	gridRow.Cells.Add("");
			//	gridRow.Cells.Add(new GridCell(""){ColorBackG=.., etc});//object initializer is not allowed
			//  GridCell cell=new GridCell("");//instead of above line
			//	cell.ColorBackG=...;
			//	gridRow.Cells.Add(cell);
			//	gridRow.Cells.Add(new GridCell(_list[i].IsHidden?"X":""));
			//	gridRow.Tag=_list[i];
			//}
			//gridMain.SetListGridRows(listGridRows);
			for(int i=0;i<table.Rows.Count;i++){
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(table.Rows[i]["LName"].ToString());
				GridCell gridCell;

				gridCell=new GridCell(table.Rows[i]["FName"].ToString());
				if(i%5==0) {
					gridRow.ColorBackG=Colors.Pink;
					gridCell.ColorBackG=Colors.Green;
					gridCell.ColorText=Colors.White;
				}
				gridRow.Cells.Add(gridCell);
				if(i%3==0) {
					gridRow.ColorLborder=Colors.Black;
				}
				if(i==11) {
					gridRow.Bold=true;
				}
				gridRow.Tag=table.Rows[i];
				listGridRows.Add(gridRow);
			}
			gridMain.SetListGridRows(listGridRows);
		}*/

		//private void FillGrid2(){
		//	List<Pat> listPats=new List<Pat>();
		//	for(int i = 0;i<300;i++) {
		//		Pat pat=new Pat();
		//		pat.LName="LName"+i;
		//		pat.FName="FName"+i;
		//		listPats.Add(pat);
		//	}
		//	grid2.Columns.Clear();
		//	System.Windows.Controls.DataGridTextColumn dataGridTextColumn = new System.Windows.Controls.DataGridTextColumn();
		//	dataGridTextColumn.Header="LName";
		//	dataGridTextColumn.Binding=new Binding("LName");
		//	grid2.Columns.Add(dataGridTextColumn);
		//	dataGridTextColumn = new System.Windows.Controls.DataGridTextColumn();
		//	dataGridTextColumn.Header="FName";
		//	dataGridTextColumn.Binding=new Binding("FName");
		//	grid2.Columns.Add(dataGridTextColumn);
		//	grid2.ItemsSource=listPats;
		//}

		private class Pat{
			public string LName { get; set; }
			public string FName { get; set; }
		}

		private void menuItemFile_Click(object sender,EventArgs e) {
			MessageBox.Show("File");
		}

		private void menuItemTop2_Click(object sender,EventArgs e) {
			MsgBox.Show("Top2");
		}

		private void menuItemHelp_Click(object sender,EventArgs e) {
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

		private void linkLabel1_LinkClicked(object sender,EventArgs e) {
			MessageBox.Show("Link clicked");
		}

		private void button2_Click(object sender,EventArgs e) {
			MessageBox.Show("button2 clicked normal");
		}

		private void button2_Click2(object sender,RoutedEventArgs e) {
			MessageBox.Show("button2 clicked Routed");
		}

		private void butGetSelected_Click(object sender,EventArgs e) {
			List<int> listInts=listBox1.SelectedIndices;
			string str=string.Join(",",listInts.ToArray());
			MsgBox.Show(str);
			/*
			FrmColorDialog frmColorDialog=new FrmColorDialog();
			frmColorDialog.Color=Colors.Red;
			frmColorDialog.ShowDialog();
			if(frmColorDialog.IsDialogOK){
				Color color=frmColorDialog.Color;
			}*/
		}

		private void butGetTag_Click(object sender,EventArgs e) {
			//DataRow dataRow=_um.Grid_GetSelectedTag<DataRow>("gridMain");
		}

		private void button1_Click_2(object sender,EventArgs e) {
			//FillGrid();
			//MsgBox.Show("OK");
			string fullMsg="";
			List<string> listStrTests=new List<string>();
			listStrTests.Add("sometxt] and more");//: match="sometxt]"
			listStrTests.Add("sometxt]");//: match="sometxt]"
			listStrTests.Add("sometxt");//: match="sometxt"
			listStrTests.Add("sometxt and more");//: no match
			listStrTests.Add("sometxt and] more");//: no match
			listStrTests.Add("sometxt and more]");//: no match
			for(int i=0;i<listStrTests.Count;i++){
				fullMsg+="\""+listStrTests[i]+"\": ";
				string pattern2=@"^"//start of string
					+@"\w+"//one or more word characters (letters, digits, or underscores)
					+@"(?=\]|$)";//positive lookahead to ensure match is followed by either a closing bracket or end of string
				Regex regex2=new Regex(pattern2);
				Match match2=regex2.Match(listStrTests[i]);
				if(match2 is null || !match2.Success){
					fullMsg+="no match\r\n";
				}
				else{
					fullMsg+="match=\""+match2.Value+"\"\r\n";
				}
			}
			MsgBox.Show(fullMsg);
		}

		private void button3_Click(object sender,EventArgs e) {
			//int selectedIndex=_um.ComboBox_GetSelectedIndex("comboBox1");
			//MsgBox.Show(selectedIndex.ToString());
			//MsgBox.Show("Cancel");
			//DialogResult=DialogResult.Cancel;
			Close();
		}

		private void button4_Click(object sender,EventArgs e) {

		}

		private void butLaunchWin_Loaded(object sender,RoutedEventArgs e) {

		}

		private void label8_Loaded(object sender,RoutedEventArgs e) {

		}

		private void butLaunchWin_Loaded_1(object sender,RoutedEventArgs e) {

		}

		private void butSetLabel_Click(object sender,EventArgs e) {
			label8.Text="Line1\r\nLine2";
		}

		private void butGetLabel_Click(object sender,EventArgs e) {
			MsgBox.Show(label8.Text);
		}

		private void textBox1_TextChanged(object sender,EventArgs e) {
			//MsgBox.Show("text changed");
			//textBox1.SelectionStart=2;
		}

		private void checkBox6_Click(object sender,EventArgs e) {
			MsgBox.Show("checkbox 6 clicked");
		}

		private void panel4_Click(object sender,EventArgs e) {
			//MsgBox.Show("panelClicked");
			//checkBox1.Checked=!checkBox1.Checked;
		}

		private void butToggleVis_Click(object sender,EventArgs e) {
			//label3.Visible=!label3.Visible;
			webBrowser.IsEnabled=!webBrowser.IsEnabled;
		}

		private void FrmODBase_FormClosed(object sender,EventArgs e) {
			//MsgBox.Show("Form closed");
		}

		private void FrmODBase_FormClosing(object sender,System.ComponentModel.CancelEventArgs e) {
			//MsgBox.Show("Form closing");
			//e.Cancel=true;
		}

		private void butSelect_Click(object sender,EventArgs e) {
			//textMoreText.SelectionStart=0;
			//textMoreText.SelectionLength=textMoreText.Text.Length;
			textBox1.SelectAll();
			bool isFocused=textBox1.Focus();
		}

		private void butGetSelection_Click(object sender,EventArgs e) {
			//string myText=textRich.Rtf;
			//int start=textRich.SelectionStart;
			//textRich.ClearWavyAll();
			string str="Start:"+textRich.SelectionStart.ToString()+", Length:"+textRich.SelectionLength.ToString();
			MsgBox.Show(str);
		}

		private void butSetSelection_Click(object sender,EventArgs e) {
			//string myText=//"Line 1\r\nLine2";
				/*
				@"{\rtf1\ansi\deff0\nouicompat{\fonttbl{\f0\fnil Microsoft Sans Serif;}}
{\*\generator Riched20 10.0.22621}\viewkind4\uc1 
\pard\f0\fs17\lang1033 Line 1 \b Bold\b0  text.\par
Line2\par
}
";
			@"{\rtf1\ansi\ansicpg1252\uc1\htmautsp\deff2{\fonttbl{\f0\fcharset0 Times New Roman;}{\f2\fcharset0 Segoe UI;}{\f3\fcharset0 Microsoft Sans Serif;}}{\colortbl\red0\green0\blue0;\red255\green255\blue255;}\loch\hich\dbch\pard\plain\ltrpar\itap0{\lang1033\fs17\f3\cf0 \cf0\ql{\f3 {\ltrch Line 1 }{\b\ltrch Bold}{\ltrch  text.}\li0\ri0\sa0\sb0\fi0\ql\par}
{\f3 {\ltrch Line2}\li0\ri0\sa0\sb0\fi0\ql\par}
}
}";*/
			//textRich.Rtf=myText;
			//textRich.SelectionStart=3;
			//.SpellCheck();
			//"This\r\n is a\r\n bunch
			textRich.Select(0,5);
		}

		private void butChildren_Click(object sender,EventArgs e) {
			List<FrameworkElement> listFrameworkElements=GetAllChildControlsFlat(this);
			List<string> listStrings=new List<string>();
			for(int i=0;i<listFrameworkElements.Count;i++){
				listStrings.Add(listFrameworkElements[i].Name);
			}
			listStrings=listStrings.OrderBy(x=>x).ToList();
			string str="";
			for(int i=0;i<listStrings.Count;i++){
				str+=listStrings[i]+"\r\n";
			}
		}

		private void butLaunchWindow_Click(object sender,EventArgs e) {
			FrmTestGrid frmTestGrid=new FrmTestGrid();
			frmTestGrid.ShowDialog();
		}

		private void butSetText_Click(object sender,EventArgs e) {
			textBox3.Focus();
		}

		private void butGetText_Click(object sender,EventArgs e) {
			//string myText=odTextBox.Rtf;
			int start=textRich.SelectionStart;
		}

		private void butLaunchWin_Click(object sender,EventArgs e) {
			//OpenDental.Wui.WindowTest windowTest=new OpenDental.Wui.WindowTest();
			//windowTest.ShowDialog();
			//OpenFileDialog openFileDialog=new OpenFileDialog();
			//openFileDialog.ShowDialog();
			//openFileDialog.FileName
			//System.Windows.Forms.PrintDialog printDialog=new System.Windows.Forms.PrintDialog();
			//printDialog.ShowDialog();
			FrmTestGrid frmTestGrid=new FrmTestGrid();
			frmTestGrid.Activated+=FrmTestGrid_Activated;
			if(count==0){
				frmTestGrid.XAdj=300;
			}
			else{
				frmTestGrid.XAdj=-200;
			}
			count++;
			frmTestGrid.Show();
		}

		private int count;

		private void FrmTestGrid_Activated(object sender,EventArgs e) {
			//MsgBox.Show("Activated");
			Debug.WriteLine(DateTime.Now.ToString()+" Activated");
		}

		private void butNow_Click(object sender,EventArgs e) {
			textTime.Text=DateTime.Now.ToShortTimeString();
		}

		private void textBox3_LostFocus(object sender,RoutedEventArgs e) {
			//MsgBox.Show("Lost focus");
		}

		private void butGetPW_Click(object sender,EventArgs e) {
			MsgBox.Show(textPassword.Text);
		}

		private void FrmODBase_FormClosing_1(object sender,System.ComponentModel.CancelEventArgs e) {
			//webView2.Dispose();
		}
	}
}
