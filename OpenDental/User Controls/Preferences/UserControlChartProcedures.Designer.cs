
namespace OpenDental {
	partial class UserControlChartProcedures {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
		if(disposing && (components != null)) {
		components.Dispose();
		}
		base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBoxProcedures = new OpenDental.UI.GroupBox();
			this.checkProcProvChangesCp = new OpenDental.UI.CheckBox();
			this.checkAllowSettingProcsComplete = new OpenDental.UI.CheckBox();
			this.checkProcEditRequireAutoCode = new OpenDental.UI.CheckBox();
			this.comboProcFeeUpdatePrompt = new OpenDental.UI.ComboBox();
			this.checkClaimProcsAllowEstimatesOnCompl = new OpenDental.UI.CheckBox();
			this.labelProcFeeUpdatePrompt = new System.Windows.Forms.Label();
			this.checkProcLockingIsAllowed = new OpenDental.UI.CheckBox();
			this.groupProcNotes = new OpenDental.UI.GroupBox();
			this.checkProcNoteSigsBlocked = new OpenDental.UI.CheckBox();
			this.checkProcsPromptForAutoNote = new OpenDental.UI.CheckBox();
			this.checkSignatureAllowDigital = new OpenDental.UI.CheckBox();
			this.checkProcGroupNoteDoesAggregate = new OpenDental.UI.CheckBox();
			this.checkNotesProviderSigOnly = new OpenDental.UI.CheckBox();
			this.checkProcNoteConcurrencyMerge = new OpenDental.UI.CheckBox();
			this.groupBoxProcedures.SuspendLayout();
			this.groupProcNotes.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxProcedures
			// 
			this.groupBoxProcedures.Controls.Add(this.checkProcProvChangesCp);
			this.groupBoxProcedures.Controls.Add(this.checkAllowSettingProcsComplete);
			this.groupBoxProcedures.Controls.Add(this.checkProcEditRequireAutoCode);
			this.groupBoxProcedures.Controls.Add(this.comboProcFeeUpdatePrompt);
			this.groupBoxProcedures.Controls.Add(this.checkClaimProcsAllowEstimatesOnCompl);
			this.groupBoxProcedures.Controls.Add(this.labelProcFeeUpdatePrompt);
			this.groupBoxProcedures.Controls.Add(this.checkProcLockingIsAllowed);
			this.groupBoxProcedures.Location = new System.Drawing.Point(20, 40);
			this.groupBoxProcedures.Name = "groupBoxProcedures";
			this.groupBoxProcedures.Size = new System.Drawing.Size(450, 201);
			this.groupBoxProcedures.TabIndex = 240;
			this.groupBoxProcedures.Text = "Procedures";
			// 
			// checkProcProvChangesCp
			// 
			this.checkProcProvChangesCp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkProcProvChangesCp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcProvChangesCp.Location = new System.Drawing.Point(5, 174);
			this.checkProcProvChangesCp.Name = "checkProcProvChangesCp";
			this.checkProcProvChangesCp.Size = new System.Drawing.Size(435, 17);
			this.checkProcProvChangesCp.TabIndex = 241;
			this.checkProcProvChangesCp.Text = "Changing treating provider updates Claim Proc provider";
			// 
			// checkAllowSettingProcsComplete
			// 
			this.checkAllowSettingProcsComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAllowSettingProcsComplete.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowSettingProcsComplete.Location = new System.Drawing.Point(15, 15);
			this.checkAllowSettingProcsComplete.Name = "checkAllowSettingProcsComplete";
			this.checkAllowSettingProcsComplete.Size = new System.Drawing.Size(425, 17);
			this.checkAllowSettingProcsComplete.TabIndex = 75;
			this.checkAllowSettingProcsComplete.Text = "Allow setting procedures complete";
			// 
			// checkProcEditRequireAutoCode
			// 
			this.checkProcEditRequireAutoCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkProcEditRequireAutoCode.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcEditRequireAutoCode.Location = new System.Drawing.Point(40, 77);
			this.checkProcEditRequireAutoCode.Name = "checkProcEditRequireAutoCode";
			this.checkProcEditRequireAutoCode.Size = new System.Drawing.Size(400, 17);
			this.checkProcEditRequireAutoCode.TabIndex = 222;
			this.checkProcEditRequireAutoCode.Text = "Require use of suggested auto codes";
			// 
			// comboProcFeeUpdatePrompt
			// 
			this.comboProcFeeUpdatePrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboProcFeeUpdatePrompt.Location = new System.Drawing.Point(221, 139);
			this.comboProcFeeUpdatePrompt.Name = "comboProcFeeUpdatePrompt";
			this.comboProcFeeUpdatePrompt.Size = new System.Drawing.Size(219, 21);
			this.comboProcFeeUpdatePrompt.TabIndex = 227;
			// 
			// checkClaimProcsAllowEstimatesOnCompl
			// 
			this.checkClaimProcsAllowEstimatesOnCompl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkClaimProcsAllowEstimatesOnCompl.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimProcsAllowEstimatesOnCompl.Checked = true;
			this.checkClaimProcsAllowEstimatesOnCompl.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkClaimProcsAllowEstimatesOnCompl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.checkClaimProcsAllowEstimatesOnCompl.Location = new System.Drawing.Point(40, 108);
			this.checkClaimProcsAllowEstimatesOnCompl.Name = "checkClaimProcsAllowEstimatesOnCompl";
			this.checkClaimProcsAllowEstimatesOnCompl.Size = new System.Drawing.Size(400, 17);
			this.checkClaimProcsAllowEstimatesOnCompl.TabIndex = 223;
			this.checkClaimProcsAllowEstimatesOnCompl.Text = "Allow estimates to be created for backdated completed procedures";
			this.checkClaimProcsAllowEstimatesOnCompl.Click += new System.EventHandler(this.checkClaimProcsAllowEstimatesOnCompl_Click);
			// 
			// labelProcFeeUpdatePrompt
			// 
			this.labelProcFeeUpdatePrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelProcFeeUpdatePrompt.Location = new System.Drawing.Point(15, 142);
			this.labelProcFeeUpdatePrompt.Name = "labelProcFeeUpdatePrompt";
			this.labelProcFeeUpdatePrompt.Size = new System.Drawing.Size(203, 18);
			this.labelProcFeeUpdatePrompt.TabIndex = 228;
			this.labelProcFeeUpdatePrompt.Text = "Procedure fee update behavior";
			this.labelProcFeeUpdatePrompt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkProcLockingIsAllowed
			// 
			this.checkProcLockingIsAllowed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkProcLockingIsAllowed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcLockingIsAllowed.Location = new System.Drawing.Point(40, 46);
			this.checkProcLockingIsAllowed.Name = "checkProcLockingIsAllowed";
			this.checkProcLockingIsAllowed.Size = new System.Drawing.Size(400, 17);
			this.checkProcLockingIsAllowed.TabIndex = 224;
			this.checkProcLockingIsAllowed.Text = "Procedure locking is allowed";
			this.checkProcLockingIsAllowed.Click += new System.EventHandler(this.checkProcLockingIsAllowed_Click);
			// 
			// groupProcNotes
			// 
			this.groupProcNotes.BackColor = System.Drawing.Color.White;
			this.groupProcNotes.Controls.Add(this.checkProcNoteSigsBlocked);
			this.groupProcNotes.Controls.Add(this.checkProcsPromptForAutoNote);
			this.groupProcNotes.Controls.Add(this.checkSignatureAllowDigital);
			this.groupProcNotes.Controls.Add(this.checkProcGroupNoteDoesAggregate);
			this.groupProcNotes.Controls.Add(this.checkNotesProviderSigOnly);
			this.groupProcNotes.Controls.Add(this.checkProcNoteConcurrencyMerge);
			this.groupProcNotes.Location = new System.Drawing.Point(20, 255);
			this.groupProcNotes.Name = "groupProcNotes";
			this.groupProcNotes.Size = new System.Drawing.Size(450, 191);
			this.groupProcNotes.TabIndex = 239;
			this.groupProcNotes.Text = "Procedure Notes";
			// 
			// checkProcNoteSigsBlocked
			// 
			this.checkProcNoteSigsBlocked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcNoteSigsBlocked.Location = new System.Drawing.Point(5, 164);
			this.checkProcNoteSigsBlocked.Name = "checkProcNoteSigsBlocked";
			this.checkProcNoteSigsBlocked.Size = new System.Drawing.Size(435, 17);
			this.checkProcNoteSigsBlocked.TabIndex = 249;
			this.checkProcNoteSigsBlocked.Text = "Block procedure note signatures when there are uncompleted auto note prompts";
			// 
			// checkProcsPromptForAutoNote
			// 
			this.checkProcsPromptForAutoNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkProcsPromptForAutoNote.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcsPromptForAutoNote.Location = new System.Drawing.Point(40, 15);
			this.checkProcsPromptForAutoNote.Name = "checkProcsPromptForAutoNote";
			this.checkProcsPromptForAutoNote.Size = new System.Drawing.Size(400, 17);
			this.checkProcsPromptForAutoNote.TabIndex = 234;
			this.checkProcsPromptForAutoNote.Text = "Procedures Prompt For Auto Note";
			// 
			// checkSignatureAllowDigital
			// 
			this.checkSignatureAllowDigital.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkSignatureAllowDigital.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSignatureAllowDigital.Location = new System.Drawing.Point(15, 46);
			this.checkSignatureAllowDigital.Name = "checkSignatureAllowDigital";
			this.checkSignatureAllowDigital.Size = new System.Drawing.Size(425, 17);
			this.checkSignatureAllowDigital.TabIndex = 238;
			this.checkSignatureAllowDigital.Text = "Allow digital signatures";
			// 
			// checkProcGroupNoteDoesAggregate
			// 
			this.checkProcGroupNoteDoesAggregate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkProcGroupNoteDoesAggregate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcGroupNoteDoesAggregate.Location = new System.Drawing.Point(15, 77);
			this.checkProcGroupNoteDoesAggregate.Name = "checkProcGroupNoteDoesAggregate";
			this.checkProcGroupNoteDoesAggregate.Size = new System.Drawing.Size(425, 17);
			this.checkProcGroupNoteDoesAggregate.TabIndex = 235;
			this.checkProcGroupNoteDoesAggregate.Text = "When creating a Group Note, aggregate procedure notes";
			// 
			// checkNotesProviderSigOnly
			// 
			this.checkNotesProviderSigOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkNotesProviderSigOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNotesProviderSigOnly.Location = new System.Drawing.Point(15, 135);
			this.checkNotesProviderSigOnly.Name = "checkNotesProviderSigOnly";
			this.checkNotesProviderSigOnly.Size = new System.Drawing.Size(425, 17);
			this.checkNotesProviderSigOnly.TabIndex = 237;
			this.checkNotesProviderSigOnly.Text = "Notes can only be signed by providers";
			// 
			// checkProcNoteConcurrencyMerge
			// 
			this.checkProcNoteConcurrencyMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkProcNoteConcurrencyMerge.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcNoteConcurrencyMerge.Location = new System.Drawing.Point(40, 106);
			this.checkProcNoteConcurrencyMerge.Name = "checkProcNoteConcurrencyMerge";
			this.checkProcNoteConcurrencyMerge.Size = new System.Drawing.Size(400, 17);
			this.checkProcNoteConcurrencyMerge.TabIndex = 236;
			this.checkProcNoteConcurrencyMerge.Text = "Procedure notes merge together when concurrency issues occur";
			// 
			// UserControlChartProcedures
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.groupBoxProcedures);
			this.Controls.Add(this.groupProcNotes);
			this.Name = "UserControlChartProcedures";
			this.Size = new System.Drawing.Size(494, 624);
			this.groupBoxProcedures.ResumeLayout(false);
			this.groupProcNotes.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.CheckBox checkAllowSettingProcsComplete;
		private OpenDental.UI.CheckBox checkProcEditRequireAutoCode;
		private OpenDental.UI.CheckBox checkClaimProcsAllowEstimatesOnCompl;
		private OpenDental.UI.CheckBox checkProcLockingIsAllowed;
		private UI.ComboBox comboProcFeeUpdatePrompt;
		private System.Windows.Forms.Label labelProcFeeUpdatePrompt;
		private OpenDental.UI.CheckBox checkSignatureAllowDigital;
		private OpenDental.UI.CheckBox checkNotesProviderSigOnly;
		private OpenDental.UI.CheckBox checkProcNoteConcurrencyMerge;
		private OpenDental.UI.CheckBox checkProcGroupNoteDoesAggregate;
		private OpenDental.UI.CheckBox checkProcsPromptForAutoNote;
		private UI.GroupBox groupProcNotes;
		private UI.GroupBox groupBoxProcedures;
		private OpenDental.UI.CheckBox checkProcProvChangesCp;
		private OpenDental.UI.CheckBox checkProcNoteSigsBlocked;
	}
}
