using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Event;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("FormatChanged")]
	public partial class TextFormat : System.Windows.Forms.UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const bool _BoldDefault = false;
		private const bool _ItalicDefault = false;
		private const bool _UnderlineDefault = false;
		private const bool _StrikeoutDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isSettingControls = false;

		private Font _font = new Font(Model.Types.FontFormat.NameDefault, Model.Types.FontFormat.SizeDefault, Model.Types.FontFormat.StyleDefault);
		private Color _color = Color.Black;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when any of the text format properties is changed.")]
		public event EventHandler FormatChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public TextFormat()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[Category("Format")]
		[Description("The font.")]
		public Font FormatFont
		{
			get { return (_font); }
			set
			{
				_font = new Font(value.Name, value.Size, value.Style);
				SetControls();
				OnFormatChanged(new EventArgs());
			}
		}

		[Browsable(false)]
		public Font FormatFontWithoutStyle
		{
			set
			{
				_font = new Font(value.Name, value.Size, _font.Style);
				SetControls();
				OnFormatChanged(new EventArgs());
			}
		}

		[Category("Format")]
		[Description("The font style.")]
		[DefaultValue(Model.Types.FontFormat.StyleDefault)]
		public FontStyle FormatFontStyle
		{
			get { return (_font.Style); }
			set
			{
				Bold = ((value & FontStyle.Bold) == FontStyle.Bold);
				Italic = ((value & FontStyle.Italic) == FontStyle.Italic);
				Underline = ((value & FontStyle.Underline) == FontStyle.Underline);
				Strikeout = ((value & FontStyle.Strikeout) == FontStyle.Strikeout);
			}
		}

		[Category("Format")]
		[Description("Bold.")]
		[DefaultValue(_BoldDefault)]
		private bool Bold
		{
			get { return (_font.Bold); }
			set
			{
				FontStyle style = _font.Style;
				if (value)
					style |= FontStyle.Bold;
				else
					style &= ~FontStyle.Bold;
				_font = new Font(_font.Name, _font.Size, style);
				SetControls();
				OnFormatChanged(new EventArgs());
			}
		}

		[Category("Format")]
		[Description("Italic.")]
		[DefaultValue(_ItalicDefault)]
		private bool Italic
		{
			get { return (_font.Italic); }
			set
			{
				FontStyle style = _font.Style;
				if (value)
					style |= FontStyle.Italic;
				else
					style &= ~FontStyle.Italic;
				_font = new Font(_font.Name, _font.Size, style);
				SetControls();
				OnFormatChanged(new EventArgs());
			}
		}

		[Category("Format")]
		[Description("Underline.")]
		[DefaultValue(_UnderlineDefault)]
		private bool Underline
		{
			get { return (_font.Underline); }
			set
			{
				FontStyle style = _font.Style;
				if (value)
					style |= FontStyle.Underline;
				else
					style &= ~FontStyle.Underline;
				_font = new Font(_font.Name, _font.Size, style);
				SetControls();
				OnFormatChanged(new EventArgs());
			}
		}

		[Category("Format")]
		[Description("Strikeout.")]
		[DefaultValue(_StrikeoutDefault)]
		private bool Strikeout
		{
			get { return (_font.Strikeout); }
			set
			{
				FontStyle style = _font.Style;
				if (value)
					style |= FontStyle.Strikeout;
				else
					style &= ~FontStyle.Strikeout;
				_font = new Font(_font.Name, _font.Size, style);
				SetControls();
				OnFormatChanged(new EventArgs());
			}
		}

		[Category("Format")]
		[Description("The color.")]
		public Color FormatColor
		{
			get { return (_color); }
			set
			{
				_color = value;
				SetControls();
				OnFormatChanged(new EventArgs());
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_Bold_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				Bold = checkBox_Bold.Checked;
		}

		private void checkBox_Italic_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				Italic = checkBox_Italic.Checked;
		}

		private void checkBox_Underline_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				Underline = checkBox_Underline.Checked;
		}

		private void checkBox_Strikeout_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				Strikeout = checkBox_Strikeout.Checked;
		}

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
			_isSettingControls = true;

			checkBox_Bold.Checked = Bold;
			checkBox_Italic.Checked = Italic;
			checkBox_Underline.Checked = Underline;
			checkBox_Strikeout.Checked = Strikeout;

			_isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnFormatChanged(EventArgs e)
		{
			EventHelper.FireSync(FormatChanged, this, e);
		}

		#endregion
	}
}
