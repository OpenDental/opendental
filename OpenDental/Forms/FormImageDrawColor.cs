using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormImageDrawColor:FormODBase {
		///<summary>Not really used here. Just shown to user if they click Transparent.</summary>
		public Color ColorBack;
		///<summary>Both lines and text.</summary>
		public Color ColorFore;
		///<summary>Can be transparent.</summary>
		public Color ColorTextBack;
		///<summary>Just shows a label for mounts.</summary>
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
				butColorTextBack.BackColor=ColorBack;
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
				//if Transparent was checked, it can stay checked.
				return;
			}
			checkTransparent.Checked=false;
			butColorTextBack.BackColor=colorDialog.Color;
		}

		private void checkTransparent_Click(object sender,EventArgs e) {
			if(checkTransparent.Checked){
				butColorTextBack.BackColor=ColorBack;//interpreted by user as transparent
			}
			else{
				//they can also do the same thing by editing the color, and the box will automatically uncheck.
				butColorTextBack.BackColor=ColorBack;
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			ColorFore=butColorFore.BackColor;
			if(checkTransparent.Checked){
				ColorTextBack=Color.Transparent;
			}
			else{
				ColorTextBack=butColorTextBack.BackColor;
			}
			DialogResult=DialogResult.OK;
		}

	}
}