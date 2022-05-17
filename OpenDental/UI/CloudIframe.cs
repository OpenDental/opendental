using System.Windows.Forms;
using CodeBase;
using OpenDental.Thinfinity;

namespace OpenDental.UI {
	public partial class CloudIframe:UserControl {
		private string _frameId;

		public CloudIframe() {
			InitializeComponent();
			this.Visible=false;
			if(!ODBuild.IsWeb()) {
				return;
			}
			_frameId=Browser.InsertIframe(this.Handle);
		}

		public void Navigate(string url) {
			ODCloudClient.ODBrowserData odBrowserData=new ODCloudClient.ODBrowserData {
				ElementId=_frameId,
				Url=url
			};
			ODCloudClient.SendToBrowser(odBrowserData,ODCloudClient.BrowserAction.NavigateIframe);
		}

		public void DisplayFile(string filepath) {
			string url=Browser.GetSafeUrl(filepath);
			Navigate(url);
		}

		public void ShowIframe() {
			ODCloudClient.ODBrowserData odBrowserData=new ODCloudClient.ODBrowserData {
				ElementId=_frameId,
				IsVisible=true
			};
			ODCloudClient.SendToBrowser(odBrowserData,ODCloudClient.BrowserAction.SetIframeVisible);
		}

		public void HideIframe() {
			ODCloudClient.ODBrowserData odBrowserData=new ODCloudClient.ODBrowserData {
				ElementId=_frameId,
				IsVisible=false
			};
			ODCloudClient.SendToBrowser(odBrowserData,ODCloudClient.BrowserAction.SetIframeVisible);
		}
	}
}
