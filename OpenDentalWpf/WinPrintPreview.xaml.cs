using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenDentalWpf {
	/// <summary>warning! Only use the print preview in debug.  It will crash if your mouse moves into the top toolbar.  Construct,set dlg.Owner=this, set Document, ShowDialog.</summary>
	public partial class WinPrintPreview:Window {
		///<summary></summary>
		public WinPrintPreview() {
			InitializeComponent();
		}

		public IDocumentPaginatorSource Document{
			get {
				return documentViewer.Document;
			}
			set { 
				documentViewer.Document=value; 
			}
		}



	}
}
