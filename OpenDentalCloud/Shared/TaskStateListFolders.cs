using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentalCloud.Core {
	public abstract class TaskStateListFolders : TaskState {

		private List<string> _listFolderPathDisplay=new List<string>();
		private List<string> _listFolderPathLower=new List<string>();
		private List<string> _listFileIds=new List<string>();
		private string _folderPath;
			
		///<summary>The folder of the corresponding file to be downloaded</summary>
		public string FolderPath {
			get {
				string folderPath="";
				lock(_lock) {
					folderPath=_folderPath;
				}
				return folderPath;
			}
			set {
				lock(_lock) {
					_folderPath=value;
				}
			}
		}
			
		///<summary>List of cased paths that were found in </summary>
		public List<string> ListFolderPathsDisplay {
			get {
				List<string> listFolderPaths=new List<string>();
				lock(_lock) {
					foreach(string path in _listFolderPathDisplay) {
						listFolderPaths.Add(path);
					}
				}
				return listFolderPaths;
			}
			set {
				lock(_lock) {
					_listFolderPathDisplay=value;
				}
			}
		}
			
		///<summary>List of folder and file paths that were found for the given folder path.
		///PathLower is the preferred path for making transactions between Dropbox and Open Dental, as PathDisplay is used for UI.</summary>
		public List<string> ListFolderPathLower {
			get {
				List<string> listFolderPaths=new List<string>();
				lock(_lock) {
					foreach(string path in _listFolderPathLower) {
						listFolderPaths.Add(path);
					}
				}
				return listFolderPaths;
			}
			set {
				lock(_lock) {
					_listFolderPathLower=value;
				}
			}
		}		
	}
}
