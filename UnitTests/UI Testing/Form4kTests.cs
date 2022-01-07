using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental;
using OpenDental.UI;
using OpenDentBusiness;

namespace UnitTests{
	public partial class Form4kTests : FormODBase{
		private List<Patient> _listPatients;
		private MenuItemOD _menuItemMonthly;
		private Image _imgToothChart;
		private TabPage _tabPage;
		private Label labelOnTab;

		public Form4kTests(){
			InitializeComponent();
			InitializeLayoutManager();
			//LayoutManager.ZoomTest=20;//10,20,30 fail
			labelOnTab=new Label();
			labelOnTab.Location = new System.Drawing.Point(3, 5);
			labelOnTab.Name = "labelOnTab";
			labelOnTab.Size = new System.Drawing.Size(70, 15);
			labelOnTab.Text = "labelOnTab";
			_tabPage=new TabPage();
			_tabPage.Controls.Add(labelOnTab);
			_tabPage.Location = new System.Drawing.Point(4, 22);
			_tabPage.Name = "_tabPage";
			_tabPage.Padding = new System.Windows.Forms.Padding(3);
			_tabPage.Size = new System.Drawing.Size(490, 85);
			_tabPage.TabIndex = 0;
			_tabPage.Text = "_tabPage";
			_tabPage.UseVisualStyleBackColor = true;
			//richTextBox1.LanguageOption=RichTextBoxLanguageOptions.UIFonts;//didn't work
			//richTextBox1.scal
		}

		//protected override bool ScaleChildren => false;
		//This is not useful.  On a form, it would prevent scaling of nested controls.  On a Control, it also makes no sense.  We want to scale everything.
		//The trick is that the math for layout might make assumptions about things not getting scaled.  Just need more explicit commands in places.

		//protected override void ScaleControl(SizeF factor, BoundsSpecified specified){
			//base.ScaleControl(factor, specified);
			//don't scale
			//Not useful because we always want to scale.
		//}

		private void FormGridTest_Load(object sender, EventArgs e){
			LayoutMenu();
			LayoutToolBar();
			FillPatients();
			FillGrid();
			//FormGridTest formGridTest=new FormGridTest();
			//formGridTest.ShowDialog();
			//for(int i=0;i<200;i++){
			//	comboBoxMulti1.Items.Add(i.ToString());
			//}
			//comboBoxMulti1.SetSelected(100,true);
		}

		private void LayoutMenu(){
			menuMain.BeginUpdate();
			//File-----------------------------------------------------------------------------------------------------------
			menuMain.Add(new MenuItemOD("File",menuItemFile_Click));
			//Reports--------------------------------------------------------------------------------------------------------
			MenuItemOD menuItemReports=new MenuItemOD("Reports");//Use a local variable if there are children
			menuMain.Add(menuItemReports);
			menuItemReports.Add("Daily",menuItemDaily_Click);//Normal simple pattern when there are no children
			MenuItemOD menuItemWeekly=new MenuItemOD("Weekly",menuItemWeekly_Click);//Also use a local variable if you want to set more properties.
			//menuItemWeekly.Checked=true;
			menuItemReports.Add(menuItemWeekly);
			menuItemReports.AddSeparator();
			_menuItemMonthly=new MenuItemOD("Monthly but really long what then",menuItemMonthly_Click);//Use class field when you need access later, for example to set Available(the alternative to Visible)
			menuItemReports.Add(_menuItemMonthly);
			_menuItemMonthly.Add("More1",null);
			_menuItemMonthly.Add("More2",null);


			MenuItemOD menuItemMore=new MenuItemOD("More");
			menuItemReports.Add(menuItemMore);
			menuItemMore.Add("More1",null);
			menuItemMore.Add("More2",null);
			//Help-----------------------------------------------------------------------------------------------------------
			menuMain.Add("Help",menuItemHelp_Click);
			menuMain.EndUpdate();
		}

		private void menuItemFile_Click(object sender, EventArgs e) {
      MsgBox.Show("File");
    }

		private void menuItemDaily_Click(object sender, EventArgs e) {
      MsgBox.Show("Daily");
    }

