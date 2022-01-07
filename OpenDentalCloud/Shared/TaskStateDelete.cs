using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentalCloud.Core {
	public abstract class TaskStateDelete : TaskState {

		private string _path;
			
		///<summary>The folder of the corresponding file to be deleted</summary>
		public string Path {
			get {
				string path="";
				lock(_lock) {
					path=_path;
				}
				return path;
			}
			set {
				lock(_lock) {
					_path=value;
				}
			}
		}
	}
}
