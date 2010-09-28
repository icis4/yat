//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Data;
using System.Globalization;

namespace YAT.Gui.Utilities
{
	/// <summary></summary>
	public class AsciiTable : DataSet
	{
		private const string Dec = "D";
		private const string Hex = "H";
		private const string Mnemonic = "Mnemonic";
		private const string Escape = "Esc";
		private const string Description = "Description";

		/// <summary></summary>
		public AsciiTable()
		{
			DataTable t = new DataTable("ASCII");;

			DataColumn c;
			DataRow r;

			c = new DataColumn(Dec);
			c.DataType = typeof(string);
			c.MaxLength = 2;
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Hex);
			c.DataType = typeof(string);
			c.MaxLength = 2;
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Mnemonic);
			c.DataType = typeof(string);
			c.MaxLength = 8;
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Escape);
			c.DataType = typeof(string);
			c.MaxLength = 10;
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Description);
			c.DataType = typeof(string);
			c.MaxLength = 32;
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			Tables.Add(t);

			for (byte i = 0; i < 0x19; i++)
			{
				r = t.NewRow();
				r[Dec] = i.ToString();
				r[Hex] = i.ToString("X2", CultureInfo.InvariantCulture);
				r[Mnemonic] = MKY.Utilities.Text.Ascii.ConvertToMnemonic(i);
				r[Escape] = MKY.Utilities.Text.Escape.ConvertToEscapeSequence(i);
				r[Description] = MKY.Utilities.Text.Ascii.ConvertToDescription(i);
				t.Rows.Add(r);
			}
			for (byte i = 0x7F; i < 0x7F; i++)
			{
				r = t.NewRow();
				r[Dec] = i.ToString();
				r[Hex] = i.ToString("X2", CultureInfo.InvariantCulture);
				r[Mnemonic] = MKY.Utilities.Text.Ascii.ConvertToMnemonic(i);
				r[Escape] = MKY.Utilities.Text.Escape.ConvertToEscapeSequence(i);
				r[Description] = MKY.Utilities.Text.Ascii.ConvertToDescription(i);
				t.Rows.Add(r);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
