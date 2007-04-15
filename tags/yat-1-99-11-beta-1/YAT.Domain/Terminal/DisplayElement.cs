using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Domain
{
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
		public class NoData : DisplayElement
		{
			public NoData()
				: base("", false)
			{
			}
		}

		public class TxData : DisplayElement
		{
			public TxData()
				: base()
			{
			}

			public TxData(ElementOrigin origin, string data)
				: base(origin, data, true)
			{
			}
		}

		public class TxControl : DisplayElement
		{
			public TxControl()
				: base()
			{
			}

			public TxControl(ElementOrigin origin, string control)
				: base(origin, control, true)
			{
			}
		}

		public class RxData : DisplayElement
		{
			public RxData()
				: base()
			{
			}

			public RxData(ElementOrigin origin, string data)
				: base(origin, data, true)
			{
			}
		}

		public class RxControl : DisplayElement
		{
			public RxControl()
				: base()
			{
			}

			public RxControl(ElementOrigin origin, string control)
				: base(origin, control, true)
			{
			}
		}

		public class TimeStamp : DisplayElement
		{
			public TimeStamp(DateTime timeStamp)
				: base("(" + timeStamp.ToLongTimeString() + "." + Utilities.Types.XString.Left((timeStamp.Millisecond/10).ToString("D2"), 2) + ")", false)
			{
			}

			public TimeStamp()
				: base("(" + DateTime.Now.ToLongTimeString() + "." + Utilities.Types.XString.Left((DateTime.Now.Millisecond/10).ToString("D2"), 2) + ")", false)
			{
			}
		}

		public class LineLength : DisplayElement
		{
			public LineLength()
				: base("(" + 0.ToString() + ")", false)
			{
			}

			public LineLength(int length)
				: base("(" + length.ToString() + ")", false)
			{
			}
		}

		public class LeftMargin : DisplayElement
		{
			public LeftMargin()
				: base(" ", false)
			{
			}
		}

		public class Space : DisplayElement
		{
			public Space()
				: base(" ", false)
			{
			}
		}

		public class RightMargin : DisplayElement
		{
			public RightMargin()
				: base(" ", false)
			{
			}
		}

		public class LineBreak : DisplayElement
		{
			public LineBreak()
				: base(Environment.NewLine, false, true)
			{
			}
		}

		public class Error : DisplayElement
		{
			public Error()
				: base("", false)
			{
			}

			public Error(string message)
				: base(message, false)
			{
			}
		}

		private ElementOrigin _origin;
		private string _text;
		private bool _isDataElement;
		private bool _isEol;

		public DisplayElement()
		{
			Initialize(null, "", true, false);
		}

		public DisplayElement(string text, bool isDataElement)
		{
			Initialize(null, text, isDataElement, false);
		}

		public DisplayElement(ElementOrigin origin, string text, bool isDataElement)
		{
			Initialize(origin, text, isDataElement, false);
		}

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

		[XmlAttribute("Origin")]
		public ElementOrigin Origin
		{
			get { return (_origin); }
			set { _origin = value; }
		}

		[XmlAttribute("Text")]
		public string Text
		{
			get { return (_text); }
			set { _text = value; }
		}

		[XmlAttribute("IsDataElement")]
		public bool IsDataElement
		{
			get { return (_isDataElement); }
			set { _isDataElement = value; }
		}

		[XmlIgnore]
		public bool IsNoDataElement
		{
			get { return (!_isDataElement); }
		}

		[XmlAttribute("IsEol")]
		public bool IsEol
		{
			get { return (_isEol); }
			set { _isEol = value; }
		}

		override public string ToString()
		{
			return (ToString(""));
		}

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

		public ElementOrigin(byte data, SerialDirection direction)
		{
			_data = data;
			_direction = direction;
		}

		public byte Data
		{
			get { return (_data); }
		}

		public SerialDirection Direction
		{
			get { return (_direction); }
		}
	}
}
