using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OpenDentBusiness.Mobile {

	/// <summary>This table is called preference in the mobile database.  This is to simply to avoid having to rewrite DataConnection.TestConnection().  The primary key of this table has an m in it to remind us that the preferences are totally different than in the main program.</summary>
	[Serializable()]
	[CrudTable(IsMobile=true)]
	public class Prefm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long PrefNum;
		///<summary>The text 'key' in the key/value pairing.</summary>
		public string PrefmName;// this is named PrefmName rather than PrefName because there would be name ambiguity with Pref.PrefName which would cause a compilation error in the main program whereever PrefName (Pref.PrefName) is used.
		///<summary>The stored value.</summary>
		public string ValueString;
	}

	///<summary>Because this enum is stored in the database as strings rather than as numbers, we can do the order alphabetically and we can change it whenever we want.</summary>
	public enum PrefmName {
		PracticeTitle
	}
}