using System;

namespace WpfControls.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.

	public class IconSelector{
		

		///<summary>This is used prior to vector transition, for future testing, and for the rare icon that will remain a bitmap.  All icons have a bitmap here.  Usually returns 22x22 or 32x32 version.</summary>
		public static string GetBase64(EnumIcons icon){
			switch(icon){
				case EnumIcons.Add:
					return Gen_Add.GetBase64();
				case EnumIcons.ArrowLeft:
					return Gen_ArrowLeft.GetBase64();
				case EnumIcons.ArrowRight:
					return Gen_ArrowRight.GetBase64();
				case EnumIcons.CommLog:
					return Gen_CommLog.GetBase64();
				case EnumIcons.Complete:
					return Gen_Complete.GetBase64();
				case EnumIcons.DeleteX:
					return Gen_DeleteX.GetBase64();
				case EnumIcons.Email:
					return Gen_Email.GetBase64();
				case EnumIcons.ImageSelectorDoc:
					return Gen_ImageSelectorDoc.GetBase64();
				case EnumIcons.ImageSelectorFolder:
					return Gen_ImageSelectorFolder.GetBase64();
				case EnumIcons.Patient:
					return Gen_Patient.GetBase64();
				case EnumIcons.PatSelect:
					return Gen_PatSelect.GetBase64();
				case EnumIcons.Recall:
					return Gen_Recall.GetBase64();
				case EnumIcons.Text:
					return Gen_Text.GetBase64();
				default:
					return "";
			}
		}
	}
}