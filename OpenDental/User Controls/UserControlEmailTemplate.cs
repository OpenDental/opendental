using System;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class UserControlEmailTemplate:UserControl {

		public UserControlEmailTemplate() {
			InitializeComponent();
		}

		public void RefreshView(string plainText,string htmlText,EmailType emailType) {
			textboxPlainText.Text=plainText;
			if(string.IsNullOrEmpty(htmlText)) {
				labelNoHtml.Visible=true;
				labelHtml.Visible=false;
				webBrowserEmail.Visible=false;
				tableLayoutPanel1.ColumnStyles[1].Width=0;
				
			}
			else {
				labelNoHtml.Visible=false;
				labelHtml.Visible=true;
				tableLayoutPanel1.ColumnStyles[1].Width=50;//50%
				webBrowserEmail.Visible=true;
				string xhtml=htmlText;
				if(emailType==EmailType.Html) {
					//This might not work for images, we should consider blocking them or warning them about sending if we detect images
					try {
						xhtml=MarkupEdit.TranslateToXhtml(htmlText,true,false,true);
					}
					catch(Exception ex) {
						ex.DoNothing();
					}
				}
				webBrowserEmail.DocumentText=xhtml;
			}
		}


	}
}
