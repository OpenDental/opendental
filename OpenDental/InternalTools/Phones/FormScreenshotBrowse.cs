using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormScreenshotBrowse:FormODBase {
		public string ScreenshotPath;
		private string[] files;

		public FormScreenshotBrowse() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormScreenshotBrowse_Load(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			pictureBoxMain.Image=Image.FromFile(ScreenshotPath);
			string folder=Path.GetDirectoryName(ScreenshotPath);
			files=Directory.GetFileSystemEntries(folder,"*-*-*");//to exclude thumbs.db
			string[] arrayFileNames=new string[files.Length];
			DateTime[] arrayDates=new DateTime[files.Length];
			for(int i=0;i<files.Length;i++) {
				string filename=Path.GetFileNameWithoutExtension(files[i]);//2011-08-20-07112332
				if(filename.Length==19) {
					filename=filename.Substring(0,10)+" "+filename.Substring(11,2)+":"+filename.Substring(13,2)+":"+filename.Substring(15,2);//2011-08-20 07:11:32
				}
				arrayFileNames[i]=files[i];
				arrayDates[i]=DateTime.ParseExact(filename,"yyyy-MM-dd HH:mm:ss",CultureInfo.CurrentCulture);
			}
			Array.Sort(arrayDates,arrayFileNames);//sort filenames by date
			//if(arrayFileNames.Length>50) {
			//	arrayFileNames=arrayFileNames.CopyTo(
			//}
			for(int i=0;i<arrayFileNames.Length;i++) {
				string filename=Path.GetFileNameWithoutExtension(arrayFileNames[i]);//2011-08-20-07112332
				if(filename.Length==19) {
					filename=filename.Substring(0,10)+" "+filename.Substring(11,2)+":"+filename.Substring(13,2)+":"+filename.Substring(15,2);//2011-08-20 07:11:32
				}
				listFiles.Items.Add(filename);
				if(ScreenshotPath==arrayFileNames[i]) {
					listFiles.SetSelected(i);
				}
			}
			Cursor=Cursors.Default;
		}

		void listFiles_SelectedIndexChanged(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			pictureBoxMain.Image=Image.FromFile(files[listFiles.SelectedIndex]);
			Cursor=Cursors.Default;
		}

		



	}
}
