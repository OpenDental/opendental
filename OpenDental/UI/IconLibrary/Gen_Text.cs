using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenDentBusiness;

namespace OpenDental.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.
	public class Gen_Text{

		public static void Draw(Direct2d d,Point origin,float scale){
			d.SaveDrawingState();
			d.Translate(origin.X,origin.Y);
			d.Scale(scale);
			d.BeginPath();
			d.BeginFigure(19.75f,22f,isFilled:true);
			d.AddLine(19f,18.32f);
			d.AddLine(20.5f,18.32f);
			d.AddArc(22f,16.84f,1.49f,1.49f,0f,false,false);
			d.AddLine(22f,9.47f);
			d.AddArc(20.5f,8f,1.49f,1.49f,0f,false,false);
			d.AddLine(8.5f,8f);
			d.AddArc(7f,9.47f,1.49f,1.49f,0f,false,false);
			d.AddLine(7f,16.84f);
			d.AddArc(8.5f,18.32f,1.49f,1.49f,0f,false,false);
			d.AddLine(14.5f,18.32f);
			d.EndFigure(isClosed:true);
			d.EndPath(isFilled:true,isOutline:false,colorFill:Color.FromArgb(0,113,188),colorOutline:Color.FromArgb(0,0,0),strokeWidth:1f);
			d.BeginPath();
			d.BeginFigure(3.55f,17f,isFilled:true);
			d.AddLine(4.4f,12.79f);
			d.AddLine(2.7f,12.79f);
			d.AddArc(1f,11.11f,1.69f,1.69f,0f,false,true);
			d.AddLine(1f,2.68f);
			d.AddArc(2.7f,1f,1.69f,1.69f,0f,false,true);
			d.AddLine(16.3f,1f);
			d.AddArc(18f,2.68f,1.69f,1.69f,0f,false,true);
			d.AddLine(18f,11.11f);
			d.AddArc(16.3f,12.79f,1.69f,1.69f,0f,false,true);
			d.AddLine(9.5f,12.79f);
			d.EndFigure(isClosed:true);
			d.EndPath(isFilled:true,isOutline:true,colorFill:Color.FromArgb(254,239,216),colorOutline:Color.FromArgb(0,113,188),strokeWidth:1.5f);
			d.RestoreDrawingState();
		}

		public static string GetBase64(){
			return "iVBORw0KGgoAAAANSUhEUgAAABYAAAAWCAYAAADEtGw7AAAACXBIWXMAAAsSAAALEgHS3X78AAABb0lEQVQ4je2UwU7CQBCG/61FDibECwkmJnLmhGcPNOkL6AsoPIHScBZ4gY0+gfAEyp0SSLyLb6AnSTyARA8N0DGz2ZJCPNgVbv7JNNNt/i+z250RRATh+Q0ADszVJ+k24m6BapcX6pm0Rce5lEiKfhrNaBqE7GvG4TZXytD+RVYUc6nEJQ9HM+G03xm+smOLH1ypCZTFvp92ahnRfqF/8FL2Blkl4fmk88q2Kr7b7s8bvAZoPX8ZAdjH/nVxS58CuI+v986zcPJplTcHUxVJFQ0hRw8hjlJ4fYiXyRyVzlhVY1vAPExGVuBIu7XeYyFrn9RLGZQ742i4GGkFnKr5n/MQe5ynd8RbsKADU/DyVgjPz0dQALfBggqmUKx1XhHAB4Azku4VSXcCoG3Ibcc7j0F5DVQi6ZaF5w8B7P9g5rXLdSCPaJLuzcoZJ5W+TS0AR9pbIeny+9+GEEm3r48wOjLO9UeijYRqtGr3QfGI8A0+q8TB0LVoOQAAAABJRU5ErkJggg==";
		}
	}
}