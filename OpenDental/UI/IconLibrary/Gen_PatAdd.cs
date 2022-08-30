using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenDentBusiness;

namespace OpenDental.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.
	public class Gen_PatAdd{

		public static void Draw(Direct2d d,Point origin,float scale){
			d.SaveDrawingState();
			d.Translate(origin.X,origin.Y);
			d.Scale(scale);
			d.BeginPath();
			d.BeginFigure(19.92f,8f,isFilled:true);
			d.AddArc(13.08f,8f,5.15f,5.15f,0f,false,true);
			d.AddBezier(11f,9.78f,11f,13.07f,11f,13.07f);
			d.AddLine(11f,19.38f);
			d.AddBezier(12.83f,22.54f,20.17f,22.54f,22f,19.38f);
			d.AddLine(22f,13.07f);
			d.AddBezier(22f,13.07f,22f,9.78f,19.92f,8f);
			d.EndFigure(isClosed:true);
			d.EndPath(isFilled:true,isOutline:false,colorFill:Color.FromArgb(119,35,27),colorOutline:Color.FromArgb(0,0,0),strokeWidth:1f);
			d.FillEllipse(Color.FromArgb(119,35,27),16.5f,4f,3.75f,3.75f);
			d.FillRectangle(Color.FromArgb(4,4,199),4f,2f,2f,10f);
			d.FillRectangle(Color.FromArgb(4,4,199),0f,6f,10f,2f);
			d.RestoreDrawingState();
		}

		public static string GetBase64(){
			return "iVBORw0KGgoAAAANSUhEUgAAABYAAAAWCAYAAADEtGw7AAAACXBIWXMAAAsSAAALEgHS3X78AAAA9UlEQVQ4je2UsQ3CMBBFf1CoYQAXQfIQlGEDRsgGpHRH6FwyQkZgA+igZABLUHiAUKcIuihBEQRydgUSv4rvvp4d3/mCqqrgIyXFEkDaCW21sbt24QVWUmwBrHpSG21s1gsej091oCznwRtoBODyYd+ZNvY6cj4uEHHyPmCWfMBXTj4IwyOret07V1JQgdY9tkfxQsfTtqKuOD+3G4BDu/DpihxADCDTxuadeEIxgmtjEyewkoKAe8YfLVyLlzI8tc8VHHN9L8V7d7eNJkzw5KseyB/8o+Ab1+cKpnkwBKf866wYkpJi2mxAr5C+WxXNdMu1scUdDolWXncokbUAAAAASUVORK5CYII=";
		}
	}
}