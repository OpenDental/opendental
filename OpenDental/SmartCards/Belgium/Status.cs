using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenDental.SmartCards.Belgium {
	[StructLayout(LayoutKind.Sequential)]
	public struct Status {
		/// <summary>
		/// Db Error Code
		/// </summary>
		[MarshalAs(UnmanagedType.I4)]
		public ErrorCodeOptions General;
		/// <summary>
		/// System Error Code
		/// </summary>
		[MarshalAs(UnmanagedType.I4)]
		public int System;
		/// <summary>
		/// PC/SC Error Code
		/// </summary>
		[MarshalAs(UnmanagedType.I4)]
		public int PCSC;
		/// <summary>
		/// Card Status Word
		/// </summary>
		[MarshalAs(UnmanagedType.I2)]
		public short CSW;
		/// <summary>
		/// Reserved for future use
		/// </summary>
		[MarshalAs(UnmanagedType.I2)]
		public short RFU1;
		[MarshalAs(UnmanagedType.I2)]
		public short RFU2;
		[MarshalAs(UnmanagedType.I2)]
		public short RFU3;
	}
}
