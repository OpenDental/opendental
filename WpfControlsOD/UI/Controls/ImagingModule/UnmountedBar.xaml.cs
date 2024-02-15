using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CodeBase;
using OpenDentBusiness;
using OpenDental.Drawing;
using OpenDental.UI;
using OpenDental;//even though they are in this project

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.
*/
	///<summary></summary>
	public partial class UnmountedBar : UserControl{
		#region Fields - Private
		///<summary>This list just holds secondary references to the various objects, including bitmaps.  This class is not responsible for disposing of bitmaps.</summary>
		private List<UnmountedObj> _listUnmountedObjects;
		private int _selectedIndex=-1;
		#endregion Fields - Private

		#region Constructor
		public UnmountedBar(){
			InitializeComponent();
			stackPanel.MouseLeftButtonDown+=StackPanel_MouseLeftButtonDown;
		}
		#endregion Constructor

		#region Events - Raise
		///<summary>Occurs when the user clicks the Close button.</summary>
		[Category("OD")]
		[Description("Occurs when the user clicks the Close button.")]
		public event EventHandler EventClose;

		///<summary>Occurs when the parent needs to be refreshed.</summary>
		[Category("OD")]
		[Description("Occurs when the parent needs to be refreshed.")]
		public event EventHandler EventRefreshParent;

		///<summary>Occurs when user clicks the Remount button.</summary>
		[Category("OD")]
		[Description("Occurs when user clicks the Remount button.")]
		public event EventHandler<UnmountedObj> EventRemount;

		///<summary>Occurs when user clicks the Retake button.</summary>
		[Category("OD")]
		[Description("Occurs when user clicks the Retake button.")]
		public event EventHandler EventRetake;
		#endregion Events - Raise

		#region Properties
		
		#endregion Properties

		#region Methods public
		public void AddObject(UnmountedObj unmountedObj){
			_listUnmountedObjects.Add(unmountedObj);
			Border border=new Border();
			border.Width=180;
			border.Height=180;
			Image image=new Image();
			image.Source=unmountedObj.BitmapImage_;
			image.Stretch=Stretch.Uniform;
			border.Child=image;
			border.BorderThickness=new Thickness(2);
			border.BorderBrush=new SolidColorBrush(ColorOD.Gray_Wpf(100));
			border.MouseLeftButtonDown+=border_MouseLeftButtonDown;
			stackPanel.Children.Add(border);
			SetSelectedIndex(-1);
		}

		public void SetColorBack(Color color){
			stackPanel.Background=new SolidColorBrush(color);
		}

		public void SetObjects(List<UnmountedObj> listUnmountedObjs){
			_listUnmountedObjects=listUnmountedObjs;
			stackPanel.Children.Clear();
			for(int i=0;i<listUnmountedObjs.Count;i++){
				Border border=new Border();
				border.Width=180;
				//If scrollbar is present, then lower 20px will be cut off. No big deal.
				border.Height=180;
				Image image=new Image();
				image.Source=listUnmountedObjs[i].BitmapImage_;
				image.Stretch=Stretch.Uniform;
				border.Child=image;
				border.BorderThickness=new Thickness(2);
				border.BorderBrush=new SolidColorBrush(ColorOD.Gray_Wpf(100));
				border.MouseLeftButtonDown+=border_MouseLeftButtonDown;
				stackPanel.Children.Add(border);
			}
			SetSelectedIndex(-1);
		}
		#endregion Methods public

		#region Methods private
		///<summary>Yes, -1 is valid.</summary>
		private void SetSelectedIndex(int idx){
			_selectedIndex=idx;
			for(int i=0;i<stackPanel.Children.Count;i++){
				Border border=(Border)stackPanel.Children[i];
				if(i==idx){
					border.BorderBrush=Brushes.Orange;
					continue;
				}
				border.BorderBrush=new SolidColorBrush(ColorOD.Gray_Wpf(100));
			}
		}
		#endregion Methods private

		#region Methods private EventHandlers
		private void border_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			Border border=(Border)sender;
			int idx=stackPanel.Children.IndexOf(border);
			SetSelectedIndex(idx);
		}

		private void butClose_Click(object sender, EventArgs e){
			if(_listUnmountedObjects.Count>0){
				OpenDental.MsgBox.Show(this,"Cannot close the unmounted bar as long as it still contains images.");
				return;
			}
			EventClose?.Invoke(this,new EventArgs());
		}

		private void butDelete_Click(object sender, EventArgs e){
			if(_listUnmountedObjects.Count==0){
				OpenDental.MsgBox.Show(this,"No unmounted images to delete.");
				return;
			}
			if(_selectedIndex==-1){
				if(_listUnmountedObjects.Count>1){
					OpenDental.MsgBox.Show(this,"Please select an an image below first.");
					return;
				}
				SetSelectedIndex(0);
			}
			if(!OpenDental.MsgBox.Show(this,OpenDental.MsgBoxButtons.OKCancel,"Permanently delete the image selected below?")){
				return;
			}
			if(_listUnmountedObjects[_selectedIndex].Document_!=null){
				Documents.Delete(_listUnmountedObjects[_selectedIndex].Document_);
			}
			MountItems.Delete(_listUnmountedObjects[_selectedIndex].MountItem_);
			EventRefreshParent?.Invoke(this,new EventArgs());
		}

		private void butRemount_Click(object sender, EventArgs e){
			if(_listUnmountedObjects.Count==0){
				OpenDental.MsgBox.Show(this,"No unmounted images to remount.");
				return;
			}
			if(_selectedIndex==-1){
				if(_listUnmountedObjects.Count>1){
					OpenDental.MsgBox.Show(this,"Please select an an image below to remount.");
					return;
				}
				SetSelectedIndex(0);
			}
			EventRemount?.Invoke(this,_listUnmountedObjects[_selectedIndex]);
		}

		private void butRetake_Click(object sender, EventArgs e){
			EventRetake?.Invoke(this,new EventArgs());
		}

		private void StackPanel_MouseLeftButtonDown(object sender,MouseButtonEventArgs e) {
			HitTestResult hitTestResult=VisualTreeHelper.HitTest(this,Mouse.GetPosition(this));
			if(hitTestResult==null){
				return;
			}
			if(hitTestResult.VisualHit is Border){
				return;
			}
			if(hitTestResult.VisualHit is Image){
				return;
			}
			//not on any item
			SetSelectedIndex(-1);
		}
		#endregion Methods private EventHandlers

	}

	
	///<summary>This is a container that stores a mountItem, document, and bitmap.  MountItem must always be valid, while the other values can be null.</summary>
	public class UnmountedObj{
		public MountItem MountItem_;
		public Document Document_;
		public BitmapImage BitmapImage_;

		public void SetBitmap(System.Drawing.Bitmap bitmap){
			using MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
			memoryStream.Position = 0;
			BitmapImage_=new BitmapImage();
			BitmapImage_.BeginInit();
			BitmapImage_.StreamSource = memoryStream;
			BitmapImage_.CacheOption = BitmapCacheOption.OnLoad;//makes it load into memory during EndInit
			BitmapImage_.EndInit();
			BitmapImage_.Freeze(); //for use in another thread
		}
	}

}
