using System;
using System.IO;
using System.Windows.Navigation;

namespace WpfControls.UI{
	//Jordan's Instructions to self for maintenance
	//This enum is manually maintained with no automation.  A similar list is stored in the database, which is filled by Adding in Icon Manager.  
	//1. Add enumeration here
	//2. Add in Icon Manager to db.
	//3. Generate individual with no images.
	//4. Add Gen_ files to project
	//5. Generate from main page.
	//6. Add images 
	public enum EnumIcons{
		None,
		Account32,
		Acquire,
		Add,
		Appt32,
		ArrowLeft,
		ArrowRight,
		BreakAptX,
		Chart32,
		Chart32G,
		Chart32W,
		ChartMed32,
		CommLog,
		Complete,
		DeleteX,
		Email,
		Family32,
		ImageSelectorDoc,
		ImageSelectorFile,
		ImageSelectorFolder,
		ImageSelectorFolderWeb,
		ImageSelectorMount,
		ImageSelectorPhoto,
		ImageSelectorXray,
		Imaging32,
		Manage32,
		PatAdd,
		PatDelete,
		Patient,
		PatMoveFam,
		PatSelect,
		PatSetGuarantor,
		Probe,
		Recall,
		Text,
		TreatPlan32,
		TreatPlanMed32,
		Video,
		WebMail
	}

	///<summary></summary>
	public partial class IconLibrary{
		
		public static System.Windows.Media.Imaging.BitmapImage DrawWpf(EnumIcons icon){
			//In the future, attach a series of Shapes (rectangles, paths, etc) to some container that's passed in.
			//For now, just always draw the image at its original size.
			int width=Width(icon);
			//wImage.Width=width;
			System.Windows.Media.Imaging.BitmapImage bitmapImage=new System.Windows.Media.Imaging.BitmapImage();
			string base64=IconSelector.GetBase64(icon);
			if(base64==""){
				return null;
			}
			byte[] byteArray=Convert.FromBase64String(base64);
			bitmapImage.BeginInit();
			bitmapImage.StreamSource=new MemoryStream(byteArray);
			//This can fail for a few bitmaps for a very small number of computers for unknown reasons
			try{
				bitmapImage.EndInit();
			}
			catch{
				bitmapImage=new System.Windows.Media.Imaging.BitmapImage();
			}
			//bitmapImage.Freeze();//Would makes the image available on any thread, but we don't need that.
			return bitmapImage;
			
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
				case EnumIcons.ArrowLeft:
				case EnumIcons.ArrowRight:
				case EnumIcons.ChartMed32:
				case EnumIcons.Chart32:
				case EnumIcons.Chart32G:
				case EnumIcons.Chart32W:
				case EnumIcons.Probe:
				case EnumIcons.TreatPlanMed32:
				case EnumIcons.ImageSelectorDoc:
				case EnumIcons.ImageSelectorPhoto:
				case EnumIcons.ImageSelectorXray:
					return false;
				default:
					return true;
/*
				case EnumIcons.Account32:
				case EnumIcons.Add:
				case EnumIcons.Appt32:
				case EnumIcons.BreakAptX:
				case EnumIcons.CommLog:
				case EnumIcons.Complete:
				case EnumIcons.DeleteX:
				case EnumIcons.Email:
				case EnumIcons.Family32:
				case EnumIcons.Imaging32:
				case EnumIcons.Manage32:
				case EnumIcons.PatAdd:
				case EnumIcons.Patient:
				case EnumIcons.PatMoveFam:
				case EnumIcons.PatSelect:
				case EnumIcons.Recall:
				case EnumIcons.Text:
				case EnumIcons.TreatPlan32:
				case EnumIcons.WebMail:
					return true;
				default:
					return false;*/
			}
		}

	}

		
}


//https://github.com/jingwood/d2dlib/issues/2