//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
		#region Image Rotation
		//==========================================================================================
		// Image Rotation
		//==========================================================================================

		#region Image Rotation > Constants
		//------------------------------------------------------------------------------------------
		// Image Rotation > Constants
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public const RotateType RotationDefault = RotateType.RotateNone;

		#endregion

		#region Image Rotation > Fields
		//------------------------------------------------------------------------------------------
		// Image Rotation > Fields
		//------------------------------------------------------------------------------------------

		private RotateType rotation = RotationDefault;

		#endregion

		#region Image Rotation > Properties
		//------------------------------------------------------------------------------------------
		// Image Rotation > Properties
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Gets or sets the image that is displayed by this picture box.
		/// </summary>
		/// <remarks>
		/// Overridden to get a notification when the image changes.
		/// </remarks>
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

		/// <summary></summary>
		[DefaultValue(RotationDefault)]
		public virtual RotateType Rotation
		{
			get { return (this.rotation); }
			set
			{
				if (this.rotation != value)
				{
					this.rotation = value;
					ApplyImageRotationAfterRotationChange();
				}
			}
		}

		#endregion

		#region Image Rotation > Methods
		//------------------------------------------------------------------------------------------
		// Image Rotation > Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// This variable is used to get the difference between the actual and the desired rotation.
		/// Without this, the image would need to be kept twice, with and without rotation. Upon a
		/// change of a property, the image would have to be redrawn from the original image.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'rotationOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private RotateType ApplyImageRotation_rotationOld = RotationDefault;

		private void ApplyImageRotationAfterRotationChange()
		{
			if (this.ApplyImageRotation_rotationOld != this.rotation)
			{
				if (Image != null)
				{
					RotateType requiredRotation = RotateTypeEx.RotationFromDifference(this.ApplyImageRotation_rotationOld, this.rotation);
					Image.RotateFlip(RotateTypeEx.ConvertToRotateFlipType(requiredRotation));
					Invalidate();

					this.ApplyImageRotation_rotationOld = this.rotation;
				}
			}
		}

		private void ApplyImageRotationAfterImageChange()
		{
			if (Image != null)
			{
				RotateType requiredRotation = this.rotation;
				Image.RotateFlip(RotateTypeEx.ConvertToRotateFlipType(requiredRotation));
				Invalidate();

				this.ApplyImageRotation_rotationOld = this.rotation;
			}
		}

		#endregion

		#endregion

		#region Scaled Image
		//==========================================================================================
		// Scaled Image
		//==========================================================================================

		#region Scaled Image > Properties
		//------------------------------------------------------------------------------------------
		// Scaled Image > Properties
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public float ImageWidthHeightRatio
		{
			get { return ((float)Image.Width / Image.Height); }
		}

		/// <summary></summary>
		public float WidthHeightRatio
		{
			get { return ((float)Width / Height); }
		}

		/// <summary></summary>
		public bool WidthHasScaled
		{
			get { return (ImageWidthHeightRatio < WidthHeightRatio); }
		}

		/// <summary></summary>
		public bool HeightHasScaled
		{
			get { return (ImageWidthHeightRatio > WidthHeightRatio); }
		}

		/// <summary>
		/// Returns the effective location of the image after it got scaled according to
		/// <see cref="PictureBox.SizeMode"/>.
		/// </summary>
		public Point ScaledImageLocation
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
						int scaledLeft = (Width - Image.Width) / 2;
						int scaledTop = (Height - Image.Height) / 2;
						return (new Point(scaledLeft, scaledTop));
					}

					case PictureBoxSizeMode.Zoom:
					{
						Size scaledSize = ScaledImageSize;
						int scaledLeft = (Width - scaledSize.Width) / 2;
						int scaledTop = (Height - scaledSize.Height) / 2;
						return (new Point(scaledLeft, scaledTop));
					}

					default:
					{
						throw (new NotSupportedException("Program execution should never get here, unknown 'SizeMode'." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
		}

		/// <summary>
		/// Returns the effective size of the image after it got scaled according to
		/// <see cref="PictureBox.SizeMode"/>.
		/// </summary>
		public Size ScaledImageSize
		{
			get
			{
				switch (SizeMode)
				{
					case PictureBoxSizeMode.Normal:
					case PictureBoxSizeMode.CenterImage:
					{
						return (new Size(Image.Width, Image.Height));
					}

					case PictureBoxSizeMode.StretchImage:
					case PictureBoxSizeMode.AutoSize:
					{
						return (new Size(Width, Height));
					}

					case PictureBoxSizeMode.Zoom:
					{
						int scaledWidth = Width;
						int scaledHeight = Height;

						if (WidthHasScaled)
							scaledWidth = (int)((float)Width / WidthHeightRatio * ImageWidthHeightRatio);

						if (HeightHasScaled)
							scaledHeight = (int)((float)Height * WidthHeightRatio / ImageWidthHeightRatio);

						return (new Size(scaledWidth, scaledHeight));
					}

					default:
					{
						throw (new NotSupportedException("Program execution should never get here, unknown 'SizeMode'." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
		}

		#endregion

		#endregion
	}
}
