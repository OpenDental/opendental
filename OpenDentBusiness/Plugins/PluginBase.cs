using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace OpenDentBusiness {
	public abstract class PluginBase {
		private Form host;

		///<summary>This will be a refrence to the main FormOpenDental so that it can be used by    the plugin if needed.  It is set once on startup, so it's a good place to put startup code.   If this is the middle tier, then this will be null.</summary>
		public virtual Form Host { 
			get { 
				return host; 
			}
			set {
				host=value; 
			}
		}

		///<summary>These types of hooks are designed to completely replace the existing functionality of specific methods.  They always belong at the top of a method.</summary>
		public virtual bool HookMethod(object sender,string methodName,params object[] parameters) {
			return false;//by default, no hooks are implemented.
		}

		///<summary>These types of hooks allow adding extra code in at some point without disturbing the existing code.</summary>
		public virtual bool HookAddCode(object sender,string hookName,params object[] parameters) {
			return false;
		}

		///<summary>Plugin button now supports no patient loaded, patNum=0.  Plugin writers may need to give error message to user in this situation.</summary>
		public virtual void LaunchToolbarButton(long patNum) {

		}

		///<summary>This method will get called if there is an exception when this plugin has an exception thrown.
		///This will allow the 3rd party developer to handle unexpected exceptions however they deem fit.
		///Most common exceptions that will get here are due to the office updating to a version that the plugin does not currently support.
		///Side note: this method will NOT get called if an exception throws within a complicated plugin.
		///Complicated plugins are ones that leave the "main thread".  E.g. timers, new forms spawned via .Show() instead of .ShowDialog(), etc.</summary>
		public virtual void HookException(Exception e) {
			
		}

	}
}
