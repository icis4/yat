﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml.Serialization;

using MKY.Collections.Generic;

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
	[XmlInclude(typeof(DisplayElement.Error))]
	public class DisplayElement
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <summary></summary>
		public class NoData : DisplayElement
		{
			/// <summary></summary>
			public NoData()
				: base()
			{
			}
		}

		/// <summary></summary>
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
		public class LeftMargin : DisplayElement
		{
			/// <summary></summary>
			public LeftMargin()
				: base(" ")
			{
			}
		}

		/// <summary></summary>
		public class Space : DisplayElement
		{
			/// <summary></summary>
			public Space()
				: base(" ")
			{
			}
		}

		/// <summary></summary>
		public class RightMargin : DisplayElement
		{
			/// <summary></summary>
			public RightMargin()
				: base(" ")
			{
			}
		}

		/// <summary></summary>
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
		public class Error : DisplayElement
		{
			/// <summary></summary>
			public Error()
				: base()
			{
			}

			/// <summary></summary>
			public Error(string message)
				: base('<' + message + '>')
			{
			}

			/// <summary></summary>
			public Error(SerialDirection direction, string message)
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
		{
			Initialize(SerialDirection.None, new List<Pair<byte[], string>>(), "", 0, false, false);
		}

		/// <summary></summary>
		private DisplayElement(string text)
		{
			Initialize(SerialDirection.None, new List<Pair<byte[], string>>(), text, 0, false, false);
		}

		/// <summary></summary>
		private DisplayElement(SerialDirection direction, string text)
		{
			Initialize(direction, new List<Pair<byte[], string>>(), text, 0, false, false);
		}

		/// <summary></summary>
		private DisplayElement(SerialDirection direction, byte origin, string text)
		{
			List<Pair<byte[], string>> l = new List<Pair<byte[], string>>();
			l.Add(new Pair<byte[], string>(new byte[] { origin }, text));
			Initialize(direction, l, text, 1, true, false);
		}

		/// <summary></summary>
		private DisplayElement(SerialDirection direction, byte[] origin, string text, int dataCount)
		{
			List<Pair<byte[], string>> l = new List<Pair<byte[], string>>();
			l.Add(new Pair<byte[], string>(origin, text));
			Initialize(direction, l, text, dataCount, true, false);
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
			this.origin = origin;
			this.text = text;
			this.dataCount = dataCount;
			this.isData = isData;
			this.isEol = isEol;
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
		[XmlAttribute("Origin")]
		public virtual List<Pair<byte[], string>> Origin
		{
			get { return (this.origin); }
			set { this.origin = value; }
		}

		/// <summary></summary>
		[XmlIgnore()]
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
			//ConstructorInfo ci = this.GetType().GetConstructor(Type.EmptyTypes);
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
			else if (this is Error)       de = new Error();
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
		/// Compares too display elements and returns <c>true</c> if both are of the same kind.
		/// </summary>
		public virtual bool IsSameKindAs(DisplayElement de)
		{
			if (this.GetType() != de.GetType())
				return (false);

			if (this.direction != de.direction)
				return (false);

			if (this.isData != de.isData)
				return (false);

			if (this.isEol != de.isEol)
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
			if (this.GetType() != de.GetType())
				throw (new InvalidOperationException("Cannot append because type doesn't match"));

			if (this.direction != de.direction)
				throw (new InvalidOperationException("Cannot append because direction doesn't match"));

			if (this.isData != de.isData)
				throw (new InvalidOperationException("Cannot append because kind doesn't match"));

			if (this.isEol != de.isEol)
				throw (new InvalidOperationException("Cannot append because EOL doesn't match"));

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
				MKY.Diagnostics.DebugEx.WriteException(this.GetType(), ex);
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
			sb.Append(indent); sb.Append("- DataCount: "); sb.AppendLine(this.dataCount.ToString(NumberFormatInfo.InvariantInfo));
			sb.Append(indent); sb.Append("- IsData:    "); sb.AppendLine(this.isData.ToString());
			sb.Append(indent); sb.Append("- IsEol:     "); sb.AppendLine(this.isEol.ToString());

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================