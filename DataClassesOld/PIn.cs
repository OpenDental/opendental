using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace OpenDental{
	
	/*=========================================================================================
	=================================== class PIn ===========================================*/
	///<summary>Converts strings coming in from the database into the appropriate type. "P" was originally short for Parameter because this class was written specifically to replace parameters in the mysql queries. Using strings instead of parameters is much easier to debug.  This will later be rewritten as a System.IConvertible interface on custom mysql types.  I would rather not ever depend on the mysql connector for this so that this program remains very db independent.</summary>
	public class PIn{
		///<summary></summary>
		public static bool PBool (string myString){
			return myString=="1";
		}

		///<summary></summary>
		public static byte PByte (string myString){
			if(myString==""){
				return 0;
			}
			else{
				return System.Convert.ToByte(myString);
			}
		}

		///<summary></summary>
		public static DateTime PDate(string myString){
			if(myString=="")
				return DateTime.MinValue;
			try{
				return (DateTime.Parse(myString));
			}
			catch{
				return DateTime.MinValue;
			}
		}

		///<summary></summary>
		public static DateTime PDateT(string myString){
			if(myString=="")
				return DateTime.MinValue;
			//if(myString=="0000-00-00 00:00:00")//useless
			//	return DateTime.MinValue;
			try{
				return (DateTime.Parse(myString));
			}
			catch{
				return DateTime.MinValue;
			}
		}

		///<summary>If blank or invalid, returns 0. Otherwise, parses.</summary>
		public static double PDouble (string myString){
			if(myString==""){
				return 0;
			}
			else{
				try{
					return System.Convert.ToDouble(myString);
				}
				catch{
					MessageBox.Show("Error converting "+myString+" to double");
					return 0;
				}
			}

		}

		///<summary></summary>
		public static int PInt (string myString){
			if(myString==""){
				return 0;
			}
			else{
				return System.Convert.ToInt32(myString);
			}
		}

		///<summary></summary>
		public static float PFloat(string myString){
			if(myString==""){
				return 0;
			}
			//try{
				return System.Convert.ToSingle(myString);
			//}
			//catch{
			//	return 0;
			//}
		}

		///<summary></summary>
		public static string PString (string myString){
			return myString;
		}
		
		//<summary></summary>
		//public static string PTime (string myTime){
		//	return DateTime.Parse(myTime).ToString("HH:mm:ss");
		//}

		//<summary></summary>
		//public static string PBytes (byte[] myBytes){
		//	return Convert.ToBase64String(myBytes);
		//}

		///<summary></summary>
		public static Bitmap PBitmap(string myString){
			if(myString==""){
				return null;
			}
			byte[] rawData=Convert.FromBase64String(myString);
			MemoryStream stream=new MemoryStream(rawData);
			Bitmap image=new Bitmap(stream);
			return image;
		}

		///<summary>Saves the string representation of a sound into a .wav file.  The timing of this is different than with the other "P" functions, and is only used by the export button in FormSigElementDefEdit</summary>
		public static void PSound(string sound, string filename) {
			if(!filename.EndsWith(".wav")) {
				throw new ApplicationException("Filename must end with .wav");
			}
			byte[] rawData=Convert.FromBase64String(sound);
			FileStream stream=new FileStream(filename,FileMode.Create,FileAccess.Write);
			stream.Write(rawData,0,rawData.Length);
		}


	}

	


}










