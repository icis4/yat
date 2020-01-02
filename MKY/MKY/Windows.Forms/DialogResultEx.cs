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
// MKY Version 1.0.28 Development
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
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using MKY.IO;

namespace MKY.Windows.Forms
{
	/// <summary></summary>
	[Serializable]
	public struct FilePathDialogResult : IEquatable<FilePathDialogResult>
	{
		/// <summary>
		/// Gets or sets the dialog result.
		/// </summary>
		/// <value>The dialog result.</value>
		[XmlElement("Result")]
		public DialogResult Result { get; set; }

		/// <summary>
		/// Gets the file path.
		/// </summary>
		/// <value>The file path.</value>
		[XmlElement("FilePath")]
		public string FilePath { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SaveAsDialogResult"/> struct.
		/// </summary>
		/// <param name="result">The dialog result.</param>
		public FilePathDialogResult(DialogResult result)
			: this (result, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SaveAsDialogResult"/> struct.
		/// </summary>
		/// <param name="result">The dialog result.</param>
		/// <param name="filePath">The file path.</param>
		public FilePathDialogResult(DialogResult result, string filePath)
		{
			Result   = result;
			FilePath = filePath;
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return (Result + " / " + FilePath);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = 0;

				hashCode = (hashCode * 397) ^  Result                     .GetHashCode();
				hashCode = (hashCode * 397) ^ (FilePath != null ? FilePath.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is FilePathDialogResult)
				return (Equals((FilePathDialogResult)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(FilePathDialogResult other)
		{
			return
			(
				Result.Equals(          other.Result) &&
				PathEx.Equals(FilePath, other.FilePath)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(FilePathDialogResult lhs, FilePathDialogResult rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(FilePathDialogResult lhs, FilePathDialogResult rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
