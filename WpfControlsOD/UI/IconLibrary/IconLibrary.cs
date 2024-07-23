using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace WpfControls.UI{
	/*
Jordan's Instructions to self for maintenance
This enum is manually maintained with no automation.  A similar list is stored in the database, which is filled by Adding in Icon Manager.  
To add an entirely new icon:
1. Add enumeration here and to IsWPF at the bottom.
2. Add folder in RawFiles. Add bitmap. Add to GIT.
2. Run. 
	Click Add Icon. Add image file. Generate.
	Click Gen Selector.
3. Add Gen_ file to project
	*/
	public enum EnumIcons{
		None,
		Account32,
		Acquire,
		Add,
		Appt32,
		ArrowLeft,
		ArrowRight,
		ArrowsAll,
		BreakAptX,
		Chart32,
		Chart32G,
		Chart32W,
		ChartMed32,
		CommLog,
		Complete,
		Copy,
		Crop,
		DeleteX,
		Email,
		Export,
		Family32,
		Flip,
		Hand,
		ImageSelectorDoc,
		ImageSelectorFile,
		ImageSelectorFolder,
		ImageSelectorFolderWeb,
		ImageSelectorMount,
		ImageSelectorPhoto,
		ImageSelectorXray,
		Imaging32,
		Import,
		Info,
		Manage32,
		Paste,
		PatAdd,
		PatDelete,
		Patient,
		PatMoveFam,
		PatSelect,
		PatSetGuarantor,
		Print,
		Probe,
		Recall,
		RotateL,
		RotateR,
		ScanMulti,
		ScanPhoto,
		ScanXray,
		Text,
		TreatPlan32,
		TreatPlanMed32,
		Video,
		WebMail
	}

	///<summary></summary>
	public partial class IconLibrary{
		public static void DrawWpf(EnumIcons enumIcons,System.Windows.Controls.Grid grid){
			//The incoming grid will already have been set to the correct width. It's our job to match that width.
			//Any margin will be set by the caller on the grid rather than on any canvas or Image in here.
			//The canvas gets set to fit the grid
			if(IsVector(enumIcons)){
				//attach a series of Shapes (rectangles, paths, etc) 
				double width=Width(enumIcons);
				//grid.ActualWidth is still 0
				double scale=grid.Width/width;//usually 22/22, but maybe 16/22 in treeview for example
				Canvas canvas=new Canvas();
				canvas.LayoutTransform=new ScaleTransform(scale,scale);
				canvas.Width=grid.Width;
				canvas.Height=grid.Width;//we only support square icons
				grid.Children.Add(canvas);
				DrawSvg drawSvg=new DrawSvg();
				drawSvg.CanvasCur=canvas;
				IconSelector.DrawVectors(drawSvg,enumIcons,scale);
				return;
			}
			//For now, just always draw the image at its original size.
			BitmapImage bitmapImage=new BitmapImage();
			string base64=IconSelector.GetBase64(enumIcons);
			if(base64==""){
				return;
			}
			byte[] byteArray=Convert.FromBase64String(base64);
			bitmapImage.BeginInit();
			bitmapImage.StreamSource=new MemoryStream(byteArray);
			//This can fail for a few bitmaps for a very small number of computers for unknown reasons
			try{
				bitmapImage.EndInit();
			}
			catch{
				bitmapImage=new BitmapImage();
			}
			//bitmapImage.Freeze();//Would makes the image available on any thread, but we don't need that.
			Image image=new Image();
			image.Source=bitmapImage;
			//width is typically 22, but treeView sets it to 16 for example.
			image.Width=grid.Width;//we currently only support square icons
			image.Height=grid.Width;
			grid.Children.Add(image);
		}

		///<summary>32 or 22</summary>
		public static int Width(EnumIcons icon){
			switch(icon){
				case EnumIcons.Account32:
				case EnumIcons.Appt32:
				case EnumIcons.Family32:
				case EnumIcons.Imaging32:
				case EnumIcons.Manage32:
				case EnumIcons.TreatPlan32:
					return 32;
				default:
					return 22;
			}
		}

		public static bool IsVector(EnumIcons icon){
			switch(icon){
				case EnumIcons.Acquire:
				case EnumIcons.ImageSelectorFile:
				case EnumIcons.ImageSelectorFolder:
				case EnumIcons.ImageSelectorFolderWeb:
				case EnumIcons.Patient:
					return true;
				default:
					return false;
			}
		}

		///<summary>Only some icons have been moved over to WPF. This keeps track of which ones so that the IconGenerator can work properly.</summary>
		public static bool IsWPF(EnumIcons icon){
			switch(icon){
				case EnumIcons.Acquire:
				case EnumIcons.Add:
				case EnumIcons.ArrowLeft:
				case EnumIcons.ArrowRight:
				case EnumIcons.ArrowsAll:
				case EnumIcons.CommLog:
				case EnumIcons.Complete:
				case EnumIcons.Copy:
				case EnumIcons.Crop:
				case EnumIcons.DeleteX:
				case EnumIcons.Email:
				case EnumIcons.Export:
				case EnumIcons.Flip:
				case EnumIcons.Hand:
				case EnumIcons.ImageSelectorDoc:
				case EnumIcons.ImageSelectorFile:
				case EnumIcons.ImageSelectorFolder:
				case EnumIcons.ImageSelectorFolderWeb:
				case EnumIcons.ImageSelectorMount:
				case EnumIcons.ImageSelectorPhoto:
				case EnumIcons.ImageSelectorXray:
				case EnumIcons.Import:
				case EnumIcons.Info:
				case EnumIcons.Paste:
				case EnumIcons.Patient:
				case EnumIcons.PatSelect:
				case EnumIcons.Print:
				case EnumIcons.Recall:
				case EnumIcons.RotateL:
				case EnumIcons.RotateR:
				case EnumIcons.ScanMulti:
				case EnumIcons.ScanPhoto:
				case EnumIcons.ScanXray:
				case EnumIcons.Text:
				case EnumIcons.Video:
					return true;
				default:
					return false;
			}
		}

	}

		
}


//https://github.com/jingwood/d2dlib/issues/2