using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	public partial class FormReport_GuarantorAllocationCheck :FormODBase
	{
		private MyAllocator1.SupportingCode.Report1_GuarantorAllocation _Report_GA = null;
		private int _Guarantor = -1;
		public FormReport_GuarantorAllocationCheck()
		{
			InitializeComponent();
		}
		public FormReport_GuarantorAllocationCheck(int Guarantor)
		{
			InitializeComponent();
		}
		private void butFillReport_Click(object sender, EventArgs e)
		{
			try
			{
				int Guarantor = Int32.Parse(this.textBox1.Text);
				if (Guarantor < 1)
					throw new Exception("Invalid Guarantor Number");
				this._Report_GA = new OpenDental.Reporting.Allocators.MyAllocator1.SupportingCode.Report1_GuarantorAllocation();
				_Report_GA.FILL(Guarantor);
				this.lblTitle.Text = _Report_GA.TITLE;
				this.dgridView_ReportData.DataSource = _Report_GA.MAIN_REPORT;
				
				this.dgvSummary.DataSource = _Report_GA.SUMMARY;

				#region Stylize the DataGridViews
				
				//DataGridViewCellStyle defaultCellStyle1 = new DataGridViewCellStyle();
				//defaultCellStyle1.Padding = new Padding(0,0,0,0);
				//defaultCellStyle1.Font = new Font("Arial", 8F);

				
				//DataGridViewCellStyle defaultCellStyle2 = new DataGridViewCellStyle(defaultCellStyle1);
				//defaultCellStyle2.BackColor = Color.LightCyan;
				//this.dgridView_ReportData.DefaultCellStyle = defaultCellStyle1;
				//this.dgridView_ReportData.AlternatingRowsDefaultCellStyle = defaultCellStyle2;
				//this.dgridView_ReportData.CellBorderStyle = DataGridViewCellBorderStyle.None;
				//this.dgridView_ReportData.RowHeadersVisible = false;
				// Above Code moved to Static Method
				MyAllocator1.ReportUI.ReportStyles.Set_DefaultDataGridViewStyle(dgridView_ReportData);
				#endregion
				#region Update The Form Caption
				int index = this.lblTitle.Text.IndexOf('\n');
				if (index > 0)
					this.Text = this.lblTitle.Text.Substring(0, index);
				else if (this.lblTitle.Text.Length > 0)
					this.Text = this.lblTitle.Text;
				else
					this.Text = "Allocation Report for Guarantor: " + Guarantor;
				#endregion
			}
			catch (Exception exc)
			{
				PU.MB = exc.Message;
			}
			
		}


		private void butPrintPreview_Click(object sender, EventArgs e)
		{
			if (this._Report_GA == null)
				_Report_GA.FILL(_Guarantor);
			_Report_GA.ShowReportPreview(_Guarantor);
			//PrintPreviewControl ppc = new PrintPreviewControl();
			//ppc.Document = new System.Drawing.Printing.PrintDocument();
			//// Set the zoom to 100 percent.
			//ppc.Zoom = 1;
			//// Set the UseAntiAlias property to true so fonts are smoothed
			//// by the operating system.
			//ppc.UseAntiAlias = true;

			////// Add the control to the form.
			////this.Controls.Add(this.PrintPreviewControl1);

			////// Associate the event-handling method with the
			////// document's PrintPage event.
			////this.docToPrint.PrintPage +=
			////    new System.Drawing.Printing.PrintPageEventHandler(
			////    docToPrint_PrintPage);

			//ppc.Show();



		}

		private void TestForm_Load(object sender, EventArgs e)
		{
			if (_Report_GA != null)
			{
				_Report_GA.FILL(_Guarantor);
				this.textBox1.Text = _Guarantor.ToString();
				butFillReport_Click(this, EventArgs.Empty);
			}
		}

		#region Commented out Printing Support - Self Contained.  I was going to work on this but I got over zealous
		////protected override void OnPaint(PaintEventArgs e)
		////{
		////    base.OnPaint(e);


		////}
		//void DrawForm(Graphics g, int resX, int resY)
		//{
			
		//    g.FillRectangle(new SolidBrush(this.BackColor), 0, 0, this.Width, this.Height);
		//    float scale = 1; // resX / ScreenResolution;
		//    // Cycle through each control on the form and paint it to the printe
		//    foreach (Control c in Controls)
		//    {
		//        // Get the time of the next control so we can unbox it
		//        string strType = c.GetType().ToString().Substring(c.GetType().ToString().LastIndexOf(".") + 1);
		//        switch (strType)
		//        {
		//            case "Button": Button b = (Button)c;
		//                // Use the ControlPaint method DrawButton in order to draw the button of the form
		//                ControlPaint.DrawButton(g, ((Button)c).Left, ((Button)c).Top, ((Button)c).Width, ((Button)c).Height, ButtonState.Normal);
		//                // We also need to draw the text
		//                g.DrawString(b.Text, b.Font, new SolidBrush(b.ForeColor), b.Left + b.Width / 2 - g.MeasureString(b.Text,
		//                b.Font).Width / 2, b.Top + b.Height / 2 - g.MeasureString("a", b.Font).Height / 2, new StringFormat());
		//                break;
		//            case "TextBox": TextBox t = (TextBox)c;
		//                // Draw a text box by drawing a pushed in button and filling the rectangle with the background color and the text
		//                // of the TextBox control
		//                // First the sunken border
		//                ControlPaint.DrawButton(g, t.Left, t.Top, t.Width, t.Height, ButtonState.Pushed);
		//                // Then fill it with the background of the textbox
		//                g.FillRectangle(new SolidBrush(t.BackColor), t.Left + 1, t.Top + 1, t.Width + 2, t.Height - 2);
		//                // Finally draw the string inside
		//                g.DrawString(t.Text, t.Font, new SolidBrush(t.ForeColor), t.Left + 2, t.Top + t.Height / 2 - g.MeasureString("a", t.Font).Height / 2, new StringFormat());
		//                break;
		//            case "CheckBox":// We have a checkbox to paint, unbox it
		//                CheckBox cb = (CheckBox)c;
		//                // Use the DrawCheckBox command to draw a checkbox and pass the button state to paint it checked or unchecked
		//                if (cb.Checked)
		//                    ControlPaint.DrawCheckBox(g, cb.Left, cb.Top, cb.Height / 2, cb.Height / 2, ButtonState.Checked);
		//                else
		//                    ControlPaint.DrawCheckBox(g, cb.Left, cb.Top, cb.Height / 2, cb.Height / 2, ButtonState.Normal);
		//                // Don't forget the checkbox text
		//                g.DrawString(cb.Text, cb.Font, new SolidBrush(cb.ForeColor), cb.Right - cb.Height - g.MeasureString(cb.Text, cb.Font).Width, cb.Top, new StringFormat());
		//                break;
		//        }
		//    }
		//}
		#endregion

	}
}