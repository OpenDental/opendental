using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmExternalLink:FrmODBase {
		public string URL;
		public string DisplayText;

		///<summary>Pass in values that will override the title of the form or the label text next to the corresponding text boxes.</summary>
		public FrmExternalLink(string title="",string url="",string displayText="") {
			InitializeComponent();
			PreviewKeyDown+=FrmExternalLink_PreviewKeyDown;
			Lang.F(this);
			if(!string.IsNullOrEmpty(title)) {
				this.Text=title;
			}
			if(!string.IsNullOrEmpty(url)) {
				labelUrl.Text=url;
			}
			if(!string.IsNullOrEmpty(displayText)) {
				labelDisplayText.Text=displayText;
			}
		}

		private void FrmExternalLink_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			URL=textURL.Text;
			DisplayText=textDisplay.Text;
			IsDialogOK=true;
		}

		private void butEmptyLink_Click(object sender,EventArgs e) {
			URL="";
			DisplayText="";
			IsDialogOK=true;
		}

	}
}