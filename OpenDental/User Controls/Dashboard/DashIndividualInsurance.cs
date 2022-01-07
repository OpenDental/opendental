using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>This is used in both the dashboard and in ControlTreat.  We paint instead of using controls because it's snappier, but mostly because we couldn't do it any other way. Dynamic addition of controls does not play well with LayoutManager and threads.</summary>
	public partial class DashIndividualInsurance:Control,IDashWidgetField {
		private List<InsPlan> _listInsPlans;
		private List<InsSub> _listInsSubs;
		private List<PatPlan> _listPatPlans;
		private List<Benefit> _listBenefits;
		private List<ClaimProcHist> _histList;
		private Patient _pat;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		private string _strPriMax="";
		private string _strPriDed="";
		private string _strPriDedRem="";
		private string _strPriUsed="";
		private string _strPriPend="";
		private string _strPriRem="";
		private string _strSecMax="";
		private string _strSecDed="";
		private string _strSecDedRem="";
		private string _strSecUsed="";
		private string _strSecPend="";
		private string _strSecRem="";

		public DashIndividualInsurance() {
			InitializeComponent();
			DoubleBuffered=true;
		}

		public void PassLayoutManager(LayoutManagerForms layoutManager){
			//this is only in dashboard.  In ControlTreat, the LayoutManager is set during layout.
			LayoutManager=layoutManager;
		}

		protected override Size DefaultSize {
			get {
				return new Size(193,160);
			}
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			Graphics g=e.Graphics;//don't dispose
			using SolidBrush brushBack=new SolidBrush(BackColor);
			g.FillRectangle(brushBack,ClientRectangle);
			using Pen penOutline=new Pen(ColorOD.Outline);
			Rectangle rectangle=new Rectangle(0,0,Width-1,Height-1);
			GraphicsHelper.DrawRoundedRectangle(g,Pens.Silver,rectangle,4);
			//We must ignore the Font and do our own.
			float scaledFontSize=LayoutManager.ScaleF(8.25f);
			scaledFontSize*=0.97f;//layout manager uses .92
			//using Font fontTitle=new Font(FontFamily.GenericSansSerif,scaledFontSize);
			using Font font=new Font(FontFamily.GenericSansSerif,scaledFontSize);
			g.DrawString(Lan.g(this,"Individual Insurance"),font,Brushes.Black,4,1);
			StringFormat stringFormat;//disposed as used and at bottom
			stringFormat=new StringFormat();
			//top labels-------------------------------------------------------------------------
			stringFormat.Alignment=StringAlignment.Center;//horiz
			stringFormat.LineAlignment=StringAlignment.Far;
			rectangle=new Rectangle(LayoutManager.Scale(74),LayoutManager.Scale(16),LayoutManager.Scale(60),LayoutManager.Scale(15));
			g.DrawString(Lan.g(this,"Primary"),font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(131),LayoutManager.Scale(16),LayoutManager.Scale(60),LayoutManager.Scale(15));
			g.DrawString(Lan.g(this,"Secondary"),font,Brushes.Black,rectangle,stringFormat);
			//row 1-----------------------------------------------------------------------
			stringFormat?.Dispose();
			stringFormat=new StringFormat();
			stringFormat.Alignment=StringAlignment.Far;
			stringFormat.LineAlignment=StringAlignment.Center;
			rectangle=new Rectangle(LayoutManager.Scale(4),LayoutManager.Scale(37),LayoutManager.Scale(66),LayoutManager.Scale(15));
			g.DrawString(Lan.g(this,"AnnualMax"),font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(73),LayoutManager.Scale(36),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strPriMax,font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(131),LayoutManager.Scale(36),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strSecMax,font,Brushes.Black,rectangle,stringFormat);
			//row 2-------------------------------------------------------------------------
			rectangle=new Rectangle(LayoutManager.Scale(4),LayoutManager.Scale(57),LayoutManager.Scale(66),LayoutManager.Scale(15));
			g.DrawString(Lan.g(this,"Deductible"),font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(73),LayoutManager.Scale(56),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strPriDed,font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(131),LayoutManager.Scale(56),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strSecDed,font,Brushes.Black,rectangle,stringFormat);
			//row 3-------------------------------------------------------------------------
			rectangle=new Rectangle(LayoutManager.Scale(4),LayoutManager.Scale(77),LayoutManager.Scale(66),LayoutManager.Scale(15));
			g.DrawString(Lan.g(this,"Ded Remain"),font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(73),LayoutManager.Scale(76),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strPriDedRem,font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(131),LayoutManager.Scale(76),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strSecDedRem,font,Brushes.Black,rectangle,stringFormat);
			//row 4-------------------------------------------------------------------------
			rectangle=new Rectangle(LayoutManager.Scale(4),LayoutManager.Scale(97),LayoutManager.Scale(66),LayoutManager.Scale(15));
			g.DrawString(Lan.g(this,"Ins Used"),font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(73),LayoutManager.Scale(96),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strPriUsed,font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(131),LayoutManager.Scale(96),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strSecUsed,font,Brushes.Black,rectangle,stringFormat);
			//row 5-------------------------------------------------------------------------
			rectangle=new Rectangle(LayoutManager.Scale(4),LayoutManager.Scale(117),LayoutManager.Scale(66),LayoutManager.Scale(15));
			g.DrawString(Lan.g(this,"Pending"),font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(73),LayoutManager.Scale(116),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strPriPend,font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(131),LayoutManager.Scale(116),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strSecPend,font,Brushes.Black,rectangle,stringFormat);
			//row 6-------------------------------------------------------------------------
			rectangle=new Rectangle(LayoutManager.Scale(4),LayoutManager.Scale(137),LayoutManager.Scale(66),LayoutManager.Scale(15));
			g.DrawString(Lan.g(this,"Remain"),font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(73),LayoutManager.Scale(136),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strPriRem,font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(131),LayoutManager.Scale(136),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strSecRem,font,Brushes.Black,rectangle,stringFormat);
			stringFormat?.Dispose();
		}

		public string GetPriMax() {
			return _strPriMax;
		}

		public string GetPriDed() {
			return _strPriDed;
		}

		public string GetPriDedRem() {
			return _strPriDedRem;
		}

		public string GetPriUsed() {
			return _strPriUsed;
		}

		public string GetPriPend() {
			return _strPriPend;
		}

		public string GetPriRem() {
			return _strPriRem;
		}

		public string GetSecMax() {
			return _strSecMax;
		}

		public string GetSecDed() {
			return _strSecDed;
		}

		public string GetSecDedRem() {
			return _strSecDedRem;
		}

		public string GetSecUsed() {
			return _strSecUsed;
		}

		public string GetSecPend() {
			return _strSecPend;
		}

		public string GetSecRem() {
			return _strSecRem;
		}

		public void SetData(PatientDashboardDataEventArgs data,SheetField sheetField) {
			if(!IsNecessaryDataAvailable(data)) {
				return;
			}
			ExtractData(data);
		}

		private bool IsNecessaryDataAvailable(PatientDashboardDataEventArgs data) {
			if(data.Pat==null || data.ListInsPlans==null || data.ListInsSubs==null || data.ListPatPlans==null || data.ListBenefits==null) {
				return false;
			}
			return true;
		}

		private void ExtractData(PatientDashboardDataEventArgs data) {
			_listInsPlans=data.ListInsPlans;
			_listInsSubs=data.ListInsSubs;
			_listPatPlans=data.ListPatPlans;
			_listBenefits=data.ListBenefits;
			_histList=data.HistList??ClaimProcs.GetHistList(_pat.PatNum,_listBenefits,_listPatPlans,_listInsPlans,DateTime.Today,_listInsSubs);
			_pat=data.Pat;
		}

		public void RefreshData(Patient pat,SheetField sheetField) {
			_listInsPlans=new List<InsPlan>();
			_listInsSubs=new List<InsSub>();
			_listPatPlans=new List<PatPlan>();
			_listBenefits=new List<Benefit>();
			_histList=new List<ClaimProcHist>();
			_pat=pat;
			if(_pat==null) {
				return;
			}
			_listPatPlans=PatPlans.Refresh(_pat.PatNum);
			_listInsSubs=InsSubs.RefreshForFam(Patients.GetFamily(_pat.PatNum));
			_listInsPlans=InsPlans.RefreshForSubList(_listInsSubs);
			_listBenefits=Benefits.Refresh(_listPatPlans,_listInsSubs);
			_histList=ClaimProcs.GetHistList(_pat.PatNum,_listBenefits,_listPatPlans,_listInsPlans,DateTime.Today,_listInsSubs);
		}

		public void RefreshView() {
			RefreshInsurance(_pat,_listInsPlans,_listInsSubs,_listPatPlans,_listBenefits,_histList);
		}

		public void RefreshInsurance(Patient pat,List<InsPlan> listInsPlans,List<InsSub> listInsSubs,List<PatPlan> listPatPlans,List<Benefit> listBenefits,List<ClaimProcHist> histList,DateTime dateAsOf=default)
		{
			_strPriMax="";
			_strPriDed="";
			_strPriDedRem="";
			_strPriUsed="";
			_strPriPend="";
			_strPriRem="";
			_strSecMax="";
			_strSecDed="";
			_strSecDedRem="";
			_strSecUsed="";
			_strSecPend="";
			_strSecRem="";
			if(pat==null){
				return;
			}
			double maxInd=0;
			double ded=0;
			double dedFam=0;
			double dedRem=0;
			double remain=0;
			double pend=0;
			double used=0;
			InsPlan isnPlanCur;//=new InsPlan();
			InsSub subCur;
			if(listPatPlans.Count>0){
				if(dateAsOf.Year<1880) {
					dateAsOf=DateTime.Today;
				}
				subCur=InsSubs.GetSub(listPatPlans[0].InsSubNum,listInsSubs);
				isnPlanCur=InsPlans.GetPlan(subCur.PlanNum,listInsPlans);
				pend=InsPlans.GetPendingDisplay(histList,dateAsOf,isnPlanCur,listPatPlans[0].PatPlanNum,-1,pat.PatNum,listPatPlans[0].InsSubNum,listBenefits);
				used=InsPlans.GetInsUsedDisplay(histList,dateAsOf,isnPlanCur.PlanNum,listPatPlans[0].PatPlanNum,-1,listInsPlans,listBenefits,pat.PatNum,listPatPlans[0].InsSubNum);
				_strPriPend=pend.ToString("F");
				_strPriUsed=used.ToString("F");
				maxInd=Benefits.GetAnnualMaxDisplay(listBenefits,isnPlanCur.PlanNum,listPatPlans[0].PatPlanNum,false);
				if(maxInd==-1){//if annual max is blank
					_strPriMax="";
					_strPriRem="";
				}
				else{
					remain=maxInd-used-pend;
					if(remain<0){
						remain=0;
					}
					//textFamPriMax.Text=max.ToString("F");
					_strPriMax=maxInd.ToString("F");
					_strPriRem=remain.ToString("F");
				}
				//deductible:
				ded=Benefits.GetDeductGeneralDisplay(listBenefits,isnPlanCur.PlanNum,listPatPlans[0].PatPlanNum,BenefitCoverageLevel.Individual);
				dedFam=Benefits.GetDeductGeneralDisplay(listBenefits,isnPlanCur.PlanNum,listPatPlans[0].PatPlanNum,BenefitCoverageLevel.Family);
				if(ded!=-1){
					_strPriDed=ded.ToString("F");
					dedRem=InsPlans.GetDedRemainDisplay(histList,DateTime.Today,isnPlanCur.PlanNum,listPatPlans[0].PatPlanNum,-1,listInsPlans,pat.PatNum,ded,dedFam);
					_strPriDedRem=dedRem.ToString("F");
				}
			}
			if(listPatPlans.Count>1){
				subCur=InsSubs.GetSub(listPatPlans[1].InsSubNum,listInsSubs);
				isnPlanCur=InsPlans.GetPlan(subCur.PlanNum,listInsPlans);
				pend=InsPlans.GetPendingDisplay(histList,DateTime.Today,isnPlanCur,listPatPlans[1].PatPlanNum,-1,pat.PatNum,listPatPlans[1].InsSubNum,listBenefits);
				_strSecPend=pend.ToString("F");
				used=InsPlans.GetInsUsedDisplay(histList,DateTime.Today,isnPlanCur.PlanNum,listPatPlans[1].PatPlanNum,-1,listInsPlans,listBenefits,pat.PatNum,listPatPlans[1].InsSubNum);
				_strSecUsed=used.ToString("F");
				maxInd=Benefits.GetAnnualMaxDisplay(listBenefits,isnPlanCur.PlanNum,listPatPlans[1].PatPlanNum,false);
				if(maxInd==-1){//if annual max is blank
					_strSecMax="";
					_strSecRem="";
				}
				else{
					remain=maxInd-used-pend;
					if(remain<0){
						remain=0;
					}
					//textFamSecMax.Text=max.ToString("F");
					_strSecMax=maxInd.ToString("F");
					_strSecRem=remain.ToString("F");
				}
				//deductible:
				ded=Benefits.GetDeductGeneralDisplay(listBenefits,isnPlanCur.PlanNum,listPatPlans[1].PatPlanNum,BenefitCoverageLevel.Individual);
				dedFam=Benefits.GetDeductGeneralDisplay(listBenefits,isnPlanCur.PlanNum,listPatPlans[1].PatPlanNum,BenefitCoverageLevel.Family);
				if(ded!=-1){
					_strSecDed=ded.ToString("F");
					dedRem=InsPlans.GetDedRemainDisplay(histList,DateTime.Today,isnPlanCur.PlanNum,listPatPlans[1].PatPlanNum,-1,listInsPlans,pat.PatNum,ded,dedFam);
					_strSecDedRem=dedRem.ToString("F");
				}
			}
			Invalidate();
		}


	}
}
