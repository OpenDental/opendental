using System;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EClipboardImageCaptureT {
		public static EClipboardImageCapture CreateEClipboardImageCapture(long patNum,long defNum,bool isSelfPortrait,DateTime dateUpserted,long docNum) {
			EClipboardImageCapture eClipboardImageCapture=new EClipboardImageCapture();
			eClipboardImageCapture.PatNum=patNum;
			eClipboardImageCapture.DefNum=defNum;
			eClipboardImageCapture.IsSelfPortrait=isSelfPortrait;
			eClipboardImageCapture.DateTimeUpserted=dateUpserted;
			eClipboardImageCapture.DocNum=docNum;
			EClipboardImageCaptures.Insert(eClipboardImageCapture);
			return eClipboardImageCapture;
		}

		public static void ClearEClipboardImageCaptureTable() {
			string command="DELETE FROM eclipboardimagecapture WHERE EClipboardImageCaptureNum > 0";
			DataCore.NonQ(command);
		}
	}
}
