using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace HSR.YAT.Gui.Settings
{
	public class LayoutSettings : Utilities.Settings.Settings
	{
		private bool _sendCommandPanelIsVisible;
		private bool _sendFilePanelIsVisible;
		private float _upperSplitterRatio;

		private bool _txMonitorPanelIsVisible;
		private bool _bidirMonitorPanelIsVisible;
		private bool _rxMonitorPanelIsVisible;
		private float _monitorLeftSplitterRatio;
		private float _monitorRightSplitterRatio;
		private Orientation _monitorOrientation;

		private bool _predefinedPanelIsVisible;
		private float _lowerSplitterRatio;

		public LayoutSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public LayoutSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public LayoutSettings(LayoutSettings rhs)
			: base(rhs)
		{
			SendCommandPanelIsVisible = rhs.SendCommandPanelIsVisible;
			SendFilePanelIsVisible    = rhs.SendFilePanelIsVisible;
			UpperSplitterRatio        = rhs.UpperSplitterRatio;

			TxMonitorPanelIsVisible    = rhs.TxMonitorPanelIsVisible;
			BidirMonitorPanelIsVisible = rhs.BidirMonitorPanelIsVisible;
			RxMonitorPanelIsVisible    = rhs.RxMonitorPanelIsVisible;
			MonitorLeftSplitterRatio   = rhs.MonitorLeftSplitterRatio;
			MonitorRightSplitterRatio  = rhs.MonitorRightSplitterRatio;
			MonitorOrientation         = rhs.MonitorOrientation;

			PredefinedPanelIsVisible = rhs.PredefinedPanelIsVisible;
			LowerSplitterRatio       = rhs.LowerSplitterRatio;

			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			SendCommandPanelIsVisible = true;
			SendFilePanelIsVisible = true;
			UpperSplitterRatio = (float)1 / 2;

			TxMonitorPanelIsVisible = false;
			BidirMonitorPanelIsVisible = true;
			RxMonitorPanelIsVisible = false;
			MonitorLeftSplitterRatio = (float)1 / 3;
			MonitorRightSplitterRatio = (float)1 / 2;
			MonitorOrientation = Orientation.Vertical;

			PredefinedPanelIsVisible = true;
			LowerSplitterRatio = (float)3 / 4;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("SendCommandPanelIsVisible")]
		public bool SendCommandPanelIsVisible
		{
			get { return (_sendCommandPanelIsVisible); }
			set
			{
				if (_sendCommandPanelIsVisible != value)
				{
					_sendCommandPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SendFilePanelIsVisible")]
		public bool SendFilePanelIsVisible
		{
			get { return (_sendFilePanelIsVisible); }
			set
			{
				if (_sendFilePanelIsVisible != value)
				{
					_sendFilePanelIsVisible = value;
					SetChanged();
				}
			}
		}

		[XmlElement("UpperSplitterRatio")]
		public float UpperSplitterRatio
		{
			get { return (_upperSplitterRatio); }
			set
			{
				if (_upperSplitterRatio != value)
				{
					_upperSplitterRatio = value;
					SetChanged();
				}
			}
		}

		[XmlElement("TxMonitorPanelIsVisible")]
		public bool TxMonitorPanelIsVisible
		{
			get { return (_txMonitorPanelIsVisible); }
			set
			{
				if (_txMonitorPanelIsVisible != value)
				{
					_txMonitorPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		[XmlElement("BidirMonitorPanelIsVisible")]
		public bool BidirMonitorPanelIsVisible
		{
			get { return (_bidirMonitorPanelIsVisible); }
			set
			{
				if (_bidirMonitorPanelIsVisible != value)
				{
					_bidirMonitorPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		[XmlElement("RxMonitorPanelIsVisible")]
		public bool RxMonitorPanelIsVisible
		{
			get { return (_rxMonitorPanelIsVisible); }
			set
			{
				if (_rxMonitorPanelIsVisible != value)
				{
					_rxMonitorPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		[XmlElement("MonitorLeftSplitterRatio")]
		public float MonitorLeftSplitterRatio
		{
			get { return (_monitorLeftSplitterRatio); }
			set
			{
				if (_monitorLeftSplitterRatio != value)
				{
					_monitorLeftSplitterRatio = value;
					SetChanged();
				}
			}
		}

		[XmlElement("MonitorRigtSplitterRatio")]
		public float MonitorRightSplitterRatio
		{
			get { return (_monitorRightSplitterRatio); }
			set
			{
				if (_monitorRightSplitterRatio != value)
				{
					_monitorRightSplitterRatio = value;
					SetChanged();
				}
			}
		}

		[XmlElement("MonitorOrientation")]
		public Orientation MonitorOrientation
		{
			get { return (_monitorOrientation); }
			set
			{
				if (_monitorOrientation != value)
				{
					_monitorOrientation = value;
					SetChanged();
				}
			}
		}

		[XmlElement("PredefinedPanelIsVisible")]
		public bool PredefinedPanelIsVisible
		{
			get { return (_predefinedPanelIsVisible); }
			set
			{
				if (_predefinedPanelIsVisible != value)
				{
					_predefinedPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		[XmlElement("LowerSplitterRatio")]
		public float LowerSplitterRatio
		{
			get { return (_lowerSplitterRatio); }
			set
			{
				if (_lowerSplitterRatio != value)
				{
					_lowerSplitterRatio = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is LayoutSettings)
				return (Equals((LayoutSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(LayoutSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_sendCommandPanelIsVisible.Equals(value._sendCommandPanelIsVisible) &&
					_sendFilePanelIsVisible.Equals(value._sendFilePanelIsVisible) &&
					_upperSplitterRatio.Equals(value._upperSplitterRatio) &&

					_txMonitorPanelIsVisible.Equals(value._txMonitorPanelIsVisible) &&
					_bidirMonitorPanelIsVisible.Equals(value._bidirMonitorPanelIsVisible) &&
					_rxMonitorPanelIsVisible.Equals(value._rxMonitorPanelIsVisible) &&
					_monitorLeftSplitterRatio.Equals(value._monitorLeftSplitterRatio) &&
					_monitorRightSplitterRatio.Equals(value._monitorRightSplitterRatio) &&
					_monitorOrientation.Equals(value._monitorOrientation) &&

					_predefinedPanelIsVisible.Equals(value._predefinedPanelIsVisible) &&
					_lowerSplitterRatio.Equals(value._lowerSplitterRatio)
					);
			}
			return (false);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
		/// </summary>
		public static bool operator ==(LayoutSettings lhs, LayoutSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(LayoutSettings lhs, LayoutSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}