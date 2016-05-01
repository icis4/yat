//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
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
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY;

#endregion

namespace YAT.Model.Types
{
	#region Enum WorkspaceLayout

	/// <summary>
	/// Specifies the layout of multiple document interface (MDI) child windows in an MDI parent window.
	/// </summary>
	public enum WorkspaceLayout
	{
		/// <summary>
		/// All child windows are automatically positioned within the client region of the MDI parent form.
		/// </summary>
		Automatic = -1,

		/// <summary>
		/// All child windows are cascaded within the client region of the MDI parent form.
		/// </summary>
		Cascade = MdiLayout.Cascade, // = 0

		/// <summary>
		/// All child windows are tiled horizontally within the client region of the MDI parent form.
		/// </summary>
		TileHorizontal = MdiLayout.TileHorizontal, // = 1

		/// <summary>
		/// All child windows are tiled vertically within the client region of the MDI parent form.
		/// </summary>
		TileVertical = MdiLayout.TileVertical, // = 2

	/////// <summary>
	/////// All MDI child icons are arranged within the client region of the MDI parent form.
	/////// </summary>
	////ArrangeIcons = MdiLayout.ArrangeIcons, // = 3 is not supported by YAT.

		/// <summary>
		/// All child windows are manually positioned within the client region of the MDI parent form.
		/// </summary>
		Manual = 4,

		/// <summary>
		/// All child windows are minimized within the client region of the MDI parent form.
		/// </summary>
		Minimize = 5,

		/// <summary>
		/// All child windows are maximized within the client region of the MDI parent form.
		/// </summary>
		Maximize = 6
	}

	#endregion

	/// <summary>
	/// Extended enum WorkspaceLayoutEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Make sure to use the underlying enum for serialization.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class WorkspaceLayoutEx : EnumEx
	{
		#region String Definitions

		private const string Automatic_string      = "Automatic";
		private const string Cascade_string        = "Cascade";
		private const string TileHorizontal_string = "TileHorizontal";
		private const string TileVertical_string   = "TileVertical";
		private const string Manual_string         = "Manual";
		private const string Minimize_string       = "Minimize";
		private const string Maximize_string       = "Maximize";

		#endregion

		/// <summary>Default is <see cref="WorkspaceLayout.Automatic"/>.</summary>
		public WorkspaceLayoutEx()
			: this(WorkspaceLayout.Automatic)
		{
		}

		/// <summary></summary>
		public WorkspaceLayoutEx(WorkspaceLayout layout)
			: base(layout)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((WorkspaceLayout)UnderlyingEnum)
			{
				case WorkspaceLayout.Automatic:      return (Automatic_string);
				case WorkspaceLayout.Cascade:        return (Cascade_string);
				case WorkspaceLayout.TileHorizontal: return (TileHorizontal_string);
				case WorkspaceLayout.TileVertical:   return (TileVertical_string);
				case WorkspaceLayout.Manual:         return (Manual_string);
				case WorkspaceLayout.Minimize:       return (Minimize_string);
				case WorkspaceLayout.Maximize:       return (Maximize_string);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enums is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static WorkspaceLayoutEx[] GetItems()
		{
			List<WorkspaceLayoutEx> a = new List<WorkspaceLayoutEx>(7); // Preset the required capactiy to improve memory management.
			a.Add(new WorkspaceLayoutEx(WorkspaceLayout.Automatic));
			a.Add(new WorkspaceLayoutEx(WorkspaceLayout.Cascade));
			a.Add(new WorkspaceLayoutEx(WorkspaceLayout.TileHorizontal));
			a.Add(new WorkspaceLayoutEx(WorkspaceLayout.TileVertical));
			a.Add(new WorkspaceLayoutEx(WorkspaceLayout.Manual));
			a.Add(new WorkspaceLayoutEx(WorkspaceLayout.Minimize));
			a.Add(new WorkspaceLayoutEx(WorkspaceLayout.Maximize));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static WorkspaceLayoutEx Parse(string s)
		{
			WorkspaceLayoutEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid workspace layout string!"));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out WorkspaceLayoutEx result)
		{
			WorkspaceLayout enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = enumResult;
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
		public static bool TryParse(string s, out WorkspaceLayout result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Automatic_string))
			{
				result = WorkspaceLayout.Automatic;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Cascade_string))
			{
				result = WorkspaceLayout.Cascade;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, TileHorizontal_string))
			{
				result = WorkspaceLayout.TileHorizontal;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, TileVertical_string))
			{
				result = WorkspaceLayout.TileVertical;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Manual_string))
			{
				result = WorkspaceLayout.Manual;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Minimize_string))
			{
				result = WorkspaceLayout.Minimize;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Maximize_string))
			{
				result = WorkspaceLayout.Maximize;
				return (true);
			}
			else if (string.IsNullOrEmpty(s))
			{
				result = new WorkspaceLayoutEx(); // Default!
				return (true); // Default silently, could e.g. happen when deserializing an XML.
			}
			else // = invalid string!
			{
				result = new WorkspaceLayoutEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator WorkspaceLayout(WorkspaceLayoutEx layout)
		{
			return ((WorkspaceLayout)layout.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator WorkspaceLayoutEx(WorkspaceLayout layout)
		{
			return (new WorkspaceLayoutEx(layout));
		}

		/// <summary></summary>
		public static implicit operator int(WorkspaceLayoutEx layout)
		{
			return (layout.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator WorkspaceLayoutEx(int layout)
		{
			return (new WorkspaceLayoutEx((WorkspaceLayout)layout));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public static implicit operator MdiLayout(WorkspaceLayoutEx layout)
		{
			switch ((WorkspaceLayout)layout.UnderlyingEnum)
			{
				case WorkspaceLayout.Automatic:      throw (new NotSupportedException("'Automatic' is not supported by 'Windows.Forms.MdiLayout'." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				case WorkspaceLayout.Cascade:        return (MdiLayout.Cascade);
				case WorkspaceLayout.TileHorizontal: return (MdiLayout.TileHorizontal);
				case WorkspaceLayout.TileVertical:   return (MdiLayout.TileVertical);
				case WorkspaceLayout.Manual:         throw (new NotSupportedException("'Manual' is not supported by 'Windows.Forms.MdiLayout'." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				case WorkspaceLayout.Minimize:       throw (new NotSupportedException("'Minimize' is not supported by 'Windows.Forms.MdiLayout'." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				case WorkspaceLayout.Maximize:       throw (new NotSupportedException("'Maximize' is not supported by 'Windows.Forms.MdiLayout'." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                             throw (new NotSupportedException("Program execution should never get here,'" + (WorkspaceLayout)layout.UnderlyingEnum + "' is an unknown workspace layout." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public static implicit operator WorkspaceLayoutEx(MdiLayout layout)
		{
			switch (layout)
			{
				case MdiLayout.Cascade:        return (new WorkspaceLayoutEx(WorkspaceLayout.Cascade));
				case MdiLayout.TileHorizontal: return (new WorkspaceLayoutEx(WorkspaceLayout.TileHorizontal));
				case MdiLayout.TileVertical:   return (new WorkspaceLayoutEx(WorkspaceLayout.TileVertical));
				case MdiLayout.ArrangeIcons:   throw (new NotSupportedException("'ArrangeIcons' is not supported by " + ApplicationEx.ProductName + "." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                       throw (new NotSupportedException("Program execution should never get here,'" + layout + "' is an unknown workspace layout." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
