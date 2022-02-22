using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralManager{
	public class WindowInfo{
		///<summary>This isn't constant because it will be the most recent active popup for the ProcessID.  It gets reset frequently.</summary>
		public IntPtr HWnd;
		//<summary>This is the state according to the Win32 api, but it doesn't </summary>
		//public ShowState State;
		public long CentralConnectionNum;
		///<summary>This is constant for the life of the application, so it's a great foundation for finding and controlling a window.</summary>
		public int ProcessId;
		///<summary>This window should be maximized when restored instead of set to normal.  Set true whenever a window is found in a maximized state.  Set false whenever a window is found in normal state.  Left alone in minimized state.</summary>
		public bool WasMaximized;
		///<summary>This window is currently minimized.</summary>
		public bool IsMinimized;

		public string GetStringState(){
			if(IsMinimized){
				return "mimimized";
			}
			return "showing";
			//switch(State){
			//	case default:
			//		return "";
			//}
			//return State.ToString();
		}

	}

	/*
	///<summary>This is our interpretation of the current state of each window.TThere is no closed state because we will delete the WindowInfo object if the ProcessID can't be found.</summary>
	public enum ShowStateOD{
		Showing,
		Minimized
	}*/
}
