using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenDentBusiness;

namespace OpenDental.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.
	public class Gen_PatMoveFam{

		public static void Draw(Direct2d d,Point origin,float scale){
			d.SaveDrawingState();
			d.Translate(origin.X,origin.Y);
			d.Scale(scale);
			d.BeginPath();
			d.BeginFigure(8.92f,8f,isFilled:true);
			d.AddArc(2.08f,8f,5.15f,5.15f,0f,false,true);
			d.AddBezier(0f,9.78f,0f,13.07f,0f,13.07f);
			d.AddLine(0f,19.38f);
			d.AddBezier(1.83f,22.54f,9.17f,22.54f,11f,19.38f);
			d.AddLine(11f,13.07f);
			d.AddBezier(11f,13.07f,11f,9.78f,8.92f,8f);
			d.EndFigure(isClosed:true);
			d.EndPath(isFilled:true,isOutline:false,colorFill:Color.FromArgb(119,35,27),colorOutline:Color.FromArgb(0,0,0),strokeWidth:1f);
			d.FillEllipse(Color.FromArgb(119,35,27),5.5f,4f,3.75f,3.75f);
			d.BeginPath();
			d.BeginFigure(17f,11.5f,true);
			d.AddLine(17f,2.5f);
			d.AddLine(22f,7f);
			d.AddLine(17f,11.5f);
			d.EndFigure();
			d.EndPath(isFilled:true,isOutline:false,colorFill:Color.FromArgb(1,92,1),colorOutline:Color.FromArgb(0,0,0),strokeWidth:1f);
			d.FillRectangle(Color.FromArgb(1,92,1),11f,5.5f,7f,3f);
			d.RestoreDrawingState();
		}

		public static string GetBase64(){
			return "iVBORw0KGgoAAAANSUhEUgAAABYAAAAWCAYAAADEtGw7AAAACXBIWXMAAAsSAAALEgHS3X78AAABM0lEQVQ4je2UQU7DMBBFP6PsYY8XSPgAHCHcoBzAUjkB7PCuzc47egNa5QBwA3qEHsALFj4AXMBBE5yQRk5iqq4q/iajzMzTeMaes6qq0EhLMQPwiF+tjHVvOEAtWEuxAvAQQRTGuuVf0RSgVwNQ1iL4hyGKZlEwgNHEBP8rKZrHwMfQCylq55OF78cEuPaToqleP5OiG1/6eXd4nLSIBLfDI0VVxB/TJuv85Fux6183ANtD2pSFatcAcgBLY13eOLUUPJCdlmJrrGO7GOB0T7qpW/F0fcmg94Qibo110eo7LSp86X/a1jv6mKbi7hsoQivy8fhWY3F3vvR7T58rPk8ED8b1oTjyA/kHnxL4KzE2Na4F8w6YSmL/3iKfUr02tRQXIZFfF9uNPsN2Wxvr2E4TgG8YWV4qv+K1QAAAAABJRU5ErkJggg==";
		}
	}
}