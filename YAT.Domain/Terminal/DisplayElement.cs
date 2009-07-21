//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

using MKY.Utilities.Types;

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
	[XmlInclude(typeof(DisplayElement.LineBreak))]
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
			public TxData(byte original, string data)
				: base(SerialDirection.Tx, original, data)
			{
			}

			/// <summary></summary>
			public TxData(byte[] original, string data, int dataCount)
				: base(SerialDirection.Tx, original, data, dataCount)
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
			public TxControl(byte original, string control)
				: base(SerialDirection.Tx, original, control)
			{
			}

			/// <summary></summary>
			public TxControl(byte[] original, string control, int controlCount)
				: base(SerialDirection.Tx, original, control, controlCount)
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
			public RxData(byte original, string data)
				: base(SerialDirection.Rx, original, data)
			{
			}

			/// <summary></summary>
			public RxData(byte[] original, string data, int dataCount)
				: base(SerialDirection.Rx, original, data, dataCount)
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
			public RxControl(byte original, string control)
				: base(SerialDirection.Rx, original, control)
			{
			}

			/// <summary></summary>
			public RxControl(byte[] original, string control, int controlCount)
				: base(SerialDirection.Rx, original, control, controlCount)
			{
			}
		}

		/// <summary></summary>
		public class TimeStamp : DisplayElement
		{
			/// <summary></summary>
			public TimeStamp()
				: base("(" + DateTime.Now.ToString("HH:mm:ss.ff", DateTimeFormatInfo.InvariantInfo) + ")")
//				: base("(" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffzzz", DateTimeFormatInfo.InvariantInfo) + ")")
//				: base("(" + DateTime.Now.ToLongTimeString() + "." + XString.Left((DateTime.Now.Millisecond/10).ToString("D2"), 2) + ")")
			{
			}

			/// <summary></summary>
			public TimeStamp(DateTime timeStamp)
				: base("(" + timeStamp.ToString("HH:mm:ss.ff", DateTimeFormatInfo.InvariantInfo) + ")")
			//				: base("(" + timeStamp.ToString("yyyy-MM-dd HH:mm:ss.ffzzz", DateTimeFormatInfo.InvariantInfo) + ")")
			//				: base("(" + timeStamp.ToLongTimeString() + "." + XString.Left((timeStamp.Millisecond/10).ToString("D2"), 2) + ")")
			{
			}

			/// <summary></summary>
			public TimeStamp(SerialDirection direction, DateTime timeStamp)
				: base(direction, "(" + timeStamp.ToString("HH:mm:ss.ff", DateTimeFormatInfo.InvariantInfo) + ")")
//				: base(direction, "(" + timeStamp.ToString("yyyy-MM-dd HH:mm:ss.ffzzz", DateTimeFormatInfo.InvariantInfo) + ")")
//				: base(direction, "(" + timeStamp.ToLongTimeString() + "." + XString.Left((timeStamp.Millisecond/10).ToString("D2"), 2) + ")")
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
				: base(message)
			{
			}

			/// <summary></summary>
			public Error(SerialDirection direction, string message)
				: base(direction, message)
			{
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SerialDirection _direction;
		private List<byte> _origin;
		private string _text;
		private int _dataCount;
		private bool _isData;
		private bool _isEol;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public DisplayElement()
		{
			Initialize(SerialDirection.None, new List<byte>(), "", 0, false, false);
		}

		/// <summary></summary>
		public DisplayElement(string text)
		{
			Initialize(SerialDirection.None, new List<byte>(), text, 0, false, false);
		}

		/// <summary></summary>
		public DisplayElement(SerialDirection direction, string text)
		{
			Initialize(direction, new List<byte>(), text, 0, false, false);
		}

		/// <summary></summary>
		public DisplayElement(SerialDirection direction, byte origin, string text)
		{
			List<byte> l = new List<byte>();
			l.Add(origin);
			Initialize(direction, l, text, 1, true, false);
		}

		/// <summary></summary>
		public DisplayElement(SerialDirection direction, byte[] origin, string text, int dataCount)
		{
			List<byte> l = new List<byte>();
			l.AddRange(origin);
			Initialize(direction, l, text, dataCount, true, false);
		}

		/// <summary></summary>
		public DisplayElement(SerialDirection direction, byte[] origin, string text, int dataCount, bool isEol)
		{
			List<byte> l = new List<byte>();
			l.AddRange(origin);
			Initialize(direction, l, text, dataCount, true, isEol);
		}

		/// <summary></summary>
		public DisplayElement(DisplayElement de)
		{
			List<byte> l = new List<byte>();
			l.AddRange(de._origin);
			Initialize(de._direction, l, de._text, de._dataCount, de._isData, de._isEol);
		}

		private void Initialize(SerialDirection direction, List<byte> origin, string text, int dataCount, bool isDataElement, bool isEol)
		{
			_direction = direction;
			_origin = origin;
			_text = text;
			_dataCount = dataCount;
			_isData = isDataElement;
			_isEol = isEol;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlAttribute("Direction")]
		public SerialDirection Direction
		{
			get { return (_direction); }
			set { _direction = value; }
		}

		/// <summary></summary>
		[XmlAttribute("Origin")]
		public List<byte> Origin
		{
			get { return (_origin); }
			set { _origin = value; }
		}

		/// <summary></summary>
		[XmlIgnore()]
		public int OriginCount
		{
			get { return (_origin.Count); }
		}

		/// <summary></summary>
		[XmlAttribute("Text")]
		public string Text
		{
			get { return (_text); }
			set { _text = value; }
		}

		/// <summary></summary>
		[XmlAttribute("DataCount")]
		public int DataCount
		{
			get { return (_dataCount); }
			set { _dataCount = value; }
		}

		/// <summary></summary>
		[XmlAttribute("IsData")]
		public bool IsData
		{
			get { return (_isData); }
			set { _isData = value; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public bool IsNoData
		{
			get { return (!_isData); }
		}

		/// <summary></summary>
		[XmlAttribute("IsEol")]
		public bool IsEol
		{
			get { return (_isEol); }
			set { _isEol = value; }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Compares too display elements and returns true if both are of the same kind.
		/// </summary>
		public bool IsSameKindAs(DisplayElement de)
		{
			if (this.GetType() != de.GetType())
				return (false);

			if (_direction != de._direction)
				return (false);

			if (_isData != de._isData)
				return (false);

			if (_isEol != de._isEol)
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
		public void Append(DisplayElement de)
		{
			if (this.GetType() != de.GetType())
				throw (new InvalidOperationException("Cannot append because type doesn't match"));

			if (_direction != de._direction)
				throw (new InvalidOperationException("Cannot append because direction doesn't match"));

			if (_isData != de._isData)
				throw (new InvalidOperationException("Cannot append because kind doesn't match"));

			if (_isEol != de._isEol)
				throw (new InvalidOperationException("Cannot append because EOL doesn't match"));

			_origin.AddRange(de._origin);
			_text += de._text;
			_dataCount += de._dataCount;
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
			return (_text);
		}

		/// <summary>
		/// Extended ToString method which can be used for trace/debug.
		/// </summary>
		public string ToString(string indent)
		{
			return (indent + "- Type: "      + GetType().Name + Environment.NewLine +
					indent + "- Direction: " + _direction     + Environment.NewLine +
					indent + "- Origin: "    + _origin        + Environment.NewLine +
					indent + "- Text: "      + _text          + Environment.NewLine +
					indent + "- DataCount: " + _dataCount     + Environment.NewLine +
					indent + "- IsData: "    + _isData        + Environment.NewLine +
					indent + "- IsEol: "     + _isEol         + Environment.NewLine);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
