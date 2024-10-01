using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenDentBusiness;

namespace OpenDental.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.
	public class Gen_PatSetGuarantor{

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
			d.FillRectangle(Color.FromArgb(1,92,1),3f,4f,3f,10f);
			d.BeginPath();
			d.BeginFigure(9f,7f,true);
			d.AddLine(0f,7f);
			d.AddLine(4.5f,0f);
			d.AddLine(9f,7f);
			d.EndFigure();
			d.EndPath(isFilled:true,isOutline:false,colorFill:Color.FromArgb(1,92,1),colorOutline:Color.FromArgb(0,0,0),strokeWidth:1f);
			d.RestoreDrawingState();
		}

		public static string GetBase64(){
			return "iVBORw0KGgoAAAANSUhEUgAAABYAAAAWCAYAAADEtGw7AAAACXBIWXMAAAsSAAALEgHS3X78AAABPElEQVQ4je1V0WnDMBB9OfLfDCBIChqgGzTdIB3AkA3iT30mf/7UCAYPkGzQjNABDPWHB0gXkIvKyagqlqVA//pAGN89vTufTufFMAxwoIJWAN4BPJnG3BCBkmIHoPQYumr7y6gVbK0BrPk5CSWFBnAG8Oyts5Li6PaMGVNBOyY7vJrGXEJxJcUGwEck7mPV9h15JdABQbM9xCb2Nc7vSnHkEvhYs/0uEBW0BXCY2Hxgv49uJlDnMo4eVOi39QNwmuCe2I+lacxYMypo7D3TmEUkmOa2LAPbddSayfYXlBQ1i66qtt+6Zd+tnf0IL0g0YyWFFXhLiP+Sm3GZwPnm5QqHHTLJyxV+SOVlH14qlj5vpsWy8GcZ/wvfLfyZyssV3ieIW//+x6xIgZLCDhsbwA0eB/vztdOtrtr+9gU/JGGORSaOcQAAAABJRU5ErkJggg==";
		}
	}
}