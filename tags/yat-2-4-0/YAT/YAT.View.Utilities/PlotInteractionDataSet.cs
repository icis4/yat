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
	public class PlotInteractionDataSet : DataSet
	{
		private const string Function = "Function";
		private const string Scope    = "Scope";
		private const string Arrow    = "Arrow";
		private const string Modifier = "Modifier";
		private const string Input    = "Input";

		/// <summary></summary>
		public PlotInteractionDataSet()
		{
			AddByFunctionTable();
			AddByInputTable();
		}

		/// <summary></summary>
		private void AddByFunctionTable()
		{
			DataTable t = new DataTable("ByFunction");
			t.Locale = CultureInfo.CurrentCulture;

			DataColumn c;
			DataRow r;

			c = new DataColumn(Function);
			c.DataType = typeof(string);
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Scope);
			c.DataType = typeof(string);
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Arrow);
			c.DataType = typeof(string);
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Modifier);
			c.DataType = typeof(string);
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Input);
			c.DataType = typeof(string);
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			Tables.Add(t);

			var content = new string[][]
			{
				new string [] { "",                "",           "",   "",             ""                        },
				new string [] { "Track",           "Series",     "→",  "",             "Mouse Hover"             }, // Ignore 'Touch', user likely finds out.
				new string [] { "      Line",      "",           "→",  "[Ctrl] +",     "Left Mouse Button"       },
				new string [] { "      Points",    "",           "→",  "[Shift] +",    "Left Mouse Button"       },
				new string [] { "",                "",           "",   "",             ""                        },
				new string [] { "Pan",             "Plot, Axes", "→",  "",             "Left Mouse Button"       }, // Ignore 'Touch', user likely finds out.
				new string [] { "      Coarse",    "",           "→",  "",             "[Left/Right/Up/Down]"    },
				new string [] { "      Fine",      "",           "→",  "[Ctrl] +",     "[Left/Right/Up/Down]"    },
				new string [] { "",                "",           "",   "",             ""                        },
				new string [] { "Zoom",            "Plot, Axes", "→",  "",             "Mouse Wheel"             }, // Ignore 'Touch', user likely finds out.
				new string [] { "",                "",           "→",  "",             "[+/-], [Page Up/Down]"   },
				new string [] { "      Fine",      "",           "→",  "",             "Mouse Button 4/5"        },
				new string [] { "",                "",           "→",  "[Ctrl] +",     "Mouse Wheel"             },
				new string [] { "",                "",           "→",  "[Ctrl] +",     "[+/-], [Page Up/Down]"   },
				new string [] { "      Rectangle", "",           "→",  "",             "Middle Mouse Button"     },
				new string [] { "",                "",           "→",  "[Ctrl] +",     "Right Mouse Button"      },
				new string [] { "",                "",           "→",  "[Ctrl+Alt] +", "Left Mouse Button"       },
				new string [] { "",                "",           "",   "",             ""                        },
				new string [] { "Reset",           "Plot, Axes", "→",  "",             "2 x Middle Mouse Button" },
				new string [] { "",                "",           "→",  "[Ctrl] +",     "2 x Right Mouse Button"  },
				new string [] { "",                "",           "→",  "[Ctrl+Alt] +", "2 x Left Mouse Button"   },
				new string [] { "",                "",           "→",  "",             "[A], [Home]"             }, // Ignore 'Shake', unlikely to be used.
				new string [] { "",                "",           "",   "",             ""                        },
				new string [] { "Menu",            "",           "→",  "",             "Right Mouse Button"      }
			};

			for (int i = 0; i < content.Length; i++)
			{
				r = t.NewRow();
				r[Function] = content[i][0];
				r[Scope]    = content[i][1];
				r[Arrow]    = content[i][2];
				r[Modifier] = content[i][3];
				r[Input]    = content[i][4];
				t.Rows.Add(r);
			}
		}

		/// <summary></summary>
		private void AddByInputTable()
		{
			DataTable t = new DataTable("ByInput");
			t.Locale = CultureInfo.CurrentCulture;

			DataColumn c;
			DataRow r;

			c = new DataColumn(Function);
			c.DataType = typeof(string);
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Scope);
			c.DataType = typeof(string);
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Arrow);
			c.DataType = typeof(string);
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Modifier);
			c.DataType = typeof(string);
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			c = new DataColumn(Input);
			c.DataType = typeof(string);
			c.DefaultValue = "";
			c.ReadOnly = true;
			t.Columns.Add(c);

			Tables.Add(t);

			var content = new string[][]
			{
				new string [] { "",             "",                        "",   "",               ""           },
				new string [] { "",             "Mouse Hover",             "→",  "Track Snap",     "Series"     },
				new string [] { "",             "",                        "",   "",               ""           },
				new string [] { "",             "Left Mouse Button",       "→",  "Pan",            "Plot, Axes" },
				new string [] { "[Shift] +",    "Left Mouse Button",       "→",  "Track Points",   "Series"     },
				new string [] { "[Ctrl] +",     "Left Mouse Button",       "→",  "Track Line",     "Series"     },
				new string [] { "[Ctrl+Alt] +", "Left Mouse Button",       "→",  "Zoom Rectangle", "Plot, Axes" },
				new string [] { "[Ctrl+Alt] +", "2 x Left Mouse Button",   "→",  "Reset",          "Plot, Axes" },
				new string [] { "",             "Middle Mouse Button",     "→",  "Zoom Rectangle", "Plot, Axes" },
				new string [] { "",             "2 x Middle Mouse Button", "→",  "Reset",          "Plot, Axes" },
				new string [] { "",             "Right Mouse Button",      "→",  "Menu",           ""           },
				new string [] { "[Ctrl] +",     "Right Mouse Button",      "→",  "Zoom Rectangle", "Plot, Axes" },
				new string [] { "[Ctrl] +",     "2 x Right Mouse Button",  "→",  "Reset",          "Plot, Axes" },
				new string [] { "",             "Mouse Button 4/5",        "→",  "Zoom Fine",      "Plot, Axes" },
				new string [] { "",             "",                        "",   "",               "Plot, Axes" },
				new string [] { "",             "Mouse Wheel",             "→",  "Zoom Coarse",    "Plot, Axes" },
				new string [] { "[Ctrl] +",     "Mouse Wheel",             "→",  "Zoom Fine",      "Plot, Axes" },
				new string [] { "",             "",                        "",   "",               ""           },
				new string [] { "",             "[Left/Right/Up/Down]",    "→",  "Pan Coarse",     "Plot, Axes" },
				new string [] { "[Ctrl] +",     "[Left/Right/Up/Down]",    "→",  "Pan Fine",       "Plot, Axes" },
				new string [] { "",             "[+/-], [Page Up/Down]",   "→",  "Zoom Coarse",    "Plot, Axes" },
				new string [] { "[Ctrl] +",     "[+/-], [Page Up/Down]",   "→",  "Zoom Fine",      "Plot, Axes" },
				new string [] { "",             "",                        "",   "",               ""           },
				new string [] { "",             "[A], [Home]",             "→",  "Reset",          "Plot, Axes" },
				//// Ignore 'Touch', user likely finds out.
				//// Ignore 'Shake', unlikely to be used.
			};

			for (int i = 0; i < content.Length; i++)
			{
				r = t.NewRow();
				r[Modifier] = content[i][0];
				r[Input]    = content[i][1];
				r[Arrow]    = content[i][2];
				r[Function] = content[i][3];
				r[Scope]    = content[i][4];
				t.Rows.Add(r);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
