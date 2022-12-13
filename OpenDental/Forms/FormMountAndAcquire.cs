using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormMountAndAcquire:FormODBase {
		public ImagingDevice ImagingDeviceSelected;
		public MountDef MountDefSelected;

		private List<ImagingDevice> _listImagingDevices;
		private List<MountDef> _listMountDefs;
		private float _ratio;
		///<summary>Rectangle of the mount within the 100x100 bitmap.</summary>
		private Rectangle _rectangleMount;

		public FormMountAndAcquire() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMountPick_Load(object sender, EventArgs e){
			ImageList imageList=new ImageList(this.components);//automatically disposed
			imageList.ImageSize=new Size(100,100);
			listViewMounts.LargeImageList=imageList;
			_listMountDefs=MountDefs.GetDeepCopy();
			List<MountItemDef> listMountItemDefsAll=MountItemDefs.GetAll();
			for(int i=0;i<_listMountDefs.Count;i++){
				CalcRatio(_listMountDefs[i]);
				List<MountItemDef> listMountItemDefs=listMountItemDefsAll.FindAll(x=>x.MountDefNum==_listMountDefs[i].MountDefNum);
				Bitmap bitmap=CreateBitmap(_listMountDefs[i],listMountItemDefs);
				imageList.Images.Add(bitmap);
				ListViewItem listViewItem=new ListViewItem();//text:"Item 1",imageIndex:0);
				listViewItem.Text=_listMountDefs[i].Description;
				listViewItem.ImageIndex=i;
				listViewMounts.Items.Add(listViewItem);
			}
			//_stringNotify="";
			if(_listMountDefs.Count!=0){
				//_stringNotify=Lan.g(this,"Set up mounts from Main Menu - Setup - Imaging - Mounts.");
			//}
			//else{
				listViewMounts.SelectedIndices.Add(0);
			}
			List<ImagingDevice> listImagingDevicesAll=ImagingDevices.GetDeepCopy();
			string workstation=ODEnvironment.MachineName;
			_listImagingDevices=listImagingDevicesAll.FindAll(x=>x.ComputerName=="" || x.ComputerName==workstation);
			listDevices.Items.AddList(_listImagingDevices,x=>x.Description);
			if(_listImagingDevices.Count!=0){
				listDevices.SetSelected(0);
			}
		}

		private void FormMountSelect_Shown(object sender, EventArgs e){
			//if(_stringNotify!=""){
			//	MsgBox.Show(_stringNotify);
			//}
			//}
		}

		private void CalcRatio(MountDef mountDef){
			Rectangle rectangleBack=new Rectangle(0,0,100,100);
			float ratioWidth=(float)rectangleBack.Width/mountDef.Width;
			float ratioHeight=(float)rectangleBack.Height/mountDef.Height;
			_ratio=ratioWidth;
			bool isWider=false;
			if(ratioHeight<ratioWidth){
				isWider=true;
				_ratio=ratioHeight;
			}
			float xMain=0;
			if(isWider){
				xMain=(rectangleBack.Width-mountDef.Width*_ratio)/2;
			}
			float yMain=0;
			if(!isWider){
				yMain=(rectangleBack.Height-mountDef.Height*_ratio)/2;
			}
			_rectangleMount=new Rectangle((int)xMain,(int)yMain,(int)(mountDef.Width*_ratio),(int)(mountDef.Height*_ratio));
		}

		private Bitmap CreateBitmap(MountDef mountDef,List<MountItemDef> listMountItemDefs){
			Bitmap bitmap=new Bitmap(100,100);
			using Graphics g=Graphics.FromImage(bitmap);
			g.Clear(Color.White);
			using SolidBrush solidBrushBack=new SolidBrush(mountDef.ColorBack);
			g.FillRectangle(solidBrushBack,_rectangleMount);
			using Pen penOutline=new Pen(ColorOD.Gray(100));
			for(int i=0;i<listMountItemDefs.Count;i++){
				RectangleF rectangleF=new RectangleF(
					_rectangleMount.X+listMountItemDefs[i].Xpos*_ratio,
					_rectangleMount.Y+listMountItemDefs[i].Ypos*_ratio,
					listMountItemDefs[i].Width*_ratio,
					listMountItemDefs[i].Height*_ratio);
				g.DrawRectangle(penOutline,rectangleF.X,rectangleF.Y,rectangleF.Width,rectangleF.Height);
			}
			g.DrawRectangle(penOutline,_rectangleMount);
			g.DrawRectangle(penOutline,new Rectangle(0,0,99,99));
			return bitmap;
			return null;
		}

		private void butAcquire_Click(object sender, EventArgs e){
			if(!ODBuild.IsTrial()
				&& !OpenDentalHelp.ODHelp.IsEncryptedKeyValid())//always true in debug
			{
				MsgBox.Show(this,"This feature requires an active support plan.");
				return;
			}
			if(listDevices.SelectedIndex==-1){
				MsgBox.Show(this,"No device selected.  Set up imaging devices in Main Menu - Setup - Imaging - Devices.");
				return;
			}
			int idxDevice=listDevices.SelectedIndex;
			ImagingDeviceSelected=_listImagingDevices[idxDevice];
			DialogResult=DialogResult.OK;
		}

		private void listViewMounts_MouseDoubleClick(object sender, MouseEventArgs e){
			//this does mount only
			if(listViewMounts.SelectedIndices.Count==0){
				return;
			}
			int idxMount=listViewMounts.SelectedIndices[0];
			MountDefSelected=_listMountDefs[idxMount];
			DialogResult=DialogResult.OK;
		}

		private void butMount_Click(object sender,EventArgs e) {
			if(listViewMounts.SelectedIndices.Count==0){
				MsgBox.Show(this,"Please select a mount first.");
				return;
			}
			int idxMount=listViewMounts.SelectedIndices[0];
			MountDefSelected=_listMountDefs[idxMount];
			DialogResult=DialogResult.OK;
		}

		private void butMountAndAcquire_Click(object sender, EventArgs e){
			if(!ODBuild.IsTrial()
				&& !OpenDentalHelp.ODHelp.IsEncryptedKeyValid())//always true in debug
			{
				MsgBox.Show(this,"This feature requires an active support plan.");
				return;
			}
			if(listDevices.SelectedIndex==-1){
				MsgBox.Show(this,"No device selected.  Set up imaging devices in Main Menu - Setup - Imaging - Devices.");
				return;
			}
			if(listViewMounts.SelectedIndices.Count==0){
				MsgBox.Show(this,"Please select a mount first.");
				return;
			}
			int idxMount=listViewMounts.SelectedIndices[0];
			MountDefSelected=_listMountDefs[idxMount];
			int idxDevice=listDevices.SelectedIndex;
			ImagingDeviceSelected=_listImagingDevices[idxDevice];
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}