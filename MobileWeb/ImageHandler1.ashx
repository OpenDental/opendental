<%@ WebHandler Language="C#" Class="ImageHandler1" %>

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Web;
using Microsoft.Web;
using OpenDentBusiness;

public class ImageHandler1 : ImageHandler {
    
    public ImageHandler1() {
        // Set caching settings and add image transformations here
        // EnableServerCache = true;
    }
    
    public override ImageInfo GenerateImage(NameValueCollection parameters) {
		return new ImageInfo(PIn.Bitmap(Appointments.GetMobileBitmap(DateTime.Today,0)));
		/*
		// Add image generation logic here and return an instance of ImageInfo
		Bitmap bit = new Bitmap(600,200);
		Graphics gra = Graphics.FromImage(bit);
		gra.Clear(Color.AliceBlue);
		//gra.DrawString(parameters["Hello"],new Font(FontFamily.GenericSansSerif,16),Brushes.Black,0,0);
		gra.DrawString("Hello",new Font(FontFamily.GenericSansSerif,16),Brushes.Black,0,0);
		
		return new ImageInfo(bit);
		*/
    }
}