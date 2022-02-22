using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenDentBusiness;

namespace OpenDental.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.
	public class Gen_Patient{

		public static void Draw(Direct2d d,Point origin,float scale){
			d.SaveDrawingState();
			d.Translate(origin.X,origin.Y);
			d.Scale(scale);
			d.BeginPath();
			d.BeginFigure(14.42f,8f,isFilled:true);
			d.AddArc(7.58f,8f,5.15f,5.15f,0f,false,true);
			d.AddBezier(5.51f,9.78f,5.5f,13.07f,5.5f,13.07f);
			d.AddLine(5.5f,19.38f);
			d.AddBezier(7.33f,22.54f,14.67f,22.54f,16.5f,19.38f);
			d.AddLine(16.5f,13.07f);
			d.AddBezier(16.5f,13.07f,16.49f,9.78f,14.42f,8f);
			d.EndFigure(isClosed:true);
			d.EndPath(isFilled:true,isOutline:false,colorFill:Color.FromArgb(119,35,27),colorOutline:Color.FromArgb(0,0,0),strokeWidth:1f);
			d.FillEllipse(Color.FromArgb(119,35,27),11f,4f,3.75f,3.75f);
			d.RestoreDrawingState();
		}

		public static string GetBase64(){
			return "iVBORw0KGgoAAAANSUhEUgAAABYAAAAWCAYAAADEtGw7AAAACXBIWXMAAAsSAAALEgHS3X78AAAA7ElEQVQ4je2VvQ3CMBBGH4geBkiRIgPABGQERkmZktJlRmCEsAFMQAZwkcIDwARBJxmBwH9Jg5D4St/d82f5zp4Nw0BIdZGtgAbI7VIPVEqba6guCK6LTGAdsHwL3WSjEHwetAt7BxS71oQKY+B8YiwKnqwYuJ8Yi4Ire1HvutmYV4uEo+audosVedutLrI1cAJa6Q6lTc+zBaVbdkCptOnGggW6jRg7K23KZLB1e4lAH9q4XPsuz+liTK4PvBoBduZ+bUD+4B8EB7+dlFwf+AAcE6CSI7kfiv15MlXy2MiIv0pGuFXayHvyKeAOAQBGSTfsIBQAAAAASUVORK5CYII=";
		}
	}
}