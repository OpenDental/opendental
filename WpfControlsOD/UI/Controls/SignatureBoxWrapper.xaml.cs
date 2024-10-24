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
/*
Jordan is the only one allowed to edit this file.

How to use the SignatureBoxWrapper control:
	-Size must always 362,79 or scaled proportionally and tested.
	-Read instructions at the top of OpenDentBusiness\UI\SignatureBoxWrapper.cs
	-You must have the following lines in your Frm Load event:
			float scale=(float)VisualTreeHelper.GetDpi(this).DpiScaleX;
			signatureBoxWrapper.SetScaleAndZoom(scale,GetZoom());
*/
	///<summary>Read instructions at the top of OpenDentBusiness\UI\SignatureBoxWrapper.cs</summary>
	public partial class SignatureBoxWrapper:UserControl {
		///<summary>The namespace on this field is misleading. This is the winforms wrapper that's over in OpenDentBusiness.</summary>
		public OpenDental.UI.SignatureBoxWrapper signatureBoxWrapper;
		//private Window window;

		///<summary>A WPF/Xaml wrapper for the Winforms signature box wrapper</summary>
		public SignatureBoxWrapper() {
			//System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
			InitializeComponent();
			signatureBoxWrapper=new OpenDental.UI.SignatureBoxWrapper();
			windowsFormsHost.Child = signatureBoxWrapper;
			Loaded+=this_Loaded;
			//System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
			//window.Content=this;
			//ElementHost.EnableModelessKeyboardInterop(window);
		}

		public event EventHandler SignatureChanged = null;

		private void this_Loaded(object sender,RoutedEventArgs e) {
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

		///<summary>keyData should not be hashed.</summary>
		public void FillSignature(bool sigIsTopaz,string keyData,string signature) {
			signatureBoxWrapper.FillSignature(sigIsTopaz, keyData, signature);
		}

		///<summary>Gets a bitmap representation of the entire control. Used in printing.</summary>
		public BitmapImage GetBitmapImage(){
			System.Drawing.Bitmap bitmap=signatureBoxWrapper.GetSigImage();
			using System.Drawing.Graphics g=System.Drawing.Graphics.FromImage(bitmap);
			g.DrawRectangle(System.Drawing.Pens.SlateGray,0,0,bitmap.Width-1,bitmap.Height-1);
			BitmapImage bitmapImage=OpenDental.Drawing.Graphics.ConvertBitmapToWpf(bitmap);
			g?.Dispose();
			bitmap?.Dispose();
			return bitmapImage;
		}

		///<summary>This can be used to determine whether the signature has changed since the control was created.  Or you can use the SignatureChanged event to track changes.</summary>
		public bool GetSigChanged(){
			return signatureBoxWrapper.GetSigChanged();
		}

		///<summary>This should NOT be used unless GetSigChanged returns true. keyData should not be hashed.</summary>
		public string GetSignature(string keyData) {
			return signatureBoxWrapper.GetSignature(keyData);
		}

		///<summary>This should NOT be used unless GetSigChanged returns true.</summary>
		public bool GetSigIsTopaz() {
			return signatureBoxWrapper.GetSigIsTopaz();
		}

		///<summary>If this is called externally, then the event SignatureChanged will also fire.</summary>
		public void ClearSignature() {
			signatureBoxWrapper.ClearSignature();
		}

		///<summary>Set to 1 to activate it to start accepting signatures.  Set to 0 to no longer accept input.  Should be called with a state of '0' in FormClosing events.</summary>
		public void SetTabletState(int v) {
			signatureBoxWrapper.SetTabletState(v);
		}

		public void SetScaleAndZoom(float scaleMS,float zoomLocal){
			signatureBoxWrapper.SetScaleAndZoom(scaleMS,zoomLocal);
		}

	}
}