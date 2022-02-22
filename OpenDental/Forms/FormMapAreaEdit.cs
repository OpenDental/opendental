using System;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMapAreaEdit:FormODBase {

		///<summary>The item being edited</summary>
		public MapArea MapItem;

		public FormMapAreaEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormMapAreaEdit_Load(object sender,EventArgs e) {
			//show/hide fields according to MaptItemType
			if(MapItem.ItemType==MapItemType.Room) {
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
			textBoxXPos.Text=MapItem.XPos.ToString();
			textBoxYPos.Text=MapItem.YPos.ToString();
			textBoxExtension.Text=MapItem.Extension.ToString();
			textBoxWidthFeet.Text=MapItem.Width.ToString();
			textBoxHeightFeet.Text=MapItem.Height.ToString();
			textBoxDescription.Text=MapItem.Description;
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
				if(MapItem.ItemType==MapItemType.DisplayLabel && PIn.String(textBoxDescription.Text)=="") {
					textBoxDescription.Focus();
					MessageBox.Show(Lan.g(this,"Invalid Text"));
					return;
				}
				MapItem.Extension=PIn.Int(textBoxExtension.Text);
				MapItem.XPos=PIn.Double(textBoxXPos.Text);
				MapItem.YPos=PIn.Double(textBoxYPos.Text);
				MapItem.Width=PIn.Double(textBoxWidthFeet.Text);
				MapItem.Height=PIn.Double(textBoxHeightFeet.Text);
				MapItem.Description=PIn.String(textBoxDescription.Text);
				if(MapItem.IsNew) {
					MapAreas.Insert(MapItem);
				}
				else {
					MapAreas.Update(MapItem);
				}
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(MapItem.IsNew) {
				DialogResult=System.Windows.Forms.DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remove Map Area?")) {
				return;
			}
			MapAreas.Delete(MapItem.MapAreaNum);
			DialogResult=System.Windows.Forms.DialogResult.OK;
		}
	}
}
