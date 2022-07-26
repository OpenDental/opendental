using CodeBase;
using OpenDental.UI;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Text;

namespace OpenDental {

	public partial class FormWebBrowserPrefs:FormODBase {
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		public string HtmlContent;
		public Point PointStart=new Point(0,0);
		public Size SizeWindow=new Size(0,0);

		///<summary>Used when opening additional details within FormPreferences.  This form will also allow links clicked and opened within this form.</summary>
		public FormWebBrowserPrefs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormWebBrowserPrefs_Load(object sender,EventArgs e) {
			if(!string.IsNullOrEmpty(HtmlContent)) {
				int fontSize=LayoutManager.Scale(11);//Font in pixels that matches our current font size.
				string html=HtmlContent;//The content passed in.
				string documentText="<p style='font-family:Microsoft Sans Serif;font-size:"+fontSize+"px'>"+html+"</p>";
				browser.DocumentText=documentText;
			}
			if(SizeWindow!=new Size(0,0)) {
				Size=new Size(LayoutManager.Scale(SizeWindow.Width),LayoutManager.Scale(SizeWindow.Height));
			}
			if(PointStart==new Point(0,0)) {
				CenterToScreen();
			}
			else {
				Location=PointStart;
			}
		}
	}
}