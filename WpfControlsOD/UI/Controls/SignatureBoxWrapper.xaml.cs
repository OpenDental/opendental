using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDental.UI;
using OpenDentBusiness;

namespace WpfControls.UI {
	public partial class SignatureBoxWrapper:UserControl {
		public OpenDental.UI.SignatureBoxWrapper signatureBoxWrapper;
		//private Window window;

		///<summary>A WPF/Xaml wrapper for the Winforms signature box wrapper</summary>
		public SignatureBoxWrapper() {
			//System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
			InitializeComponent();
			signatureBoxWrapper=new OpenDental.UI.SignatureBoxWrapper();
			windowsFormsHost.Child = signatureBoxWrapper;
			Loaded+=SignatureBoxWrapper_Loaded;
			//System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
			//window.Content=this;
			//ElementHost.EnableModelessKeyboardInterop(window);
		}

		public event EventHandler SignatureChanged = null;

		private void SignatureBoxWrapper_Loaded(object sender,RoutedEventArgs e) {
			//Events need to be here since if they are in the constructor, the event subscription in the hosting form of this control will fire after this one does (therefore making this event null). EX: The subscription event here will fire, with the event being null, then the one in FrmCommItem will fire, which will result in this event still being null.
			signatureBoxWrapper.SignatureChanged+=SignatureChanged;
			///Have tried and failed:
			//ElementHost.EnableModelessKeyboardInterop();//No window to pass in
			//Window.GetWindow();//Returns null whether passing in this, signatureBoxWrapper, windowsFormsHost, and can't cast the windowsFormsHost either.
			//Window.GetWindow((Window)windowsFormsHost);
			//System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();//Doesn't seem to work
			///Creating a window and setting its content to this, or to the windowsFormsHost, and it doesn't work either.
			//window.Content=this;
			//ElementHost.EnableModelessKeyboardInterop(window);
			///Key up/down events also don't work either.
			
		}

		public void FillSignature(bool sigIsTopaz,string keyData,string signature) {
			signatureBoxWrapper.FillSignature(sigIsTopaz, keyData, signature);
		}

		public string GetSignature(string keyData) {
			return signatureBoxWrapper.GetSignature(keyData);
		}

		public bool GetSigIsTopaz() {
			return signatureBoxWrapper.GetSigIsTopaz();
		}

		public void ClearSignature() {
			signatureBoxWrapper.ClearSignature();
		}

		public void SetTabletState(int v) {
			signatureBoxWrapper.SetTabletState(v);
		}

	}
}