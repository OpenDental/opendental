using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormScheduleEdit {
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
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScheduleEdit));
			this.labelStop = new System.Windows.Forms.Label();
			this.labelStart = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboStop = new System.Windows.Forms.ComboBox();
			this.comboStart = new System.Windows.Forms.ComboBox();
			this.listBoxOps = new OpenDental.UI.ListBoxOD();
			this.labelOps = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.SuspendLayout();
			// 
			// labelStop
			// 
			this.labelStop.Location = new System.Drawing.Point(6, 39);
			this.labelStop.Name = "labelStop";
			this.labelStop.Size = new System.Drawing.Size(89, 16);
			this.labelStop.TabIndex = 9;
			this.labelStop.Text = "Stop Time";
			this.labelStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStart
			// 
			this.labelStart.Location = new System.Drawing.Point(6, 12);
			this.labelStart.Name = "labelStart";
			this.labelStart.Size = new System.Drawing.Size(89, 16);
			this.labelStart.TabIndex = 7;
			this.labelStart.Text = "Start Time";
			this.labelStart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(97, 92);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Schedule;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(220, 113);
			this.textNote.TabIndex = 15;
			this.textNote.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 93);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(89, 16);
			this.label4.TabIndex = 16;
			this.label4.Text = "Note";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStop
			// 
			this.comboStop.FormattingEnabled = true;
			this.comboStop.Location = new System.Drawing.Point(97, 38);
			this.comboStop.MaxDropDownItems = 48;
			this.comboStop.Name = "comboStop";
			this.comboStop.Size = new System.Drawing.Size(120, 21);
			this.comboStop.TabIndex = 25;
			// 
			// comboStart
			// 
			this.comboStart.FormattingEnabled = true;
			this.comboStart.Location = new System.Drawing.Point(97, 11);
			this.comboStart.MaxDropDownItems = 48;
			this.comboStart.Name = "comboStart";
			this.comboStart.Size = new System.Drawing.Size(120, 21);
			this.comboStart.TabIndex = 24;
			// 
			// listOps
			// 
			this.listBoxOps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxOps.IntegralHeight = false;
			this.listBoxOps.Location = new System.Drawing.Point(348, 34);
			this.listBoxOps.Name = "listOps";
			this.listBoxOps.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxOps.Size = new System.Drawing.Size(243, 349);
			this.listBoxOps.TabIndex = 27;
			// 
			// labelOps
			// 
			this.labelOps.Location = new System.Drawing.Point(348, 2);
			this.labelOps.Name = "labelOps";
			this.labelOps.Size = new System.Drawing.Size(243, 28);
			this.labelOps.TabIndex = 26;
			this.labelOps.Text = "Operatories. Usually, do not select operatories.  Only used to override default o" +
    "peratory provider.";
			this.labelOps.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(516, 393);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 14;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(428, 393);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 12;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(61, 65);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(234, 21);
			this.comboClinic.TabIndex = 94;
			this.comboClinic.Visible = false;
			// 
			// FormScheduleEdit
			// 
			this.ClientSize = new System.Drawing.Size(603, 431);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.listBoxOps);
			this.Controls.Add(this.labelOps);
			this.Controls.Add(this.comboStop);
			this.Controls.Add(this.comboStart);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelStop);
			this.Controls.Add(this.labelStart);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormScheduleEdit";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Edit Schedule";
			this.Load += new System.EventHandler(this.FormScheduleEdit_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label labelStop;
		private System.Windows.Forms.Label labelStart;
		private OpenDental.ODtextBox textNote;
		private System.Windows.Forms.Label label4;
		private ComboBox comboStop;
		private ComboBox comboStart;
		private OpenDental.UI.ListBoxOD listBoxOps;
		private Label labelOps;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
	}
}
