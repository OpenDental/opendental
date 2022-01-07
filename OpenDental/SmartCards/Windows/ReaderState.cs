using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OpenDental.SmartCards  {
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	struct ReaderState {
		public string Reader;
		public IntPtr UserData;
		public CardState CurrentState;
		public CardState EventState;
		public uint cbAtr;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst=13)]
		public byte[] rgbAtr;
	}
}
