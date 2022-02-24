using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;

namespace OpenDental.UI {
	public interface IFrameSource {
		event Action<IFrameSource,byte[],System.Drawing.Size> NewFrame;

		void StartFrameCapture();
		void StopFrameCapture();
	}
}
