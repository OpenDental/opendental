
namespace OpenDental {
	partial class UserControlImagingGeneral {
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
			this.checkImagesModuleUsesOld2020 = new OpenDental.UI.CheckBox();
			this.labelImageCategoryDefaultDetails = new System.Windows.Forms.Label();
			this.labelVideoImageCategoryDefaultDetails = new System.Windows.Forms.Label();
			this.butVideoImageCategoryDefault = new OpenDental.UI.Button();
			this.label41 = new System.Windows.Forms.Label();
			this.textVideoImageCategoryDefault = new System.Windows.Forms.TextBox();
			this.groupBoxOD2 = new OpenDental.UI.GroupBoxOD();
			this.textDecimals = new OpenDental.ValidNum();
			this.textScale = new OpenDental.ValidDouble();
			this.textUnits = new System.Windows.Forms.TextBox();
			this.label34 = new System.Windows.Forms.Label();
			this.label35 = new System.Windows.Forms.Label();
			this.label36 = new System.Windows.Forms.Label();
			this.butBrowseImageCategoryDefault = new OpenDental.UI.Button();
			this.labelDefaultImageCategory = new System.Windows.Forms.Label();
			this.textImageCategoryDefault = new System.Windows.Forms.TextBox();
			this.butBrowseAutoImportFolder = new OpenDental.UI.Button();
			this.butBrowseDefaultImageImportFolder = new OpenDental.UI.Button();
			this.checkPDFLaunchWindow = new OpenDental.UI.CheckBox();
			this.label61 = new System.Windows.Forms.Label();
			this.textDefaultImageImportFolder = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.textAutoImportFolder = new System.Windows.Forms.TextBox();
			this.labelImagesModuleUsesOld2020Details = new System.Windows.Forms.Label();
			this.labelPDFLaunchWindowDetails = new System.Windows.Forms.Label();
			this.groupImport = new OpenDental.UI.GroupBoxOD();
			this.groupBoxFunctionality = new OpenDental.UI.GroupBoxOD();
			this.labelDefaultMeasurementScaleDetails = new System.Windows.Forms.Label();
			this.groupBoxOD2.SuspendLayout();
			this.groupImport.SuspendLayout();
			this.groupBoxFunctionality.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkImagesModuleUsesOld2020
			// 
			this.checkImagesModuleUsesOld2020.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkImagesModuleUsesOld2020.Location = new System.Drawing.Point(5, 41);
			this.checkImagesModuleUsesOld2020.Name = "checkImagesModuleUsesOld2020";
			this.checkImagesModuleUsesOld2020.Size = new System.Drawing.Size(435, 17);
			this.checkImagesModuleUsesOld2020.TabIndex = 316;
			this.checkImagesModuleUsesOld2020.Text = "Use old Imaging module interface, pre 2020";
			this.checkImagesModuleUsesOld2020.Click += new System.EventHandler(this.checkImagesModuleUsesOld2020_Click);
			// 
			// labelImageCategoryDefaultDetails
			// 
			this.labelImageCategoryDefaultDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelImageCategoryDefaultDetails.Location = new System.Drawing.Point(476, 121);
			this.labelImageCategoryDefaultDetails.Name = "labelImageCategoryDefaultDetails";
			this.labelImageCategoryDefaultDetails.Size = new System.Drawing.Size(498, 17);
			this.labelImageCategoryDefaultDetails.TabIndex = 315;
			this.labelImageCategoryDefaultDetails.Text = "for new images when no category selected";
			this.labelImageCategoryDefaultDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelVideoImageCategoryDefaultDetails
			// 
			this.labelVideoImageCategoryDefaultDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelVideoImageCategoryDefaultDetails.Location = new System.Drawing.Point(476, 154);
			this.labelVideoImageCategoryDefaultDetails.Name = "labelVideoImageCategoryDefaultDetails";
			this.labelVideoImageCategoryDefaultDetails.Size = new System.Drawing.Size(498, 18);
			this.labelVideoImageCategoryDefaultDetails.TabIndex = 314;
			this.labelVideoImageCategoryDefaultDetails.Text = "unless a mount is showing";
			this.labelVideoImageCategoryDefaultDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butVideoImageCategoryDefault
			// 
			this.butVideoImageCategoryDefault.BackColor = System.Drawing.SystemColors.Control;
			this.butVideoImageCategoryDefault.Location = new System.Drawing.Point(417, 112);
			this.butVideoImageCategoryDefault.Name = "butVideoImageCategoryDefault";
			this.butVideoImageCategoryDefault.Size = new System.Drawing.Size(23, 20);
			this.butVideoImageCategoryDefault.TabIndex = 312;
			this.butVideoImageCategoryDefault.Text = "...";
			this.butVideoImageCategoryDefault.UseVisualStyleBackColor = true;
			this.butVideoImageCategoryDefault.Click += new System.EventHandler(this.butVideoImageCategoryDefault_Click);
			// 
			// label41
			// 
			this.label41.Location = new System.Drawing.Point(54, 115);
			this.label41.Name = "label41";
			this.label41.Size = new System.Drawing.Size(232, 17);
			this.label41.TabIndex = 313;
			this.label41.Text = "Default Image Category for video capture";
			this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVideoImageCategoryDefault
			// 
			this.textVideoImageCategoryDefault.Location = new System.Drawing.Point(289, 112);
			this.textVideoImageCategoryDefault.Name = "textVideoImageCategoryDefault";
			this.textVideoImageCategoryDefault.Size = new System.Drawing.Size(122, 20);
			this.textVideoImageCategoryDefault.TabIndex = 311;
			// 
			// groupBoxOD2
			// 
			this.groupBoxOD2.BackColor = System.Drawing.Color.White;
			this.groupBoxOD2.Controls.Add(this.textDecimals);
			this.groupBoxOD2.Controls.Add(this.textScale);
			this.groupBoxOD2.Controls.Add(this.textUnits);
			this.groupBoxOD2.Controls.Add(this.label34);
			this.groupBoxOD2.Controls.Add(this.label35);
			this.groupBoxOD2.Controls.Add(this.label36);
			this.groupBoxOD2.Location = new System.Drawing.Point(20, 278);
			this.groupBoxOD2.Name = "groupBoxOD2";
			this.groupBoxOD2.Size = new System.Drawing.Size(450, 98);
			this.groupBoxOD2.TabIndex = 307;
			this.groupBoxOD2.Text = "Default Measurement Scale";
			// 
			// textDecimals
			// 
			this.textDecimals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDecimals.Location = new System.Drawing.Point(392, 67);
			this.textDecimals.MaxVal = 20;
			this.textDecimals.Name = "textDecimals";
			this.textDecimals.Size = new System.Drawing.Size(48, 20);
			this.textDecimals.TabIndex = 2;
			// 
			// textScale
			// 
			this.textScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textScale.Location = new System.Drawing.Point(392, 41);
			this.textScale.MaxVal = 100000000D;
			this.textScale.MinVal = -100000000D;
			this.textScale.Name = "textScale";
			this.textScale.Size = new System.Drawing.Size(48, 20);
			this.textScale.TabIndex = 1;
			// 
			// textUnits
			// 
			this.textUnits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textUnits.Location = new System.Drawing.Point(392, 15);
			this.textUnits.Name = "textUnits";
			this.textUnits.Size = new System.Drawing.Size(48, 20);
			this.textUnits.TabIndex = 0;
			this.textUnits.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label34
			// 
			this.label34.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label34.Location = new System.Drawing.Point(265, 44);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(124, 17);
			this.label34.TabIndex = 46;
			this.label34.Text = "Scale, pixels per unit";
			this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label35
			// 
			this.label35.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label35.Location = new System.Drawing.Point(271, 18);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(118, 17);
			this.label35.TabIndex = 45;
			this.label35.Text = "Optional Units (mm)";
			this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label36
			// 
			this.label36.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label36.Location = new System.Drawing.Point(289, 70);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(100, 17);
			this.label36.TabIndex = 44;
			this.label36.Text = "Decimal Places";
			this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butBrowseImageCategoryDefault
			// 
			this.butBrowseImageCategoryDefault.BackColor = System.Drawing.SystemColors.Control;
			this.butBrowseImageCategoryDefault.Location = new System.Drawing.Point(417, 78);
			this.butBrowseImageCategoryDefault.Name = "butBrowseImageCategoryDefault";
			this.butBrowseImageCategoryDefault.Size = new System.Drawing.Size(23, 20);
			this.butBrowseImageCategoryDefault.TabIndex = 305;
			this.butBrowseImageCategoryDefault.Text = "...";
			this.butBrowseImageCategoryDefault.UseVisualStyleBackColor = true;
			this.butBrowseImageCategoryDefault.Click += new System.EventHandler(this.butBrowseImageCategoryDefault_Click);
			// 
			// labelDefaultImageCategory
			// 
			this.labelDefaultImageCategory.Location = new System.Drawing.Point(93, 81);
			this.labelDefaultImageCategory.Name = "labelDefaultImageCategory";
			this.labelDefaultImageCategory.Size = new System.Drawing.Size(193, 17);
			this.labelDefaultImageCategory.TabIndex = 310;
			this.labelDefaultImageCategory.Text = "Default Image Category";
			this.labelDefaultImageCategory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textImageCategoryDefault
			// 
			this.textImageCategoryDefault.Location = new System.Drawing.Point(289, 78);
			this.textImageCategoryDefault.Name = "textImageCategoryDefault";
			this.textImageCategoryDefault.Size = new System.Drawing.Size(122, 20);
			this.textImageCategoryDefault.TabIndex = 304;
			// 
			// butBrowseAutoImportFolder
			// 
			this.butBrowseAutoImportFolder.BackColor = System.Drawing.SystemColors.Control;
			this.butBrowseAutoImportFolder.Location = new System.Drawing.Point(417, 44);
			this.butBrowseAutoImportFolder.Name = "butBrowseAutoImportFolder";
			this.butBrowseAutoImportFolder.Size = new System.Drawing.Size(23, 20);
			this.butBrowseAutoImportFolder.TabIndex = 303;
			this.butBrowseAutoImportFolder.Text = "...";
			this.butBrowseAutoImportFolder.UseVisualStyleBackColor = true;
			this.butBrowseAutoImportFolder.Click += new System.EventHandler(this.butBrowseAutoImportFolder_Click);
			// 
			// butBrowseDefaultImageImportFolder
			// 
			this.butBrowseDefaultImageImportFolder.BackColor = System.Drawing.SystemColors.Control;
			this.butBrowseDefaultImageImportFolder.Location = new System.Drawing.Point(417, 10);
			this.butBrowseDefaultImageImportFolder.Name = "butBrowseDefaultImageImportFolder";
			this.butBrowseDefaultImageImportFolder.Size = new System.Drawing.Size(23, 20);
			this.butBrowseDefaultImageImportFolder.TabIndex = 301;
			this.butBrowseDefaultImageImportFolder.Text = "...";
			this.butBrowseDefaultImageImportFolder.UseVisualStyleBackColor = true;
			this.butBrowseDefaultImageImportFolder.Click += new System.EventHandler(this.butBrowseImportFolder_Click);
			// 
			// checkPDFLaunchWindow
			// 
			this.checkPDFLaunchWindow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPDFLaunchWindow.Location = new System.Drawing.Point(90, 10);
			this.checkPDFLaunchWindow.Name = "checkPDFLaunchWindow";
			this.checkPDFLaunchWindow.Size = new System.Drawing.Size(350, 17);
			this.checkPDFLaunchWindow.TabIndex = 306;
			this.checkPDFLaunchWindow.Text = "PDF files always launch in a separate window";
			// 
			// label61
			// 
			this.label61.Location = new System.Drawing.Point(44, 13);
			this.label61.Name = "label61";
			this.label61.Size = new System.Drawing.Size(134, 17);
			this.label61.TabIndex = 309;
			this.label61.Text = "Default import folder";
			this.label61.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDefaultImageImportFolder
			// 
			this.textDefaultImageImportFolder.Location = new System.Drawing.Point(181, 10);
			this.textDefaultImageImportFolder.Name = "textDefaultImageImportFolder";
			this.textDefaultImageImportFolder.Size = new System.Drawing.Size(230, 20);
			this.textDefaultImageImportFolder.TabIndex = 300;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(3, 47);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(175, 17);
			this.label15.TabIndex = 308;
			this.label15.Text = "Default folder for automatic import";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAutoImportFolder
			// 
			this.textAutoImportFolder.Location = new System.Drawing.Point(181, 44);
			this.textAutoImportFolder.Name = "textAutoImportFolder";
			this.textAutoImportFolder.Size = new System.Drawing.Size(230, 20);
			this.textAutoImportFolder.TabIndex = 302;
			// 
			// labelImagesModuleUsesOld2020Details
			// 
			this.labelImagesModuleUsesOld2020Details.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelImagesModuleUsesOld2020Details.Location = new System.Drawing.Point(476, 236);
			this.labelImagesModuleUsesOld2020Details.Name = "labelImagesModuleUsesOld2020Details";
			this.labelImagesModuleUsesOld2020Details.Size = new System.Drawing.Size(498, 17);
			this.labelImagesModuleUsesOld2020Details.TabIndex = 317;
			this.labelImagesModuleUsesOld2020Details.Text = "not recommended";
			this.labelImagesModuleUsesOld2020Details.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPDFLaunchWindowDetails
			// 
			this.labelPDFLaunchWindowDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelPDFLaunchWindowDetails.Location = new System.Drawing.Point(476, 205);
			this.labelPDFLaunchWindowDetails.Name = "labelPDFLaunchWindowDetails";
			this.labelPDFLaunchWindowDetails.Size = new System.Drawing.Size(498, 17);
			this.labelPDFLaunchWindowDetails.TabIndex = 318;
			this.labelPDFLaunchWindowDetails.Text = "can help with Remote Desktop";
			this.labelPDFLaunchWindowDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupImport
			// 
			this.groupImport.Controls.Add(this.label61);
			this.groupImport.Controls.Add(this.textAutoImportFolder);
			this.groupImport.Controls.Add(this.label15);
			this.groupImport.Controls.Add(this.textDefaultImageImportFolder);
			this.groupImport.Controls.Add(this.butBrowseDefaultImageImportFolder);
			this.groupImport.Controls.Add(this.butBrowseAutoImportFolder);
			this.groupImport.Controls.Add(this.butVideoImageCategoryDefault);
			this.groupImport.Controls.Add(this.textImageCategoryDefault);
			this.groupImport.Controls.Add(this.label41);
			this.groupImport.Controls.Add(this.labelDefaultImageCategory);
			this.groupImport.Controls.Add(this.textVideoImageCategoryDefault);
			this.groupImport.Controls.Add(this.butBrowseImageCategoryDefault);
			this.groupImport.Location = new System.Drawing.Point(20, 40);
			this.groupImport.Name = "groupImport";
			this.groupImport.Size = new System.Drawing.Size(450, 142);
			this.groupImport.TabIndex = 319;
			this.groupImport.Text = "Imports";
			// 
			// groupBoxFunctionality
			// 
			this.groupBoxFunctionality.Controls.Add(this.checkPDFLaunchWindow);
			this.groupBoxFunctionality.Controls.Add(this.checkImagesModuleUsesOld2020);
			this.groupBoxFunctionality.Location = new System.Drawing.Point(20, 196);
			this.groupBoxFunctionality.Name = "groupBoxFunctionality";
			this.groupBoxFunctionality.Size = new System.Drawing.Size(450, 68);
			this.groupBoxFunctionality.TabIndex = 320;
			this.groupBoxFunctionality.Text = "Functionality";
			// 
			// labelDefaultMeasurementScaleDetails
			// 
			this.labelDefaultMeasurementScaleDetails.ForeColor = System.Drawing.Color.MidnightBlue;
			this.labelDefaultMeasurementScaleDetails.Location = new System.Drawing.Point(476, 278);
			this.labelDefaultMeasurementScaleDetails.Name = "labelDefaultMeasurementScaleDetails";
			this.labelDefaultMeasurementScaleDetails.Size = new System.Drawing.Size(498, 17);
			this.labelDefaultMeasurementScaleDetails.TabIndex = 321;
			this.labelDefaultMeasurementScaleDetails.Text = "for new single images, does not apply to mounts";
			this.labelDefaultMeasurementScaleDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// UserControlImagingGeneral
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.labelDefaultMeasurementScaleDetails);
			this.Controls.Add(this.groupBoxFunctionality);
			this.Controls.Add(this.groupImport);
			this.Controls.Add(this.labelPDFLaunchWindowDetails);
			this.Controls.Add(this.labelImagesModuleUsesOld2020Details);
			this.Controls.Add(this.labelImageCategoryDefaultDetails);
			this.Controls.Add(this.labelVideoImageCategoryDefaultDetails);
			this.Controls.Add(this.groupBoxOD2);
			this.Name = "UserControlImagingGeneral";
			this.Size = new System.Drawing.Size(974, 624);
			this.groupBoxOD2.ResumeLayout(false);
			this.groupBoxOD2.PerformLayout();
			this.groupImport.ResumeLayout(false);
			this.groupImport.PerformLayout();
			this.groupBoxFunctionality.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.CheckBox checkImagesModuleUsesOld2020;
		private System.Windows.Forms.Label labelImageCategoryDefaultDetails;
		private System.Windows.Forms.Label labelVideoImageCategoryDefaultDetails;
		private UI.Button butVideoImageCategoryDefault;
		private System.Windows.Forms.Label label41;
		private System.Windows.Forms.TextBox textVideoImageCategoryDefault;
		private UI.GroupBoxOD groupBoxOD2;
		private ValidNum textDecimals;
		private ValidDouble textScale;
		private System.Windows.Forms.TextBox textUnits;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.Label label36;
		private UI.Button butBrowseImageCategoryDefault;
		private System.Windows.Forms.Label labelDefaultImageCategory;
		private System.Windows.Forms.TextBox textImageCategoryDefault;
		private UI.Button butBrowseAutoImportFolder;
		private UI.Button butBrowseDefaultImageImportFolder;
		private OpenDental.UI.CheckBox checkPDFLaunchWindow;
		private System.Windows.Forms.Label label61;
		private System.Windows.Forms.TextBox textDefaultImageImportFolder;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textAutoImportFolder;
		private System.Windows.Forms.Label labelImagesModuleUsesOld2020Details;
		private System.Windows.Forms.Label labelPDFLaunchWindowDetails;
		private UI.GroupBoxOD groupImport;
		private UI.GroupBoxOD groupBoxFunctionality;
		private System.Windows.Forms.Label labelDefaultMeasurementScaleDetails;
	}
}
