﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CodeBase {

	///<summary></summary>
	public class ODClipboard {

		///<summary>Clears the clipboard.  For ODCloud this will set the clipboard to an empty string instead, since the clear function doesn't work for ODCloud.  Also, for ODCloud
		///this will use System.Windows.Clipboard since System.Windows.Forms.Clipboard causes heap corruption and crashes OD with ntdll.dll errors in ODCloud.</summary>
		public static void Clear() {
			if(ODBuild.IsWeb()) {
				System.Windows.Clipboard.SetText(string.Empty);//setting text so that the browser clipboard will match the local workstation clipboard
				ODCloudClient.SendDataToBrowser(string.Empty,(int)ODCloudClient.BrowserAction.SetClipboard);
			}
			else {
				Clipboard.Clear();
			}
		}

		///<summary>Writes the text to the user's clipboard.  For ODCloud this will use System.Windows.Clipboard since System.Windows.Forms.Clipboard causes heap corruption and
		///crashes OD with ntdll.dll errors in ODCloud.</summary>
		public static void SetClipboard(string text) {
			if(string.IsNullOrEmpty(text)) {
				return;
			}
			if(ODBuild.IsWeb()) {
				System.Windows.Clipboard.SetText(text);//setting text so that the browser clipboard will match the local workstation clipboard.
				ODCloudClient.SendDataToBrowser(text,(int)ODCloudClient.BrowserAction.SetClipboard);
			}
			else {
				Clipboard.SetText(text);
			}
		}

		///<summary>Gets the contents of the user's clipboard as text.</summary>
		public static string GetText() {
			if(ODBuild.IsWeb()) {
				return ODCloudClient.SendToBrowserSynchronously("",ODCloudClient.BrowserAction.GetClipboardText);
			}
			return Clipboard.GetText();
		}


		///<summary>Gets the contents of the user's clipboard as an image. Returns null if the clipboard does not contain an image.</summary>
		public static Bitmap GetImage() {
			if(ODBuild.IsWeb()) {
				string base64=ODCloudClient.SendToBrowserSynchronously("",ODCloudClient.BrowserAction.GetClipboardImage,timeoutSecs:30);
				if(base64.IsNullOrEmpty()) {
					return null;
				}
				byte[] rawData=Convert.FromBase64String(base64);
				MemoryStream stream=new MemoryStream(rawData);
				Bitmap image=new Bitmap(stream);
				return image;
			}
			if(!Clipboard.ContainsImage()) {
				return null;
			}
			IDataObject iDataObject=null;
			try{
				iDataObject=Clipboard.GetDataObject();
			}
			catch {
				//rare, but can happen
				return null;
			}
			if(iDataObject is null) {
				return null;
			}
			Bitmap bitmapPaste=null;
			if(iDataObject.GetDataPresent(DataFormats.Bitmap)) {
				bitmapPaste=(Bitmap)iDataObject.GetData(DataFormats.Bitmap);
			}
			return bitmapPaste;
		}

		///<summary>
		///Gets the contents of the user's clipboard as a list of file paths. Returns null if the clipboard does not contain any files.
		///If ODCloud, files are transferred from the ODCloudClient to the temp opendental directory and the new paths are returned.
		///</summary>
		public static string[] GetFileDropList() {
			if(ODBuild.IsWeb()) {
				return ODCloudClient.GetClipboardFilesFromODCloudClient();
			}
			//Not ODCloud, use the clipboard on this machine
			return (string[])Clipboard.GetDataObject()?.GetData(DataFormats.FileDrop);
		}
	}
}
