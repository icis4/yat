﻿//==================================================================================================
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

using MKY;
using MKY.Collections.Generic;
using MKY.Diagnostics;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
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
	[XmlInclude(typeof(IODeviceInfo))]
	[XmlInclude(typeof(DirectionInfo))]
	[XmlInclude(typeof(DataLength))]
	[XmlInclude(typeof(FormatElement))]
	[XmlInclude(typeof(ContentSpace))]
	[XmlInclude(typeof(InfoSeparator))]
	[XmlInclude(typeof(LineStart))]
	[XmlInclude(typeof(LineBreak))]
	[XmlInclude(typeof(InlineElement))]
	[XmlInclude(typeof(IOControlInfo))]
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
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class Nonentity : DisplayElement
		{
			/// <summary></summary>
			public Nonentity()
				: base(ElementAttributes.None)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TxData : DisplayElement
		{
			/// <summary></summary>
			public TxData()
				: base(Direction.Tx, ElementAttributes.DataContent)
			{
			}

			/// <summary></summary>
			public TxData(byte origin, string text)
				: base(Direction.Tx, origin, text, 1, ElementAttributes.DataContent)
			{                                   // ^ Elements are always created corresponding to a single shown character.
			}                                   //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <summary></summary>
			public TxData(byte[] origin, string text)
				: base(Direction.Tx, origin, text, 1, ElementAttributes.DataContent) // Elements are always created corresponding to a single shown character.
			{                                   // ^ Elements are always created corresponding to a single shown character.
			}                                   //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TxControl : DisplayElement
		{
			/// <summary></summary>
			public TxControl()
				: base(Direction.Tx, ElementAttributes.ControlContent)
			{
			}

			/// <summary></summary>
			public TxControl(byte origin, string text)
				: base(Direction.Tx, origin, text, 1, ElementAttributes.ControlContent)
			{                                   // ^ Elements are always created corresponding to a single shown character.
			}                                   //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <summary></summary>
			public TxControl(byte[] origin, string text)
				: base(Direction.Tx, origin, text, 1, ElementAttributes.ControlContent)
			{                                   // ^ Elements are always created corresponding to a single shown character.
			}                                   //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class RxData : DisplayElement
		{
			/// <summary></summary>
			public RxData()
				: base(Direction.Rx, ElementAttributes.DataContent)
			{
			}

			/// <summary></summary>
			public RxData(byte origin, string text)
				: base(Direction.Rx, origin, text, 1, ElementAttributes.DataContent)
			{                                   // ^ Elements are always created corresponding to a single shown character.
			}                                   //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <summary></summary>
			public RxData(byte[] origin, string text)
				: base(Direction.Rx, origin, text, 1, ElementAttributes.DataContent)
			{                                   // ^ Elements are always created corresponding to a single shown character.
			}                                   //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class RxControl : DisplayElement
		{
			/// <summary></summary>
			public RxControl()
				: base(Direction.Rx, ElementAttributes.ControlContent)
			{
			}

			/// <summary></summary>
			public RxControl(byte origin, string text)
				: base(Direction.Rx, origin, text, 1, ElementAttributes.ControlContent)
			{                                   // ^ Elements are always created corresponding to a single shown character.
			}                                   //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.

			/// <summary></summary>
			public RxControl(byte[] origin, string text)
				: base(Direction.Rx, origin, text, 1, ElementAttributes.ControlContent)
			{                                   // ^ Elements are always created corresponding to a single shown character.
			}                                   //   ASCII mnemonics (e.g. <CR>) also account for a single shown character.
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public abstract class InfoElement : DisplayElement
		{
			/// <summary></summary>
			protected InfoElement(string text)
				: this(Direction.None, text)
			{
			}

			/// <summary></summary>
			protected InfoElement(Direction direction, string text)
				: base(direction, text, ElementAttributes.Info)
			{
			}

			/// <summary></summary>
			protected InfoElement(InfoElement other)
				: base(other)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TimeStampInfo : InfoElement
		{
			/// <summary></summary>
			public DateTime TimeStamp { get; }

			/// <summary></summary>
			public TimeStampInfo(TimeStampInfo other)
				: base(other)
			{
				TimeStamp = other.TimeStamp;
			}

			/// <summary></summary>
			public TimeStampInfo(DateTime timeStamp, string format, bool useUtc, string enclosureLeft, string enclosureRight)
				: base(Direction.None, ToText(timeStamp, format, useUtc, enclosureLeft, enclosureRight))
			{
				TimeStamp = timeStamp;
			}

			/// <summary></summary>
			protected static string ToText(DateTime timeStamp, string format, bool useUtc, string enclosureLeft, string enclosureRight)
			{
				if (useUtc)                          // UTC
					return (enclosureLeft + timeStamp.ToUniversalTime().ToString(format, DateTimeFormatInfo.CurrentInfo) + enclosureRight);
				else
					return (enclosureLeft + timeStamp.ToString(format, DateTimeFormatInfo.CurrentInfo) + enclosureRight);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TimeSpanInfo : InfoElement
		{
			/// <summary></summary>
			public TimeSpan TimeSpan { get; }

			/// <summary></summary>
			public TimeSpanInfo(TimeSpanInfo other)
				: base(other)
			{
				TimeSpan = other.TimeSpan;
			}

			/// <summary></summary>
			public TimeSpanInfo(TimeSpan timeSpan, string format, string enclosureLeft, string enclosureRight)
				: base(Direction.None, ToText(timeSpan, format, enclosureLeft, enclosureRight))
			{
				TimeSpan = timeSpan;
			}

			/// <summary></summary>
			protected static string ToText(TimeSpan timeSpan, string format, string enclosureLeft, string enclosureRight)
			{
				return (enclosureLeft + TimeSpanEx.FormatInvariantThousandthsEnforceMinutes(timeSpan, format) + enclosureRight);
			}                                                                      // Attention, slightly different than time delta below!
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TimeDeltaInfo : InfoElement
		{
			/// <summary></summary>
			public TimeSpan TimeDelta { get; }

			/// <summary></summary>
			public TimeDeltaInfo(TimeDeltaInfo other)
				: base(other)
			{
				TimeDelta = other.TimeDelta;
			}

			/// <summary></summary>
			public TimeDeltaInfo(TimeSpan timeDelta, string format, string enclosureLeft, string enclosureRight)
				: base(Direction.None, ToText(timeDelta, format, enclosureLeft, enclosureRight))
			{
				TimeDelta = timeDelta;
			}

			/// <summary></summary>
			protected static string ToText(TimeSpan timeDelta, string format, string enclosureLeft, string enclosureRight)
			{
				return (enclosureLeft + TimeSpanEx.FormatInvariantThousandths(timeDelta, format) + enclosureRight);
			}                                                         // Attention, slightly different than time span above!
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TimeDurationInfo : InfoElement
		{
			/// <summary></summary>
			public TimeSpan TimeDuration { get; }

			/// <summary></summary>
			public TimeDurationInfo(TimeDurationInfo other)
				: base(other)
			{
				TimeDuration = other.TimeDuration;
			}

			/// <summary></summary>
			public TimeDurationInfo(TimeSpan timeDuration, string format, string enclosureLeft, string enclosureRight)
				: base(Direction.None, ToText(timeDuration, format, enclosureLeft, enclosureRight))
			{
				TimeDuration = timeDuration;
			}

			/// <summary></summary>
			protected static string ToText(TimeSpan timeDuration, string format, string enclosureLeft, string enclosureRight)
			{
				return (enclosureLeft + TimeSpanEx.FormatInvariantThousandths(timeDuration, format) + enclosureRight);
			}                                                         // Attention, slightly different than time span above!
		}

		/// <remarks>
		/// Named 'IODeviceInfo' instead of just 'IOInfo' for better comprehensibility.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class IODeviceInfo : InfoElement
		{
			/// <summary></summary>
			public IODeviceInfo()
				: this(null, null, null)
			{
			}

			/// <summary></summary>
			public IODeviceInfo(string infoText, string enclosureLeft, string enclosureRight)
				: base(enclosureLeft + infoText + enclosureRight)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class DirectionInfo : InfoElement
		{
			/// <summary></summary>
			public DirectionInfo()
				: this(Direction.None, null, null)
			{
			}

			/// <summary></summary>
			public DirectionInfo(Direction direction, string enclosureLeft, string enclosureRight)
				: base(direction, enclosureLeft + (DirectionEx)direction + enclosureRight)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class DataLength : InfoElement
		{
			/// <summary></summary>
			public int Length { get; }

			/// <summary></summary>
			[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "Why not? 'Byte' not only is a type, but also emphasizes a purpose.")]
			public DataLength(int length, string enclosureLeft, string enclosureRight)
				: base(enclosureLeft + length.ToString(CultureInfo.InvariantCulture) + enclosureRight)
			{
				Length = length;
			}

			/// <summary></summary>
			public DataLength(DataLength other)
				: base(other)
			{
				Length = other.Length;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public abstract class FormatElement : DisplayElement
		{
			/// <summary></summary>
			protected FormatElement()
				: this(Direction.None)
			{
			}

			/// <summary></summary>
			protected FormatElement(Direction direction)
				: this(direction, null)
			{
			}

			/// <summary></summary>
			protected FormatElement(string text)
				: this(Direction.None, text)
			{
			}

			/// <summary></summary>
			protected FormatElement(Direction direction, string text)
				: base(direction, text, ElementAttributes.Auxiliary)
			{
			}
		}

		/// <summary>The space that is added inbetween characters of the data/control content (e.g. radix = char).</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'inbetween' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class ContentSpace : FormatElement
		{
			/// <summary></summary>
			public ContentSpace()
				: this(Direction.None)
			{
			}

			/// <remarks>Using direction since adjacent content is also directed.</remarks>
			public ContentSpace(Direction direction)
				: base(direction, " ") // Content space is fixed to a space. If this is no longer the case, rename to 'ContentSeparator'.
			{
			}
		}

		/// <summary>The margin that is added to the right of the data/control content.</summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class InfoSeparator : FormatElement
		{
			/// <summary></summary>
			public InfoSeparator()
				: this(null)
			{
			}

			/// <summary></summary>
			public InfoSeparator(string whiteSpace)
				: base(whiteSpace)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class LineStart : FormatElement
		{
			/// <summary></summary>
			public LineStart()
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class LineBreak : FormatElement
		{
			/// <summary></summary>
			public LineBreak()
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public abstract class InlineElement : DisplayElement
		{
			/// <summary></summary>
			protected InlineElement(string text)
				: this(Direction.None, text)
			{
			}

			/// <summary></summary>
			protected InlineElement(Direction direction, string text)
				: base(direction, text, ElementAttributes.Inline)
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
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class IOControlInfo : InlineElement
		{
			/// <summary></summary>
			public IOControlInfo()
				: this(null)
			{
			}

			/// <summary></summary>
			public IOControlInfo(string message)
				: this(Direction.None, message)
			{
			}

			/// <summary></summary>
			public IOControlInfo(Direction direction, string message)
				: base(direction, "[" + message + "]")
			{
			}
		}

		/// <remarks>
		/// This element type should rather be called 'Error' because it applies to any errors
		/// that shall be displayed in the terminal. However, 'Error' is a keyword in certain
		/// .NET languages such as VB.NET. As a result, any identifier called 'Error' or 'error'
		/// will cause StyleCop/FxCop to issue a severe warning. So 'ErrorInfo' is used instead.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class ErrorInfo : InlineElement
		{
			/// <summary></summary>
			public ErrorInfo()
				: this(null)
			{
			}

			/// <summary></summary>
			public ErrorInfo(string message)
				: this(Direction.None, message)
			{
			}

			/// <summary></summary>
			public ErrorInfo(Direction direction, string message)
				: this(direction, message, false)
			{
			}

			/// <summary></summary>
			public ErrorInfo(Direction direction, string message, bool isWarningOnly)
				: base(direction, (isWarningOnly ? ("[Warning: " + message + "]") : ("[Error: " + message + "]")))
			{
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

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
		private DisplayElement()
			: this(ElementAttributes.None)
		{
		}

		/// <summary></summary>
		private DisplayElement(ElementAttributes flags)
			: this(Direction.None, flags)
		{
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, ElementAttributes flags)
			: this(direction, null, flags)
		{
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, string text, ElementAttributes flags)
		{
			Initialize(direction, null, text, 0, 0, flags);
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, byte origin, string text, int charCount, ElementAttributes attributes)
			: this(direction, new byte[] { origin }, text, charCount, attributes)
		{
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, byte[] origin, string text, int charCount, ElementAttributes attributes)
		{                                                                   // Makes sense since elements of the same type will be appended.
			var l = new List<Pair<byte[], string>>(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
			l.Add(new Pair<byte[], string>(origin, text));
			Initialize(direction, l, text, charCount, origin.Length, attributes);
		}

		private void Initialize(Direction direction, List<Pair<byte[], string>> origin, string text, int charCount, int byteCount, ElementAttributes attributes)
		{
			this.direction  = direction;
			this.origin     = origin;
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
				other.Direction,              // 'Pair' is a value type, thus the origin pairs will be cloned.
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

		/// <summary></summary>
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
			else if (this is TimeStampInfo)    clone = new TimeStampInfo((TimeStampInfo)this);
			else if (this is TimeSpanInfo)     clone = new TimeSpanInfo((TimeSpanInfo)this);
			else if (this is TimeDeltaInfo)    clone = new TimeDeltaInfo((TimeDeltaInfo)this);
			else if (this is TimeDurationInfo) clone = new TimeDurationInfo((TimeDurationInfo)this);
			else if (this is IODeviceInfo)     clone = new IODeviceInfo();
			else if (this is DirectionInfo)    clone = new DirectionInfo();
			else if (this is DataLength)       clone = new DataLength((DataLength)this);
			else if (this is ContentSpace)     clone = new ContentSpace();
			else if (this is InfoSeparator)    clone = new InfoSeparator();
			else if (this is LineStart)        clone = new LineStart();
			else if (this is LineBreak)        clone = new LineBreak();
			else if (this is IOControlInfo)    clone = new IOControlInfo();
			else if (this is ErrorInfo)        clone = new ErrorInfo();
			else throw (new TypeLoadException(MessageHelper.InvalidExecutionPreamble + "'" + GetType() + "' is a display element that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			clone.direction  = this.direction;
			clone.origin     = PerformDeepClone(this.origin);
			clone.text       = this.text;
			clone.charCount  = this.charCount;
			clone.byteCount  = this.byteCount;
			clone.attributes = this.attributes;

			clone.Highlight  = this.Highlight;

			return (clone);
		}

	#if (FALSE)
		/// <remarks>
		/// <paramref name="origin"/> must correspond to a single byte or character, i.e. result
		/// in a single element, same as when creating elements "the normal way". ASCII mnemonics
		/// (e.g. <CR>) are considered a single shown character.
		/// </remarks>
		public virtual DisplayElement RecreateFromOrigin(Pair<byte[], string> origin)
		{
			var clone = Clone(); // Ensure to recreate the proper type.

			// Keep direction and attributes.

			// Replace origin and byteCount:
			var clonedOrigin = new List<Pair<byte[], string>>(1); // Preset the required capacity to improve memory management.
			clonedOrigin.Add(PerformDeepClone(origin));
			clone.origin = clonedOrigin;
			clone.byteCount = origin.Value1.Length;

			// Replace text and charCount:
			string text = origin.Value2;
			clone.text = text;
			clone.charCount = 1; // See remark.

			return (clone);
		}
	#endif

		/// <summary>
		/// Returns <c>true</c> if <paramref name="other"/> can be appended to this element. Only
		/// data elements of the same direction can be appended. Appending other elements would
		/// lead to missing elements.
		/// </summary>
		/// <remarks>
		/// Note that the type of the element also has to be checked. This ensures that control
		/// elements are not appended to 'normal' data elements.
		/// </remarks>
		public virtual bool AcceptsAppendOf(DisplayElement other)
		{
			if (GetType() != other.GetType())
				return (false);

			if (!IsContent || !other.IsContent) // Disallow non-content elements.
				return (false);

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
		/// possible, thus iteration through display element gets faster.
		/// </remarks>
		public virtual void Append(DisplayElement other)
		{
			if (!AcceptsAppendOf(other))
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The given element '" + other + "' cannot be appended to this element '" + this + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if (this.origin != null)
				this.origin.AddRange(PerformDeepClone(other.origin));
			else
				this.origin = PerformDeepClone(other.origin);

			if (this.text != null)
				this.text += other.text;
			else
				this.text = other.text;

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
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using exact native parameter names.")]
		public virtual void RemoveLastChar()
		{
			if (!IsContent)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The element is no content!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if (this.origin.Count == 0)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The origin is empty!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			// Unfold the element:
			var originConstructor = GetType().GetConstructor(new Type[] { typeof(byte[]), typeof(string) }); // All content elements have such constructor.
			var unfolded = new List<DisplayElement>(this.origin.Count); // Preset the required capacity to improve memory management.
			foreach (var p in this.origin)
				unfolded.Add((DisplayElement)originConstructor.Invoke(new object[] { p.Value1, p.Value2 }));

			// Remove the last character:
			for (int index = (unfolded.Count - 1); index >= 0; index--)
			{
				var current = unfolded[index];
				if (current.CharCount < 1)
				{
					unfolded.RemoveAt(index); // A preceeding whitespace content has to be removed,
					continue;                 // but then continue searching for char.
				}
				else if (current.CharCount == 1)
				{
					unfolded.RemoveAt(index); // A single element can be removed,
					break;                    // done.
				}
				else if (current.CharCount > 1)
				{
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A newly created element can never have a character count of '" + current.CharCount + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			// Recreate the element:
			var emptyConstructor = GetType().GetConstructor(Type.EmptyTypes); // All content elements have such constructor.
			var recreated = (DisplayElement)emptyConstructor.Invoke(null);
			foreach (var de in unfolded)
				recreated.Append(de);

		////this.direction can be kept.
			this.origin    = recreated.origin;
			this.text      = recreated.text;
			this.charCount = recreated.charCount;
			this.byteCount = recreated.byteCount;
		////this.attributes can be kept.

		////this.Highlight can be kept.
		}

		/// <summary></summary>
		public virtual byte[] ToOrigin()
		{
			if (Origin != null)
			{
				List<byte> l = new List<byte>();

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
		{
			return (new Pair<byte[], string>((byte[])originItem.Value1.Clone(), originItem.Value2));
		}

		private static List<Pair<byte[], string>> PerformDeepClone(List<Pair<byte[], string>> origin)
		{
			if (origin != null)
			{
				List<Pair<byte[], string>> clone = new List<Pair<byte[], string>>(origin.Capacity); // Preset the required capacity to improve memory management.

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
			var sb = new StringBuilder();

			sb.Append(indent);
			sb.Append((Text != null) ? (@"""" + StringEx.ConvertToPrintableString(Text) + @"""") : "None");
			sb.Append(" | CharCount = ");
			sb.Append(CharCount.ToString(CultureInfo.CurrentCulture));
			sb.Append(" | ByteCount = ");
			sb.Append(ByteCount.ToString(CultureInfo.CurrentCulture));
			sb.Append(" | OriginCount = ");
			sb.Append((Origin != null) ? Origin.Count.ToString(CultureInfo.CurrentCulture) : "None");
			sb.Append(" | Type = ");
			sb.Append(GetType().Name);
			sb.Append(" | Direction = ");
			sb.Append(Direction.ToString());
			sb.Append(" | Flags = ");
			sb.Append(Attributes.ToString());

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
