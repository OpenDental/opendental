using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDental.SmartCards  {
	enum ScopeOptions {
		User = 0, // The context is a user context, and any
		// database operations are performed within the
		// domain of the user.

		Terminal = 1, // The context is that of the current terminal,

		// and any database operations are performed
		// within the domain of that terminal.  (The
		// calling application must have appropriate
		// access permissions for any database actions.)

		System = 2 // The context is the system context, and any
		// database operations are performed within the
		// domain of the system.  (The calling
		// application must have appropriate access
		// permissions for any database actions.)
	}
}
