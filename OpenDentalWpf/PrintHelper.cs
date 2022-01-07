using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace OpenDentalWpf {
	public class PrintHelper {
		///<summary>Adds a page to the document and returns a canvas to put all the page UIElements into.  The returned canvas is the size and position of the bounds passed in.  If no bounds are passed in, it gives a standard margin of 48 (1/2 inch).  Includes automatically adding the required PageContent and FixedPage without the user having to be bothered with it.  Also adds a master canvas onto which the child canvas is placed to handle margins.</summary>
		public static Canvas GetCanvas(FixedDocument document){
			Rect bounds=new Rect(48,48,document.DocumentPaginator.PageSize.Width-96,document.DocumentPaginator.PageSize.Height-96);
			return GetCanvas(document,bounds);
		}

		///<summary>Adds a page to the document and returns a canvas to put all the page UIElements into.  The returned canvas is the size and position of the bounds passed in.  If no bounds are passed in, it gives a standard margin of 50.  Includes automatically adding the required PageContent and FixedPage without the user having to be bothered with it.  Also adds a master canvas onto which the child canvas is placed to handle margins.</summary>
		public static Canvas GetCanvas(FixedDocument document,Rect bounds){
			FixedPage page=new FixedPage();
			page.Width=document.DocumentPaginator.PageSize.Width;
			page.Height=document.DocumentPaginator.PageSize.Height;
			Canvas canvas=new Canvas();
			canvas.Width=bounds.Width;
			canvas.Height=bounds.Height;
			Canvas canvasMain=new Canvas();
			canvasMain.Width=page.Width;
			canvasMain.Height=page.Height;
			canvasMain.Children.Add(canvas);
			Canvas.SetLeft(canvas,bounds.Left);
			Canvas.SetTop(canvas,bounds.Top);
			page.Children.Add(canvasMain);
			PageContent pageContent = new PageContent();
			pageContent.MaxWidth=1600;
			((IAddChild)pageContent).AddChild(page);
			document.Pages.Add(pageContent);
			return canvas;
		}

		

	}
}
