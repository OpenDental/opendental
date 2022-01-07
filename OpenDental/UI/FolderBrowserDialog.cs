using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>This class purposely obscures System.Windows.Forms.FolderBrowserDialog. We want to give it different behavior in web mode.</summary>
	public class FolderBrowserDialog:IDisposable {

		private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;

		public FolderBrowserDialog() {
			_folderBrowserDialog=new System.Windows.Forms.FolderBrowserDialog();
		}

		// Summary:
		//     Gets or sets the descriptive text displayed above the tree view control in the
		//     dialog box.
		//
		// Returns:
		//     The description to display. The default is an empty string ("").
		public string Description {
			get {
				return _folderBrowserDialog.Description;
			}
			set {
				_folderBrowserDialog.Description=value;
			}
		}

		//
		// Summary:
		//     Gets or sets the path selected by the user.
		//
		// Returns:
		//     The path of the folder first selected in the dialog box or the last folder selected
		//     by the user. The default is an empty string ("").
		public string SelectedPath {
			get {
				return _folderBrowserDialog.SelectedPath;
			}
			set {
				_folderBrowserDialog.SelectedPath=value;
			}
		}

		public DialogResult ShowDialog() {
			if(ODBuild.IsWeb()) {
				MessageBox.Show(Lans.g(this,"Folder browsing not allowed in web mode."));
				return DialogResult.Cancel;
			}
			return _folderBrowserDialog.ShowDialog();
		}

		public void Dispose() {
			_folderBrowserDialog.Dispose();
		}

	}
}
