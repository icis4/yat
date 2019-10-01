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
// YAT Version 2.0.1 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.Format.Types;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[DefaultEvent("FormatChanged")]
	public partial class TextFormat : UserControl
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Font font = new Font(FontFormat.NameDefault, FontFormat.SizeDefault, FontFormat.StyleDefault);
		private Color color = DefaultForeColor;
		private int[] customColors; // = null;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when any of the text format properties is changed.")]
		public event EventHandler FormatChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when any of the CustomColors property is changed.")]
		public event EventHandler CustomColorsChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TextFormat()
		{
			InitializeComponent();
			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Category("Format")]
		[Description("The font.")]
		public virtual Font FormatFont
		{
			get { return (this.font); }
			set
			{
				if ((this.font.Name  != value.Name) ||
					(this.font.Size  != value.Size) ||
					(this.font.Style != value.Style))
				{
					this.font = new Font(value.Name, value.Size, value.Style);

					SetControls();
					OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Font FormatFontWithoutStyle
		{
			set
			{
				if ((this.font.Name != value.Name) ||
					(this.font.Size != value.Size))
				{
					this.font = new Font(value.Name, value.Size, this.font.Style);

					SetControls();
					OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("Format")]
		[Description("The font style.")]
		[DefaultValue(FontFormat.StyleDefault)]
		public virtual FontStyle FormatFontStyle
		{
			get { return (this.font.Style); }
			set
			{
				Bold      = ((value & FontStyle.Bold)      != 0);
				Italic    = ((value & FontStyle.Italic)    != 0);
				Underline = ((value & FontStyle.Underline) != 0);
				Strikeout = ((value & FontStyle.Strikeout) != 0);
			}
		}

		private bool Bold
		{
			get { return (this.font.Bold); }
			set
			{
				FontStyle style = this.font.Style;
				if (((style & FontStyle.Bold) != 0) != value)
				{
					if (value)
						style |= FontStyle.Bold;
					else
						style &= ~FontStyle.Bold;

					this.font = new Font(this.font.Name, this.font.Size, style);

					SetControls();
					OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		private bool Italic
		{
			get { return (this.font.Italic); }
			set
			{
				FontStyle style = this.font.Style;
				if (((style & FontStyle.Italic) != 0) != value)
				{
					if (value)
						style |= FontStyle.Italic;
					else
						style &= ~FontStyle.Italic;

					this.font = new Font(this.font.Name, this.font.Size, style);

					SetControls();
					OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		private bool Underline
		{
			get { return (this.font.Underline); }
			set
			{
				FontStyle style = this.font.Style;
				if (((style & FontStyle.Underline) != 0) != value)
				{
					if (value)
						style |= FontStyle.Underline;
					else
						style &= ~FontStyle.Underline;

					this.font = new Font(this.font.Name, this.font.Size, style);

					SetControls();
					OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		private bool Strikeout
		{
			get { return (this.font.Strikeout); }
			set
			{
				FontStyle style = this.font.Style;
				if (((style & FontStyle.Strikeout) != 0) != value)
				{
					if (value)
						style |= FontStyle.Strikeout;
					else
						style &= ~FontStyle.Strikeout;

					this.font = new Font(this.font.Name, this.font.Size, style);

					SetControls();
					OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		/// <remarks>
		/// The default is the value of the <see cref="Control.DefaultForeColor"/> property.
		/// </remarks>
		[Category("Format")]
		[Description("The format color.")]
		public virtual Color FormatColor
		{
			get { return (this.color); }
			set
			{
				if (this.color != value)
				{
					this.color = value;

					SetControls();
					OnFormatChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the set of custom colors shown in the dialog box.
		/// </summary>
		/// <returns>
		/// A set of custom colors shown by the dialog box. The default value is null.
		/// </returns>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Same type as ColorDialog.CustomColors.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int[] CustomColors
		{
			get { return (this.customColors); }
			set
			{
				if (!ArrayEx.ValuesEqual(this.customColors, value))
				{
					this.customColors = value;

					SetControls();
					OnCustomColorsChanged(EventArgs.Empty);
				}
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_Bold_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			Bold = checkBox_Bold.Checked;
		}

		private void checkBox_Italic_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			Italic = checkBox_Italic.Checked;
		}

		private void checkBox_Underline_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			Underline = checkBox_Underline.Checked;
		}

		private void checkBox_Strikeout_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			Strikeout = checkBox_Strikeout.Checked;
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Color_Click(object sender, EventArgs e)
		{
			colorDialog.CustomColors = CustomColors;
			colorDialog.Color        = FormatColor;
			if (colorDialog.ShowDialog(this) == DialogResult.OK)
			{
				FormatColor  = colorDialog.Color;
				CustomColors = colorDialog.CustomColors;
			}
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				checkBox_Bold.Checked      = Bold;
				checkBox_Italic.Checked    = Italic;
				checkBox_Underline.Checked = Underline;
				checkBox_Strikeout.Checked = Strikeout;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnFormatChanged(EventArgs e)
		{
			EventHelper.RaiseSync(FormatChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnCustomColorsChanged(EventArgs e)
		{
			EventHelper.RaiseSync(CustomColorsChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
