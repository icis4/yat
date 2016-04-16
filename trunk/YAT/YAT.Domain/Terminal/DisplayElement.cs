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
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	[Serializable]
	[XmlInclude(typeof(NoData))]
	[XmlInclude(typeof(TxData))]
	[XmlInclude(typeof(TxControl))]
	[XmlInclude(typeof(RxData))]
	[XmlInclude(typeof(RxControl))]
	[XmlInclude(typeof(DateInfo))]
	[XmlInclude(typeof(TimeInfo))]
	[XmlInclude(typeof(PortInfo))]
	[XmlInclude(typeof(DirectionInfo))]
	[XmlInclude(typeof(Length))]
	[XmlInclude(typeof(LeftMargin))]
	[XmlInclude(typeof(Space))]
	[XmlInclude(typeof(RightMargin))]
	[XmlInclude(typeof(LineBreak))]
	[XmlInclude(typeof(ErrorInfo))]
	public class DisplayElement
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class NoData : DisplayElement
		{
			/// <summary></summary>
			public NoData()
				: base(true)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TxData : DisplayElement
		{
			/// <summary></summary>
			public TxData()
				: base()
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
				: base()
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
				: base()
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
				: base()
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
		public class DateInfo : DisplayElement
		{
			/// <summary></summary>
			public const string Format = "yyyy-MM-dd";

			/// <summary></summary>
			public DateInfo()
				: base("(" + DateTime.Now.ToString(Format, DateTimeFormatInfo.InvariantInfo) + ")")
			{
			}

			/// <summary></summary>
			public DateInfo(DateTime timeStamp)
				: base("(" + timeStamp.ToString(Format, DateTimeFormatInfo.InvariantInfo) + ")")
			{
			}

			/// <summary></summary>
			public DateInfo(Direction direction, DateTime timeStamp)
				: base(direction, "(" + timeStamp.ToString(Format, DateTimeFormatInfo.InvariantInfo) + ")")
			{
			}

			/// <summary></summary>
			public DateInfo(Direction direction, string timeStamp)
				: base(direction, timeStamp)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TimeInfo : DisplayElement
		{
			/// <remarks>
			/// Output milliseconds for readability, even though last digit only provides limited accuracy.
			/// </remarks>
			public const string Format = "HH:mm:ss.fff";

			/// <summary></summary>
			public TimeInfo()
				: base("(" + DateTime.Now.ToString(Format, DateTimeFormatInfo.InvariantInfo) + ")")
			{
			}

			/// <summary></summary>
			public TimeInfo(DateTime timeStamp)
				: base("(" + timeStamp.ToString(Format, DateTimeFormatInfo.InvariantInfo) + ")")
			{
			}

			/// <summary></summary>
			public TimeInfo(Direction direction, DateTime timeStamp)
				: base(direction, "(" + timeStamp.ToString(Format, DateTimeFormatInfo.InvariantInfo) + ")")
			{
			}

			/// <summary></summary>
			public TimeInfo(Direction direction, string timeStamp)
				: base(direction, timeStamp)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class PortInfo : DisplayElement
		{
			/// <summary></summary>
			public PortInfo()
				: base(Direction.None, null)
			{
			}

			/// <summary></summary>
			public PortInfo(Direction direction, string info)
				: base(direction, "(" + info + ")")
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class DirectionInfo : DisplayElement
		{
			/// <summary></summary>
			public DirectionInfo()
				: base(Direction.None, "(" + (DirectionEx)Direction.None + ")")
			{
			}

			/// <summary></summary>
			public DirectionInfo(Direction direction)
				: base(direction, "(" + (DirectionEx)direction + ")")
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class Length : DisplayElement
		{
			/// <summary></summary>
			public Length()
				: base("(" + 0 + ")")
			{
			}

			/// <summary></summary>
			public Length(int length)
				: base("(" + length + ")")
			{
			}

			/// <summary></summary>
			public Length(Direction direction, int length)
				: base(direction, "(" + length + ")")
			{
			}

			/// <summary></summary>
			public Length(Direction direction, string length)
				: base(direction, length)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class LeftMargin : DisplayElement
		{
			/// <summary></summary>
			public LeftMargin()
				: base(" ", true)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class Space : DisplayElement
		{
			/// <summary></summary>
			public Space()
				: base(" ", true)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class RightMargin : DisplayElement
		{
			/// <summary></summary>
			public RightMargin()
				: base(" ", true)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class LineBreak : DisplayElement
		{
			/// <summary></summary>
			public LineBreak()
				: base(true)
			{
			}

			/// <summary></summary>
			public LineBreak(Direction direction)
				: base(direction, null, true)
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
				: base()
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
			public ErrorInfo(Direction direction, string message, bool warningOnly)
				: base(direction, (warningOnly ? ("<Warning: " + message + ">") : ("<Error: " + message + ">")))
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
		private bool isData;
		private bool isEol;
		private bool isWhiteSpace;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		private DisplayElement()
			: this(null)
		{
		}

		/// <summary></summary>
		private DisplayElement(string text)
			: this(Direction.None, text)
		{
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, string text)
		{
			Initialize(direction, null, text, 0, false, false, false);
		}

		/// <summary></summary>
		private DisplayElement(bool isWhiteSpace)
			: this(null, isWhiteSpace)
		{
		}

		/// <summary></summary>
		private DisplayElement(string text, bool isWhiteSpace)
			: this(Direction.None, text, isWhiteSpace)
		{
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, string text, bool isWhiteSpace)
		{
			Initialize(direction, null, text, 0, false, false, isWhiteSpace);
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, byte origin, string text)
			: this(direction, new byte[] { origin }, text, 1)
		{
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, byte[] origin, string text, int dataCount)
			: this(direction, origin, text, dataCount, false)
		{
		}

		/// <summary></summary>
		private DisplayElement(Direction direction, byte[] origin, string text, int dataCount, bool isEol)
		{
			List<Pair<byte[], string>> l = new List<Pair<byte[], string>>(1); // Preset the required capactiy to improve memory management.
			l.Add(new Pair<byte[], string>(origin, text));
			Initialize(direction, l, text, dataCount, true, isEol, false);
		}

		private void Initialize(Direction direction, List<Pair<byte[], string>> origin, string text, int dataCount, bool isData, bool isEol, bool isWhiteSpace)
		{
			this.direction    = direction;
			this.origin       = origin;
			this.text         = text;
			this.dataCount    = dataCount;
			this.isData       = isData;
			this.isEol        = isEol;
			this.isWhiteSpace = isWhiteSpace;
		}

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
		[XmlAttribute("IsData")]
		public virtual bool IsData
		{
			get { return (this.isData); }
			set { this.isData = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsNoData
		{
			get { return (!this.isData); }
		}

		/// <summary></summary>
		[XmlAttribute("IsEol")]
		public virtual bool IsEol
		{
			get { return (this.isEol); }
			set { this.isEol = value; }
		}

		/// <summary></summary>
		[XmlAttribute("IsWhiteSpace")]
		public virtual bool IsWhiteSpace
		{
			get { return (this.isWhiteSpace); }
			set { this.isWhiteSpace = value;  }
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

			if      (this is NoData)        clone = new NoData();
			else if (this is TxData)        clone = new TxData();
			else if (this is TxControl)     clone = new TxControl();
			else if (this is RxData)        clone = new RxData();
			else if (this is RxControl)     clone = new RxControl();
			else if (this is DateInfo)      clone = new DateInfo();
			else if (this is TimeInfo)      clone = new TimeInfo();
			else if (this is PortInfo)      clone = new PortInfo();
			else if (this is DirectionInfo) clone = new DirectionInfo();
			else if (this is Length)        clone = new Length();
			else if (this is LeftMargin)    clone = new LeftMargin();
			else if (this is Space)         clone = new Space();
			else if (this is RightMargin)   clone = new RightMargin();
			else if (this is LineBreak)     clone = new LineBreak();
			else if (this is ErrorInfo)     clone = new ErrorInfo();
			else throw (new TypeLoadException("Program execution should never get here, '" + this.GetType() + "' is an unknown display element type." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			clone.direction = this.direction;
			clone.origin    = PerformDeepClone(this.origin);
			clone.text      = this.text;
			clone.dataCount = this.dataCount;
			clone.isData    = this.isData;
			clone.isEol     = this.isEol;

			return (clone);
		}

		/// <summary></summary>
		public virtual DisplayElement RecreateFromOriginItem(Pair<byte[], string> originItem)
		{
			DisplayElement clone = Clone(); // Ensure to recreate the proper type.

			// Keep direction, isData and isEol.

			// Replace origin and dataCount.
			List<Pair<byte[], string>> clonedOrigin = new List<Pair<byte[], string>>(1); // Preset the required capactiy to improve memory management.
			clonedOrigin.Add(PerformDeepClone(originItem));
			clone.origin = clonedOrigin;
			clone.dataCount = 1;

			// Replace text.
			string text = originItem.Value2;
			clone.text = text;

			return (clone);
		}

		/// <summary>
		/// Returns <c>true</c> if <param name="de"></param> can be appended to this element. Only
		/// data elements of the same direction can be appended. Appending other elements would
		/// lead to missing elements.
		/// </summary>
		/// <remarks>
		/// Note that the type of the element also has to be checked. This ensures that control
		/// elements are not appended to 'normal' data elements.
		/// </remarks>
		public virtual bool AcceptsAppendOf(DisplayElement de)
		{
			if (GetType() != de.GetType())
				return (false);

			if (!this.isData || !de.isData) // Disallow non-data elements.
				return (false);

			if (this.direction != de.direction) // Self-explaining.
				return (false);

			if (this.isEol != de.isEol) // Check needed to deal with incomplete EOL sequences.
				return (false);

			if (this.isWhiteSpace != de.isWhiteSpace) // Self-explaining.
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
		public virtual void Append(DisplayElement de)
		{
			if (!AcceptsAppendOf(de))
				throw (new InvalidOperationException(@"Program execution should never get here, the given element """ + de + @""" cannot be appended to this element """ + this + @"""!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			// \fixme (2010-04-01 / MKY):
			// Weird ArgumentException when receiving large chunks of data.
			try
			{
				if (this.origin != null)
					this.origin.AddRange(PerformDeepClone(de.origin));

				if (this.text != null)
					this.text += de.text;

				this.dataCount += de.dataCount;
			}
			catch (ArgumentException ex)
			{
				MKY.Diagnostics.DebugEx.WriteException(GetType(), ex, de.ToString());
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
				List<Pair<byte[], string>> clone = new List<Pair<byte[], string>>(origin.Capacity); // Preset the required capactiy to improve memory management.

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
			if (this.text != null)
				return (this.text);
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
			sb.Append(indent); sb.Append("> Direction:    "); sb.AppendLine(this.direction.ToString());
			sb.Append(indent); sb.Append("> Origin:       "); sb.AppendLine(this.origin != null ? this.origin.ToString() : "'null'");
			sb.Append(indent); sb.Append("> Text:         "); sb.AppendLine(this.text   != null ? this.text              :    ""   );
			sb.Append(indent); sb.Append("> DataCount:    "); sb.AppendLine(this.dataCount.ToString(CultureInfo.InvariantCulture));
			sb.Append(indent); sb.Append("> IsData:       "); sb.AppendLine(this.isData.ToString());
			sb.Append(indent); sb.Append("> IsEol:        "); sb.AppendLine(this.isEol.ToString());
			sb.Append(indent); sb.Append("> IsWhiteSpace: "); sb.AppendLine(this.isWhiteSpace.ToString());

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
