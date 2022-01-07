using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	/// <summary>
	/// DialogResult values are either Yes or No (cancel is indicated by No)
	/// </summary>
	public partial class FormWarnToCloseComputers :FormODBase
	{
		public FormWarnToCloseComputers()
		{
			InitializeComponent();
		}

		private void FormWarnToCloseComputers_Load(object sender, EventArgs e)
		{
			Image[] _Images = new Image[8];
			System.Reflection.Assembly thisExe;
			thisExe = System.Reflection.Assembly.GetExecutingAssembly();
			
			
			string[] resources = thisExe.GetManifestResourceNames();
string baseName = "SnuffLight";
			List<string> list_resources = new List<string>();
			foreach (string s in resources)
			{
				if (s.Contains(baseName))
					list_resources.Add(s);
			}
			//list_resources.AddRange(resources);



			for (int i = 1; i < 9; i++)
			{
				string lookupName = baseName + i.ToString() + ".gif";
				for (int j = 1; j < list_resources.Count; j++)
				{
					if (list_resources[j].Contains(lookupName))
					{
						System.IO.Stream file =
							thisExe.GetManifestResourceStream(list_resources[j]);
						_Images[i - 1] = Image.FromStream(file);
						j = list_resources.Count;
					}
				}

			}
			this.imageAnimator1.SET_IMAGES = _Images;
			this.imageAnimator1.StartAnimation();
			
		
		}

		private void butYes_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Yes;
		}

		private void butNoCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.No;
		}
	}
}
