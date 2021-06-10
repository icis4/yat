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
// YAT Version 2.4.1
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using MKY;
using MKY.Collections;
using MKY.Collections.Generic;
using MKY.Diagnostics;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// This class cannot be implemented immutable [ImmutableObject(true)] because of the
	/// performance improving /// <see cref="Append(DisplayElement)"/> method.
	/// </remarks>
	[Serializable]
	[XmlInclude(typeof(Nonentity))]
	[XmlInclude(typeof(TxData))]
	[XmlInclude(typeof(TxControl))]
	[XmlInclude(typeof(RxData))]
	[XmlInclude(typeof(RxControl))]
	[XmlInclude(typeof(InfoElement))]
	[XmlInclude(typeof(TimeStampInfo))]
	[XmlInclude(typeof(TimeSpanInfo))]
	[XmlInclude(typeof(TimeDeltaInfo))]
	[XmlInclude(typeof(TimeDurationInfo))]
	[XmlInclude(typeof(DeviceInfo))]
	[XmlInclude(typeof(DirectionInfo))]
	[XmlInclude(typeof(ContentLength))]
	[XmlInclude(typeof(FormatElement))]
	[XmlInclude(typeof(ContentSeparator))]
	[XmlInclude(typeof(InfoSeparator))]
	[XmlInclude(typeof(LineStart))]
	[XmlInclude(typeof(LineBreak))]
	[XmlInclude(typeof(InlineElement))]
	[XmlInclude(typeof(IOControlInfo))]
	[XmlInclude(typeof(WarningInfo))]
	[XmlInclude(typeof(ErrorInfo))]
	public abstract class DisplayElement
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
		// warnings for each undocumented member below. Documenting each member makes little sense
		// since they pretty much tell their purpose and documentation tags between the members
		// makes the code less readable.
		#pragma warning disable 1591

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Public property is required for default XML serialization/deserialization.")]
		[Flags]
		public enum ElementAttributes
		{
			None           =  0,
			Content        =  1,
			Data           =  2,
			DataContent    = (Data | Content),
			Control        =  4,
			ControlContent = (Control | Content),
			Inline         =  8,
			Info           = 16,
			Auxiliary      = 32
		}

		#pragma warning restore 1591

		/// <remarks>Using 'nonentitiy' instead of 'nothing' as that is a keyword in other .NET languages.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Nonentity' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class Nonentity : DisplayElement
		{
			/// <summary></summary>
			public Nonentity()
				: base(ElementAttributes.None)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class TxData : DisplayElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
			public TxData()
				: base(Direction.Tx, ElementAttributes.DataContent)
			{
			}

			/// <summary></summary>
			public TxData(DateTime timeStamp, byte origin, string text)
				: base(timeStamp, Direction.Tx, origin, text, 1, ElementAttributes.DataContent)
			{                                              // ^ Elements are always created corresponding to a single shown character.
			}                                              //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <summary></summary>
			public TxData(DateTime timeStamp, byte[] origin, string text)
				: base(timeStamp, Direction.Tx, origin, text, 1, ElementAttributes.DataContent) // Elements are always created corresponding to a single shown character.
			{                                              // ^ Elements are always created corresponding to a single shown character.
			}                                              //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <remarks>This reduced signature is required for potential unfolding in <see cref="RemoveLastContentChar"/>.</remarks>
			public TxData(byte[] origin, string text)
				: this(TimeStampDefault, origin, text)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class TxControl : DisplayElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
			public TxControl()
				: base(Direction.Tx, ElementAttributes.ControlContent)
			{
			}

			/// <summary></summary>
			public TxControl(DateTime timeStamp, byte origin, string text)
				: base(timeStamp, Direction.Tx, origin, text, 1, ElementAttributes.ControlContent)
			{                                              // ^ Elements are always created corresponding to a single shown character.
			}                                              //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <summary></summary>
			public TxControl(DateTime timeStamp, byte[] origin, string text)
				: base(timeStamp, Direction.Tx, origin, text, 1, ElementAttributes.ControlContent)
			{                                              // ^ Elements are always created corresponding to a single shown character.
			}                                              //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <remarks>This reduced signature is required for potential unfolding in <see cref="RemoveLastContentChar"/>.</remarks>
			public TxControl(byte[] origin, string text)
				: this(TimeStampDefault, origin, text)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class RxData : DisplayElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
			public RxData()
				: base(Direction.Rx, ElementAttributes.DataContent)
			{
			}

			/// <summary></summary>
			public RxData(DateTime timeStamp, byte origin, string text)
				: base(timeStamp, Direction.Rx, origin, text, 1, ElementAttributes.DataContent)
			{                                              // ^ Elements are always created corresponding to a single shown character.
			}                                              //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <summary></summary>
			public RxData(DateTime timeStamp, byte[] origin, string text)
				: base(timeStamp, Direction.Rx, origin, text, 1, ElementAttributes.DataContent)
			{                                              // ^ Elements are always created corresponding to a single shown character.
			}                                              //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <remarks>This reduced signature is required for potential unfolding in <see cref="RemoveLastContentChar"/>.</remarks>
			public RxData(byte[] origin, string text)
				: this(TimeStampDefault, origin, text)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class RxControl : DisplayElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
			public RxControl()
				: base(Direction.Rx, ElementAttributes.ControlContent)
			{
			}

			/// <summary></summary>
			public RxControl(DateTime timeStamp, byte origin, string text)
				: base(timeStamp, Direction.Rx, origin, text, 1, ElementAttributes.ControlContent)
			{                                              // ^ Elements are always created corresponding to a single shown character.
			}                                              //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <summary></summary>
			public RxControl(DateTime timeStamp, byte[] origin, string text)
				: base(timeStamp, Direction.Rx, origin, text, 1, ElementAttributes.ControlContent)
			{                                              // ^ Elements are always created corresponding to a single shown character.
			}                                              //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <remarks>This reduced signature is required for potential unfolding in <see cref="RemoveLastContentChar"/>.</remarks>
			public RxControl(byte[] origin, string text)
				: this(TimeStampDefault, origin, text)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public abstract class InfoElement : DisplayElement
		{
			/// <summary></summary>
			protected InfoElement()
				: base(ElementAttributes.Info)
			{
			}

			/// <summary></summary>
			protected InfoElement(string text)
				: this(DirectionDefault, text)
			{
			}

			/// <summary></summary>
			protected InfoElement(Direction direction, string text)
				: this(TimeStampDefault, direction, text)
			{
			}

			/// <summary></summary>
			protected InfoElement(DateTime timeStamp, Direction direction, string text)
				: base(timeStamp, direction, text, ElementAttributes.Info)
			{
			}

			/// <summary></summary>
			protected InfoElement(InfoElement other)
				: base(other)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class TimeStampInfo : InfoElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
			public TimeStampInfo()
				: base()
			{
			}

			/// <summary></summary>
			public TimeStampInfo(DateTime timeStamp, string format, bool useUtc, string enclosureLeft, string enclosureRight)
				: base(timeStamp, DirectionDefault, ToText(timeStamp, format, useUtc, enclosureLeft, enclosureRight))
			{
			}

			/// <summary></summary>
			protected static string ToText(DateTime timeStamp, string format, bool useUtc, string enclosureLeft, string enclosureRight)
			{
				if (useUtc)                          // UTC
					return (enclosureLeft + timeStamp.ToUniversalTime().ToString(format, DateTimeFormatInfo.CurrentInfo) + enclosureRight);
				else
					return (enclosureLeft + timeStamp.ToString(format, DateTimeFormatInfo.CurrentInfo) + enclosureRight);
			}

			/// <summary>
			/// Replaces <see cref="TimeStamp"/> and <see cref="Text"/> according to given arguments.
			/// </summary>
			public virtual void Change(DateTime timeStamp, string format, bool useUtc, string enclosureLeft, string enclosureRight)
			{
				TimeStamp = timeStamp;
				Text = ToText(timeStamp, format, useUtc, enclosureLeft, enclosureRight);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class TimeSpanInfo : InfoElement
		{
			/// <summary></summary>
			public TimeSpan TimeSpan { get; }

			/// <remarks>This copy-constructor is required for <see cref="Clone"/>.</remarks>
			public TimeSpanInfo(TimeSpanInfo other)
				: base(other)
			{
				TimeSpan = other.TimeSpan;
			}

			/// <summary></summary>
			public TimeSpanInfo(TimeSpan timeSpan, string format, string enclosureLeft, string enclosureRight)
				: base(DirectionDefault, ToText(timeSpan, format, enclosureLeft, enclosureRight))
			{
				TimeSpan = timeSpan;
			}

			/// <summary></summary>
			protected static string ToText(TimeSpan timeSpan, string format, string enclosureLeft, string enclosureRight)
			{
				return (enclosureLeft + TimeSpanEx.FormatInvariantThousandthsEnforceMinutes(timeSpan, format) + enclosureRight);
			}                                                                      // Attention, slightly different than time delta below!

			/// <summary>
			/// Replaces <see cref="Text"/> according to given arguments.
			/// </summary>
			public virtual void Change(TimeSpan timeSpan, string format, string enclosureLeft, string enclosureRight)
			{
				Text = ToText(timeSpan, format, enclosureLeft, enclosureRight);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class TimeDeltaInfo : InfoElement
		{
			/// <summary></summary>
			public TimeSpan TimeDelta { get; }

			/// <remarks>This copy-constructor is required for <see cref="Clone"/>.</remarks>
			public TimeDeltaInfo(TimeDeltaInfo other)
				: base(other)
			{
				TimeDelta = other.TimeDelta;
			}

			/// <summary></summary>
			public TimeDeltaInfo(TimeSpan timeDelta, string format, string enclosureLeft, string enclosureRight)
				: base(DirectionDefault, ToText(timeDelta, format, enclosureLeft, enclosureRight))
			{
				TimeDelta = timeDelta;
			}

			/// <summary></summary>
			protected static string ToText(TimeSpan timeDelta, string format, string enclosureLeft, string enclosureRight)
			{
				return (enclosureLeft + TimeSpanEx.FormatInvariantThousandths(timeDelta, format) + enclosureRight);
			}                                                         // Attention, slightly different than time span above!

			/// <summary>
			/// Replaces <see cref="Text"/> according to given arguments.
			/// </summary>
			public virtual void Change(TimeSpan timeDelta, string format, string enclosureLeft, string enclosureRight)
			{
				Text = ToText(timeDelta, format, enclosureLeft, enclosureRight);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class TimeDurationInfo : InfoElement
		{
			/// <summary></summary>
			public TimeSpan TimeDuration { get; }

			/// <remarks>This copy-constructor is required for <see cref="Clone"/>.</remarks>
			public TimeDurationInfo(TimeDurationInfo other)
				: base(other)
			{
				TimeDuration = other.TimeDuration;
			}

			/// <summary></summary>
			public TimeDurationInfo(TimeSpan timeDuration, string format, string enclosureLeft, string enclosureRight)
				: base(DirectionDefault, ToText(timeDuration, format, enclosureLeft, enclosureRight))
			{
				TimeDuration = timeDuration;
			}

			/// <summary></summary>
			protected static string ToText(TimeSpan timeDuration, string format, string enclosureLeft, string enclosureRight)
			{
				return (enclosureLeft + TimeSpanEx.FormatInvariantThousandths(timeDuration, format) + enclosureRight);
			}                                                         // Attention, slightly different than time span above!
		}

		/// <remarks>Named "Device" for simplicity even though using "I/O Device" for view.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class DeviceInfo : InfoElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
			public DeviceInfo()
				: base()
			{
			}

			/// <summary></summary>
			public DeviceInfo(string infoText, string enclosureLeft, string enclosureRight)
				: base(ToText(infoText, enclosureLeft, enclosureRight))
			{
			}

			/// <summary></summary>
			protected static string ToText(string infoText, string enclosureLeft, string enclosureRight)
			{
				return (enclosureLeft + infoText + enclosureRight);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class DirectionInfo : InfoElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
			public DirectionInfo()
				: base()
			{
			}

			/// <summary></summary>
			public DirectionInfo(Direction direction, string enclosureLeft, string enclosureRight)
				: base(direction, ToText(direction, enclosureLeft, enclosureRight))
			{
			}

			/// <summary></summary>
			protected static string ToText(Direction direction, string enclosureLeft, string enclosureRight)
			{
				return (enclosureLeft + (DirectionEx)direction + enclosureRight);
			}

			/// <summary>
			/// Replaces <see cref="Direction"/> and <see cref="Text"/> according to given arguments.
			/// </summary>
			public virtual void Change(Direction direction, string enclosureLeft, string enclosureRight)
			{
				Direction = direction;
				Text = ToText(direction, enclosureLeft, enclosureRight);
			}
		}

		/// <summary>The length of the data/control content.</summary>
		/// <remarks>Prefixed "Content" for a) preventing naming conflict and b) orthogonality with <see cref="ContentSeparator"/> .</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'orthogonality' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class ContentLength : InfoElement
		{
			/// <summary></summary>
			public int Length { get; }

			/// <remarks>This copy-constructor is required for <see cref="Clone"/>.</remarks>
			public ContentLength(ContentLength other)
				: base(other)
			{
				Length = other.Length;
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "'Byte' not only is a type, it also tells the semantic.")]
			public ContentLength(int length, string enclosureLeft, string enclosureRight)
				: base(ToText(length, enclosureLeft, enclosureRight))
			{
				Length = length;
			}

			/// <summary></summary>
			protected static string ToText(int length, string enclosureLeft, string enclosureRight)
			{
				return (enclosureLeft + length.ToString(CultureInfo.InvariantCulture) + enclosureRight);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public abstract class FormatElement : DisplayElement
		{
			/// <summary></summary>
			protected FormatElement()
				: this(DirectionDefault)
			{
			}

			/// <summary></summary>
			protected FormatElement(Direction direction)
				: this(direction, null)
			{
			}

			/// <summary></summary>
			protected FormatElement(string text)
				: this(DirectionDefault, text)
			{
			}

			/// <summary></summary>
			protected FormatElement(Direction direction, string text)
				: base(TimeStampDefault, direction, text, ElementAttributes.Auxiliary)
			{
			}
		}

		/// <summary>The separator that is added inbetween characters of the data/control content (e.g. radix = char).</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'inbetween' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class ContentSeparator : FormatElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			public ContentSeparator()
				: base()
			{
			}

			/// <remarks>Using direction since adjacent content is also directed.</remarks>
			public ContentSeparator(Direction direction, string whiteSpace)
				: base(direction, whiteSpace)
			{
			}
		}

		/// <summary>The separator that is added inbetween info elements as well as to the left/right of the data/control content.</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'inbetween' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class InfoSeparator : FormatElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
			public InfoSeparator()
				: base()
			{
			}

			/// <summary></summary>
			public InfoSeparator(string whiteSpace)
				: base(whiteSpace)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class LineStart : FormatElement
		{
			/// <summary></summary>
			public LineStart()
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class LineBreak : FormatElement
		{
			/// <summary></summary>
			public LineBreak()
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public abstract class InlineElement : DisplayElement
		{
			/// <summary></summary>
			protected InlineElement()
				: base(ElementAttributes.Inline)
			{
			}

			/// <summary></summary>
			protected InlineElement(string text)
				: this(DirectionDefault, text)
			{
			}

			/// <summary></summary>
			protected InlineElement(Direction direction, string text)
				: this(TimeStampDefault, direction, text)
			{
			}

			/// <summary></summary>
			protected InlineElement(DateTime timeStamp, Direction direction, string text)
				: base(timeStamp, direction, text, ElementAttributes.Inline)
			{
			}

			/// <summary></summary>
			protected InlineElement(DateTime timeStamp, Direction direction, byte origin, string text)
				: base(timeStamp, direction, origin, text, ElementAttributes.Inline)
			{
			}

			/// <summary></summary>
			protected InlineElement(DateTime timeStamp, Direction direction, byte[] origin, string text)
				: base(timeStamp, direction, origin, text, ElementAttributes.Inline)
			{
			}

			/// <summary>
			/// Creates and returns a new object that is a deep-copy of this instance.
			/// </summary>
			public new InlineElement Clone()
			{
				return ((InlineElement)base.Clone());
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class IOControlInfo : InlineElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
			public IOControlInfo()
				: base()
			{
			}

			/// <summary></summary>
			public IOControlInfo(string message)
				: this(TimeStampDefault, DirectionDefault, message)
			{
			}

			/// <summary></summary>
			public IOControlInfo(DateTime timeStamp, Direction direction, string message)
				: base(timeStamp, direction, "[" + message + "]")
			{
			}
		}

		/// <remarks>
		/// Named "WarningInfo" rather than just "Warning" for orthogonality with <see cref="ErrorInfo"/>.
		/// <para>
		/// Note that related format setting is called "WarningFormat", i.e. without "Info".
		/// </para></remarks>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Orthogonality' is a correct English term.")]
		public class WarningInfo : InlineElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
			public WarningInfo()
				: base()
			{
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public WarningInfo(string message, bool omitBracketsAndLabel = false)
				: this(DirectionDefault, message, omitBracketsAndLabel)
			{
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public WarningInfo(Direction direction, string message, bool omitBracketsAndLabel = false)
				: this(TimeStampDefault, direction, message, omitBracketsAndLabel)
			{
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public WarningInfo(DateTime timeStamp, Direction direction, string message, bool omitBracketsAndLabel = false)
				: this(timeStamp, direction, null, message, omitBracketsAndLabel)
			{
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public WarningInfo(DateTime timeStamp, Direction direction, byte origin, string message, bool omitBracketsAndLabel = false)
				: this(timeStamp, direction, new byte[] { origin }, message, omitBracketsAndLabel)
			{
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public WarningInfo(DateTime timeStamp, Direction direction, byte[] origin, string message, bool omitBracketsAndLabel = false)
				: base(timeStamp, direction, origin, (omitBracketsAndLabel ? message : "[Warning: " + message + "]"))
			{
			}
		}

		/// <remarks>
		/// This element type should rather be called "Error" because it applies to any error
		/// that shall be displayed in the terminal. However, "Error" is a keyword in certain
		/// .NET languages such as VB.NET. As a result, any identifier called "Error" or "error"
		/// will cause StyleCop/FxCop to issue a severe warning. So "ErrorInfo" is used instead.
		/// <para>
		/// Note that related format setting is called "ErrorFormat", i.e. without "Info".
		/// </para></remarks>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		public class ErrorInfo : InlineElement
		{
			/// <remarks>This parameterless constructor is required for <see cref="Clone"/>.</remarks>
			[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
			public ErrorInfo()
				: base()
			{
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public ErrorInfo(string message, bool omitBracketsAndLabel = false)
				: this(DirectionDefault, message, omitBracketsAndLabel)
			{
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public ErrorInfo(Direction direction, string message, bool omitBracketsAndLabel = false)
				: this(TimeStampDefault, direction, message, omitBracketsAndLabel)
			{
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public ErrorInfo(DateTime timeStamp, Direction direction, string message, bool omitBracketsAndLabel = false)
				: this(timeStamp, direction, null, message, omitBracketsAndLabel)
			{
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public ErrorInfo(DateTime timeStamp, Direction direction, byte origin, string message, bool omitBracketsAndLabel = false)
				: this(timeStamp, direction, new byte[] { origin }, message, omitBracketsAndLabel)
			{
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public ErrorInfo(DateTime timeStamp, Direction direction, byte[] origin, string message, bool omitBracketsAndLabel = false)
				: base(timeStamp, direction, origin, (omitBracketsAndLabel ? message : "[Error: " + message + "]"))
			{
			}
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary>
		/// The <see cref="DateTime"/> used if no dedicated time stamp information is available.
		/// </summary>
		/// <remarks>
		/// Corresponds to <see cref="DateTime.MinValue"/>.
		/// </remarks>
		public static readonly DateTime TimeStampDefault = DateTime.MinValue;

		/// <summary>
		/// The <see cref="Direction"/> used if no dedicated direction information is available.
		/// </summary>
		/// <remarks>
		/// Corresponds to <see cref="Direction.None"/>.
		/// </remarks>
		public const Direction DirectionDefault = Direction.None;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private DateTime timeStamp;                // = TimeStampDefault = DateTime.MinValue;
		private Direction direction;               // = Direction.None;
		private List<Pair<byte[], string>> origin; // = null;
		private string text;                       // = null;
		private int charCount;                     // = 0;
		private int byteCount;                     // = 0;
		private ElementAttributes attributes;      // = ElementAttributes.None.

		/// <summary>
		/// Indicates whether this <see cref="DisplayElement"/> is highlighted.
		/// </summary>
		[XmlIgnore]
		public virtual bool Highlight { get; set; } // = false;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		protected DisplayElement()
			: this(ElementAttributes.None)
		{
		}

		/// <summary></summary>
		protected DisplayElement(ElementAttributes attributes)
			: this(DirectionDefault, attributes)
		{
		}

		/// <summary></summary>
		protected DisplayElement(Direction direction, ElementAttributes attributes)
			: this(TimeStampDefault, direction, attributes)
		{
		}

		/// <summary></summary>
		protected DisplayElement(DateTime timeStamp, Direction direction, ElementAttributes attributes)
			: this(timeStamp, direction, null, attributes)
		{
		}

		/// <summary></summary>
		protected DisplayElement(DateTime timeStamp, Direction direction, string text, ElementAttributes attributes)
		{
			Initialize(timeStamp, direction, null, text, 0, 0, attributes);
		}

		/// <summary></summary>
		protected DisplayElement(DateTime timeStamp, Direction direction, byte origin, string text, ElementAttributes attributes)
			: this(timeStamp, direction, new byte[] { origin }, text, 0, attributes)
		{
		}

		/// <summary></summary>
		protected DisplayElement(DateTime timeStamp, Direction direction, byte origin, string text, int charCount, ElementAttributes attributes)
			: this(timeStamp, direction, new byte[] { origin }, text, charCount, attributes)
		{
		}

		/// <summary></summary>
		protected DisplayElement(DateTime timeStamp, Direction direction, byte[] origin, string text, ElementAttributes attributes)
			: this(timeStamp, direction, origin, text, 0, attributes)
		{
		}

		/// <summary></summary>
		protected DisplayElement(DateTime timeStamp, Direction direction, byte[] origin, string text, int charCount, ElementAttributes attributes)
		{
			List<Pair<byte[], string>> l = null;

			if (!ArrayEx.IsNullOrEmpty(origin))
			{                                                               // Makes sense since elements of the same type will likely be appended.
				l = new List<Pair<byte[], string>>(DisplayElementCollection.TypicalNumberOfElementsPerLine) // Preset the typical capacity to improve memory management.
				{
					new Pair<byte[], string>(origin, text)
				};
			}
			Initialize(timeStamp, direction, l, text, charCount, ((origin != null) ? (origin.Length) : (0)), attributes);
		}

		private void Initialize(DateTime timeStamp, Direction direction, List<Pair<byte[], string>> origin, string text, int charCount, int byteCount, ElementAttributes attributes)
		{
			this.timeStamp  = timeStamp;
			this.direction  = direction;
			this.origin     = origin; // While most origins are a single char, e.g. 'Ä', some are not, e.g. <CR>, thus string rather than char.
			this.text       = text;
			this.charCount  = charCount;
			this.byteCount  = byteCount;
			this.attributes = attributes;
		}

		/// <summary></summary>
		protected DisplayElement(DisplayElement other)
		{
			Initialize
			(
				other.TimeStamp,
				other.Direction,              // "Pair" is a value type, thus the origin pairs will be cloned.
			  ((other.origin != null) ? (new List<Pair<byte[], string>>(other.origin)) : null),
				other.Text,
				other.CharCount,
				other.ByteCount,
				other.Attributes
			);
		}

	#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~DisplayElement()
		{
			DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

	#endif // DEBUG

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlAttribute("TimeStamp")]
		public virtual DateTime TimeStamp
		{
			get { return (this.timeStamp); }
			set { this.timeStamp = value; }
		}

		/// <summary></summary>
		[XmlAttribute("Direction")]
		public virtual Direction Direction
		{
			get { return (this.direction); }
			set { this.direction = value; }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
		[XmlIgnore]
		public virtual List<Pair<byte[], string>> Origin
		{
			get { return (this.origin); }
		}

		/// <summary></summary>
		[XmlAttribute("Text")]
		public virtual string Text
		{
			get { return (this.text); }
			set { this.text = value;  }
		}

		/// <summary></summary>
		[XmlAttribute("CharCount")]
		public virtual int CharCount
		{
			get { return (this.charCount); }
			set { this.charCount = value;  }
		}

		/// <remarks>
		/// Positioned after <see cref="CharCount"/> as counts are displayed at the end of a line.
		/// </remarks>
		[XmlAttribute("ByteCount")]
		public virtual int ByteCount
		{
			get { return (this.byteCount); }
			set { this.byteCount = value;  }
		}

		/// <summary></summary>
		[XmlAttribute("Attributes")]
		public virtual ElementAttributes Attributes
		{
			get { return (this.attributes); }
			set { this.attributes = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsContent
		{
			get { return ((this.attributes & ElementAttributes.Content) != 0); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsDataContent
		{
			get { return (IsContent && (this.attributes & ElementAttributes.Data) != 0); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsControlContent
		{
			get { return (IsContent && (this.attributes & ElementAttributes.Control) != 0); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsInline
		{
			get { return ((this.attributes & ElementAttributes.Inline) != 0); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsInfo
		{
			get { return ((this.attributes & ElementAttributes.Info) != 0); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsAuxiliary
		{
			get { return ((this.attributes & ElementAttributes.Auxiliary) != 0); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Creates and returns a new object that is a deep-copy of this instance.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Yepp, I know...")]
		public virtual DisplayElement Clone()
		{
			DisplayElement clone;

			if      (this is Nonentity)        clone = new Nonentity();
			else if (this is TxData)           clone = new TxData();
			else if (this is TxControl)        clone = new TxControl();
			else if (this is RxData)           clone = new RxData();
			else if (this is RxControl)        clone = new RxControl();
			else if (this is TimeStampInfo)    clone = new TimeStampInfo();
			else if (this is TimeSpanInfo)     clone = new TimeSpanInfo((TimeSpanInfo)this);
			else if (this is TimeDeltaInfo)    clone = new TimeDeltaInfo((TimeDeltaInfo)this);
			else if (this is TimeDurationInfo) clone = new TimeDurationInfo((TimeDurationInfo)this);
			else if (this is DeviceInfo)       clone = new DeviceInfo();
			else if (this is DirectionInfo)    clone = new DirectionInfo();
			else if (this is ContentLength)    clone = new ContentLength((ContentLength)this);
			else if (this is ContentSeparator) clone = new ContentSeparator();
			else if (this is InfoSeparator)    clone = new InfoSeparator();
			else if (this is LineStart)        clone = new LineStart();
			else if (this is LineBreak)        clone = new LineBreak();
			else if (this is IOControlInfo)    clone = new IOControlInfo();
			else if (this is WarningInfo)      clone = new WarningInfo();
			else if (this is ErrorInfo)        clone = new ErrorInfo();
			else throw (new TypeLoadException(MessageHelper.InvalidExecutionPreamble + "'" + GetType() + "' is a display element that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			clone.timeStamp  = this.timeStamp;
			clone.direction  = this.direction;
			clone.origin     = PerformDeepClone(this.origin);
			clone.text       = this.text;
			clone.charCount  = this.charCount;
			clone.byteCount  = this.byteCount;
			clone.attributes = this.attributes;

			clone.Highlight  = this.Highlight;

			return (clone);
		}

		/// <summary>
		/// Returns <c>true</c> if <paramref name="other"/> can be appended to this element.
		/// </summary>
		/// <remarks>
		/// Note that the type of the element also has to be checked. This ensures that control
		/// elements are not appended to 'normal' data elements.
		/// </remarks>
		public virtual bool AcceptsAppendOf(DisplayElement other)
		{
			// Special case:

			var otherType = other.GetType();
			if (otherType == typeof(ContentSeparator))
			{
				if (string.IsNullOrWhiteSpace(other.Text)) // For performance reasons, whitespace content separators
					return (true);                         // are appended to the preceeding element.
			}

			// Standard case:

			if (GetType() != otherType)
				return (false);

			if (!IsContent || !other.IsContent) // Disallow non-content elements.
				return (false);

		////if (this.timeStamp != other.timeStamp) // TimeStamp may differ.
		////	return (N/A);

			if (this.direction != other.direction) // Self-explaining.
				return (false);

			if (this.attributes != other.attributes) // Self-explaining.
				return (false);

			return (true);
		}

		/// <summary>
		/// Appends contents to this element.
		/// </summary>
		/// <remarks>
		/// Useful to improve performance. Appending keeps number of display elements as low as
		/// possible, thus iteration through display elements gets faster.
		/// </remarks>
		public virtual void Append(DisplayElement other)
		{
			if (!AcceptsAppendOf(other))
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The given element '" + other + "' cannot be appended to this element '" + this + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			// TimeStamp of this element is kept, other is ignored.
			// Direction of this element is kept, other is same as asserted by AcceptsAppendOf() above.

			if (this.origin == null) {
				this.origin = PerformDeepClone(other.origin);
			}                             // No need to add empty origin.
			else if (!ICollectionEx.IsNullOrEmpty(other.origin)) {
				this.origin.AddRange(PerformDeepClone(other.origin));
			}

			if (this.text == null) {
				this.text = other.text;
			}                      // No need to add empty text.
			else if (!string.IsNullOrEmpty(other.text)) {
				this.text += other.text;
			}

			this.charCount += other.charCount;
			this.byteCount += other.byteCount;

			if (other.Highlight) // Activate if needed, leave unchanged otherwise as it could have become highlighted by a previous element.
				this.Highlight = true;
		}

		/// <summary>
		/// Removes the last character from the element.
		/// </summary>
		/// <remarks>
		/// Needed to handle backspace; consequence of <see cref="Append(DisplayElement)"/> above.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// The element is no <see cref="ElementAttributes.Content"/>.
		/// -or-
		/// The element contains text but the origin is empty.
		/// </exception>
		public virtual void RemoveLastContentChar()
		{
			if (!IsContent)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The element is no content!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if (this.text.Length > 0) // Calling this method shall be permitted for zeor-length elements, same as e.g. string.Remove(0, 0) is permitted.
			{
				if (this.origin.Count == 0)
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The element contains text but the origin is empty!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				// Remove the last char:
				this.text = this.text.Substring(0, (this.text.Length - 1));
				this.charCount--;

				// Remove the related origin:
				var originToRemove = this.origin.Last();              // Simply remove the last origin item.
				var byteCountToRemove = originToRemove.Value1.Length; // This approach covers cases where e.g.
				this.origin.RemoveAt(this.origin.Count - 1);          // substitution is active (e.g. 'a' -> 'A').
				this.byteCount -= byteCountToRemove;
			}
		}

		/// <summary>
		/// Remove a potential trailing content separator from the <see cref="DisplayElementCollection"/>.
		/// </summary>
		/// <remarks>
		/// Needed to handle backspace, thus applies to content only.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// The element is not content.
		/// </exception>
		public virtual void RemoveTrailingContentSeparator(string contentSeparator)
		{
			if (!IsContent)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The element is no content!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if (Text.Length > 0)
			{
				if (Text.EndsWith(contentSeparator, StringComparison.CurrentCulture))
				{
					var textLengthWithoutContentSeparator = (Text.Length - contentSeparator.Length);
					Text = Text.Substring(0, textLengthWithoutContentSeparator);
				}
			}
		}

		/// <summary></summary>
		public virtual byte[] ToOrigin()
		{
			if (Origin != null)
			{
				var l = new List<byte>(Origin.Count); // Preset the required capacity to improve memory management.

				foreach (var item in Origin)
					l.AddRange(item.Value1);

				return (l.ToArray());
			}
			else
			{
				return (null);
			}
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private static Pair<byte[], string> PerformDeepClone(Pair<byte[], string> originItem)
		{                                                              // Shallow copy of array is good enough for byte[].
			return (new Pair<byte[], string>((byte[])originItem.Value1.Clone(), originItem.Value2));
		}

		private static List<Pair<byte[], string>> PerformDeepClone(List<Pair<byte[], string>> origin)
		{
			if (origin != null)
			{
				var clone = new List<Pair<byte[], string>>(origin.Capacity); // Preset the required capacity to improve memory management.

				foreach (Pair<byte[], string> originItem in origin)
					clone.Add(PerformDeepClone(originItem));

				return (clone);
			}
			else
			{
				return (null);
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			if (Text != null)
				return (Text);
			else
				return ("");
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		/// <remarks>
		/// Limited to a single line to keep debug output compact, same as <see cref="ToString()"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string ToDiagnosticsString(string indent = "")
		{
			var sb = new StringBuilder(indent);

			sb.Append((Text != null) ? (@"""" + StringEx.ConvertToPrintableString(Text) + @"""") : "(none)");
			sb.Append(" | CharCount = ");
			sb.Append(CharCount.ToString(CultureInfo.CurrentCulture));
			sb.Append(" | ByteCount = ");
			sb.Append(ByteCount.ToString(CultureInfo.CurrentCulture));
			sb.Append(" | OriginCount = ");
			sb.Append((Origin != null) ? Origin.Count.ToString(CultureInfo.CurrentCulture) : "(none)");
			sb.Append(" | Type = ");
			sb.Append(GetType().Name);
			sb.Append(" | TimeStamp = ");
			sb.Append(TimeStamp.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo));
			sb.Append(" | Direction = ");
			sb.Append(Direction.ToString());
			sb.Append(" | Attributes = ");
			sb.Append(Attributes.ToString());
			sb.Append(" | Highlight = ");
			sb.Append(Highlight.ToString());

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
