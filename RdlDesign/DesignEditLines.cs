/* ====================================================================
    Copyright (C) 2004-2006  fyiReporting Software, LLC

    This file is part of the fyiReporting RDL project.
	
    The RDL project is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    For additional information, email info@fyireporting.com or visit
    the website www.fyiReporting.com.
*/
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Globalization;
using System.Net;

namespace fyiReporting.RdlDesign
{

	/// <summary>
	/// Control for providing a designer image of RDL.   Works directly off the RDL XML.
	/// </summary>
    internal class DesignEditLines : UserControl, System.ComponentModel.ISupportInitialize
    {
        System.Windows.Forms.RichTextBox editor;
        int saveTbEditorLines = -1;

        internal DesignEditLines(System.Windows.Forms.RichTextBox e)
            : base()
        {
            // force to double buffering for smoother drawing
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint,
                true);

            editor = e;
            editor.TextChanged += new System.EventHandler(editor_TextChanged);
            editor.Resize += new System.EventHandler(editor_Resize);
            editor.VScroll += new System.EventHandler(editor_VScroll);

            this.Paint += new PaintEventHandler(DesignEditLinesPaint);
        }

		private void DesignEditLinesPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Lines_Draw(e.Graphics);
        } 
        
        private void Lines_Draw(Graphics g)
        {
            if (!this.Visible)
                return;
            int lineHeight = 0;
            try
            {  // its possible that there are less than 2 lines; so trap the error
                lineHeight = editor.GetPositionFromCharIndex(editor.GetFirstCharIndexFromLine(2)).Y -
                          editor.GetPositionFromCharIndex(editor.GetFirstCharIndexFromLine(1)).Y;
            }
            catch { return; }
            if (lineHeight <= 0)
                return;

            // Get the first line index and location
            int first_index;
            int first_line;
            int first_line_y;
            first_index = editor.GetCharIndexFromPosition(new
                     Point(0, (int)(g.VisibleClipBounds.Y + lineHeight / 3)));
            first_line = editor.GetLineFromCharIndex(first_index);
            first_line_y = editor.GetPositionFromCharIndex(first_index).Y;

            //  Draw the lines
            SolidBrush sb = new SolidBrush(Control.DefaultBackColor);
            g.FillRectangle(sb, g.VisibleClipBounds);
            sb.Dispose();

            int i = first_line;
            float y = first_line_y + lineHeight * (i - first_line - 1);
            while (y < g.VisibleClipBounds.Y + g.VisibleClipBounds.Height)
            {
                string l = i.ToString();
                g.DrawString(l, editor.Font, Brushes.DarkBlue,
                    this.Width - (g.MeasureString(l, editor.Font).Width + 4), y);
                i += 1;
                if (i > editor.Lines.Length)
                    break;
                y = first_line_y + lineHeight * (i - first_line - 1);
            }
        }

        private void editor_Resize(object sender, EventArgs e)
        {
            using (Graphics g = this.CreateGraphics())
            {
                Lines_Draw(g);
            }
        }

        private void editor_VScroll(object sender, EventArgs e)
        {
            using (Graphics g = this.CreateGraphics())
            {
                Lines_Draw(g);
            }
        }

		private void editor_TextChanged(object sender, System.EventArgs e)
        {
            // when # of lines change we may need to repaint the line #s
            if (saveTbEditorLines != editor.Lines.Length)
            {
                using (Graphics g = this.CreateGraphics())
                {
                    Lines_Draw(g);
                }
            }
            saveTbEditorLines = editor.Lines.Length;
        }

        #region ISupportInitialize Members

        void System.ComponentModel.ISupportInitialize.BeginInit()
        {
            return;
        }

        void System.ComponentModel.ISupportInitialize.EndInit()
        {
            return;
        }

        #endregion
    }
}
