//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
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

using MKY.Collections.Generic;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	[Serializable]
	[XmlInclude(typeof(DisplayElement.NoData))]
	[XmlInclude(typeof(DisplayElement.TxData))]
	[XmlInclude(typeof(DisplayElement.TxControl))]
	[XmlInclude(typeof(DisplayElement.RxData))]
	[XmlInclude(typeof(DisplayElement.RxControl))]
	[XmlInclude(typeof(DisplayElement.TimeStamp))]
	[XmlInclude(typeof(DisplayElement.LineLength))]
	[XmlInclude(typeof(DisplayElement.LeftMargin))]
	[XmlInclude(typeof(DisplayElement.Space))]
	[XmlInclude(typeof(DisplayElement.RightMargin))]
	[XmlInclude(typeof(DisplayElement.LineBreak))]
	[XmlInclude(typeof(DisplayElement.IOError))]
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
				: base()
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
				: base(SerialDirection.Tx, origin, data)
			{
			}

			/// <summary></summary>
			public TxData(byte[] origin, string data, int dataCount)
				: base(SerialDirection.Tx, origin, data, dataCount)
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
				: base(SerialDirection.Tx, origin, control)
			{
			}

			/// <summary></summary>
			public TxControl(byte[] origin, string control, int controlCount)
				: base(SerialDirection.Tx, origin, control, controlCount)
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
				: base(SerialDirection.Rx, origin, data)
			{
			}

			/// <summary></summary>
			public RxData(byte[] origin, string data, int dataCount)
				: base(SerialDirection.Rx, origin, data, dataCount)
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
				: base(SerialDirection.Rx, origin, control)
			{
			}

			/// <summary></summary>
			public RxControl(byte[] origin, string control, int controlCount)
				: base(SerialDirection.Rx, origin, control, controlCount)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class TimeStamp : DisplayElement
		{
			/// <summary></summary>
			public TimeStamp()
				: base("(" + DateTime.Now.ToString("HH:mm:ss.ff", DateTimeFormatInfo.InvariantInfo) + ")")
			{
			}

			/// <summary></summary>
			public TimeStamp(DateTime timeStamp)
				: base("(" + timeStamp.ToString("HH:mm:ss.ff", DateTimeFormatInfo.InvariantInfo) + ")")
			{
			}

			/// <summary></summary>
			public TimeStamp(SerialDirection direction, DateTime timeStamp)
				: base(direction, "(" + timeStamp.ToString("HH:mm:ss.ff", DateTimeFormatInfo.InvariantInfo) + ")")
			{
			}

			/// <summary></summary>
			public TimeStamp(SerialDirection direction, string timeStamp)
				: base(direction, timeStamp)
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class LineLength : DisplayElement
		{
			/// <summary></summary>
			public LineLength()
				: base("(" + 0 + ")")
			{
			}

			/// <summary></summary>
			public LineLength(int length)
				: base("(" + length + ")")
			{
			}

			/// <summary></summary>
			public LineLength(SerialDirection direction, int length)
				: base(direction, "(" + length + ")")
			{
			}

			/// <summary></summary>
			public LineLength(SerialDirection direction, string length)
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
				: base(" ")
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class Space : DisplayElement
		{
			/// <summary></summary>
			public Space()
				: base(" ")
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class RightMargin : DisplayElement
		{
			/// <summary></summary>
			public RightMargin()
				: base(" ")
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class LineBreak : DisplayElement
		{
			/// <summary></summary>
			public LineBreak()
				: base()
			{
			}

			/// <summary></summary>
			public LineBreak(SerialDirection direction)
				: base(direction, "")
			{
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Well, this is what is intended here...")]
		public class IOError : DisplayElement
		{
			/// <summary></summary>
			public IOError()
				: base()
			{
			}

			/// <summary></summary>
			public IOError(string message)
				: base('<' + message + '>')
			{
			}

			/// <summary></summary>
			public IOError(SerialDirection direction, string message)
				: base(direction, '<' + message + '>')
			{
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SerialDirection direction;
		private List<Pair<byte[], string>> origin;
		private string text;
		private int dataCount;
		private bool isData;
		private bool isEol;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		private DisplayElement()
			: this("")
		{
		}

		/// <summary></summary>
		private DisplayElement(string text)
			: this(SerialDirection.None, text)
		{
		}

		/// <summary></summary>
		private DisplayElement(SerialDirection direction, string text)
		{
			Initialize(direction, new List<Pair<byte[], string>>(), text, 0, false, false);
		}

		/// <summary></summary>
		private DisplayElement(SerialDirection direction, byte origin, string text)
			: this(direction, new byte[] { origin }, text, 1)
		{
		}

		/// <summary></summary>
		private DisplayElement(SerialDirection direction, byte[] origin, string text, int dataCount)
			: this(direction, origin, text, dataCount, false)
		{
		}

		/// <summary></summary>
		private DisplayElement(SerialDirection direction, byte[] origin, string text, int dataCount, bool isEol)
		{
			List<Pair<byte[], string>> l = new List<Pair<byte[], string>>();
			l.Add(new Pair<byte[], string>(origin, text));
			Initialize(direction, l, text, dataCount, true, isEol);
		}

		private void Initialize(SerialDirection direction, List<Pair<byte[], string>> origin, string text, int dataCount, bool isData, bool isEol)
		{
			this.direction = direction;
			this.origin    = origin;
			this.text      = text;
			this.dataCount = dataCount;
			this.isData    = isData;
			this.isEol     = isEol;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlAttribute("Direction")]
		public virtual SerialDirection Direction
		{
			get { return (this.direction); }
			set { this.direction = value; }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlAttribute("Origin")]
		public virtual List<Pair<byte[], string>> Origin
		{
			get { return (this.origin); }
			set { this.origin = value; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int OriginCount
		{
			get { return (this.origin.Count); }
		}

		/// <summary></summary>
		[XmlAttribute("Text")]
		public virtual string Text
		{
			get { return (this.text); }
			set { this.text = value; }
		}

		/// <summary></summary>
		[XmlAttribute("DataCount")]
		public virtual int DataCount
		{
			get { return (this.dataCount); }
			set { this.dataCount = value; }
		}

		/// <summary></summary>
		[XmlAttribute("IsData")]
		public virtual bool IsData
		{
			get { return (this.isData); }
			set { this.isData = value; }
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

		#endregion

		#region Static  Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary></summary>
		public static DisplayElement Recreate(DisplayElement baseElement, Pair<byte[], string> origin)
		{
			DisplayElement clone = baseElement.Clone(); // Ensure to recreate the proper type.

			// Keep direction, isData and isEol.

			// Replace rest based on given origin.
			List<Pair<byte[], string>> l = new List<Pair<byte[], string>>();
			l.Add(origin);
			clone.origin = l;
			clone.text = origin.Value2;
			clone.dataCount = 1;

			return (clone);
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual DisplayElement Clone()
		{
			// Ensure to use the proper constructor.

			// Attention: For performance reasons, reflection as shown below is not used
			//ConstructorInfo ci = GetType().GetConstructor(Type.EmptyTypes);
			//DisplayElement de = (DisplayElement)ci.Invoke(null);

			DisplayElement de;

			if      (this is TxData)      de = new TxData();
			else if (this is TxControl)   de = new TxControl();
			else if (this is RxData)      de = new RxData();
			else if (this is RxControl)   de = new RxControl();
			else if (this is TimeStamp)   de = new TimeStamp();
			else if (this is LineLength)  de = new LineLength();
			else if (this is LeftMargin)  de = new LeftMargin();
			else if (this is Space)       de = new Space();
			else if (this is RightMargin) de = new RightMargin();
			else if (this is LineBreak)   de = new LineBreak();
			else if (this is IOError)     de = new IOError();
			else throw (new TypeLoadException("Unknown display element type"));

			de.direction = this.direction;
			de.origin    = this.origin;
			de.text      = this.text;
			de.dataCount = this.dataCount;
			de.isData    = this.isData;
			de.isEol     = this.isEol;

			return (de);
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
				throw (new InvalidOperationException("Cannot append because the given element cannot be appended to this element"));

			// \fixme (2010-04-01 / mky):
			// Weird ArgumentException when receiving large chunks of data.
			try
			{
				this.origin.AddRange(de.origin);
				this.text      += de.text;
				this.dataCount += de.dataCount;
			}
			catch (ArgumentException ex)
			{
				MKY.Diagnostics.DebugEx.WriteException(GetType(), ex);
				System.Diagnostics.Debug.WriteLine(de.ToString());
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
			return (this.text);
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

			sb.Append(indent); sb.Append("- Type:      "); sb.AppendLine(GetType().Name);
			sb.Append(indent); sb.Append("- Direction: "); sb.AppendLine(this.direction.ToString());
			sb.Append(indent); sb.Append("- Origin:    "); sb.AppendLine(this.origin.ToString());
			sb.Append(indent); sb.Append("- Text:      "); sb.AppendLine(this.text);
			sb.Append(indent); sb.Append("- DataCount: "); sb.AppendLine(this.dataCount.ToString(CultureInfo.InvariantCulture));
			sb.Append(indent); sb.Append("- IsData:    "); sb.AppendLine(this.isData.ToString());
			sb.Append(indent); sb.Append("- IsEol:     "); sb.AppendLine(this.isEol.ToString());

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
