using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenDental.SmartCards.Belgium {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
	public struct CertifCheck {
		/// <summary>
		/// Policy used: 0=None/1=OCSP/2=CRL
		/// </summary>
		public int Policy;
		/// <summary>
		/// Array of BEID_Certif structures
		/// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = BelgianIdentityCard.MAX_CERT_NUMBER * 2324, ArraySubType = UnmanagedType.U1)]
		public byte[] Certificates;
		/// <summary>
		/// Number of elements in Array
		/// </summary>
		public int Length;
		/// <summary>
		/// Status of signature (for ID and Address) or hash (for Picture) on retrieved field
		/// </summary>
		public int SignatureCheck;
		/// <summary>
		/// reserved for future use 
		/// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.U1)]
		public byte[] RFU;
	}
}
