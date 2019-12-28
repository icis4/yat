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
// YAT Version 2.1.1 Development
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

namespace YAT.Model.Types
{
	#region Enum AutoActionPlot

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum AutoActionPlot
	{
		LineChartIndex,
		LineChartTime,
		ScatterPlotXY,
		ScatterPlotTime,
		Histogram
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum AutoActionPlotEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class AutoActionPlotEx : EnumEx, IEquatable<AutoActionPlotEx>
	{
		#region String Definitions

		private const string             LineChartIndex_string = "[Line Chart]";
		private static readonly string[] LineChartIndex_stringAlternatives = new string[] { "[LC]" };

		private const string             LineChartTime_string = "[Line Chart (Time)]";
		private static readonly string[] LineChartTime_stringAlternatives = new string[] { "[LCT]" };

		private const string             ScatterPlotXY_string = "[Scatter Plot]";
		private static readonly string[] ScatterPlotXY_stringAlternatives = new string[] { "[SP]" };

		private const string             ScatterPlotTime_string = "[Scatter Plot (Time)]";
		private static readonly string[] ScatterPlotTime_stringAlternatives = new string[] { "[SPT]" };

		private const string             Histogram_string = "[Histogram]";
		private static readonly string[] Histogram_stringAlternatives = new string[] { "[HG]" };

		#endregion

		/// <summary>Default is <see cref="AutoActionPlot.LineChartIndex"/>.</summary>
		public const AutoActionPlot Default = AutoActionPlot.LineChartIndex;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public AutoActionPlotEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public AutoActionPlotEx(AutoActionPlot action)
			: base(action)
		{
		}

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
			switch ((AutoActionPlot)UnderlyingEnum)
			{
				case AutoActionPlot.LineChartIndex:  return (LineChartIndex_string);
				case AutoActionPlot.LineChartTime:   return (LineChartTime_string);
				case AutoActionPlot.ScatterPlotXY:   return (ScatterPlotXY_string);
				case AutoActionPlot.ScatterPlotTime: return (ScatterPlotTime_string);
				case AutoActionPlot.Histogram:       return (Histogram_string);

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
			return (Equals(obj as AutoActionPlotEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(AutoActionPlotEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return (base.Equals(other));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(AutoActionPlotEx lhs, AutoActionPlotEx rhs)
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
		public static bool operator !=(AutoActionPlotEx lhs, AutoActionPlotEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static AutoActionPlotEx[] GetItems()
		{
			var a = new List<AutoActionPlotEx>(5); // Preset the required capacity to improve memory management.

			a.Add(new AutoActionPlotEx(AutoActionPlot.LineChartIndex));
			a.Add(new AutoActionPlotEx(AutoActionPlot.LineChartTime));
			a.Add(new AutoActionPlotEx(AutoActionPlot.ScatterPlotXY));
			a.Add(new AutoActionPlotEx(AutoActionPlot.ScatterPlotTime));
			a.Add(new AutoActionPlotEx(AutoActionPlot.Histogram));

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
		public static AutoActionPlotEx Parse(string s)
		{
			AutoActionPlotEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid action string! String must be a valid action string, or one of the predefined actions."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out AutoActionPlotEx result)
		{
			AutoActionPlot enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new AutoActionPlotEx(enumResult);
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
		public static bool TryParse(string s, out AutoActionPlot result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // Default!
			{
				result = new AutoActionPlotEx(); // Default!
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, LineChartIndex_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, LineChartIndex_stringAlternatives))
			{
				result = AutoActionPlot.LineChartIndex;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, LineChartTime_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, LineChartTime_stringAlternatives))
			{
				result = AutoActionPlot.LineChartTime;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, ScatterPlotXY_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ScatterPlotXY_stringAlternatives))
			{
				result = AutoActionPlot.ScatterPlotXY;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, ScatterPlotTime_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, ScatterPlotTime_stringAlternatives))
			{
				result = AutoActionPlot.ScatterPlotTime;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase   (s, Histogram_string) ||
			         StringEx.EqualsAnyOrdinalIgnoreCase(s, Histogram_stringAlternatives))
			{
				result = AutoActionPlot.Histogram;
				return (true);
			}
			else // Invalid string!
			{
				result = new AutoActionPlotEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator AutoActionPlot(AutoActionPlotEx autoAction)
		{
			return ((AutoActionPlot)autoAction.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator AutoActionPlotEx(AutoActionPlot autoAction)
		{
			return (new AutoActionPlotEx(autoAction));
		}

		/// <summary></summary>
		public static implicit operator string(AutoActionPlotEx autoAction)
		{
			return (autoAction.ToString());
		}

		/// <summary></summary>
		public static implicit operator AutoActionPlotEx(string autoAction)
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
