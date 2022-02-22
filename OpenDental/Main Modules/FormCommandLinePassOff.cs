using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormCommandLinePassOff:FormODBase {
		public string[] CommandLineArgs;

		public FormCommandLinePassOff() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormCommandLinePassOff_Load(object sender,EventArgs e) {
			try {
				TcpClient client= new TcpClient("localhost",2123);
				NetworkStream ns = client.GetStream();
				XmlSerializer serializer=new XmlSerializer(typeof(string[]));
				serializer.Serialize(ns,CommandLineArgs);
				ns.Close();
				client.Close();
			}
			catch { }
			Close();
		}
	}
}
