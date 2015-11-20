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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DefaultEvent("FormatChanged")]
	public partial class TextFormat : System.Windows.Forms.UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const bool BoldDefault = false;
		private const bool ItalicDefault = false;
		private const bool UnderlineDefault = false;
		private const bool StrikeoutDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Font font = new Font(Model.Types.FontFormat.NameDefault, Model.Types.FontFormat.SizeDefault, Model.Types.FontFormat.StyleDefault);
		private Color color = Color.Black;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when any of the text format properties is changed.")]
		public event EventHandler FormatChanged;

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
		public Font FormatFont
		{
			get { return (this.font); }
			set
			{
				this.font = new Font(value.Name, value.Size, value.Style);
				SetControls();
				OnFormatChanged(EventArgs.Empty);
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Font FormatFontWithoutStyle
		{
			set
			{
				this.font = new Font(value.Name, value.Size, this.font.Style);
				SetControls();
				OnFormatChanged(EventArgs.Empty);
			}
		}

		/// <summary></summary>
		[Category("Format")]
		[Description("The font style.")]
		[DefaultValue(Model.Types.FontFormat.StyleDefault)]
		public FontStyle FormatFontStyle
		{
			get { return (this.font.Style); }
			set
			{
				Bold = ((value & FontStyle.Bold) == FontStyle.Bold);
				Italic = ((value & FontStyle.Italic) == FontStyle.Italic);
				Underline = ((value & FontStyle.Underline) == FontStyle.Underline);
				Strikeout = ((value & FontStyle.Strikeout) == FontStyle.Strikeout);
			}
		}

		/// <summary></summary>
		[Category("Format")]
		[Description("Bold.")]
		[DefaultValue(BoldDefault)]
		private bool Bold
		{
			get { return (this.font.Bold); }
			set
			{
				FontStyle style = this.font.Style;
				if (value)
					style |= FontStyle.Bold;
				else
					style &= ~FontStyle.Bold;
				this.font = new Font(this.font.Name, this.font.Size, style);
				SetControls();
				OnFormatChanged(EventArgs.Empty);
			}
		}

		/// <summary></summary>
		[Category("Format")]
		[Description("Italic.")]
		[DefaultValue(ItalicDefault)]
		private bool Italic
		{
			get { return (this.font.Italic); }
			set
			{
				FontStyle style = this.font.Style;
				if (value)
					style |= FontStyle.Italic;
				else
					style &= ~FontStyle.Italic;
				this.font = new Font(this.font.Name, this.font.Size, style);
				SetControls();
				OnFormatChanged(EventArgs.Empty);
			}
		}

		/// <summary></summary>
		[Category("Format")]
		[Description("Underline.")]
		[DefaultValue(UnderlineDefault)]
		private bool Underline
		{
			get { return (this.font.Underline); }
			set
			{
				FontStyle style = this.font.Style;
				if (value)
					style |= FontStyle.Underline;
				else
					style &= ~FontStyle.Underline;
				this.font = new Font(this.font.Name, this.font.Size, style);
				SetControls();
				OnFormatChanged(EventArgs.Empty);
			}
		}

		/// <summary></summary>
		[Category("Format")]
		[Description("Strikeout.")]
		[DefaultValue(StrikeoutDefault)]
		private bool Strikeout
		{
			get { return (this.font.Strikeout); }
			set
			{
				FontStyle style = this.font.Style;
				if (value)
					style |= FontStyle.Strikeout;
				else
					style &= ~FontStyle.Strikeout;
				this.font = new Font(this.font.Name, this.font.Size, style);
				SetControls();
				OnFormatChanged(EventArgs.Empty);
			}
		}

		/// <summary></summary>
		[Category("Format")]
		[Description("The color.")]
		public Color FormatColor
		{
			get { return (this.color); }
			set
			{
				this.color = value;
				SetControls();
				OnFormatChanged(EventArgs.Empty);
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_Bold_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				Bold = checkBox_Bold.Checked;
		}

		private void checkBox_Italic_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				Italic = checkBox_Italic.Checked;
		}

		private void checkBox_Underline_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				Underline = checkBox_Underline.Checked;
		}

		private void checkBox_Strikeout_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				Strikeout = checkBox_Strikeout.Checked;
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void button_Color_Click(object sender, EventArgs e)
		{
			ColorDialog cd = new ColorDialog();
			cd.Color = FormatColor;
			if (cd.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				FormatColor = cd.Color;
			}
		}

		private void SetControls()
		{
			this.isSettingControls.Enter();

			checkBox_Bold.Checked = Bold;
			checkBox_Italic.Checked = Italic;
			checkBox_Underline.Checked = Underline;
			checkBox_Strikeout.Checked = Strikeout;

			this.isSettingControls.Leave();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnFormatChanged(EventArgs e)
		{
			EventHelper.FireSync(FormatChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
