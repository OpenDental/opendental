using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI {
    ///<summary>Primarily used for the Job Manager to reduce flicker of the TreeView on refresh. </summary>
	public partial class BufferedTreeView:TreeView {

        //The solution for this double-buffered treeview comes from stack overflow: https://stackoverflow.com/questions/10362988/treeview-flickering/10364283#10364283
        //Author: Hans Passant, #10 all time reputation rank on stack overflow. Not quite Jon Skeet but not far off either.
        protected override void OnHandleCreated(EventArgs e) {
            SendMessage(this.Handle,TVM_SETEXTENDEDSTYLE,(IntPtr)TVS_EX_DOUBLEBUFFER,(IntPtr)TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }
        // Pinvoke:
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;//This is flagged as unused but all examples I can find set this and then do nothing with it.
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd,int msg,IntPtr wp,IntPtr lp);
    }
}
