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
	public partial class FormTabControlTests : FormODBase{
		private OpenDental.UI.TabPage _tabPage;

		public FormTabControlTests(){
			InitializeComponent();
			InitializeLayoutManager();
			//LayoutManager.ZoomTest=50;
		}

		private void FormTabControlTest_Load(object sender, EventArgs e){
			//System.Windows.Forms.TabPage tabPage=new System.Windows.Forms.TabPage();
			
			/*
				Panel panel=new Panel();
			panel.BorderStyle=BorderStyle.FixedSingle;
			panel.Location=new Point(71,290);//LayoutManager.Scale(71),LayoutManager.Scale(290));
			panel.Size=new Size(367,212);//LayoutManager.Scale(367),LayoutManager.Scale(212));
			OpenDental.UI.TabControl tabControl=new OpenDental.UI.TabControl();
			tabControl.Size=new Size(357,202);
			tabControl.Location=new Point(5,5);
			//first tab page
			System.Windows.Forms.TabPage tabPage=new System.Windows.Forms.TabPage();
			tabPage.Name="tabPage1";
			tabPage.Text="tabPage1";
			Label label=new Label();
			label.Text="My Label 1";
			label.Size=new Size(150,18);
			tabPage.Controls.Add(label);
			tabControl.Controls.Add(tabPage);
			//second tab page
			tabPage=new System.Windows.Forms.TabPage();
			tabPage.Name="tabPage2";
			tabPage.Text="tabPage2";
			label=new Label();
			label.Text="My Label 2";
			label.Size=new Size(150,18);
			tabPage.Controls.Add(label);
			tabControl.Controls.Add(tabPage);
			//
			panel.Controls.Add(tabControl);
			//Controls.Add(panel);
			LayoutManager.Add(panel,this);*/
		}

		private void FormTabControlTests_MouseDown(object sender,MouseEventArgs e) {
			Point point=e.Location;
		}

		private void button1_Click(object sender,EventArgs e) {
			//MsgBox.Show(tabControl2.SelectedIndex.ToString());
			tabControl2.TabPages.Clear();
		}

		private void button2_Click(object sender,EventArgs e) {
			_tabPage=new OpenDental.UI.TabPage();
			_tabPage.Name="tabPageMine";
			_tabPage.Text="MyTab";
			_tabPage.Size=new Size(100,100);
			Label label=new Label();
			label.Text="My Label";
			label.Size=new Size(150,18);
			_tabPage.Controls.Add(label);
			tabControl2.TabPages.Add(_tabPage);
		}

		private void button3_Click(object sender,EventArgs e) {
			tabControl2.TabPages.Remove(_tabPage);
		}

		private void tabControl1_Selected(object sender,TabControlEventArgs e) {
			MsgBox.Show("selected");
		}

		private void button4_Click(object sender,EventArgs e) {
			MsgBox.Show(tabPage21.Size.ToString());//366,176
		}

		private void button5_Click(object sender,EventArgs e) {
			tabControl2.TabsAreCollapsed=true;
		}
	}
}
