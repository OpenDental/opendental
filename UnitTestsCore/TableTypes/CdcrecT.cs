using OpenDentBusiness;
using System.Collections.Generic;

namespace UnitTestsCore {
	public class CdcrecT {

		/// <summary>Fill the cdcrec table with a portion of the cdcrec actual data.</summary>
		public static void FillCdcrecTableIfNeeded() {
			if(Cdcrecs.GetAll().Count!=0) {
				return;
			}
			List<Cdcrec> listcdcrec=new List<Cdcrec>();
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1002-5",HeirarchicalCode="R1",Description="AMERICAN INDIAN OR ALASKA NATIVE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1004-1",HeirarchicalCode="R1.01",Description="AMERICAN INDIAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1054-6",HeirarchicalCode="R1.01.014.001",Description="CAHTO"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1060-3",HeirarchicalCode="R1.01.014.007",Description="MATTOLE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1066-0",HeirarchicalCode="R1.01.014.013",Description="YUKI"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1072-8",HeirarchicalCode="R1.01.015.004",Description="MEXICAN AMERICAN INDIAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1078-5",HeirarchicalCode="R1.01.017",Description="CAYUSE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1084-3",HeirarchicalCode="R1.01.019.002",Description="QUILEUTE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1091-8",HeirarchicalCode="R1.01.021.003",Description="CHEROKEES OF SOUTHEAST ALABAMA"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1096-7",HeirarchicalCode="R1.01.021.008",Description="TUSCOLA"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1102-3",HeirarchicalCode="R1.01.023",Description="CHEYENNE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1189-0",HeirarchicalCode="R1.01.044",Description="COWLITZ"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1197-3",HeirarchicalCode="R1.01.046.004",Description="EASTERN MUSCOGEE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1203-9",HeirarchicalCode="R1.01.046.010",Description="STAR CLAN OF MUSCOGEE CREEKS"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1358-1",HeirarchicalCode="R1.01.090",Description="MIAMI"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1368-0",HeirarchicalCode="R1.01.093",Description="MISSION INDIANS"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1382-1",HeirarchicalCode="R1.01.100",Description="NAVAJO"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1392-0",HeirarchicalCode="R1.01.103.001",Description="ALSEA"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1400-1",HeirarchicalCode="R1.01.103.009",Description="WENATCHEE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1412-6",HeirarchicalCode="R1.01.108.001",Description="BURT LAKE OTTAWA"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1421-7",HeirarchicalCode="R1.01.109.005",Description="FORT BIDWELL"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1429-0",HeirarchicalCode="R1.01.109.013",Description="NORTHERN PAIUTE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1436-5",HeirarchicalCode="R1.01.109.020",Description="WALKER RIVER"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1445-6",HeirarchicalCode="R1.01.112",Description="PAWNEE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1457-1",HeirarchicalCode="R1.01.116.001",Description="GILA RIVER PIMA-MARICOPA"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1466-2",HeirarchicalCode="R1.01.119.002",Description="DRY CREEK"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1474-6",HeirarchicalCode="R1.01.120",Description="PONCA"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1483-7",HeirarchicalCode="R1.01.121.005",Description="POKAGON POTAWATOMI"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1492-8",HeirarchicalCode="R1.01.123.003",Description="COCHITI"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1499-3",HeirarchicalCode="R1.01.123.010",Description="PICURIS"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1634-5",HeirarchicalCode="R1.01.145.025",Description="TETON SIOUX"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1640-2",HeirarchicalCode="R1.01.145.031",Description="YANKTON SIOUX"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1653-5",HeirarchicalCode="R1.01.151",Description="TOHONO O'ODHAM"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1663-4",HeirarchicalCode="R1.01.154",Description="TYGH"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1675-8",HeirarchicalCode="R1.01.158",Description="WAILAKI"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1687-3",HeirarchicalCode="R1.01.163",Description="WASHOE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1698-0",HeirarchicalCode="R1.01.166.002",Description="NEBRASKA WINNEBAGO"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1709-5",HeirarchicalCode="R1.01.171",Description="YAKAMA COWLITZ"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1719-4",HeirarchicalCode="R1.01.174.002",Description="TACHI"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="1729-3",HeirarchicalCode="R1.01.176.005",Description="MOHAVE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2033-9",HeirarchicalCode="R2.05",Description="CAMBODIAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2041-2",HeirarchicalCode="R2.13",Description="LAOTIAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2048-7",HeirarchicalCode="R2.20",Description="IWO JIMAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2060-2",HeirarchicalCode="R3.03",Description="AFRICAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2068-5",HeirarchicalCode="R3.05",Description="BARBADIAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2076-8",HeirarchicalCode="R4",Description="NATIVE HAWAIIAN OR OTHER PACIFIC ISLANDER"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2086-7",HeirarchicalCode="R4.02.001",Description="GUAMANIAN OR CHAMORRO"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2094-1",HeirarchicalCode="R4.02.009",Description="POHNPEIAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2103-0",HeirarchicalCode="R4.03.003",Description="SOLOMON ISLANDER"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2111-3",HeirarchicalCode="R5.01.003",Description="FRENCH"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2121-2",HeirarchicalCode="R5.02.003",Description="IRANIAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2131-1",HeirarchicalCode="R9",Description="OTHER RACE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2141-0",HeirarchicalCode="E1.01.004",Description="CATALONIAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2149-3",HeirarchicalCode="E1.02.001",Description="MEXICAN AMERICAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2157-6",HeirarchicalCode="E1.03.002",Description="GUATEMALAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2163-4",HeirarchicalCode="E1.03.008",Description="CANAL ZONE"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2172-5",HeirarchicalCode="E1.04.007",Description="PERUVIAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2178-2",HeirarchicalCode="E1.05",Description="LATIN AMERICAN"});
			listcdcrec.Add(new Cdcrec(){ CdcrecCode="2186-5",HeirarchicalCode="E2",Description="NOT HISPANIC OR LATINO"});
			for(int i = 0;i<listcdcrec.Count;i++) {
				Cdcrecs.Insert(listcdcrec[i]);
			}
		}
		public static void ClearCdcrecTable() {
			string command="DELETE FROM cdcrec WHERE CdcrecNum > 0";
			DataCore.NonQ(command);
		}

	}
}