using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using Intuit.Ipp.Data;

namespace OpenDental {
	public partial class FormMapAreaContainerEdit:FormODBase {
		public MapAreaContainer MapAreaContainerCur;
		private bool _isChanged;

		public FormMapAreaContainerEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMapAreaContainerEdit_Load(object sender,EventArgs e) {
			mapPanel.IsEditMode=true;
			mapPanel.IsChanged+=MapPanel_IsChanged;
			mapPanel.MapAreaContainerNum=MapAreaContainerCur.MapAreaContainerNum;
			mapPanel.RefreshCubicles();
			LayoutManager.MoveSize(mapPanel,new Size(MapAreaContainerCur.FloorWidthFeet*17,MapAreaContainerCur.FloorHeightFeet*17));
			textDescription.Text=MapAreaContainerCur.Description;
			textWidth.Text=MapAreaContainerCur.FloorWidthFeet.ToString();
			textHeight.Text=MapAreaContainerCur.FloorHeightFeet.ToString();
			textSite.Text=Sites.GetDescription(MapAreaContainerCur.SiteNum);
		}

		private void MapPanel_IsChanged(object sender,EventArgs e) {
			_isChanged=true;
		}

		private void butEdit_Click(object sender,EventArgs e) {
			using FormMapAreaContainerDetail formMapAreaContainerDetail=new FormMapAreaContainerDetail();
			formMapAreaContainerDetail.MapAreaContainerCur=MapAreaContainerCur;
			formMapAreaContainerDetail.ShowDialog();
			if(formMapAreaContainerDetail.DialogResult==DialogResult.OK){
				_isChanged=true;
			}
			mapPanel.RefreshCubicles();
			LayoutManager.MoveSize(mapPanel,new Size(MapAreaContainerCur.FloorWidthFeet*17,MapAreaContainerCur.FloorHeightFeet*17));
			textDescription.Text=MapAreaContainerCur.Description;
			textWidth.Text=MapAreaContainerCur.FloorWidthFeet.ToString();
			textHeight.Text=MapAreaContainerCur.FloorHeightFeet.ToString();
			textSite.Text=Sites.GetDescription(MapAreaContainerCur.SiteNum);
		}

		private void checkSnap_Click(object sender,EventArgs e) {
			mapPanel.SnapToFeet=checkSnap.Checked;
		}

		private void butAddSmall_Click(object sender,EventArgs e){
			MapArea mapArea=new MapArea();
			mapArea.IsNew=true;
			mapArea.Width=3;
			mapArea.Height=3;
			mapArea.ItemType=MapItemType.Cubicle;
			mapArea.Description="";
			mapArea.MapAreaContainerNum=MapAreaContainerCur.MapAreaContainerNum;
			using FormMapAreaEdit formMapAreaEdit=new FormMapAreaEdit();
			formMapAreaEdit.MapAreaCur=mapArea;
			if(formMapAreaEdit.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			_isChanged=true;
			mapPanel.RefreshCubicles();
		}

		private void butAddBig_Click(object sender,EventArgs e){
			MapArea mapArea=new MapArea();
			mapArea.IsNew=true;
			mapArea.Width=6;
			mapArea.Height=6;
			mapArea.ItemType=MapItemType.Cubicle;
			mapArea.Description="";
			mapArea.MapAreaContainerNum=MapAreaContainerCur.MapAreaContainerNum;
			using FormMapAreaEdit formMapAreaEdit=new FormMapAreaEdit();
			formMapAreaEdit.MapAreaCur=mapArea;
			if(formMapAreaEdit.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			_isChanged=true;
			mapPanel.RefreshCubicles();
		}

		private void butAddLabel_Click(object sender,EventArgs e){
			MapArea mapArea=new MapArea();
			mapArea.IsNew=true;
			mapArea.Width=6;
			mapArea.Height=2;
			mapArea.ItemType=MapItemType.Label;
			mapArea.Description="";
			mapArea.MapAreaContainerNum=MapAreaContainerCur.MapAreaContainerNum;
			using FormMapAreaEdit formMapAreaEdit=new FormMapAreaEdit();
			formMapAreaEdit.MapAreaCur=mapArea;
			if(formMapAreaEdit.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			_isChanged=true;
			mapPanel.RefreshCubicles();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(MapAreaContainerCur.IsNew && mapPanel.ListMapAreas.Count==0){
				MapAreaContainers.Delete(MapAreaContainerCur.MapAreaContainerNum);
				Close();
				return;
			}
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,"This will delete the entire container and all cubicles within it.  Are you sure you want to delete?"))
			{
				return;
			}
			MapAreas.DeleteAll(MapAreaContainerCur.MapAreaContainerNum);
			MapAreaContainers.Delete(MapAreaContainerCur.MapAreaContainerNum);
			_isChanged=true;
			Close();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormMapAreaContainerEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.PhoneMap);
			}
		}
	}
}