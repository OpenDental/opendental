using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	///<summary></summary>
	public partial class FormProcCodeNew : FormODBase {
		public bool Changed;
		private List<Def> _listProcCodeCatDefs;

		///<summary></summary>
		public FormProcCodeNew(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormProcCodeNew_Load(object sender,EventArgs e) {
			ProcedureCodes.RefreshCache();
			listType.Items.Add(Lan.g(this,"none"));
			listType.Items.Add(Lan.g(this,"Exam"));
			listType.Items.Add(Lan.g(this,"Xray"));
			listType.Items.Add(Lan.g(this,"Prophy"));
			listType.Items.Add(Lan.g(this,"Fluoride"));
			listType.Items.Add(Lan.g(this,"Sealant"));
			listType.Items.Add(Lan.g(this,"Amalgam"));
			listType.Items.Add(Lan.g(this,"Composite, Anterior"));
			listType.Items.Add(Lan.g(this,"Composite, Posterior"));
			listType.Items.Add(Lan.g(this,"Buildup/Post"));
			listType.Items.Add(Lan.g(this,"Pulpotomy"));
			listType.Items.Add(Lan.g(this,"RCT"));
			listType.Items.Add(Lan.g(this,"SRP"));
			listType.Items.Add(Lan.g(this,"Denture"));
			listType.Items.Add(Lan.g(this,"RPD"));
			listType.Items.Add(Lan.g(this,"Denture Repair"));
			listType.Items.Add(Lan.g(this,"Reline"));
			listType.Items.Add(Lan.g(this,"Ceramic Inlay"));
			listType.Items.Add(Lan.g(this,"Metallic Inlay"));
			listType.Items.Add(Lan.g(this,"Whitening"));
			listType.Items.Add(Lan.g(this,"All-Ceramic Crown"));
			listType.Items.Add(Lan.g(this,"PFM Crown"));
			listType.Items.Add(Lan.g(this,"Full Gold Crown"));
			listType.Items.Add(Lan.g(this,"Bridge Pontic or Retainer - Ceramic"));
			listType.Items.Add(Lan.g(this,"Bridge Pontic or Retainer - PFM"));
			listType.Items.Add(Lan.g(this,"Bridge Pontic or Retainer - Metal"));
			listType.Items.Add(Lan.g(this,"Extraction"));
			listType.Items.Add(Lan.g(this,"Ortho"));
			listType.Items.Add(Lan.g(this,"Nitrous"));
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				labelListType.Visible=false;
				listType.Visible=false;
				labelTreatArea.Visible=false;
				comboTreatArea.Visible=false;
			}
			listType.SelectedIndex=0;
			for(int i=0;i<Enum.GetNames(typeof(ToothPaintingType)).Length;i++) {
				comboPaintType.Items.Add(Enum.GetNames(typeof(ToothPaintingType))[i]);
			}
			comboPaintType.SelectedIndex=(int)ToothPaintingType.None;
			for(int i=0;i<Enum.GetNames(typeof(TreatmentArea)).Length;i++) {
				comboTreatArea.Items.Add(Lan.g(this,Enum.GetNames(typeof(TreatmentArea))[i]));
			}
			comboTreatArea.SelectedIndex=(int)TreatmentArea.None;
			_listProcCodeCatDefs=Defs.GetDefsForCategory(DefCat.ProcCodeCats,true);
			for(int i=0;i<_listProcCodeCatDefs.Count;i++) {
				comboCategory.Items.Add(_listProcCodeCatDefs[i].ItemName);
			}
			comboCategory.SelectedIndex=0;
			textNewCode.Focus();
			textNewCode.Select(textNewCode.Text.Length,1);
		}
		
		private void textNewCode_TextChanged(object sender, System.EventArgs e) {
			if(textNewCode.Text=="d"){
				textNewCode.Text="D";
				textNewCode.SelectionStart=1;
			}
		}

		private void listType_Click(object sender,EventArgs e) {
			if(CultureInfo.CurrentCulture.Name=="en-US" && listType.SelectedIndex!=0){
				textNewCode.Text="D";
			}
			else{
				textNewCode.Text="";
			}
			textNewCode.Focus();
			textNewCode.Select(textNewCode.Text.Length,1);
			textDescription.Text="";
			textAbbreviation.Text="";
			checkNoBillIns.Checked=false;
			checkIsHygiene.Checked=false;
			checkIsProsth.Checked=false;
			comboPaintType.SelectedIndex=(int)ToothPaintingType.None;
			comboTreatArea.SelectedIndex=(int)TreatmentArea.None;
			comboCategory.SelectedIndex=0;
			switch(listType.SelectedIndex){
				case 0://none
					break;
				case 1://Exam
					textDescription.Text="Exam";
					textAbbreviation.Text="Ex";
					comboCategory.SelectedIndex=GetCategoryIndex("Exams & Xrays");
					break;
				case 2://Xray
					textDescription.Text="Intraoral Periapical Film";
					textAbbreviation.Text="PA";
					checkIsHygiene.Checked=true;
					comboCategory.SelectedIndex=GetCategoryIndex("Exams & Xrays");
					break;
				case 3://Prophy
					textDescription.Text="Prophy, Adult";
					textAbbreviation.Text="Pro";
					checkIsHygiene.Checked=true;
					comboCategory.SelectedIndex=GetCategoryIndex("Cleanings");
					break;
				case 4://Fluoride
					textDescription.Text="Fluoride";
					textAbbreviation.Text="Flo";
					checkIsHygiene.Checked=true;
					comboCategory.SelectedIndex=GetCategoryIndex("Cleanings");
					break;
				case 5://Sealant
					textDescription.Text="Sealant";
					textAbbreviation.Text="Seal";
					checkIsHygiene.Checked=true;
					comboPaintType.SelectedIndex=(int)ToothPaintingType.Sealant;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Tooth;
					comboCategory.SelectedIndex=GetCategoryIndex("Fillings");
					break;
				case 6://Amalgam
					textDescription.Text="Amalgam-1 Surf";
					textAbbreviation.Text="A1";
					comboPaintType.SelectedIndex=(int)ToothPaintingType.FillingDark;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Surf;
					comboCategory.SelectedIndex=GetCategoryIndex("Fillings");
					break;
				case 7://Composite, Anterior
					textDescription.Text="Composite-1 Surf, Anterior";
					textAbbreviation.Text="C1";
					comboPaintType.SelectedIndex=(int)ToothPaintingType.FillingLight;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Surf;
					comboCategory.SelectedIndex=GetCategoryIndex("Fillings");
					break;
				case 8://Composite, Posterior
					textDescription.Text="Composite-1 Surf, Posterior";
					textAbbreviation.Text="C1(P)";
					comboPaintType.SelectedIndex=(int)ToothPaintingType.FillingLight;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Surf;
					comboCategory.SelectedIndex=GetCategoryIndex("Fillings");
					break;
				case 9://Buildup/Post
					textDescription.Text="Build Up";
					textAbbreviation.Text="BU";
					comboPaintType.SelectedIndex=(int)ToothPaintingType.PostBU;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Tooth;
					comboCategory.SelectedIndex=GetCategoryIndex("Fillings");
					break;
				case 10://Pulpotomy
					textDescription.Text="Pulpotomy";
					textAbbreviation.Text="Pulp";
					checkNoBillIns.Checked=true;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Tooth;
					comboCategory.SelectedIndex=GetCategoryIndex("Endo");
					break;
				case 11://RCT
					textDescription.Text="Root Canal, Anterior";
					textAbbreviation.Text="RCT-Ant";
					comboPaintType.SelectedIndex=(int)ToothPaintingType.RCT;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Tooth;
					comboCategory.SelectedIndex=GetCategoryIndex("Endo");
					break;
				case 12://SRP
					textDescription.Text="Scaling & Root Planing, Quadrant";
					textAbbreviation.Text="SRP";
					checkIsHygiene.Checked=true;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Quad;
					comboCategory.SelectedIndex=GetCategoryIndex("Perio");
					break;
				case 13://Denture
					textDescription.Text="Maxillary Denture";
					textAbbreviation.Text="MaxDent";
					checkIsProsth.Checked=true;
					comboPaintType.SelectedIndex=(int)ToothPaintingType.DentureLight;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Arch;
					comboCategory.SelectedIndex=GetCategoryIndex("Dentures");
					break;
				case 14://RPD
					textDescription.Text="Maxillary RPD";
					textAbbreviation.Text="MaxRPD";
					checkIsProsth.Checked=true;
					comboPaintType.SelectedIndex=(int)ToothPaintingType.DentureLight;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.ToothRange;
					comboCategory.SelectedIndex=GetCategoryIndex("Dentures");
					break;
				case 15://Denture Repair
					textDescription.Text="Repair Broken Denture";
					textAbbreviation.Text="RepairDent";
					comboCategory.SelectedIndex=GetCategoryIndex("Dentures");
					break;
				case 16://Reline
					textDescription.Text="Reline Max Denture Chairside";
					textAbbreviation.Text="RelMaxDntChair";
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Arch;
					comboCategory.SelectedIndex=GetCategoryIndex("Dentures");
					break;
				case 17://Ceramic Inlay
					textDescription.Text="Ceramic Inlay-1 Surf";
					textAbbreviation.Text="CerInlay1";
					comboPaintType.SelectedIndex=(int)ToothPaintingType.FillingLight;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Surf;
					comboCategory.SelectedIndex=GetCategoryIndex("Cosmetic");
					break;
				case 18://Metal Inlay
					textDescription.Text="Metallic Inlay-1 Surf";
					textAbbreviation.Text="MetInlay1";
					comboPaintType.SelectedIndex=(int)ToothPaintingType.FillingDark;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Surf;
					comboCategory.SelectedIndex=GetCategoryIndex("Fillings");
					break;
				case 19://Whitening
					textDescription.Text="Whitening Tray";
					textAbbreviation.Text="White";
					checkNoBillIns.Checked=true;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Arch;
					comboCategory.SelectedIndex=GetCategoryIndex("Cosmetic");
					break;
				case 20://All-Ceramic Crown
					textDescription.Text="All-Ceramic Crown";
					textAbbreviation.Text="AllCerCrn";
					checkIsProsth.Checked=true;
					comboPaintType.SelectedIndex=(int)ToothPaintingType.CrownLight;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Tooth;
					comboCategory.SelectedIndex=GetCategoryIndex("Crown & Bridge");
					break;
				case 21://PFM Crown
					textDescription.Text="PFM Crown";
					textAbbreviation.Text="PFM";
					checkIsProsth.Checked=true;
					comboPaintType.SelectedIndex=(int)ToothPaintingType.CrownLight;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Tooth;
					comboCategory.SelectedIndex=GetCategoryIndex("Crown & Bridge");
					break;
				case 22://Full Gold Crown
					textDescription.Text="Full Gold Crown";
					textAbbreviation.Text="FGCrn";
					checkIsProsth.Checked=true;
					comboPaintType.SelectedIndex=(int)ToothPaintingType.CrownDark;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Tooth;
					comboCategory.SelectedIndex=GetCategoryIndex("Crown & Bridge");
					break;
				case 23://Bridge Pontic or Retainer - Ceramic
					textDescription.Text="Bridge Pontic, Ceramic";
					textAbbreviation.Text="PontCeram";
					checkIsProsth.Checked=true;
					comboPaintType.SelectedIndex=(int)ToothPaintingType.BridgeLight;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Tooth;
					comboCategory.SelectedIndex=GetCategoryIndex("Crown & Bridge");
					break;
				case 24://Bridge Pontic or Retainer - PFM
					textDescription.Text="Bridge Pontic, PFM";
					textAbbreviation.Text="PontPFM";
					checkIsProsth.Checked=true;
					comboPaintType.SelectedIndex=(int)ToothPaintingType.BridgeLight;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Tooth;
					comboCategory.SelectedIndex=GetCategoryIndex("Crown & Bridge");
					break;
				case 25://Bridge Pontic or Retainer - Metal
					textDescription.Text="Bridge Pontic, Cast Noble Metal";
					textAbbreviation.Text="PontCastNM";
					checkIsProsth.Checked=true;
					comboPaintType.SelectedIndex=(int)ToothPaintingType.BridgeDark;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Tooth;
					comboCategory.SelectedIndex=GetCategoryIndex("Crown & Bridge");
					break;
				case 26://Extraction
					textDescription.Text="Extraction";
					textAbbreviation.Text="Ext";
					comboPaintType.SelectedIndex=(int)ToothPaintingType.Extraction;
					comboTreatArea.SelectedIndex=(int)TreatmentArea.Tooth;
					comboCategory.SelectedIndex=GetCategoryIndex("Oral Surgery");
					break;
				case 27://Ortho
					textDescription.Text="Comprehensive Ortho, Adult";
					textAbbreviation.Text="CompOrthoAdlt";
					comboCategory.SelectedIndex=GetCategoryIndex("Ortho");
					break;
				case 28://Nitrous
					textDescription.Text="Nitrous Oxide, Under 1 hour";
					textAbbreviation.Text="Nitrous30";
					comboCategory.SelectedIndex=GetCategoryIndex("Misc");
					break;
			}
		}

		///<summary>Returns the index of the category with the supplied name.  Zero if the name does not exist.</summary>
		private int GetCategoryIndex(string name){
			for(int i=0;i<_listProcCodeCatDefs.Count;i++) {
				if(_listProcCodeCatDefs[i].ItemName==name){
					return i;
				}
			}
			return 0;
		}

		private bool AddProc(){
			if(textNewCode.Text=="") {
				MsgBox.Show(this,"Code not allowed to be blank.");
				return false;
			}
			if(ProcedureCodes.IsValidCode(textNewCode.Text)){
				MsgBox.Show(this,"That code already exists.");
				return false;
			}
			if(textDescription.Text=="") {
				MsgBox.Show(this,"Description not allowed to be blank.");
				return false;
			}
			if(textAbbreviation.Text=="") {
				MsgBox.Show(this,"Abbreviation not allowed to be blank.");
				return false;
			}
			//ok to add code-----------------------------------------------------------------------------------
			ProcedureCode code=new ProcedureCode();
			code.ProcCode=textNewCode.Text;
			//code.ProcTime="/X/";//moved to contructor.
			//code.GraphicColor=Color.FromArgb(0);//moved to contructor.
			code.Descript=textDescription.Text;
			code.AbbrDesc=textAbbreviation.Text;
			code.NoBillIns=checkNoBillIns.Checked;
			code.IsHygiene=checkIsHygiene.Checked;
			code.IsProsth=checkIsProsth.Checked;
			code.PaintType=(ToothPaintingType)comboPaintType.SelectedIndex;
			code.TreatArea=(TreatmentArea)comboTreatArea.SelectedIndex;
			//if(comboCategory.SelectedIndex!=-1)
			code.ProcCat=_listProcCodeCatDefs[comboCategory.SelectedIndex].DefNum;
			ProcedureCodes.Insert(code);
			Changed=true;
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Added Procedure Code: "+code.ProcCode);
			return true;
		}

		private void butDefault_Click(object sender,EventArgs e) {
			Changed=true;//because of add definition for new proc category
			if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete all 'T' codes which came with trial version?")){
				
			}
		}

		private void butAnother_Click(object sender,EventArgs e) {
			string previous=textNewCode.Text;
			if(AddProc()){
				ProcedureCodes.RefreshCache();
				if(CultureInfo.CurrentCulture.Name=="en-US" && listType.SelectedIndex!=0) {
					textNewCode.Text="D";
				}
				else {
					textNewCode.Text="";
				}
				textCodePrevious.Text=previous;
			}
			textNewCode.Focus();
			textNewCode.Select(textNewCode.Text.Length,1);
			
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(AddProc()){
				Close();
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			Close();
		}

		

		

		

		

		
		
	}
}
