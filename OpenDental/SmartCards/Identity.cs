using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OpenDental.SmartCards {
	public class Identity {
		private string firstName="";
		public string FirstName {
			get { return firstName; }
		}

		private string lastName="";
		public string LastName {
			get { return lastName; }
		}

		private Image picture=null;
		public Image Picture {
			get { return picture; }
		}
	}
}
