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
// MKY Version 1.0.25
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of image rotation initialization/calculation/update:
////#define DEBUG_IMAGE_ROTATION

#endif // DEBUG

#endregion

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

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Image imageNotRotated; // = null;
		private RotateType imageRotation = ImageRotationDefault;

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
		/// Gets or sets the image that is displayed by this picture box in 'face up' orientation,
		/// i.e. as if 'RotateNone' was selected.
		/// </summary>
		/// <remarks>
		/// Overridden to handle image changes.
		/// </remarks>
		/// <remarks>
		/// The image is cloned to the base image to ensure each instance has its own clone of the image
		/// and can perform operations as needed without creating retroactive effects.
		/// </remarks>
		[Description("The image that is displayed by this picture box.")]
		[Localizable(true)]
		[Bindable(true)]
		public new Image Image
		{
			get { return (this.imageNotRotated); }
			set
			{
				if (value != null)
				{
					this.imageNotRotated = value;
					base.Image = (Image)value.Clone();
					ApplyBaseImageRotationAfterImageChange();
				}
				else
				{
					this.imageNotRotated = null;
					base.Image = null;
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
					ApplyBaseImageRotationAfterRotationChange();
				}
			}
		}

		/// <summary>
		/// This variable is used to get the difference between the actual and the desired rotation.
		/// Without this, the image would have to be re-cloned from <see cref="imageNotRotated"/>.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'imageRotationOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private RotateType ApplyBaseImageRotation_imageRotationOld = ImageRotationDefault;

		private void ApplyBaseImageRotationAfterRotationChange()
		{
		#if DEBUG_IMAGE_ROTATION
			// There's a tricky thing about this calculation, as it must correctly work in four situations:
			//  a) At design time, when the developer instantiates this control (i.e. DesignMode = true).
			//  b) At design time, when the designer generated code gets executed (i.e. DesignMode = true).
			//  c) At initialization, when the designer generated code gets executed (i.e. DesignMode = false).
			//  d) During normal operation, when the application updates the image.
			// Debugging a) and b) is not that easy, thus using a message box to output details.
			using (var sw = new System.IO.StringWriter())
			{
				Diagnostics.AnyWriter.WriteStack(sw, this.GetType(), "imageRotationOld = " + this.ApplyImageRotation_imageRotationOld + " / imageRotation = " + this.imageRotation + " / DesignMode = " + this.DesignMode);
				MessageBox.Show(sw.ToString());
			}
		#endif

			if (this.ApplyBaseImageRotation_imageRotationOld != this.imageRotation)
			{
				if (base.Image != null)
				{
					// Calculate the difference between the 'new' and 'old' rotation:
					RotateType deltaRotation = RotateTypeEx.RotationFromDifference(this.ApplyBaseImageRotation_imageRotationOld, this.imageRotation);

					// Then rotate the image:
					base.Image.RotateFlip(RotateTypeEx.ConvertToRotateFlipType(deltaRotation));
					Invalidate();
				}

				this.ApplyBaseImageRotation_imageRotationOld = this.imageRotation;
			}
		}

		private void ApplyBaseImageRotationAfterImageChange()
		{
		#if DEBUG_IMAGE_ROTATION
			// There's a tricky thing about this calculation, as it must correctly work in four situations:
			//  a) At design time, when the developer instantiates this control (i.e. DesignMode = true).
			//  b) At design time, when the designer generated code gets executed (i.e. DesignMode = true).
			//  c) At initialization, when the designer generated code gets executed (i.e. DesignMode = false).
			//  d) During normal operation, when the application updates the image.
			// Debugging a) and b) is not that easy, thus using a message box to output details.
			using (var sw = new System.IO.StringWriter())
			{
				Diagnostics.AnyWriter.WriteStack(sw, this.GetType(), "imageRotationOld = " + this.ApplyImageRotation_imageRotationOld + " / imageRotation = " + this.imageRotation + " / DesignMode = " + this.DesignMode);
				MessageBox.Show(sw.ToString());
			}
		#endif

			if (base.Image != null)
			{
				// The is given in 'face up' orientation, i.e. as if 'RotateNone' was selected.
				// So, simply apply the rotation to the image:
				base.Image.RotateFlip(RotateTypeEx.ConvertToRotateFlipType(this.imageRotation));
				Invalidate();
			}

			this.ApplyBaseImageRotation_imageRotationOld = this.imageRotation;
		}

		#endregion

		#region Image Scale Properties
		//==========================================================================================
		// Image Scale Properties
		//==========================================================================================

		/// <summary>
		/// Gets the aspect ratio of the scaled image rectangle.
		/// </summary>
		[Description("The aspect ratio of the scaled image rectangle.")]
		public float ImageScaledAspectRatio
		{
			get
			{
				if (base.Image != null)
					return ((float)base.Image.Size.Width / base.Image.Size.Height);
				else
					return (1.0f);
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

						if (ImageScaledAspectRatio < AspectRatio)
							scaledWidth  = (int)(((float)Width  / AspectRatio * ImageScaledAspectRatio) + 0.5f); // Equals non-existent 'int Floor(float)' rounding toward negative infinity.

						if (ImageScaledAspectRatio > AspectRatio)
							scaledHeight = (int)(((float)Height * AspectRatio / ImageScaledAspectRatio) + 0.5f); // Equals non-existent 'int Floor(float)' rounding toward negative infinity.

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
