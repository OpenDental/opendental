using System;
using System.Drawing;

namespace OpenDental.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.

	public class IconSelector{
		public static void DrawVectors(Direct2d d,EnumIcons icon,Point origin,float scale){
			switch(icon){
				case EnumIcons.Account32:
					Gen_Account32.Draw(d,origin,scale);
					return;
				case EnumIcons.Acquire:
					Gen_Acquire.Draw(d,origin,scale);
					return;
				case EnumIcons.Add:
					Gen_Add.Draw(d,origin,scale);
					return;
				case EnumIcons.Appt32:
					Gen_Appt32.Draw(d,origin,scale);
					return;
				case EnumIcons.BreakAptX:
					Gen_BreakAptX.Draw(d,origin,scale);
					return;
				case EnumIcons.CommLog:
					Gen_CommLog.Draw(d,origin,scale);
					return;
				case EnumIcons.Complete:
					Gen_Complete.Draw(d,origin,scale);
					return;
				case EnumIcons.DeleteX:
					Gen_DeleteX.Draw(d,origin,scale);
					return;
				case EnumIcons.Email:
					Gen_Email.Draw(d,origin,scale);
					return;
				case EnumIcons.Family32:
					Gen_Family32.Draw(d,origin,scale);
					return;
				case EnumIcons.ImageSelectorDoc:
					Gen_ImageSelectorDoc.Draw(d,origin,scale);
					return;
				case EnumIcons.ImageSelectorFile:
					Gen_ImageSelectorFile.Draw(d,origin,scale);
					return;
				case EnumIcons.ImageSelectorFolder:
					Gen_ImageSelectorFolder.Draw(d,origin,scale);
					return;
				case EnumIcons.ImageSelectorFolderWeb:
					Gen_ImageSelectorFolderWeb.Draw(d,origin,scale);
					return;
				case EnumIcons.ImageSelectorMount:
					Gen_ImageSelectorMount.Draw(d,origin,scale);
					return;
				case EnumIcons.ImageSelectorPhoto:
					Gen_ImageSelectorPhoto.Draw(d,origin,scale);
					return;
				case EnumIcons.ImageSelectorXray:
					Gen_ImageSelectorXray.Draw(d,origin,scale);
					return;
				case EnumIcons.Imaging32:
					Gen_Imaging32.Draw(d,origin,scale);
					return;
				case EnumIcons.Manage32:
					Gen_Manage32.Draw(d,origin,scale);
					return;
				case EnumIcons.PatAdd:
					Gen_PatAdd.Draw(d,origin,scale);
					return;
				case EnumIcons.PatDelete:
					Gen_PatDelete.Draw(d,origin,scale);
					return;
				case EnumIcons.Patient:
					Gen_Patient.Draw(d,origin,scale);
					return;
				case EnumIcons.PatMoveFam:
					Gen_PatMoveFam.Draw(d,origin,scale);
					return;
				case EnumIcons.PatSelect:
					Gen_PatSelect.Draw(d,origin,scale);
					return;
				case EnumIcons.PatSetGuarantor:
					Gen_PatSetGuarantor.Draw(d,origin,scale);
					return;
				case EnumIcons.Recall:
					Gen_Recall.Draw(d,origin,scale);
					return;
				case EnumIcons.Text:
					Gen_Text.Draw(d,origin,scale);
					return;
				case EnumIcons.TreatPlan32:
					Gen_TreatPlan32.Draw(d,origin,scale);
					return;
				case EnumIcons.Video:
					Gen_Video.Draw(d,origin,scale);
					return;
				case EnumIcons.WebMail:
					Gen_WebMail.Draw(d,origin,scale);
					return;
				default:
					return;
			}
		}

		///<summary>This is used prior to vector transition, for future testing, and for the rare icon that will remain a bitmap.  All icons have a bitmap here.  Usually returns 22x22 or 32x32 version.</summary>
		public static string GetBase64(EnumIcons icon){
			switch(icon){
				case EnumIcons.Account32:
					return Gen_Account32.GetBase64();
				case EnumIcons.Acquire:
					return Gen_Acquire.GetBase64();
				case EnumIcons.Add:
					return Gen_Add.GetBase64();
				case EnumIcons.Appt32:
					return Gen_Appt32.GetBase64();
				case EnumIcons.ArrowLeft:
					return Gen_ArrowLeft.GetBase64();
				case EnumIcons.ArrowRight:
					return Gen_ArrowRight.GetBase64();
				case EnumIcons.BreakAptX:
					return Gen_BreakAptX.GetBase64();
				case EnumIcons.Chart32:
					return Gen_Chart32.GetBase64();
				case EnumIcons.Chart32G:
					return Gen_Chart32G.GetBase64();
				case EnumIcons.Chart32W:
					return Gen_Chart32W.GetBase64();
				case EnumIcons.ChartMed32:
					return Gen_ChartMed32.GetBase64();
				case EnumIcons.CommLog:
					return Gen_CommLog.GetBase64();
				case EnumIcons.Complete:
					return Gen_Complete.GetBase64();
				case EnumIcons.DeleteX:
					return Gen_DeleteX.GetBase64();
				case EnumIcons.Email:
					return Gen_Email.GetBase64();
				case EnumIcons.Family32:
					return Gen_Family32.GetBase64();
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
				case EnumIcons.Imaging32:
					return Gen_Imaging32.GetBase64();
				case EnumIcons.Manage32:
					return Gen_Manage32.GetBase64();
				case EnumIcons.PatAdd:
					return Gen_PatAdd.GetBase64();
				case EnumIcons.PatDelete:
					return Gen_PatDelete.GetBase64();
				case EnumIcons.Patient:
					return Gen_Patient.GetBase64();
				case EnumIcons.PatMoveFam:
					return Gen_PatMoveFam.GetBase64();
				case EnumIcons.PatSelect:
					return Gen_PatSelect.GetBase64();
				case EnumIcons.PatSetGuarantor:
					return Gen_PatSetGuarantor.GetBase64();
				case EnumIcons.Probe:
					return Gen_Probe.GetBase64();
				case EnumIcons.Recall:
					return Gen_Recall.GetBase64();
				case EnumIcons.Text:
					return Gen_Text.GetBase64();
				case EnumIcons.TreatPlan32:
					return Gen_TreatPlan32.GetBase64();
				case EnumIcons.TreatPlanMed32:
					return Gen_TreatPlanMed32.GetBase64();
				case EnumIcons.Video:
					return Gen_Video.GetBase64();
				case EnumIcons.WebMail:
					return Gen_WebMail.GetBase64();
				default:
					return "";
			}
		}
	}
}