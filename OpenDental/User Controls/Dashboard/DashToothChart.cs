using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Drawing.Drawing2D;
using CodeBase;

namespace OpenDental {
	public partial class DashToothChart:PictureBox,IDashWidgetField {
		///<summary>Disposed</summary>
		private Image _imgToothChart;
		private SheetField _sheetField;
		public LayoutManagerForms LayoutManager;

		public DashToothChart() {
			InitializeComponent();
		}

		public void PassLayoutManager(LayoutManagerForms layoutManager){
			LayoutManager=layoutManager;
		}

		public void SetData(PatientDashboardDataEventArgs data,SheetField sheetField) {
			if(!IsNecessaryDataAvailable(data)) {
				return;
			}
			_imgToothChart?.Dispose();
			if(data.ImageToothChart==null) {//Check for this first, since it will be quicker than creating an image from the active treatment plan.
				_imgToothChart=null;
			}
			else{
				_imgToothChart=(Image)data.ImageToothChart.Clone();
			}
		}

		private bool IsNecessaryDataAvailable(PatientDashboardDataEventArgs data) {
			if(data.Pat==null || data.ImageToothChart==null) {
				return false;
			}
			return true;
		}

		private Image ExtractToothChart(PatientDashboardDataEventArgs data) {
			if(data.ImageToothChart!=null) {//Check for this first, since it will be quicker than creating an image from the active treatment plan.
				return (Image)data.ImageToothChart.Clone();
			}
			return null;
		}

		public void RefreshData(Patient pat,SheetField sheetField) {
			long patNum=pat?.PatNum??0;
			_sheetField=sheetField;
			TreatPlan treatPlan=TreatPlans.GetActiveForPat(patNum);
			List<Appointment> listAppts=Appointments.GetTodaysApptsForPat(patNum);
			_imgToothChart?.Dispose();
			_imgToothChart=GetToothChart(pat,treatPlan,listAppts:listAppts);
		}

		private Image GetToothChart(Patient pat,TreatPlan treatPlan,List<Procedure> listProcs=null,List<Appointment> listAppts=null) {
			long patNum=pat?.PatNum??0;
			if(treatPlan!=null && treatPlan.ListProcTPs.IsNullOrEmpty()) {
				listProcs=listProcs??Procedures.Refresh(patNum);
				treatPlan.ListProcTPs=GetTreatProcTPs(listProcs);
			}
			List<Procedure> listProcsFiltered=OpenDentBusiness.SheetPrinting.FilterProceduresForToothChart(listProcs,treatPlan,true);
			return SheetPrinting.GetToothChartHelper(patNum,true,treatPlan,listProcsFiltered,isInPatientDashboard:true,pat,listAppts);
		}

		///<summary>Returns a list of limited versions of ProcTP corresponding to the Procedures in listProcs.  Intended to mimic the logic in 
		///ContrTreat.LoadActiveTP, on a limited data basis in order to extract the necessary data to create a ToothChart.</summary>
		private List<ProcTP> GetTreatProcTPs(List<Procedure> listProcsAll) {
			List<Procedure> listProcs=Procedures.SortListByTreatPlanPriority(listProcsAll.FindAll(x => x.ProcStatus==ProcStat.TP),PrefC.IsTreatPlanSortByTooth).ToList();
			List<ProcTP> listProcTPs=new List<ProcTP>();
			foreach(Procedure proc in listProcs) {
				ProcTP procTP=new ProcTP();
				procTP.ProcNumOrig=proc.ProcNum;
				procTP.ToothNumTP=Tooth.ToInternat(proc.ToothNum);
				procTP.ProcCode=ProcedureCodes.GetStringProcCode(proc.CodeNum);
				if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.Surf) {
					procTP.Surf=Tooth.SurfTidyFromDbToDisplay(proc.Surf,proc.ToothNum);
				}
				else {
					procTP.Surf=proc.Surf;//for UR, L, etc.
				}
				listProcTPs.Add(procTP);
			}
			return listProcTPs;
		}

		public void RefreshView() {
			if(_imgToothChart is null) {
				return;
			}
			Image img=new Bitmap(Width,Height);
			using Graphics g = Graphics.FromImage(img);
			g.SmoothingMode=SmoothingMode.HighQuality;
			OpenDentBusiness.SheetPrinting.DrawScaledImage(0,0,Width,Height,g,null,_imgToothChart);
			Image?.Dispose();
			Image=img;
		}
	}
}
