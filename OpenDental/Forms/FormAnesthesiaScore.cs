using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Data;
using OpenDentBusiness.DataAccess;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;
using System.IO;

namespace OpenDental {
	public partial class FormAnesthesiaScore:Form {

		public static Userod CurUser;
		private Patient PatCur;
		public int ActQ2;
		public int ActQ1;
		public int ActQ0;
		public int RespQ2;
		public int RespQ1;
		public int RespQ0;
		public int CircQ2;
		public int CircQ1;
		public int CircQ0;
		public int ConcQ2;
		public int ConcQ1;
		public int ConcQ0;
		public int ColorQ2;
		public int ColorQ1;
		public int ColorQ0;
		public int DischAmb;
		public int DischWheelChr;
		public int DischAmbulance;
		public int DischCondStable;
		public int DischCondUnstable;
		public int AnestheticRecordNum;
		public string AnesthClose;
		public PrintWindowL printWindow;

		public FormAnesthesiaScore(Patient patCur,int anestheticRecordNum) {
			this.components = new System.ComponentModel.Container();
			InitializeComponent();
			Lan.F(this);
			PatCur = patCur;
			AnestheticRecordNum = anestheticRecordNum;
		}

		private void FormAnesthesiaScore_Load(object sender,EventArgs e) {
			//display Patient name
			textPatient.Text = Patients.GetPat(PatCur.PatNum).GetNameFL();
			//display Patient ID number
			textPatID.Text = PatCur.PatNum.ToString();
			//AnesthClose time is used as time for this form since the Anesthesia Score should be done right around discharge time
			string curDateTime = AnestheticRecords.GetAnesthDate(AnestheticRecordNum) + " " +  AnestheticRecords.GetAnesthCloseTime(AnestheticRecordNum);
			textDate.Text = curDateTime;
			FillControls();

		}

		private void RefreshScore() {
			ActQ2 = 0;
			ActQ1 = 0;
			ActQ0 = 0;
			RespQ2 = 0;
			RespQ1 = 0;
			RespQ0 = 0;
			CircQ2 = 0;
			CircQ1 = 0;
			CircQ0 = 0;
			ConcQ2 = 0;
			ConcQ1 = 0;
			ConcQ0 = 0;
			ColorQ2 = 0;
			ColorQ1 = 0;
			ColorQ0 = 0;

			if(radActivityQ2.Checked == true) {
				ActQ2 = 2;
			}
			else if(radActivityQ1.Checked == true) {
				ActQ1 = 1;
			}

			if(radRespQ2.Checked == true) {
				RespQ2 = 2;
			}
			else if(radRespQ1.Checked == true) {
				RespQ1 = 1;
			}

			if(radCircQ2.Checked == true) {
				CircQ2 = 2;
			}
			else if(radCircQ1.Checked == true) {
				CircQ1 = 1;
			}

			if(radConcQ2.Checked == true) {
				ConcQ2 = 2;
			}
			else if(radConcQ1.Checked == true) {
				ConcQ1 = 1;
			}

			if(radColorQ2.Checked == true) {
				ColorQ2 = 2;
			}
			else if(radColorQ1.Checked == true) {
				ColorQ1 = 1;
			}
			textPARSSTotal.Text = Convert.ToString(ActQ2 + ActQ1 + ActQ0 + RespQ2 + RespQ1 + RespQ0 + CircQ2 + CircQ1 + CircQ0 + ConcQ2 + ConcQ1 + ConcQ0 + ColorQ2 + ColorQ1 + ColorQ0);

		}

		private void butOK_Click(object sender,EventArgs e) {
			int QActivity = 0;
			int QResp = 0;
			int QCirc = 0;
			int QConc = 0;
			int QColor = 0;
			int AnesthesiaScore = 0;
			if(radDischAmb.Checked == true) {
				DischAmb = 1;
			}

			if(radDischWheelChr.Checked == true) {
				DischWheelChr = 1;
			}

			if(radDischAmbulance.Checked == true) {
				DischAmbulance = 1;
			}

			if(radDischCondStable.Checked == true) {
				DischCondStable = 1;
			}

			if(radDischCondUnstable.Checked == true) {
				DischCondUnstable = 1;
			}

			//calculate Anesthesia Score
			QActivity = ActQ2 + ActQ1;
			QResp = RespQ2 + RespQ1;
			QCirc = CircQ2 + CircQ1;
			QConc = ConcQ2 + ConcQ1;
			QColor = ColorQ2 + ColorQ1;
			AnesthesiaScore = QActivity + QResp + QCirc + QConc + QColor;

			int AnesthRecordNum = AnestheticRecords.GetScoreRecordNum(AnestheticRecordNum);
			if(AnesthRecordNum == 0) //no Anesthesia Score record exists yet
			{
				AnestheticRecords.InsertAnesthScore(AnestheticRecordNum,QActivity,QResp,QCirc,QConc,QColor,AnesthesiaScore,DischAmb,DischWheelChr,DischAmbulance,DischCondStable,DischCondUnstable);
			}
			else AnestheticRecords.UpdateAnesthScore(AnestheticRecordNum,QActivity,QResp,QCirc,QConc,QColor,AnesthesiaScore,DischAmb,DischWheelChr,DischAmbulance,DischCondStable,DischCondUnstable);//update existing Anesthesia Score

			DialogResult=DialogResult.OK;
		}

