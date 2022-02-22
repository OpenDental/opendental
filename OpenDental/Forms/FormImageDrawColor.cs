using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormImageDrawColor:FormODBase {
		///<summary>Both lines and text.</summary>
		public Color ColorFore;
		///<summary>Can be transparent.</summary>
		public Color ColorTextBack;
		///<summary></summary>
		public bool IsMount;

		public FormImageDrawColor() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormImageDrawEdit_Load(object sender, EventArgs e){
			if(!IsMount){
				labelMount.Visible=false;
			}
			butColorFore.BackColor=ColorFore;
			butColorTextBack.BackColor=ColorTextBack;
			if(ColorTextBack.ToArgb()==Color.Transparent.ToArgb()){
				checkTransparent.Checked=true;
				butColorTextBack.BackColor=Color.Transparent;
			}
		}

		private void butColorFore_Click(object sender, EventArgs e){
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColorFore.BackColor;
			colorDialog.ShowDialog();
			butColorFore.BackColor=colorDialog.Color;
		}

		private void butColorTextBack_Click(object sender,EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColorTextBack.BackColor;
			DialogResult dialogResult=colorDialog.ShowDialog();
			if(dialogResult!=DialogResult.OK){
				//if None was checked, it can stay checked.
				return;
			}
			checkTransparent.Checked=false;
			butColorTextBack.BackColor=colorDialog.Color;
		}

		private void checkNone_Click(object sender,EventArgs e) {
			if(checkTransparent.Checked){
				//butColorBack.BackColor=Color.Empty;//this doesn't work.  Button won't take an empty color.
				butColorTextBack.BackColor=Color.White;//interpreted by user as empty
			}
			else{
				//they can also do the same thing by editing the color, and the box will automatically uncheck.
				butColorTextBack.BackColor=Color.White;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			ColorFore=butColorFore.BackColor;
			if(checkTransparent.Checked){
				ColorTextBack=Color.Transparent;
			}
			else{
				ColorTextBack=butColorTextBack.BackColor;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}