using System;
using System.IO;
using System.Windows.Forms;
using CodeBase;

namespace OpenDentBusiness.Eclaims {

	public partial class x837Controller {
		///<summary>Func that abstracts Lans.g.</summary>
		private static Func<string,string,string> _lans_g=(sender,text) => text;

		///<summary>Copies the given file to an archive directory within the same directory as the file.</summary>
		public static void CopyToArchive(string fileName,bool doThrow=false) {
			string direct=Path.GetDirectoryName(fileName);
			string fileOnly=Path.GetFileName(fileName);
			direct = direct.Replace("\\"+ODEnvironment.MachineName,"");
			string archiveDir=ODFileUtils.CombinePaths(direct,"archive");
			try {
				if(!Directory.Exists(archiveDir)) {
					Directory.CreateDirectory(archiveDir);
				}
				File.Copy(fileName,ODFileUtils.CombinePaths(archiveDir,fileOnly),true);
			}
			catch(Exception ex) {
				string error=_lans_g("FormClaimsSend","Unable to copy file to the archive directory. Check to make sure you have "
					+"permission to modify the archive located at")+" "+archiveDir+"\r\n\r\n"+_lans_g("FormClaimsSend","Error message:")+" "
					+ex.Message;
				if(doThrow) {
					throw new ODException(error,ODException.ErrorCodes.ClaimArchiveFailed);
				}
				MessageBox.Show(error);
			}
		}
	}
}
