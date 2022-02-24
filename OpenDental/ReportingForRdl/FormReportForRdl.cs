using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Xml;
using fyiReporting.RDL;
//using MySql.Data.MySqlClient;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

//SEE THE BOTTOM OF THIS FILE FOR INFO ON HOW TO EDIT THE ORIGINAL RDL FILES TO MAKE THEM WORK BETTER WITH OPEN DENTAL.
//THIS WILL ALLOW US TO UPDATE VERSIONS OF RDL WITHOUT 'BREAKING' THE EXISTING FUNCTIONALITY

namespace OpenDental{
	/// <summary></summary>
	public class FormReportForRdl:FormODBase {
		private fyiReporting.RdlViewer.RdlViewer viewer;
		private OpenDental.UI.ToolBarOD ToolBarMain;
		private System.Windows.Forms.ImageList imageListMain;
		private System.Windows.Forms.ContextMenu menuScrollMode;
		private System.Windows.Forms.MenuItem menuItemContinuous;
		private System.Windows.Forms.MenuItem menuItemContinuousFacing;
		private System.Windows.Forms.MenuItem menuItemFacing;
		private System.Windows.Forms.MenuItem menuItemSinglePage;
		private System.ComponentModel.IContainer components;

		///<summary></summary>
		public FormReportForRdl()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			SourceFilePath=null;
			SourceRdlString=null;
			Lan.F(this);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReportForRdl));
			this.viewer = new fyiReporting.RdlViewer.RdlViewer();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.menuScrollMode = new System.Windows.Forms.ContextMenu();
			this.menuItemContinuous = new System.Windows.Forms.MenuItem();
			this.menuItemContinuousFacing = new System.Windows.Forms.MenuItem();
			this.menuItemFacing = new System.Windows.Forms.MenuItem();
			this.menuItemSinglePage = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// viewer
			// 
			this.viewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.viewer.Cursor = System.Windows.Forms.Cursors.Default;
			this.viewer.Folder = null;
			this.viewer.Location = new System.Drawing.Point(45,56);
			this.viewer.Name = "viewer";
			this.viewer.PageCurrent = 1;
			this.viewer.Parameters = null;
			this.viewer.ReportName = null;
			this.viewer.ScrollMode = fyiReporting.RdlViewer.ScrollModeEnum.Continuous;
			this.viewer.ShowParameterPanel = true;
			this.viewer.Size = new System.Drawing.Size(856,453);
			this.viewer.SourceFile = null;
			this.viewer.SourceRdl = null;
			this.viewer.TabIndex = 2;
			this.viewer.Text = "rdlViewer1";
			this.viewer.Zoom = 0.3662712F;
			this.viewer.ZoomMode = fyiReporting.RdlViewer.ZoomEnum.FitPage;
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0,0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(987,25);
			this.ToolBarMain.TabIndex = 5;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0,"");
			this.imageListMain.Images.SetKeyName(1,"");
			this.imageListMain.Images.SetKeyName(2,"");
			this.imageListMain.Images.SetKeyName(3,"");
			this.imageListMain.Images.SetKeyName(4,"");
			this.imageListMain.Images.SetKeyName(5,"");
			this.imageListMain.Images.SetKeyName(6,"");
			// 
			// menuScrollMode
			// 
			this.menuScrollMode.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemContinuous,
            this.menuItemContinuousFacing,
            this.menuItemFacing,
            this.menuItemSinglePage});
			// 
			// menuItemContinuous
			// 
			this.menuItemContinuous.Index = 0;
			this.menuItemContinuous.Text = "Continuous";
			this.menuItemContinuous.Click += new System.EventHandler(this.menuItemContinuous_Click);
			// 
			// menuItemContinuousFacing
			// 
			this.menuItemContinuousFacing.Index = 1;
			this.menuItemContinuousFacing.Text = "Continuous Facing";
			this.menuItemContinuousFacing.Click += new System.EventHandler(this.menuItemContinuousFacing_Click);
			// 
			// menuItemFacing
			// 
			this.menuItemFacing.Index = 2;
			this.menuItemFacing.Text = "Facing";
			this.menuItemFacing.Click += new System.EventHandler(this.menuItemFacing_Click);
			// 
			// menuItemSinglePage
			// 
			this.menuItemSinglePage.Index = 3;
			this.menuItemSinglePage.Text = "Single Page";
			this.menuItemSinglePage.Click += new System.EventHandler(this.menuItemSinglePage_Click);
			// 
			// FormReportForRdl
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(987,712);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.viewer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReportForRdl";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Report";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormRDLreport_Load);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FormReport_Layout);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormRDLreport_Load(object sender, System.EventArgs e) {
			LayoutToolBar();
			viewer.ShowParameterPanel=false;
			viewer.Rebuild();
		}

		///<summary>Either this or SourceRdlString should be set before opening the form.</summary>
		public string SourceFilePath{
			get{return viewer.SourceFile;}
			set{viewer.SourceFile=value;}
		}

		///<summary>Either this or SourceFilePath should be set before opening the form.</summary>
		public string SourceRdlString{
			get {return viewer.SourceRdl;}
			set {viewer.SourceRdl=value;}
		}

		public fyiReporting.RDL.Report RdlReport{
			get{
				return viewer.Report;
			}
		}

		private void FormReport_Layout(object sender, System.Windows.Forms.LayoutEventArgs e) {
			//the viewer dockFill does not work right, so this handles it manually:
			LayoutManager.MoveLocation(viewer,new Point(0,ToolBarMain.Bottom));
			LayoutManager.MoveSize(viewer,new Size(ClientSize.Width,ClientSize.Height-viewer.Top));
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar(){
			ToolBarMain.Buttons.Clear();
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),0,"","Print"));
//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Save PDF"),4,"Save as Adobe PDF","PDF"));
//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Export"),3,"","Export"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			//ToolBarMain.Buttons.Add(new ODToolBarButton("",1,"Go Back One Page","Back"));
			//ODToolBarButton button=new ODToolBarButton("",-1,"","PageNum");
			//button.Style=ODToolBarButtonStyle.Label;
			//ToolBarMain.Buttons.Add(button);
			//ToolBarMain.Buttons.Add(new ODToolBarButton("",2,"Go Forward One Page","Fwd"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Zoom In"),6,"","Zoom"));
			//ODToolBarButton button=new ODToolBarButton("Scroll Mode",-1,"","");
			//button.Style=ODToolBarButtonStyle.DropDownButton;
			//button.DropDownMenu=menuScrollMode;
			//ToolBarMain.Buttons.Add(button);
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"Close This Window","Close"));
			//ToolBarMain.Invalidate();
		}

		private void ToolBarMain_ButtonClick(object sender, OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			//MessageBox.Show(e.Button.Tag.ToString());
			switch(e.Button.Tag.ToString()){
				case "Print":
					Print_Click();
					break;
				case "PDF":
					PDF_Click();
					break;
				case "Export":
					Export_Click();
					break;
				case "Back":
					Back_Click();
					break;
				case "Fwd":
					Fwd_Click();
					break;
				case "Zoom":
					Zoom_Click();
					break;
				case "Close":
					Close();
					break;
			}
		}

		private void Print_Click(){//TODO: Implement ODprintout pattern
			PrintDocument pd=new PrintDocument();
			pd.DocumentName=viewer.SourceFile;
			pd.PrinterSettings.FromPage=1;
			pd.PrinterSettings.ToPage=viewer.PageCount;
			pd.PrinterSettings.MaximumPage=viewer.PageCount;
			pd.PrinterSettings.MinimumPage=1;
			pd.DefaultPageSettings.Landscape=viewer.PageWidth>viewer.PageHeight;
			pd.DefaultPageSettings.Margins=new Margins(50,50,50,50);//Half-inch all around.
			//This prevents a bug caused by some printer drivers not reporting their papersize.
			//But remember that other countries use A4 paper instead of 8 1/2 x 11.
			if(pd.DefaultPageSettings.PrintableArea.Height==0){
				pd.DefaultPageSettings.PaperSize=new PaperSize("default",850,1100);
			}
			try{
				if(PrinterL.SetPrinter(pd,PrintSituation.Default,0,"Report printed "+pd.DocumentName)){
					if(pd.PrinterSettings.PrintRange==PrintRange.Selection) {
						pd.PrinterSettings.FromPage=viewer.PageCurrent;
					}
					viewer.Print(pd);
				}
			}
			catch{
				MessageBox.Show(Lan.g(this,"Printer not available"));
			}
		}

		private void Back_Click(){
			/*viewer.  .se.PageCurrent--;
			ToolBarMain.Buttons["PageNum"].Text=(viewer.PageCurrent).ToString()
				+" / "+viewer.PageCount.ToString();
			ToolBarMain.Invalidate();*/
		}

		private void Fwd_Click(){
			/*
			//if(printPreviewControl2.StartPage==totalPages-1) return;
			viewer.PageCurrent++;
			ToolBarMain.Buttons["PageNum"].Text=(viewer.PageCurrent).ToString()
				+" / "+viewer.PageCount.ToString();
			ToolBarMain.Invalidate();*/
		}

		private void PDF_Click(){
			MessageBox.Show("Not functional yet");
		}

		private void Export_Click(){
			MessageBox.Show("Not functional yet");
			/*SaveFileDialog saveFileDialog2=new SaveFileDialog();
			saveFileDialog2.AddExtension=true;
			//saveFileDialog2.Title=Lan.g(this,"Select Folder to Save File To");
			saveFileDialog2.FileName=MyReport.ReportName+".txt";
			if(!Directory.Exists(PrefC.GetString(PrefName.ExportPath"))){
				try{
					Directory.CreateDirectory(PrefC.GetString(PrefName.ExportPath"));
					saveFileDialog2.InitialDirectory=PrefC.GetString(PrefName.ExportPath");
				}
				catch{
					//initialDirectory will be blank
				}
			}
			else{
				saveFileDialog2.InitialDirectory=PrefC.GetString(PrefName.ExportPath");
			}
			//saveFileDialog2.DefaultExt="txt";
			saveFileDialog2.Filter="Text files(*.txt)|*.txt|Excel Files(*.xls)|*.xls|All files(*.*)|*.*";
			saveFileDialog2.FilterIndex=0;
			if(saveFileDialog2.ShowDialog()!=DialogResult.OK){
				return;
			}
			try{
				using(StreamWriter sw=new StreamWriter(saveFileDialog2.FileName,false)){
					String line="";  
					for(int i=0;i<MyReport.ReportTable.Columns.Count;i++){
						line+=MyReport.ReportTable.Columns[i].Caption;
						if(i<MyReport.ReportTable.Columns.Count-1){
							line+="\t";
						}
					}
					sw.WriteLine(line);
					string cell;
					for(int i=0;i<MyReport.ReportTable.Rows.Count;i++){
						line="";
						for(int j=0;j<MyReport.ReportTable.Columns.Count;j++){
							cell=MyReport.ReportTable.Rows[i][j].ToString();
							cell=cell.Replace("\r","");
							cell=cell.Replace("\n","");
							cell=cell.Replace("\t","");
							cell=cell.Replace("\"","");
							line+=cell;
							if(j<MyReport.ReportTable.Columns.Count-1){
								line+="\t";
							}
						}
						sw.WriteLine(line);
					}
				}//using
			}
			catch{
				MessageBox.Show(Lan.g(this,"File in use by another program.  Close and try again."));
				return;
			}
			MessageBox.Show(Lan.g(this,"File created successfully"));*/
		}

		private void Zoom_Click(){
			if(viewer.ZoomMode==fyiReporting.RdlViewer.ZoomEnum.FitPage){
				//then zoom in
				viewer.ZoomMode=fyiReporting.RdlViewer.ZoomEnum.FitWidth;
				ToolBarMain.Buttons["Zoom"].Text=Lan.g(this,"Zoom Out");
				ToolBarMain.Buttons["Zoom"].ImageIndex=6;
			}
			else{
				//zoom out
				viewer.ZoomMode=fyiReporting.RdlViewer.ZoomEnum.FitPage;
				ToolBarMain.Buttons["Zoom"].Text=Lan.g(this,"Zoom In");
				ToolBarMain.Buttons["Zoom"].ImageIndex=5;
			}
			ToolBarMain.Invalidate();
		}

		private void menuItemContinuous_Click(object sender, System.EventArgs e) {
			viewer.ScrollMode=fyiReporting.RdlViewer.ScrollModeEnum.Continuous;
		}

		private void menuItemContinuousFacing_Click(object sender, System.EventArgs e) {
			viewer.ScrollMode=fyiReporting.RdlViewer.ScrollModeEnum.ContinuousFacing;
		}

		private void menuItemFacing_Click(object sender, System.EventArgs e) {
			viewer.ScrollMode=fyiReporting.RdlViewer.ScrollModeEnum.Facing;
		}

		private void menuItemSinglePage_Click(object sender, System.EventArgs e) {
			viewer.ScrollMode=fyiReporting.RdlViewer.ScrollModeEnum.SinglePage;
		}

		

		

		

		


	}
}




/*
Things to keep in mind the next time we upgrade to a new version of RDL.
These details may also need to be fine tuned to support various features that we want in OD.

Definition\Query.cs in an important class.
Query.GetData() is important where it sets the timeout.  Setting timeout is not allowed on the MySQL connector.  They might have fixed this bug.

Runtime\RdlEngineConfig.cs, must make sure there is a datasource like this:
		<DataSource>
			<DataProvider>MySQL.NET</DataProvider>
			<CodeModule>MySql.Data.dll</CodeModule>
			<ClassName>MySql.Data.MySqlClient.MySqlConnection</ClassName>
			<TableSelect>show tables</TableSelect>
			<Interface>SQL</Interface>
			<ReplaceParameters>true</ReplaceParameters>
		</DataSource>
Must also have MySQL.Data.dll in the same folder as RdlDesigner.exe.
 */
















