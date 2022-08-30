using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentalCloud.Core {
	public abstract class TaskStateMove : TaskState {

		private int _countMoveFailed=0;
		private int _countMoveSuccess=0;
		private int _countMoveTotal=0;
		private string _fromPath;
		private string _toPath;
			
		///<summary>The folder of the corresponding file to be downloaded</summary>
		public string FromPath {
			get {
				string fromPath="";
				lock(_lock) {
					fromPath=_fromPath;
				}
				return fromPath;
			}
			set {
				lock(_lock) {
					_fromPath=value;
				}
			}
		}
			
		///<summary>The folder of the corresponding file to be downloaded</summary>
		public string ToPath {
			get {
				string toPath="";
				lock(_lock) {
					toPath=_toPath;
				}
				return toPath;
			}
			set {
				lock(_lock) {
					_toPath=value;
				}
			}
		}
			
		///<summary>Number of move attempts that failed and are still in the original folder.</summary>
		public int CountFailed {
			get {
				int countMoveFailed=0;
				lock(_lock) {
					countMoveFailed=_countMoveFailed;
				}
				return countMoveFailed;
			}
			set {
				lock(_lock) {
					_countMoveFailed=value;
				}
			}
		}
			
		///<summary>Number of move attempts that succeeded and have been removed from the original folder.</summary>
		public int CountSuccess {
			get {
				int countMoveSuccess=0;
				lock(_lock) {
					countMoveSuccess=_countMoveSuccess;
				}
				return countMoveSuccess;
			}
			set {
				lock(_lock) {
					_countMoveSuccess=value;
				}
			}
		}
			
		///<summary>Number of total files to move from the original folder.</summary>
		public int CountTotal {
			get {
				int countMoveTotal=0;
				lock(_lock) {
					countMoveTotal=_countMoveTotal;
				}
				return countMoveTotal;
			}
			set {
				lock(_lock) {
					_countMoveTotal=value;
				}
			}
		}
	}
}
