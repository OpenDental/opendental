using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MapAreaContainers{
		///<summary>Order is by sitenum, then by description.</summary>
		public static List<MapAreaContainer> GetAll(long siteNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<MapAreaContainer>>(MethodBase.GetCurrentMethod(),siteNum);
			}
			//Add a custom order to this map list which will prefer maps that are associated with the local computer's site.
			//_listMapAreaContainers=_listMapAreaContainers.OrderBy(x => x.SiteNum!=_siteThisComputer.SiteNum)
			//	.ThenBy(x => x.Description).ToList();
			string command="SELECT * FROM mapareacontainer ORDER BY SiteNum <> "+POut.Long(siteNum)
				+", Description";
			return Crud.MapAreaContainerCrud.SelectMany(command);
		}

		public static void SaveWholeListToDb(List<MapAreaContainer> listMapAreaContainers) {	
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listMapAreaContainers);
			}
			//We do it the same as before the refactor.  Delete everything and re-insert.  Improve later
			string command="DELETE FROM mapareacontainer";
			Db.NonQ(command);
			for(int i=0;i<listMapAreaContainers.Count;i++){
				Crud.MapAreaContainerCrud.Insert(listMapAreaContainers[i],useExistingPK:true);
			}
		}

		///<summary></summary>
		public static long Insert(MapAreaContainer mapAreaContainer){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				mapAreaContainer.MapAreaContainerNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mapAreaContainer);
				return mapAreaContainer.MapAreaContainerNum;
			}
			return Crud.MapAreaContainerCrud.Insert(mapAreaContainer);
		}

		///<summary></summary>
		public static void Update(MapAreaContainer mapAreaContainer){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mapAreaContainer);
				return;
			}
			Crud.MapAreaContainerCrud.Update(mapAreaContainer);
		}

		///<summary></summary>
		public static void Delete(long mapAreaContainerNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mapAreaContainerNum);
				return;
			}
			string command= "DELETE FROM mapareacontainer WHERE MapAreaContainerNum = "+POut.Long(mapAreaContainerNum);
			Db.NonQ(command);
		}
	}
}


/*Original data:
[{"MapAreaContainerNum":5,"FloorWidthFeet":80,"FloorHeightFeet":57,"PixelsPerFoot":17,"ShowGrid":false,"ShowOutline":true,"Description":"B2","ExtensionStartRange":-1,"ExtensionEndRange":-1,"SiteNum":3},{"MapAreaContainerNum":6,"FloorWidthFeet":100,"FloorHeightFeet":56,"PixelsPerFoot":17,"ShowGrid":false,"ShowOutline":true,"Description":"F","ExtensionStartRange":-1,"ExtensionEndRange":-1,"SiteNum":1},{"MapAreaContainerNum":7,"FloorWidthFeet":90,"FloorHeightFeet":60,"PixelsPerFoot":17,"ShowGrid":true,"ShowOutline":true,"Description":"B1/C","ExtensionStartRange":-1,"ExtensionEndRange":-1,"SiteNum":3},{"MapAreaContainerNum":8,"FloorWidthFeet":90,"FloorHeightFeet":60,"PixelsPerFoot":17,"ShowGrid":true,"ShowOutline":true,"Description":"A/B1","ExtensionStartRange":-1,"ExtensionEndRange":-1,"SiteNum":3},{"MapAreaContainerNum":9,"FloorWidthFeet":90,"FloorHeightFeet":57,"PixelsPerFoot":17,"ShowGrid":true,"ShowOutline":true,"Description":"Tech Teams 1-14","ExtensionStartRange":-1,"ExtensionEndRange":-1,"SiteNum":3},{"MapAreaContainerNum":10,"FloorWidthFeet":90,"FloorHeightFeet":58,"PixelsPerFoot":17,"ShowGrid":true,"ShowOutline":true,"Description":"Specialty Teams","ExtensionStartRange":-1,"ExtensionEndRange":-1,"SiteNum":3},{"MapAreaContainerNum":11,"FloorWidthFeet":100,"FloorHeightFeet":56,"PixelsPerFoot":17,"ShowGrid":true,"ShowOutline":true,"Description":"G","ExtensionStartRange":-1,"ExtensionEndRange":-1,"SiteNum":3},{"MapAreaContainerNum":12,"FloorWidthFeet":90,"FloorHeightFeet":57,"PixelsPerFoot":17,"ShowGrid":true,"ShowOutline":true,"Description":"Tech Teams 15+","ExtensionStartRange":-1,"ExtensionEndRange":-1,"SiteNum":3},{"MapAreaContainerNum":13,"FloorWidthFeet":76,"FloorHeightFeet":40,"PixelsPerFoot":17,"ShowGrid":true,"ShowOutline":true,"Description":"Advanced Services","ExtensionStartRange":-1,"ExtensionEndRange":-1,"SiteNum":3},

{"MapAreaContainerNum":14,"FloorWidthFeet":90,"FloorHeightFeet":57,"PixelsPerFoot":17,"ShowGrid":true,"ShowOutline":true,"Description":"Education","ExtensionStartRange":-1,"ExtensionEndRange":-1,"SiteNum":0}]
*/

