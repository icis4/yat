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
// YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YAT.Gui.Utilities
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "No need to serialize/deserialize, not marking this class eliminates the requirement to implement the ISerialzable constructors.")]
	public class AsciiTableDataSet : DataSet
	{
		private const string Dec = "Dec";
		private const string Hex = "Hex";
		private const string Mnemonic = "Mnemonic";
		private const string Escape = "Esc";
		private const string Description = "Description";

		/// <summary></summary>
		public AsciiTableDataSet()
		{
			DataTable t = new DataTable("ASCII");
			t.Locale = CultureInfo.InvariantCulture;

			DataColumn c;
			DataRow r;

			c = new DataColumn(Dec);
			c.DataType = typeof(string);
			c.MaxLength = 3;
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

			for (byte i = 0; i <= 0x1F; i++)
			{
				r = t.NewRow();
				r[Dec] = i.ToString("D",  NumberFormatInfo.InvariantInfo);
				r[Hex] = i.ToString("X2", NumberFormatInfo.InvariantInfo);
				r[Mnemonic]    = MKY.Text.Ascii.ConvertToMnemonic(i);
				r[Escape]      = MKY.Text.Escape.ConvertToEscapeSequence(i);
				r[Description] = MKY.Text.Ascii.ConvertToDescription(i);
				t.Rows.Add(r);
			}
			for (byte i = 0x7F; i <= 0x7F; i++)
			{
				r = t.NewRow();
				r[Dec] = i.ToString("D",  NumberFormatInfo.InvariantInfo);
				r[Hex] = i.ToString("X2", NumberFormatInfo.InvariantInfo);
				r[Mnemonic]    = MKY.Text.Ascii.ConvertToMnemonic(i);
				r[Escape]      = MKY.Text.Escape.ConvertToEscapeSequence(i);
				r[Description] = MKY.Text.Ascii.ConvertToDescription(i);
				t.Rows.Add(r);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
