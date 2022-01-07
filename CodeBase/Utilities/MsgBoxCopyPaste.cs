using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CodeBase{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class MsgBoxCopyPaste:System.Windows.Forms.Form {
		private Button butOK;
		public TextBox textMain;
		private Button butPrint;//this way, the text will be available on close.
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Button butCopyAll;
		protected int pagesPrinted;

		///<summary>This presents a message box to the user, but is better because it allows us to copy the text and paste it into another program for testing.  Especially useful for queries.</summary>
		public MsgBoxCopyPaste(string displayText)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//Lan.F(this);
			textMain.Text=displayText;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MsgBoxCopyPaste));
			this.butOK = new System.Windows.Forms.Button();
			this.textMain = new System.Windows.Forms.TextBox();
			this.butPrint = new System.Windows.Forms.Button();
			this.butCopyAll = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(615,606);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textMain
			// 
			this.textMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMain.BackColor = System.Drawing.SystemColors.Window;
			this.textMain.Font = new System.Drawing.Font("Courier New",8.25F,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,((byte)(0)));
			this.textMain.Location = new System.Drawing.Point(12,12);
			this.textMain.Multiline = true;
			this.textMain.Name = "textMain";
			this.textMain.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textMain.Size = new System.Drawing.Size(678,588);
			this.textMain.TabIndex = 2;
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = ((System.Drawing.Image)(resources.GetObject("butPrint.Image")));
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(530,606);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(79,26);
			this.butPrint.TabIndex = 3;
			this.butPrint.Text = "    &Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butCopyAll
			// 
			this.butCopyAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCopyAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopyAll.Location = new System.Drawing.Point(126,606);
			this.butCopyAll.Name = "butCopyAll";
			this.butCopyAll.Size = new System.Drawing.Size(79,26);
			this.butCopyAll.TabIndex = 4;
			this.butCopyAll.Text = "Copy All";
			this.butCopyAll.Click += new System.EventHandler(this.butCopyAll_Click);
			// 
			// MsgBoxCopyPaste
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(702,644);
			this.Controls.Add(this.butCopyAll);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.textMain);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.Name = "MsgBoxCopyPaste";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Load += new System.EventHandler(this.MsgBoxCopyPaste_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		public void NormalizeContent() {
			textMain.Text=Regex.Replace(textMain.Text,@"\r\n|\n|\r","\r\n");
		}

		private void MsgBoxCopyPaste_Load(object sender,EventArgs e) {

		}

		protected virtual void butPrint_Click(object sender,EventArgs e) {//TODO: Implement ODprintout pattern
			PrintDocument pd=new PrintDocument();
			pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
			pd.DefaultPageSettings.Margins=new Margins(50,50,50,50);//Half-inch all around.
			//This prevents a bug caused by some printer drivers not reporting their papersize.
			//But remember that other countries use A4 paper instead of 8 1/2 x 11.
			if(pd.DefaultPageSettings.PrintableArea.Height==0){
				pd.DefaultPageSettings.PaperSize=new PaperSize("default",850,1100);
			}
			pd.PrinterSettings.Duplex=Duplex.Horizontal;//Print double sided when possible.
			pagesPrinted=0;
			pd.Print();//No print previews, since this form is in and of itself a print preview.
		}

		///<summary>Called for each page to be printed.</summary>
		protected void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e){
			e.HasMorePages=!Print(e.Graphics,pagesPrinted++,e.MarginBounds);
		}

		///<summary>Prints one page. Returns true if pageToPrint is the last page in this print job.</summary>
		private bool Print(Graphics g,int pageToPrint,Rectangle margins){
			//Messages may span multiple pages. We print the header on each page as well as the page number.
			float baseY=margins.Top;
			string text="Page "+(pageToPrint+1);
			Font font=Font;
			SizeF textSize=g.MeasureString(text,font);
			g.DrawString(text,font,Brushes.Black,margins.Right-textSize.Width,baseY);
			baseY+=textSize.Height;
			text=Text;
			font=new Font(Font.FontFamily,16,FontStyle.Bold);
			textSize=g.MeasureString(text,font);
			g.DrawString(text,font,Brushes.Black,(margins.Width-textSize.Width)/2,baseY);
			baseY+=textSize.Height;
			font.Dispose();
			font=new Font(Font.FontFamily,14,FontStyle.Bold);
			text=DateTime.Now.ToString();
			textSize=g.MeasureString(text,font);
			g.DrawString(text,font,Brushes.Black,(margins.Width-textSize.Width)/2,baseY);
			baseY+=textSize.Height;
			font.Dispose();
			string[] messageLines=textMain.Text.Split(new string[] {Environment.NewLine},StringSplitOptions.None);
			font=Font;
			bool isLastPage=false;
			float y=0;
			for(int curPage=0,msgLine=0;curPage<=pageToPrint;curPage++){
				//Set y to its initial value for the current page (right after the header).
				y=curPage*(margins.Bottom-baseY);
				while(msgLine<messageLines.Length){
					//If a line is blank, we need to make sure that is counts for some vertical space.
					if(messageLines[msgLine]==""){
						textSize=g.MeasureString("A",font);
					}
					else{
						textSize=g.MeasureString(messageLines[msgLine],font);
					}
					//Would the current text line go past the bottom margin?
					if(y+textSize.Height>(curPage+1)*(margins.Bottom-baseY)){
						break;
					}
					if(curPage==pageToPrint){
						g.DrawString(messageLines[msgLine],font,Brushes.Black,margins.Left,baseY+y-curPage*(margins.Bottom-baseY));
						if(msgLine==messageLines.Length-1){
							isLastPage=true;
						}
					}
					y+=textSize.Height;
					msgLine++;
				}
			}
			return isLastPage;
		}

		public void SetButOKText(string buttonText) {
			butOK.Text=buttonText;
		}

		private void butCopyAll_Click(object sender,EventArgs e) {
			try {
				ODClipboard.SetClipboard(textMain.Text);
			}
			catch(Exception ex) {
				MessageBox.Show("Could not copy contents to the clipboard. Please try again.");
				ex.DoNothing();
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();//have to have this also because sometimes this box is non-modal.
		}
		


	}
}





















