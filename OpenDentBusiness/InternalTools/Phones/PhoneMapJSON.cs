using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary></summary>
	public class PhoneMapJSON {
		private const string PREF_NAME="HQSerializedMapContainers";
		public static void SaveToDb(List<MapAreaContainer> maps) {			
			Prefs.UpdateRaw(PREF_NAME,JsonConvert.SerializeObject(maps));
		}

		public static List<MapAreaContainer> GetFromDb() {
			List<MapAreaContainer> ret;
			try {
				ret=JsonConvert.DeserializeObject<List<MapAreaContainer>>(Prefs.GetPref(PREF_NAME).ValueString)??GetDefaultMap();
			}
			catch{
				ret=GetDefaultMap();
			}
			return ret.OrderBy(x => x.MapAreaContainerNum).ToList();
		}

		public static List<MapAreaContainer> GetDefaultMap() {
			return new List<MapAreaContainer> { new MapAreaContainer(1,71,57,17,false,true,"Dolphin Room"),new MapAreaContainer(2,10,10,17,true,true,"Orca Room") };
		}
	}
	
	///<summary>DO NOT EDIT THE VARIABLE NAMES OF THIS TABLE.  
	///The variable names are hard-coded into JSON and serialized/deserialized into the DB and the program.</summary>
	public class MapAreaContainer {
		///<summary>Essentially the PK to this "table."  Room Number.</summary>
		public long MapAreaContainerNum;
		public int FloorWidthFeet;
		public int FloorHeightFeet;
		public int PixelsPerFoot;
		public bool ShowGrid;
		public bool ShowOutline;
		public string Description;
		///<summary>Deprecated.  Use SiteNum instead of relying on extension ranges.</summary>
		[DefaultValue(-1)]
		[JsonProperty(DefaultValueHandling=DefaultValueHandling.Populate)]
		public int ExtensionStartRange;
		///<summary>Deprecated.  Use SiteNum instead of relying on extension ranges.</summary>
		[DefaultValue(-1)]
		[JsonProperty(DefaultValueHandling=DefaultValueHandling.Populate)]
		public int ExtensionEndRange;
		///<summary>FK to site.SiteNum  Represents the site or location which this room belongs to.  Used with escalation and conf rooms.</summary>
		public long SiteNum;

		public MapAreaContainer(long containerNum,int width,int height,int pixels,bool showGrid,bool showOutline,string descript) {
			MapAreaContainerNum=containerNum;
			FloorWidthFeet=width;
			FloorHeightFeet=height;
			PixelsPerFoot=pixels;
			ShowGrid=showGrid;
			ShowOutline=showOutline;
			Description=descript;
			ExtensionStartRange=-1;
			ExtensionEndRange=-1;
		}
	}
}