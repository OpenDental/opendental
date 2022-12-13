using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMapAreaEdit:FormODBase {

		///<summary>The item being edited</summary>
		public MapArea MapAreaItem;

		public FormMapAreaEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormMapAreaEdit_Load(object sender,EventArgs e) {
			//show/hide fields according to MaptItemType
			if(MapAreaItem.ItemType==MapItemType.Cubicle) {
				textBoxExtension.Visible=true;
				labelExtension.Visible=true;
				textBoxHeightFeet.Visible=true;
				labelHeight.Visible=true;
				labelDescription.Text="Description (shown when extension is 0)";			
			}
			else {
				textBoxExtension.Visible=false;
				labelExtension.Visible=false;
				textBoxHeightFeet.Visible=false;
				labelHeight.Visible=false;
				labelDescription.Text="Text";			
			}
			//populate the fields
			textBoxXPos.Text=MapAreaItem.XPos.ToString();
			textBoxYPos.Text=MapAreaItem.YPos.ToString();
			textBoxExtension.Text=MapAreaItem.Extension.ToString();
			textBoxWidthFeet.Text=MapAreaItem.Width.ToString();
			textBoxHeightFeet.Text=MapAreaItem.Height.ToString();
			textBoxDescription.Text=MapAreaItem.Description;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			try {
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
				if(MapAreaItem.ItemType==MapItemType.Label && PIn.String(textBoxDescription.Text)=="") {
					textBoxDescription.Focus();
					MessageBox.Show(Lan.g(this,"Invalid Text"));
					return;
				}
				MapAreaItem.Extension=PIn.Int(textBoxExtension.Text);
				MapAreaItem.XPos=PIn.Double(textBoxXPos.Text);
				MapAreaItem.YPos=PIn.Double(textBoxYPos.Text);
				MapAreaItem.Width=PIn.Double(textBoxWidthFeet.Text);
				MapAreaItem.Height=PIn.Double(textBoxHeightFeet.Text);
				MapAreaItem.Description=PIn.String(textBoxDescription.Text);
				if(MapAreaItem.IsNew) {
					MapAreas.Insert(MapAreaItem);
				}
				else {
					MapAreas.Update(MapAreaItem);
				}
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(MapAreaItem.IsNew) {
				DialogResult=System.Windows.Forms.DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remove Map Area?")) {
				return;
			}
			MapAreas.Delete(MapAreaItem.MapAreaNum);
			DialogResult=System.Windows.Forms.DialogResult.OK;
		}
	}
}
