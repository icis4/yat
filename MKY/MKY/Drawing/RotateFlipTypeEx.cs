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
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace MKY.Drawing
{
	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary>
	/// This enum reduces <see cref="RotateFlipType"/> to rotation.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "These are NOT flags!")]
	public enum RotateType
	{
		RotateNone = RotateFlipType.RotateNoneFlipNone,
		Rotate90   = RotateFlipType.Rotate90FlipNone,
		Rotate180  = RotateFlipType.Rotate180FlipNone,
		Rotate270  = RotateFlipType.Rotate270FlipNone,
	}

	/// <summary>
	/// This class provides explicit conversion methods for <see cref="RotateType"/>.
	/// </summary>
	/// <remarks>
	/// The conversions are intentionally implemented using this helper class because this
	/// implementation is faster than using a <see cref="System.ComponentModel.TypeConverter"/>.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class RotateTypeEx
	{
		/// <summary>Default is <see cref="RotateType.RotateNone"/>.</summary>
		public const RotateType Default = RotateType.RotateNone;

		/// <summary></summary>
		public static int AngleFromDifference(RotateType rotationA, RotateType rotationB)
		{
			if (rotationA == rotationB)
				return (0);

			if (((rotationA == RotateType.RotateNone) && (rotationB == RotateType.Rotate90))  ||
				((rotationA == RotateType.Rotate90)   && (rotationB == RotateType.Rotate180)) ||
				((rotationA == RotateType.Rotate180)  && (rotationB == RotateType.Rotate270)) ||
				((rotationA == RotateType.Rotate270)  && (rotationB == RotateType.RotateNone)))
				return (90);

			if (((rotationA == RotateType.RotateNone) && (rotationB == RotateType.Rotate270))  ||
				((rotationA == RotateType.Rotate90)   && (rotationB == RotateType.RotateNone)) ||
				((rotationA == RotateType.Rotate180)  && (rotationB == RotateType.Rotate90))   ||
				((rotationA == RotateType.Rotate270)  && (rotationB == RotateType.Rotate180)))
				return (270);

			return (180);
		}

		/// <summary></summary>
		public static RotateType RotationFromAngle(int angle)
		{
			angle %= 360;

			if (angle < 45)  return (RotateType.RotateNone);
			if (angle < 135) return (RotateType.Rotate90);
			if (angle < 225) return (RotateType.Rotate180);
			if (angle < 315) return (RotateType.Rotate270);
			                 return (RotateType.RotateNone);
		}

		/// <summary></summary>
		public static RotateType RotationFromDifference(RotateType rotationA, RotateType rotationB)
		{
			return (RotationFromAngle(AngleFromDifference(rotationA, rotationB)));
		}

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static RotateFlipType ConvertToRotateFlipType(RotateType rotate)
		{
			return ((RotateFlipType)rotate);
		}

		/// <summary></summary>
		public static RotateType ConvertToRotateType(RotateFlipType rotateFlip)
		{
			switch (rotateFlip)
			{
				case RotateFlipType.RotateNoneFlipNone:
			////case RotateFlipType.RotateNoneFlipX:
			////case RotateFlipType.RotateNoneFlipXY:
				case RotateFlipType.RotateNoneFlipY:
														return (RotateType.RotateNone);

				case RotateFlipType.Rotate90FlipNone:
				case RotateFlipType.Rotate90FlipX:
			////case RotateFlipType.Rotate90FlipXY:
			////case RotateFlipType.Rotate90FlipY:
														return (RotateType.Rotate90);

				case RotateFlipType.Rotate180FlipNone:
			////case RotateFlipType.Rotate180FlipX:
			////case RotateFlipType.Rotate180FlipXY:
				case RotateFlipType.Rotate180FlipY:
														return (RotateType.Rotate180);

				case RotateFlipType.Rotate270FlipNone:
				case RotateFlipType.Rotate270FlipX:
			////case RotateFlipType.Rotate270FlipXY:
			////case RotateFlipType.Rotate270FlipY:
														return (RotateType.Rotate270);
			}

			throw (new ArgumentOutOfRangeException("rotateFlip", rotateFlip, MessageHelper.InvalidExecutionPreamble + "'" + rotateFlip + "' is a rotate/flip type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion
	}

	/// <summary>
	/// This enum reduces <see cref="RotateFlipType"/> to flipping.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "These are NOT flags!")]
	public enum FlipType
	{
		FlipNone = RotateFlipType.RotateNoneFlipNone,
		FlipX    = RotateFlipType.RotateNoneFlipX,
		FlipXY   = RotateFlipType.RotateNoneFlipXY,
		FlipY    = RotateFlipType.RotateNoneFlipY,
	}

	/// <summary>
	/// This class provides explicit conversion methods for <see cref="FlipType"/>.
	/// </summary>
	/// <remarks>
	/// The conversions are intentionally implemented using this helper class because this
	/// implementation is faster than using a <see cref="System.ComponentModel.TypeConverter"/>.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class FlipTypeEx
	{
		/// <summary>Default is <see cref="FlipType.FlipNone"/>.</summary>
		public const FlipType Default = FlipType.FlipNone;

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static RotateFlipType ConvertToRotateFlipType(FlipType flip)
		{
			return ((RotateFlipType)flip);
		}

		/// <summary></summary>
		public static FlipType ConvertToFlipType(RotateFlipType rotateFlip)
		{
			switch (rotateFlip)
			{
				case RotateFlipType.RotateNoneFlipNone:
			////case RotateFlipType.Rotate90FlipNone:
			////case RotateFlipType.Rotate180FlipNone:
				case RotateFlipType.Rotate270FlipNone:
														return (FlipType.FlipNone);

				case RotateFlipType.RotateNoneFlipX:
				case RotateFlipType.Rotate90FlipX:
			////case RotateFlipType.Rotate180FlipX:
			////case RotateFlipType.Rotate270FlipX:
														return (FlipType.FlipX);

				case RotateFlipType.RotateNoneFlipXY:
			////case RotateFlipType.Rotate90FlipXY:
			////case RotateFlipType.Rotate180FlipXY:
				case RotateFlipType.Rotate270FlipXY:
														return (FlipType.FlipXY);

				case RotateFlipType.RotateNoneFlipY:
				case RotateFlipType.Rotate90FlipY:
			////case RotateFlipType.Rotate180FlipY:
			////case RotateFlipType.Rotate270FlipY:
														return (FlipType.FlipY);
			}

			throw (new ArgumentOutOfRangeException("rotateFlip", rotateFlip, MessageHelper.InvalidExecutionPreamble + "'" + rotateFlip + "' is a rotate/flip type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion
	}

	#pragma warning restore 1591
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
