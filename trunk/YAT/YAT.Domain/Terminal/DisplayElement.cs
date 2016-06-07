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
	[XmlInclude(typeof(Nothing))]
	[XmlInclude(typeof(TxData))]
	[XmlInclude(typeof(TxControl))]
	[XmlInclude(typeof(RxData))]
	[XmlInclude(typeof(RxControl))]
	[XmlInclude(typeof(InfoDisplayElement))]
	[XmlInclude(typeof(DateInfo))]
	[XmlInclude(typeof(TimeInfo))]
	[XmlInclude(typeof(PortInfo))]
	[XmlInclude(typeof(DirectionInfo))]
	[XmlInclude(typeof(DataLength))]
	[XmlInclude(typeof(WhiteSpaceDisplayElement))]
	[XmlInclude(typeof(DataSpace))]
	[XmlInclude(typeof(InfoSpace))]
	[XmlInclude(typeof(LineStart))]
	[XmlInclude(typeof(LineBreak))]
	[XmlInclude(typeof(ErrorInfo))]
	public abstract class DisplayElement
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <summary></summary>
		[Flags]
		public enum ModifierFlags
		{
			/// <summary></summary>
			None       =  0,

			/// <summary></summary>
			Data       =  1,

			/// <summary></summary>
			Eol        =  2,

			/// <summary></summary>
			Inline     =  4,

			/// <summary></summary>
			Info       =  8,

			/// <summary></summary>
			WhiteSpace = 16
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class Nothing : DisplayElement
		{
			/// <summary></summary>
			public Nothing()
				: base(ModifierFlags.None)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TxData : DisplayElement
		{
			/// <summary></summary>
			public TxData()
				: base(ModifierFlags.Data)
			{
			}

			/// <summary></summary>
			public TxData(byte origin, string data)
				: base(Direction.Tx, origin, data)
			{
			}

			/// <summary></summary>
			public TxData(byte[] origin, string data, int dataCount)
				: base(Direction.Tx, origin, data, dataCount)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TxControl : DisplayElement
		{
			/// <summary></summary>
			public TxControl()
				: base(ModifierFlags.Data)
			{
			}

			/// <summary></summary>
			public TxControl(byte origin, string control)
				: base(Direction.Tx, origin, control)
			{
			}

			/// <summary></summary>
			public TxControl(byte[] origin, string control, int controlCount)
				: base(Direction.Tx, origin, control, controlCount)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class RxData : DisplayElement
		{
			/// <summary></summary>
			public RxData()
				: base(ModifierFlags.Data)
			{
			}

			/// <summary></summary>
			public RxData(byte origin, string data)
				: base(Direction.Rx, origin, data)
			{
			}

			/// <summary></summary>
			public RxData(byte[] origin, string data, int dataCount)
				: base(Direction.Rx, origin, data, dataCount)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class RxControl : DisplayElement
		{
			/// <summary></summary>
			public RxControl()
				: base(ModifierFlags.Data)
			{
			}

			/// <summary></summary>
			public RxControl(byte origin, string control)
				: base(Direction.Rx, origin, control)
			{
			}

			/// <summary></summary>
			public RxControl(byte[] origin, string control, int controlCount)
				: base(Direction.Rx, origin, control, controlCount)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public abstract class InfoDisplayElement : DisplayElement
		{
			/// <summary></summary>
			public InfoDisplayElement(string info)
				: this(Direction.None, info)
			{
			}

			/// <summary></summary>
			public InfoDisplayElement(Direction direction, string info)
				: base(direction, info, ModifierFlags.Info)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class DateInfo : InfoDisplayElement
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
		public class TimeInfo : InfoDisplayElement
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
		public class PortInfo : InfoDisplayElement
		{
			/// <summary></summary>
			public PortInfo()
				: this(Direction.None, null, null, null)
			{
			}

			/// <summary></summary>
			public PortInfo(string info, string enclosureLeft, string enclosureRight)
				: this(Direction.None, info, enclosureLeft, enclosureRight)
			{
			}

			/// <summary></summary>
			public PortInfo(Direction direction, string info, string enclosureLeft, string enclosureRight)
				: base(direction, enclosureLeft + info + enclosureRight)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class DirectionInfo : InfoDisplayElement
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
		public class DataLength : InfoDisplayElement
		{
			/// <summary></summary>
			public DataLength()
				: this(0, null, null)
			{
			}

			/// <summary></summary>
			public DataLength(int length, string enclosureLeft, string enclosureRight)
				: this(Direction.None, length, enclosureLeft, enclosureRight)
			{
			}

			/// <summary></summary>
			public DataLength(Direction direction, int length, string enclosureLeft, string enclosureRight)
				: base(direction, enclosureLeft + length.ToString(CultureInfo.InvariantCulture) + enclosureRight)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public abstract class WhiteSpaceDisplayElement : DisplayElement
		{
			/// <summary></summary>
			public WhiteSpaceDisplayElement(Direction direction)
				: this(direction, null)
			{
			}

			/// <summary></summary>
			public WhiteSpaceDisplayElement(string whiteSpace)
				: this(Direction.None, whiteSpace)
			{
			}

			/// <summary></summary>
			public WhiteSpaceDisplayElement(Direction direction, string whiteSpace)
				: base(direction, whiteSpace, ModifierFlags.WhiteSpace)
			{
			}
		}

		/// <summary>The space that is added inbetween characters of the data content (i.e. radix = char).</summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'inbetween' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class DataSpace : WhiteSpaceDisplayElement
		{
			/// <summary></summary>
			public DataSpace()
				: this(Direction.None)
			{
			}

			/// <summary></summary>
			public DataSpace(Direction direction)
				: base(direction, " ") // Data space is fixed to a normal space.
			{
			}
		}

		/// <summary>The margin that is added to the right of the data content.</summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class InfoSpace : WhiteSpaceDisplayElement
		{
			/// <summary></summary>
			public InfoSpace()
				: this(Direction.None, null)
			{
			}

			/// <summary></summary>
			public InfoSpace(string space)
				: this(Direction.None, space)
			{
			}

			/// <summary></summary>
			public InfoSpace(Direction direction, string space)
				: base(direction, space)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class LineStart : WhiteSpaceDisplayElement
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
		public class LineBreak : WhiteSpaceDisplayElement
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
				: this(Direction.None, message, false)
			{
			}

			/// <summary></summary>
			public ErrorInfo(Direction direction, string message, bool isWarningOnly)
				: base(direction, (isWarningOnly ? ("<Warning: " + message + ">") : ("<Error: " + message + ">")), ModifierFlags.Inline)
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
		private int dataCount;
		private ModifierFlags flags;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		private DisplayElement()
			: this(ModifierFlags.None)
		{
		}

		/// <summary></summary>
		private DisplayElement(ModifierFlags flags)
			: this(Direction.None, null, ModifierFlags.None)
		{
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, string text, ModifierFlags flags)
		{
			Initialize(direction, null, text, 0, flags);
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, byte origin, string text)
			: this(direction, new byte[] { origin }, text, 1)
		{
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, byte[] origin, string text, int dataCount)
		{
			List<Pair<byte[], string>> l = new List<Pair<byte[], string>>(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the required capacity to improve memory management.
			l.Add(new Pair<byte[], string>(origin, text));
			Initialize(direction, l, text, dataCount, ModifierFlags.Data);
		}

		private void Initialize(Direction direction, List<Pair<byte[], string>> origin, string text, int dataCount, ModifierFlags flags)
		{
			this.direction = direction;
			this.origin    = origin;
			this.text      = text;
			this.dataCount = dataCount;
			this.flags     = flags;
		}

#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
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
		[XmlAttribute("DataCount")]
		public virtual int DataCount
		{
			get { return (this.dataCount); }
			set { this.dataCount = value;  }
		}

		/// <summary></summary>
		[XmlAttribute("Flags")]
		public virtual ModifierFlags Flags
		{
			get { return (this.flags); }
			set { this.flags = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsData
		{
			get { return ((this.flags & ModifierFlags.Data) != 0); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsEol
		{
			get { return ((this.flags & ModifierFlags.Eol) != 0); }
			set
			{
				if (value)
					this.flags |= ModifierFlags.Eol;
				else
					this.flags &= ~ModifierFlags.Eol;
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsInline
		{
			get { return ((this.flags & ModifierFlags.Inline) != 0); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsInfo
		{
			get { return ((this.flags & ModifierFlags.Info) != 0); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsWhiteSpace
		{
			get { return ((this.flags & ModifierFlags.WhiteSpace) != 0); }
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

			if      (this is Nothing)		clone = new Nothing();
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
			else if (this is InfoSpace)		clone = new InfoSpace();
			else if (this is LineStart)		clone = new LineStart();
			else if (this is LineBreak)		clone = new LineBreak();
			else if (this is ErrorInfo)		clone = new ErrorInfo();
			else throw (new TypeLoadException("Program execution should never get here, '" + GetType() + "' is an unknown display element type." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			clone.direction = this.direction;
			clone.origin    = PerformDeepClone(this.origin);
			clone.text      = this.text;
			clone.dataCount = this.dataCount;
			clone.flags     = this.flags;

			return (clone);
		}

		/// <summary></summary>
		public virtual DisplayElement RecreateFromOriginItem(Pair<byte[], string> originItem)
		{
			DisplayElement clone = Clone(); // Ensure to recreate the proper type.

			// Keep direction, isData and isEol.

			// Replace origin and dataCount.
			List<Pair<byte[], string>> clonedOrigin = new List<Pair<byte[], string>>(1); // Preset the required capacity to improve memory management.
			clonedOrigin.Add(PerformDeepClone(originItem));
			clone.origin = clonedOrigin;
			clone.dataCount = 1;

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

			if (!IsData || !other.IsData) // Disallow non-data elements.
				return (false);

			if (this.direction != other.direction) // Self-explaining.
				return (false);

			if (this.flags != other.flags) // Self-explaining.
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
				throw (new NotSupportedException(@"Program execution should never get here, the given element """ + other + @""" cannot be appended to this element """ + this + @"""!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			// \fixme (2010-04-01 / MKY):
			// Weird ArgumentException when receiving large chunks of data.
			try
			{
				if (this.origin != null)
					this.origin.AddRange(PerformDeepClone(other.origin));

				if (this.text != null)
					this.text += other.text;

				this.dataCount += other.dataCount;
			}
			catch (ArgumentException ex)
			{
				MKY.Diagnostics.DebugEx.WriteException(GetType(), ex, other.ToString());
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
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
		/// Standard ToString method returning the element contents only.
		/// </summary>
		public override string ToString()
		{
			if (Text != null)
				return (Text);
			else
				return ("");
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Extended ToString method which can be used for trace/debug.
		/// </summary>
		public virtual string ToString(string indent)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(indent); sb.Append("> Type:         "); sb.AppendLine(GetType().Name);
			sb.Append(indent); sb.Append("> Direction:    "); sb.AppendLine(Direction.ToString());
			sb.Append(indent); sb.Append("> Origin:       "); sb.AppendLine(Origin != null ? Origin.ToString() : "'null'");
			sb.Append(indent); sb.Append("> Text:         "); sb.AppendLine(Text   != null ? Text              :    ""   );
			sb.Append(indent); sb.Append("> DataCount:    "); sb.AppendLine(DataCount.ToString(CultureInfo.InvariantCulture));
			sb.Append(indent); sb.Append("> Flags:        "); sb.AppendLine(Flags.ToString());

			return (sb.ToString());
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
