using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormInnoDb:FormODBase {

		/// <summary></summary>
		public FormInnoDb() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.C(this,new System.Windows.Forms.Control[]{
				this.textBox1
			});
			Lan.F(this);
		}

		private void FormInnoDb_Load(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			StringBuilder strB=new StringBuilder();
			strB.Append('-',60);
			textBox1.Text=DateTime.Now.ToString()+strB.ToString()+"\r\n";
			Application.DoEvents();
			textBox1.Text+=Lans.g("FormInnoDb","Default Storage Engine: "+InnoDb.GetDefaultEngine().ToString()+"\r\n");
			Application.DoEvents();
			textBox1.Text+=InnoDb.GetEngineCount();
			Application.DoEvents();
			Cursor=Cursors.Default;
		}

		/// <summary>Will only convert to MyISAM if default storage engine set to MyISAM.</summary>
		private void butToMyIsam_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will convert all tables in the database to the MyISAM storage engine.  This may take several minutes.\r\nContinue?")) {
				return;
			}
			if(InnoDb.GetDefaultEngine()=="InnoDB") {
				using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(
					Lan.g("FormInnoDB","You will first need to change your default storage engine to MyISAM.  Make sure that the following line is in your my.ini file: \r\n"
					+"default-storage-engine=MyISAM.\r\n"
					+"Then, restart the MySQL service and return here."));
				msgbox.ShowDialog();
				return;
			}
			if(!Shared.MakeABackup(BackupLocation.InnoDbTool)) {
				return;//A message has already shown that the backup failed.
			}
			Cursor=Cursors.WaitCursor;
			textBox1.Text+=Lans.g("FormInnoDb","Default Storage Engine: "+InnoDb.GetDefaultEngine().ToString()+"\r\n");
			Application.DoEvents();
			int numchanged=InnoDb.ConvertTables("InnoDB","MyISAM");
			textBox1.Text+=Lan.g("FormInnoDb","Number of tables converted to MyISAM: ")+numchanged.ToString()+"\r\n";
			Application.DoEvents();
			textBox1.Text+=InnoDb.GetEngineCount();
			Application.DoEvents();
			Cursor=Cursors.Default;
		}

		/// <summary>Will only convert to InnoDB if default storage engine set to InnoDB and skip-innodb is not in my.ini file, which disables InnoDB engine.</summary>
		private void butToInnoDb_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will convert all tables in the database to the InnoDB storage engine.  This may take several minutes.\r\nContinue?")) {
				return;
			}
			if(!InnoDb.IsInnodbAvail()) {
				using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(
					Lan.g("FormInnoDb","InnoDB storage engine is disabled.  In order for InnoDB tables to work you must comment out the skip-innodb line in your my.ini file, like this:\r\n"
					+"#skip-innodb\r\n"
					+"and, if present, comment out the default-storage-engine line like this: \r\n"
					+"#default-storage-engine=MyISAM.\r\n"
					+"Then, restart the MySQL service and return here."));
				msgbox.ShowDialog();
				return;
			}
			if(InnoDb.GetDefaultEngine()=="MyISAM") {
				using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(
					Lan.g("FormInnoDB","You will first need to change your default storage engine to InnoDB.  In your my.ini file, comment out the default-storage-engine line like this: \r\n"
					+"#default-storage-engine=MyISAM.\r\n"
					+"Then, restart the MySQL service and return here."));
				msgbox.ShowDialog();
				return;
			}
			if(!Shared.MakeABackup(BackupLocation.InnoDbTool)) {
				return;//A message has already shown that the backup failed.
			}
			Cursor=Cursors.WaitCursor;
			textBox1.Text+=Lans.g("FormInnoDb","Default Storage Engine: "+InnoDb.GetDefaultEngine().ToString()+"\r\n");
			Application.DoEvents();
			int numchanged=InnoDb.ConvertTables("MyISAM","InnoDB");
			textBox1.Text+=Lan.g("FormInnoDb","Number of tables converted to InnoDB: ")+numchanged.ToString()+"\r\n";
			Application.DoEvents();
			textBox1.Text+=InnoDb.GetEngineCount();
			Application.DoEvents();
			Cursor=Cursors.Default;
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}