/*New data:
DROP TABLE IF EXISTS mapareacontainer;
CREATE TABLE mapareacontainer (
	MapAreaContainerNum bigint NOT NULL auto_increment PRIMARY KEY,
	FloorWidthFeet int NOT NULL,
	FloorHeightFeet int NOT NULL,
	PixelsPerFoot int NOT NULL,
	ShowGrid tinyint NOT NULL,
	ShowOutline tinyint NOT NULL,
	Description varchar(255) NOT NULL,
	SiteNum bigint NOT NULL
	) DEFAULT CHARSET=utf8;
INSERT INTO mapareacontainer (MapAreaContainerNum,FloorWidthFeet,FloorHeightFeet,PixelsPerFoot,ShowGrid,ShowOutline,Description,SiteNum) 
	VALUES(5,80,57,17,0,1,'B2',3);
INSERT INTO mapareacontainer (MapAreaContainerNum,FloorWidthFeet,FloorHeightFeet,PixelsPerFoot,ShowGrid,ShowOutline,Description,SiteNum) 
	VALUES(6,100,56,17,0,1,'F',1);
INSERT INTO mapareacontainer (MapAreaContainerNum,FloorWidthFeet,FloorHeightFeet,PixelsPerFoot,ShowGrid,ShowOutline,Description,SiteNum) 
	VALUES(7,90,60,17,1,1,'B1',3);
INSERT INTO mapareacontainer (MapAreaContainerNum,FloorWidthFeet,FloorHeightFeet,PixelsPerFoot,ShowGrid,ShowOutline,Description,SiteNum) 
	VALUES(8,90,60,17,1,1,'A/B1',3);
INSERT INTO mapareacontainer (MapAreaContainerNum,FloorWidthFeet,FloorHeightFeet,PixelsPerFoot,ShowGrid,ShowOutline,Description,SiteNum) 
	VALUES(9,90,57,17,1,1,'Tech Teams 1-14',3);
INSERT INTO mapareacontainer (MapAreaContainerNum,FloorWidthFeet,FloorHeightFeet,PixelsPerFoot,ShowGrid,ShowOutline,Description,SiteNum) 
	VALUES(10,90,58,17,1,1,'Specialty Teams',3);
INSERT INTO mapareacontainer (MapAreaContainerNum,FloorWidthFeet,FloorHeightFeet,PixelsPerFoot,ShowGrid,ShowOutline,Description,SiteNum) 
	VALUES(11,100,56,17,1,1,'G',3);
INSERT INTO mapareacontainer (MapAreaContainerNum,FloorWidthFeet,FloorHeightFeet,PixelsPerFoot,ShowGrid,ShowOutline,Description,SiteNum) 
	VALUES(12,90,57,17,1,1,'Tech Teams 15+',3);
INSERT INTO mapareacontainer (MapAreaContainerNum,FloorWidthFeet,FloorHeightFeet,PixelsPerFoot,ShowGrid,ShowOutline,Description,SiteNum) 
	VALUES(13,76,40,17,1,1,'Advanced Services',3);
INSERT INTO mapareacontainer (MapAreaContainerNum,FloorWidthFeet,FloorHeightFeet,PixelsPerFoot,ShowGrid,ShowOutline,Description,SiteNum) 
	VALUES(14,90,57,17,1,1,'Education',0);
		

*/