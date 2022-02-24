using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.UI {
	public partial class ContrYN:UserControl {

		///<summary></summary>
		public ContrYN() {
			InitializeComponent();
			Lan.C(this,new Control[] {
				checkY,
				checkN
			});
			checkN.Location=new Point(checkY.Right+10,checkN.Top);
		}

		/// <summary>Gets or sets the value of the control</summary>
		[Category("Data"),
			Description("Yes or No will check the appropriate box.  Unknown means neither box is checked.")
		]
		public YN CurrentValue{
			get{
				if(checkY.Checked){
					return YN.Yes;
				}
				else if(checkN.Checked){
					return YN.No;
				}
				return YN.Unknown;
			}
			set{
				if(value==YN.Yes){
					checkY.Checked=true;
					checkN.Checked=false;
				}
				else if(value==YN.No) {
					checkY.Checked=false;
					checkN.Checked=true;
				}
				else{
					checkY.Checked=false;
					checkN.Checked=false;
				}
			}
		}

		private void checkY_Click(object sender,EventArgs e) {
			if(checkY.Checked){
				checkN.Checked=false;
			}
		}

		private void checkN_Click(object sender,EventArgs e) {
			if(checkN.Checked) {
				checkY.Checked=false;
			}
		}




	}
	
}
