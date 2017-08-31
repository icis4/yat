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
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	[Serializable]
	[XmlInclude(typeof(Nonentity))]
	[XmlInclude(typeof(TxData))]
	[XmlInclude(typeof(TxControl))]
	[XmlInclude(typeof(RxData))]
	[XmlInclude(typeof(RxControl))]
	[XmlInclude(typeof(InfoElement))]
	[XmlInclude(typeof(DateInfo))]
	[XmlInclude(typeof(TimeInfo))]
	[XmlInclude(typeof(PortInfo))]
	[XmlInclude(typeof(DirectionInfo))]
	[XmlInclude(typeof(DataLength))]
	[XmlInclude(typeof(AuxiliaryElement))]
	[XmlInclude(typeof(DataSpace))]
	[XmlInclude(typeof(InfoSeparator))]
	[XmlInclude(typeof(LineStart))]
	[XmlInclude(typeof(LineBreak))]
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
			None      =  0,
			Content   =  1,
			Eol       =  2,
			Inline    =  4,
			Info      =  8,
			Auxiliary = 16
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
				: base(ElementAttributes.Content)
			{
			}

			/// <summary></summary>
			public TxData(byte origin, string text)
				: base(Direction.Tx, origin, text)
			{
			}

			/// <summary></summary>
			public TxData(byte[] origin, string text, int byteCount)
				: base(Direction.Tx, origin, text, byteCount)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TxControl : DisplayElement
		{
			/// <summary></summary>
			public TxControl()
				: base(ElementAttributes.Content)
			{
			}

			/// <summary></summary>
			public TxControl(byte origin, string text)
				: base(Direction.Tx, origin, text)
			{
			}

			/// <summary></summary>
			public TxControl(byte[] origin, string text, int byteCount)
				: base(Direction.Tx, origin, text, byteCount)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class RxData : DisplayElement
		{
			/// <summary></summary>
			public RxData()
				: base(ElementAttributes.Content)
			{
			}

			/// <summary></summary>
			public RxData(byte origin, string text)
				: base(Direction.Rx, origin, text)
			{
			}

			/// <summary></summary>
			public RxData(byte[] origin, string text, int byteCount)
				: base(Direction.Rx, origin, text, byteCount)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class RxControl : DisplayElement
		{
			/// <summary></summary>
			public RxControl()
				: base(ElementAttributes.Content)
			{
			}

			/// <summary></summary>
			public RxControl(byte origin, string text)
				: base(Direction.Rx, origin, text)
			{
			}

			/// <summary></summary>
			public RxControl(byte[] origin, string text, int byteCount)
				: base(Direction.Rx, origin, text, byteCount)
			{
			}
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
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class DateInfo : InfoElement
		{
			/// <summary></summary>
			public const string Format = "yyyy-MM-dd";

			/// <summary></summary>
			public DateInfo()
				: this(DateTime.Now, null, null)
			{
			}

			/// <summary></summary>
			public DateInfo(DateTime timeStamp, string enclosureLeft, string enclosureRight)
				: this(Direction.None, timeStamp, enclosureLeft, enclosureRight)
			{
			}

			/// <summary></summary>
			public DateInfo(Direction direction, DateTime timeStamp, string enclosureLeft, string enclosureRight)
				: base(direction, enclosureLeft + timeStamp.ToString(Format, DateTimeFormatInfo.InvariantInfo) + enclosureRight)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TimeInfo : InfoElement
		{
			/// <remarks>
			/// Output milliseconds for readability, even though last digit only provides limited accuracy.
			/// </remarks>
			public const string Format = "HH:mm:ss.fff";

			/// <summary></summary>
			public TimeInfo()
				: this(DateTime.Now, null, null)
			{
			}

			/// <summary></summary>
			public TimeInfo(DateTime timeStamp, string enclosureLeft, string enclosureRight)
				: this(Direction.None, timeStamp, enclosureLeft, enclosureRight)
			{
			}

			/// <summary></summary>
			public TimeInfo(Direction direction, DateTime timeStamp, string enclosureLeft, string enclosureRight)
				: base(direction, enclosureLeft + timeStamp.ToString(Format, DateTimeFormatInfo.InvariantInfo) + enclosureRight)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class PortInfo : InfoElement
		{
			/// <summary></summary>
			public PortInfo()
				: this(Direction.None, null, null, null)
			{
			}

			/// <summary></summary>
			public PortInfo(string infoText, string enclosureLeft, string enclosureRight)
				: this(Direction.None, infoText, enclosureLeft, enclosureRight)
			{
			}

			/// <summary></summary>
			public PortInfo(Direction direction, string infoText, string enclosureLeft, string enclosureRight)
				: base(direction, enclosureLeft + infoText + enclosureRight)
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
			public DataLength()
				: this(0, null, null)
			{
			}

			/// <summary></summary>
			public DataLength(int byteCount, string enclosureLeft, string enclosureRight)
				: this(Direction.None, byteCount, enclosureLeft, enclosureRight)
			{
			}

			/// <summary></summary>
			public DataLength(Direction direction, int byteCount, string enclosureLeft, string enclosureRight)
				: base(direction, enclosureLeft + byteCount.ToString(CultureInfo.InvariantCulture) + enclosureRight)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public abstract class AuxiliaryElement : DisplayElement
		{
			/// <summary></summary>
			protected AuxiliaryElement(Direction direction)
				: this(direction, null)
			{
			}

			/// <summary></summary>
			protected AuxiliaryElement(string text)
				: this(Direction.None, text)
			{
			}

			/// <summary></summary>
			protected AuxiliaryElement(Direction direction, string text)
				: base(direction, text, ElementAttributes.Auxiliary)
			{
			}
		}

		/// <summary>The space that is added inbetween characters of the data content (i.e. radix = char).</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'inbetween' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class DataSpace : AuxiliaryElement
		{
			/// <summary></summary>
			public DataSpace()
				: this(Direction.None)
			{
			}

			/// <summary></summary>
			public DataSpace(Direction direction)
				: base(direction, " ") // Data space is fixed to a normal space. If this is no longer the case, rename to 'DataSeparator'.
			{
			}
		}

		/// <summary>The margin that is added to the right of the data content.</summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class InfoSeparator : AuxiliaryElement
		{
			/// <summary></summary>
			public InfoSeparator()
				: this(Direction.None, null)
			{
			}

			/// <summary></summary>
			public InfoSeparator(string whiteSpace)
				: this(Direction.None, whiteSpace)
			{
			}

			/// <summary></summary>
			public InfoSeparator(Direction direction, string whiteSpace)
				: base(direction, whiteSpace)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class LineStart : AuxiliaryElement
		{
			/// <summary></summary>
			public LineStart()
				: this(Direction.None)
			{
			}

			/// <summary></summary>
			public LineStart(Direction direction)
				: base(direction)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class LineBreak : AuxiliaryElement
		{
			/// <summary></summary>
			public LineBreak()
				: this(Direction.None)
			{
			}

			/// <summary></summary>
			public LineBreak(Direction direction)
				: base(direction)
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
		public class ErrorInfo : DisplayElement
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
				: base(direction, (isWarningOnly ? ("<Warning: " + message + ">") : ("<Error: " + message + ">")), ElementAttributes.Inline)
			{
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Direction direction;
		private List<Pair<byte[], string>> origin;
		private string text;
		private int byteCount;
		private ElementAttributes attributes;

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
			: this(Direction.None, null, flags)
		{
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, string text, ElementAttributes flags)
		{
			Initialize(direction, null, text, 0, flags);
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, byte origin, string text)
			: this(direction, new byte[] { origin }, text, 1)
		{
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, byte[] origin, string text, int byteCount)
		{
			var l = new List<Pair<byte[], string>>(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
			l.Add(new Pair<byte[], string>(origin, text));
			Initialize(direction, l, text, byteCount, ElementAttributes.Content);
		}

		private void Initialize(Direction direction, List<Pair<byte[], string>> origin, string text, int byteCount, ElementAttributes attributes)
		{
			this.direction  = direction;
			this.origin     = origin;
			this.text       = text;
			this.byteCount  = byteCount;
			this.attributes = attributes;
		}

	#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~DisplayElement()
		{
			MKY.Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
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
		[XmlIgnore]
		public virtual int OriginCount
		{
			get
			{
				if (this.origin != null)
					return (this.origin.Count);
				else
					return (0);
			}
		}

		/// <summary></summary>
		[XmlAttribute("Text")]
		public virtual string Text
		{
			get { return (this.text); }
			set { this.text = value;  }
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
		public virtual bool IsEol
		{
			get { return ((this.attributes & ElementAttributes.Eol) != 0); }
			set
			{
				if (value)
					this.attributes |= ElementAttributes.Eol;
				else
					this.attributes &= ~ElementAttributes.Eol;
			}
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
		public virtual DisplayElement Clone()
		{
			// Ensure to use the proper constructor.

			// Attention: For performance reasons, reflection as shown below is not used
			//ConstructorInfo ci = GetType().GetConstructor(Type.EmptyTypes);
			//DisplayElement clone = (DisplayElement)ci.Invoke(null);

			DisplayElement clone;

			if      (this is Nonentity)		clone = new Nonentity();
			else if (this is TxData)		clone = new TxData();
			else if (this is TxControl)		clone = new TxControl();
			else if (this is RxData)		clone = new RxData();
			else if (this is RxControl)		clone = new RxControl();
			else if (this is DateInfo)		clone = new DateInfo();
			else if (this is TimeInfo)		clone = new TimeInfo();
			else if (this is PortInfo)		clone = new PortInfo();
			else if (this is DirectionInfo)	clone = new DirectionInfo();
			else if (this is DataLength)	clone = new DataLength();
			else if (this is DataSpace)		clone = new DataSpace();
			else if (this is InfoSeparator)	clone = new InfoSeparator();
			else if (this is LineStart)		clone = new LineStart();
			else if (this is LineBreak)		clone = new LineBreak();
			else if (this is ErrorInfo)		clone = new ErrorInfo();
			else throw (new TypeLoadException(MessageHelper.InvalidExecutionPreamble + "'" + GetType() + "' is a display element that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			clone.direction  = this.direction;
			clone.origin     = PerformDeepClone(this.origin);
			clone.text       = this.text;
			clone.byteCount  = this.byteCount;
			clone.attributes = this.attributes;

			return (clone);
		}

		/// <summary></summary>
		public virtual DisplayElement RecreateFromOriginItem(Pair<byte[], string> originItem)
		{
			var clone = Clone(); // Ensure to recreate the proper type.

			// Keep direction, isData and isEol.

			// Replace origin and byteCount.
			var clonedOrigin = new List<Pair<byte[], string>>(1); // Preset the required capacity to improve memory management.
			clonedOrigin.Add(PerformDeepClone(originItem));
			clone.origin = clonedOrigin;
			clone.byteCount = 1;

			// Replace text.
			string text = originItem.Value2;
			clone.text = text;

			return (clone);
		}

		/// <summary>
		/// Returns <c>true</c> if <param name="other"></param> can be appended to this element. Only
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
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "The given element '" + other + "' cannot be appended to this element '" + this + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			// \fixme (2010-04-01 / MKY):
			// Weird ArgumentException when receiving large chunks of data.
			try
			{
				if (this.origin != null)
					this.origin.AddRange(PerformDeepClone(other.origin));

				if (this.text != null)
					this.text += other.text;

				this.byteCount += other.byteCount;
			}
			catch (ArgumentException ex)
			{
				MKY.Diagnostics.DebugEx.WriteException(GetType(), ex, other.ToString());
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
		public virtual string ToString(string indent)
		{
			var sb = new StringBuilder();

			sb.Append(indent); sb.Append("> Type:         "); sb.AppendLine(GetType().Name);
			sb.Append(indent); sb.Append("> Direction:    "); sb.AppendLine(Direction.ToString());
			sb.Append(indent); sb.Append("> Origin:       "); sb.AppendLine(Origin != null ? Origin.ToString() : "'null'");
			sb.Append(indent); sb.Append("> Text:         "); sb.AppendLine(Text   != null ? Text              :    ""   );
			sb.Append(indent); sb.Append("> ByteCount:    "); sb.AppendLine(ByteCount.ToString(CultureInfo.InvariantCulture));
			sb.Append(indent); sb.Append("> Flags:        "); sb.AppendLine(Attributes.ToString());

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
