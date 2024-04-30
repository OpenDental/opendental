using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace OpenDental{
	///<summary>Handles a global event to keep local data synchronized.</summary>
	public class DataValid{
		///<summary>FormOpenDental subscribes to this event. Frms fire this event to cause event handler to run.</summary>
		public static event EventHandler<ValidEventArgs> EventInvalid;

		///<summary>Triggers an event that causes a signal to be sent to all other computers telling them what kind of locally stored data needs to be updated.  Either supply a set of flags for the types, or supply a date if the appointment screen needs to be refreshed.  Yes, this does immediately refresh the local data, too.  The AllLocal override does all types except appointment date for the local computer only, such as when starting up. If you only want to refresh local data, use the RefreshCache() method of your s class. This works on MT.</summary>
		public static void SetInvalid(params InvalidType[] arrayITypes) {
			ValidEventArgs e=new ValidEventArgs(DateTime.MinValue,arrayITypes,false,0);
			EventInvalid?.Invoke(null,e);
			//FormOpenDental.S_DataValid_BecomeInvalid();
		}

		///<summary>Triggers an event that causes a signal to be sent to all other computers telling them what kind of locally stored data needs to be updated.  Either supply a set of flags for the types, or supply a date if the appointment screen needs to be refreshed.  Yes, this does immediately refresh the local data, too.  The AllLocal override does all types except appointment date for the local computer only, such as when starting up. If you only want to refresh local data, use the RefreshCache() method of your s class. This works on MT.</summary>
		public static void SetInvalid(bool onlyLocal) {
			ValidEventArgs e=new ValidEventArgs(DateTime.MinValue,new[] { InvalidType.AllLocal },true,0);
			EventInvalid?.Invoke(null,e);
			//FormOpenDental.S_DataValid_BecomeInvalid();
		}

	}

	///<summary></summary>
	public class ValidEventArgs{
		public DateTime DateViewing;
		public InvalidType[] ITypes;
		public bool OnlyLocal;
		public long TaskNum;
		
		///<summary></summary>
		public ValidEventArgs(DateTime dateViewing,InvalidType[] itypes,bool onlyLocal,long taskNum){
			DateViewing=dateViewing;
			ITypes=itypes;
			OnlyLocal=onlyLocal;
			TaskNum=taskNum;
		}
	}
}
