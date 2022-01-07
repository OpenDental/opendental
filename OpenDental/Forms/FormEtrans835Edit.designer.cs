namespace OpenDental{
	partial class FormEtrans835Edit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEtrans835Edit));
			this.label5 = new System.Windows.Forms.Label();
			this.textTransHandlingDesc = new System.Windows.Forms.TextBox();
			this.textPaymentMethod = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textPaymentAmount = new System.Windows.Forms.TextBox();
			this.labelPaymentAmount = new System.Windows.Forms.Label();
			this.textAcctNumEndingIn = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textDateEffective = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textCheckNumOrRefNum = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textPayerName = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textPayerID = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textPayerAddress1 = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textPayerCity = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.textPayerState = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.textPayerZip = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textPayerContactInfo = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textPayeeName = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.labelPayeeIdType = new System.Windows.Forms.Label();
			this.textPayeeID = new System.Windows.Forms.TextBox();
			this.textClaimInsPaidSum = new System.Windows.Forms.TextBox();
			this.textProjAdjAmtSum = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.textPayAmountCalc = new System.Windows.Forms.TextBox();
			this.groupBalancing = new System.Windows.Forms.GroupBox();
			this.labelEquation3 = new System.Windows.Forms.Label();
			this.labelEquation2 = new System.Windows.Forms.Label();
			this.labelEquation1 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butClaimDetails = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.butRawMessage = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butBatch = new OpenDental.UI.Button();
			this.butDetachClaim = new OpenDental.UI.Button();
			this.butPreview = new OpenDental.UI.Button();
			this.buttonFindClaimMatch = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butAutoProcess = new OpenDental.UI.Button();
			this.gridProviderAdjustments = new OpenDental.UI.GridOD();
			this.gridClaimDetails = new OpenDental.UI.GridOD();
			this.groupBalancing.SuspendLayout();
			this.SuspendLayout();
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(467, 51);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(159, 20);
			this.label5.TabIndex = 136;
			this.label5.Text = "Trans Handling Desc";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTransHandlingDesc
			// 
			this.textTransHandlingDesc.Location = new System.Drawing.Point(626, 51);
			this.textTransHandlingDesc.Name = "textTransHandlingDesc";
			this.textTransHandlingDesc.ReadOnly = true;
			this.textTransHandlingDesc.Size = new System.Drawing.Size(339, 20);
			this.textTransHandlingDesc.TabIndex = 137;
			// 
			// textPaymentMethod
			// 
			this.textPaymentMethod.Location = new System.Drawing.Point(626, 71);
			this.textPaymentMethod.Name = "textPaymentMethod";
			this.textPaymentMethod.ReadOnly = true;
			this.textPaymentMethod.Size = new System.Drawing.Size(339, 20);
			this.textPaymentMethod.TabIndex = 139;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(467, 71);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(159, 20);
			this.label1.TabIndex = 138;
			this.label1.Text = "Payment Method";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPaymentAmount
			// 
			this.textPaymentAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textPaymentAmount.Location = new System.Drawing.Point(797, 630);
			this.textPaymentAmount.Name = "textPaymentAmount";
			this.textPaymentAmount.ReadOnly = true;
			this.textPaymentAmount.Size = new System.Drawing.Size(90, 20);
			this.textPaymentAmount.TabIndex = 141;
			this.textPaymentAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelPaymentAmount
			// 
			this.labelPaymentAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPaymentAmount.Location = new System.Drawing.Point(638, 630);
			this.labelPaymentAmount.Name = "labelPaymentAmount";
			this.labelPaymentAmount.Size = new System.Drawing.Size(159, 20);
			this.labelPaymentAmount.TabIndex = 140;
			this.labelPaymentAmount.Text = "Payment Amount";
			this.labelPaymentAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAcctNumEndingIn
			// 
			this.textAcctNumEndingIn.Location = new System.Drawing.Point(626, 90);
			this.textAcctNumEndingIn.Name = "textAcctNumEndingIn";
			this.textAcctNumEndingIn.ReadOnly = true;
			this.textAcctNumEndingIn.Size = new System.Drawing.Size(90, 20);
			this.textAcctNumEndingIn.TabIndex = 145;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(467, 90);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(159, 20);
			this.label4.TabIndex = 144;
			this.label4.Text = "Acct Num Ending In";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateEffective
			// 
			this.textDateEffective.Location = new System.Drawing.Point(626, 130);
			this.textDateEffective.Name = "textDateEffective";
			this.textDateEffective.ReadOnly = true;
			this.textDateEffective.Size = new System.Drawing.Size(90, 20);
			this.textDateEffective.TabIndex = 147;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(467, 130);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(159, 20);
			this.label6.TabIndex = 146;
			this.label6.Text = "Date Effective";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCheckNumOrRefNum
			// 
			this.textCheckNumOrRefNum.Location = new System.Drawing.Point(626, 110);
			this.textCheckNumOrRefNum.Name = "textCheckNumOrRefNum";
			this.textCheckNumOrRefNum.ReadOnly = true;
			this.textCheckNumOrRefNum.Size = new System.Drawing.Size(90, 20);
			this.textCheckNumOrRefNum.TabIndex = 149;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(467, 110);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(159, 20);
			this.label7.TabIndex = 148;
			this.label7.Text = "Check# or Reference#";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayerName
			// 
			this.textPayerName.Location = new System.Drawing.Point(140, 11);
			this.textPayerName.Name = "textPayerName";
			this.textPayerName.ReadOnly = true;
			this.textPayerName.Size = new System.Drawing.Size(325, 20);
			this.textPayerName.TabIndex = 151;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(2, 11);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(138, 20);
			this.label8.TabIndex = 150;
			this.label8.Text = "Payer Name";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayerID
			// 
			this.textPayerID.Location = new System.Drawing.Point(140, 31);
			this.textPayerID.Name = "textPayerID";
			this.textPayerID.ReadOnly = true;
			this.textPayerID.Size = new System.Drawing.Size(90, 20);
			this.textPayerID.TabIndex = 153;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(2, 31);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(138, 20);
			this.label9.TabIndex = 152;
			this.label9.Text = "Payer ID";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayerAddress1
			// 
			this.textPayerAddress1.Location = new System.Drawing.Point(140, 51);
			this.textPayerAddress1.Name = "textPayerAddress1";
			this.textPayerAddress1.ReadOnly = true;
			this.textPayerAddress1.Size = new System.Drawing.Size(325, 20);
			this.textPayerAddress1.TabIndex = 155;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(2, 51);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(138, 20);
			this.label10.TabIndex = 154;
			this.label10.Text = "Payer Address";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayerCity
			// 
			this.textPayerCity.Location = new System.Drawing.Point(140, 71);
			this.textPayerCity.Name = "textPayerCity";
			this.textPayerCity.ReadOnly = true;
			this.textPayerCity.Size = new System.Drawing.Size(325, 20);
			this.textPayerCity.TabIndex = 157;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(2, 71);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(138, 20);
			this.label11.TabIndex = 156;
			this.label11.Text = "Payer City";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayerState
			// 
			this.textPayerState.Location = new System.Drawing.Point(140, 91);
			this.textPayerState.Name = "textPayerState";
			this.textPayerState.ReadOnly = true;
			this.textPayerState.Size = new System.Drawing.Size(90, 20);
			this.textPayerState.TabIndex = 159;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(2, 91);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(138, 20);
			this.label12.TabIndex = 158;
			this.label12.Text = "Payer State";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayerZip
			// 
			this.textPayerZip.Location = new System.Drawing.Point(140, 111);
			this.textPayerZip.Name = "textPayerZip";
			this.textPayerZip.ReadOnly = true;
			this.textPayerZip.Size = new System.Drawing.Size(90, 20);
			this.textPayerZip.TabIndex = 161;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(2, 111);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(138, 20);
			this.label13.TabIndex = 160;
			this.label13.Text = "Payer Zip";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayerContactInfo
			// 
			this.textPayerContactInfo.Location = new System.Drawing.Point(140, 131);
			this.textPayerContactInfo.Name = "textPayerContactInfo";
			this.textPayerContactInfo.ReadOnly = true;
			this.textPayerContactInfo.Size = new System.Drawing.Size(325, 20);
			this.textPayerContactInfo.TabIndex = 163;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(2, 131);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(138, 20);
			this.label14.TabIndex = 162;
			this.label14.Text = "Payer Contact Info";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayeeName
			// 
			this.textPayeeName.Location = new System.Drawing.Point(626, 11);
			this.textPayeeName.Name = "textPayeeName";
			this.textPayeeName.ReadOnly = true;
			this.textPayeeName.Size = new System.Drawing.Size(339, 20);
			this.textPayeeName.TabIndex = 165;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(467, 11);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(159, 20);
			this.label15.TabIndex = 164;
			this.label15.Text = "Payee Name";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPayeeIdType
			// 
			this.labelPayeeIdType.Location = new System.Drawing.Point(467, 31);
			this.labelPayeeIdType.Name = "labelPayeeIdType";
			this.labelPayeeIdType.Size = new System.Drawing.Size(159, 20);
			this.labelPayeeIdType.TabIndex = 166;
			this.labelPayeeIdType.Text = "Payee Medicaid ID/NPI/TIN";
			this.labelPayeeIdType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayeeID
			// 
			this.textPayeeID.Location = new System.Drawing.Point(626, 31);
			this.textPayeeID.Name = "textPayeeID";
			this.textPayeeID.ReadOnly = true;
			this.textPayeeID.Size = new System.Drawing.Size(90, 20);
			this.textPayeeID.TabIndex = 169;
			// 
			// textClaimInsPaidSum
			// 
			this.textClaimInsPaidSum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textClaimInsPaidSum.Location = new System.Drawing.Point(6, 37);
			this.textClaimInsPaidSum.Name = "textClaimInsPaidSum";
			this.textClaimInsPaidSum.ReadOnly = true;
			this.textClaimInsPaidSum.Size = new System.Drawing.Size(110, 20);
			this.textClaimInsPaidSum.TabIndex = 176;
			// 
			// textProjAdjAmtSum
			// 
			this.textProjAdjAmtSum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textProjAdjAmtSum.Location = new System.Drawing.Point(137, 37);
			this.textProjAdjAmtSum.Name = "textProjAdjAmtSum";
			this.textProjAdjAmtSum.ReadOnly = true;
			this.textProjAdjAmtSum.Size = new System.Drawing.Size(110, 20);
			this.textProjAdjAmtSum.TabIndex = 178;
			// 
			// label18
			// 
			this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label18.Location = new System.Drawing.Point(118, 37);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(12, 20);
			this.label18.TabIndex = 180;
			this.label18.Text = "-";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label20
			// 
			this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label20.Location = new System.Drawing.Point(251, 37);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(12, 20);
			this.label20.TabIndex = 182;
			this.label20.Text = "=";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayAmountCalc
			// 
			this.textPayAmountCalc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textPayAmountCalc.Location = new System.Drawing.Point(265, 37);
			this.textPayAmountCalc.Name = "textPayAmountCalc";
			this.textPayAmountCalc.ReadOnly = true;
			this.textPayAmountCalc.Size = new System.Drawing.Size(110, 20);
			this.textPayAmountCalc.TabIndex = 184;
			// 
			// groupBalancing
			// 
			this.groupBalancing.Controls.Add(this.labelEquation3);
			this.groupBalancing.Controls.Add(this.labelEquation2);
			this.groupBalancing.Controls.Add(this.textPayAmountCalc);
			this.groupBalancing.Controls.Add(this.labelEquation1);
			this.groupBalancing.Controls.Add(this.textClaimInsPaidSum);
			this.groupBalancing.Controls.Add(this.label20);
			this.groupBalancing.Controls.Add(this.textProjAdjAmtSum);
			this.groupBalancing.Controls.Add(this.label18);
			this.groupBalancing.Location = new System.Drawing.Point(9, 176);
			this.groupBalancing.Name = "groupBalancing";
			this.groupBalancing.Size = new System.Drawing.Size(456, 65);
			this.groupBalancing.TabIndex = 211;
			this.groupBalancing.TabStop = false;
			this.groupBalancing.Text = "Balancing - Pay Amount Calc should exactly match Payment Amount";
			// 
			// labelEquation3
			// 
			this.labelEquation3.Location = new System.Drawing.Point(265, 16);
			this.labelEquation3.Name = "labelEquation3";
			this.labelEquation3.Size = new System.Drawing.Size(110, 20);
			this.labelEquation3.TabIndex = 187;
			this.labelEquation3.Text = "Pay Amount Calc";
			this.labelEquation3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelEquation2
			// 
			this.labelEquation2.Location = new System.Drawing.Point(137, 16);
			this.labelEquation2.Name = "labelEquation2";
			this.labelEquation2.Size = new System.Drawing.Size(110, 20);
			this.labelEquation2.TabIndex = 186;
			this.labelEquation2.Text = "Prov AdjAmt Sum";
			this.labelEquation2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelEquation1
			// 
			this.labelEquation1.Location = new System.Drawing.Point(6, 16);
			this.labelEquation1.Name = "labelEquation1";
			this.labelEquation1.Size = new System.Drawing.Size(110, 20);
			this.labelEquation1.TabIndex = 185;
			this.labelEquation1.Text = "Claim InsPaid Sum";
			this.labelEquation1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(626, 183);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(339, 58);
			this.textNote.TabIndex = 214;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(466, 183);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(159, 20);
			this.label2.TabIndex = 215;
			this.label2.Text = "Note";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(890, 663);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 216;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butClaimDetails
			// 
			this.butClaimDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butClaimDetails.Location = new System.Drawing.Point(9, 663);
			this.butClaimDetails.Name = "butClaimDetails";
			this.butClaimDetails.Size = new System.Drawing.Size(92, 26);
			this.butClaimDetails.TabIndex = 213;
			this.butClaimDetails.Text = "EOB Details";
			this.butClaimDetails.Click += new System.EventHandler(this.butClaimDetails_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(107, 663);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(79, 26);
			this.butPrint.TabIndex = 212;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butRawMessage
			// 
			this.butRawMessage.Location = new System.Drawing.Point(883, 152);
			this.butRawMessage.Name = "butRawMessage";
			this.butRawMessage.Size = new System.Drawing.Size(82, 25);
			this.butRawMessage.TabIndex = 116;
			this.butRawMessage.Text = "Raw Message";
			this.butRawMessage.Click += new System.EventHandler(this.butRawMessage_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(809, 663);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butBatch
			// 
			this.butBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butBatch.Icon = OpenDental.UI.EnumIcons.Add;
			this.butBatch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBatch.Location = new System.Drawing.Point(495, 663);
			this.butBatch.Name = "butBatch";
			this.butBatch.Size = new System.Drawing.Size(145, 24);
			this.butBatch.TabIndex = 217;
			this.butBatch.Text = "Finalize Payment";
			this.butBatch.Click += new System.EventHandler(this.butBatch_Click);
			// 
			// butDetachClaim
			// 
			this.butDetachClaim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDetachClaim.Location = new System.Drawing.Point(9, 630);
			this.butDetachClaim.Name = "butDetachClaim";
			this.butDetachClaim.Size = new System.Drawing.Size(90, 26);
			this.butDetachClaim.TabIndex = 220;
			this.butDetachClaim.Text = "Detach Claim";
			this.butDetachClaim.Click += new System.EventHandler(this.butDetachClaim_Click);
			// 
			// butPreview
			// 
			this.butPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPreview.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPreview.Location = new System.Drawing.Point(192, 663);
			this.butPreview.Name = "butPreview";
			this.butPreview.Size = new System.Drawing.Size(100, 26);
			this.butPreview.TabIndex = 221;
			this.butPreview.Text = "&Print Preview";
			this.butPreview.Click += new System.EventHandler(this.butPreview_Click);
			// 
			// buttonFindClaimMatch
			// 
			this.buttonFindClaimMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonFindClaimMatch.Location = new System.Drawing.Point(105, 630);
			this.buttonFindClaimMatch.Name = "buttonFindClaimMatch";
			this.buttonFindClaimMatch.Size = new System.Drawing.Size(181, 26);
			this.buttonFindClaimMatch.TabIndex = 222;
			this.buttonFindClaimMatch.Text = "Find Claim Matches For Detached";
			this.buttonFindClaimMatch.Click += new System.EventHandler(this.buttonFindClaimMatch_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(658, 662);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(82, 26);
			this.butDelete.TabIndex = 223;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butAutoProcess
			// 
			this.butAutoProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAutoProcess.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAutoProcess.Location = new System.Drawing.Point(334, 663);
			this.butAutoProcess.Name = "butAutoProcess";
			this.butAutoProcess.Size = new System.Drawing.Size(110, 24);
			this.butAutoProcess.TabIndex = 224;
			this.butAutoProcess.Text = "Auto Process";
			this.butAutoProcess.Visible = false;
			this.butAutoProcess.Click += new System.EventHandler(this.butAutoProcess_Click);
			// 
			// gridProviderAdjustments
			// 
			this.gridProviderAdjustments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProviderAdjustments.Location = new System.Drawing.Point(9, 247);
			this.gridProviderAdjustments.Name = "gridProviderAdjustments";
			this.gridProviderAdjustments.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProviderAdjustments.Size = new System.Drawing.Size(956, 95);
			this.gridProviderAdjustments.TabIndex = 170;
			this.gridProviderAdjustments.TabStop = false;
			this.gridProviderAdjustments.Title = "Provider Adjustments";
			this.gridProviderAdjustments.TranslationName = "FormEtrans835Edit";
			this.gridProviderAdjustments.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridProviderAdjustments_CellDoubleClick);
			// 
			// gridClaimDetails
			// 
			this.gridClaimDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridClaimDetails.HasMultilineHeaders = true;
			this.gridClaimDetails.Location = new System.Drawing.Point(9, 348);
			this.gridClaimDetails.Name = "gridClaimDetails";
			this.gridClaimDetails.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridClaimDetails.Size = new System.Drawing.Size(956, 277);
			this.gridClaimDetails.TabIndex = 0;
			this.gridClaimDetails.TabStop = false;
			this.gridClaimDetails.Title = "Claims Paid";
			this.gridClaimDetails.TranslationName = "FormEtrans835Edit";
			this.gridClaimDetails.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridClaimDetails_CellDoubleClick);
			// 
			// FormEtrans835Edit
			// 
			this.ClientSize = new System.Drawing.Size(974, 695);
			this.Controls.Add(this.butAutoProcess);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.buttonFindClaimMatch);
			this.Controls.Add(this.butPreview);
			this.Controls.Add(this.butDetachClaim);
			this.Controls.Add(this.butBatch);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butClaimDetails);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.groupBalancing);
			this.Controls.Add(this.gridProviderAdjustments);
			this.Controls.Add(this.textPayeeID);
			this.Controls.Add(this.labelPayeeIdType);
			this.Controls.Add(this.textPayeeName);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.textPayerContactInfo);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.textPayerZip);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.textPayerState);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.textPayerCity);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.textPayerAddress1);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textPayerID);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textPayerName);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textCheckNumOrRefNum);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textDateEffective);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textAcctNumEndingIn);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textPaymentAmount);
			this.Controls.Add(this.labelPaymentAmount);
			this.Controls.Add(this.textPaymentMethod);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textTransHandlingDesc);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butRawMessage);
			this.Controls.Add(this.gridClaimDetails);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEtrans835Edit";
			this.Text = "Electronic Remittance Advice (ERA) - Electronic EOB - Format X12 835";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEtrans835Edit_FormClosing);
			this.Load += new System.EventHandler(this.FormEtrans835Edit_Load);
			this.Shown += new System.EventHandler(this.FormEtrans835Edit_Shown);
			this.Resize += new System.EventHandler(this.FormEtrans835Edit_Resize);
			this.groupBalancing.ResumeLayout(false);
			this.groupBalancing.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.GridOD gridClaimDetails;
		private UI.Button butRawMessage;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textTransHandlingDesc;
		private System.Windows.Forms.TextBox textPaymentMethod;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textPaymentAmount;
		private System.Windows.Forms.Label labelPaymentAmount;
		private System.Windows.Forms.TextBox textAcctNumEndingIn;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textDateEffective;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textCheckNumOrRefNum;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textPayerName;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textPayerID;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textPayerAddress1;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textPayerCity;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textPayerState;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textPayerZip;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textPayerContactInfo;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textPayeeName;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label labelPayeeIdType;
		private System.Windows.Forms.TextBox textPayeeID;
		private UI.GridOD gridProviderAdjustments;
		private System.Windows.Forms.TextBox textClaimInsPaidSum;
		private System.Windows.Forms.TextBox textProjAdjAmtSum;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox textPayAmountCalc;
		private System.Windows.Forms.GroupBox groupBalancing;
		private UI.Button butPrint;
		private UI.Button butClaimDetails;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label2;
		private UI.Button butCancel;
		private UI.Button butBatch;
		private UI.Button butDetachClaim;
		private UI.Button butPreview;
		private UI.Button buttonFindClaimMatch;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelEquation3;
		private System.Windows.Forms.Label labelEquation2;
		private System.Windows.Forms.Label labelEquation1;
		private UI.Button butAutoProcess;
	}
}