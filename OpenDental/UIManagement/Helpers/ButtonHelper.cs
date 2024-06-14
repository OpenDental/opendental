using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenDental.UIManagement {
	/*
	///<summary>This class is for internal use by UIManagement. Calls from OD proper should use Um.</summary>
	public class ButtonHelper{
		public static Button CreateButton(OpenDental.UI.Button button,UIManager uIManager){
			//Button contains a Grid with two columns and one row.  The two cells contain an image and a textBlock.
			//The Button already has an internal Border that I can control.
			Button buttonNew=new Button();
			Grid grid=new Grid();
			buttonNew.Content=grid;
			ColumnDefinition columnDefinitionAuto=new ColumnDefinition();
			columnDefinitionAuto.Width=new GridLength(0,GridUnitType.Auto);
			ColumnDefinition columnDefinitionStar=new ColumnDefinition();
			columnDefinitionStar.Width=new GridLength(1,GridUnitType.Star);
			switch(button.ImageAlign){
				case System.Drawing.ContentAlignment.MiddleLeft:
				case System.Drawing.ContentAlignment.MiddleCenter:
					grid.ColumnDefinitions.Add(columnDefinitionAuto);//if no image in first column, its width will be 0
					grid.ColumnDefinitions.Add(columnDefinitionStar);
					break;
				case System.Drawing.ContentAlignment.MiddleRight:
					grid.ColumnDefinitions.Add(columnDefinitionStar);
					grid.ColumnDefinitions.Add(columnDefinitionAuto);
					break;
				default:
					throw new Exception("ImageAlign value not supported yet.");
			}
			buttonNew.HorizontalContentAlignment=HorizontalAlignment.Stretch;
			buttonNew.VerticalContentAlignment=VerticalAlignment.Stretch;
			//ContainerVisual containerVisual=new ContainerVisual();
			//https://swharden.com/csdv/platforms/wpf-primitives/
			//https://www.codeproject.com/Tips/1089489/Vector-Icons-in-WPF
			//https://stackoverflow.com/questions/3526366/wpf-what-is-the-correct-way-of-using-svg-files-as-icons-in-wpf   toward the bottom shows path objects
			if(button.Image!=null){
				Image image=new Image();
				image.Margin=new Thickness(left:2,top:0,right:2,bottom:0);
				BitmapImage bitmapImage=new BitmapImage();
				using MemoryStream memoryStream = new MemoryStream();
				button.Image.Save(memoryStream,System.Drawing.Imaging.ImageFormat.Png);
				memoryStream.Position = 0;
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memoryStream;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				bitmapImage.Freeze();
				image.Source=bitmapImage;
				grid.Children.Add(image);
			}
			if(button.Icon!=UI.EnumIcons.None){
				//todo: switch from bitmap to vectors
				Image image=new Image();
				BitmapImage bitmapImage=UI.IconLibrary.DrawWpf(button.Icon);
				image.Source=bitmapImage;
				//image.Stretch=Stretch.None;//I thought this would prevent image from shrinking slightly, but it just makes it huge.
				image.Margin=new Thickness(left:1,top:0,right:1,bottom:0);
				switch(button.ImageAlign){
					case System.Drawing.ContentAlignment.MiddleLeft:
						Grid.SetColumn(image,0);
						break;
					case System.Drawing.ContentAlignment.MiddleCenter:
						Grid.SetColumn(image,1);//This is star col. Assumes no text.
						break;
					case System.Drawing.ContentAlignment.MiddleRight:
						Grid.SetColumn(image,1);
						break;
					default:
						throw new Exception("ImageAlign value not supported yet.");
				}
				grid.Children.Add(image);
			}
			TextBlock textBlock=new TextBlock();
			string txt=button.Text;
			int idxAmp=txt.IndexOf("&");
			if(idxAmp!=-1 && idxAmp<txt.Length-1){//&present and not last char. Example abcd_e. idxAmp=4, which is less than 6-1. idxLetter=5.
				int idxLetter=idxAmp+1;
				string letter=txt.Substring(idxLetter,1).ToUpper();
				KeyShortcut keyShortcut=new KeyShortcut();
				keyShortcut.Key_=(Key)Enum.Parse(typeof(Key),letter);
				keyShortcut.Modifiers=EnumShortCutMod.Alt;
				keyShortcut.Button=button;
				uIManager.ListKeyShortcuts.Add(keyShortcut);
				if(idxAmp>0){
					textBlock.Inlines.Add(txt.Substring(0,idxAmp));
				}
				Run run=new Run(txt.Substring(idxLetter,1));//don't change capitalization
				run.TextDecorations=TextDecorations.Underline;
				textBlock.Inlines.Add(run);
				if(idxLetter<txt.Length-1){//example abcd_ef, idxLetter=5, which is less than 7-1
					textBlock.Inlines.Add(txt.Substring(idxLetter+1));//skip the e since we already showed it as underline
				}
			}
			else{
				textBlock.Text=txt;
			}
			//WPF does not have a button.DialogResult property
			if(uIManager.FormODBase_.AcceptButton==button){
				KeyShortcut keyShortcut=new KeyShortcut();
				keyShortcut.Key_=Key.Enter;
				keyShortcut.Modifiers=EnumShortCutMod.None;
				keyShortcut.Button=button;
				uIManager.ListKeyShortcuts.Add(keyShortcut);
			}
			if(uIManager.FormODBase_.CancelButton==button){
				KeyShortcut keyShortcut=new KeyShortcut();
				keyShortcut.Key_=Key.Escape;
				keyShortcut.Modifiers=EnumShortCutMod.None;
				keyShortcut.Button=button;
				uIManager.ListKeyShortcuts.Add(keyShortcut);
			}
			//todo: text align if there's any demand for it later.
			textBlock.VerticalAlignment=VerticalAlignment.Center;
			textBlock.HorizontalAlignment=HorizontalAlignment.Center;
			switch(button.ImageAlign){
				case System.Drawing.ContentAlignment.MiddleLeft:
				case System.Drawing.ContentAlignment.MiddleCenter://assumes no image
					Grid.SetColumn(textBlock,1);
					break;
				case System.Drawing.ContentAlignment.MiddleRight:
					Grid.SetColumn(textBlock,0);
					break;
				default:
					throw new Exception("ImageAlign value not supported yet.");
			}
			grid.Children.Add(textBlock);
			Color colorStart=Color.FromRgb(255,255,255);
			Color colorEnd=Color.FromRgb(225,232,235);//#E1E8EB
			LinearGradientBrush linearGradientBrush=new LinearGradientBrush(colorStart,colorEnd,90);
			buttonNew.Background=linearGradientBrush;
			buttonNew.BorderThickness=new Thickness(1.2);
			buttonNew.BorderBrush=new SolidColorBrush(Color.FromRgb(28,81,128));
			Style styleBorder=new Style();
			styleBorder.TargetType=typeof(Border);
			Setter setter=new Setter();
			setter.Property=Border.CornerRadiusProperty;
			double radius=3;
			CornerRadius cornerRadius=new CornerRadius(radius);
			setter.Value=cornerRadius;
			styleBorder.Setters.Add(setter);
			//buttonNew.Style=styleBorder;//doesn't work because wrong TargetType
			buttonNew.Resources.Add(typeof(Border),styleBorder);
			//Remember that the lambdas below don't fire until needed.
			buttonNew.Click+=(sender,routedEventArgs)=>EventHelper.InvokeClick(button);
			return buttonNew;
		}
	}*/
}
