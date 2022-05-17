using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Data;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental
{
	///<summary>Summary description for ContrAnesthesia.</summary>
	public class ContrAnesthesiaData : System.Windows.Forms.Control{
		///<summary>Required designer variable.</summary>
		private System.ComponentModel.Container components = null;
		///<summary>The index in Anesthesics.List of the currently selected anesthetic.</summary>
		private int selectedAnestheticData;

		public AnestheticData AnestheticDataCur;
		///<summary>The index in Anesthetics.List of the currently selected anesthetic.</summary>
		public int SelectedAnestheticData{
			get{
				return selectedAnestheticData;
			}
				set{
				selectedAnestheticData=value;
			}
		}

		///<summary>Used in LoadData.</summary>

		///<summary></summary>
		public void SaveCurAnestheticData (int anestheticDataNum)
		{
		}
		///<summary></summary>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Focus();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		
		}



}
















