using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using OpenDentBusiness;

namespace OpenDental.SmartCards  {
	public abstract class SmartCardService {
		public SmartCardService(ISmartCardManager manager) {
			this.supportedAtrs = new Collection<byte[]>();
			this.manager = manager;
		}

		private ISmartCardManager manager;
		private Collection<byte[]> supportedAtrs;
		protected Collection<byte[]> SupportedAtrs {
			get { return supportedAtrs; }
		}

		public bool IsSupported(byte[] atr) {
			// Assume a valid ATR is not found.
			bool retValue = false;
			// Check incoming data.
			if(atr != null) {
				// Loop over the supported ATRs.
				foreach(byte[] currentAtr in supportedAtrs) {
					// Always check the length.
					if(currentAtr.Length == atr.Length) {
						// Loop over the array to compare the bytes.
						for(int i = 0; i < atr.Length; i++) {
							retValue = (atr[i] == currentAtr[i]);
							if(!retValue) {
								// We have a mismatch, break out.
								break;
							}
						}
					}
					if(retValue) {
						// We have a supported ATR, break out.
						break;
					}
				}
			}
			return retValue;
		}

		public abstract Patient GetPatientInfo(string reader);
	}
}
