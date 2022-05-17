using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenDentBusiness;

namespace OpenDental.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.
	public class Gen_Video{

		public static void Draw(Direct2d d,Point origin,float scale){
			d.SaveDrawingState();
			d.Translate(origin.X,origin.Y);
			d.Scale(scale);
			d.BeginPath();
			d.BeginFigure(12.57f,11.56f,isFilled:true);
			d.AddLine(19.69f,15.82f);
			d.AddArc(21.63f,14.72f,1.28f,1.28f,0f,false,false);
			d.AddLine(21.63f,7.28f);
			d.AddArc(19.69f,6.18f,1.29f,1.29f,0f,false,false);
			d.AddLine(12.57f,10.44f);
			d.AddArc(12.57f,11.56f,0.66f,0.66f,0f,false,false);
			d.EndFigure(isClosed:true);
			d.EndPath(isFilled:true,isOutline:false,colorFill:Color.FromArgb(68,59,44),colorOutline:Color.FromArgb(0,0,0),strokeWidth:1f);
			d.FillRoundedRectangle(Color.FromArgb(68,59,44),0.37f,5f,16.26f,12f,3.01f);
			d.RestoreDrawingState();
		}

		public static string GetBase64(){
			return "iVBORw0KGgoAAAANSUhEUgAAABYAAAAWCAYAAADEtGw7AAAACXBIWXMAAAsSAAALEgHS3X78AAAAlUlEQVQ4jWP8//8/Ay0AE01MHTUYGbCA2K42uhMYGBjyydA/cfeRywXYJBhdrHUSGBgY5lPguMTdRy4vcLXRDWBgYAA5cAGIBhl8gIGBwZ4Cg08yMDD8QDPjIAsFBsKAOTbB0XQ8wg0GJbeD2AxeQKHBM3YfuezAwMAQyMDA8BCUGxkYGALA5TFNsvRoQT9EDWZgYAAAGsEm5/eBFBsAAAAASUVORK5CYII=";
		}
	}
}