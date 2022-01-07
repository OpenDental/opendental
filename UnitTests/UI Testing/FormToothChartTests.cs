using OpenDental;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using UnitTestsCore;

namespace UnitTests {
	public partial class FormToothChartTests : FormODBase {
		private Patient _patient;
		private List<ToothInitial> _listToothInitials=new List<ToothInitial>();
		private List<DataRow> _listRowsProcForGraphical=new List<DataRow>();

		public FormToothChartTests() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void InitializeData() {
			if(_patient!=null) {
				return;//Data has already been initialized.
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.StartingMessage="Initialize Data";
			progressOD.ActionMain=() => {
				//Initialize a unit test database for the corresponding maj.min version if one doesn't already exist.
				TestBase.Initialize(null);
				//Create a new patient that will be used to chart treatment and tooth initials.
				_patient=PatientT.CreatePatient("ToothChartTests");
				#region Tooth Initials
				//Create a drawing of a black line.
				ToothInitialT.CreateToothInitial(_patient,"176,187;180,187;188,187;194,186;199,186;206,185;222,185;230,185;235,185;239,185;244,185;249,185",toothNum:"",
					colorDraw:Color.FromArgb(-16777216));
				//Mark tooth 5 as missing so that the procedures for it do not show up.
				ToothInitialT.CreateToothInitial(_patient,"",toothNum:"5",toothInitialType:ToothInitialType.Missing);
				//Move tooth 14 around a bit.
				ToothInitialT.CreateToothInitial(_patient,"",toothNum:"14",toothInitialType:ToothInitialType.TipB,movement:-10);
				ToothInitialT.CreateToothInitial(_patient,"",toothNum:"14",toothInitialType:ToothInitialType.Rotate,movement:-20);
				ToothInitialT.CreateToothInitial(_patient,"",toothNum:"14",toothInitialType:ToothInitialType.ShiftO,movement:2);
				ToothInitialT.CreateToothInitial(_patient,"",toothNum:"14",toothInitialType:ToothInitialType.ShiftM,movement:2);
				_listToothInitials=ToothInitials.Refresh(_patient.PatNum);
				#endregion
				#region Procs For Graphical
				//Create several procedure codes with unique PaintTypes.
				ProcedureCodeT.AddIfNotPresent("TCT4119",toothPaintingType:ToothPaintingType.None);
				ProcedureCodeT.AddIfNotPresent("TCT7140",toothPaintingType:ToothPaintingType.Extraction);
				ProcedureCodeT.AddIfNotPresent("TCT3330",toothPaintingType:ToothPaintingType.RCT);
				ProcedureCodeT.AddIfNotPresent("TCT2950",toothPaintingType:ToothPaintingType.PostBU);
				ProcedureCodeT.AddIfNotPresent("TCT2150",toothPaintingType:ToothPaintingType.FillingDark);
				ProcedureCodeT.AddIfNotPresent("TCT2160",toothPaintingType:ToothPaintingType.FillingDark);
				ProcedureCodeT.AddIfNotPresent("TCT2392",toothPaintingType:ToothPaintingType.FillingLight);
				ProcedureCodeT.AddIfNotPresent("TCT2331",toothPaintingType:ToothPaintingType.FillingLight);
				ProcedureCodeT.AddIfNotPresent("TCT2740",toothPaintingType:ToothPaintingType.CrownLight);
				//Create a bunch of procedures.
				ProcedureT.CreateProcedure(_patient,"TCT2160",ProcStat.C,"13",80,procDate:new DateTime(2021,04,08),surf:"MOD");
				ProcedureT.CreateProcedure(_patient,"TCT4119",ProcStat.EO,"14",0,procDate:new DateTime(0001,01,01),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT2740",ProcStat.EO,"14",0,procDate:new DateTime(0001,01,01),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT4119",ProcStat.EO,"15",0,procDate:new DateTime(0001,01,01),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT2740",ProcStat.EO,"15",0,procDate:new DateTime(0001,01,01),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT4119",ProcStat.EO,"16",0,procDate:new DateTime(0001,01,01),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT2740",ProcStat.EO,"16",0,procDate:new DateTime(0001,01,01),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT4119",ProcStat.TP,"17",0,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT2740",ProcStat.TP,"17",1200,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT3330",ProcStat.TP,"17",0,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT2950",ProcStat.TP,"17",0,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT4119",ProcStat.TP,"18",0,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT2740",ProcStat.TP,"18",1200,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT3330",ProcStat.TP,"18",0,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT2950",ProcStat.TP,"18",0,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT4119",ProcStat.TP,"19",0,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT2740",ProcStat.TP,"19",1200,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT3330",ProcStat.TP,"19",0,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT2950",ProcStat.TP,"19",0,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT2160",ProcStat.TP,"2",80,procDate:new DateTime(2021,04,08),surf:"MVL");
				ProcedureT.CreateProcedure(_patient,"TCT2150",ProcStat.TP,"20",0,procDate:new DateTime(2021,04,08),surf:"ML");
				ProcedureT.CreateProcedure(_patient,"TCT2160",ProcStat.C,"30",80,procDate:new DateTime(2021,04,08),surf:"MOD");
				ProcedureT.CreateProcedure(_patient,"TCT2160",ProcStat.C,"31",80,procDate:new DateTime(2021,04,08),surf:"MOD");
				ProcedureT.CreateProcedure(_patient,"TCT2160",ProcStat.C,"32",80,procDate:new DateTime(2021,04,08),surf:"MOD");
				ProcedureT.CreateProcedure(_patient,"TCT2392",ProcStat.TP,"4",260,procDate:new DateTime(2021,04,08),surf:"DL");
				ProcedureT.CreateProcedure(_patient,"TCT2740",ProcStat.EO,"5",0,procDate:new DateTime(0001,01,01),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT7140",ProcStat.C,"5",1000,procDate:new DateTime(2021,04,08),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT4119",ProcStat.EO,"5",0,procDate:new DateTime(0001,01,01),surf:"");
				ProcedureT.CreateProcedure(_patient,"TCT2150",ProcStat.TP,"6",0,procDate:new DateTime(2021,04,08),surf:"IF");
				ProcedureT.CreateProcedure(_patient,"TCT2331",ProcStat.TP,"8",110,procDate:new DateTime(2021,04,08),surf:"DL");
				_listRowsProcForGraphical=ChartModules.GetProgNotes(_patient.PatNum,false,null).Select().ToList();
				#endregion
			};
			progressOD.ShowDialogProgress();
		}

		private void butToothChartBig_Click(object sender,EventArgs e) {
			InitializeData();
			#region Update Tooth Chart DrawingMode
			ComputerPref computerPrefOld=ComputerPrefs.LocalComputer;
			ComputerPref computerPrefNew=computerPrefOld.Copy();
			if(radioDirectX.Checked) {
				computerPrefNew.GraphicsSimple=DrawingMode.DirectX;
			}
			else if(radioOpenGL.Checked) {
				computerPrefNew.GraphicsSimple=DrawingMode.OpenGL;
			}
			else if(radioSimple2D.Checked) {
				computerPrefNew.GraphicsSimple=DrawingMode.Simple2D;
			}
			ComputerPrefs.Update(computerPrefNew,computerPrefOld);
			#endregion
			FormToothChartingBig formTCB=new FormToothChartingBig(false,_listToothInitials,_listRowsProcForGraphical);
			formTCB.PatCur=_patient.Copy();
			formTCB.Show();
		}
	}
}
