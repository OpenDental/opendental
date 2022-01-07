using System;
using OpenDentBusiness;
using CodeBase;
using System.Globalization;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using DataConnectionBase;

namespace OpenDental {
	///<summary>A splash screen to show when the program launches.</summary>
	public partial class FormSplash : Form {

		///<summary>Launches a splash screen.</summary>
		public FormSplash() {
			InitializeComponent();
			Lan.F(this);
		}

		private void FormSplash_Load(object sender,EventArgs e) {
			CreateBencoSplash(); 
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				BackgroundImage=Properties.Resources.splashCanada;
			}
			if(File.Exists(Directory.GetCurrentDirectory()+@"\Splash.jpg")) {
				BackgroundImage=new Bitmap(Directory.GetCurrentDirectory()+@"\Splash.jpg");
			}
			if(Plugins.PluginsAreLoaded) {
				Plugins.HookAddCode(this,"FormSplash.FormSplash_Load_end");
			}
		}

		/// <summary>Only creates the Benco splash if Benco is enabled and a splash does not exist</summary>
		private void CreateBencoSplash() {
			try {
				if(DataConnection.HasDatabaseConnection && Programs.GetCur(ProgramName.BencoPracticeManagement).Enabled
					&& !File.Exists(Directory.GetCurrentDirectory()+@"\Splash.jpg")) 
				{
					Properties.Resources.splashBenco.Save(Directory.GetCurrentDirectory()+@"\Splash.jpg");
				}
			}
			catch (Exception ex) {
				ex.DoNothing();
			}
		}
  }
}