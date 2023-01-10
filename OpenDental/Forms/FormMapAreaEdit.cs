using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMapAreaEdit:FormODBase {

		///<summary>The item being edited</summary>
		public MapArea MapAreaCur;

		public FormMapAreaEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormMapAreaEdit_Load(object sender,EventArgs e) {
			//show/hide fields according to MaptItemType
			if(MapAreaCur.ItemType==MapItemType.Cubicle) {
				labelDescription.Text="Description. Example: B2 5:3";			
			}
			else {
				textBoxExtension.Visible=false;
				labelExtension.Visible=false;
				textBoxHeightFeet.Visible=false;
				labelHeight.Visible=false;
				labelDescription.Text="Text";			
			}
			textBoxXPos.Text=MapAreaCur.XPos.ToString();
			textBoxYPos.Text=MapAreaCur.YPos.ToString();
			textBoxExtension.Text=MapAreaCur.Extension.ToString();
			textBoxWidthFeet.Text=MapAreaCur.Width.ToString();
			textBoxHeightFeet.Text=MapAreaCur.Height.ToString();
			textBoxDescription.Text=MapAreaCur.Description;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(MapAreaCur.IsNew) {
				DialogResult=System.Windows.Forms.DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remove Map Area?")) {
				return;
			}
			MapAreas.Delete(MapAreaCur.MapAreaNum);
			DialogResult=System.Windows.Forms.DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {






			if(PIn.Double(textBoxXPos.Text)<0) {
				textBoxXPos.Focus();
				MessageBox.Show(Lan.g(this,"Invalid XPos"));
				return;
			}
			if(PIn.Double(textBoxYPos.Text)<0) {
				textBoxYPos.Focus();
				MessageBox.Show(Lan.g(this,"Invalid YPos"));
				return;
			}
			if(PIn.Double(textBoxWidthFeet.Text)<=0) {
				textBoxWidthFeet.Focus();
				MessageBox.Show(Lan.g(this,"Invalid Width"));
				return;
			}
			if(PIn.Double(textBoxHeightFeet.Text)<=0) {
				textBoxHeightFeet.Focus();
				MessageBox.Show(Lan.g(this,"Invalid Height"));
				return;
			}
			if(PIn.Int(textBoxExtension.Text)<0) {
				textBoxExtension.Focus();
				MessageBox.Show(Lan.g(this,"Invalid Extension"));
				return;
			}
			if(MapAreaCur.ItemType==MapItemType.Label && PIn.String(textBoxDescription.Text)=="") {
				textBoxDescription.Focus();
				MessageBox.Show(Lan.g(this,"Invalid Text"));
				return;
			}
			MapAreaCur.Extension=PIn.Int(textBoxExtension.Text);
			MapAreaCur.XPos=PIn.Double(textBoxXPos.Text);
			MapAreaCur.YPos=PIn.Double(textBoxYPos.Text);
			MapAreaCur.Width=PIn.Double(textBoxWidthFeet.Text);
			MapAreaCur.Height=PIn.Double(textBoxHeightFeet.Text);
			MapAreaCur.Description=PIn.String(textBoxDescription.Text);
			if(MapAreaCur.IsNew) {
				MapAreas.Insert(MapAreaCur);
			}
			else {
				MapAreas.Update(MapAreaCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
