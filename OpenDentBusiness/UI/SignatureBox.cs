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
	///<summary>This class is specifically designed to duplicate the functionality of the Topaz SigPlusNET control.  So even if I would have done it differenly, I didn't have a choice.  Size of box will always be 362,79, although that seems to be arbitrary.  We just don't want to be changing it.</summary>
	public partial class SignatureBox:Control {
		///<summary>0=not accepting input. 1=accepting input.</summary>
		private int tabletState;
		///<summary>Collection of points that will be connected to draw the signature.  0,0 represents pen up.</summary>
		private List<Point> pointList;
		//<summary>0=none, 1=lossless.  Always use 1.</summary>
		//private int compressionMode;
		//<summary>0=clear text. 1=40 bit DES.  2=higher security.</summary>
		//private int encryptionMode;
		///<summary>The hash of the document which will be used to encrypt the signature.</summary>
		private byte[] hash;
		private bool mouseIsDown;



		public SignatureBox() {
			InitializeComponent();
			pointList=new List<Point>();
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
			pointList=new List<Point>();
			Invalidate();
		}

		public int NumberOfTabletPoints(){
			return pointList.Count;
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
			if(pointList.Count==0){
				return "";
			}
			string rawString="";
			for(int i=0;i<pointList.Count;i++){
				if(i>0){
					rawString+=";";
				}
				rawString+=pointList[i].X.ToString()+","+pointList[i].Y.ToString();
			}
			return OpenDentBusiness.UI.SigBox.EncryptSigString(hash,rawString);
		}

		///<summary>Unencrypts the signature coming in from the database.  The key data etc needs to be set first before calling this function.</summary>
		public void SetSigString(string sigString){
			pointList=new List<Point>();
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
					pointList.Add(pt);
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
				var sf = StringFormat.GenericDefault;
				sf.Alignment=StringAlignment.Center;
				g.DrawString(GetEncryptedString(),Font,Brushes.Black,Width/2,Height/2,sf);
			}
			else {
				for(int i = 1;i<pointList.Count;i++) {//skip the first point
					if(pointList[i-1].X==0 && pointList[i-1].Y==0) {
						continue;
					}
					if(pointList[i].X==0 && pointList[i].Y==0) {
						continue;
					}
					g.DrawLine(pen,pointList[i-1],pointList[i]);
				}
			}
			if(includeBorder){
				g.DrawRectangle(Pens.Black,0,0,Width-1,Height-1);
			}
			g.Dispose();
			return img;
		}

		public void SetPointList(List<Point> listPoints) {
			pointList=new List<Point>(listPoints);
			Invalidate();
		}

		///<summary>Used in the OC plugin to display sigs that aren't encrypted or hashed.  Format x1,y1;x2,y2;x3,y3.  0,0 indicates pen up and start of new segment path.</summary>
		public void SetPointList(string pointString) {
			pointList=new List<Point>();
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
				pointList.Add(pt);
			}
			Invalidate();
		}

		public string GetPointString() {
			return string.Join(";",pointList);
		}

		///<summary></summary>
		protected override void OnPaintBackground(PaintEventArgs pea) {
			//base.OnPaintBackground (pea);
			//don't paint background.  This reduces flickering when using double buffering.
		}

		protected override void OnPaint(PaintEventArgs e) {
			using(Bitmap doubleBuffer = new Bitmap(Width,Height,e.Graphics))
			using(Graphics g = Graphics.FromImage(doubleBuffer))
			using(Pen pen = new Pen(Color.Black,2f)) {
				g.FillRectangle(Brushes.White,0,0,this.Width,this.Height);
				g.SmoothingMode=SmoothingMode.HighQuality;
				if(IsDigitalSignature()) {
					using(StringFormat sf = new StringFormat(StringFormat.GenericDefault) { Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center }) {
						g.DrawString(GetEncryptedString(),Font,Brushes.Black,Width/2,Height/2,sf);
					}
				}
				else {
					for(int i = 1;i<pointList.Count;i++) {//skip the first point
						if(pointList[i-1].X==0 && pointList[i-1].Y==0) {
							continue;
						}
						if(pointList[i].X==0 && pointList[i].Y==0) {
							continue;
						}
						g.DrawLine(pen,pointList[i-1],pointList[i]);
					}
				}
				e.Graphics.DrawImageUnscaled(doubleBuffer,0,0);
			}
			base.OnPaint(e);
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(tabletState==0){
				return;
			}
			mouseIsDown=true;
			pointList.Add(new Point(e.X,e.Y));
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
			pointList.Add(new Point(e.X,e.Y));
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(tabletState==0) {
				return;
			}
			mouseIsDown=false;
			pointList.Add(new Point(0,0));
		}

		public string GetEncryptedString() {
			if(!IsDigitalSignature()) {
				return "";
			}
			try {
				return Encoding.UTF8.GetString(pointList.SelectMany(x => new[] { x.X,x.Y }).Where(x=>x>int.MinValue).Select(x=>(byte)x).ToArray());
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

		public bool IsDigitalSignature() {
			if(pointList==null || pointList.Count<2 || pointList[0]!=pointList[1] || pointList[0].X!=int.MinValue || pointList[0].Y!=int.MinValue) {
				//The first two points MUST be Point(Int.MinValue,Int.MinValue), otherwise this is a "Normal" signature.
				return false;
			}
			return true;
		}



	}
}
