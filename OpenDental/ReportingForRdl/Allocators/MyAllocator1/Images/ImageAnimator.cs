using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace OpenDental.Reporting.Allocators.MyAllocator1.Images
{
	/// <summary>
	/// Creates a simple txbox for animation of images.  Uses 100ms for animation speed.  If you want to change this
	/// just add it to the code to change the timer.  Size change of the control changes the size of the image box.
	/// </summary>
	public partial class ImageAnimator : UserControl
	{
		private Image[] _Images = null;
		private int _CurrentImageIndex = 0;
		public ImageAnimator()
		{
			InitializeComponent();
			this.ImageBox.Size = this.Size;
		}
		/// <summary>
		/// If SET_IMAGES != NULL  animation starts using standard timer
		/// </summary>
		public void StartAnimation()
		{
			if (_Images != null)
			this.LoopTimer.Start();
		}
		/// <summary>
		/// Stops the timer that creates the animation.
		/// </summary>
		public void StopAnimation()
		{
			this.LoopTimer.Start();
		}

		private void LoopTimer_Tick(object sender, EventArgs e)
		{
			if (_Images != null)
			{
				_CurrentImageIndex++;
				if (_CurrentImageIndex >= _Images.Length)
					_CurrentImageIndex = 0;
				if (_Images[_CurrentImageIndex] != null)
					this.ImageBox.Image = _Images[_CurrentImageIndex];

			}

		}

		private void ImageAnimator_SizeChanged(object sender, EventArgs e)
		{
			this.ImageBox.Size = this.Size;
		}
		/// <summary>
		/// Gets or Sets the Images for animating
		/// </summary>
		public Image[] SET_IMAGES
		{
			set
			{
				this._Images = value;
				if (value == null)
					this.LoopTimer.Stop();
			}
			get
			{
				return this._Images;
			}
		}


	}
}
