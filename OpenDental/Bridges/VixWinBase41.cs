using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental.Bridges {
	/// <summary></summary>
	public class VixWinBase41 {

		/// <summary></summary>
		public VixWinBase41() {

		}

		///<summary>Sends data for Patient.Cur by command line interface.</summary>
		public static void SendData(Program ProgramCur, Patient pat) {
			string path=Programs.GetProgramPath(ProgramCur);
			if(pat==null) {
				return;
			}
			//Example: c:\vixwin\vixwin -I 12AB@# -N Bill^Smith -P X:\VXImages\12AB#$\
			string info="-I "+ConvertToBase41(pat.PatNum);
			string ppImagePath=ProgramProperties.GetPropVal(ProgramCur.ProgramNum,"Image Path");
			info+=" -N "+pat.FName.Replace(" ","")+"^"+pat.LName.Replace(" ","");//no spaces allowed
			if(ppImagePath=="") {
				MessageBox.Show("Image Path cannot be left blank.");
				return;
			}
			if(!ppImagePath.EndsWith("\\")) {//if program path doesn't end with "\" then add it to the end.
				ppImagePath+="\\";
			}
			ppImagePath+=ConvertToBase41(pat.PatNum)+"\\";//if we later allow ChartNumbers, then we will have to validate them to be 6 digits or less.
			info+=" -P "+ ppImagePath;
			//create image directory if it doesn't exist
			if(!Directory.Exists(ppImagePath)) {//if the directory doesn't exist
				try {
					Directory.CreateDirectory(ppImagePath);
				}
				catch {
					MessageBox.Show("Unable to create patient image directory "+ppImagePath);
					return;
				}
			}
			//MessageBox.Show(info);
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

		///<summary>This function will translate the Base10 Dentrix ID to the Base41 VixWin ID.</summary>
		private static string ConvertToBase41(long intPatNum) {
			string retVal = "";
			string base41Array = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ@#$%^";
			long intID = intPatNum;
			long intTemp = 0;
			long intMultiplier = 0;
			for(int i=5;i>=0;i--) {
				intMultiplier = Pow(41,i);
				intTemp = 0;
				if(intID >= intMultiplier) {
					intTemp = intID/intMultiplier;//resulting float is truncated intentionally
				}
				intID = intID-intTemp*intMultiplier;
				retVal = retVal + base41Array[(int)intTemp];
			}
			return retVal;
		}

	}
}











