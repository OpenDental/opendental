using System;
using System.Collections;
using System.Text;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>This sorts a variety objects based on their dates and times.</summary>
	public class ObjectDateComparer:IComparer {
		///<summary>This sorts a variety objects based on their dates and times: Procedure, RxPat, Commlog, ClockEvent, TimeAdjust</summary>
		int IComparer.Compare(Object x,Object y) {
			DateTime datex;
			DateTime datey;
			Type typex=x.GetType();
			Type typey=y.GetType();
			if(typex==typeof(Procedure)) {
				datex=((Procedure)x).ProcDate;
			}
			else if(typex==typeof(RxPat)) {
				datex=((RxPat)x).RxDate;
			}
			else if(typex==typeof(Commlog)) {
				datex=((Commlog)x).CommDateTime;
			}
			else if(typex==typeof(ClockEvent)) {
				datex=((ClockEvent)x).TimeDisplayed1;
			}
			else if(typex==typeof(TimeAdjust)) {
				datex=((TimeAdjust)x).TimeEntry;
			}
			else {
				throw new Exception("Types don't match");//only for debugging.
			}
			if(typey==typeof(Procedure)) {
				datey=((Procedure)y).ProcDate;
			}
			else if(typey==typeof(RxPat)) {
				datey=((RxPat)y).RxDate;
			}
			else if(typey==typeof(Commlog)) {
				datey=((Commlog)y).CommDateTime;
			}
			else if(typey==typeof(ClockEvent)) {
				datey=((ClockEvent)y).TimeDisplayed1;
			}
			else if(typey==typeof(TimeAdjust)) {
				datey=((TimeAdjust)y).TimeEntry;
			}
			else {
				throw new Exception("Types don't match");//only for debugging.
			}
			return datex.CompareTo(datey);
		}

	}
}
