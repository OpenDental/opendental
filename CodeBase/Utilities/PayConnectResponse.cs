using System;

namespace CodeBase {

	///<summary>Response class that can hold information for a web service response or a terminal response.</summary>
	public class PayConnectResponse {
		public string Description;
		public string StatusCode;
		public string AuthCode;
		public string RefNumber;
		public string PaymentToken;
		public DateTime TokenExpiration;
		public string CardType;
		public decimal Amount;
		public decimal OriginalAmount;
		public string EntryMode;
		public string CardNumber;
		public string MerchantId;
		public string TerminalId;
		public string Mode;
		public string CardVerificationMethod;
		public TransactionType TransType;
		public EmvData EMV;

		public class EmvData {
			public string AppId;
			public string TermVerifResults;
			public string IssuerAppData;
			public string TransStatusInfo;
			public string AuthResponseCode;
		}

		public enum TransactionType {
			///<summary>0 - Sale</summary>
			Sale,
			///<summary>1 - Authorize</summary>
			Authorize,
			///<summary>2 - Refund</summary>
			Refund,
			///<summary>3 - Capture</summary>
			Capture,
			///<summary>4 - Void</summary>
			Void,
			///<summary>5 - Unknown</summary>
			Unknown
		}

		public PayConnectResponse() {
		}
	}
}
