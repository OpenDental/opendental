namespace OpenDental {
	partial class FormEhrLabNoteEdit {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrLabNoteEdit));
			this.butCancel = new System.Windows.Forms.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butSave = new System.Windows.Forms.Button();
			this.butDelete = new System.Windows.Forms.Button();
			this.butAddComment = new System.Windows.Forms.Button();
			this.label57 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(627, 357);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(609, 339);
			this.gridMain.TabIndex = 5;
			this.gridMain.Title = "Lab Note Comments";
			this.gridMain.TranslationName = "TableLabNote";
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(546, 357);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 23);
			this.butSave.TabIndex = 10;
			this.butSave.Text = "Save";
			this.butSave.UseVisualStyleBackColor = true;
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12, 357);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 11;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			// 
			// butAddComment
			// 
			this.butAddComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddComment.Location = new System.Drawing.Point(627, 12);
			this.butAddComment.Name = "butAddComment";
			this.butAddComment.Size = new System.Drawing.Size(75, 23);
			this.butAddComment.TabIndex = 12;
			this.butAddComment.Text = "Add";
			this.butAddComment.UseVisualStyleBackColor = true;
			this.butAddComment.Click += new System.EventHandler(this.butAddComment_Click);
			// 
			// label57
			// 
			this.label57.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label57.Location = new System.Drawing.Point(93, 360);
			this.label57.Name = "label57";
			this.label57.Size = new System.Drawing.Size(137, 17);
			this.label57.TabIndex = 259;
			this.label57.Text = "Deletes the entire lab note";
			this.label57.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormEhrLabNoteEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(714, 392);
			this.Controls.Add(this.label57);
			this.Controls.Add(this.butAddComment);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrLabNoteEdit";
			this.Text = "Lab Note";
			this.Load += new System.EventHandler(this.FormEhrLabOrders_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butSave;
		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.Button butAddComment;
		private System.Windows.Forms.Label label57;
	}
}