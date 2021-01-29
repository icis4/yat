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
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YAT.View.Utilities
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "No need to serialize/deserialize, not marking this class eliminates the requirement to implement the ISerializable constructors.")]
	public class AsciiTableDataSet : DataSet
	{
		private const string Dec         = "Dec";
		private const string Hex         = "Hex";
		private const string Mnemonic    = "Mnemonic";
		private const string Escape      = "Esc";
		private const string Description = "Description";

		/// <summary></summary>
		public AsciiTableDataSet()
		{
			AddAsciiTable();
		}

		/// <summary></summary>
		private void AddAsciiTable()
		{
			DataTable t = new DataTable("ASCII");
			t.Locale = CultureInfo.CurrentCulture;

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
				r[Dec] = i.ToString("D",  CultureInfo.InvariantCulture); // 'InvariantCulture' since this is a technical table.
				r[Hex] = i.ToString("X2", CultureInfo.InvariantCulture); // 'InvariantCulture' since this is a technical table.
				r[Mnemonic]    = MKY.Text.Ascii.ConvertToMnemonic(i);
				r[Escape]      = MKY.Text.Escape.ConvertToEscapeSequence(i);
				r[Description] = MKY.Text.Ascii.ConvertToDescription(i);
				t.Rows.Add(r);
			}
			for (byte i = 0x7F; i <= 0x7F; i++)
			{
				r = t.NewRow();
				r[Dec] = i.ToString("D",  CultureInfo.InvariantCulture); // 'InvariantCulture' since this is a technical table.
				r[Hex] = i.ToString("X2", CultureInfo.InvariantCulture); // 'InvariantCulture' since this is a technical table.
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
