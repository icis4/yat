//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a picture box that extends <see cref="PictureBox"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	[DesignerCategory("Windows Forms")]
	public partial class PictureBoxEx : PictureBox
	{
		/// <summary></summary>
		public PictureBoxEx()
		{
			InitializeComponent();
		}

		/// <summary></summary>
		public PictureBoxEx(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}

		/// <summary></summary>
		public float ImageWidthHeightRatio
		{
			get { return ((float)Image.Width / Image.Height); }
		}

		/// <summary></summary>
		public float WidthHeightRatio
		{
			get { return ((float)this.Width / this.Height); }
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
						throw (new InvalidOperationException("Unknown SizeMode"));
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
						throw (new InvalidOperationException("Unknown SizeMode"));
					}
				}
			}
		}
	}
}
