using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	/// <summary></summary>
	public class VixWinBase36 {

		/// <summary></summary>
		public VixWinBase36() {

		}

		///<summary>Sends data for Patient.Cur by command line interface.</summary>
		public static void SendData(Program ProgramCur,Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				MsgBox.Show("VixWinBase36","Please select a patient first.");
				return;
			}
			//Example: c:\vixwin\vixwin -I 12ABYZ -N Bill^Smith -P X:\VXImages\12AB#$\
			string info="-I "+ConvertToBase36(pat.PatNum);
			string ppImagePath=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Image Path");
			if(pat.FName.ToString()!="") {
				info+=" -N "+Tidy(pat.FName)+"^"+Tidy(pat.LName);
			}
			if(ppImagePath!="") {//optional image path
				if(!Directory.Exists(ppImagePath)) {
					MessageBox.Show("Unable to find image path "+ppImagePath);
					return;
				}
				if(!ppImagePath.EndsWith("\\")) {//if program path doesn't end with "\" then add it to the end.
					ppImagePath+="\\";
				}
				ppImagePath+=ConvertToBase36(pat.PatNum)+"\\";//if we later allow ChartNumbers, then we will have to validate them to be 6 digits or less.
				info+=" -P "+ ppImagePath;
			}
			try {
				ODFileUtils.ProcessStart(path,info);
			}
			catch {
				MessageBox.Show(path+" is not available. Ensure that the program and image paths are valid.");
			}
		}

		///<summary>Returns the value of x^y. We cannot use Math.Pow(), because Math.Pow() uses doubles only, which it has rounding errors with large numbers.
		///We need our result to be a perfect integer. We assume y>=0.</summary>
		private static long Pow(long x,long y) {
			long result=1;
			for(int p=0;p<y;p++) {
				result=result*x;
			}
			return result;
		}

		///<summary>This function will translate the Base10 Dentrix ID to the Base36 VixWin ID.</summary>
		private static string ConvertToBase36(long intPatNum) {
			string retVal = "";
			string base36Array = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"; //removed "@#$%^" from base41Array
			long intID = intPatNum;
			long intTemp = 0;
			long intMultiplier = 0;
			for(int i=5;i>=0;i--) {
				intMultiplier = Pow(36,i);
				intTemp = 0;
				if(intID >= intMultiplier) {
					intTemp = intID/intMultiplier;//resulting float is truncated intentionally
				}
				intID = intID-intTemp*intMultiplier;
				retVal = retVal + base36Array[(int)intTemp];
			}
			return retVal;
		}

		///<summary>Removes semicolons and spaces.</summary>
		private static string Tidy(string input) {
			string retVal=input.Replace(";","");//get rid of any semicolons.
			retVal=retVal.Replace(" ","");
			return retVal;
		}

	}
}











