using System.Windows.Forms;

namespace OpenDental {
	///<summary></summary>
	public partial class ValidDate {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ){
			if(disposing){
				components?.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		private void InitializeComponent(){
			this.SuspendLayout();
			// 
			// ValidDate
			// 
			this.Validated += new System.EventHandler(this.ValidDate_Validated);
			this.Validating += new System.ComponentModel.CancelEventHandler(this.ValidDate_Validating);
			this.TextChanged += new System.EventHandler(this.ValidDate_TextChanged);
			this.ResumeLayout(false);

		}
		#endregion

	}
}