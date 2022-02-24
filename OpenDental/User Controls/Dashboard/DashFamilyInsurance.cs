using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>This is used in both the dashboard and in ControlTreat.  We paint instead of using controls because it's snappier, but mostly because we couldn't do it any other way. Dynamic addition of controls does not play well with LayoutManager and threads.</summary>
	public partial class DashFamilyInsurance:Control,IDashWidgetField {
		private List<InsPlan> _listInsPlans;
		private List<InsSub> _listInsSubs;
		private List<PatPlan> _listPatPlans;
		private List<Benefit> _listBenefits;
		private Patient _pat;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		private string _strFamPriMax="";
		private string _strFamPriDed="";
		private string _strFamSecMax="";
		private string _strFamSecDed="";

		public DashFamilyInsurance() {
			InitializeComponent();
			DoubleBuffered=true;
		}

		public void PassLayoutManager(LayoutManagerForms layoutManager){
			//this is only in dashboard.  In ControlTreat, the LayoutManager is set during layout.
			LayoutManager=layoutManager;
		}

		protected override Size DefaultSize {
			get {
				return new Size(193,80);
			}
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			/*
			groupBoxFamilyIns.Bounds=ClientRectangle;
			//LayoutManager is not allowed to touch dashboard, so we have to do manual layout ourselves.
			//It would be better to do this with paint instead of controls.
			//WARNING: Changing the position of controls in this designer will not change them in the UI because we lay them all out here:
			float scaledFontSize=LayoutManager.ScaleF(8.25f);
			scaledFontSize*=0.92f;//because this is what the layout manager usually does
			using Font fontLabel=new Font(FontFamily.GenericSansSerif,scaledFontSize);
			labelPri.Font=fontLabel;
			labelSec.Font=fontLabel;
			labelAnnual.Font=fontLabel;
			labelDed.Font=fontLabel;
			labelPri.Bounds=new Rectangle(LayoutManager.Scale(74),LayoutManager.Scale(16),LayoutManager.Scale(60),LayoutManager.Scale(15));
			labelSec.Bounds=new Rectangle(LayoutManager.Scale(131),LayoutManager.Scale(16),LayoutManager.Scale(60),LayoutManager.Scale(14));
			labelAnnual.Bounds=new Rectangle(LayoutManager.Scale(4),LayoutManager.Scale(37),LayoutManager.Scale(66),LayoutManager.Scale(15));*/
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
			g.DrawString(Lan.g(this,"Family Insurance"),font,Brushes.Black,4,1);
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
			g.DrawString(_strFamPriMax,font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(131),LayoutManager.Scale(36),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strFamSecMax,font,Brushes.Black,rectangle,stringFormat);
			//row 2-------------------------------------------------------------------------
			rectangle=new Rectangle(LayoutManager.Scale(4),LayoutManager.Scale(57),LayoutManager.Scale(66),LayoutManager.Scale(15));
			g.DrawString(Lan.g(this,"Fam Ded"),font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(73),LayoutManager.Scale(56),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strFamPriDed,font,Brushes.Black,rectangle,stringFormat);
			rectangle=new Rectangle(LayoutManager.Scale(131),LayoutManager.Scale(56),LayoutManager.Scale(58),LayoutManager.Scale(18));
			g.FillRectangle(Brushes.White,rectangle);
			g.DrawRectangle(penOutline,rectangle);
			g.DrawString(_strFamSecDed,font,Brushes.Black,rectangle,stringFormat);
			stringFormat?.Dispose();
		}

		public string GetFamPriMax() {
			return _strFamPriMax;
		}

		public string GetFamPriDed() {
			return _strFamPriDed;
		}

		public string GetFamSecMax() {
			return _strFamSecMax;
		}

		public string GetFamSecDed() {
			return _strFamSecDed;
		}

		public void SetData(PatientDashboardDataEventArgs data,SheetField sheetField) {
			if(!IsNecessaryDataAvailable(data)) {
				return;
			}
			ExtractData(data);
		}

		private bool IsNecessaryDataAvailable(PatientDashboardDataEventArgs data) {
			if(data.Pat==null || data.ListInsPlans==null || data.ListInsSubs==null || data.ListPatPlans==null || data.ListBenefits==null || data.Pat==null) {
				return false;
			}
			return true;
		}

		private void ExtractData(PatientDashboardDataEventArgs data) {
			_listInsPlans=data.ListInsPlans;
			_listInsSubs=data.ListInsSubs;
			_listPatPlans=data.ListPatPlans;
			_listBenefits=data.ListBenefits;
			_pat=data.Pat;
		}

		public void RefreshData(Patient pat,SheetField sheetField) {
			_listInsPlans=new List<InsPlan>();
			_listInsSubs=new List<InsSub>();
			_listPatPlans=new List<PatPlan>();
			_listBenefits=new List<Benefit>();
			_pat=pat;
			if(_pat==null) {
				return;
			}
			_listPatPlans=PatPlans.Refresh(_pat.PatNum);
			_listInsSubs=InsSubs.RefreshForFam(Patients.GetFamily(_pat.PatNum));
			_listInsPlans=InsPlans.RefreshForSubList(_listInsSubs);
			_listBenefits=Benefits.Refresh(_listPatPlans,_listInsSubs);
		}

		public void RefreshView() {
			RefreshInsurance(_pat,_listInsPlans,_listInsSubs,_listPatPlans,_listBenefits);
		}

		public void RefreshInsurance(Patient pat,List<InsPlan> listInsPlans,List<InsSub> listInsSubs,List<PatPlan> listPatPlans,List<Benefit> listBenefits){
			_strFamPriMax="";
			_strFamPriDed="";
			_strFamSecMax="";
			_strFamSecDed="";
			if(pat==null){
				return;
			}
			double maxFam=0;
			double maxInd=0;
			double dedFam=0;
			InsPlan PlanCur;//=new InsPlan();
			InsSub SubCur;
			if(listPatPlans.Count>0){
				SubCur=InsSubs.GetSub(listPatPlans[0].InsSubNum,listInsSubs);
				PlanCur=InsPlans.GetPlan(SubCur.PlanNum,listInsPlans);
				maxFam=Benefits.GetAnnualMaxDisplay(listBenefits,PlanCur.PlanNum,listPatPlans[0].PatPlanNum,true);
				maxInd=Benefits.GetAnnualMaxDisplay(listBenefits,PlanCur.PlanNum,listPatPlans[0].PatPlanNum,false);
				if(maxFam==-1){
					_strFamPriMax="";
				}
				else{
					_strFamPriMax=maxFam.ToString("F");
				}
				//deductible:
				dedFam=Benefits.GetDeductGeneralDisplay(listBenefits,PlanCur.PlanNum,listPatPlans[0].PatPlanNum,BenefitCoverageLevel.Family);
				if(dedFam!=-1) {
					_strFamPriDed=dedFam.ToString("F");
				}
			}
			if(listPatPlans.Count>1){
				SubCur=InsSubs.GetSub(listPatPlans[1].InsSubNum,listInsSubs);
				PlanCur=InsPlans.GetPlan(SubCur.PlanNum,listInsPlans);
				//max=Benefits.GetAnnualMaxDisplay(listBenefits,PlanCur.PlanNum,listPatPlans[1].PatPlanNum);
				maxFam=Benefits.GetAnnualMaxDisplay(listBenefits,PlanCur.PlanNum,listPatPlans[1].PatPlanNum,true);
				maxInd=Benefits.GetAnnualMaxDisplay(listBenefits,PlanCur.PlanNum,listPatPlans[1].PatPlanNum,false);
				if(maxFam==-1){
					_strFamSecMax="";
				}
				else{
					_strFamSecMax=maxFam.ToString("F");
				}
				//deductible:
				dedFam=Benefits.GetDeductGeneralDisplay(listBenefits,PlanCur.PlanNum,listPatPlans[1].PatPlanNum,BenefitCoverageLevel.Family);
				if(dedFam!=-1) {
					_strFamSecDed=dedFam.ToString("F");
				}
			}
			Invalidate();
		}

		
	}
}
