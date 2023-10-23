
namespace UnitTests{
	partial class FormEscalationViewTests {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEscalationViewTests));
			this.escalationView = new OpenDental.EscalationViewControl();
			this.escalationView2 = new OpenDental.InternalTools.Phones.EscalationView();
			this.SuspendLayout();
			// 
			// escalationView
			// 
			this.escalationView.BackColor = System.Drawing.Color.White;
			this.escalationView.BorderThickness = 1;
			this.escalationView.FadeAlphaIncrement = 20;
			this.escalationView.Font = new System.Drawing.Font("Calibri", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.escalationView.Items = ((System.ComponentModel.BindingList<string>)(resources.GetObject("escalationView.Items")));
			this.escalationView.LinePadding = -6;
			this.escalationView.Location = new System.Drawing.Point(32, 69);
			this.escalationView.MinAlpha = 60;
			this.escalationView.MinScrollable = 1;
			this.escalationView.Name = "escalationView";
			this.escalationView.OuterColor = System.Drawing.Color.Black;
			this.escalationView.Size = new System.Drawing.Size(304, 323);
			this.escalationView.StartFadeIndex = 0;
			this.escalationView.TabIndex = 86;
			// 
			// escalationView2
			// 
			this.escalationView2.Font = new System.Drawing.Font("Calibri", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.escalationView2.Location = new System.Drawing.Point(414, 69);
			this.escalationView2.Name = "escalationView2";
			this.escalationView2.Size = new System.Drawing.Size(304, 323);
			this.escalationView2.TabIndex = 92;
			this.escalationView2.Text = "escalationView1";
			// 
			// FormEscalationViewTests
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(778, 512);
			this.Controls.Add(this.escalationView2);
			this.Controls.Add(this.escalationView);
			this.Name = "FormEscalationViewTests";
			this.Text = "FormEscalationViewTests";
			this.Load += new System.EventHandler(this.FormEscalationViewTests_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.EscalationViewControl escalationView;
		private OpenDental.InternalTools.Phones.EscalationView escalationView2;
	}
}