		private void menuItemWeekly_Click(object sender, EventArgs e) {
      MsgBox.Show("Weekly");
    }

		private void menuItemMonthly_Click(object sender, EventArgs e) {
      MsgBox.Show("Monthly");
    }

		private void menuItemHelp_Click(object sender, EventArgs e) {
      MsgBox.Show("Help");
    }

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar() {
			ToolBarMain.Buttons.Clear();
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Add"),0,"","Add"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Edit"),1,Lan.g(this,"Edit Selected Account"),"Edit"));
			/*ODToolBarButton button=new ODToolBarButton("",-1,"","PageNum");
			button.Style=ODToolBarButtonStyle.Label;
			ToolBarMain.Buttons.Add(button);
			ToolBarMain.Buttons.Add(new ODToolBarButton("",2,"Go Forward One Page","Fwd"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));*/
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Export"),2,Lan.g(this,"Export the Chart of Accounts"),"Export"){Enabled=false });
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"Close This Window","Close"));
		}

		private void FillPatients(){
			_listPatients=new List<Patient>();
			Patient patient;
			for(int i=0;i<200;i++){
				patient=new Patient();
				patient.LName="LNamg"+i.ToString();
				patient.FName="FNamj"+i.ToString();
				_listPatients.Add(patient);
			}

		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("LNamg plus\r\nsome other stuff",80){IsWidthDynamic=true};
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("FNamj",80){IsWidthDynamic=true};
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("MidName",80,HorizontalAlignment.Right);//{IsWidthDynamic=true};
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Empty",100);//{IsEditable=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.NoteSpanStop=1;
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listPatients.Count;i++){
				row=new GridRow();
				if(i==2){
					row.Cells.Add(new GridCell("click a really long button"){ColorBackG=Color.LightGray,ClickEvent=DeleteClick });
				}
				else{
					row.Cells.Add(_listPatients[i].LName+" "+_listPatients[i].FName+" "+_listPatients[i].FName);
				}
				if(i==15){
					row.Cells.Add(new GridCell(_listPatients[i].FName){ ColorBackG=Color.Yellow });
				}
				else{
					row.Cells.Add(_listPatients[i].FName);
				}
				row.Cells.Add("MiddleName");
				if(i==5){
					row.DropDownParent=gridMain.ListGridRows[4];
				}
				if(i==6){
					row.DropDownParent=gridMain.ListGridRows[4];
				}
				if(i==7){
					row.DropDownParent=gridMain.ListGridRows[4];
				}
				if(i==8){
					row.DropDownParent=gridMain.ListGridRows[7];
				}
				if(i==10){
					row.Note="Some note plus some more really long text to make the note wrap.";
				}
				row.Cells.Add("empty");
				gridMain.ListGridRows.Add(row);

			}
			gridMain.EndUpdate();
		}

		private void DeleteClick(object sender,EventArgs e) {
			MsgBox.Show("Clicked");
		}

		private void button1_Click_1(object sender, EventArgs e){
			Sparks3D.ToothChart toothChart=new Sparks3D.ToothChart(false);
			toothChart.ResetTeeth();
			toothChart.EndUpdate();
			Image imgToothChart=toothChart.GetBitmap();
			imgToothChart.Dispose();
			toothChart.Dispose();
			//_imgToothChart?.Dispose();
			//_imgToothChart=imgToothChart;

			//List<Procedure> listProcs=new List<Procedure>();
			//long patNum=293;
			//Image imgToothChart=OpenDental.SheetPrinting.GetToothChartHelper(patNum,true,null,listProcs,isInPatientDashboard:true);
			//_imgToothChart?.Dispose();
			//_imgToothChart=imgToothChart;



			//menuMain.EndUpdate();
			//CodeBase.ODMessageBox.Show(menuStrip.Height.ToString());
			//FormSandboxJordan formSandboxJordan=new FormSandboxJordan();
			//formSandboxJordan.Left-=200;
			//formSandboxJordan.LocationSetManually=true;
			//formSandboxJordan.ShowDialog();
			//FormMainContainer formMainContainer=new FormMainContainer();
			//formMainContainer.Show();
			/*
			Font font=new Font(FontFamily.GenericSansSerif,8.5f);
			Font fontBold=new Font(FontFamily.GenericSansSerif,8.5f,FontStyle.Bold);
			Graphics g=this.CreateGraphics();
			string str="Some text";//\r\n\r\nO"
			float h1=g.MeasureString(str,font,20).Height;
			float h2=g.MeasureString(str,fontBold,20).Height;*/
			//Dpi.SetOld();
			//FormComboTests formComboTests=new FormComboTests();
			//Dpi.SetDefault();
			//formComboTests.ShowDialog();
		}

		#region TestResults for specific controls
		//splitContainer 
		//Spent hours fighting with it.  
		//Specific situation: ScaleChildren => false; Launch at 96 dpi, drag to 150% screen. 
		//Trying to make height and width change through SizeChanged, etc. I tried all 7 likely event handlers.
		//It responds only intermittently. For example, if only change width, it works. But if change both width and height it fails.
		//Tried many different situations. This user reported nearly my identical situation:
		//https://stackoverflow.com/questions/31682487/winforms-deep-nested-controls-resize-kernel-bug-splitter-panels-not-scaling-co
		//This will not be a problem for us because we will instead use ScaleChildren true.
		//Tested again the next day, but with ScaleChildren true.  Worked fine.
		#endregion TestResults for specific controls

		#region Recommended Patterns
		//This is all very old and outdated, but I'm keeping it.
		//All the patterns below assume dpi awareness is turned on in config file and manifest.
		//MS performs scaling as follows:
		//1. MS Scales Form
		//2. This causes events to be fired such as Resize/SizeChanged
		//3. MS then scales all the children on the form in one pass, including nested ones.
		//The obvious problem is that any manual layout that you want to do in Resize/SizeChanged, will get subsequently scaled.  This is bad.
		//The solution is to call the layout commands in Resize/SizeChanged again, but from a different event handler: ResizeEnd.

		//Testing sequence;
		//Turn on dpi awareness in config file by uncommenting the line at the bottom.
		//All development must be done in VS 96dpi environment.
		//Launch application in 96dpi, then drag to 150% screen, for example.  Everything should resize automatically.
		//Pay attention to the difference between the size of each control versus how that control draws.  They are separate issues.
		//If the size or positions of a control is wrong, then it's a problem with how the parent did the layout math.
		//If the contents of a control misdraw, then it's a problem with how that control is internally drawing itself.
		//To layout controls properly, any form that also has any Resize/SizeChanged logic needs another change:
		//Move the Resize/SizeChanged code into a separate method such as LayoutControls or similar.
		//Call LayoutControls from Resize/SizeChanged to take care of ordinary resizing logic.
		//Also call LayoutControls from ResizeEnd to handle dpi changes.
		//Resizing and layout logic called from ResizeEnd will fire on the mouse up, after a dragging a form.		

		//Scaling custom controls:
		//The starting point is to just review and improve any section that performs layout, such as Resize/SizeChanged.
		//It's currently unclear if more would ever be needed.
		//It's also unclear how and when the scaling of nested controls works.
		#endregion Recommended Patterns

		private void Form4kTest_SizeChanged(object sender, EventArgs e){
			
		}

		private void Form4kTest_Resize(object sender, EventArgs e){
			//int width=ClientSize.Width;
			//button3.Width=ClientSize.Width-button3.Left-100;
			//button3.Height=ClientSize.Height-button3.Top-100;
			//gridMain.Width=ClientSize.Width-gridMain.Left-15;
			//LayoutControls();
			//Invalidate();
		}

		private void Form4kTest_DpiChanged(object sender, DpiChangedEventArgs e){
			//PerformAutoScale();//didn't work.
			//Bounds=e.SuggestedRectangle;
			//float factor=(float)e.DeviceDpiNew/e.DeviceDpiOld;
			//SizeF sizeFactor=new SizeF(factor,factor);
			//foreach(Control control in this.Controls){
				//control.Scale(sizeFactor);//didn't work. Controls scaled, but not their text.
			//}
			//Form4kTest_SizeChanged(this,new EventArgs());
			//e.Cancel=true;
		}

		private void Form4kTest_DpiChangedAfterParent(object sender, EventArgs e){
			//this is inherited from Control and is not useful here
		}

		private void Form4kTest_DpiChangedBeforeParent(object sender, EventArgs e){
			//this is inherited from Control and is not useful here
		}

		private void Form4kTest_Layout(object sender, LayoutEventArgs e){
			
		}

		private void Form4kTest_ResizeEnd(object sender, EventArgs e){
			//Fires on the mouse up after a resize, so it does not handle smooth drags.
			//Also fires after simply moving a form, also on the mouse up.
			//Does not fire when programmatically changing form size
			//So this event is interesting because if we drag a form to a new screen, this event happens after the auto scaling and you can see the effect on the mouse up.
			//Resize/SizeChanged first messes things up, and then this fixes them.
			//LayoutControls();
		}	

		private void LayoutControls(){
			//button3.Width=ClientSize.Width-button3.Left-100;
			//splitContainer1.Width=ClientSize.Width-splitContainer1.Left-100;
			//splitContainer1.Height=ClientSize.Height-splitContainer1.Top-100;
		}

		private void menuItemSetup_Click(object sender, EventArgs e)
		{
			MsgBox.Show("Setup");
		}

		private void menu4ToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void Form4kTest_Paint(object sender, PaintEventArgs e)
		{
			//e.Graphics.FillRectangle(new SolidBrush(Color.Blue),this.ClientRectangle);
			//e.Graphics.FillRectangle(Brushes.Black,new Rectangle(Width-100,400,99,99));
		}

		private void FormClaimEdit_CloseXClicked(object sender, CancelEventArgs e){
			//if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Changes will be lost.  Continue anyway?")){
			//	e.Cancel=true;
			//}
		}

		private void button4_Click(object sender, EventArgs e){
			listBox1.Height=78;//no change
			listBox1.Anchor=AnchorStyles.Left|AnchorStyles.Top|AnchorStyles.Bottom;
			LayoutManager.Synch(listBox1);

			//string str="PanelClient: "+PanelClient.Size.ToString()+"\r\n"
			//	+"butDelete: "+butDelete.Location.ToString()+"\r\n"
			//	+"ratio:"+LayoutManager.ScaleMy.ToString();
			//MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(str);
			//msgBoxCopyPaste.Show();
		}

		private void button5_Click(object sender, EventArgs e){
			//MsgBox.Show(listBox1.Top.ToString());
			OpenDental.UI.Button button=new OpenDental.UI.Button();
			button.Text="New";
			button.Anchor=AnchorStyles.Right | AnchorStyles.Bottom;
			button.Location=new Point(button5.Right,button5.Bottom);
			button.Size=button5.Size;
			button.Font=Font;
			LayoutManager.Add(button,panel4);
			//panel4.Controls.Add(button);
		}

		private void butDelete_LocationChanged(object sender, EventArgs e){
			//Debug.WriteLine(butDelete.Location.ToString()+" Dpi:"+this.DeviceDpi.ToString()+" FormSize:"+this.Size.ToString());
		}

		private void butTab_Click(object sender, EventArgs e){
			LayoutManager.Add(_tabPage,tabControl1);
		}

		private void butTabRemove_Click(object sender, EventArgs e){
			LayoutManager.Remove(_tabPage);
		}

		private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e){
			if(!HasShown){
				return;
			}
			LayoutManager.LayoutControlBoundsAndFonts(splitContainer);
		}

		private void panel4_Paint(object sender, PaintEventArgs e){

		}

		private void button3_Click(object sender, EventArgs e){

		}

		private void butMove_Click(object sender, EventArgs e){
			LayoutManager.MoveWidth(splitContainer,butMove.Left-splitContainer.Left-5);
		}

		private void gridOD1_LocationChanged(object sender, EventArgs e){
			Point loc=gridOD1.Location;
		}

		private void butFont_Click(object sender, EventArgs e){
			richTextBox1.Font.ToString();
		}

		private void butMaximized_Click(object sender,EventArgs e) {
			FormButtonTest formButtonTest=new FormButtonTest();
			formButtonTest.Show();
		}



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








	}
}

//https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setthreaddpiawarenesscontext

