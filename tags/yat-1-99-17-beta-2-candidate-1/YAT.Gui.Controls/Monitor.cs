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
	/// <summary>
	/// This monitor implements a list box based terminal monitor in a speed optimized way
	/// </summary>
	[DesignerCategory("Windows Forms")]
	public partial class Monitor : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.RepositoryType _RepositoryTypeDefault = Domain.RepositoryType.None;
		private const int _MaximalLineCountDefault = 100;

		private const bool _ShowCountStatusDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Domain.RepositoryType _repositoryType = _RepositoryTypeDefault;
		private int _maximalLineCount = _MaximalLineCountDefault;
		private Model.Settings.FormatSettings _formatSettings = new Model.Settings.FormatSettings();
		private List<List<Domain.DisplayElement>> _lines = new List<List<Domain.DisplayElement>>();

		// count status
		private bool _showCountStatus = _ShowCountStatusDefault;
		private int _txByteCountStatus = 0;
		private int _rxByteCountStatus = 0;
		private int _txLineCountStatus = 0;
		private int _rxLineCountStatus = 0;

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
				if (de.IsEol)
				{
					// remove lines if maximum exceeded
					while (lb.Items.Count >= (_maximalLineCount))
					{
						_lines.RemoveAt(0);
						lb.Items.RemoveAt(0);
					}
					List<Domain.DisplayElement> line = new List<Domain.DisplayElement>();
					_lines.Add(line);
					lb.Items.Add(line);
				}
				else
				{
					if (_lines.Count == 0)       // if first line, add a new line
					{
						List<Domain.DisplayElement> line = new List<Domain.DisplayElement>();
						line.Add(de);
						_lines.Add(line);
						lb.Items.Add(line);
					}
					else                         // else add elements to the current line
					{
						List<Domain.DisplayElement> partialLine = _lines[_lines.Count - 1];
						partialLine.Add(de);
						_lines[_lines.Count - 1] = partialLine;
						lb.Items[lb.Items.Count - 1] = partialLine;
					}
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

		public void ReplaceLastLine(List<Domain.DisplayElement> line)
		{
			ListBox lb = listBox_Monitor;
			lb.BeginUpdate();

			if (_lines.Count == 0)               // if first line, simply add it
			{
				AddLine(line);
			}                                    // if last line is empty, replace line before
			else if (_lines[_lines.Count - 1].Count == 0)
			{
				// check that a "line before" actually exists
				if (_lines.Count >= 2)
					_lines[_lines.Count - 2] = line;

				if (lb.Items.Count >= 2)
					lb.Items[lb.Items.Count - 2] = line;
			}
			else                                 // if last line isn't finish yet, add to it
			{
				List<Domain.DisplayElement> partialLine = _lines[_lines.Count - 1];
				partialLine.AddRange(line);
				_lines[_lines.Count - 1] = partialLine;
				lb.Items[lb.Items.Count - 1] = partialLine;
			}

			lb.EndUpdate();
		}

		public void Clear()
		{
			_lines.Clear();
			ClearListBox();
		}

		public void Reload()
		{
			ClearListBox();
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
			label_CountStatus.Left = (Width / 2) + 14;
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		#if (false)

		// measures item height only, not needed for OwnerDrawnFixed
		private void listBox_Monitor_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			ListBox lb = listBox_Monitor;

			SizeF size = Draw.MeasureItem((List<Domain.DisplayElement>)(lb.Items[e.Index]), _formatSettings, e.Graphics);

			int width  = (int)Math.Ceiling(size.Width);
			int height = (int)Math.Ceiling(size.Height);

			e.ItemWidth  = width;
			e.ItemHeight = height;

			if (width > lb.HorizontalExtent)
				lb.HorizontalExtent = width;

			if (height != lb.ItemHeight)
				lb.ItemHeight = height;
		}

		#endif

		private void listBox_Monitor_DrawItem(object sender, DrawItemEventArgs e)
		{
			ListBox lb = listBox_Monitor;

			if ((e.Index >= 0) && (lb.DisplayRectangle.IntersectsWith(lb.GetItemRectangle(e.Index))))
			{
				e.DrawBackground();
				SizeF size = Draw.DrawItem((List<Domain.DisplayElement>)(lb.Items[e.Index]), _formatSettings, e.Graphics, e.Bounds, e.State);
				e.DrawFocusRectangle();

				int width  = (int)Math.Ceiling(size.Width);
				int height = (int)Math.Ceiling(size.Height);

				if (width > lb.HorizontalExtent)
					lb.HorizontalExtent = width;

				if (height != lb.ItemHeight)
					lb.ItemHeight = height;
			}
		}

		private void listBox_Monitor_Leave(object sender, EventArgs e)
		{
			ListBox lb = listBox_Monitor;

			lb.ClearSelected();
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
				Image image = null;
				switch (_repositoryType)
				{
					case Domain.RepositoryType.Tx:    image = Properties.Resources.Image_Monitor_Tx_28x28; break;
					case Domain.RepositoryType.Bidir: image = Properties.Resources.Image_Monitor_Bidir_28x28; break;
					case Domain.RepositoryType.Rx:    image = Properties.Resources.Image_Monitor_Rx_28x28; break;
				}
				pictureBox_Monitor.Image = image;

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
			SetCountStatusControls();
		}


		private void SetFormatDependentControls()
		{
			listBox_Monitor.ItemHeight = _formatSettings.Font.Height;
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
