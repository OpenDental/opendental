using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.Email {
	public class BasicEmailAttachment {
		///<summary>The full path to the file.</summary>
		public string FullPath;
		///<summary>The human readable displayed filename.</summary>
		public string DisplayedFilename;

		///<summary>For serialization</summary>
		public BasicEmailAttachment() {
		}
	
		public BasicEmailAttachment(string fullPath,string displayedFilename) {
			FullPath=fullPath;
			DisplayedFilename=displayedFilename;
		}
	}
}
