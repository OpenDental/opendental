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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CodeBase;

namespace OpenDental.UIManagement {
	/*
	///<summary></summary>
	public class PictureBoxHelper{
		public static Border CreateImageControl(System.Windows.Forms.PictureBox pictureBox){
			//Border-Image
			Border border=new Border();
			Image image=new Image();//This is an image control
			if(pictureBox.Image!=null){
				BitmapImage bitmapImage=new BitmapImage();
				using MemoryStream memoryStream = new MemoryStream();
				pictureBox.Image.Save(memoryStream,System.Drawing.Imaging.ImageFormat.Png);
				memoryStream.Position = 0;
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memoryStream;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				bitmapImage.Freeze();
				image.Source=bitmapImage;
			}
			if(pictureBox.BorderStyle==System.Windows.Forms.BorderStyle.FixedSingle
				|| pictureBox.BorderStyle==System.Windows.Forms.BorderStyle.Fixed3D)
			{
				border.BorderBrush=Brushes.DarkGray;
				border.BorderThickness=new Thickness(1);
			}
			if(pictureBox.SizeMode==System.Windows.Forms.PictureBoxSizeMode.Normal){
				image.Stretch=Stretch.None;
			}
			else if(pictureBox.SizeMode==System.Windows.Forms.PictureBoxSizeMode.Zoom){
				image.Stretch=Stretch.Uniform;
			}
			else{
				throw new Exception("Size mode not supported:"+pictureBox.SizeMode.ToString());
			}
			border.Child=image;
			return border;
		}

		public static System.Drawing.Image ConvertFromWpf(BitmapImage bitmapImage){
			using MemoryStream memoryStream=new MemoryStream();
			BitmapEncoder bitmapEncoder=new PngBitmapEncoder();
			bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
			bitmapEncoder.Save(memoryStream);
			System.Drawing.Bitmap bitmap=new System.Drawing.Bitmap(memoryStream);
			return bitmap;
		}
		
		public static BitmapImage ConvertBitmapToWpf(System.Drawing.Bitmap bitmap){
			BitmapImage bitmapImage=new BitmapImage();
			using MemoryStream memoryStream = new MemoryStream();
			//todo: support for filetypes other than png
			bitmap.Save(memoryStream,System.Drawing.Imaging.ImageFormat.Png);
			memoryStream.Position = 0;
			bitmapImage.BeginInit();
			bitmapImage.StreamSource = memoryStream;
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			return bitmapImage;
		}
	}*/
}
