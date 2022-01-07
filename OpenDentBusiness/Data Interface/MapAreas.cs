using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class MapAreas{
		///<summary>Pass in a MapAreaContainerNum to limit the list to a single room.  Otherwise all cubicles from every map will be returned.</summary>
		public static List<MapArea> Refresh(long mapAreaContainerNum=0) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<MapArea>>(MethodBase.GetCurrentMethod(),mapAreaContainerNum);
			}
			string command="SELECT * FROM maparea";
			if(mapAreaContainerNum>0) {
				command+=$" WHERE MapAreaContainerNum={POut.Long(mapAreaContainerNum)}";
			}
			return Crud.MapAreaCrud.SelectMany(command);
		}
		/*		
		///<summary>Gets one MapArea from the db.</summary>
		public static MapArea GetOne(long mapAreaNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<MapArea>(MethodBase.GetCurrentMethod(),mapAreaNum);
			}
			return Crud.MapAreaCrud.SelectOne(mapAreaNum);
		}
		*/
		///<summary></summary>
		public static long Insert(MapArea mapArea){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				mapArea.MapAreaNum=Meth.GetLong(MethodBase.GetCurrentMethod(),mapArea);
				return mapArea.MapAreaNum;
			}
			return Crud.MapAreaCrud.Insert(mapArea);
		}

		///<summary></summary>
		public static void Update(MapArea mapArea){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mapArea);
				return;
			}
			Crud.MapAreaCrud.Update(mapArea);
		}

		///<summary></summary>
		public static void Delete(long mapAreaNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),mapAreaNum);
				return;
			}
			string command= "DELETE FROM maparea WHERE MapAreaNum = "+POut.Long(mapAreaNum);
			Db.NonQ(command);
		}
	}
}