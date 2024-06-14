using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Vitalsigns{
		///<summary></summary>
		public static List<Vitalsign> Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Vitalsign>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM vitalsign WHERE PatNum = "+POut.Long(patNum)+" ORDER BY DateTaken";
			return Crud.VitalsignCrud.SelectMany(command);
		}

		///<summary>Gets one Vitalsign from the db.</summary>
		public static Vitalsign GetOne(long vitalsignNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<Vitalsign>(MethodBase.GetCurrentMethod(),vitalsignNum);
			}
			return Crud.VitalsignCrud.SelectOne(vitalsignNum);
		}

		///<summary>Get most recent Vitalsign that has a valid height and weight from the db.</summary>
		public static Vitalsign GetOneWithValidHeightAndWeight(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Vitalsign>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=$@"
				SELECT *
				FROM vitalsign
				WHERE PatNum={POut.Long(patNum)}
				AND Height > 0
				AND Weight > 0
				ORDER BY DateTaken DESC";
			return Crud.VitalsignCrud.SelectOne(command);
		}

		///<summary>Gets one Vitalsign from the db.</summary>
		public static Vitalsign GetMostRecent(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Vitalsign>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM vitalsign WHERE PatNum = "+POut.Long(patNum)+" ORDER BY DateTaken DESC";
			return Crud.VitalsignCrud.SelectOne(command);
		}

		///<summary>Get vitalsign that this EhrNotPerformed object is linked to. Returns null if not found.</summary>
		public static Vitalsign GetFromEhrNotPerformedNum(long ehrNotPerfNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Vitalsign>(MethodBase.GetCurrentMethod(),ehrNotPerfNum);
			}
			string command="SELECT * FROM vitalsign WHERE EhrNotPerformedNum="+POut.Long(ehrNotPerfNum);
			return Crud.VitalsignCrud.SelectOne(command);
		}

		///<summary>Gets one Vitalsign with the given DiseaseNum as the PregDiseaseNum.</summary>
		public static List<Vitalsign> GetListFromPregDiseaseNum(long pregDiseaseNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Vitalsign>>(MethodBase.GetCurrentMethod(),pregDiseaseNum);				
			}
			string command="SELECT * FROM vitalsign WHERE vitalsign.PregDiseaseNum="+POut.Long(pregDiseaseNum);
			return Crud.VitalsignCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(Vitalsign vitalsign){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				vitalsign.VitalsignNum=Meth.GetLong(MethodBase.GetCurrentMethod(),vitalsign);
				return vitalsign.VitalsignNum;
			}
			return Crud.VitalsignCrud.Insert(vitalsign);
		}

		///<summary></summary>
		public static void Update(Vitalsign vitalsign){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),vitalsign);
				return;
			}
			Crud.VitalsignCrud.Update(vitalsign);
		}

		///<summary></summary>
		public static bool Update(Vitalsign vitalsign,Vitalsign vitalsignOld){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetBool(MethodBase.GetCurrentMethod(),vitalsign,vitalsignOld);
			}
			return Crud.VitalsignCrud.Update(vitalsign,vitalsignOld);
		}

		///<summary></summary>
		public static void Delete(long vitalsignNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),vitalsignNum);
				return;
			}
			string command= "DELETE FROM vitalsign WHERE VitalsignNum = "+POut.Long(vitalsignNum);
			Db.NonQ(command);
		}

		public static float CalcBMI(float weight,float height) {
			//No need to check MiddleTierRole; no call to db.
			//BMI = (lbs*703)/(in^2)
			if(weight==0 || height==0) {
				return 0;
			}
			float bmi = (float)((weight*703f)/(height*height));
			return bmi;
		}

		///<summary>Takes in bmi and age and returns an intervention code. Possible return codes are None, Nutrition, BelowNormalWeight, and AboveNormalWeight.</summary>
		public static InterventionCodeSet GetBMIInterventionCode(float bmi, int ageBeforeJanFirst) {
			if(ageBeforeJanFirst<18) {//Do not clasify children as over/underweight
				return InterventionCodeSet.Nutrition;//we will sent Nutrition to FormInterventionEdit, but this could also be a physical activity intervention
			}
			if(ageBeforeJanFirst<65) {
				if(bmi<18.5) {
					return InterventionCodeSet.BelowNormalWeight;
				}
				if(bmi<25) {
					return InterventionCodeSet.None;
				}
				return InterventionCodeSet.AboveNormalWeight;
			}
			if(bmi < 23) {
				return InterventionCodeSet.BelowNormalWeight;
			}
			if(bmi < 30) {
				return InterventionCodeSet.None;
			}
			return InterventionCodeSet.AboveNormalWeight;
		}

		///<summary>Fills a list with GenderAge_LMS objects. The GenderAge field is a string containing the gender (m or f) and age in months. The LMS field is list of floats, the L=power of Box-Cox transformation, M=median, 
		///and S=generalized coefficient of variation. The L, M, and S values are from the CDC website http://www.cdc.gov/nchs/data/series/sr_11/sr11_246.pdf page 178-186.</summary>
		public static List<GenderAge_LMS> GetListLMS() {
			List<GenderAge_LMS> listGenderAge_LMSs=new List<GenderAge_LMS>();
			#region Add LMS values for each Gender-Age group
			listGenderAge_LMSs.Add(new GenderAge_LMS("m36",new List<float>() { -1.419991255f,16.00030401f,0.072634432f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m37",new List<float>() { -1.404277619f,15.96304277f,0.072327649f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m38",new List<float>() { -1.39586317f,15.92695418f,0.07206864f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m39",new List<float>() { -1.394935252f,15.89202582f,0.071856805f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m40",new List<float>() { -1.401671596f,15.85824093f,0.071691278f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m41",new List<float>() { -1.416100312f,15.82558822f,0.071571093f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m42",new List<float>() { -1.438164899f,15.79405728f,0.071495113f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m43",new List<float>() { -1.467669032f,15.76364255f,0.071462106f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m44",new List<float>() { -1.504376347f,15.73433668f,0.071470646f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m45",new List<float>() { -1.547942838f,15.70613566f,0.071519218f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m46",new List<float>() { -1.597896397f,15.67904062f,0.071606277f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m47",new List<float>() { -1.653732283f,15.65305192f,0.071730167f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m48",new List<float>() { -1.714869347f,15.62817269f,0.071889214f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m49",new List<float>() { -1.780673181f,15.604408f,0.072081737f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m50",new List<float>() { -1.850468473f,15.58176458f,0.072306081f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m51",new List<float>() { -1.923551865f,15.56025067f,0.072560637f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m52",new List<float>() { -1.999220429f,15.5398746f,0.07284384f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m53",new List<float>() { -2.076707178f,15.52064993f,0.073154324f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m54",new List<float>() { -2.155348017f,15.50258427f,0.073490667f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m55",new List<float>() { -2.234438552f,15.48568973f,0.073851672f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m56",new List<float>() { -2.313321723f,15.46997718f,0.074236235f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m57",new List<float>() { -2.391381273f,15.45545692f,0.074643374f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m58",new List<float>() { -2.468032491f,15.44213961f,0.075072264f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m59",new List<float>() { -2.542781541f,15.43003207f,0.075522104f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m60",new List<float>() { -2.61516595f,15.41914163f,0.07599225f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m61",new List<float>() { -2.684789516f,15.40947356f,0.076482128f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m62",new List<float>() { -2.751316949f,15.40103139f,0.076991232f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m63",new List<float>() { -2.81445945f,15.39381785f,0.077519149f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m64",new List<float>() { -2.87402476f,15.38783094f,0.07806539f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m65",new List<float>() { -2.92984048f,15.38306945f,0.078629592f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m66",new List<float>() { -2.981796828f,15.37952958f,0.079211369f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m67",new List<float>() { -3.029831343f,15.37720582f,0.079810334f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m68",new List<float>() { -3.073924224f,15.37609107f,0.080426086f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m69",new List<float>() { -3.114093476f,15.37617677f,0.081058206f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m70",new List<float>() { -3.15039004f,15.37745304f,0.081706249f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m71",new List<float>() { -3.182893018f,15.37990886f,0.082369741f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m72",new List<float>() { -3.21170511f,15.38353217f,0.083048178f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m73",new List<float>() { -3.23694834f,15.38831005f,0.083741021f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m74",new List<float>() { -3.25876011f,15.39422883f,0.0844477f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m75",new List<float>() { -3.277281546f,15.40127496f,0.085167651f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m76",new List<float>() { -3.292683774f,15.40943252f,0.085900184f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m77",new List<float>() { -3.305124073f,15.41868691f,0.086644667f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m78",new List<float>() { -3.314768951f,15.42902273f,0.087400421f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m79",new List<float>() { -3.321785992f,15.44042439f,0.088166744f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m80",new List<float>() { -3.326345795f,15.45287581f,0.088942897f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m81",new List<float>() { -3.328602731f,15.46636218f,0.089728202f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m82",new List<float>() { -3.328725277f,15.48086704f,0.090521875f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m83",new List<float>() { -3.32687018f,15.49637465f,0.091323162f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m84",new List<float>() { -3.323188896f,15.51286936f,0.092131305f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m85",new List<float>() { -3.317827016f,15.53033563f,0.092945544f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m86",new List<float>() { -3.310923871f,15.54875807f,0.093765118f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m87",new List<float>() { -3.302612272f,15.56812143f,0.09458927f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m88",new List<float>() { -3.293018361f,15.58841065f,0.095417247f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m89",new List<float>() { -3.282260813f,15.60961101f,0.096248301f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m90",new List<float>() { -3.270454609f,15.63170735f,0.097081694f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m91",new List<float>() { -3.257703616f,15.65468563f,0.097916698f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m92",new List<float>() { -3.244108214f,15.67853139f,0.098752593f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m93",new List<float>() { -3.229761713f,15.70323052f,0.099588675f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m94",new List<float>() { -3.214751287f,15.72876911f,0.100424251f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m95",new List<float>() { -3.199158184f,15.75513347f,0.101258643f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m96",new List<float>() { -3.18305795f,15.78231007f,0.102091189f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m97",new List<float>() { -3.166520664f,15.8102856f,0.102921245f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m98",new List<float>() { -3.1496103f,15.83904708f,0.103748189f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m99",new List<float>() { -3.132389637f,15.86858123f,0.104571386f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m100",new List<float>() { -3.114911153f,15.89887562f,0.105390269f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m101",new List<float>() { -3.097226399f,15.92991765f,0.106204258f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m102",new List<float>() { -3.079383079f,15.96169481f,0.107012788f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m103",new List<float>() { -3.061423765f,15.99419489f,0.107815327f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m104",new List<float>() { -3.043386071f,16.02740607f,0.108611374f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m105",new List<float>() { -3.025310003f,16.0613159f,0.109400388f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m106",new List<float>() { -3.007225737f,16.09591292f,0.110181915f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m107",new List<float>() { -2.989164598f,16.13118532f,0.110955478f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m108",new List<float>() { -2.971148225f,16.16712234f,0.111720691f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m109",new List<float>() { -2.953208047f,16.20371168f,0.112477059f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m110",new List<float>() { -2.935363951f,16.24094239f,0.1132242f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m111",new List<float>() { -2.917635157f,16.27880346f,0.113961734f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m112",new List<float>() { -2.900039803f,16.31728385f,0.114689291f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m113",new List<float>() { -2.882593796f,16.35637267f,0.115406523f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m114",new List<float>() { -2.865311266f,16.39605916f,0.116113097f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m115",new List<float>() { -2.848204697f,16.43633265f,0.116808702f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m116",new List<float>() { -2.831285052f,16.47718256f,0.117493042f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m117",new List<float>() { -2.81456189f,16.51859843f,0.11816584f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m118",new List<float>() { -2.79804347f,16.56056987f,0.118826835f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m119",new List<float>() { -2.781736856f,16.60308661f,0.119475785f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m120",new List<float>() { -2.765648008f,16.64613844f,0.120112464f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m121",new List<float>() { -2.749782197f,16.68971518f,0.120736656f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m122",new List<float>() { -2.734142443f,16.73380695f,0.121348181f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m123",new List<float>() { -2.718732873f,16.77840363f,0.121946849f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m124",new List<float>() { -2.703555506f,16.82349538f,0.122532501f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m125",new List<float>() { -2.688611957f,16.86907238f,0.123104991f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m126",new List<float>() { -2.673903164f,16.91512487f,0.123664186f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m127",new List<float>() { -2.659429443f,16.96164317f,0.124209969f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m128",new List<float>() { -2.645190534f,17.00861766f,0.124742239f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m129",new List<float>() { -2.631185649f,17.05603879f,0.125260905f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m130",new List<float>() { -2.617413511f,17.10389705f,0.125765895f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m131",new List<float>() { -2.603872392f,17.15218302f,0.126257147f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m132",new List<float>() { -2.590560148f,17.20088732f,0.126734613f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m133",new List<float>() { -2.577474253f,17.25000062f,0.12719826f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m134",new List<float>() { -2.564611831f,17.29951367f,0.127648067f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m135",new List<float>() { -2.551969684f,17.34941726f,0.128084023f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m136",new List<float>() { -2.539539972f,17.39970308f,0.128506192f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m137",new List<float>() { -2.527325681f,17.45036072f,0.128914497f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m138",new List<float>() { -2.515320235f,17.50138161f,0.129309001f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m139",new List<float>() { -2.503519447f,17.55275674f,0.129689741f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m140",new List<float>() { -2.491918934f,17.60447714f,0.130056765f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m141",new List<float>() { -2.480514136f,17.6565339f,0.130410133f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m142",new List<float>() { -2.469300331f,17.70891811f,0.130749913f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m143",new List<float>() { -2.458272656f,17.76162094f,0.131076187f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m144",new List<float>() { -2.447426113f,17.81463359f,0.131389042f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m145",new List<float>() { -2.436755595f,17.86794729f,0.131688579f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m146",new List<float>() { -2.426255887f,17.92155332f,0.131974905f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m147",new List<float>() { -2.415921689f,17.97544299f,0.132248138f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m148",new List<float>() { -2.405747619f,18.02960765f,0.132508403f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m149",new List<float>() { -2.395728233f,18.08403868f,0.132755834f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m150",new List<float>() { -2.385858029f,18.1387275f,0.132990575f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m151",new List<float>() { -2.376131459f,18.19366555f,0.133212776f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m152",new List<float>() { -2.366542942f,18.24884431f,0.133422595f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m153",new List<float>() { -2.357086871f,18.3042553f,0.133620197f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m154",new List<float>() { -2.347757625f,18.35989003f,0.133805756f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m155",new List<float>() { -2.338549576f,18.41574009f,0.133979452f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m156",new List<float>() { -2.3294571f,18.47179706f,0.13414147f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m157",new List<float>() { -2.320474586f,18.52805255f,0.134292005f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m158",new List<float>() { -2.311596446f,18.5844982f,0.134431256f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m159",new List<float>() { -2.302817124f,18.64112567f,0.134559427f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m160",new List<float>() { -2.294131107f,18.69792663f,0.134676731f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m161",new List<float>() { -2.285532933f,18.75489278f,0.134783385f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m162",new List<float>() { -2.277017201f,18.81201584f,0.134879611f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m163",new List<float>() { -2.268578584f,18.86928753f,0.134965637f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m164",new List<float>() { -2.260211837f,18.92669959f,0.135041695f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m165",new List<float>() { -2.251911809f,18.98424378f,0.135108024f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m166",new List<float>() { -2.243673453f,19.04191185f,0.135164867f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m167",new List<float>() { -2.235491842f,19.09969557f,0.135212469f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m168",new List<float>() { -2.227362173f,19.15758672f,0.135251083f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m169",new List<float>() { -2.21927979f,19.21557707f,0.135280963f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m170",new List<float>() { -2.211240187f,19.27365839f,0.135302371f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m171",new List<float>() { -2.203239029f,19.33182247f,0.135315568f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m172",new List<float>() { -2.195272161f,19.39006106f,0.135320824f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m173",new List<float>() { -2.187335625f,19.44836594f,0.135318407f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m174",new List<float>() { -2.179425674f,19.50672885f,0.135308594f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m175",new List<float>() { -2.171538789f,19.56514153f,0.135291662f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m176",new List<float>() { -2.163671689f,19.62359571f,0.135267891f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m177",new List<float>() { -2.155821357f,19.6820831f,0.135237567f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m178",new List<float>() { -2.147985046f,19.74059538f,0.135200976f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m179",new List<float>() { -2.140160305f,19.7991242f,0.135158409f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m180",new List<float>() { -2.132344989f,19.85766121f,0.135110159f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m181",new List<float>() { -2.124537282f,19.916198f,0.135056522f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m182",new List<float>() { -2.116735712f,19.97472615f,0.134997797f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m183",new List<float>() { -2.108939167f,20.03323719f,0.134934285f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m184",new List<float>() { -2.10114692f,20.09172262f,0.134866291f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m185",new List<float>() { -2.093358637f,20.15017387f,0.134794121f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m186",new List<float>() { -2.085574403f,20.20858236f,0.134718085f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m187",new List<float>() { -2.077794735f,20.26693944f,0.134638494f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m188",new List<float>() { -2.070020599f,20.32523642f,0.134555663f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m189",new List<float>() { -2.062253431f,20.38346455f,0.13446991f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m190",new List<float>() { -2.054495145f,20.44161501f,0.134381553f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m191",new List<float>() { -2.046748156f,20.49967894f,0.134290916f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m192",new List<float>() { -2.039015385f,20.5576474f,0.134198323f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m193",new List<float>() { -2.031300282f,20.6155114f,0.134104101f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m194",new List<float>() { -2.023606828f,20.67326189f,0.134008581f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m195",new List<float>() { -2.015942013f,20.73088905f,0.133912066f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m196",new List<float>() { -2.008305745f,20.7883851f,0.133814954f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m197",new List<float>() { -2.000706389f,20.84574003f,0.133717552f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m198",new List<float>() { -1.993150137f,20.90294449f,0.1336202f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m199",new List<float>() { -1.985643741f,20.95998909f,0.133523244f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m200",new List<float>() { -1.97819451f,21.01686433f,0.133427032f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m201",new List<float>() { -1.970810308f,21.07356067f,0.133331914f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m202",new List<float>() { -1.96349954f,21.1300685f,0.133238245f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m203",new List<float>() { -1.956271141f,21.18637813f,0.133146383f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m204",new List<float>() { -1.949134561f,21.24247982f,0.13305669f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m205",new List<float>() { -1.942099744f,21.29836376f,0.132969531f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m206",new List<float>() { -1.935177101f,21.35402009f,0.132885274f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m207",new List<float>() { -1.92837748f,21.40943891f,0.132804292f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m208",new List<float>() { -1.921712136f,21.46461026f,0.132726962f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m209",new List<float>() { -1.915192685f,21.51952414f,0.132653664f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m210",new List<float>() { -1.908831065f,21.57417053f,0.132584784f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m211",new List<float>() { -1.902639482f,21.62853937f,0.132520711f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m212",new List<float>() { -1.896630358f,21.68262062f,0.132461838f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m213",new List<float>() { -1.890816268f,21.73640419f,0.132408563f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m214",new List<float>() { -1.885209876f,21.78988003f,0.132361289f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("m215",new List<float>() { -1.879823505f,21.84303819f,0.132320427f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f36",new List<float>() { -2.096828937f,15.69924188f,0.078605255f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f37",new List<float>() { -2.189211877f,15.65523282f,0.078378696f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f38",new List<float>() { -2.279991982f,15.61321371f,0.078196674f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f39",new List<float>() { -2.368732949f,15.57316843f,0.078058667f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f40",new List<float>() { -2.455021314f,15.53508019f,0.077964169f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f41",new List<float>() { -2.538471972f,15.49893145f,0.077912684f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f42",new List<float>() { -2.618732901f,15.46470384f,0.077903716f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f43",new List<float>() { -2.695488973f,15.43237817f,0.077936763f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f44",new List<float>() { -2.768464816f,15.40193436f,0.078011309f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f45",new List<float>() { -2.837426693f,15.37335154f,0.078126817f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f46",new List<float>() { -2.902178205f,15.34660842f,0.078282739f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f47",new List<float>() { -2.962580386f,15.32168181f,0.078478449f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f48",new List<float>() { -3.018521987f,15.29854897f,0.078713325f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f49",new List<float>() { -3.069936555f,15.27718618f,0.078986694f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f50",new List<float>() { -3.116795864f,15.2575692f,0.079297841f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f51",new List<float>() { -3.159107331f,15.23967338f,0.079646006f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f52",new List<float>() { -3.196911083f,15.22347371f,0.080030389f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f53",new List<float>() { -3.230276759f,15.20894491f,0.080450145f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f54",new List<float>() { -3.259300182f,15.19606152f,0.080904391f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f55",new List<float>() { -3.284099963f,15.18479799f,0.081392203f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f56",new List<float>() { -3.30481415f,15.17512871f,0.081912623f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f57",new List<float>() { -3.321596954f,15.16702811f,0.082464661f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f58",new List<float>() { -3.334615646f,15.16047068f,0.083047295f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f59",new List<float>() { -3.344047622f,15.15543107f,0.083659478f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f60",new List<float>() { -3.35007771f,15.15188405f,0.084300139f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f61",new List<float>() { -3.352893805f,15.14980479f,0.0849682f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f62",new List<float>() { -3.352691376f,15.14916825f,0.085662539f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f63",new List<float>() { -3.34966438f,15.14994984f,0.086382035f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f64",new List<float>() { -3.343998803f,15.15212585f,0.087125591f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f65",new List<float>() { -3.335889574f,15.15567186f,0.087892047f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f66",new List<float>() { -3.325522491f,15.16056419f,0.088680264f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f67",new List<float>() { -3.31307846f,15.16677947f,0.089489106f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f68",new List<float>() { -3.298732648f,15.17429464f,0.090317434f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f69",new List<float>() { -3.282653831f,15.18308694f,0.091164117f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f70",new List<float>() { -3.265003896f,15.1931339f,0.092028028f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f71",new List<float>() { -3.245937506f,15.20441335f,0.092908048f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f72",new List<float>() { -3.225606516f,15.21690296f,0.093803033f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f73",new List<float>() { -3.204146115f,15.2305815f,0.094711916f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f74",new List<float>() { -3.181690237f,15.24542745f,0.095633595f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f75",new List<float>() { -3.158363475f,15.26141966f,0.096566992f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f76",new List<float>() { -3.134282833f,15.27853728f,0.097511046f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f77",new List<float>() { -3.109557879f,15.29675967f,0.09846471f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f78",new List<float>() { -3.084290931f,15.31606644f,0.099426955f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f79",new List<float>() { -3.058577292f,15.33643745f,0.100396769f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f80",new List<float>() { -3.032505499f,15.35785274f,0.101373159f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f81",new List<float>() { -3.0061576f,15.38029261f,0.10235515f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f82",new List<float>() { -2.979609448f,15.40373754f,0.103341788f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f83",new List<float>() { -2.952930993f,15.42816819f,0.104332139f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f84",new List<float>() { -2.926186592f,15.45356545f,0.105325289f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f85",new List<float>() { -2.899435307f,15.47991037f,0.106320346f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f86",new List<float>() { -2.872731211f,15.50718419f,0.10731644f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f87",new List<float>() { -2.846123683f,15.53536829f,0.108312721f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f88",new List<float>() { -2.819657704f,15.56444426f,0.109308364f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f89",new List<float>() { -2.793374145f,15.5943938f,0.110302563f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f90",new List<float>() { -2.767310047f,15.6251988f,0.111294537f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f91",new List<float>() { -2.741498897f,15.65684126f,0.112283526f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f92",new List<float>() { -2.715970894f,15.68930333f,0.113268793f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f93",new List<float>() { -2.690753197f,15.7225673f,0.114249622f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f94",new List<float>() { -2.665870146f,15.75661555f,0.115225321f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f95",new List<float>() { -2.641343436f,15.79143062f,0.116195218f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f96",new List<float>() { -2.617192204f,15.82699517f,0.117158667f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f97",new List<float>() { -2.593430614f,15.86329241f,0.118115073f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f98",new List<float>() { -2.570076037f,15.90030484f,0.119063807f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f99",new List<float>() { -2.547141473f,15.93801545f,0.12000429f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f100",new List<float>() { -2.524635245f,15.97640787f,0.120935994f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f101",new List<float>() { -2.502569666f,16.01546483f,0.121858355f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f102",new List<float>() { -2.48095189f,16.05516984f,0.12277087f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f103",new List<float>() { -2.459785573f,16.09550688f,0.123673085f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f104",new List<float>() { -2.439080117f,16.13645881f,0.124564484f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f105",new List<float>() { -2.418838304f,16.17800955f,0.125444639f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f106",new List<float>() { -2.399063683f,16.22014281f,0.126313121f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f107",new List<float>() { -2.379756861f,16.26284277f,0.127169545f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f108",new List<float>() { -2.360920527f,16.30609316f,0.128013515f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f109",new List<float>() { -2.342557728f,16.34987759f,0.128844639f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f110",new List<float>() { -2.324663326f,16.39418118f,0.129662637f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f111",new List<float>() { -2.307240716f,16.43898741f,0.130467138f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f112",new List<float>() { -2.290287663f,16.48428082f,0.131257852f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f113",new List<float>() { -2.273803847f,16.53004554f,0.132034479f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f114",new List<float>() { -2.257782149f,16.57626713f,0.132796819f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f115",new List<float>() { -2.242227723f,16.62292864f,0.133544525f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f116",new List<float>() { -2.227132805f,16.67001572f,0.134277436f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f117",new List<float>() { -2.212495585f,16.71751288f,0.134995324f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f118",new List<float>() { -2.19831275f,16.76540496f,0.135697996f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f119",new List<float>() { -2.184580762f,16.81367689f,0.136385276f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f120",new List<float>() { -2.171295888f,16.86231366f,0.137057004f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f121",new List<float>() { -2.158454232f,16.91130036f,0.137713039f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f122",new List<float>() { -2.146051754f,16.96062216f,0.138353254f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f123",new List<float>() { -2.134084303f,17.0102643f,0.138977537f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f124",new List<float>() { -2.122547629f,17.06021213f,0.139585795f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f125",new List<float>() { -2.111437411f,17.11045106f,0.140177947f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f126",new List<float>() { -2.100749266f,17.16096656f,0.140753927f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f127",new List<float>() { -2.090478774f,17.21174424f,0.141313686f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f128",new List<float>() { -2.080621484f,17.26276973f,0.141857186f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f129",new List<float>() { -2.071172932f,17.31402878f,0.142384404f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f130",new List<float>() { -2.062128649f,17.3655072f,0.142895332f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f131",new List<float>() { -2.053484173f,17.4171909f,0.143389972f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f132",new List<float>() { -2.045235058f,17.46906585f,0.143868341f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f133",new List<float>() { -2.03737688f,17.52111811f,0.144330469f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f134",new List<float>() { -2.029906684f,17.57333347f,0.144776372f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f135",new List<float>() { -2.022817914f,17.62569869f,0.145206138f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f136",new List<float>() { -2.016107084f,17.67819987f,0.145619819f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f137",new List<float>() { -2.009769905f,17.7308234f,0.146017491f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f138",new List<float>() { -2.003802134f,17.78355575f,0.146399239f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f139",new List<float>() { -1.998199572f,17.83638347f,0.146765161f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f140",new List<float>() { -1.992958064f,17.88929321f,0.147115364f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f141",new List<float>() { -1.988073505f,17.94227168f,0.147449967f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f142",new List<float>() { -1.983541835f,17.9953057f,0.147769097f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f143",new List<float>() { -1.979359041f,18.04838216f,0.148072891f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f144",new List<float>() { -1.975521156f,18.10148804f,0.148361495f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f145",new List<float>() { -1.972024258f,18.15461039f,0.148635067f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f146",new List<float>() { -1.968864465f,18.20773639f,0.148893769f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f147",new List<float>() { -1.966037938f,18.26085325f,0.149137776f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f148",new List<float>() { -1.963540872f,18.31394832f,0.14936727f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f149",new List<float>() { -1.961369499f,18.36700902f,0.149582439f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f150",new List<float>() { -1.959520079f,18.42002284f,0.149783482f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f151",new List<float>() { -1.9579889f,18.47297739f,0.149970604f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f152",new List<float>() { -1.956772271f,18.52586035f,0.15014402f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f153",new List<float>() { -1.95586652f,18.57865951f,0.15030395f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f154",new List<float>() { -1.955267984f,18.63136275f,0.150450621f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f155",new List<float>() { -1.954973011f,18.68395801f,0.15058427f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f156",new List<float>() { -1.954977947f,18.73643338f,0.150705138f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f157",new List<float>() { -1.955279136f,18.788777f,0.150813475f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f158",new List<float>() { -1.955872909f,18.84097713f,0.150909535f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f159",new List<float>() { -1.956755579f,18.89302212f,0.150993582f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f160",new List<float>() { -1.957923436f,18.94490041f,0.151065883f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f161",new List<float>() { -1.959372737f,18.99660055f,0.151126714f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f162",new List<float>() { -1.9610997f,19.04811118f,0.151176355f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f163",new List<float>() { -1.963100496f,19.09942105f,0.151215094f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f164",new List<float>() { -1.96537124f,19.15051899f,0.151243223f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f165",new List<float>() { -1.967907983f,19.20139397f,0.151261042f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f166",new List<float>() { -1.970706706f,19.25203503f,0.151268855f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f167",new List<float>() { -1.973763307f,19.30243131f,0.151266974f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f168",new List<float>() { -1.977073595f,19.35257209f,0.151255713f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f169",new List<float>() { -1.980633277f,19.40244671f,0.151235395f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f170",new List<float>() { -1.984437954f,19.45204465f,0.151206347f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f171",new List<float>() { -1.988483106f,19.50135548f,0.151168902f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f172",new List<float>() { -1.992764085f,19.55036888f,0.151123398f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f173",new List<float>() { -1.997276103f,19.59907464f,0.15107018f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f174",new List<float>() { -2.002014224f,19.64746266f,0.151009595f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f175",new List<float>() { -2.00697335f,19.69552294f,0.150942f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f176",new List<float>() { -2.012148213f,19.7432456f,0.150867753f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f177",new List<float>() { -2.017533363f,19.79062086f,0.150787221f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f178",new List<float>() { -2.023123159f,19.83763907f,0.150700774f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f179",new List<float>() { -2.028911755f,19.88429066f,0.150608788f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f180",new List<float>() { -2.034893091f,19.9305662f,0.150511645f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f181",new List<float>() { -2.041060881f,19.97645636f,0.150409731f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f182",new List<float>() { -2.047408604f,20.02195192f,0.15030344f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f183",new List<float>() { -2.05392949f,20.06704377f,0.150193169f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f184",new List<float>() { -2.060616513f,20.11172291f,0.150079322f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f185",new List<float>() { -2.067462375f,20.15598047f,0.149962308f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f186",new List<float>() { -2.074459502f,20.19980767f,0.14984254f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f187",new List<float>() { -2.081600029f,20.24319586f,0.149720441f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f188",new List<float>() { -2.088875793f,20.28613648f,0.149596434f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f189",new List<float>() { -2.096278323f,20.32862109f,0.149470953f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f190",new List<float>() { -2.103798828f,20.37064138f,0.149344433f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f191",new List<float>() { -2.111428194f,20.41218911f,0.149217319f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f192",new List<float>() { -2.119156972f,20.45325617f,0.14909006f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f193",new List<float>() { -2.126975375f,20.49383457f,0.14896311f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f194",new List<float>() { -2.134873266f,20.5339164f,0.148836931f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f195",new List<float>() { -2.142840157f,20.57349387f,0.148711989f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f196",new List<float>() { -2.150865204f,20.61255929f,0.148588757f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f197",new List<float>() { -2.158937201f,20.65110506f,0.148467715f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f198",new List<float>() { -2.167044578f,20.6891237f,0.148349348f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f199",new List<float>() { -2.175176987f,20.72660728f,0.14823412f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f200",new List<float>() { -2.183317362f,20.76355011f,0.148122614f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f201",new List<float>() { -2.191457792f,20.79994337f,0.148015249f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f202",new List<float>() { -2.199583649f,20.83578051f,0.147912564f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f203",new List<float>() { -2.207681525f,20.87105449f,0.147815078f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f204",new List<float>() { -2.215737645f,20.90575839f,0.147723315f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f205",new List<float>() { -2.223739902f,20.93988477f,0.147637768f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f206",new List<float>() { -2.231667995f,20.97342858f,0.147559083f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f207",new List<float>() { -2.239511942f,21.00638171f,0.147487716f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f208",new List<float>() { -2.247257081f,21.0387374f,0.14742421f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f209",new List<float>() { -2.254885145f,21.07048996f,0.147369174f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f210",new List<float>() { -2.26238209f,21.10163241f,0.147323144f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f211",new List<float>() { -2.269731517f,21.13215845f,0.147286698f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f212",new List<float>() { -2.276917229f,21.16206171f,0.147260415f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f213",new List<float>() { -2.283925442f,21.1913351f,0.147244828f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f214",new List<float>() { -2.290731442f,21.21997472f,0.147240683f }));
			listGenderAge_LMSs.Add(new GenderAge_LMS("f215",new List<float>() { -2.29732427f,21.24797262f,0.147248467f }));
			#endregion
			return listGenderAge_LMSs;
		}

		///<summary>Returns the percentile for BMI the passed in z-score is in.</summary>
		public static int GetBMIPercentileUsingZScore(float z) {
			List<float> listPercentiles=new List<float>();
			#region Add All Percentiles z-score Table Lookup
			listPercentiles.Add(-2.325f);//0th percentile
			listPercentiles.Add(-2.055f);//1st percentile
			listPercentiles.Add(-1.885f);//2nd percentile
			listPercentiles.Add(-1.755f);//3rd
			listPercentiles.Add(-1.645f);//4th
			listPercentiles.Add(-1.555f);//5th
			listPercentiles.Add(-1.475f);//6th
			listPercentiles.Add(-1.405f);//7th
			listPercentiles.Add(-1.345f);//8th
			listPercentiles.Add(-1.285f);//9th
			listPercentiles.Add(-1.225f);//10th
			listPercentiles.Add(-1.175f);//11th
			listPercentiles.Add(-1.125f);//12th
			listPercentiles.Add(-1.085f);//13th
			listPercentiles.Add(-1.035f);//14th
			listPercentiles.Add(-0.995f);//15th
			listPercentiles.Add(-0.955f);//16th
			listPercentiles.Add(-0.915f);//17th
			listPercentiles.Add(-0.875f);//18th
			listPercentiles.Add(-0.845f);//19th
			listPercentiles.Add(-0.805f);//20th
			listPercentiles.Add(-0.775f);//21st
			listPercentiles.Add(-0.735f);//22nd
			listPercentiles.Add(-0.705f);//23rd
			listPercentiles.Add(-0.675f);//24th
			listPercentiles.Add(-0.645f);//25th
			listPercentiles.Add(-0.615f);//26th
			listPercentiles.Add(-0.585f);//27th
			listPercentiles.Add(-0.555f);//28th
			listPercentiles.Add(-0.525f);//29th
			listPercentiles.Add(-0.495f);//30th
			listPercentiles.Add(-0.465f);//31st
			listPercentiles.Add(-0.435f);//32nd
			listPercentiles.Add(-0.415f);//33rd
			listPercentiles.Add(-0.385f);//34th
			listPercentiles.Add(-0.355f);//35th
			listPercentiles.Add(-0.335f);//36th
			listPercentiles.Add(-0.305f);//37th
			listPercentiles.Add(-0.275f);//38th
			listPercentiles.Add(-0.255f);//39th
			listPercentiles.Add(-0.225f);//40th
			listPercentiles.Add(-0.205f);//41st
			listPercentiles.Add(-0.175f);//42nd
			listPercentiles.Add(-0.155f);//43rd
			listPercentiles.Add(-0.125f);//44th
			listPercentiles.Add(-0.105f);//45th
			listPercentiles.Add(-0.075f);//46th
			listPercentiles.Add(-0.055f);//47th
			listPercentiles.Add(-0.025f);//48th
			listPercentiles.Add(-0.005f);//49th
			listPercentiles.Add(0.025f);//50th
			listPercentiles.Add(0.055f);//51st
			listPercentiles.Add(0.075f);//52nd
			listPercentiles.Add(0.105f);//53rd
			listPercentiles.Add(0.125f);//54th
			listPercentiles.Add(0.155f);//55th
			listPercentiles.Add(0.175f);//56th
			listPercentiles.Add(0.205f);//57th
			listPercentiles.Add(0.225f);//58th
			listPercentiles.Add(0.255f);//59th
			listPercentiles.Add(0.275f);//60th
			listPercentiles.Add(0.305f);//61st
			listPercentiles.Add(0.335f);//62nd
			listPercentiles.Add(0.355f);//63rd
			listPercentiles.Add(0.385f);//64th
			listPercentiles.Add(0.415f);//65th
			listPercentiles.Add(0.435f);//66th
			listPercentiles.Add(0.465f);//67th
			listPercentiles.Add(0.495f);//68th
			listPercentiles.Add(0.525f);//69th
			listPercentiles.Add(0.555f);//70th
			listPercentiles.Add(0.585f);//71st
			listPercentiles.Add(0.615f);//72nd
			listPercentiles.Add(0.645f);//73rd
			listPercentiles.Add(0.675f);//74th
			listPercentiles.Add(0.705f);//75th
			listPercentiles.Add(0.735f);//76th
			listPercentiles.Add(0.775f);//77th
			listPercentiles.Add(0.805f);//78th
			listPercentiles.Add(0.845f);//79th
			listPercentiles.Add(0.875f);//80th
			listPercentiles.Add(0.915f);//81st
			listPercentiles.Add(0.955f);//82nd
			listPercentiles.Add(0.995f);//83rd
			listPercentiles.Add(1.035f);//84th
			listPercentiles.Add(1.085f);//85th
			listPercentiles.Add(1.125f);//86th
			listPercentiles.Add(1.175f);//87th
			listPercentiles.Add(1.225f);//88th
			listPercentiles.Add(1.285f);//89th
			listPercentiles.Add(1.345f);//90th
			listPercentiles.Add(1.405f);//91st
			listPercentiles.Add(1.475f);//92nd
			listPercentiles.Add(1.555f);//93rd
			listPercentiles.Add(1.645f);//94th
			listPercentiles.Add(1.755f);//95th
			listPercentiles.Add(1.885f);//96th
			listPercentiles.Add(2.055f);//97th
			listPercentiles.Add(2.325f);//98th
			listPercentiles.Add(float.MaxValue);//99th
			#endregion
			for(int i=0;i<listPercentiles.Count;i++) {
				if(z<listPercentiles[i]) {
					return i;
				}
			}
			return -1;
		}

		///<summary>Should only be called if patient is under 18 at the time of the exam. Calculates BMI percentile.</summary>
		public static int GetBMIPercentile(float bmi,Patient patient,DateTime dateExam,List<GenderAge_LMS> listGenderAge_LMSs) {
			//get age at time of exam for BMI percentile in months.  Examples: 13 years 11 months = 13*12+11 = 167.
			if(dateExam==DateTime.MinValue || dateExam<patient.Birthdate || patient.Birthdate==DateTime.MinValue) {
				return -1;
			}
			int years=dateExam.Year-patient.Birthdate.Year;
			if(dateExam.Month<patient.Birthdate.Month || (dateExam.Month==patient.Birthdate.Month && dateExam.Day<patient.Birthdate.Day)) {//have not had birthday this year
				years--;
			}
			int months=dateExam.Month-patient.Birthdate.Month;
			if(patient.Birthdate.Day>dateExam.Day) {
				months--;
			}
			if(months<0) {
				months=months+12;
			}
			months=months+(years*12);
			string genderAgeGroup="";
			if(patient.Gender==PatientGender.Male) {
				genderAgeGroup="m";
			}
			else if(patient.Gender==PatientGender.Female) {
				genderAgeGroup="f";
			}
			else {
				return -1;
			}
			genderAgeGroup+=months.ToString();
			//get L, M, and S for the patient's gender and age from list
			List<float> listFloats=listGenderAge_LMSs.Find(x => x.GenderAge==genderAgeGroup).ListFloats;
			//use (((bmi/M)^L)-1)/(L*S) to get z-score
			float zScore=(((float)Math.Pow(bmi/listFloats[1],listFloats[0]))-1)/(listFloats[0]*listFloats[2]);
			//use GetPercentile helper function to get percentile from z-score
			return Vitalsigns.GetBMIPercentileUsingZScore(zScore);
		}

		///<summary>Returns list of Loinc records related to BMI from DB.</summary>
		public static List<Loinc> GetLoincsBMI() {
			//The list returned will only contain the Loincs that are actually in the loinc table.
			return Loincs.GetForCodeList("59574-4,59575-1,59576-9");//Body mass index (BMI) [Percentile],Body mass index (BMI) [Percentile] Per age,Body mass index (BMI) [Percentile] Per age and gender
		}

		///<summary>Returns list of Loinc records related to height from DB.</summary>
		public static List<Loinc> GetLoincsHeight() {
			//The list returned will only contain the Loincs that are actually in the loinc table.
			return Loincs.GetForCodeList("8302-2,3137-7,3138-5,8306-3,8307-1,8308-9");//Body height,Body height Measured,Body height Stated,Body height --lying,Body height --pre surgery,Body height --standing
		}

		///<summary>Returns list of Loinc records related to weight from DB.</summary>
		public static List<Loinc> GetLoincsWeight() {
			//The list returned will only contain the Loincs that are actually in the loinc table.
			return Loincs.GetForCodeList("29463-7,18833-4,3141-9,3142-7,8350-1,8351-9");//Body weight,First Body weight,Body weight Measured,Body weight Stated,Body weight Measured --with clothes,Body weight Measured --without clothes
		}

		///<summary>Returns list of Loinc records related to blood pressure from DB.</summary>
		public static List<Loinc> GetLoincsBloodPressure() {
			//The list returned will only contain the Loincs that are actually in the loinc table.
			return Loincs.GetForCodeList("8480-6,8462-4");//BP Systolic exan,BP Diastolic exam
		}

		///<summary>Returns list of Loinc records related to BMI Exam procedure from DB.</summary>
		public static List<Loinc> GetLoincsBMIExam() {
			//The list returned will only contain the Loincs that are actually in the loinc table.
			return Loincs.GetForCodeList("39156-5");//BMI Exam procedure
		}

		///<summary>Returns list of Loinc records related to BMI Percentiles from DB.</summary>
		public static List<Loinc> GetLoincsBMIPercentile() {
			//The list returned will only contain the Loincs that are actually in the loinc table.
			return Loincs.GetForCodeList("59576-9");//Body mass index (BMI) [Percentile] Per age and gender, only code we will allow for percentile
		}

		///<summary>Takes in vitalsign and various fields. Assigns the vitalsign's fields to those passed in.</summary>
		public static Vitalsign SetFields(Vitalsign vitalsign,DateTime date,int pulse,float height,float weight,int bpDiastolic,int bpSystolic,int bmiPercentile,Loinc loincHeightCode,Loinc loincWeightCode) {
			vitalsign.DateTaken=date;
			vitalsign.Pulse=pulse;
			vitalsign.Height=height;
			vitalsign.Weight=weight;
			vitalsign.BpDiastolic=bpDiastolic;
			vitalsign.BpSystolic=bpSystolic;
			//textBMIPercentile will be the calculated percentile or -1 if not in age range or there is an error calculating the percentile.
			//In this case the text box will be not visible, but the text will be set to -1 and we will store it that way to indicate no valid BMIPercentile
			vitalsign.BMIPercentile=bmiPercentile;//could be -1 if not in age range or error calculating percentile
			vitalsign.BMIExamCode="";
			if(bmiPercentile>-1 && Loincs.GetByCode("59576-9")!=null) {
				vitalsign.BMIExamCode="59576-9";//Body mass index (BMI) [Percentile] Per age and gender, only code used for percentile, only visible if under 17 at time of exam
			}
			if(loincHeightCode.LoincNum!=0) {
				vitalsign.HeightExamCode=loincHeightCode.LoincCode;
			}
			if(loincWeightCode.LoincNum!=0) {
				vitalsign.WeightExamCode=loincWeightCode.LoincCode;
			}
			return vitalsign;
		}

		///<summary>Returns the Weightcode for it depending on the passed-in InterventionCodeSet enum value.</summary>
		public static string SetWeightCodes(InterventionCodeSet interventionCodeSet){
			switch(interventionCodeSet) {
				case InterventionCodeSet.AboveNormalWeight:
					if(Snomeds.GetByCode("238131007")!=null) {
						return "238131007";
					}
					break;
				case InterventionCodeSet.BelowNormalWeight:
					if(Snomeds.GetByCode("248342006")!=null) {
						return "248342006";
					}
					break;
				case InterventionCodeSet.Nutrition:
				case InterventionCodeSet.None:
				default:
					return "";
			}
			return "";
		}

		///<summary>Takes in a vitalsign object and returns its PregDiseaseNum. If set, verifies the exam is within the active dates of the attached pregnancy problem.</summary>
		public static long SetPregnancyDisease(Vitalsign vitalsign,long diseaseDefNumPreg) {
			if(diseaseDefNumPreg==0) {
				//shouldn't happen, if IsPreg is true, this must be set to either an existing problem def or a new problem that requires inserting disease.
				throw new Exception("This exam must point to a valid pregnancy diagnosis.");
			}
			if(vitalsign.PregDiseaseNum==0) {//insert new preg disease and update vitalsign to point to it
				Disease diseaseNew=new Disease();
				diseaseNew.PatNum=vitalsign.PatNum;
				diseaseNew.DiseaseDefNum=diseaseDefNumPreg;
				diseaseNew.DateStart=vitalsign.DateTaken;
				diseaseNew.ProbStatus=ProblemStatus.Active;
				return Diseases.Insert(diseaseNew);
			}
			Disease disease=Diseases.GetOne(vitalsign.PregDiseaseNum);
			if(vitalsign.DateTaken<disease.DateStart
				|| (disease.DateStop.Year>1880 && vitalsign.DateTaken>disease.DateStop))
			{
				//the current exam is no longer within dates of preg problem, uncheck the pregnancy box and remove the pointer to the disease
				throw new Exception("This exam is not within the active dates of the attached pregnancy problem.");
			}
			return vitalsign.PregDiseaseNum;
		}
	}

	///<summary>Nested class used to remove dictionary from FormVitalSignEdit2014.cs.</summary>
	public class GenderAge_LMS {
		///<summary>String containing the gender (m or f) and age in months.</summary>
		public string GenderAge;
		///<summary>Contains 3 floats, the L=power of Box-Cox transformation, M=median, and S=generalized coefficient of variation. The L, M, and S values are from the CDC website http://www.cdc.gov/nchs/data/series/sr_11/sr11_246.pdf page 178-186.</summary>
		public List<float> ListFloats;

		public GenderAge_LMS(string genderAge,List<float> listFloats) {
			GenderAge=genderAge;
			ListFloats=listFloats;
		}
	}
}