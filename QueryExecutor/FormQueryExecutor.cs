using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace QueryExecutor {
	public partial class FormQueryExecutor:Form {
		private string conStr;

		public FormQueryExecutor() {
			InitializeComponent();
		}

		private void FormQueryExecutor_Load(object sender,EventArgs e) {
			//this.Handle
			//Char char4=Convert.ToChar(4);
			//string command=char4.ToString();
			//string command=
			//	"\0x04";
				//"\004";
			//MessageBox.Show("*"+command+"*");
			SetConnStr();
		}

		private void SetConnStr(){
			conStr="Server=localhost"
				+";Database="+textDatabase.Text
				+";User ID=root"
				+";Password="
				+";CharSet=utf8";
		}

		private void butChange_Click(object sender,EventArgs e) {
			SetConnStr();
		}

		private void butExecute_Click(object sender,EventArgs e) {
			MySqlConnection con=new MySqlConnection(conStr);
			MySqlCommand cmd = new MySqlCommand();
			cmd.Connection=con;
			cmd.CommandText=textQuery.Text;
			int rowsChanged=0;
			try {
				con.Open();
				rowsChanged=cmd.ExecuteNonQuery();
				con.Close();
				MessageBox.Show("Rows changed:"+rowsChanged.ToString());
			}
			catch(MySqlException ex) {
				MessageBox.Show(ex.Message);
			}
			catch(Exception ex){
				MessageBox.Show("Error: "+ex.Message);
			}
		}

		



	}
}