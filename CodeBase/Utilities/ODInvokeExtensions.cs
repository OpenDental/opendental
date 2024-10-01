using System;
using System.Windows.Forms;

namespace CodeBase {
	public static class ODInvokeExtensions {
		///<summary>Invoke an action on a control.</summary>
		public static void Invoke(this Control control,Action action) {
			//jordan OK
			//This allows anonymous methods to be used as the delegate.
			control.Invoke(action);
		}

		///<summary>Invoke an action on a control if InvokeRequired is true.</summary>
		public static void InvokeIfRequired(this Control control,Action action) {
			//jordan OK
			if(control.InvokeRequired) {
				control.Invoke(action);
				return;
			}
			action();
		}

		///<summary>Invoke an action on a control. If the control is disposing or disposed, will return without performing the action.</summary>
		public static void InvokeIfNotDisposed(this Control control,Action action) {
			//jordan OK
			if(control.Disposing || control.IsDisposed) {
				return;
			}
			bool invokeSuccessful=false;
			try {
				//There is a chance this can throw if the invoke is reached and the form is disposed while the invoke is waiting for its turn.
				control.Invoke(() => {
					invokeSuccessful=true;
					action();
				});
			}
			catch(Exception ex) {
				if(!invokeSuccessful) {//Only swallow if the exception threw while trying to invoke.
					ex.DoNothing();
				}
				else {
					throw;
				}
			}
		}

		///<summary>BeginInvoke an action on a control.</summary>
		public static void BeginInvoke(this Control control,Action action) {
			//jordan OK
			control.BeginInvoke(action);
		}
	}
}
