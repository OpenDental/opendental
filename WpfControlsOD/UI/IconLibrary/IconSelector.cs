using System;
using System.Windows.Controls;

namespace WpfControls.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.

	public class IconSelector{
		public static void DrawVectors(DrawSvg drawSvg,EnumIcons enumIcons,double scale){
			switch(enumIcons){
				case EnumIcons.Acquire:
					Gen_Acquire.Draw(drawSvg,scale);
					return;
				case EnumIcons.ImageSelectorFile:
					Gen_ImageSelectorFile.Draw(drawSvg,scale);
					return;
				case EnumIcons.ImageSelectorFolder:
					Gen_ImageSelectorFolder.Draw(drawSvg,scale);
					return;
				case EnumIcons.ImageSelectorFolderWeb:
					Gen_ImageSelectorFolderWeb.Draw(drawSvg,scale);
					return;
				case EnumIcons.Patient:
					Gen_Patient.Draw(drawSvg,scale);
					return;
				default:
					return;
			}
		}

		///<summary>This is used prior to vector transition, for future testing, and for the rare icon that will remain a bitmap.  All icons have a bitmap here.  Usually returns 22x22 or 32x32 version.</summary>
		public static string GetBase64(EnumIcons icon){
			switch(icon){
				case EnumIcons.Acquire:
					return Gen_Acquire.GetBase64();
				case EnumIcons.Add:
					return Gen_Add.GetBase64();
				case EnumIcons.ArrowLeft:
					return Gen_ArrowLeft.GetBase64();
				case EnumIcons.ArrowRight:
					return Gen_ArrowRight.GetBase64();
				case EnumIcons.ArrowsAll:
					return Gen_ArrowsAll.GetBase64();
				case EnumIcons.CommLog:
					return Gen_CommLog.GetBase64();
				case EnumIcons.Complete:
					return Gen_Complete.GetBase64();
				case EnumIcons.Copy:
					return Gen_Copy.GetBase64();
				case EnumIcons.Crop:
					return Gen_Crop.GetBase64();
				case EnumIcons.DeleteX:
					return Gen_DeleteX.GetBase64();
				case EnumIcons.Email:
					return Gen_Email.GetBase64();
				case EnumIcons.Export:
					return Gen_Export.GetBase64();
				case EnumIcons.Flip:
					return Gen_Flip.GetBase64();
				case EnumIcons.Hand:
					return Gen_Hand.GetBase64();
				case EnumIcons.ImageSelectorDoc:
					return Gen_ImageSelectorDoc.GetBase64();
				case EnumIcons.ImageSelectorFile:
					return Gen_ImageSelectorFile.GetBase64();
				case EnumIcons.ImageSelectorFolder:
					return Gen_ImageSelectorFolder.GetBase64();
				case EnumIcons.ImageSelectorFolderWeb:
					return Gen_ImageSelectorFolderWeb.GetBase64();
				case EnumIcons.ImageSelectorMount:
					return Gen_ImageSelectorMount.GetBase64();
				case EnumIcons.ImageSelectorPhoto:
					return Gen_ImageSelectorPhoto.GetBase64();
				case EnumIcons.ImageSelectorXray:
					return Gen_ImageSelectorXray.GetBase64();
				case EnumIcons.Import:
					return Gen_Import.GetBase64();
				case EnumIcons.Info:
					return Gen_Info.GetBase64();
				case EnumIcons.Paste:
					return Gen_Paste.GetBase64();
				case EnumIcons.Patient:
					return Gen_Patient.GetBase64();
				case EnumIcons.PatSelect:
					return Gen_PatSelect.GetBase64();
				case EnumIcons.Print:
					return Gen_Print.GetBase64();
				case EnumIcons.Recall:
					return Gen_Recall.GetBase64();
				case EnumIcons.RotateL:
					return Gen_RotateL.GetBase64();
				case EnumIcons.RotateR:
					return Gen_RotateR.GetBase64();
				case EnumIcons.ScanMulti:
					return Gen_ScanMulti.GetBase64();
				case EnumIcons.ScanPhoto:
					return Gen_ScanPhoto.GetBase64();
				case EnumIcons.ScanXray:
					return Gen_ScanXray.GetBase64();
				case EnumIcons.Text:
					return Gen_Text.GetBase64();
				case EnumIcons.Video:
					return Gen_Video.GetBase64();
				default:
					return "";
			}
		}
	}
}