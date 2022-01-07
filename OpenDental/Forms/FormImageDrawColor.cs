using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormImageDrawColor:FormODBase {
		///<summary>Not used except for text.</summary>
		public Color ColorText;
		///<summary>Used for text background and also for drawing color. Can be transparent.</summary>
		public Color ColorBackground;
		///<summary>Otherwise, just single drawing color.</summary>
		public bool IsText;

		public FormImageDrawColor() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormImageDrawEdit_Load(object sender, EventArgs e){
			butColor.BackColor=ColorText;
			butColorBack.BackColor=ColorBackground;
			if(ColorBackground.ToArgb()==Color.Transparent.ToArgb()){
				checkTransparent.Checked=true;
				butColorBack.BackColor=Color.Transparent;
			}
			else{
				butColorBack.BackColor=ColorBackground;
			}
		}

		private void butColor_Click(object sender, EventArgs e){
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColor.BackColor;
			colorDialog.ShowDialog();
			butColor.BackColor=colorDialog.Color;
		}

		private void butColorBack_Click(object sender,EventArgs e) {
			using ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColorBack.BackColor;
			DialogResult dialogResult=colorDialog.ShowDialog();
			if(dialogResult!=DialogResult.OK){
				//if None was checked, it can stay checked.
				return;
			}
			checkTransparent.Checked=false;
			butColorBack.BackColor=colorDialog.Color;
		}

		private void checkNone_Click(object sender,EventArgs e) {
			if(checkTransparent.Checked){
				//butColorBack.BackColor=Color.Empty;//this doesn't work.  Button won't take an empty color.
				butColorBack.BackColor=Color.White;//interpreted by user as empty
			}
			else{
				//they can also do the same thing by editing the color, and the box will automatically uncheck.
				butColorBack.BackColor=Color.White;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			ColorText=butColor.BackColor;
			if(checkTransparent.Checked){
				ColorBackground=Color.Transparent;
			}
			else{
				ColorBackground=butColorBack.BackColor;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}