namespace OpenDental {
	partial class FormEduResourceSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEduResourceSetup));
			this.butClose = new System.Windows.Forms.Button();
			this.butAdd = new System.Windows.Forms.Button();
			this.gridEdu = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(805, 496);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Location = new System.Drawing.Point(12, 496);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 23);
			this.butAdd.TabIndex = 4;
			this.butAdd.Text = "Add";
			this.butAdd.UseVisualStyleBackColor = true;
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridEdu
			// 
			this.gridEdu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridEdu.Location = new System.Drawing.Point(12, 12);
			this.gridEdu.Name = "gridEdu";
			this.gridEdu.Size = new System.Drawing.Size(868, 478);
			this.gridEdu.TabIndex = 3;
			this.gridEdu.Title = "Educational Resources";
			this.gridEdu.TranslationName = "TableResources";
			this.gridEdu.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridEdu_CellDoubleClick);
			// 
			// FormEduResourceSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(892, 531);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridEdu);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEduResourceSetup";
			this.Text = "Education Resources Setup";
			this.Load += new System.EventHandler(this.FormEduResourceSetup_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GridOD gridEdu;
		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.Button butAdd;
	}
}