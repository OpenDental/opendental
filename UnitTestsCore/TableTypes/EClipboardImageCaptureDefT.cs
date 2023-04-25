using System.Collections.Generic;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EClipboardImageCaptureDefT {
		public static EClipboardImageCaptureDef CreateEClipboardImageCaptureDef(long defNum,bool isSelfPortrait,int frequency,long clinicNum) {
			EClipboardImageCaptureDef eClipboardImageCaptureDef=new EClipboardImageCaptureDef();
			eClipboardImageCaptureDef.DefNum=defNum;
			eClipboardImageCaptureDef.IsSelfPortrait=isSelfPortrait;
			eClipboardImageCaptureDef.FrequencyDays=frequency;
			eClipboardImageCaptureDef.ClinicNum=clinicNum;
			long eClipboardImageCaptureDefNum=EClipboardImageCaptureDefs.Insert(eClipboardImageCaptureDef);
			eClipboardImageCaptureDef.EClipboardImageCaptureDefNum=eClipboardImageCaptureDefNum;
			return eClipboardImageCaptureDef;
		}

		public static void ClearEClipboardImageCaptureDefTable() {
			string command="DELETE FROM eclipboardimagecapturedef WHERE EClipboardImageCaptureDefNum > 0";
			DataCore.NonQ(command);
		}
	}
}
