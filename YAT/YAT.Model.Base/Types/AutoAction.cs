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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

namespace YAT.Model.Types
{
	#region Enum AutoAction

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum AutoAction
	{
		None,

		Highlight,
		Filter,
		Suppress,

		Beep,
		ShowMessageBox,

		PlotByteCountRate,
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "PlotLine", Justification = "Orthogonality with 'PlotByte' above.")]
		PlotLineCountRate,
		LineChartIndex,
		LineChartTime,
		LineChartTimeStamp,
		ScatterPlot,         // Located right after 'LineChartTime*' since they are technically also scatter plots.
		HistogramHorizontal, // Opposed to histograms, which are bar plots.
		HistogramVertical,

		ClearRepositories,
		ClearRepositoriesOnSubsequentRx,
		ResetCountAndRate,
		SwitchLogOn,
		SwitchLogOff,
		ToggleLogOnOrOff,
		Stop,
		CloseTerminal,
		ExitApplication
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum AutoActionEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class AutoActionEx : EnumEx, IEquatable<AutoActionEx>
	{
		#region String Definitions
		                                 // Attention:
		                                 // These strings are used for XML serialization!
		                                 // Not ideal, but required for explicit 'AutoTrigger' and 'AutoResponse', thus here too.
		private const string             None_string                = "[None]";
		private const string             Highlight_string           = "[Highlight]";               //// Old name, needed for backward compatibility with old settings!
		private static readonly string[] Highlight_stringAlternatives = new string[] { "[Highlight Only]" };
		private const string             Filter_string              = "[Filter]";
		private const string             Suppress_string            = "[Suppress]";
		private const string             Beep_string                = "[Beep]";
		private const string             ShowMessageBox_string      = "[Show Message Box]";
		private const string             PlotByteCountRate_string   = "[Plot Byte Count/Rate]";
		private const string             PlotLineCountRate_string   = "[Plot Line Count/Rate]";
		private const string             LineChartIndex_string      = "[Line Chart]";
		private const string             LineChartTime_string       = "[Line Chart (X = Time)]";
		private const string             LineChartTimeStamp_string  = "[Line Chart (X = Time Stamp)]";
		private const string             ScatterPlot_string         = "[Scatter Plot]";
		private const string             HistogramHorizontal_string = "[Histogram (Horizontal)]";
		private const string             HistogramVertical_string   = "[Histogram (Vertical)]";
		private const string             ClearRepositories_string   = "[Clear Monitor]";                           // Translating from code to user terminology.
		private const string             ClearRepositoriesOnSubsequentRx_string = "[Clear Mon. on Subsequent Rx]"; // Translating from code to user terminology.
		private static readonly string[] ClearRepositoriesOnSubsequentRx_stringAlternativeStarts = new string[] { "[Clear Mon. ", "[Clear Monitor " }; // Including ' ' to distinguish from [Clear Monitor]!
		private const string             ResetCountAndRate_string   = "[Reset Count/Rate]";                                                  //// Old name, needed for backward compatibility with old settings!
		private const string             SwitchLogOn_string         = "[Log On]";                                  // Translating from code to user terminology.
		private const string             SwitchLogOff_string        = "[Log Off]";                                 // Translating from code to user terminology.
		private const string             ToggleLogOnOrOff_string    = "[Toggle Log On/Off]";                       // Translating from code to user terminology.
		private const string             Stop_string                = "[Close/Stop I/O]";                          // Translating from code to user terminology.
		private const string             CloseTerminal_string       = "[Close Terminal]";
		private static readonly string   ExitApplication_string     = "[Exit " + ApplicationEx.ProductName + "]"; // "YAT" or "YATConsole", as indicated in main title bar.
		private const string             ExitApplication_stringAlternativeStart = "[Exit " + ApplicationEx.CommonName;

		#endregion

		/// <summary>Default is <see cref="AutoAction.None"/>.</summary>
		public const AutoAction Default = AutoAction.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public AutoActionEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public AutoActionEx(AutoAction action)
			: base(action)
		{
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public bool IsActive
		{
			get { return ((AutoAction)UnderlyingEnum != AutoAction.None); }
		}

		/// <summary></summary>
		public bool ShallHighlight
		{
			get
			{
				switch ((AutoAction)UnderlyingEnum)
				{
					// Explicitly list those actions that shall not highlight...
					case AutoAction.None:
					case AutoAction.Filter:
					case AutoAction.Suppress:
					case AutoAction.PlotByteCountRate:
					case AutoAction.PlotLineCountRate:
					case AutoAction.LineChartIndex:
					case AutoAction.LineChartTime:
					case AutoAction.LineChartTimeStamp:
					case AutoAction.ScatterPlot:
					case AutoAction.HistogramHorizontal:
					case AutoAction.HistogramVertical:
						return (false);

					default: // ...all others shall:
						return (true);
				}
			}
		}

		/// <summary></summary>
		public bool AlsoAppliesToTx
		{
			get
			{
				switch ((AutoAction)UnderlyingEnum)
				{
					// Explicitly list those actions that apply...
					case AutoAction.PlotByteCountRate:
					case AutoAction.PlotLineCountRate:
						return (true);

					default: // ...all others don't:
						return (false);
				}
			}
		}

		/// <summary></summary>
		public bool IsCountRatePlot
		{
			get
			{
				switch ((AutoAction)UnderlyingEnum)
				{
					case AutoAction.PlotByteCountRate:
					case AutoAction.PlotLineCountRate:
						return (true);

					default:
						return (false);
				}
			}
		}

		/// <summary></summary>
		public bool IsLineScatterHistogramPlot
		{
			get
			{
				switch ((AutoAction)UnderlyingEnum)
				{
					case AutoAction.LineChartIndex:
					case AutoAction.LineChartTime:
					case AutoAction.LineChartTimeStamp:
					case AutoAction.ScatterPlot:
					case AutoAction.HistogramHorizontal:
					case AutoAction.HistogramVertical:
						return (true);

					default:
						return (false);
				}
			}
		}

		/// <summary></summary>
		public bool IsAnyPlot
		{
			get { return (IsCountRatePlot || IsLineScatterHistogramPlot); }
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public override string ToString()
		{
			switch ((AutoAction)UnderlyingEnum)
			{
				case AutoAction.None:                            return (None_string);

				case AutoAction.Highlight:                       return (Highlight_string);
				case AutoAction.Filter:                          return (Filter_string);
				case AutoAction.Suppress:                        return (Suppress_string);

				case AutoAction.Beep:                            return (Beep_string);
				case AutoAction.ShowMessageBox:                  return (ShowMessageBox_string);

				case AutoAction.PlotByteCountRate:               return (PlotByteCountRate_string);
				case AutoAction.PlotLineCountRate:               return (PlotLineCountRate_string);
				case AutoAction.LineChartIndex:                  return (LineChartIndex_string);
				case AutoAction.LineChartTime:                   return (LineChartTime_string);
				case AutoAction.LineChartTimeStamp:              return (LineChartTimeStamp_string);
				case AutoAction.ScatterPlot:                     return (ScatterPlot_string);
				case AutoAction.HistogramHorizontal:             return (HistogramHorizontal_string);
				case AutoAction.HistogramVertical:               return (HistogramVertical_string);

				case AutoAction.ClearRepositories:               return (ClearRepositories_string);
				case AutoAction.ClearRepositoriesOnSubsequentRx: return (ClearRepositoriesOnSubsequentRx_string);
				case AutoAction.ResetCountAndRate:               return (ResetCountAndRate_string);
				case AutoAction.SwitchLogOn:                     return (SwitchLogOn_string);
				case AutoAction.SwitchLogOff:                    return (SwitchLogOff_string);
				case AutoAction.ToggleLogOnOrOff:                return (ToggleLogOnOrOff_string);
				case AutoAction.Stop:                            return (Stop_string);
				case AutoAction.CloseTerminal:                   return (CloseTerminal_string);
				case AutoAction.ExitApplication:                 return (ExitApplication_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();
				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as AutoActionEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(AutoActionEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return (base.Equals(other));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(AutoActionEx lhs, AutoActionEx rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(AutoActionEx lhs, AutoActionEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static AutoActionEx[] GetItems()
		{
			var a = new List<AutoActionEx>(23); // Preset the required capacity to improve memory management.

			a.Add(new AutoActionEx(AutoAction.None));

			a.Add(new AutoActionEx(AutoAction.Highlight));
			a.Add(new AutoActionEx(AutoAction.Filter));
			a.Add(new AutoActionEx(AutoAction.Suppress));

			a.Add(new AutoActionEx(AutoAction.Beep));
			a.Add(new AutoActionEx(AutoAction.ShowMessageBox));

			a.Add(new AutoActionEx(AutoAction.PlotByteCountRate));
			a.Add(new AutoActionEx(AutoAction.PlotLineCountRate));
			a.Add(new AutoActionEx(AutoAction.LineChartIndex));
			a.Add(new AutoActionEx(AutoAction.LineChartTime));
			a.Add(new AutoActionEx(AutoAction.LineChartTimeStamp));
			a.Add(new AutoActionEx(AutoAction.ScatterPlot));
			a.Add(new AutoActionEx(AutoAction.HistogramHorizontal));
			a.Add(new AutoActionEx(AutoAction.HistogramVertical));

			a.Add(new AutoActionEx(AutoAction.ClearRepositories));
			a.Add(new AutoActionEx(AutoAction.ClearRepositoriesOnSubsequentRx));
			a.Add(new AutoActionEx(AutoAction.ResetCountAndRate));
			a.Add(new AutoActionEx(AutoAction.SwitchLogOn));
			a.Add(new AutoActionEx(AutoAction.SwitchLogOff));
			a.Add(new AutoActionEx(AutoAction.ToggleLogOnOrOff));
			a.Add(new AutoActionEx(AutoAction.Stop));
			a.Add(new AutoActionEx(AutoAction.CloseTerminal));
			a.Add(new AutoActionEx(AutoAction.ExitApplication));

			return (a.ToArray());
		}

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static AutoActionEx[] GetLineScatterHistogramPlotItems()
		{
			var a = new List<AutoActionEx>(6); // Preset the required capacity to improve memory management.

			a.Add(new AutoActionEx(AutoAction.LineChartIndex));
			a.Add(new AutoActionEx(AutoAction.LineChartTime));
			a.Add(new AutoActionEx(AutoAction.LineChartTimeStamp));
			a.Add(new AutoActionEx(AutoAction.ScatterPlot));
			a.Add(new AutoActionEx(AutoAction.HistogramHorizontal));
			a.Add(new AutoActionEx(AutoAction.HistogramVertical));

			return (a.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static AutoActionEx Parse(string s)
		{
			AutoActionEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid action string! String must be a valid action string, or one of the predefined actions."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out AutoActionEx result)
		{
			AutoAction enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new AutoActionEx(enumResult);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out AutoAction result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = AutoAction.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_string))
			{
				result = AutoAction.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, Highlight_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, Highlight_stringAlternatives))
			{
				result = AutoAction.Highlight;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Filter_string))
			{
				result = AutoAction.Filter;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Suppress_string))
			{
				result = AutoAction.Suppress;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Beep_string))
			{
				result = AutoAction.Beep;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ShowMessageBox_string))
			{
				result = AutoAction.ShowMessageBox;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, PlotByteCountRate_string))
			{
				result = AutoAction.PlotByteCountRate;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, PlotLineCountRate_string))
			{
				result = AutoAction.PlotLineCountRate;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LineChartIndex_string))
			{
				result = AutoAction.LineChartIndex;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LineChartTime_string))
			{
				result = AutoAction.LineChartTime;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, LineChartTimeStamp_string))
			{
				result = AutoAction.LineChartTimeStamp;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ScatterPlot_string))
			{
				result = AutoAction.ScatterPlot;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, HistogramHorizontal_string))
			{
				result = AutoAction.HistogramHorizontal;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, HistogramVertical_string))
			{
				result = AutoAction.HistogramVertical;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ClearRepositories_string))
			{
				result = AutoAction.ClearRepositories;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase       (s, ClearRepositoriesOnSubsequentRx_string) ||
			         StringEx.StartsWithAnyOrdinalIgnoreCase(s, ClearRepositoriesOnSubsequentRx_stringAlternativeStarts))
			{
				result = AutoAction.ClearRepositoriesOnSubsequentRx;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ResetCountAndRate_string))
			{
				result = AutoAction.ResetCountAndRate;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, SwitchLogOn_string))
			{
				result = AutoAction.SwitchLogOn;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, SwitchLogOff_string))
			{
				result = AutoAction.SwitchLogOff;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ToggleLogOnOrOff_string))
			{
				result = AutoAction.ToggleLogOnOrOff;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Stop_string))
			{
				result = AutoAction.Stop;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CloseTerminal_string))
			{
				result = AutoAction.CloseTerminal;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ExitApplication_string) ||
			         StringEx.StartsWithOrdinalIgnoreCase(s, ExitApplication_stringAlternativeStart))
			{
				result = AutoAction.ExitApplication;
				return (true);
			}
			else // Invalid string!
			{
				result = new AutoActionEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator AutoAction(AutoActionEx autoAction)
		{
			return ((AutoAction)autoAction.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator AutoActionEx(AutoAction autoAction)
		{
			return (new AutoActionEx(autoAction));
		}

		/// <summary></summary>
		public static implicit operator string(AutoActionEx autoAction)
		{
			return (autoAction.ToString());
		}

		/// <summary></summary>
		public static implicit operator AutoActionEx(string autoAction)
		{
			return (Parse(autoAction));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
