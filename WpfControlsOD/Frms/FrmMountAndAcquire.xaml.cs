using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;
using CodeBase;
using OpenDental.Drawing;

namespace OpenDental {
	public partial class FrmMountAndAcquire:FrmODBase {
		public ImagingDevice ImagingDeviceSelected;
		public MountDef MountDefSelected;

		private List<ImagingDevice> _listImagingDevices;
		private List<MountDef> _listMountDefs;
		private float _ratio;
		///<summary>Rectangle of the mount within the 100x100 bitmap.</summary>
		private Rect _rectMount;

		public FrmMountAndAcquire() {
			InitializeComponent();
			Load+=FrmMountAndAcquite_Load;
			listViewMounts.MouseDoubleClick+=listViewMounts_MouseDoubleClick;
		}

		private void FrmMountAndAcquite_Load(object sender, EventArgs e){
			Lang.F(this);
			_listMountDefs=MountDefs.GetDeepCopy();
			List<MountItemDef> listMountItemDefsAll=MountItemDefs.GetAll();
			for(int i=0;i<_listMountDefs.Count;i++){
				CalcRatio(_listMountDefs[i]);
				List<MountItemDef> listMountItemDefs=listMountItemDefsAll.FindAll(x=>x.MountDefNum==_listMountDefs[i].MountDefNum);
				BitmapImage bitmapImage=CreateBitmap(_listMountDefs[i],listMountItemDefs);
				ListViewItem listViewItem=new ListViewItem();
				listViewItem.Text=_listMountDefs[i].Description;
				listViewItem.BitmapImage_=bitmapImage;
				listViewMounts.AddItem(listViewItem);
			}
			if(_listMountDefs.Count!=0){
				listViewMounts.SelectedIndex=0;
			}
			List<ImagingDevice> listImagingDevicesAll=ImagingDevices.GetDeepCopy();
			string workstation=ODEnvironment.MachineName;
			_listImagingDevices=listImagingDevicesAll.FindAll(x=>x.ComputerName=="" || x.ComputerName==workstation);
			listDevices.Items.AddList(_listImagingDevices,x=>x.Description);
			if(_listImagingDevices.Count!=0){
				listDevices.SetSelected(0);
			}
		}

		private void CalcRatio(MountDef mountDef){
			Rect rectBack=new Rect(0,0,100,100);
			float ratioWidth=(float)rectBack.Width/mountDef.Width;
			float ratioHeight=(float)rectBack.Height/mountDef.Height;
			_ratio=ratioWidth;
			bool isWider=false;
			if(ratioHeight<ratioWidth){
				isWider=true;
				_ratio=ratioHeight;
			}
			double xMain=0;
			if(isWider){
				xMain=(rectBack.Width-mountDef.Width*_ratio)/2;
			}
			double yMain=0;
			if(!isWider){
				yMain=(rectBack.Height-mountDef.Height*_ratio)/2;
			}
			_rectMount=new Rect((int)xMain,(int)yMain,(int)(mountDef.Width*_ratio),(int)(mountDef.Height*_ratio));
		}

		private BitmapImage CreateBitmap(MountDef mountDef,List<MountItemDef> listMountItemDefs){
			Graphics g = Graphics.BitmapInit(100,100);
			g.Clear(Colors.White);
			Color colorBack = ColorOD.ToWpf(mountDef.ColorBack);
			g.FillRectangle(colorBack,_rectMount);
			Color colorOutline = ColorOD.Gray_Wpf(100);
			for(int i = 0;i<listMountItemDefs.Count;i++) {
				Rect rect = new Rect(
					_rectMount.X+listMountItemDefs[i].Xpos*_ratio,
					_rectMount.Y+listMountItemDefs[i].Ypos*_ratio,
					listMountItemDefs[i].Width*_ratio,
					listMountItemDefs[i].Height*_ratio);
				g.DrawRectangle(colorOutline,rect.X,rect.Y,rect.Width,rect.Height);
			}
			g.DrawRectangle(colorOutline,_rectMount);
			g.DrawRectangle(colorOutline,new Rect(0,0,99,99));
			BitmapImage bitmapImage = g.BitmapCreate();
			return bitmapImage;
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
			IsDialogOK=true;
		}

		private void listViewMounts_MouseDoubleClick(object sender, MouseEventArgs e){
			//this does mount only
			if(listViewMounts.SelectedIndex==-1) {
				return;
			}
			int idxMount = listViewMounts.SelectedIndex;
			MountDefSelected=_listMountDefs[idxMount];
			IsDialogOK=true;
		}

		private void butMount_Click(object sender,EventArgs e) {
			if(listViewMounts.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a mount first.");
				return;
			}
			int idxMount = listViewMounts.SelectedIndex;
			MountDefSelected=_listMountDefs[idxMount];
			IsDialogOK=true;
		}

		private void butMountAndAcquire_Click(object sender, EventArgs e){
			if(!ODBuild.IsTrial()
				&& !OpenDentalHelp.ODHelp.IsEncryptedKeyValid())//always true in debug
			{
				MsgBox.Show(this,"This feature requires an active support plan.");
				return;
			}
			if(listDevices.SelectedIndex==-1) {
				MsgBox.Show(this,"No device selected.  Set up imaging devices in Main Menu - Setup - Imaging - Devices.");
				return;
			}
			if(listViewMounts.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a mount first.");
				return;
			}
			int idxMount = listViewMounts.SelectedIndex;
			MountDefSelected=_listMountDefs[idxMount];
			int idxDevice = listDevices.SelectedIndex;
			ImagingDeviceSelected=_listImagingDevices[idxDevice];
			IsDialogOK=true;
		}

	}
}