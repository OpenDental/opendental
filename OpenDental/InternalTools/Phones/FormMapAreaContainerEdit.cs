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
			mapPanel.RefreshEditMode(MapAreaContainerCur);
			VerifyNotTooSmall();
			Size sizeRoom=new Size(MapAreaContainerCur.FloorWidthFeet,MapAreaContainerCur.FloorHeightFeet);
			mapPanel.SetZoomInitialFit(mapPanel.Size,sizeRoom);
			mapPanel.IsChanged+=(sender,e)=>_isChanged=true;
			//LayoutManager.MoveSize(mapPanel,new Size(MapAreaContainerCur.FloorWidthFeet*17,MapAreaContainerCur.FloorHeightFeet*17));
			textDescription.Text=MapAreaContainerCur.Description;
			textWidth.Text=MapAreaContainerCur.FloorWidthFeet.ToString();
			textHeight.Text=MapAreaContainerCur.FloorHeightFeet.ToString();
			textSite.Text=Sites.GetDescription(MapAreaContainerCur.SiteNum);
			mapPanel.Invalidate();
		}

		private void VerifyNotTooSmall(){
			//This doesn't solve the problem of negatives, but we don't allow those.
			Size sizeRoom=new Size(MapAreaContainerCur.FloorWidthFeet,MapAreaContainerCur.FloorHeightFeet);
			Size sizeRoomNew=sizeRoom;
			for(int i=0;i<mapPanel.ListMapAreas.Count;i++){
				Rectangle rectangleRoom=new Rectangle(0,0,sizeRoomNew.Width,sizeRoomNew.Height);//this can change with each loop
				RectangleF rectangleFIntersect=RectangleF.Intersect(rectangleRoom,mapPanel.ListMapAreaMores[i].RectangleFBounds);
				if(!rectangleFIntersect.IsEmpty){//we can see the cubicle
					continue;
				}
				int widthMin=(int)Math.Ceiling(mapPanel.ListMapAreas[i].XPos+mapPanel.ListMapAreas[i].Width);
				if(widthMin>sizeRoomNew.Width){
					sizeRoomNew=new Size(widthMin,sizeRoomNew.Height);
				}
				int heightMin=(int)Math.Ceiling(mapPanel.ListMapAreas[i].YPos+mapPanel.ListMapAreas[i].Height);
				if(heightMin>sizeRoomNew.Height){
					sizeRoomNew=new Size(sizeRoomNew.Width,heightMin);
				}
			}
			if(sizeRoom==sizeRoomNew){
				return;
			}
			MapAreaContainerCur.FloorWidthFeet=sizeRoomNew.Width;
			MapAreaContainerCur.FloorHeightFeet=sizeRoomNew.Height;
			MapAreaContainers.Update(MapAreaContainerCur);
			MsgBox.Show("At least one MapArea was outside the bounds of the Container(room), so the Container was enlarged.");
		}

		private void butEdit_Click(object sender,EventArgs e) {
			using FormMapAreaContainerDetail formMapAreaContainerDetail=new FormMapAreaContainerDetail();
			formMapAreaContainerDetail.MapAreaContainerCur=MapAreaContainerCur;
			formMapAreaContainerDetail.ShowDialog();
			if(formMapAreaContainerDetail.DialogResult==DialogResult.OK){
				_isChanged=true;
			}
			VerifyNotTooSmall();
			Size sizeRoom=new Size(MapAreaContainerCur.FloorWidthFeet,MapAreaContainerCur.FloorHeightFeet);
			mapPanel.SetZoomInitialFit(mapPanel.Size,sizeRoom);
			mapPanel.RefreshEditMode(MapAreaContainerCur);
			//LayoutManager.MoveSize(mapPanel,new Size(MapAreaContainerCur.FloorWidthFeet*17,MapAreaContainerCur.FloorHeightFeet*17));
			textDescription.Text=MapAreaContainerCur.Description;
			textWidth.Text=MapAreaContainerCur.FloorWidthFeet.ToString();
			textHeight.Text=MapAreaContainerCur.FloorHeightFeet.ToString();
			textSite.Text=Sites.GetDescription(MapAreaContainerCur.SiteNum);
		}

		private void checkSnap_Click(object sender,EventArgs e) {
			mapPanel.SnapToFeet=checkSnap.Checked;
		}

		private void butAddCubicle_Click(object sender,EventArgs e){
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
			mapPanel.RefreshEditMode(MapAreaContainerCur);
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
			mapPanel.RefreshEditMode(MapAreaContainerCur);
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