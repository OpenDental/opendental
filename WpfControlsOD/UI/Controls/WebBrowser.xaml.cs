using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using OpenDentBusiness;
using CodeBase;
using NHunspell;
using OpenDental;

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.

How to use the WebBrowser control:


*/
	///<summary></summary>
	public partial class WebBrowser : UserControl{

		#region Constructor
		public WebBrowser(){
			InitializeComponent();
			webBrowser.Navigated+=WebBrowser_Navigated;
		}
		#endregion Constructor

		#region Events
		[Category("OD")]
		public event NavigatedEventHandler Navigated;
		#endregion Events

		#region Properties

		#endregion Properties

		#region Methods - public
		public bool CanGoBack(){
			return webBrowser.CanGoBack;
		}

		public bool CanGoForward(){
			return webBrowser.CanGoForward;
		}

		public Uri GetUri(){
			return webBrowser.Source;
		}

		public void GoBack(){
			webBrowser.GoBack();
		}

		public void GoForward(){
			webBrowser.GoForward();
		}

		public void Navigate(string url){
			webBrowser.Navigate(url);
		}

		public void NavigateToString(string text){
			webBrowser.NavigateToString(text);
		}
		#endregion Methods - public

		#region Methods - private event handlers
		private void WebBrowser_Navigated(object sender,NavigationEventArgs e) {
			Navigated?.Invoke(sender,e);
		}
		#endregion Methods - private event handlers

		#region Methods - private
		
		#endregion Methods - private
	}
}

