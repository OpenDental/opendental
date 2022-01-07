using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDental.SmartCards {
	[Flags]
	enum CardState : uint {
		Unaware = 0x00000000,
		Ignore = 0x00000001,
		Changed = 0x00000002,
		Unkown = 0x00000004,
		Unavailable = 0x00000008,
		Empty = 0x00000010,
		Present = 0x00000020,
		AtrMatch= 0x00000040,
		Exclusive = 0x00000080,
		Inuse = 0x00000100,
		Mute = 0x00000200,
		Unpowered = 0x00000400
	}
}
