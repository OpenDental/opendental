using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPrinterSetup {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrinterSetup));
			this.label1 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.comboDefault = new OpenDental.UI.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboLabelSheet = new OpenDental.UI.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboClaim = new OpenDental.UI.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.comboRx = new OpenDental.UI.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboTPPerio = new OpenDental.UI.ComboBox();
			this.labelTPandPerio = new System.Windows.Forms.Label();
			this.comboStatement = new OpenDental.UI.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.comboLabelSingle = new OpenDental.UI.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this.comboPostcard = new OpenDental.UI.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.checkDefault = new OpenDental.UI.CheckBox();
			this.checkClaim = new OpenDental.UI.CheckBox();
			this.checkLabelSheet = new OpenDental.UI.CheckBox();
			this.checkLabelSingle = new OpenDental.UI.CheckBox();
			this.checkPostcard = new OpenDental.UI.CheckBox();
			this.checkRx = new OpenDental.UI.CheckBox();
			this.checkTPPerio = new OpenDental.UI.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.checkAppointments = new OpenDental.UI.CheckBox();
			this.label12 = new System.Windows.Forms.Label();
			this.comboAppointments = new OpenDental.UI.ComboBox();
			this.checkStatement = new OpenDental.UI.CheckBox();
			this.checkSimple = new OpenDental.UI.CheckBox();
			this.panelSimple = new System.Windows.Forms.Panel();
			this.comboReceipt = new OpenDental.UI.ComboBox();
			this.label14 = new System.Windows.Forms.Label();
			this.checkReceipt = new OpenDental.UI.CheckBox();
			this.label13 = new System.Windows.Forms.Label();
			this.checkRxControlled = new OpenDental.UI.CheckBox();
			this.comboRxControlled = new OpenDental.UI.ComboBox();
			this.labelRxMulti = new System.Windows.Forms.Label();
			this.checkRxMulti = new OpenDental.UI.CheckBox();
			this.comboRxMulti = new OpenDental.UI.ComboBox();
			this.panelSimple.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(19, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(438, 18);
			this.label1.TabIndex = 2;
			this.label1.Text = "These settings only apply to this workstation";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(550, 359);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(550, 393);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// comboDefault
			// 
			this.comboDefault.Location = new System.Drawing.Point(208, 38);
			this.comboDefault.Name = "comboDefault";
			this.comboDefault.Size = new System.Drawing.Size(253, 21);
			this.comboDefault.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 34);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(195, 19);
			this.label2.TabIndex = 5;
			this.label2.Text = "Default";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 271);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(195, 19);
			this.label3.TabIndex = 7;
			this.label3.Text = "Statements";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboLabelSheet
			// 
			this.comboLabelSheet.Location = new System.Drawing.Point(202, 111);
			this.comboLabelSheet.Name = "comboLabelSheet";
			this.comboLabelSheet.Size = new System.Drawing.Size(253, 21);
			this.comboLabelSheet.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(4, 87);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(195, 19);
			this.label4.TabIndex = 9;
			this.label4.Text = "Claims";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClaim
			// 
			this.comboClaim.Location = new System.Drawing.Point(202, 85);
			this.comboClaim.Name = "comboClaim";
			this.comboClaim.Size = new System.Drawing.Size(253, 21);
			this.comboClaim.TabIndex = 8;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(4, 113);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(195, 19);
			this.label5.TabIndex = 11;
			this.label5.Text = "Labels - Sheet";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboRx
			// 
			this.comboRx.Location = new System.Drawing.Point(202, 190);
			this.comboRx.Name = "comboRx";
			this.comboRx.Size = new System.Drawing.Size(253, 21);
			this.comboRx.TabIndex = 10;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(4, 192);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(195, 19);
			this.label6.TabIndex = 13;
			this.label6.Text = "Rx\'s";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTPPerio
			// 
			this.comboTPPerio.Location = new System.Drawing.Point(202, 296);
			this.comboTPPerio.Name = "comboTPPerio";
			this.comboTPPerio.Size = new System.Drawing.Size(253, 21);
			this.comboTPPerio.TabIndex = 12;
			// 
			// labelTPandPerio
			// 
			this.labelTPandPerio.Location = new System.Drawing.Point(4, 297);
			this.labelTPandPerio.Name = "labelTPandPerio";
			this.labelTPandPerio.Size = new System.Drawing.Size(195, 19);
			this.labelTPandPerio.TabIndex = 15;
			this.labelTPandPerio.Text = "Treatment Plans and Perio";
			this.labelTPandPerio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatement
			// 
			this.comboStatement.Location = new System.Drawing.Point(202, 270);
			this.comboStatement.Name = "comboStatement";
			this.comboStatement.Size = new System.Drawing.Size(253, 21);
			this.comboStatement.TabIndex = 14;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(4, 139);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(195, 19);
			this.label8.TabIndex = 17;
			this.label8.Text = "Labels - Single";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboLabelSingle
			// 
			this.comboLabelSingle.Location = new System.Drawing.Point(202, 137);
			this.comboLabelSingle.Name = "comboLabelSingle";
			this.comboLabelSingle.Size = new System.Drawing.Size(253, 21);
			this.comboLabelSingle.TabIndex = 16;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(4, 165);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(195, 19);
			this.label9.TabIndex = 19;
			this.label9.Text = "Postcards";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPostcard
			// 
			this.comboPostcard.Location = new System.Drawing.Point(202, 163);
			this.comboPostcard.Name = "comboPostcard";
			this.comboPostcard.Size = new System.Drawing.Size(253, 21);
			this.comboPostcard.TabIndex = 18;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(457, 5);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(116, 23);
			this.label10.TabIndex = 20;
			this.label10.Text = "Prompt";
			this.label10.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkDefault
			// 
			this.checkDefault.Location = new System.Drawing.Point(467, 37);
			this.checkDefault.Name = "checkDefault";
			this.checkDefault.Size = new System.Drawing.Size(24, 15);
			this.checkDefault.TabIndex = 21;
			// 
			// checkClaim
			// 
			this.checkClaim.Location = new System.Drawing.Point(467, 89);
			this.checkClaim.Name = "checkClaim";
			this.checkClaim.Size = new System.Drawing.Size(24, 15);
			this.checkClaim.TabIndex = 22;
			// 
			// checkLabelSheet
			// 
			this.checkLabelSheet.Location = new System.Drawing.Point(467, 115);
			this.checkLabelSheet.Name = "checkLabelSheet";
			this.checkLabelSheet.Size = new System.Drawing.Size(24, 15);
			this.checkLabelSheet.TabIndex = 23;
			// 
			// checkLabelSingle
			// 
			this.checkLabelSingle.Location = new System.Drawing.Point(467, 141);
			this.checkLabelSingle.Name = "checkLabelSingle";
			this.checkLabelSingle.Size = new System.Drawing.Size(24, 15);
			this.checkLabelSingle.TabIndex = 24;
			// 
			// checkPostcard
			// 
			this.checkPostcard.Location = new System.Drawing.Point(467, 167);
			this.checkPostcard.Name = "checkPostcard";
			this.checkPostcard.Size = new System.Drawing.Size(24, 15);
			this.checkPostcard.TabIndex = 25;
			// 
			// checkRx
			// 
			this.checkRx.Location = new System.Drawing.Point(467, 193);
			this.checkRx.Name = "checkRx";
			this.checkRx.Size = new System.Drawing.Size(24, 15);
			this.checkRx.TabIndex = 26;
			// 
			// checkTPPerio
			// 
			this.checkTPPerio.Location = new System.Drawing.Point(467, 300);
			this.checkTPPerio.Name = "checkTPPerio";
			this.checkTPPerio.Size = new System.Drawing.Size(24, 15);
			this.checkTPPerio.TabIndex = 27;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(500, 53);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(110, 150);
			this.label11.TabIndex = 28;
			this.label11.Text = "It is recommended to use the prompt option for most functions.  But if you are us" +
    "ing a default, it is not necessary to check the box.";
			// 
			// checkAppointments
			// 
			this.checkAppointments.Location = new System.Drawing.Point(467, 63);
			this.checkAppointments.Name = "checkAppointments";
			this.checkAppointments.Size = new System.Drawing.Size(24, 15);
			this.checkAppointments.TabIndex = 31;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(4, 61);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(195, 19);
			this.label12.TabIndex = 30;
			this.label12.Text = "Appointments";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboAppointments
			// 
			this.comboAppointments.Location = new System.Drawing.Point(202, 59);
			this.comboAppointments.Name = "comboAppointments";
			this.comboAppointments.Size = new System.Drawing.Size(253, 21);
			this.comboAppointments.TabIndex = 29;
			// 
			// checkStatement
			// 
			this.checkStatement.Location = new System.Drawing.Point(467, 274);
			this.checkStatement.Name = "checkStatement";
			this.checkStatement.Size = new System.Drawing.Size(24, 15);
			this.checkStatement.TabIndex = 32;
			// 
			// checkSimple
			// 
			this.checkSimple.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkSimple.Location = new System.Drawing.Point(49, 396);
			this.checkSimple.Name = "checkSimple";
			this.checkSimple.Size = new System.Drawing.Size(440, 24);
			this.checkSimple.TabIndex = 33;
			this.checkSimple.Text = "This is too complicated.  Show me the simple interface.";
			this.checkSimple.Click += new System.EventHandler(this.checkSimple_Click);
			// 
			// panelSimple
			// 
			this.panelSimple.Controls.Add(this.labelRxMulti);
			this.panelSimple.Controls.Add(this.checkRxMulti);
			this.panelSimple.Controls.Add(this.comboRxMulti);
			this.panelSimple.Controls.Add(this.comboReceipt);
			this.panelSimple.Controls.Add(this.label14);
			this.panelSimple.Controls.Add(this.checkReceipt);
			this.panelSimple.Controls.Add(this.label13);
			this.panelSimple.Controls.Add(this.checkRxControlled);
			this.panelSimple.Controls.Add(this.comboRxControlled);
			this.panelSimple.Controls.Add(this.comboTPPerio);
			this.panelSimple.Controls.Add(this.label3);
			this.panelSimple.Controls.Add(this.comboLabelSingle);
			this.panelSimple.Controls.Add(this.label6);
			this.panelSimple.Controls.Add(this.comboPostcard);
			this.panelSimple.Controls.Add(this.labelTPandPerio);
			this.panelSimple.Controls.Add(this.label10);
			this.panelSimple.Controls.Add(this.label8);
			this.panelSimple.Controls.Add(this.checkDefault);
			this.panelSimple.Controls.Add(this.checkClaim);
			this.panelSimple.Controls.Add(this.checkStatement);
			this.panelSimple.Controls.Add(this.label9);
			this.panelSimple.Controls.Add(this.comboAppointments);
			this.panelSimple.Controls.Add(this.label12);
			this.panelSimple.Controls.Add(this.checkAppointments);
			this.panelSimple.Controls.Add(this.label11);
			this.panelSimple.Controls.Add(this.checkTPPerio);
			this.panelSimple.Controls.Add(this.checkRx);
			this.panelSimple.Controls.Add(this.checkPostcard);
			this.panelSimple.Controls.Add(this.checkLabelSingle);
			this.panelSimple.Controls.Add(this.checkLabelSheet);
			this.panelSimple.Controls.Add(this.comboStatement);
			this.panelSimple.Controls.Add(this.comboRx);
			this.panelSimple.Controls.Add(this.label5);
			this.panelSimple.Controls.Add(this.comboClaim);
			this.panelSimple.Controls.Add(this.label4);
			this.panelSimple.Controls.Add(this.comboLabelSheet);
			this.panelSimple.Controls.Add(this.label2);
			this.panelSimple.Location = new System.Drawing.Point(6, 5);
			this.panelSimple.Name = "panelSimple";
			this.panelSimple.Size = new System.Drawing.Size(620, 350);
			this.panelSimple.TabIndex = 34;
			// 
			// comboReceipt
			// 
			this.comboReceipt.Location = new System.Drawing.Point(202, 322);
			this.comboReceipt.Name = "comboReceipt";
			this.comboReceipt.Size = new System.Drawing.Size(253, 21);
			this.comboReceipt.TabIndex = 36;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(4, 323);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(195, 19);
			this.label14.TabIndex = 37;
			this.label14.Text = "Receipts";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkReceipt
			// 
			this.checkReceipt.Location = new System.Drawing.Point(467, 326);
			this.checkReceipt.Name = "checkReceipt";
			this.checkReceipt.Size = new System.Drawing.Size(24, 15);
			this.checkReceipt.TabIndex = 38;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(4, 219);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(195, 19);
			this.label13.TabIndex = 34;
			this.label13.Text = "Controlled Rx\'s";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRxControlled
			// 
			this.checkRxControlled.Location = new System.Drawing.Point(467, 220);
			this.checkRxControlled.Name = "checkRxControlled";
			this.checkRxControlled.Size = new System.Drawing.Size(24, 15);
			this.checkRxControlled.TabIndex = 35;
			// 
			// comboRxControlled
			// 
			this.comboRxControlled.Location = new System.Drawing.Point(202, 217);
			this.comboRxControlled.Name = "comboRxControlled";
			this.comboRxControlled.Size = new System.Drawing.Size(253, 21);
			this.comboRxControlled.TabIndex = 33;
			// 
			// labelRxMulti
			// 
			this.labelRxMulti.Location = new System.Drawing.Point(4, 245);
			this.labelRxMulti.Name = "labelRxMulti";
			this.labelRxMulti.Size = new System.Drawing.Size(195, 19);
			this.labelRxMulti.TabIndex = 40;
			this.labelRxMulti.Text = "Multi Rx\'s";
			this.labelRxMulti.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkRxMulti
			// 
			this.checkRxMulti.Location = new System.Drawing.Point(467, 247);
			this.checkRxMulti.Name = "checkRxMulti";
			this.checkRxMulti.Size = new System.Drawing.Size(24, 15);
			this.checkRxMulti.TabIndex = 41;
			// 
			// comboRxMulti
			// 
			this.comboRxMulti.Location = new System.Drawing.Point(202, 243);
			this.comboRxMulti.Name = "comboRxMulti";
			this.comboRxMulti.Size = new System.Drawing.Size(253, 21);
			this.comboRxMulti.TabIndex = 39;
			// 
			// FormPrinterSetup
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(654, 433);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboDefault);
			this.Controls.Add(this.panelSimple);
			this.Controls.Add(this.checkSimple);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPrinterSetup";
			this.ShowInTaskbar = false;
			this.Text = "Printer Setup";
			this.Load += new System.EventHandler(this.FormPrinterSetup_Load);
			this.panelSimple.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.ComboBox comboDefault;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label labelTPandPerio;
		private OpenDental.UI.ComboBox comboLabelSheet;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private OpenDental.UI.ComboBox comboLabelSingle;
		private OpenDental.UI.ComboBox comboPostcard;
		private OpenDental.UI.ComboBox comboClaim;
		private OpenDental.UI.ComboBox comboRx;
		private OpenDental.UI.ComboBox comboStatement;
		private OpenDental.UI.ComboBox comboTPPerio;
		private System.Windows.Forms.Label label10;
		private OpenDental.UI.CheckBox checkDefault;
		private OpenDental.UI.CheckBox checkClaim;
		private OpenDental.UI.CheckBox checkLabelSheet;
		private OpenDental.UI.CheckBox checkLabelSingle;
		private OpenDental.UI.CheckBox checkPostcard;
		private OpenDental.UI.CheckBox checkRx;
		private OpenDental.UI.CheckBox checkTPPerio;
		private System.Windows.Forms.Label label11;
		private OpenDental.UI.CheckBox checkAppointments;
		private System.Windows.Forms.Label label12;
		private OpenDental.UI.ComboBox comboAppointments;
		private OpenDental.UI.CheckBox checkStatement;
		private System.Windows.Forms.Panel panelSimple;
		private OpenDental.UI.CheckBox checkSimple;
		private Label label13;
		private OpenDental.UI.CheckBox checkRxControlled;
		private OpenDental.UI.ComboBox comboRxControlled;
		private OpenDental.UI.ComboBox comboReceipt;
		private Label label14;
		private OpenDental.UI.CheckBox checkReceipt;
		private Label labelRxMulti;
		private OpenDental.UI.CheckBox checkRxMulti;
		private OpenDental.UI.ComboBox comboRxMulti;
	}
}
