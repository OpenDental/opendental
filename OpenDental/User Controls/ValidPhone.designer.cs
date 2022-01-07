using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDental {
	partial class ValidPhone {
		private System.ComponentModel.Container components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		private void InitializeComponent(){
			this.SuspendLayout();
			// 
			// ValidPhone
			// 
			this.TextChanged += new System.EventHandler(this.ValidPhone_TextChanged);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
