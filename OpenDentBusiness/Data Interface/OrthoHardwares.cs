using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrthoHardwares{
		#region Methods - Get
		///<summary></summary>
		public static List<OrthoHardware> GetPatientData(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<OrthoHardware>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM orthohardware WHERE PatNum = "+POut.Long(patNum);
			List<OrthoHardware> listOrthoHardwares=Crud.OrthoHardwareCrud.SelectMany(command);
			listOrthoHardwares=listOrthoHardwares.OrderBy(x=>x.DateExam).ThenBy(x=>x.OrthoHardwareType).ThenBy(GetToothInt).ToList();

			return listOrthoHardwares;
		}

		///<summary>Returns the int representation of the tooth or range so that proper ordering can take place.</summary>
		private static int GetToothInt(OrthoHardware orthoHardware){
			//No need to check MiddleTierRole; no call to db.
			if(orthoHardware.OrthoHardwareType==EnumOrthoHardwareType.Bracket){
				if(Tooth.IsValidDB(orthoHardware.ToothRange)){
					return Tooth.ToInt(orthoHardware.ToothRange);
				}
			}
			if(orthoHardware.OrthoHardwareType==EnumOrthoHardwareType.Elastic){
				string[] stringArrayTeeth=orthoHardware.ToothRange.Split(',');
				if(stringArrayTeeth.Length<1){
					return 0;
				}
				string tooth_id=stringArrayTeeth[0];
				if(Tooth.IsValidDB(tooth_id)){
					return Tooth.ToInt(tooth_id);
				}
			}
			if(orthoHardware.OrthoHardwareType==EnumOrthoHardwareType.Wire){
				string[] stringArrayTeeth=orthoHardware.ToothRange.Split('-');
				if(stringArrayTeeth.Length<1){
					return 0;
				}
				string tooth_id=stringArrayTeeth[0];
				if(Tooth.IsValidDB(tooth_id)){
					return Tooth.ToInt(tooth_id);
				}
			}
			return 0;
		}
		
		/*
		///<summary>Gets one OrthoHardware from the db.</summary>
		public static OrthoHardware GetOne(long orthoHardwareNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<OrthoHardware>(MethodBase.GetCurrentMethod(),orthoHardwareNum);
			}
			return Crud.OrthoHardwareCrud.SelectOne(orthoHardwareNum);
		}*/
		#endregion Methods - Get

		///<summary></summary>
		public static long Insert(OrthoHardware orthoHardware){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				orthoHardware.OrthoHardwareNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoHardware);
				return orthoHardware.OrthoHardwareNum;
			}
			return Crud.OrthoHardwareCrud.Insert(orthoHardware);
		}

		///<summary></summary>
		public static void Update(OrthoHardware orthoHardware){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoHardware);
				return;
			}
			Crud.OrthoHardwareCrud.Update(orthoHardware);
		}

		///<summary></summary>
		public static void Delete(long orthoHardwareNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoHardwareNum);
				return;
			}
			Crud.OrthoHardwareCrud.Delete(orthoHardwareNum);
		}

		///<summary>An elastic is actually a combination of multiple line segements. This calculates all the segments.</summary>
		public static List<OrthoWire> GetElastics(string toothRange,Color colorItem){
			List<OrthoWire> listOrthoWires=new List<OrthoWire>();
			if(toothRange.IsNullOrEmpty()) {
				return listOrthoWires;
			}
			string[] stringArrayTeeth=toothRange.Split(',');
			if(stringArrayTeeth.Length<2){
				return listOrthoWires;
			}
			for(int i=0;i<stringArrayTeeth.Length-1;i++){//loop does not include last tooth
				if(!Tooth.IsValidDB(stringArrayTeeth[i])){
					return listOrthoWires;
				}
				if(!Tooth.IsValidDB(stringArrayTeeth[i+1])){
					return listOrthoWires;
				}
				OrthoWire orthoWire=new OrthoWire();
				orthoWire.OrthoWireType=EnumOrthoWireType.Elastic;
				orthoWire.ColorDraw=colorItem;
				orthoWire.ToothIDstart=stringArrayTeeth[i];
				orthoWire.ToothIDend=stringArrayTeeth[i+1];
				listOrthoWires.Add(orthoWire);
			}
			return listOrthoWires;
		}

		///<summary>A single wire is actually a combination of a bunch of different wire segments. This calculates all the shorter wire segments.</summary>
		public static List<OrthoWire> GetWires(string toothRange,Color colorItem){
			List<OrthoWire> listOrthoWires=new List<OrthoWire>();
			if(toothRange.IsNullOrEmpty()) {
				return listOrthoWires;
			}
			if(toothRange.Contains("-")){
				string[] stringArrayTeeth=toothRange.Split('-');
				if(stringArrayTeeth.Length!=2){
					return listOrthoWires;
				}
				if(!Tooth.IsValidDB(stringArrayTeeth[0])){
					return listOrthoWires;
				}
				if(!Tooth.IsValidDB(stringArrayTeeth[1])){
					return listOrthoWires;
				}
				int int1=Tooth.ToInt(stringArrayTeeth[0]);
				int int2=Tooth.ToInt(stringArrayTeeth[1]);
				if(int1==int2){
					return listOrthoWires;
				}
				if(int1>int2){
					//flip them
					int temp=int1;
					int1=int2;
					int2=temp;
				}
				//They will all be in one arch
				for(int i=int1;i<=int2;i++){
					OrthoWire orthoWire=new OrthoWire();
					orthoWire.OrthoWireType=EnumOrthoWireType.InBracket;
					orthoWire.ColorDraw=colorItem;
					orthoWire.ToothIDstart=Tooth.FromInt(i);
					listOrthoWires.Add(orthoWire);
					if(i<=int2-1){//this gets skipped on the last item of loop
						orthoWire=new OrthoWire();
						orthoWire.OrthoWireType=EnumOrthoWireType.BetweenBrackets;
						orthoWire.ColorDraw=colorItem;
						orthoWire.ToothIDstart=Tooth.FromInt(i);
						orthoWire.ToothIDend=Tooth.FromInt(i+1);
						listOrthoWires.Add(orthoWire);
					}			
				}
			}
			if(toothRange.Contains(",")){

			}
			return listOrthoWires;
		}

		///<summary>Also used for ortho elastics.</summary>
		public class OrthoWire{
			public EnumOrthoWireType OrthoWireType;
			public string ToothIDstart;
			///<summary>Only used for BetweenBracket and Elastic.</summary>
			public string ToothIDend;
			public Color ColorDraw;
		}

		public enum EnumOrthoWireType{
			InBracket,
			BetweenBrackets,
			Elastic
		}

	}

	
}