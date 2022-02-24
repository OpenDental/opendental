using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI {
	public partial class ODCodeRangeFilter:UserControl {
		/// <summary>Gets the start value of the control.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string StartRange {
			get {
				return GetRange(true);
			}
		}

		/// <summary>Gets the end value of the control.</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string EndRange {
			get {
				return GetRange(false);
			}
		}

		public ODCodeRangeFilter() {
			InitializeComponent();
		}

		private string GetRange(bool getStart) {
			string codeStart="";
			string codeEnd="";
			if(!string.IsNullOrEmpty(textCodeRange.Text.Trim())) {
				if(textCodeRange.Text.Contains("-")) {
					string[] codeSplit=textCodeRange.Text.Split('-');
					codeStart=codeSplit[0].Trim().Replace('d','D');
					codeEnd=codeSplit[1].Trim().Replace('d','D');
				}
				else {
					codeStart=textCodeRange.Text.Trim().Replace('d','D');
					codeEnd=textCodeRange.Text.Trim().Replace('d','D');
				}
			}
			return (getStart ? codeStart : codeEnd);
		}
	}
}
