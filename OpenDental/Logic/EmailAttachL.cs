using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public class EmailAttachL {

		///<summary>Allow the user to pick the files to be attached. The 'pat' argument can be null. If the user cancels at any step, the return value
		///will be an empty list.</summary>
		public static List<EmailAttach> PickAttachments(Patient patient) {
			List<EmailAttach> listEmailAttaches=new List<EmailAttach>();
			OpenFileDialog openFileDialog=new OpenFileDialog();
			openFileDialog.Multiselect=true;
			bool isLocalFileSelected=false;
			List<string> listFileNames;
			if(patient != null && PrefC.AtoZfolderUsed != DataStorageType.InDatabase) {
				string patFolder=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
				if(CloudStorage.IsCloudStorage) {
					using FormFilePicker formFilePicker=new FormFilePicker(patFolder);
					if(formFilePicker.ShowDialog() != DialogResult.OK) {
						return listEmailAttaches;
					}
					isLocalFileSelected=formFilePicker.WasLocalFileSelected;
					listFileNames=formFilePicker.ListSelectedFiles;
				}
				else {
					openFileDialog.InitialDirectory=patFolder;
					if(openFileDialog.ShowDialog() != DialogResult.OK) {
						return listEmailAttaches;
					}
					isLocalFileSelected=true;
					listFileNames=openFileDialog.FileNames.ToList();
				}
			}
			else {//No patient selected or images in database
				//Use the OS default directory for this type of file viewer.
				openFileDialog.InitialDirectory="";
				if(openFileDialog.ShowDialog() != DialogResult.OK) {
					return listEmailAttaches;
				}
				isLocalFileSelected=true;
				listFileNames=openFileDialog.FileNames.ToList();
			}
			try {
				for(int i=0;i<listFileNames.Count;i++) {
					if(!CloudStorage.IsCloudStorage){
						listEmailAttaches.Add(EmailAttaches.CreateAttach(Path.GetFileName(listFileNames[i]),File.ReadAllBytes(listFileNames[i])));
						continue;
					}
					FileAtoZSourceDestination fileAtoZSourceDestination;
					if(isLocalFileSelected) {
						fileAtoZSourceDestination=FileAtoZSourceDestination.LocalToAtoZ;
					}
					else {
						fileAtoZSourceDestination=FileAtoZSourceDestination.AtoZToAtoZ;
					}
					//Create EmailAttach using EmailAttaches.CreateAttach logic, shortened for our specific purpose.
					EmailAttach emailAttach=new EmailAttach();
					emailAttach.DisplayedFileName=Path.GetFileName(listFileNames[i]);
					string attachDir=EmailAttaches.GetAttachPath();
					string subDir="Out";
					emailAttach.ActualFileName=ODFileUtils.CombinePaths(subDir,
						DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()
							+"_"+MiscUtils.CreateRandomAlphaNumericString(4)+"_"+emailAttach.DisplayedFileName).Replace("\\","/");
					FileAtoZ.Copy(listFileNames[i],FileAtoZ.CombinePaths(attachDir,emailAttach.ActualFileName),fileAtoZSourceDestination);
					listEmailAttaches.Add(emailAttach);
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
			return listEmailAttaches;
		}
	}
}
