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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Event;

using YAT.Gui.Utilities;

namespace YAT.Gui.Controls
{
	public enum MonitorActivityState
	{
		Inactive,
		Active,
		Pending,
	}

	/// <summary>
	/// This monitor implements a list box based terminal monitor in a speed optimized way
	/// </summary>
	[DesignerCategory("Windows Forms")]
	public partial class Monitor : UserControl
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		public enum OpacityState
		{
			Inactive,
			Incrementing,
			Decrementing,
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.RepositoryType _RepositoryTypeDefault = Domain.RepositoryType.None;
		private const MonitorActivityState  _ActivityStateDefault  = MonitorActivityState.Inactive;

		private const double _MinimumImageOpacity   =  0.00; //   0%
		private const double _MaximumImageOpacity   =  1.00; // 100%
		private const double _ImageOpacityIncrement = +0.10; // +10%
		private const double _ImageOpacityDecrement = -0.10; // -10%

		private const int _MaximalLineCountDefault = Domain.Settings.DisplaySettings.MaximalLineCountDefault;

		private const bool _ShowTimeStatusDefault  = false;
		private const bool _ShowCountStatusDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// state
		private Domain.RepositoryType _repositoryType = _RepositoryTypeDefault;
		private MonitorActivityState _activityState = _ActivityStateDefault;
		private MonitorActivityState _activityStateOld = _ActivityStateDefault;

		// image
		private Image _imageInactive = null;
		private Image _imageActive = null;
		private OpacityState _imageOpacityState = OpacityState.Inactive;
		private double _imageOpacity = _MinimumImageOpacity;

		// lines
		private int _maximalLineCount = _MaximalLineCountDefault;
		private Model.Settings.FormatSettings _formatSettings = new Model.Settings.FormatSettings();
		private List<List<Domain.DisplayElement>> _lines = new List<List<Domain.DisplayElement>>();

		// time status
		private bool _showTimeStatus = _ShowTimeStatusDefault;
		private TimeSpan _connectTime;

		// count status
		private bool _showCountStatus = _ShowCountStatusDefault;
		private int _txByteCountStatus;
		private int _rxByteCountStatus;
		private int _txLineCountStatus;
		private int _rxLineCountStatus;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Action")]
		[Description("Event raised when copying is requested.")]
		public event EventHandler CopyRequest;

		[Category("Action")]
		[Description("Event raised when printing is requested.")]
		public event EventHandler PrintRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public Monitor()
		{
			InitializeComponent();
			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[Category("Monitor")]
		[Description("The repository type.")]
		[DefaultValue(_RepositoryTypeDefault)]
		public Domain.RepositoryType RepositoryType
		{
			get { return (_repositoryType); }
			set
			{
				if (_repositoryType != value)
				{
					_repositoryType = value;
					SetControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The activity state.")]
		[DefaultValue(_ActivityStateDefault)]
		public MonitorActivityState ActivityState
		{
			get { return (_activityState); }
			set
			{
				if (_activityState != value)
				{
					_activityState = value;
					SetControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The maxmimal number of lines to display.")]
		[DefaultValue(_MaximalLineCountDefault)]
		public int MaximalLineCount
		{
			set
			{
				if (_maximalLineCount != value)
				{
					_maximalLineCount = value;
					Reload();
				}
			}
		}

		[Browsable(false)]
		public Model.Settings.FormatSettings FormatSettings
		{
			set
			{
				if (_formatSettings != value)
				{
					_formatSettings = value;
					SetFormatDependentControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("Show the time status.")]
		[DefaultValue(_ShowTimeStatusDefault)]
		public bool ShowTimeStatus
		{
			get { return (_showTimeStatus); }
			set
			{
				if (_showTimeStatus != value)
				{
					_showTimeStatus = value;
					SetTimeStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The connect time status.")]
		[DefaultValue(0)]
		public TimeSpan ConnectTime
		{
			get { return (_connectTime); }
			set
			{
				if (_connectTime != value)
				{
					_connectTime = value;
					SetTimeStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("Show the count status.")]
		[DefaultValue(_ShowCountStatusDefault)]
		public bool ShowCountStatus
		{
			get { return (_showCountStatus); }
			set
			{
				if (_showCountStatus != value)
				{
					_showCountStatus = value;
					SetCountStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The Tx byte count status.")]
		[DefaultValue(0)]
		public int TxByteCountStatus
		{
			get { return (_txByteCountStatus); }
			set
			{
				if (_txByteCountStatus != value)
				{
					_txByteCountStatus = value;
					SetCountStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The Tx line count status.")]
		[DefaultValue(0)]
		public int TxLineCountStatus
		{
			get { return (_txLineCountStatus); }
			set
			{
				if (_txLineCountStatus != value)
				{
					_txLineCountStatus = value;
					SetCountStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The Rx byte count status.")]
		[DefaultValue(0)]
		public int RxByteCountStatus
		{
			get { return (_rxByteCountStatus); }
			set
			{
				if (_rxByteCountStatus != value)
				{
					_rxByteCountStatus = value;
					SetCountStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The Rx line count status.")]
		[DefaultValue(0)]
		public int RxLineCountStatus
		{
			get { return (_rxLineCountStatus); }
			set
			{
				if (_rxLineCountStatus != value)
				{
					_rxLineCountStatus = value;
					SetCountStatusControls();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		public void AddElement(Domain.DisplayElement element)
		{
			List<Domain.DisplayElement> elements = new List<Domain.DisplayElement>();
			elements.Add(element);
			AddElements(elements);
		}

		public void AddElements(List<Domain.DisplayElement> elements)
		{
			ListBox lb = listBox_Monitor;
			lb.BeginUpdate();

			foreach (Domain.DisplayElement de in elements)
			{
				// if first line, add a new empty line
				if (_lines.Count == 0)
				{
					List<Domain.DisplayElement> line = new List<Domain.DisplayElement>();
					_lines.Add(line);
					lb.Items.Add(line);
				}

				// add element to the current line
				List<Domain.DisplayElement> partialLine = _lines[_lines.Count - 1];
				partialLine.Add(de);
				_lines[_lines.Count - 1] = partialLine;
				lb.Items[lb.Items.Count - 1] = partialLine;

				// process EOL
				if (de.IsEol)
				{
					// remove lines if maximum exceeded
					while (lb.Items.Count >= (_maximalLineCount))
					{
						_lines.RemoveAt(0);
						lb.Items.RemoveAt(0);
					}

					// add new empty line
					List<Domain.DisplayElement> line = new List<Domain.DisplayElement>();
					_lines.Add(line);
					lb.Items.Add(line);
				}
			}

			// scroll list
			if ((lb.SelectedItems.Count == 0) && (lb.Items.Count > 0))
				lb.TopIndex = lb.Items.Count - 1;

			lb.EndUpdate();
		}

		public void AddLine(List<Domain.DisplayElement> line)
		{
			AddElements(line);
		}

		public void AddLines(List<List<Domain.DisplayElement>> lines)
		{
			foreach (List<Domain.DisplayElement> line in lines)
				AddLine(line);
		}

		public void ReplaceLine(int offset, List<Domain.DisplayElement> line)
		{
			int lastIndex = _lines.Count - 1;
			int indexToReplace = lastIndex - offset;

			if (indexToReplace >= 0)
			{
				_lines[indexToReplace] = line;

				ListBox lb = listBox_Monitor;
				lb.Items[indexToReplace] = line;
			}
			else
			{
				throw (new InvalidOperationException("Invalid attempt to replace a line of the monitor"));
			}
		}

		public void Clear()
		{
			_lines.Clear();
			ClearListBox();
		}

		public void Reload()
		{
			Clear();
			AddLines(_lines);
		}

		public void Reload(List<Domain.DisplayElement> elements)
		{
			Clear();
			AddElements(elements);
		}

		private void Reload(List<List<Domain.DisplayElement>> lines)
		{
			Clear();
			AddLines(lines);
		}

		public void ResetTimeStatus()
		{
			_connectTime = TimeSpan.Zero;

			SetTimeStatusControls();
		}

		public void ResetCountStatus()
		{
			_txByteCountStatus = 0;
			_txLineCountStatus = 0;
			_rxByteCountStatus = 0;
			_rxLineCountStatus = 0;

			SetCountStatusControls();
		}

		public List<List<Domain.DisplayElement>> SelectedLines
		{
			get
			{
				ListBox lb = listBox_Monitor;

				List<List<Domain.DisplayElement>> selectedLines = new List<List<YAT.Domain.DisplayElement>>();
				if (lb.SelectedItems.Count > 0)
				{
					foreach (int i in lb.SelectedIndices)
						selectedLines.Add(_lines[i]);
				}
				else
				{
					for (int i = 0; i < lb.Items.Count; i++)
						selectedLines.Add(_lines[i]);
				}
				return (selectedLines);
			}
		}

		#endregion

		#region Control Special Keys
		//==========================================================================================
		// Control Special Keys
		//==========================================================================================

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.C))
			{
				OnCopyRequest(new EventArgs());
				return (true);
			}
			else if (keyData == (Keys.Control | Keys.P))
			{
				OnPrintRequest(new EventArgs());
				return (true);
			}
			return (base.ProcessCmdKey(ref msg, keyData));
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		private void Monitor_Resize(object sender, EventArgs e)
		{
			int middle = Width / 2;
			label_TimeStatus.Width = middle - 14;
			label_CountStatus.Left = middle + 14;
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void timer_Opacity_Tick(object sender, EventArgs e)
		{
			if (_imageOpacityState != OpacityState.Inactive)
			{
				if (_imageOpacityState == OpacityState.Incrementing)
				{
					_imageOpacity += _ImageOpacityIncrement;
					if (_imageOpacity > _MaximumImageOpacity)
					{
						_imageOpacity = _MaximumImageOpacity;
						_imageOpacityState = OpacityState.Decrementing;
					}
				}
				else
				{
					_imageOpacity += _ImageOpacityDecrement;
					if (_imageOpacity < _MinimumImageOpacity)
					{
						_imageOpacity = _MinimumImageOpacity;
						_imageOpacityState = OpacityState.Incrementing;
					}
				}

				#if false
				// \fixme Don't know how to alter image opacity yet
				pictureBox_Monitor.Image.Opacity = _imageOpacity
				#endif

				if (_imageOpacity >= ((_MaximumImageOpacity - _MinimumImageOpacity) / 2))
					pictureBox_Monitor.Image = _imageActive;
				else
					pictureBox_Monitor.Image = null;
			}
		}

		// measures item height only, not needed for OwnerDrawnFixed
		#if (false)

		private void listBox_Monitor_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			if (e.Index >= 0)
			{
				if (e.Index >= 0)
				{
					ListBox lb = listBox_Monitor;

					SizeF size = Draw.MeasureItem((List<Domain.DisplayElement>)(lb.Items[e.Index]), _formatSettings, e.Graphics, e.Bounds);

					int width  = (int)Math.Ceiling(size.Width);
					int height = (int)Math.Ceiling(size.Height);

					e.ItemWidth  = width;
					e.ItemHeight = height;

					if (width > lb.HorizontalExtent)
						lb.HorizontalExtent = width;

					if (height != lb.ItemHeight)
						lb.ItemHeight = height;
				}
			}
		}

		#endif

		/// <remarks>
		/// Whether we like it or not, <see cref="System.Windows.Forms.ListBox.OnDrawItem()"/> calls
		/// this method pretty often. Actually it's called twice each time a new line is added. In
		/// addition, another call is needed for the next still empty line. Thus:
		/// 1st line received => 3 calls to DrawItem() at index 0 | 0 | 1
		/// 2nd line received => 5                     at index 0 | 1 | 0 | 1 | 2
		/// 3rd line received => 7                     at index 0 | 1 | 2 | 0 | 1 | 2 | 3
		/// ...
		/// Nth line received => 2*N + 1               at index 0 | 1 | 2...N | 0 | 1 | 2...N | N+1
		/// 
		/// Each call takes a 0..2ms. For 25 lines this results in something like:
		/// 51 x 2ms = 100ms per update!
		/// At least scrolling is handled properly, i.e. as soon as the listbox starts to scroll,
		/// the number of calls doesn't increase anymore.
		/// </remarks>
		private void listBox_Monitor_DrawItem(object sender, DrawItemEventArgs e)
		{
			unchecked
			{
				if (e.Index >= 0)
				{
					ListBox lb = listBox_Monitor;

					e.DrawBackground();
					SizeF size = Drawing.DrawItem((List<Domain.DisplayElement>)(lb.Items[e.Index]), _formatSettings, e.Graphics, e.Bounds, e.State);
					e.DrawFocusRectangle();

					int width  = (int)Math.Ceiling(size.Width);
					int height = (int)Math.Ceiling(size.Height);

					if ((width > 0) && (width > lb.HorizontalExtent))
						lb.HorizontalExtent = width;

					if ((height > 0) && (height != lb.ItemHeight))
						lb.ItemHeight = height;
				}
			}
		}

		private void listBox_Monitor_Leave(object sender, EventArgs e)
		{
			ListBox lb = listBox_Monitor;
			lb.ClearSelected();
		}

		private void timer_Redraw_Tick(object sender, EventArgs e)
		{
			// do be done
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			if (_repositoryType != Domain.RepositoryType.None)
			{
				switch (_repositoryType)
				{
					case Domain.RepositoryType.Tx:    _imageInactive = Properties.Resources.Image_Monitor_Tx_28x28;    _imageActive = Properties.Resources.Image_Monitor_Tx_28x28_Green;    break;
					case Domain.RepositoryType.Bidir: _imageInactive = Properties.Resources.Image_Monitor_Bidir_28x28; _imageActive = Properties.Resources.Image_Monitor_Bidir_28x28_Green; break;
					case Domain.RepositoryType.Rx:    _imageInactive = Properties.Resources.Image_Monitor_Rx_28x28;    _imageActive = Properties.Resources.Image_Monitor_Rx_28x28_Green;    break;
				}
				pictureBox_Monitor.BackgroundImage = _imageInactive;

				// image blending
				switch (_activityState)
				{
					case MonitorActivityState.Active:   _imageOpacityState = OpacityState.Inactive; pictureBox_Monitor.Image = _imageActive; break;
					case MonitorActivityState.Inactive: _imageOpacityState = OpacityState.Inactive; pictureBox_Monitor.Image = null;         break;
					case MonitorActivityState.Pending:
					{
						if (_imageOpacityState == OpacityState.Inactive)
						{
							if (_activityStateOld == MonitorActivityState.Active)
							{
								pictureBox_Monitor.Image = _imageActive;
								_imageOpacity = _MaximumImageOpacity;
								_imageOpacityState = OpacityState.Decrementing;
							}
							if (_activityStateOld == MonitorActivityState.Inactive)
							{
								pictureBox_Monitor.Image = _imageActive;
								_imageOpacity = _MinimumImageOpacity;
								_imageOpacityState = OpacityState.Incrementing;
							}
						}
						break;
					}
				}
				_activityStateOld = _activityState;

				timer_Opacity.Enabled = (_imageOpacityState != OpacityState.Inactive);
				panel_Picture.Visible = true;

				listBox_Monitor.BringToFront();
				listBox_Monitor.Top = panel_Picture.Height;
			}
			else
			{
				panel_Picture.Visible = false;
				listBox_Monitor.SendToBack();
			}

			SetFormatDependentControls();
			SetTimeStatusControls();
			SetCountStatusControls();
		}

		private void SetFormatDependentControls()
		{
			listBox_Monitor.ItemHeight = _formatSettings.Font.Height;
			listBox_Monitor.Invalidate();
		}

		private void SetCharReplaceDependentControls()
		{
			listBox_Monitor.Invalidate();
		}

		private void SetTimeStatusControls()
		{
			StringBuilder sb = new StringBuilder();
			TimeSpan ts = _connectTime;

			sb.Insert(0, ts.Seconds.ToString("D2"));
			sb.Insert(0, ":");
			sb.Insert(0, ts.Minutes.ToString());
			if (ts.Hours > 0)
			{
				sb.Insert(0, ":");
				sb.Insert(0, ts.Hours.ToString());

				if (ts.Days > 0)
				{
					sb.Insert(0, "days ");
					sb.Insert(0, ts.Days.ToString());
				}
			}
			label_TimeStatus.Text = sb.ToString();
			label_TimeStatus.Visible = _showTimeStatus;
		}

		private void SetCountStatusControls()
		{
			StringBuilder sb = new StringBuilder();
			switch (_repositoryType)
			{
				case Domain.RepositoryType.Tx:
				{
					sb.Append(_txByteCountStatus.ToString());
					sb.Append(" / ");
					sb.Append(_txLineCountStatus.ToString());
					break;
				}
				case Domain.RepositoryType.Bidir:
				{
					sb.Append(_txByteCountStatus.ToString());
					sb.Append(" / ");
					sb.Append(_txLineCountStatus.ToString());
					sb.Append(Environment.NewLine);
					sb.Append(_rxByteCountStatus.ToString());
					sb.Append(" / ");
					sb.Append(_rxLineCountStatus.ToString());
					break;
				}
				case Domain.RepositoryType.Rx:
				{
					sb.Append(_rxByteCountStatus.ToString());
					sb.Append(" / ");
					sb.Append(_rxLineCountStatus.ToString());
					break;
				}
			}
			label_CountStatus.Text = sb.ToString();
			label_CountStatus.Visible = _showCountStatus;
		}

		private void ClearListBox()
		{
			ListBox lb = listBox_Monitor;

			lb.BeginUpdate();
			lb.Items.Clear();
			lb.HorizontalExtent = 0;
			lb.EndUpdate();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnCopyRequest(EventArgs e)
		{
			EventHelper.FireSync(CopyRequest, this, e);
		}

		protected virtual void OnPrintRequest(EventArgs e)
		{
			EventHelper.FireSync(PrintRequest, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