		//Load saved data into form for selected Anesthetic Record
		private void FillControls() {
			DataTable table=AnestheticRecords.GetAnesthScoreTable(AnestheticRecordNum);
			AnesthScore Cur;
			for(int i = 0;i < table.Rows.Count;i++) {
				Cur = new AnesthScore();
				Cur.AnesthScoreNum = PIn.PInt(table.Rows[i][0].ToString());
				Cur.AnestheticRecordNum = PIn.PInt(table.Rows[i][1].ToString());
				Cur.QActivity = PIn.PInt(table.Rows[i][2].ToString());
				Cur.QResp = PIn.PInt(table.Rows[i][3].ToString());
				Cur.QCirc = PIn.PInt(table.Rows[i][4].ToString());
				Cur.QConc = PIn.PInt(table.Rows[i][5].ToString());
				Cur.QColor = PIn.PInt(table.Rows[i][6].ToString());
				Cur.AnesthesiaScore = PIn.PInt(table.Rows[i][7].ToString());
				Cur.DischAmb = PIn.PInt(table.Rows[i][8].ToString());
				Cur.DischWheelChr = PIn.PInt(table.Rows[i][9].ToString());
				Cur.DischAmbulance = PIn.PInt(table.Rows[i][10].ToString());
				Cur.DischCondStable = PIn.PInt(table.Rows[i][11].ToString());
				Cur.DischCondUnstable = PIn.PInt(table.Rows[i][12].ToString());
				//fill radQActivity
				if(Cur.QActivity == 2) {
					radActivityQ2.Checked = true;
				}
				else if(Cur.QActivity == 1) {
					radActivityQ1.Checked = true;
				}
				else if(Cur.QActivity == 0) {
					radActivityQ0.Checked = true;
				}
				//fill radQResp
				if(Cur.QResp == 2) {
					radRespQ2.Checked = true;
				}
				else if(Cur.QResp == 1) {
					radRespQ1.Checked = true;
				}
				else if(Cur.QResp == 0) {
					radRespQ0.Checked = true;
				}
				//fill radQCirc
				if(Cur.QCirc == 2) {
					radCircQ2.Checked = true;
				}
				else if(Cur.QCirc == 1) {
					radCircQ1.Checked = true;
				}
				else if(Cur.QCirc == 0) {
					radCircQ0.Checked = true;
				}
				//fill radQConc
				if(Cur.QConc == 2) {
					radConcQ2.Checked = true;
				}
				else if(Cur.QConc == 1) {
					radConcQ1.Checked = true;
				}
				else if(Cur.QConc == 0) {
					radConcQ0.Checked = true;
				}
				//fill radQColor
				if(Cur.QColor == 2) {
					radColorQ2.Checked = true;
				}
				else if(Cur.QColor == 1) {
					radColorQ1.Checked = true;
				}
				else if(Cur.QColor == 0) {
					radColorQ0.Checked = true;
				}
				//fill radDischAmb
				if(Cur.DischAmb == 1) {
					radDischAmb.Checked = true;
				}
				//fill radDischWheelChr
				if(Cur.DischWheelChr == 1) {
					radDischWheelChr.Checked = true;
				}
				//fill radDischAmbulance
				if(Cur.DischAmbulance == 1) {
					radDischAmbulance.Checked = true;
				}
				//fill radDischCondStable
				if(Cur.DischCondStable == 1) {
					radDischCondStable.Checked = true;
				}
				//fill radDischCondUntable
				if(Cur.DischCondUnstable == 1) {
					radDischCondUnstable.Checked = true;
				}

			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


		private void radDischUnstable_CheckedChanged(object sender,EventArgs e) {
		}

		private void textPatient_TextChanged(object sender,EventArgs e) {
		}

		private void radActivityQ2_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radActivityQ1_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radActivityQ0_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radRespQ2_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radRespQ1_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radRespQ0_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radCircQ2_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radCircQ1_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radCircQ0_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radConcQ2_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radConcQ1_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radConcQ0_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radColorQ2_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radColorQ1_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void radColorQ0_CheckedChanged(object sender,EventArgs e) {
			RefreshScore();
		}

		private void textDate_TextChanged(object sender,EventArgs e) {
		}

		public void butPrint_Click(object sender,EventArgs e) {
			printWindow = new PrintWindowL();
			printWindow.Print(this.Handle);
		}

	}
}