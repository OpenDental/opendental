namespace OpenDental{
	partial class FormRpUnearnedIncome {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpUnearnedIncome));
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabPageUnallocatedBalances = new System.Windows.Forms.TabPage();
			this.checkUnearnedAllocationShowProv = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.checkUnearnedAllocationExcludeZero = new System.Windows.Forms.CheckBox();
			this.butUnallocatedCancel = new OpenDental.UI.Button();
			this.butUnearnedAllocationOK = new OpenDental.UI.Button();
			this.checkUnearnedAllocationAllTypes = new System.Windows.Forms.CheckBox();
			this.listUnearnedAllocationTypes = new OpenDental.UI.ListBoxOD();
			this.label10 = new System.Windows.Forms.Label();
			this.checkUnearnedAllocationAllProvs = new System.Windows.Forms.CheckBox();
			this.listUnearnedAllocationProvs = new OpenDental.UI.ListBoxOD();
			this.label11 = new System.Windows.Forms.Label();
			this.checkUnearnedAllocationAllClins = new System.Windows.Forms.CheckBox();
			this.listUnearnedAllocationClins = new OpenDental.UI.ListBoxOD();
			this.labelUnearnedAllocationClins = new System.Windows.Forms.Label();
			this.tabPageNetUnearned = new System.Windows.Forms.TabPage();
			this.checkNetUnearnedExcludeZero = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butNetUnearnedCancel = new OpenDental.UI.Button();
			this.butNetUnearnedOK = new OpenDental.UI.Button();
			this.checkNetUnearnedAllTypes = new System.Windows.Forms.CheckBox();
			this.listNetUnearnedTypes = new OpenDental.UI.ListBoxOD();
			this.label9 = new System.Windows.Forms.Label();
			this.checkNetUnearnedAllProvs = new System.Windows.Forms.CheckBox();
			this.listNetUnearnedProvs = new OpenDental.UI.ListBoxOD();
			this.label8 = new System.Windows.Forms.Label();
			this.checkNetUnearnedAllClins = new System.Windows.Forms.CheckBox();
			this.listNetUnearnedClins = new OpenDental.UI.ListBoxOD();
			this.labelNetUnearnedClins = new System.Windows.Forms.Label();
			this.tabPageLineItemUnearned = new System.Windows.Forms.TabPage();
			this.checkLineItemShowProv = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.butLineItemCancel = new OpenDental.UI.Button();
			this.butLineItemOK = new OpenDental.UI.Button();
			this.label17 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.dateLineItemTo = new System.Windows.Forms.MonthCalendar();
			this.dateLineItemFrom = new System.Windows.Forms.MonthCalendar();
			this.checkLineItemAllClins = new System.Windows.Forms.CheckBox();
			this.listLineItemClins = new OpenDental.UI.ListBoxOD();
			this.labelLineItemClins = new System.Windows.Forms.Label();
			this.tabPageUnearnedAccounts = new System.Windows.Forms.TabPage();
			this.butUnearnedAcctCancel = new OpenDental.UI.Button();
			this.butUnearnedAcctOk = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.checkUnearnedAcctAllClins = new System.Windows.Forms.CheckBox();
			this.listUnearnedAcctClins = new OpenDental.UI.ListBoxOD();
			this.labelUnearnedAcctClins = new System.Windows.Forms.Label();
			this.tabControlMain.SuspendLayout();
			this.tabPageUnallocatedBalances.SuspendLayout();
			this.tabPageNetUnearned.SuspendLayout();
			this.tabPageLineItemUnearned.SuspendLayout();
			this.tabPageUnearnedAccounts.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPageUnearnedAccounts);
			this.tabControlMain.Controls.Add(this.tabPageUnallocatedBalances);
			this.tabControlMain.Controls.Add(this.tabPageNetUnearned);
			this.tabControlMain.Controls.Add(this.tabPageLineItemUnearned);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.Location = new System.Drawing.Point(0, 0);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(623, 519);
			this.tabControlMain.TabIndex = 61;
			// 
			// tabPageUnallocatedBalances
			// 
			this.tabPageUnallocatedBalances.Controls.Add(this.checkUnearnedAllocationShowProv);
			this.tabPageUnallocatedBalances.Controls.Add(this.label4);
			this.tabPageUnallocatedBalances.Controls.Add(this.checkUnearnedAllocationExcludeZero);
			this.tabPageUnallocatedBalances.Controls.Add(this.butUnallocatedCancel);
			this.tabPageUnallocatedBalances.Controls.Add(this.butUnearnedAllocationOK);
			this.tabPageUnallocatedBalances.Controls.Add(this.checkUnearnedAllocationAllTypes);
			this.tabPageUnallocatedBalances.Controls.Add(this.listUnearnedAllocationTypes);
			this.tabPageUnallocatedBalances.Controls.Add(this.label10);
			this.tabPageUnallocatedBalances.Controls.Add(this.checkUnearnedAllocationAllProvs);
			this.tabPageUnallocatedBalances.Controls.Add(this.listUnearnedAllocationProvs);
			this.tabPageUnallocatedBalances.Controls.Add(this.label11);
			this.tabPageUnallocatedBalances.Controls.Add(this.checkUnearnedAllocationAllClins);
			this.tabPageUnallocatedBalances.Controls.Add(this.listUnearnedAllocationClins);
			this.tabPageUnallocatedBalances.Controls.Add(this.labelUnearnedAllocationClins);
			this.tabPageUnallocatedBalances.Location = new System.Drawing.Point(4, 22);
			this.tabPageUnallocatedBalances.Name = "tabPageUnallocatedBalances";
			this.tabPageUnallocatedBalances.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageUnallocatedBalances.Size = new System.Drawing.Size(615, 493);
			this.tabPageUnallocatedBalances.TabIndex = 1;
			this.tabPageUnallocatedBalances.Text = "Unearned Allocation";
			// 
			// checkUnearnedAllocationShowProv
			// 
			this.checkUnearnedAllocationShowProv.Location = new System.Drawing.Point(21, 68);
			this.checkUnearnedAllocationShowProv.Name = "checkUnearnedAllocationShowProv";
			this.checkUnearnedAllocationShowProv.Size = new System.Drawing.Size(182, 16);
			this.checkUnearnedAllocationShowProv.TabIndex = 77;
			this.checkUnearnedAllocationShowProv.Text = "Show provider column";
			this.checkUnearnedAllocationShowProv.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.Transparent;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(18, 11);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(571, 53);
			this.label4.TabIndex = 1;
			this.label4.Text = resources.GetString("label4.Text");
			// 
			// checkUnearnedAllocationExcludeZero
			// 
			this.checkUnearnedAllocationExcludeZero.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUnearnedAllocationExcludeZero.Location = new System.Drawing.Point(21, 84);
			this.checkUnearnedAllocationExcludeZero.Name = "checkUnearnedAllocationExcludeZero";
			this.checkUnearnedAllocationExcludeZero.Size = new System.Drawing.Size(474, 16);
			this.checkUnearnedAllocationExcludeZero.TabIndex = 76;
			this.checkUnearnedAllocationExcludeZero.Text = "Exclude families with a net $0.00 unearned income balance";
			// 
			// butUnallocatedCancel
			// 
			this.butUnallocatedCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butUnallocatedCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butUnallocatedCancel.Location = new System.Drawing.Point(528, 456);
			this.butUnallocatedCancel.Name = "butUnallocatedCancel";
			this.butUnallocatedCancel.Size = new System.Drawing.Size(75, 26);
			this.butUnallocatedCancel.TabIndex = 75;
			this.butUnallocatedCancel.Text = "&Cancel";
			this.butUnallocatedCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butUnearnedAllocationOK
			// 
			this.butUnearnedAllocationOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butUnearnedAllocationOK.Location = new System.Drawing.Point(528, 424);
			this.butUnearnedAllocationOK.Name = "butUnearnedAllocationOK";
			this.butUnearnedAllocationOK.Size = new System.Drawing.Size(75, 26);
			this.butUnearnedAllocationOK.TabIndex = 74;
			this.butUnearnedAllocationOK.Text = "&OK";
			this.butUnearnedAllocationOK.Click += new System.EventHandler(this.butUnearnedAllocationOK_Click);
			// 
			// checkUnearnedAllocationAllTypes
			// 
			this.checkUnearnedAllocationAllTypes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUnearnedAllocationAllTypes.Location = new System.Drawing.Point(21, 132);
			this.checkUnearnedAllocationAllTypes.Name = "checkUnearnedAllocationAllTypes";
			this.checkUnearnedAllocationAllTypes.Size = new System.Drawing.Size(95, 16);
			this.checkUnearnedAllocationAllTypes.TabIndex = 72;
			this.checkUnearnedAllocationAllTypes.Text = "All";
			this.checkUnearnedAllocationAllTypes.CheckedChanged += new System.EventHandler(this.checkUnearnedAllocationAllTypes_CheckedChanged);
			// 
			// listUnearnedAllocationTypes
			// 
			this.listUnearnedAllocationTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listUnearnedAllocationTypes.IntegralHeight = false;
			this.listUnearnedAllocationTypes.Location = new System.Drawing.Point(21, 149);
			this.listUnearnedAllocationTypes.Name = "listUnearnedAllocationTypes";
			this.listUnearnedAllocationTypes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listUnearnedAllocationTypes.Size = new System.Drawing.Size(154, 333);
			this.listUnearnedAllocationTypes.TabIndex = 71;
			this.listUnearnedAllocationTypes.Click += new System.EventHandler(this.listUnearnedAllocationTypes_Click);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(18, 114);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(104, 16);
			this.label10.TabIndex = 70;
			this.label10.Text = "Unearned Types";
			this.label10.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkUnearnedAllocationAllProvs
			// 
			this.checkUnearnedAllocationAllProvs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUnearnedAllocationAllProvs.Location = new System.Drawing.Point(181, 132);
			this.checkUnearnedAllocationAllProvs.Name = "checkUnearnedAllocationAllProvs";
			this.checkUnearnedAllocationAllProvs.Size = new System.Drawing.Size(95, 16);
			this.checkUnearnedAllocationAllProvs.TabIndex = 69;
			this.checkUnearnedAllocationAllProvs.Text = "All";
			this.checkUnearnedAllocationAllProvs.CheckedChanged += new System.EventHandler(this.checkUnearnedAllocationAllProvs_CheckedChanged);
			// 
			// listUnearnedAllocationProvs
			// 
			this.listUnearnedAllocationProvs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listUnearnedAllocationProvs.IntegralHeight = false;
			this.listUnearnedAllocationProvs.Location = new System.Drawing.Point(181, 149);
			this.listUnearnedAllocationProvs.Name = "listUnearnedAllocationProvs";
			this.listUnearnedAllocationProvs.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listUnearnedAllocationProvs.Size = new System.Drawing.Size(154, 333);
			this.listUnearnedAllocationProvs.TabIndex = 68;
			this.listUnearnedAllocationProvs.Click += new System.EventHandler(this.listUnearnedAllocationProvs_Click);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(178, 114);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(104, 16);
			this.label11.TabIndex = 67;
			this.label11.Text = "Providers";
			this.label11.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkUnearnedAllocationAllClins
			// 
			this.checkUnearnedAllocationAllClins.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUnearnedAllocationAllClins.Location = new System.Drawing.Point(341, 132);
			this.checkUnearnedAllocationAllClins.Name = "checkUnearnedAllocationAllClins";
			this.checkUnearnedAllocationAllClins.Size = new System.Drawing.Size(154, 16);
			this.checkUnearnedAllocationAllClins.TabIndex = 66;
			this.checkUnearnedAllocationAllClins.Text = "All (Includes hidden)";
			this.checkUnearnedAllocationAllClins.CheckedChanged += new System.EventHandler(this.checkUnearnedAllocationAllClins_CheckedChanged);
			// 
			// listUnearnedAllocationClins
			// 
			this.listUnearnedAllocationClins.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listUnearnedAllocationClins.IntegralHeight = false;
			this.listUnearnedAllocationClins.Location = new System.Drawing.Point(341, 149);
			this.listUnearnedAllocationClins.Name = "listUnearnedAllocationClins";
			this.listUnearnedAllocationClins.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listUnearnedAllocationClins.Size = new System.Drawing.Size(154, 333);
			this.listUnearnedAllocationClins.TabIndex = 65;
			this.listUnearnedAllocationClins.Click += new System.EventHandler(this.listUnearnedAllocationClins_Click);
			// 
			// labelUnearnedAllocationClins
			// 
			this.labelUnearnedAllocationClins.Location = new System.Drawing.Point(338, 114);
			this.labelUnearnedAllocationClins.Name = "labelUnearnedAllocationClins";
			this.labelUnearnedAllocationClins.Size = new System.Drawing.Size(104, 16);
			this.labelUnearnedAllocationClins.TabIndex = 64;
			this.labelUnearnedAllocationClins.Text = "Clinics";
			this.labelUnearnedAllocationClins.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// tabPageNetUnearned
			// 
			this.tabPageNetUnearned.Controls.Add(this.checkNetUnearnedExcludeZero);
			this.tabPageNetUnearned.Controls.Add(this.label5);
			this.tabPageNetUnearned.Controls.Add(this.butNetUnearnedCancel);
			this.tabPageNetUnearned.Controls.Add(this.butNetUnearnedOK);
			this.tabPageNetUnearned.Controls.Add(this.checkNetUnearnedAllTypes);
			this.tabPageNetUnearned.Controls.Add(this.listNetUnearnedTypes);
			this.tabPageNetUnearned.Controls.Add(this.label9);
			this.tabPageNetUnearned.Controls.Add(this.checkNetUnearnedAllProvs);
			this.tabPageNetUnearned.Controls.Add(this.listNetUnearnedProvs);
			this.tabPageNetUnearned.Controls.Add(this.label8);
			this.tabPageNetUnearned.Controls.Add(this.checkNetUnearnedAllClins);
			this.tabPageNetUnearned.Controls.Add(this.listNetUnearnedClins);
			this.tabPageNetUnearned.Controls.Add(this.labelNetUnearnedClins);
			this.tabPageNetUnearned.Location = new System.Drawing.Point(4, 22);
			this.tabPageNetUnearned.Name = "tabPageNetUnearned";
			this.tabPageNetUnearned.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageNetUnearned.Size = new System.Drawing.Size(615, 493);
			this.tabPageNetUnearned.TabIndex = 2;
			this.tabPageNetUnearned.Text = "Net Unearned Income";
			// 
			// checkNetUnearnedExcludeZero
			// 
			this.checkNetUnearnedExcludeZero.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNetUnearnedExcludeZero.Location = new System.Drawing.Point(21, 74);
			this.checkNetUnearnedExcludeZero.Name = "checkNetUnearnedExcludeZero";
			this.checkNetUnearnedExcludeZero.Size = new System.Drawing.Size(474, 16);
			this.checkNetUnearnedExcludeZero.TabIndex = 78;
			this.checkNetUnearnedExcludeZero.Text = "Exclude families with a net $0.00 unearned income balance";
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.Transparent;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(18, 11);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(562, 53);
			this.label5.TabIndex = 2;
			this.label5.Text = resources.GetString("label5.Text");
			// 
			// butNetUnearnedCancel
			// 
			this.butNetUnearnedCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNetUnearnedCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butNetUnearnedCancel.Location = new System.Drawing.Point(528, 456);
			this.butNetUnearnedCancel.Name = "butNetUnearnedCancel";
			this.butNetUnearnedCancel.Size = new System.Drawing.Size(75, 26);
			this.butNetUnearnedCancel.TabIndex = 77;
			this.butNetUnearnedCancel.Text = "&Cancel";
			this.butNetUnearnedCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butNetUnearnedOK
			// 
			this.butNetUnearnedOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNetUnearnedOK.Location = new System.Drawing.Point(528, 424);
			this.butNetUnearnedOK.Name = "butNetUnearnedOK";
			this.butNetUnearnedOK.Size = new System.Drawing.Size(75, 26);
			this.butNetUnearnedOK.TabIndex = 76;
			this.butNetUnearnedOK.Text = "&OK";
			this.butNetUnearnedOK.Click += new System.EventHandler(this.butNetUnearnedOK_Click);
			// 
			// checkNetUnearnedAllTypes
			// 
			this.checkNetUnearnedAllTypes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNetUnearnedAllTypes.Location = new System.Drawing.Point(21, 114);
			this.checkNetUnearnedAllTypes.Name = "checkNetUnearnedAllTypes";
			this.checkNetUnearnedAllTypes.Size = new System.Drawing.Size(95, 16);
			this.checkNetUnearnedAllTypes.TabIndex = 63;
			this.checkNetUnearnedAllTypes.Text = "All";
			this.checkNetUnearnedAllTypes.CheckedChanged += new System.EventHandler(this.checkNetUnearnedAllTypes_CheckedChanged);
			// 
			// listNetUnearnedTypes
			// 
			this.listNetUnearnedTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listNetUnearnedTypes.IntegralHeight = false;
			this.listNetUnearnedTypes.Location = new System.Drawing.Point(21, 131);
			this.listNetUnearnedTypes.Name = "listNetUnearnedTypes";
			this.listNetUnearnedTypes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listNetUnearnedTypes.Size = new System.Drawing.Size(154, 351);
			this.listNetUnearnedTypes.TabIndex = 62;
			this.listNetUnearnedTypes.Click += new System.EventHandler(this.listNetUnearnedTypes_Click);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(18, 96);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(104, 16);
			this.label9.TabIndex = 61;
			this.label9.Text = "Unearned Types";
			this.label9.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkNetUnearnedAllProvs
			// 
			this.checkNetUnearnedAllProvs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNetUnearnedAllProvs.Location = new System.Drawing.Point(181, 114);
			this.checkNetUnearnedAllProvs.Name = "checkNetUnearnedAllProvs";
			this.checkNetUnearnedAllProvs.Size = new System.Drawing.Size(95, 16);
			this.checkNetUnearnedAllProvs.TabIndex = 60;
			this.checkNetUnearnedAllProvs.Text = "All";
			this.checkNetUnearnedAllProvs.CheckedChanged += new System.EventHandler(this.checkNetUnearnedAllProvs_CheckedChanged);
			// 
			// listNetUnearnedProvs
			// 
			this.listNetUnearnedProvs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listNetUnearnedProvs.IntegralHeight = false;
			this.listNetUnearnedProvs.Location = new System.Drawing.Point(181, 131);
			this.listNetUnearnedProvs.Name = "listNetUnearnedProvs";
			this.listNetUnearnedProvs.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listNetUnearnedProvs.Size = new System.Drawing.Size(154, 351);
			this.listNetUnearnedProvs.TabIndex = 59;
			this.listNetUnearnedProvs.Click += new System.EventHandler(this.listNetUnearnedProvs_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(178, 96);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(104, 16);
			this.label8.TabIndex = 58;
			this.label8.Text = "Providers";
			this.label8.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkNetUnearnedAllClins
			// 
			this.checkNetUnearnedAllClins.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNetUnearnedAllClins.Location = new System.Drawing.Point(341, 114);
			this.checkNetUnearnedAllClins.Name = "checkNetUnearnedAllClins";
			this.checkNetUnearnedAllClins.Size = new System.Drawing.Size(154, 16);
			this.checkNetUnearnedAllClins.TabIndex = 57;
			this.checkNetUnearnedAllClins.Text = "All (Includes hidden)";
			this.checkNetUnearnedAllClins.CheckedChanged += new System.EventHandler(this.checkNetUnearnedAllClins_CheckedChanged);
			// 
			// listNetUnearnedClins
			// 
			this.listNetUnearnedClins.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listNetUnearnedClins.IntegralHeight = false;
			this.listNetUnearnedClins.Location = new System.Drawing.Point(341, 131);
			this.listNetUnearnedClins.Name = "listNetUnearnedClins";
			this.listNetUnearnedClins.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listNetUnearnedClins.Size = new System.Drawing.Size(154, 351);
			this.listNetUnearnedClins.TabIndex = 56;
			this.listNetUnearnedClins.Click += new System.EventHandler(this.listNetUnearnedClins_Click);
			// 
			// labelNetUnearnedClins
			// 
			this.labelNetUnearnedClins.Location = new System.Drawing.Point(338, 96);
			this.labelNetUnearnedClins.Name = "labelNetUnearnedClins";
			this.labelNetUnearnedClins.Size = new System.Drawing.Size(104, 16);
			this.labelNetUnearnedClins.TabIndex = 55;
			this.labelNetUnearnedClins.Text = "Clinics";
			this.labelNetUnearnedClins.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// tabPageLineItemUnearned
			// 
			this.tabPageLineItemUnearned.Controls.Add(this.checkLineItemShowProv);
			this.tabPageLineItemUnearned.Controls.Add(this.label7);
			this.tabPageLineItemUnearned.Controls.Add(this.butLineItemCancel);
			this.tabPageLineItemUnearned.Controls.Add(this.butLineItemOK);
			this.tabPageLineItemUnearned.Controls.Add(this.label17);
			this.tabPageLineItemUnearned.Controls.Add(this.label16);
			this.tabPageLineItemUnearned.Controls.Add(this.dateLineItemTo);
			this.tabPageLineItemUnearned.Controls.Add(this.dateLineItemFrom);
			this.tabPageLineItemUnearned.Controls.Add(this.checkLineItemAllClins);
			this.tabPageLineItemUnearned.Controls.Add(this.listLineItemClins);
			this.tabPageLineItemUnearned.Controls.Add(this.labelLineItemClins);
			this.tabPageLineItemUnearned.Location = new System.Drawing.Point(4, 22);
			this.tabPageLineItemUnearned.Name = "tabPageLineItemUnearned";
			this.tabPageLineItemUnearned.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageLineItemUnearned.Size = new System.Drawing.Size(615, 493);
			this.tabPageLineItemUnearned.TabIndex = 0;
			this.tabPageLineItemUnearned.Text = "Line Item Unearned Income";
			// 
			// checkLineItemShowProv
			// 
			this.checkLineItemShowProv.Location = new System.Drawing.Point(255, 259);
			this.checkLineItemShowProv.Name = "checkLineItemShowProv";
			this.checkLineItemShowProv.Size = new System.Drawing.Size(183, 17);
			this.checkLineItemShowProv.TabIndex = 80;
			this.checkLineItemShowProv.Text = "Show provider column";
			this.checkLineItemShowProv.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.BackColor = System.Drawing.Color.Transparent;
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(18, 11);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(525, 39);
			this.label7.TabIndex = 79;
			this.label7.Text = "Shows all unearned income transactions for the specified date range.\r\nUseful when" +
    " you want to see a line-by-line representation of transactions relating to unear" +
    "ned income.";
			// 
			// butLineItemCancel
			// 
			this.butLineItemCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butLineItemCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butLineItemCancel.Location = new System.Drawing.Point(528, 456);
			this.butLineItemCancel.Name = "butLineItemCancel";
			this.butLineItemCancel.Size = new System.Drawing.Size(75, 26);
			this.butLineItemCancel.TabIndex = 78;
			this.butLineItemCancel.Text = "&Cancel";
			this.butLineItemCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butLineItemOK
			// 
			this.butLineItemOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butLineItemOK.Location = new System.Drawing.Point(528, 424);
			this.butLineItemOK.Name = "butLineItemOK";
			this.butLineItemOK.Size = new System.Drawing.Size(75, 26);
			this.butLineItemOK.TabIndex = 77;
			this.butLineItemOK.Text = "&OK";
			this.butLineItemOK.Click += new System.EventHandler(this.butLineItemOK_Click);
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(252, 61);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(104, 16);
			this.label17.TabIndex = 76;
			this.label17.Text = "To:";
			this.label17.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(18, 59);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(104, 16);
			this.label16.TabIndex = 75;
			this.label16.Text = "From:";
			this.label16.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// dateLineItemTo
			// 
			this.dateLineItemTo.Location = new System.Drawing.Point(255, 77);
			this.dateLineItemTo.MaxSelectionCount = 1;
			this.dateLineItemTo.Name = "dateLineItemTo";
			this.dateLineItemTo.TabIndex = 4;
			// 
			// dateLineItemFrom
			// 
			this.dateLineItemFrom.Location = new System.Drawing.Point(21, 77);
			this.dateLineItemFrom.MaxSelectionCount = 1;
			this.dateLineItemFrom.Name = "dateLineItemFrom";
			this.dateLineItemFrom.TabIndex = 3;
			// 
			// checkLineItemAllClins
			// 
			this.checkLineItemAllClins.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkLineItemAllClins.Location = new System.Drawing.Point(21, 259);
			this.checkLineItemAllClins.Name = "checkLineItemAllClins";
			this.checkLineItemAllClins.Size = new System.Drawing.Size(154, 16);
			this.checkLineItemAllClins.TabIndex = 54;
			this.checkLineItemAllClins.Text = "All (Includes hidden)";
			this.checkLineItemAllClins.CheckedChanged += new System.EventHandler(this.checkLineItemAllClins_CheckedChanged);
			// 
			// listLineItemClins
			// 
			this.listLineItemClins.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listLineItemClins.IntegralHeight = false;
			this.listLineItemClins.Location = new System.Drawing.Point(21, 278);
			this.listLineItemClins.Name = "listLineItemClins";
			this.listLineItemClins.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listLineItemClins.Size = new System.Drawing.Size(154, 204);
			this.listLineItemClins.TabIndex = 53;
			this.listLineItemClins.Click += new System.EventHandler(this.listLineItemClins_Click);
			// 
			// labelLineItemClins
			// 
			this.labelLineItemClins.Location = new System.Drawing.Point(18, 241);
			this.labelLineItemClins.Name = "labelLineItemClins";
			this.labelLineItemClins.Size = new System.Drawing.Size(104, 16);
			this.labelLineItemClins.TabIndex = 52;
			this.labelLineItemClins.Text = "Clinics";
			this.labelLineItemClins.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// tabPageUnearnedAccounts
			// 
			this.tabPageUnearnedAccounts.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageUnearnedAccounts.Controls.Add(this.butUnearnedAcctCancel);
			this.tabPageUnearnedAccounts.Controls.Add(this.butUnearnedAcctOk);
			this.tabPageUnearnedAccounts.Controls.Add(this.label1);
			this.tabPageUnearnedAccounts.Controls.Add(this.checkUnearnedAcctAllClins);
			this.tabPageUnearnedAccounts.Controls.Add(this.listUnearnedAcctClins);
			this.tabPageUnearnedAccounts.Controls.Add(this.labelUnearnedAcctClins);
			this.tabPageUnearnedAccounts.Location = new System.Drawing.Point(4, 22);
			this.tabPageUnearnedAccounts.Name = "tabPageUnearnedAccounts";
			this.tabPageUnearnedAccounts.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageUnearnedAccounts.Size = new System.Drawing.Size(615, 493);
			this.tabPageUnearnedAccounts.TabIndex = 3;
			this.tabPageUnearnedAccounts.Text = "Unearned Accounts";
			// 
			// butUnearnedAcctCancel
			// 
			this.butUnearnedAcctCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butUnearnedAcctCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butUnearnedAcctCancel.Location = new System.Drawing.Point(528, 456);
			this.butUnearnedAcctCancel.Name = "butUnearnedAcctCancel";
			this.butUnearnedAcctCancel.Size = new System.Drawing.Size(75, 26);
			this.butUnearnedAcctCancel.TabIndex = 85;
			this.butUnearnedAcctCancel.Text = "&Cancel";
			this.butUnearnedAcctCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butUnearnedAcctOk
			// 
			this.butUnearnedAcctOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butUnearnedAcctOk.Location = new System.Drawing.Point(528, 424);
			this.butUnearnedAcctOk.Name = "butUnearnedAcctOk";
			this.butUnearnedAcctOk.Size = new System.Drawing.Size(75, 26);
			this.butUnearnedAcctOk.TabIndex = 84;
			this.butUnearnedAcctOk.Text = "&OK";
			this.butUnearnedAcctOk.Click += new System.EventHandler(this.butUnearnedAcctOk_Click);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(18, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(585, 72);
			this.label1.TabIndex = 83;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// checkUnearnedAcctAllClins
			// 
			this.checkUnearnedAcctAllClins.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUnearnedAcctAllClins.Location = new System.Drawing.Point(21, 114);
			this.checkUnearnedAcctAllClins.Name = "checkUnearnedAcctAllClins";
			this.checkUnearnedAcctAllClins.Size = new System.Drawing.Size(154, 16);
			this.checkUnearnedAcctAllClins.TabIndex = 82;
			this.checkUnearnedAcctAllClins.Text = "All (Includes hidden)";
			this.checkUnearnedAcctAllClins.CheckedChanged += new System.EventHandler(this.checkUnearnedAcctAllClins_CheckedChanged);
			// 
			// listUnearnedAcctClins
			// 
			this.listUnearnedAcctClins.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listUnearnedAcctClins.IntegralHeight = false;
			this.listUnearnedAcctClins.Location = new System.Drawing.Point(21, 131);
			this.listUnearnedAcctClins.Name = "listUnearnedAcctClins";
			this.listUnearnedAcctClins.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listUnearnedAcctClins.Size = new System.Drawing.Size(154, 351);
			this.listUnearnedAcctClins.TabIndex = 81;
			this.listUnearnedAcctClins.Click += new System.EventHandler(this.listUnearnedAcctClins_Click);
			// 
			// labelUnearnedAcctClins
			// 
			this.labelUnearnedAcctClins.Location = new System.Drawing.Point(18, 96);
			this.labelUnearnedAcctClins.Name = "labelUnearnedAcctClins";
			this.labelUnearnedAcctClins.Size = new System.Drawing.Size(104, 16);
			this.labelUnearnedAcctClins.TabIndex = 80;
			this.labelUnearnedAcctClins.Text = "Clinics";
			this.labelUnearnedAcctClins.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormRpUnearnedIncome
			// 
			this.ClientSize = new System.Drawing.Size(623, 519);
			this.Controls.Add(this.tabControlMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpUnearnedIncome";
			this.Text = "Unearned Income Reports";
			this.Load += new System.EventHandler(this.FormRpUnearnedIncome_Load);
			this.tabControlMain.ResumeLayout(false);
			this.tabPageUnallocatedBalances.ResumeLayout(false);
			this.tabPageNetUnearned.ResumeLayout(false);
			this.tabPageLineItemUnearned.ResumeLayout(false);
			this.tabPageUnearnedAccounts.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.TabPage tabPageUnallocatedBalances;
		private System.Windows.Forms.CheckBox checkUnearnedAllocationAllTypes;
		private OpenDental.UI.ListBoxOD listUnearnedAllocationTypes;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox checkUnearnedAllocationAllProvs;
		private OpenDental.UI.ListBoxOD listUnearnedAllocationProvs;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.CheckBox checkUnearnedAllocationAllClins;
		private OpenDental.UI.ListBoxOD listUnearnedAllocationClins;
		private System.Windows.Forms.Label labelUnearnedAllocationClins;
		private System.Windows.Forms.TabPage tabPageNetUnearned;
		private System.Windows.Forms.CheckBox checkNetUnearnedAllTypes;
		private OpenDental.UI.ListBoxOD listNetUnearnedTypes;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.CheckBox checkNetUnearnedAllProvs;
		private OpenDental.UI.ListBoxOD listNetUnearnedProvs;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox checkNetUnearnedAllClins;
		private OpenDental.UI.ListBoxOD listNetUnearnedClins;
		private System.Windows.Forms.Label labelNetUnearnedClins;
		private System.Windows.Forms.TabPage tabPageLineItemUnearned;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.MonthCalendar dateLineItemTo;
		private System.Windows.Forms.MonthCalendar dateLineItemFrom;
		private System.Windows.Forms.CheckBox checkLineItemAllClins;
		private OpenDental.UI.ListBoxOD listLineItemClins;
		private System.Windows.Forms.Label labelLineItemClins;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private UI.Button butUnallocatedCancel;
		private UI.Button butUnearnedAllocationOK;
		private UI.Button butNetUnearnedCancel;
		private UI.Button butNetUnearnedOK;
		private UI.Button butLineItemCancel;
		private UI.Button butLineItemOK;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox checkUnearnedAllocationExcludeZero;
		private System.Windows.Forms.CheckBox checkNetUnearnedExcludeZero;
		private System.Windows.Forms.TabPage tabPageUnearnedAccounts;
		private UI.Button butUnearnedAcctCancel;
		private UI.Button butUnearnedAcctOk;
		private System.Windows.Forms.Label labelUnearnedAcctClins;
		private OpenDental.UI.ListBoxOD listUnearnedAcctClins;
		private System.Windows.Forms.CheckBox checkUnearnedAcctAllClins;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkUnearnedAllocationShowProv;
		private System.Windows.Forms.CheckBox checkLineItemShowProv;
	}
}