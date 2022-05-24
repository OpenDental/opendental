using System.Windows.Forms;
using CodeBase;
using OpenDental.Thinfinity;

namespace OpenDental.UI {
	public partial class CloudIframe:UserControl {
		private string _frameId;

		public CloudIframe() {
			InitializeComponent();
			this.Visible=false;
		}

		///<summary>Inserts the iFrame into the DOM and initializes it.</summary>
		public void Initialize(string url="") {
			_frameId=Browser.InsertIframe(this.Handle,url);
        }

		///<summary>Navigates the iFrame to the url.</summary>
		public void Navigate(string url) {
			if(_frameId.IsNullOrEmpty()) {
				Initialize(url);
				return;
			}
			ODCloudClient.ODBrowserData odBrowserData=new ODCloudClient.ODBrowserData {
				ElementId=_frameId,
				Url=url
			};
			ODCloudClient.SendToBrowser(odBrowserData,ODCloudClient.BrowserAction.NavigateIframe);
		}

		///<summary>Displays a file in the iFrame.</summary>
		public void DisplayFile(string filepath) {
			string url=Browser.GetSafeUrl(filepath);
			Navigate(url);
		}

		///<summary>Makes the iFrame visible.</summary>
		public void ShowIframe() {
			if(_frameId.IsNullOrEmpty()) {
				Initialize();
				return;
			}
			ODCloudClient.ODBrowserData odBrowserData=new ODCloudClient.ODBrowserData {
				ElementId=_frameId,
				IsVisible=true
			};
			ODCloudClient.SendToBrowser(odBrowserData,ODCloudClient.BrowserAction.SetIframeVisible);
		}

		///<summary>Hides the iFrame.</summary>
		public void HideIframe() {
			if(_frameId.IsNullOrEmpty()) {
				return;
			}
			ODCloudClient.ODBrowserData odBrowserData=new ODCloudClient.ODBrowserData {
				ElementId=_frameId,
				IsVisible=false
			};
			ODCloudClient.SendToBrowser(odBrowserData,ODCloudClient.BrowserAction.SetIframeVisible);
		}
	}
}
