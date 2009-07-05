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
		/// <summary></summary>
		public class NoData : DisplayElement
		{
			/// <summary></summary>
			public NoData()
				: base("", false)
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
			public TxData(ElementOrigin origin, string data)
				: base(origin, data, true)
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
			public TxControl(ElementOrigin origin, string control)
				: base(origin, control, true)
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
			public RxData(ElementOrigin origin, string data)
				: base(origin, data, true)
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
			public RxControl(ElementOrigin origin, string control)
				: base(origin, control, true)
			{
			}
		}

		/// <summary></summary>
		public class TimeStamp : DisplayElement
		{
			/// <summary></summary>
			public TimeStamp(DateTime timeStamp)
				: base("(" + timeStamp.ToString("HH:mm:ss.ff", DateTimeFormatInfo.InvariantInfo) + ")", false)
//				: base("(" + timeStamp.ToString("yyyy-MM-dd HH:mm:ss.ffzzz", DateTimeFormatInfo.InvariantInfo) + ")", false)
//				: base("(" + timeStamp.ToLongTimeString() + "." + XString.Left((timeStamp.Millisecond/10).ToString("D2"), 2) + ")", false)
			{
			}

			/// <summary></summary>
			public TimeStamp()
				: base("(" + DateTime.Now.ToString("HH:mm:ss.ff", DateTimeFormatInfo.InvariantInfo) + ")", false)
//				: base("(" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffzzz", DateTimeFormatInfo.InvariantInfo) + ")", false)
//				: base("(" + DateTime.Now.ToLongTimeString() + "." + XString.Left((DateTime.Now.Millisecond/10).ToString("D2"), 2) + ")", false)
			{
			}
		}

		/// <summary></summary>
		public class LineLength : DisplayElement
		{
			/// <summary></summary>
			public LineLength()
				: base("(" + 0 + ")", false)
			{
			}

			/// <summary></summary>
			public LineLength(int length)
				: base("(" + length + ")", false)
			{
			}
		}

		/// <summary></summary>
		public class LeftMargin : DisplayElement
		{
			/// <summary></summary>
			public LeftMargin()
				: base(" ", false)
			{
			}
		}

		/// <summary></summary>
		public class Space : DisplayElement
		{
			/// <summary></summary>
			public Space()
				: base(" ", false)
			{
			}
		}

		/// <summary></summary>
		public class RightMargin : DisplayElement
		{
			/// <summary></summary>
			public RightMargin()
				: base(" ", false)
			{
			}
		}

		/// <summary></summary>
		public class LineBreak : DisplayElement
		{
			/// <summary></summary>
			public LineBreak()
				: base(Environment.NewLine, false, true)
			{
			}
		}

		/// <summary></summary>
		public class Error : DisplayElement
		{
			/// <summary></summary>
			public Error()
				: base("", false)
			{
			}

			/// <summary></summary>
			public Error(string message)
				: base(message, false)
			{
			}
		}

		private ElementOrigin _origin;
		private string _text;
		private bool _isDataElement;
		private bool _isEol;

		/// <summary></summary>
		public DisplayElement()
		{
			Initialize(null, "", true, false);
		}

		/// <summary></summary>
		public DisplayElement(string text, bool isDataElement)
		{
			Initialize(null, text, isDataElement, false);
		}

		/// <summary></summary>
		public DisplayElement(ElementOrigin origin, string text, bool isDataElement)
		{
			Initialize(origin, text, isDataElement, false);
		}

		/// <summary></summary>
		public DisplayElement(string text, bool isDataElement, bool isEol)
		{
			Initialize(null, text, isDataElement, isEol);
		}

		private void Initialize(ElementOrigin origin, string text, bool isDataElement, bool isEol)
		{
			_origin = origin;
			_text = text;
			_isDataElement = isDataElement;
			_isEol = isEol;
		}

		/// <summary></summary>
		[XmlAttribute("Origin")]
		public ElementOrigin Origin
		{
			get { return (_origin); }
			set { _origin = value; }
		}

		/// <summary></summary>
		[XmlAttribute("Text")]
		public string Text
		{
			get { return (_text); }
			set { _text = value; }
		}

		/// <summary></summary>
		[XmlAttribute("IsDataElement")]
		public bool IsDataElement
		{
			get { return (_isDataElement); }
			set { _isDataElement = value; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public bool IsNoDataElement
		{
			get { return (!_isDataElement); }
		}

		/// <summary></summary>
		[XmlAttribute("IsEol")]
		public bool IsEol
		{
			get { return (_isEol); }
			set { _isEol = value; }
		}

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
			return (indent + "- Type: " + GetType().Name + Environment.NewLine +
					indent + "- Text: " + _text + Environment.NewLine +
					indent + "- IsDataElement: " + _isDataElement + Environment.NewLine +
					indent + "- IsEol: " + _isEol + Environment.NewLine);
		}
	}

	/// <summary>
	/// Stores data of the origin of the display element.
	/// </summary>
	public class ElementOrigin
	{
		private byte _data;
		private SerialDirection _direction;

		/// <summary></summary>
		public ElementOrigin(byte data, SerialDirection direction)
		{
			_data = data;
			_direction = direction;
		}

		/// <summary></summary>
		public byte Data
		{
			get { return (_data); }
		}

		/// <summary></summary>
		public SerialDirection Direction
		{
			get { return (_direction); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
