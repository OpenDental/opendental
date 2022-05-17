using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenDentBusiness;

namespace OpenDental.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.
	public class Gen_Add{

		public static void Draw(Direct2d d,Point origin,float scale){
			d.SaveDrawingState();
			d.Translate(origin.X,origin.Y);
			d.Scale(scale);
			d.FillRectangle(Color.FromArgb(255,255,255),8f,1f,3f,17f);
			d.FillRectangle(Color.FromArgb(255,255,255),1f,8f,17f,3f);
			d.FillRectangle(Color.FromArgb(128,128,128),9.5f,2.5f,3f,17f);
			d.FillRectangle(Color.FromArgb(128,128,128),2.5f,9.5f,17f,3f);
			d.FillRectangle(Color.FromArgb(64,73,158),9f,2f,3f,17f);
			d.FillRectangle(Color.FromArgb(64,73,158),2f,9f,17f,3f);
			d.RestoreDrawingState();
		}

		public static string GetBase64(){
			return "iVBORw0KGgoAAAANSUhEUgAAABYAAAAWCAYAAADEtGw7AAAACXBIWXMAAAsSAAALEgHS3X78AAAAo0lEQVQ4jWP8//8/Ay0ACxFqkG1mpKbBDI5e8+GWOJg/cqyvrz9ASA8TsS5AAg7EKCLHYKLAqMEYBv/Hg1GAvb19PQ61KACWQf4jJSmyAXJSpHZQwJMizGCicxSxAJ7zQN7AlvhBYdrYdZ8BSd1BBgYGXDkPLk5MIYQS/g7mjxrr6+sbCGkazSCDw2CykiJRBT1aUiRYyINdQ6s6jzaRx8DAAABT6DgLcwa1ywAAAABJRU5ErkJggg==";
		}
	}
}