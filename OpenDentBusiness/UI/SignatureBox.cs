using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using CodeBase;

namespace OpenDental.UI {
	///<summary>This class is specifically designed to duplicate the functionality of the Topaz SigPlusNET control.  So even if I would have done it differenly, I didn't have a choice.  Size of box will always be 362,79, although that seems to be arbitrary.  We just don't want to be changing it.  But it can scale proportionally under the control of the LayoutManager to handle high dpi.</summary>
	public partial class SignatureBox:Control {
		///<summary>Default 1. Typical value 1.5</summary>
		private float _scaleMS = 1;
		///<summary>Default 1. Typical value 1.2</summary>
		private float _zoomLocal=1;
		///<summary>0=not accepting input. 1=accepting input.</summary>
		private int tabletState;
		///<summary>Collection of points that will be connected to draw the signature.  0,0 represents pen up.</summary>
		private List<Point> listPoints;
		//<summary>0=none, 1=lossless.  Always use 1.</summary>
		//private int compressionMode;
		//<summary>0=clear text. 1=40 bit DES.  2=higher security.</summary>
		//private int encryptionMode;
		///<summary>The hash of the document which will be used to encrypt the signature.</summary>
		private byte[] hash;
		private bool mouseIsDown;

		public SignatureBox() {
			InitializeComponent();
			DoubleBuffered=true;
			listPoints=new List<Point>();
		}

		///<summary>Defaults are 1,1.</summary>
		public void SetScaleAndZoom(float scaleMS,float zoomLocal){
			if(scaleMS==_scaleMS && zoomLocal==_zoomLocal){
				return;
			}
			_scaleMS = scaleMS;
			_zoomLocal=zoomLocal;
		}

		///<summary>Converts from 96dpi to current total scale.</summary>
		private float ScaleF(float val96){
			float scaleTotal=_scaleMS*_zoomLocal;//example 1.5*1.2
			return val96*scaleTotal;
		}

		///<summary>Converts from scaled back to 96dpi.</summary>
		private int Unscale(float valScreen){
			int val96=(int)Math.Round(valScreen/(_scaleMS*_zoomLocal));//example 180/(1.5*1.2)=100
			return val96;
		}

		///<summary>Set to 1 to activate it to start accepting signatures.  Set to 0 to no longer accept input.</summary>
		public void SetTabletState(int state){
			tabletState=state;
		}

		///<summary>1 if the control is accepting signature input. 0 if not.</summary>
		public int GetTabletState() {
			return tabletState;
		}

		///<summary>Clears the display and the stored signature.</summary>
		public void ClearTablet(){
			listPoints=new List<Point>();
			Invalidate();
		}

		public int NumberOfTabletPoints(){
			return listPoints.Count;
		}

		/*
		///<summary>0=none, 1=lossless.  2-8 not used?</summary>
		public void SetSigCompressionMode(int compressMode){
			compressionMode=compressMode;
		}

		///<summary>0=clear text. 1=low DES (do not use).  2=high Rijndael.</summary>
		public void SetEncryptionMode(int encryptMode){
			encryptionMode=encryptMode;
		}*/

		///<summary>Set it to "0000000000000000" (16 zeros) to indicate no key string to be used for encryption.  Use this OR SetAutoKeyData.</summary>
		public void SetKeyString(string keyStr){
			UTF8Encoding enc=new UTF8Encoding();
			hash=enc.GetBytes(keyStr);
		}

		///<summary>The data that's begin signed.  A 16 byte hash will be created off this data, and used to encrypt the signature.  Use this OR SetKeyString.  But once the choice is made for a particular data type, it may never be changed.</summary>
		public void SetAutoKeyData(string keyData){
			hash=ODCrypt.MD5.Hash(Encoding.UTF8.GetBytes(keyData));
		}

		///<summary>Encrypts signature text and returns a base 64 string so that it can go directly into the database.</summary>
		public string GetSigString(){
			if(listPoints.Count==0){
				return "";
			}
			string rawString="";
			for(int i=0;i<listPoints.Count;i++){
				if(i>0){
					rawString+=";";
				}
				rawString+=listPoints[i].X.ToString()+","+listPoints[i].Y.ToString();
			}
			return OpenDentBusiness.UI.SigBox.EncryptSigString(hash,rawString);
		}

		///<summary>Unencrypts the signature coming in from the database.  The key data etc needs to be set first before calling this function.</summary>
		public void SetSigString(string sigString){
			listPoints=new List<Point>();
			if(sigString==""){
				return;
			}
			try{
				//convert base64 string to bytes
				byte[] encryptedBytes=Convert.FromBase64String(sigString);
				//create the streams
				MemoryStream ms=new MemoryStream();
				//ms.Write(encryptedBytes,0,(int)encryptedBytes.Length);
				//create a crypto stream
				Rijndael crypt=Rijndael.Create();
				crypt.KeySize=128;//16 bytes;
				crypt.Key=hash;
				crypt.IV=new byte[16];
				CryptoStream cs=new CryptoStream(ms,crypt.CreateDecryptor(),CryptoStreamMode.Write);
				cs.Write(encryptedBytes,0,encryptedBytes.Length);
				cs.FlushFinalBlock();
				byte[] sigBytes=new byte[ms.Length];
				ms.Position=0;
				ms.Read(sigBytes,0,(int)ms.Length);
				cs.Dispose();
				ms.Dispose();
				//now convert the bytes into a string.
				string rawString=Encoding.UTF8.GetString(sigBytes);
				//convert the raw string into a series of points
				string[] pointArray=rawString.Split(new char[] {';'});
				Point pt;
				string[] coords;
				for(int i=0;i<pointArray.Length;i++){
					coords=pointArray[i].Split(new char[] {','});
					pt=new Point(Convert.ToInt32(coords[0]),Convert.ToInt32(coords[1]));
					listPoints.Add(pt);
				}
				Invalidate();
			}
			catch(Exception e) {
				e.DoNothing();
				//this will leave the list with zero points
			}
		}

