
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
			this.checkAllowSettingProcsComplete = new System.Windows.Forms.CheckBox();
			this.checkProcEditRequireAutoCode = new System.Windows.Forms.CheckBox();
			this.checkClaimProcsAllowEstimatesOnCompl = new System.Windows.Forms.CheckBox();
			this.checkProcLockingIsAllowed = new System.Windows.Forms.CheckBox();
			this.comboProcFeeUpdatePrompt = new OpenDental.UI.ComboBoxOD();
			this.labelProcFeeUpdatePrompt = new System.Windows.Forms.Label();
			this.checkSignatureAllowDigital = new System.Windows.Forms.CheckBox();
			this.checkNotesProviderSigOnly = new System.Windows.Forms.CheckBox();
			this.checkProcNoteConcurrencyMerge = new System.Windows.Forms.CheckBox();
			this.checkProcGroupNoteDoesAggregate = new System.Windows.Forms.CheckBox();
			this.checkProcsPromptForAutoNote = new System.Windows.Forms.CheckBox();
			this.groupProcNotes = new OpenDental.UI.GroupBoxOD();
			this.checkProcNoteSigsBlocked = new System.Windows.Forms.CheckBox();
			this.groupBoxProcedures = new OpenDental.UI.GroupBoxOD();
			this.checkProcProvChangesCp = new System.Windows.Forms.CheckBox();
			this.labelAllowSettingProcsCompleteDetails = new System.Windows.Forms.Label();
			this.labelProcProvChangesCpDetails = new System.Windows.Forms.Label();
			this.labelProcEditRequireAutoCodeDetails = new System.Windows.Forms.Label();
			this.labelProcFeeUpdatePromptDetails = new System.Windows.Forms.Label();
			this.labelProcsPromptForAutoNoteDetails = new System.Windows.Forms.Label();
			this.groupProcNotes.SuspendLayout();
			this.groupBoxProcedures.SuspendLayout();
			this.SuspendLayout();
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
			this.checkAllowSettingProcsComplete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAllowSettingProcsComplete.UseVisualStyleBackColor = true;
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
			this.checkProcEditRequireAutoCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcEditRequireAutoCode.UseVisualStyleBackColor = true;
			// 
			// checkClaimProcsAllowEstimatesOnCompl
			// 
			this.checkClaimProcsAllowEstimatesOnCompl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkClaimProcsAllowEstimatesOnCompl.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimProcsAllowEstimatesOnCompl.Checked = true;
			this.checkClaimProcsAllowEstimatesOnCompl.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkClaimProcsAllowEstimatesOnCompl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.checkClaimProcsAllowEstimatesOnCompl.Location = new System.Drawing.Point(40, 100);
			this.checkClaimProcsAllowEstimatesOnCompl.Name = "checkClaimProcsAllowEstimatesOnCompl";
			this.checkClaimProcsAllowEstimatesOnCompl.Size = new System.Drawing.Size(400, 34);
			this.checkClaimProcsAllowEstimatesOnCompl.TabIndex = 223;
			this.checkClaimProcsAllowEstimatesOnCompl.Text = "Allow estimates to be created for backdated completed procedures\r\n(not recommende" +
    "d, see manual)";
			this.checkClaimProcsAllowEstimatesOnCompl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimProcsAllowEstimatesOnCompl.UseVisualStyleBackColor = true;
			this.checkClaimProcsAllowEstimatesOnCompl.CheckedChanged += new System.EventHandler(this.checkClaimProcsAllowEstimatesOnCompl_CheckedChanged);
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
			this.checkProcLockingIsAllowed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcLockingIsAllowed.UseVisualStyleBackColor = true;
			this.checkProcLockingIsAllowed.Click += new System.EventHandler(this.checkProcLockingIsAllowed_Click);
			// 
			// comboProcFeeUpdatePrompt
			// 
			this.comboProcFeeUpdatePrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboProcFeeUpdatePrompt.Location = new System.Drawing.Point(221, 140);
			this.comboProcFeeUpdatePrompt.Name = "comboProcFeeUpdatePrompt";
			this.comboProcFeeUpdatePrompt.Size = new System.Drawing.Size(219, 21);
			this.comboProcFeeUpdatePrompt.TabIndex = 227;
			// 
			// labelProcFeeUpdatePrompt
			// 
			this.labelProcFeeUpdatePrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelProcFeeUpdatePrompt.Location = new System.Drawing.Point(57, 143);
			this.labelProcFeeUpdatePrompt.Name = "labelProcFeeUpdatePrompt";
			this.labelProcFeeUpdatePrompt.Size = new System.Drawing.Size(161, 15);
			this.labelProcFeeUpdatePrompt.TabIndex = 228;
			this.labelProcFeeUpdatePrompt.Text = "Procedure fee update behavior";
			this.labelProcFeeUpdatePrompt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
			this.checkSignatureAllowDigital.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSignatureAllowDigital.UseVisualStyleBackColor = true;
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
			this.checkNotesProviderSigOnly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNotesProviderSigOnly.UseVisualStyleBackColor = true;
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
			this.checkProcNoteConcurrencyMerge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcNoteConcurrencyMerge.UseVisualStyleBackColor = true;
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
			this.checkProcGroupNoteDoesAggregate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcGroupNoteDoesAggregate.UseVisualStyleBackColor = true;
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
			this.checkProcsPromptForAutoNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcsPromptForAutoNote.UseVisualStyleBackColor = true;
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
			this.groupProcNotes.Location = new System.Drawing.Point(20, 256);
			this.groupProcNotes.Name = "groupProcNotes";
			this.groupProcNotes.Size = new System.Drawing.Size(450, 191);
			this.groupProcNotes.TabIndex = 239;
			this.groupProcNotes.Text = "Procedure Notes";
			// 
			// checkProcNoteSigsBlocked
			// 
			this.checkProcNoteSigsBlocked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcNoteSigsBlocked.Location = new System.Drawing.Point(15, 164);
			this.checkProcNoteSigsBlocked.Name = "checkProcNoteSigsBlocked";
			this.checkProcNoteSigsBlocked.Size = new System.Drawing.Size(425, 17);
			this.checkProcNoteSigsBlocked.TabIndex = 249;
			this.checkProcNoteSigsBlocked.Text = "Block procedure note signatures when there are uncompleted auto note prompts";
			this.checkProcNoteSigsBlocked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcNoteSigsBlocked.UseVisualStyleBackColor = true;
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
			this.groupBoxProcedures.Size = new System.Drawing.Size(450, 202);
			this.groupBoxProcedures.TabIndex = 240;
			this.groupBoxProcedures.Text = "Procedures";
			// 
			// checkProcProvChangesCp
			// 
			this.checkProcProvChangesCp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkProcProvChangesCp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcProvChangesCp.Location = new System.Drawing.Point(5, 175);
			this.checkProcProvChangesCp.Name = "checkProcProvChangesCp";
			this.checkProcProvChangesCp.Size = new System.Drawing.Size(435, 17);
			this.checkProcProvChangesCp.TabIndex = 241;
			this.checkProcProvChangesCp.Text = "Procedures on claims must use the treating provider";
			this.checkProcProvChangesCp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcProvChangesCp.UseVisualStyleBackColor = true;
			// 
			// labelAllowSettingProcsCompleteDetails
			// 
			this.labelAllowSettingProcsCompleteDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAllowSettingProcsCompleteDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelAllowSettingProcsCompleteDetails.Location = new System.Drawing.Point(476, 54);
			this.labelAllowSettingProcsCompleteDetails.Name = "labelAllowSettingProcsCompleteDetails";
			this.labelAllowSettingProcsCompleteDetails.Size = new System.Drawing.Size(498, 17);
			this.labelAllowSettingProcsCompleteDetails.TabIndex = 349;
			this.labelAllowSettingProcsCompleteDetails.Text = "not usually recommended, it\'s better to only set appointments complete";
			this.labelAllowSettingProcsCompleteDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProcProvChangesCpDetails
			// 
			this.labelProcProvChangesCpDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProcProvChangesCpDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelProcProvChangesCpDetails.Location = new System.Drawing.Point(476, 214);
			this.labelProcProvChangesCpDetails.Name = "labelProcProvChangesCpDetails";
			this.labelProcProvChangesCpDetails.Size = new System.Drawing.Size(498, 17);
			this.labelProcProvChangesCpDetails.TabIndex = 350;
			this.labelProcProvChangesCpDetails.Text = "normally checked";
			this.labelProcProvChangesCpDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProcEditRequireAutoCodeDetails
			// 
			this.labelProcEditRequireAutoCodeDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProcEditRequireAutoCodeDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelProcEditRequireAutoCodeDetails.Location = new System.Drawing.Point(476, 116);
			this.labelProcEditRequireAutoCodeDetails.Name = "labelProcEditRequireAutoCodeDetails";
			this.labelProcEditRequireAutoCodeDetails.Size = new System.Drawing.Size(498, 17);
			this.labelProcEditRequireAutoCodeDetails.TabIndex = 354;
			this.labelProcEditRequireAutoCodeDetails.Text = "otherwise a user is allowed to chart the mismatched code";
			this.labelProcEditRequireAutoCodeDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProcFeeUpdatePromptDetails
			// 
			this.labelProcFeeUpdatePromptDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProcFeeUpdatePromptDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelProcFeeUpdatePromptDetails.Location = new System.Drawing.Point(476, 182);
			this.labelProcFeeUpdatePromptDetails.Name = "labelProcFeeUpdatePromptDetails";
			this.labelProcFeeUpdatePromptDetails.Size = new System.Drawing.Size(498, 17);
			this.labelProcFeeUpdatePromptDetails.TabIndex = 355;
			this.labelProcFeeUpdatePromptDetails.Text = "when changing the provider";
			this.labelProcFeeUpdatePromptDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProcsPromptForAutoNoteDetails
			// 
			this.labelProcsPromptForAutoNoteDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProcsPromptForAutoNoteDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelProcsPromptForAutoNoteDetails.Location = new System.Drawing.Point(476, 270);
			this.labelProcsPromptForAutoNoteDetails.Name = "labelProcsPromptForAutoNoteDetails";
			this.labelProcsPromptForAutoNoteDetails.Size = new System.Drawing.Size(498, 17);
			this.labelProcsPromptForAutoNoteDetails.TabIndex = 356;
			this.labelProcsPromptForAutoNoteDetails.Text = "if auto note prompts were not completed, be prompted when the procedure is next o" +
    "pened";
			this.labelProcsPromptForAutoNoteDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// UserControlChartProcedures
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.labelProcsPromptForAutoNoteDetails);
			this.Controls.Add(this.labelProcFeeUpdatePromptDetails);
			this.Controls.Add(this.labelProcEditRequireAutoCodeDetails);
			this.Controls.Add(this.labelProcProvChangesCpDetails);
			this.Controls.Add(this.labelAllowSettingProcsCompleteDetails);
			this.Controls.Add(this.groupBoxProcedures);
			this.Controls.Add(this.groupProcNotes);
			this.Name = "UserControlChartProcedures";
			this.Size = new System.Drawing.Size(974, 641);
			this.groupProcNotes.ResumeLayout(false);
			this.groupBoxProcedures.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox checkAllowSettingProcsComplete;
		private System.Windows.Forms.CheckBox checkProcEditRequireAutoCode;
		private System.Windows.Forms.CheckBox checkClaimProcsAllowEstimatesOnCompl;
		private System.Windows.Forms.CheckBox checkProcLockingIsAllowed;
		private UI.ComboBoxOD comboProcFeeUpdatePrompt;
		private System.Windows.Forms.Label labelProcFeeUpdatePrompt;
		private System.Windows.Forms.CheckBox checkSignatureAllowDigital;
		private System.Windows.Forms.CheckBox checkNotesProviderSigOnly;
		private System.Windows.Forms.CheckBox checkProcNoteConcurrencyMerge;
		private System.Windows.Forms.CheckBox checkProcGroupNoteDoesAggregate;
		private System.Windows.Forms.CheckBox checkProcsPromptForAutoNote;
		private UI.GroupBoxOD groupProcNotes;
		private UI.GroupBoxOD groupBoxProcedures;
		private System.Windows.Forms.CheckBox checkProcProvChangesCp;
		private System.Windows.Forms.Label labelAllowSettingProcsCompleteDetails;
		private System.Windows.Forms.Label labelProcProvChangesCpDetails;
		private System.Windows.Forms.Label labelProcEditRequireAutoCodeDetails;
		private System.Windows.Forms.Label labelProcFeeUpdatePromptDetails;
		private System.Windows.Forms.Label labelProcsPromptForAutoNoteDetails;
		private System.Windows.Forms.CheckBox checkProcNoteSigsBlocked;
	}
}
