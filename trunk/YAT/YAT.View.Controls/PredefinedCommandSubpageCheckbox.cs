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

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

using MKY;

using YAT.Model.Types;

namespace YAT.View.Controls
{
	/// <summary>
	/// YAT predefined command subpage variant of <see cref="CheckBox"/>.
	/// </summary>
	public class PredefinedCommandSubpageCheckBox : CheckBox
	{
		private const int SubpageDefault = PredefinedCommandPage.FirstSubpageId;

		private int subpage = SubpageDefault;

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Raises the <see cref="M:ButtonBase.OnPaint(System.Windows.Forms.PaintEventArgs)" /> event.
		/// </summary>
		/// <param name="pevent">A <see cref="T:PaintEventArgs" /> that contains the event data.</param>
		protected override void OnPaint(PaintEventArgs pevent)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;

				SetControls(); // Required to properly draw/show also in case of default values.
			}

			base.OnPaint(pevent);
		}

		/// <remarks>Overridden to hide in designer since fixed by code.</remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text
		{
			get { return base.Text;  }
			set { base.Text = value; }
		}

		/// <summary></summary>
		[Category("Behavior")]
		[Description("The represented subpage.")]
		[DefaultValue(SubpageDefault)]
		public virtual int Subpage
		{
			get { return (this.subpage); }
			set
			{
				this.subpage = value;
				SetControls();
			}
		}

		private void SetControls()
		{
			switch (this.subpage)
			{
				case 1: Text =  "1..12";  break;
				case 2: Text = "13..24";  break;
				case 3: Text = "25..36";  break;
				case 4: Text = "37..48";  break;
				case 5: Text = "49..64";  break;
				case 6: Text = "65..72";  break;
				case 7: Text = "73..84";  break;
				case 8: Text = "85..96";  break;
				case 9: Text = "97..108"; break;

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + this.subpage.ToString(CultureInfo.InvariantCulture) + "' is a subpage value that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
