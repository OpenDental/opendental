using System;

namespace OpenDentBusiness {
	/// <summary> Contains all information needed to use Azure OCR Insurance ID Form Recognizer. </summary>
	[Serializable]
	public class OcrInsScanRequest {
		public string APIKey;
		public string APIEndPoint;
		public byte[] ImageReq;
	}
}
