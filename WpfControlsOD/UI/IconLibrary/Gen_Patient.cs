using System;
using System.Windows.Media;
using OpenDentBusiness;

namespace WpfControls.UI{
	//---------------This file is generated automatically from the IconOrganizer project.  Do not edit manually.
	public class Gen_Patient{

		public static void Draw(DrawSvg drawSvg,double scale){
			drawSvg.BeginPath();
			drawSvg.BeginFigure(14.42,8,isFilled:true);
			drawSvg.AddArc(7.58,8,5.15,5.15,0,false,true);
			drawSvg.AddBezier(5.51,9.78,5.5,13.07,5.5,13.07);
			drawSvg.AddLine(5.5,19.38);
			drawSvg.AddBezier(7.33,22.54,14.67,22.54,16.5,19.38);
			drawSvg.AddLine(16.5,13.07);
			drawSvg.AddBezier(16.5,13.07,16.49,9.78,14.42,8);
			drawSvg.EndFigure(isClosed:true);
			drawSvg.EndPath(isFilled:true,isOutline:false,colorFill:Color.FromRgb(119,35,27),colorOutline:Color.FromRgb(0,0,0),strokeWidth:1);
			drawSvg.FillEllipse(Color.FromRgb(119,35,27),11,4,3.75,3.75,scale);
		}

		public static string GetBase64(){
			return "iVBORw0KGgoAAAANSUhEUgAAABYAAAAWCAYAAADEtGw7AAAACXBIWXMAAAsSAAALEgHS3X78AAAA7ElEQVQ4je2VvQ3CMBBGH4geBkiRIgPABGQERkmZktJlRmCEsAFMQAZwkcIDwARBJxmBwH9Jg5D4St/d82f5zp4Nw0BIdZGtgAbI7VIPVEqba6guCK6LTGAdsHwL3WSjEHwetAt7BxS71oQKY+B8YiwKnqwYuJ8Yi4Ire1HvutmYV4uEo+audosVedutLrI1cAJa6Q6lTc+zBaVbdkCptOnGggW6jRg7K23KZLB1e4lAH9q4XPsuz+liTK4PvBoBduZ+bUD+4B8EB7+dlFwf+AAcE6CSI7kfiv15MlXy2MiIv0pGuFXayHvyKeAOAQBGSTfsIBQAAAAASUVORK5CYII=";
		}
	}
}