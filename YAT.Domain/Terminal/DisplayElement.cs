using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.Utilities.Types;

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
				: base("(" + timeStamp.ToLongTimeString() + "." + XString.Left((timeStamp.Millisecond/10).ToString("D2"), 2) + ")", false)
			{
			}

			/// <summary></summary>
			public TimeStamp()
				: base("(" + DateTime.Now.ToLongTimeString() + "." + XString.Left((DateTime.Now.Millisecond/10).ToString("D2"), 2) + ")", false)
			{
			}
		}

		/// <summary></summary>
		public class LineLength : DisplayElement
		{
			/// <summary></summary>
			public LineLength()
				: base("(" + 0.ToString() + ")", false)
			{
			}

			/// <summary></summary>
			public LineLength(int length)
				: base("(" + length.ToString() + ")", false)
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

		/// <summary></summary>
		override public string ToString()
		{
			return (ToString(""));
		}

		/// <summary></summary>
		public string ToString(string indent)
		{
			return (indent + "- Type: " + GetType().Name + Environment.NewLine +
					indent + "- Text: " + _text + Environment.NewLine +
					indent + "- IsDataElement: " + _isDataElement.ToString() + Environment.NewLine +
					indent + "- IsEol: " + _isEol.ToString() + Environment.NewLine);
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
