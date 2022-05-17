using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDental.SmartCards  {
	public class SmartCardStateChangedEventArgs : EventArgs {
		public SmartCardStateChangedEventArgs(string reader, SmartCardState state, byte[] atr) {
			this.reader = reader;
			this.state = state;
			this.atr = atr;
		}

		private string reader;
		public string Reader {
			get { return reader; }
		}

		private SmartCardState state;
		public SmartCardState State {
			get { return state; }
		}

		private byte[] atr;
		public byte[] Atr {
			get { return atr; }
		}
	}
}
