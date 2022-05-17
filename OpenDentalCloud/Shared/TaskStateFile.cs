using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentalCloud.Core {
	public abstract class TaskStateFile : TaskState {

		///<summary>If a file is greater than 2MB in size, we will break it up into chunks when uploading it to Dropbox.</summary>
		protected const int MAX_FILE_SIZE_BYTES=2000000;
		
		private string _folder;
		private string _fileName;
		private byte[] _fileContent=new byte[1];
		private string _fileId;
		
		///<summary>The folder of the corresponding file to be downloaded</summary>
		public string Folder {
			get {
				string folder="";
				lock(_lock) {
					folder=_folder;
				}
				return folder;
			}
			set {
				lock(_lock) {
					_folder=value;
				}
			}
		}
			
		///<summary>The file name of the file to be downloaded.</summary>
		public string FileName {
			get {
				string fileName="";
				lock(_lock) {
					fileName=_fileName;
				}
				return fileName;
			}
			set {
				lock(_lock) {
					_fileName=value;
				}
			}
		}
			
		///<summary>The file stored in bytes.  This value will grow while the download is still in progress.</summary>
		public byte[] FileContent {
			get {
				byte[] fileContent=new byte[1];
				lock(_lock) {
					fileContent=_fileContent;
				}
				return fileContent;
			}
			set {
				lock(_lock) {
					_fileContent=value;
				}
			}
		}			
	}
}
