using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenDentBusiness;

namespace OpenDental.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.
	public class Gen_PatSelect{

		public static void Draw(Direct2d d,Point origin,float scale){
			d.SaveDrawingState();
			d.Translate(origin.X,origin.Y);
			d.Scale(scale);
			d.BeginPath();
			d.BeginFigure(0f,11.75f,true);
			d.AddLine(2f,7.25f);
			d.AddLine(0f,2.75f);
			d.AddLine(12f,7.25f);
			d.AddLine(0f,11.75f);
			d.EndFigure();
			d.EndPath(isFilled:true,isOutline:false,colorFill:Color.FromArgb(77,77,166),colorOutline:Color.FromArgb(0,0,0),strokeWidth:1f);
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
			d.RestoreDrawingState();
		}

		public static string GetBase64(){
			return "iVBORw0KGgoAAAANSUhEUgAAABYAAAAWCAYAAADEtGw7AAAACXBIWXMAAAsSAAALEgHS3X78AAABTklEQVQ4je1VQW6DMBCcIB7QB1hKDn5An0AfkG2vuZEXNEff0t64NT8op/qK+EDTH+QBSEklHkBfQLXREtEWMObajISwvct4Z9deZnVdYwqMVg8ANq2lXVKUWTOZRGy02gF47DA9J0X5xIOAyN56ki56SBlbsSMEkBFZHnMUWZ6vTg7uxQj7KQDAoc8BvAA4ElneKPZR0QUm5oR/tWz3AF6JbEVk045UuRSd7efiEVmOejvg/NlOldGqz/9SvFAWDo4o5pIyjj6WTQ6/jxuAfTMJ5L35y3XBB4A1FyXPV7HRKhXSm6Qoo+bhOa+LHbPl8o2reOyQzg5p+5QYrZjg3aGOcReKREgBMyHb93wwpOyHXygS1lKYyvFBNJI48rrSRqvRzsEIn0m4Ev8j4nYXHPTzJeYG5CJne+z9zzNa8U3lDZrG04BvLbeCNCnK6htomnUuHowItQAAAABJRU5ErkJggg==";
		}
	}
}