using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	class PluginContainer {
		public PluginBase Plugin;
		public long ProgramNum;
		///<summary>Used by reflection for "s" class calls to middle tier.</summary>
		public Assembly Assemb;
		///<summary>The dll name without extension, and stripped clean.  Used by reflection for "s" class calls to middle tier.</summary>
		public string Name;
	}
}
