﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class PopupT {

		///<summary>Inserts a new popup.</summary>
		public static Popup CreatePopupAPI(long patNum=0,string description="",EnumPopupLevel popupLevel=EnumPopupLevel.Patient) {
			Popup popup=new Popup();
			popup.PatNum=patNum;
			popup.Description=description;
			popup.PopupLevel=popupLevel;
			Popups.Insert(popup);
			return popup;
		}


	}
}
