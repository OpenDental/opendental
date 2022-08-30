using Microsoft.CSharp;
//using Microsoft.Vsa;
using System.CodeDom.Compiler;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.UI{
	///<summary></summary>
	public class PrintPreview : System.Windows.Forms.Form{
		private System.ComponentModel.IContainer components;
		///<summary></summary>
		private int TotalPages;
		private OpenDental.UI.ODToolBar ToolBarMain;
		private System.Windows.Forms.ImageList imageListMain;
		private System.Windows.Forms.PrintPreviewControl printPreviewControl2;
		///<summary></summary>
		private PrintDocument Document;
		///<summary></summary>
		private PrintSituation Sit;

		///<summary>Must supply the printSituation so that when user clicks print, we know where to send it.  Must supply total pages</summary>
		public PrintPreview(PrintSituation sit,PrintDocument document,int totalPages){
			InitializeComponent();// Required for Windows Form Designer support
			Sit=sit;
			Document=document;
			TotalPages=totalPages;
		}

		/// <summary>Clean up any resources being used.</summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintPreview));
			this.ToolBarMain = new OpenDental.UI.ODToolBar();
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.printPreviewControl2 = new System.Windows.Forms.PrintPreviewControl();
			this.SuspendLayout();
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0,0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(831,25);
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
			// 
			// printPreviewControl2
			// 
			this.printPreviewControl2.AutoZoom = false;
			this.printPreviewControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.printPreviewControl2.Location = new System.Drawing.Point(0,0);
			this.printPreviewControl2.Name = "printPreviewControl2";
			this.printPreviewControl2.Size = new System.Drawing.Size(831,570);
			this.printPreviewControl2.TabIndex = 6;
			// 
			// PrintPreview
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(831,570);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.printPreviewControl2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "PrintPreview";
			this.ShowInTaskbar = false;
			this.Text = "Report";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormPrintPreview_Load);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FormReport_Layout);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormPrintPreview_Load(object sender, System.EventArgs e) {
			LayoutToolBar();
			if(Document.DefaultPageSettings.PrintableArea.Height==0) {
				Document.DefaultPageSettings.PaperSize=new PaperSize("default",850,1100);
			}
			//if document fits within window, then don't zoom it bigger; leave it at 100%
			if(Document.DefaultPageSettings.PaperSize.Height<printPreviewControl2.ClientSize.Height
				&& Document.DefaultPageSettings.PaperSize.Width<printPreviewControl2.ClientSize.Width)
			{
				printPreviewControl2.Zoom=1;
			}
			//if document ratio is taller than screen ratio, shrink by height.
			else if(Document.DefaultPageSettings.PaperSize.Height
				/Document.DefaultPageSettings.PaperSize.Width
				> printPreviewControl2.ClientSize.Height / printPreviewControl2.ClientSize.Width)
			{
				printPreviewControl2.Zoom=((double)printPreviewControl2.ClientSize.Height
					/(double)Document.DefaultPageSettings.PaperSize.Height);
			}
			//otherwise, shrink by width
			else{
				printPreviewControl2.Zoom=((double)printPreviewControl2.ClientSize.Width
					/(double)Document.DefaultPageSettings.PaperSize.Width);
			}
			printPreviewControl2.Document=Document;
			ToolBarMain.Buttons["PageNum"].Text=(printPreviewControl2.StartPage+1).ToString()
				+" / "+TotalPages.ToString();
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar(){
			ToolBarMain.Buttons.Clear();
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),0,"","Print"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton("",1,"Go Back One Page","Back"));
			ODToolBarButton button=new ODToolBarButton("",-1,"","PageNum");
			button.Style=ODToolBarButtonStyle.Label;
			ToolBarMain.Buttons.Add(button);
			ToolBarMain.Buttons.Add(new ODToolBarButton("",2,"Go Forward One Page","Fwd"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"Close This Window","Close"));
		}

		private void FormReport_Layout(object sender, System.Windows.Forms.LayoutEventArgs e) {
			printPreviewControl2.Width=ClientSize.Width;	
		}

		private void ToolBarMain_ButtonClick(object sender, OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			//MessageBox.Show(e.Button.Tag.ToString());
			switch(e.Button.Tag.ToString()){
				case "Print":
					OnPrint_Click();
					break;
				case "Back":
					OnBack_Click();
					break;
				case "Fwd":
					OnFwd_Click();
					break;
				case "Close":
					OnClose_Click();
					break;
			}
		}

		private void OnPrint_Click() {
			if(!PrinterL.SetPrinter(Document,Sit)){
				return;
			}
			if(Document.OriginAtMargins){
				//In the sheets framework,we had to set margins to 20 because of a bug in their preview control.
				//We now need to set it back to 0 for the actual printing.
				//Hopefully, this doesn't break anything else.
				Document.DefaultPageSettings.Margins=new Margins(0,0,0,0);
			}
			try{
				Document.Print();
			}
			catch(Exception e){
				MessageBox.Show(Lan.g(this,"Error: ")+e.Message);
			}
			DialogResult=DialogResult.OK;
		}

		private void OnClose_Click() {
			this.Close();
		}

		private void OnBack_Click(){
			if(printPreviewControl2.StartPage==0) return;
			printPreviewControl2.StartPage--;
			ToolBarMain.Buttons["PageNum"].Text=(printPreviewControl2.StartPage+1).ToString()
				+" / "+TotalPages.ToString();
			ToolBarMain.Invalidate();
		}

		private void OnFwd_Click(){
			//if(printPreviewControl2.StartPage==totalPages-1) return;
			printPreviewControl2.StartPage++;
			ToolBarMain.Buttons["PageNum"].Text=(printPreviewControl2.StartPage+1).ToString()
				+" / "+TotalPages.ToString();
			ToolBarMain.Invalidate();
		}

	
	

		

		

		

		


	}
}
