namespace OpenDental {
	partial class FormEhrEduResourcesPat {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrEduResourcesPat));
			this.butClose = new System.Windows.Forms.Button();
			this.gridEdu = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.gridProvided = new OpenDental.UI.GridOD();
			this.label2 = new System.Windows.Forms.Label();
			this.butDelete = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(784, 636);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridEdu
			// 
			this.gridEdu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridEdu.Location = new System.Drawing.Point(12, 53);
			this.gridEdu.Name = "gridEdu";
			this.gridEdu.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridEdu.Size = new System.Drawing.Size(847, 264);
			this.gridEdu.TabIndex = 1;
			this.gridEdu.Title = "Educational Resources";
			this.gridEdu.TranslationName = "TableResources";
			this.gridEdu.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridEdu_CellClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(679, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "To generate a patient education resource, single click on one of the links below," +
    " then print.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// gridProvided
			// 
			this.gridProvided.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProvided.Location = new System.Drawing.Point(12, 357);
			this.gridProvided.Name = "gridProvided";
			this.gridProvided.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProvided.Size = new System.Drawing.Size(847, 273);
			this.gridProvided.TabIndex = 3;
			this.gridProvided.Title = "Education Provided";
			this.gridProvided.TranslationName = "TableProvided";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 338);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(679, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "This is a historical record of education resources provided to this patient.  Del" +
    "ete any entries that are inaccurate.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Location = new System.Drawing.Point(12, 636);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 5;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 29);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(798, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "Please note that it will not be possible to enter patient-specific educational re" +
    "sources for patients who have no medications, problems, or lab results.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormEhrEduResourcesPat
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(871, 671);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.gridProvided);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridEdu);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrEduResourcesPat";
			this.Text = "Educational Resources";
			this.Load += new System.EventHandler(this.FormEduResourcesPat_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button butClose;
		private OpenDental.UI.GridOD gridEdu;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.GridOD gridProvided;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.Label label3;
	}
}