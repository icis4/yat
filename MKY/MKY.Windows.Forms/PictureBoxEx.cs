//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

using MKY.Drawing;

#endregion

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a picture box that extends <see cref="PictureBox"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public class PictureBoxEx : PictureBox
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const RotateType ImageRotationDefault = RotateTypeEx.Default;

		/// <summary></summary>
		public const int ImageZoomPercentDefault = 100;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private RotateType imageRotation = ImageRotationDefault;
		private int imageZoomPercent = ImageZoomPercentDefault;

		#endregion

		#region Control Properties
		//==========================================================================================
		// Control Properties
		//==========================================================================================

		/// <summary>
		/// Gets the aspect ratio of the control's rectangle.
		/// </summary>
		[Description("The aspect ratio of the control's rectangle.")]
		public float AspectRatio
		{
			get { return ((float)Width / Height); }
		}

		#endregion

		#region Image Properties
		//==========================================================================================
		// Image Properties
		//==========================================================================================

		/// <summary>
		/// Gets or sets the image that is displayed by this picture box.
		/// </summary>
		/// <remarks>
		/// Overridden to get a notification when the image changes.
		/// </remarks>
		[Description("The image that is displayed by this picture box.")]
		[Localizable(true)]
		[Bindable(true)]
		public new Image Image
		{
			get { return (base.Image); }
			set
			{
				if (base.Image != value)
				{
					base.Image = value;
					ApplyImageRotationAfterImageChange();
				}
			}
		}

		/// <summary>
		/// Gets the aspect ratio of the original's image rectangle.
		/// </summary>
		[Description("The aspect ratio of the original's image rectangle.")]
		public float ImageAspectRatio
		{
			get
			{
				if (Image != null)
					return ((float)Image.Size.Width / Image.Size.Height);
				else
					return (1.0f);
			}
		}

		#endregion

		#region Image Rotation Properties and Methods
		//==========================================================================================
		// Image Rotation Properties and Methods
		//==========================================================================================

		/// <summary></summary>
		[DefaultValue(ImageRotationDefault)]
		public virtual RotateType ImageRotation
		{
			get { return (this.imageRotation); }
			set
			{
				if (this.imageRotation != value)
				{
					this.imageRotation = value;
					ApplyImageRotationAfterRotationChange();
				}
			}
		}

		/// <summary>
		/// This variable is used to get the difference between the actual and the desired rotation.
		/// Without this, the image would need to be kept twice, with and without rotation. Upon a
		/// change of a property, the image would have to be redrawn from the not rotated image.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'rotationOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private RotateType ApplyImageRotation_imageRotationOld = ImageRotationDefault;

		private void ApplyImageRotationAfterRotationChange()
		{
			// Skip calculation during designer generated initalization sequence to ensure that the
			// designer generated local resource is shown with the correct orientation:
			if (!this.Created)
			{
				this.ApplyImageRotation_imageRotationOld = this.imageRotation;
				return;
			}

			// Calculate the difference between the desired and the 'old' rotation and rotate the image:
			if ((Image != null) &&
				(this.ApplyImageRotation_imageRotationOld != this.imageRotation))
			{
				RotateType deltaRotation = RotateTypeEx.RotationFromDifference(this.ApplyImageRotation_imageRotationOld, this.imageRotation);
				Image.RotateFlip(RotateTypeEx.ConvertToRotateFlipType(deltaRotation));
				Invalidate();

				this.ApplyImageRotation_imageRotationOld = this.imageRotation;
			}
		}

		private void ApplyImageRotationAfterImageChange()
		{
			// Skip calculation during designer generated initalization sequence to ensure that the
			// designer generated local resource is shown with the correct orientation:
			if (!this.Created)
			{
				this.ApplyImageRotation_imageRotationOld = this.imageRotation;
				return;
			}

			// The image must be given 'face up', i.e. in 'normal' rotation.
			// So, simply apply the selected rotation to the image:
			if (Image != null)
			{
				Image.RotateFlip(RotateTypeEx.ConvertToRotateFlipType(this.imageRotation));
				Invalidate();

				this.ApplyImageRotation_imageRotationOld = this.imageRotation;
			}
		}

		#endregion

		#region Image Zoom Properties and Methods
		//==========================================================================================
		// Image Zoom Properties and Methods
		//==========================================================================================

		/// <summary></summary>
		[DefaultValue(ImageZoomPercentDefault)]
		public virtual int ImageZoomPercent
		{
			get { return (this.imageZoomPercent); }
			set
			{
				if (this.imageZoomPercent != value)
				{
					this.imageZoomPercent = value;
					ApplyImageZoom();
				}
			}
		}

		/// <summary></summary>
		protected virtual float ImageZoomFactor
		{
			get { return ((float)ImageZoomPercent / 100); }
		}

		/// <remarks>
		/// Workaround for border issue in 'Graphics.DrawImage()'.
		/// See http://www.codeproject.com/csharp/BorderBug.asp for details.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
		protected virtual void ApplyImageZoom()
		{
			if (Image != null)
			{
				/*

				// Clone the image before making transformations:
				Image src = new Bitmap(this.image.Width, this.image.Height);
				float srcAspect = (float)src.Width / (float)src.Height;

				// Rotate the image according to the selected orientation:
				src.RotateFlip(RotateTypeEx.ConvertToRotateFlipType(pictureBox_Screen.ImageRotation));

				// Draw the image according to size mode (and zoom):
				switch (pictureBox_Screen.SizeMode)
				{
					case PictureBoxSizeMode.AutoSize:
					{
						throw (new NotSupportedException(AutoSizeErrorMessage));
					}

					case PictureBoxSizeMode.Normal:
					case PictureBoxSizeMode.CenterImage:
					{
						// \todo
						break;
					}

					case PictureBoxSizeMode.Zoom:
					case PictureBoxSizeMode.StretchImage:
					default:
					{
						RectangleF srcRect = new RectangleF(-0.5f, -0.5f, src.Width, src.Height);

						int destWidth = 0;
						int destHeight = 0;
						if (pictureBox_Screen.SizeMode == PictureBoxSizeMode.Zoom)
						{	// Zoom but preserve aspect ratio:
							destWidth  = (int)(ZoomFactor * pictureBox_Screen.Width);
							destHeight = (int)(ZoomFactor * pictureBox_Screen.Height);
						}
						else
						{	// Stretch:
							destWidth  = (int)(ZoomFactor * pictureBox_Screen.Width);
							destHeight = (int)(ZoomFactor * pictureBox_Screen.Height);
						}
						RectangleF destRect = new RectangleF(0, 0, destWidth, destHeight);

						Image complete = new Bitmap(src.Width, src.Height);
						using (Graphics g = Graphics.FromImage(complete))
						{
							g.InterpolationMode = InterpolationMode.NearestNeighbor;
							g.DrawImage(src, destRect, srcRect, GraphicsUnit.Pixel);
						}
						pictureBox_Screen.Image = complete;

						break;
					}

				*/
			}
		}

		#endregion

		#region Image Scale Properties
		//==========================================================================================
		// Image Scale Properties
		//==========================================================================================

		/// <summary>
		/// The aspect ratio of the scaled image's rectangle.
		/// </summary>
		public float ImageScaledAspectRatio
		{
			get
			{
				Size scaled = ImageScaledSize;
				return ((float)scaled.Width / scaled.Height);
			}
		}

		/// <summary>
		/// Returns the effective size of the image after it got scaled according to
		/// <see cref="PictureBox.SizeMode"/>.
		/// </summary>
		public Size ImageScaledSize
		{
			get
			{
				switch (SizeMode)
				{
					case PictureBoxSizeMode.Normal:
					case PictureBoxSizeMode.AutoSize:
					case PictureBoxSizeMode.CenterImage:
					{
						return (new Size(Image.Width, Image.Height));
					}

					case PictureBoxSizeMode.StretchImage:
					{
						return (new Size(Width, Height));
					}

					case PictureBoxSizeMode.Zoom:
					{
						int scaledWidth = Width;
						int scaledHeight = Height;

						if (ImageAspectRatio < AspectRatio)
							scaledWidth  = (int)(((float)Width  / AspectRatio * ImageAspectRatio) + 0.5f); // Equals non-existent 'int Floor(float)' rounding toward negative infinity.

						if (ImageAspectRatio > AspectRatio)
							scaledHeight = (int)(((float)Height * AspectRatio / ImageAspectRatio) + 0.5f); // Equals non-existent 'int Floor(float)' rounding toward negative infinity.

						return (new Size(scaledWidth, scaledHeight));
					}

					default:
					{
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "Unknown 'SizeMode'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
		}

		/// <summary>
		/// Returns the effective location of the image after it got scaled according to
		/// <see cref="PictureBox.SizeMode"/>.
		/// </summary>
		public Point ImageScaledLocation
		{
			get
			{
				switch (SizeMode)
				{
					case PictureBoxSizeMode.Normal:
					case PictureBoxSizeMode.StretchImage:
					case PictureBoxSizeMode.AutoSize:
					{
						return (new Point(0, 0));
					}

					case PictureBoxSizeMode.CenterImage:
					{
						Size imageSize = Image.Size;
						int scaledLeft = (int)(((float)(Width - imageSize.Width) / 2) + 0.5f);  // Equals non-existent 'int Floor(float)' rounding toward negative infinity.
						int scaledTop = (int)(((float)(Height - imageSize.Height) / 2) + 0.5f); // Equals non-existent 'int Floor(float)' rounding toward negative infinity.
						return (new Point(scaledLeft, scaledTop));
					}

					case PictureBoxSizeMode.Zoom:
					{
						Size scaledSize = ImageScaledSize;
						int scaledLeft = (int)(((float)(Width - scaledSize.Width) / 2) + 0.5f);  // Equals non-existent 'int Floor(float)' rounding toward negative infinity.
						int scaledTop = (int)(((float)(Height - scaledSize.Height) / 2) + 0.5f); // Equals non-existent 'int Floor(float)' rounding toward negative infinity.
						return (new Point(scaledLeft, scaledTop));
					}

					default:
					{
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "Unknown 'SizeMode'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
		}

		/// <summary>
		/// Returns the rectangle of the image after it got scaled according to
		/// <see cref="PictureBox.SizeMode"/>.
		/// </summary>
		public Rectangle ImageScaledRectangle
		{
			get { return (new Rectangle(ImageScaledLocation, ImageScaledSize)); }
		}

		/// <summary>
		/// Returns the effectively visible rectangle of the image after it got scaled according to
		/// <see cref="PictureBox.SizeMode"/>.
		/// </summary>
		public Rectangle ImageVisibleRectangle
		{
			get
			{
				Rectangle r = ImageScaledRectangle;
				r.Intersect(ClientRectangle);
				return (r);
			}
		}

		/// <summary>
		/// Returns the size of the effectively visible rectangle of the image after it got scaled
		/// according to <see cref="PictureBox.SizeMode"/>.
		/// </summary>
		public Size ImageVisibleSize
		{
			get
			{
				Rectangle r = ImageVisibleRectangle;
				Size s = new Size(r.Width, r.Height);
				return (s);
			}
		}

		#endregion
	}
}