		///<Summary>Also includes a surrounding box.</Summary>
		public Image GetSigImage(bool includeBorder){
			Image img=new Bitmap(Width,Height);
			Graphics g=Graphics.FromImage(img);
			g.FillRectangle(Brushes.White,0,0,this.Width,this.Height);
			Pen pen=new Pen(Color.Black,2f);
			g.SmoothingMode=SmoothingMode.HighQuality;
			if(IsDigitalSignature()) {
				StringFormat stringFormat = StringFormat.GenericDefault;
				stringFormat.Alignment=StringAlignment.Center;
				g.DrawString(GetEncryptedString(),Font,Brushes.Black,Width/2,Height/2,stringFormat);
				stringFormat?.Dispose();
			}
			else {
				for(int i = 1;i<listPoints.Count;i++) {//skip the first point
					if(listPoints[i-1].X==0 && listPoints[i-1].Y==0) {
						continue;
					}
					if(listPoints[i].X==0 && listPoints[i].Y==0) {
						continue;
					}
					g.DrawLine(pen,listPoints[i-1],listPoints[i]);
				}
			}
			if(includeBorder){
				g.DrawRectangle(Pens.Black,0,0,Width-1,Height-1);
			}
			g.Dispose();
			return img;
		}

		public void SetPointList(List<Point> listPoints) {
			this.listPoints=new List<Point>(listPoints);
			Invalidate();
		}

		///<summary>Used in the OC plugin to display sigs that aren't encrypted or hashed.  Format x1,y1;x2,y2;x3,y3.  0,0 indicates pen up and start of new segment path.</summary>
		public void SetPointList(string pointString) {
			listPoints=new List<Point>();
			if(pointString==""){
				return;
			}
			//convert the raw string into a series of points
			string[] pointArray=pointString.Split(';');
			Point pt;
			string[] coords;
			for(int i=0;i<pointArray.Length;i++){
				coords=pointArray[i].Split(',');
				pt=new Point(Convert.ToInt32(coords[0]),Convert.ToInt32(coords[1]));
				listPoints.Add(pt);
			}
			Invalidate();
		}

		public string GetPointString() {
			return string.Join(";",listPoints);
		}

		///<summary></summary>
		protected override void OnPaintBackground(PaintEventArgs pea) {
			//base.OnPaintBackground (pea);
			//don't paint background.  This reduces flickering when using double buffering.
		}

		protected override void OnPaint(PaintEventArgs e) {
			Graphics g=e.Graphics;
			using Pen pen = new Pen(Color.Black,2f);
			g.FillRectangle(Brushes.White,0,0,this.Width,this.Height);
			g.SmoothingMode=SmoothingMode.HighQuality;
			if(IsDigitalSignature()) {
				StringFormat stringFormat = new StringFormat(StringFormat.GenericDefault);
				stringFormat.Alignment=StringAlignment.Center;
				stringFormat.LineAlignment=StringAlignment.Center;
				g.DrawString(GetEncryptedString(),Font,Brushes.Black,Width/2,Height/2,stringFormat);
				stringFormat?.Dispose();
				return;
			}
			for(int i = 1;i<listPoints.Count;i++) {//skip the first point
				if(listPoints[i-1].X==0 && listPoints[i-1].Y==0) {
					continue;
				}
				if(listPoints[i].X==0 && listPoints[i].Y==0) {
					continue;
				}
				g.DrawLine(pen,ScaleF(listPoints[i-1].X),ScaleF(listPoints[i-1].Y),ScaleF(listPoints[i].X),ScaleF(listPoints[i].Y));
			}
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(tabletState==0){
				return;
			}
			mouseIsDown=true;
			listPoints.Add(new Point(Unscale(e.X),Unscale(e.Y)));
			//Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(tabletState==0) {
				return;
			}
			if(!mouseIsDown){
				return;
			}
			listPoints.Add(new Point(Unscale(e.X),Unscale(e.Y)));
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(tabletState==0) {
				return;
			}
			mouseIsDown=false;
			listPoints.Add(new Point(0,0));
		}

		public string GetEncryptedString() {
			if(!IsDigitalSignature()) {
				return "";
			}
			try {
				return Encoding.UTF8.GetString(listPoints.SelectMany(x => new[] { x.X,x.Y }).Where(x=>x>int.MinValue).Select(x=>(byte)x).ToArray());
			}
			catch(Exception ex) { ex.DoNothing(); }
			return "";
		}

		public List<Point> EncryptString(string input) {
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			List<Point> retVal = new List<Point>() {
				//These two points indicate that this is NOT a "Normal" signature.
				new Point(int.MinValue,int.MinValue),
				new Point(int.MinValue,int.MinValue)
			};
			for(int i = 0;i<bytes.Length;i=i+2) {
				retVal.Add(new Point(bytes[i],(i+2)>bytes.Length?int.MinValue:bytes[i+1]));
			}
			return retVal;
		}

		/// <summary>A "digital" signature has the first two dummy points of int.min, followed by a user readable string.</summary>
		public bool IsDigitalSignature() {
			if(listPoints==null || listPoints.Count<2 || listPoints[0]!=listPoints[1] || listPoints[0].X!=int.MinValue || listPoints[0].Y!=int.MinValue) {
				//The first two points MUST be Point(Int.MinValue,Int.MinValue), otherwise this is a "Normal" signature.
				return false;
			}
			return true;
		}



	}
}
