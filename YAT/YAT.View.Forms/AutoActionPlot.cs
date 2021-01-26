//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using MKY;
using MKY.IO;
using MKY.Windows.Forms;

using YAT.Application.Utilities;
using YAT.Model;
using YAT.Model.Types;
using YAT.Model.Utilities;
using YAT.Settings.Application;

#endregion

namespace YAT.View.Forms
{
	/// <remarks>
	/// Separate <see cref="Form"/> rather than integrated into <see cref="Terminal"/> for...
	/// <list type="bullet">
	/// <item><description>...allowing a user to freely size and position the plot.</description></item>
	/// <item><description>...better decoupling monitor and plot update performance.</description></item>
	/// <item><description>...not adding even more to <see cref="Terminal"/>.</description></item>
	/// </list>
	/// Note that <see cref="Terminal"/> invokes this form using <see cref="Form.Show(IWin32Window)"/>,
	/// specifying the terminal as the owner. Advantages:
	/// <list type="bullet">
	/// <item><description>Behavior as described above.</description></item>
	/// <item><description>Keep plot on top of <see cref="Terminal"/> as well as <see cref="Main"/>.</description></item>
	/// <item><description>Thus possible to position the plot inside the <see cref="Main"/> window.</description></item>
	/// </list>
	/// Disadvantages:
	/// <list type="bullet">
	/// <item><description>Not possible to minimize YAT while still showing the plot.</description></item>
	/// </list>
	/// Potential refinement of this behavior is tracked in https://sourceforge.net/p/y-a-terminal/feature-requests/391/.
	/// </remarks>
	public partial class AutoActionPlot : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Color plotAreaBackColor = SystemColors.Control;

		private bool isInitiating = true;
		private bool isClosing = false;

		private SettingControlsHelper isSettingControls;

		private Model.Terminal terminal; // = null;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the PlotAreaBackColor property is changed.")]
		public event EventHandler PlotAreaBackColorChanged;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when the AutoAction is requested to be suspended.")]
		public event EventHandler SuspendAutoAction;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when the AutoAction is requested to be resumed.")]
		public event EventHandler ResumeAutoAction;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when the AutoAction is requested to be deactivated.")]
		public event EventHandler DeactivateAutoAction;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public AutoActionPlot(Model.Terminal terminal)
		{
			this.terminal = terminal;

			InitializeComponent();

			plotView.Model = this.terminal.AutoActionPlotModel.OxyModel;
		////plotView.Model.DefaultFont     = Font.Name;
		////plotView.Model.DefaultFontSize = Font.SizeInPoints;
		////plotView.Model.DefaultFont     = SystemFonts.DefaultFont.Name;
		////plotView.Model.DefaultFontSize = SystemFonts.DefaultFont.SizeInPoints;
		////plotView.Model.DefaultFontSize = SystemFonts.DefaultFont.Height; // Attention, see bug #485 "link label text rendering issues after plotting"!

			var controller = new OxyPlot.PlotController();
			controller.Unbind(OxyPlot.PlotCommands.CopyCode);       // [Ctrl+Alt+C] by default; no use case; [Alt] may cause issues.
			controller.Unbind(OxyPlot.PlotCommands.CopyTextReport); // [Ctrl+Alt+R] by default; no use case; [Alt] may cause issues.
			controller.Unbind(OxyPlot.PlotCommands.SnapTrack);      // [MouseLeft] shall be usable for [PanAt].
			controller.Bind(new OxyPlot.OxyMouseEnterGesture(), OxyPlot.PlotCommands.HoverSnapTrack);
			controller.Unbind(OxyPlot.PlotCommands.PanAt);          // [PanAt] mapped to [MouseLeft].
			controller.Bind(new OxyPlot.OxyMouseDownGesture(OxyPlot.OxyMouseButton.Left), OxyPlot.PlotCommands.PanAt);
			plotView.Controller = controller;                       // [MouseRight] free'd for opening context menu.

			PlotAreaBackColor = this.terminal.SettingsRoot.Format.BackColor;

			ApplyShowLegend();
			ApplyWindowSettingsAccordingToStartupState();
			ApplySuspendResume();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Gets or sets the back color of the plot area.
		/// </summary>
		[Category("Appearance")]
		[Description("The back color of the plot area.")]
		[DefaultValue(typeof(SystemColors), "Control")]
		public Color PlotAreaBackColor
		{
			get { return (this.plotAreaBackColor); }
			set
			{
				if (this.plotAreaBackColor != value)
				{
					this.plotAreaBackColor = value;
					OnPlotAreaBackColorChanged(EventArgs.Empty);
				}
			}
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// The 'Shown' event is only raised the first time a form is displayed; subsequently
		/// minimizing, maximizing, restoring, hiding, showing, or invalidating and repainting will
		/// not raise this event again.
		/// Note that the 'Shown' event is raised after the 'Load' event and will also be raised if
		/// the application is started minimized. Also note that operations called in the 'Shown'
		/// event can depend on a properly drawn form, as the 'Paint' event of this form and its
		/// child controls has been raised before this 'Shown' event.
		/// </remarks>
		private void AutoActionPlot_Shown(object sender, EventArgs e)
		{
			this.isInitiating = false;
		}

		private void AutoActionPlot_PlotAreaBackColorChanged(object sender, EventArgs e)
		{
			ApplyPlotAreaBackColor();
		}

		private void AutoActionPlot_LocationChanged(object sender, EventArgs e)
		{
			if (!this.isInitiating && !this.isClosing)
				UpdateWindowSettings(true);
		}

		private void AutoActionPlot_SizeChanged(object sender, EventArgs e)
		{
			if (!this.isInitiating && !this.isClosing)
				UpdateWindowSettings(false);
		}

		private void AutoActionPlot_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Skip if WinForms has already determined to cancel:
			if (e.Cancel)
				return;

			this.isClosing = true;

			// Save window settings (which has not yet been done at UpdateWindowSettings()):
			SaveWindowSettings();
		}

		private void AutoActionPlot_FormClosed(object sender, FormClosedEventArgs e)
		{
			plotView.Model = null; // Detach, required to potentially attach to model later again.
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void plotView_Paint(object sender, PaintEventArgs e)
		{
			// Manage histogram bin labels:

			lock (this.terminal.AutoActionPlotModelSyncObj) // Guarantee consistency with ongoing
			{                                               // automatic action requests.
				var yatModel = this.terminal.AutoActionPlotModel;
				switch (yatModel.Action)
				{
					case AutoAction.HistogramHorizontal:
					{
						if (yatModel.Histogram != null) // 'null' when no data yet or just cleared.
						{
							var oxyModel = plotView.Model;

							var widthPerBin = (0.8 * plotView.Width / yatModel.Histogram.BinCount);
							var binsPerStep = (widthPerBin / 48.0); // Arbitrary number ~ "8.88E88", chosen experimentally by resizing plot.

							var axis = (OxyPlot.Axes.CategoryAxis)(oxyModel.Axes[0]);
							axis.MajorStep = ToMajorStep(binsPerStep);

							var showCountLabels = (widthPerBin >= 32.0); // Arbitrary number ~ "8888".
							var series = (OxyPlot.Series.ColumnSeries)(oxyModel.Series[0]);
							series.LabelFormatString = (showCountLabels ? "{0}" : "");
						}
						break;
					}

					case AutoAction.HistogramVertical:
					{
						if (yatModel.Histogram != null) // 'null' when no data yet or just cleared.
						{
							var oxyModel = plotView.Model;

							var heightPerBin = (0.8 * plotView.Height / yatModel.Histogram.BinCount);
							var binsPerStep = (heightPerBin / plotView.Font.Height);

							var axis = (OxyPlot.Axes.CategoryAxis)(oxyModel.Axes[0]);
							axis.MajorStep = ToMajorStep(binsPerStep);

							var showCountLabels = (heightPerBin >= plotView.Font.Height);
							var series = (OxyPlot.Series.BarSeries)(oxyModel.Series[0]);
							series.LabelFormatString = (showCountLabels ? "{0}" : "");
						}
						break;
					}

					default:
					{
						break; // Nothing to do.
					}
				}
			}
		}

		/// <summary>
		/// Calculates the major step of the histogram bin axis.
		/// </summary>
		protected static double ToMajorStep(double binsPerStep)
		{
			if (binsPerStep >= 1.0)
			{
				return (1.0);
			}
			else
			{
				var log10 = (int)Math.Ceiling(Math.Log10(binsPerStep));
				var magnitude = Math.Pow(10, log10);
				var normalized = (binsPerStep / magnitude);

				if      (normalized >= 0.5)
					return (2.0 / magnitude);
				else if (normalized >= 0.2)
					return (5.0 / magnitude);
				else
					return (10.0 / magnitude);
			}
		}

		private void button_ResetAxes_Click(object sender, EventArgs e)
		{
			ResetAxes();
		}

		private void checkBox_ShowLegend_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			ToggleShowLegend();
		}

		private void button_Clear_Click(object sender, EventArgs e)
		{
			Clear();
		}

		private void button_SuspendResume_Click(object sender, EventArgs e)
		{
			OnSuspendResume();
		}

		private void button_Deactivate_Click(object sender, EventArgs e)
		{
			OnDeactivateAutoAction(EventArgs.Empty);
		}

		private void button_DeactivateAndClose_Click(object sender, EventArgs e)
		{
			OnDeactivateAutoAction(EventArgs.Empty);
			Close();
		}

		private void button_Close_Click(object sender, EventArgs e)
		{
			Close();
		}

		#region Controls Event Handlers > Context Menu
		//==========================================================================================
		// Controls Event Handlers > Context Menu
		//==========================================================================================

		private void contextMenuStrip_Plot_Opening(object sender, CancelEventArgs e)
		{
			var oxyModelIsDefined = (plotView.Model != null);

			toolStripMenuItem_Plot_CopyToClipboard.Enabled = oxyModelIsDefined;
			toolStripMenuItem_Plot_SaveToFile.Enabled      = oxyModelIsDefined;
			toolStripMenuItem_Plot_ResetAxes.Enabled       = oxyModelIsDefined;

			toolStripMenuItem_Plot_ShowLegend.Checked = ApplicationSettings.RoamingUserSettings.Plot.ShowLegend;
		}

		private void toolStripMenuItem_Plot_CopyToClipboard_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				Clipboard.Clear(); // Prevent handling errors in case copying takes long.

				var exporter = GetPngExporter();
				using (var image = exporter.ExportToBitmap(plotView.Model))
					Clipboard.SetImage(image);

				Cursor = Cursors.Default;
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				Cursor = Cursors.Default;
			}
		}

		private void toolStripMenuItem_Plot_SaveToFile_Click(object sender, EventArgs e)
		{
			SaveToFile();
		}

		private void toolStripMenuItem_Plot_ResetAxes_Click(object sender, EventArgs e)
		{
			ResetAxes();
		}

		private void toolStripMenuItem_Plot_ShowLegend_Click(object sender, EventArgs e)
		{
			ToggleShowLegend();
		}

		private void toolStripMenuItem_Plot_Help_Click(object sender, EventArgs e)
		{
			var f = new AutoActionPlotHelp();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void ApplyWindowSettingsAccordingToStartupState()
		{
			// Attention:
			// Almost the same code exists in Main.ApplyWindowSettingsAccordingToStartupState().
			// Changes here likely have to be applied there too.

			if (ApplicationSettings.LocalUserSettingsSuccessfullyLoadedFromFile)
			{
				// Do not Suspend/ResumeLayout() when changing the form itself!

				// Window state:
				WindowState = ApplicationSettings.LocalUserSettings.PlotWindow.State;

				// Start position:
				var savedStartPosition = ApplicationSettings.LocalUserSettings.PlotWindow.StartPosition;
				var savedLocation      = ApplicationSettings.LocalUserSettings.PlotWindow.Location; // Note the issue/limitation described
				var savedSize          = ApplicationSettings.LocalUserSettings.PlotWindow.Size;     // in SaveWindowSettings() below.

				var savedBounds = new Rectangle(savedLocation, savedSize);
				var isWithin = ScreenEx.IsWithinAnyWorkingArea(savedBounds); // 'WorkingArea' rather than 'Bounds' as e.g. Firefox, Thunderbird and LibraOffice.
				if (isWithin) // Restore saved settings if within working area:
				{
					StartPosition = savedStartPosition;
					Location      = savedLocation;
					Size          = savedSize;
				}
				else // Let the operating system adjust the position if out of bounds:
				{
					StartPosition = FormStartPosition.WindowsDefaultBounds;
				}

				// Note that check must be done regardless of the window state, since the state may
				// be changed by the user at any time after the initial layout.
			}
		}

		/// <summary>
		/// Updates the window settings without saving it to the local user settings (yet).
		/// </summary>
		/// <remarks>
		/// Advantage: Prevents many save operations on resizing the form.
		/// Disadvantage: State gets lost if application crashes.
		/// </remarks>
		private void UpdateWindowSettings(bool setStartPositionToManual)
		{
			// Attention:
			// Almost the same code exists in Main.SaveWindowSettings().
			// Changes here likely have to be applied there too.

			if (setStartPositionToManual)
			{
				ApplicationSettings.LocalUserSettings.PlotWindow.StartPosition = FormStartPosition.Manual;
				StartPosition = ApplicationSettings.LocalUserSettings.PlotWindow.StartPosition;
			}

			ApplicationSettings.LocalUserSettings.PlotWindow.State = WindowState;

			if (WindowState == FormWindowState.Normal)
			{
				if (StartPosition == FormStartPosition.Manual)
					ApplicationSettings.LocalUserSettings.PlotWindow.Location = Location;

				ApplicationSettings.LocalUserSettings.PlotWindow.Size = Size;

				// Note the following issue/limitation:
				// Windows or WinForm seems to consider the shadow around a form to belong to the form,
				// i.e. a form that is placed at a screen's edge, may tell values outside the screen.
				//
				// Example with two screens [2] [1] (where 1 is the main screen, and both screens are 1920 × 1080)
				// and the main form placed at the upper left corner, spreading across the whole screen. This may
				// result in the following [LocalUserSettings] values:
				//
				//    <Location>
				//      <X>-1924</X>
				//      <Y>2</Y>
				//    </Location>
				//    <Size>
				//      <Width>1926</Width>
				//      <Height>480</Height>
				//    </Size>
				//
				// Location.X and Size.Width are outside the screen's dimensions even though the form is inside!
				// As a consequence, MKY.Windows.Forms.ScreenEx.IsWithinAnyBounds() will wrongly determine that
				// the form doesn't fit a screen and ApplyWindowSettingsAccordingToStartup() will fall back to
				// 'FormStartPosition.WindowsDefaultBounds'.
				//
				// Issue/limitation is considered very acceptable, neither bug filed nor added to release notes.
			}

			// Don't save right now, see remarks of this method as well as 'SaveWindowSettings()' below.
		}

		private static void SaveWindowSettings()
		{
			if (ApplicationSettings.LocalUserSettings.PlotWindow.HaveChanged)
				ApplicationSettings.SaveLocalUserSettings();
		}

		private void ApplyPlotAreaBackColor()
		{
			var model = plotView.Model;
			if (model != null)
			{
				var oxyColor = OxyPlotEx.ConvertTo(PlotAreaBackColor);

			////model.Background         = oxyColor; // Same as plotView.BackColor ?!?
				model.PlotAreaBackground = oxyColor; // Back color is only appied to inner part same as it is only applied to inner part of monitors.
			////model.LegendBackground is not configurable (yet).

				model.InvalidatePlot(false);
			}
		}

		private OxyPlot.WindowsForms.PngExporter GetPngExporter()
		{
			var exporter = new OxyPlot.WindowsForms.PngExporter
			{
				Width  = plotView.Width,
				Height = plotView.Height,
				Background = OxyPlot.OxyColors.White
			};

			return (exporter);
		}

		private OxyPlot.WindowsForms.SvgExporter GetSvgExporter()
		{
			var exporter = new OxyPlot.WindowsForms.SvgExporter
			{
				Width  = plotView.Width,
				Height = plotView.Height,
			};

			return (exporter);
		}

		private OxyPlot.PdfExporter GetPdfExporter()
		{
			var exporter = new OxyPlot.PdfExporter
			{
				Width  = plotView.Width,
				Height = plotView.Height,
				Background = OxyPlot.OxyColors.White
			};

			return (exporter);
		}

		private void SaveToFile()
		{
			string initialExtension = ApplicationSettings.RoamingUserSettings.Extensions.PlotFiles;

			var sfd = new SaveFileDialog();
			sfd.Title       = "Save Plot As";
			sfd.Filter      = ExtensionHelper.PlotFilesFilter;
			sfd.FilterIndex = ExtensionHelper.PlotFilesFilterHelper(initialExtension);
			sfd.DefaultExt  = PathEx.DenormalizeExtension(initialExtension);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.PlotFiles;

			var dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(sfd.FileName)))
			{
				ApplicationSettings.RoamingUserSettings.Extensions.PlotFiles = Path.GetExtension(sfd.FileName);
				ApplicationSettings.LocalUserSettings.Paths.PlotFiles = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUserSettings();
				ApplicationSettings.SaveRoamingUserSettings();

				Refresh(); // Ensure that form has been refreshed before continuing.

				Exception ex;
				if (TrySaveToFile(sfd.FileName, out ex))
					return;

				string errorMessage;
				if (!string.IsNullOrEmpty(sfd.FileName))
					errorMessage = Model.Utilities.MessageHelper.ComposeMessage("Unable to save", sfd.FileName, ex);
				else
					errorMessage = Model.Utilities.MessageHelper.ComposeMessage("Unable to save file!", ex);

				MessageBoxEx.Show
				(
					this,
					errorMessage,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
		}

		private bool TrySaveToFile(string filePath, out Exception ex)
		{
			ImageFormat format;
			if      (ExtensionHelper.IsImageFile(filePath, out format))
				return (TrySaveToFile(filePath, GetPngExporter(), format, out ex));
			else if (ExtensionHelper.IsSvgFile(filePath))
				return (TrySaveToFile(filePath, GetSvgExporter(), out ex));
			else if (ExtensionHelper.IsPdfFile(filePath))
				return (TrySaveToFile(filePath, GetPdfExporter(), out ex));
			else
				return (TrySaveToFile(filePath, GetPngExporter(), ImageFormat.Png, out ex)); // Fallback to PNG.
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private bool TrySaveToFile(string filePath, OxyPlot.WindowsForms.PngExporter exporter, ImageFormat imageFormat, out Exception exceptionOnFailure)
		{
			try
			{
				using (var stream = File.OpenWrite(filePath))
				{
					using (var image = exporter.ExportToBitmap(plotView.Model))
						image.Save(stream, imageFormat);
				}

				exceptionOnFailure = null;
				return (true);
			}
			catch (Exception ex)
			{
				exceptionOnFailure = ex;
				return (false);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private bool TrySaveToFile(string filePath, OxyPlot.IExporter exporter, out Exception exceptionOnFailure)
		{
			try
			{
				using (var stream = File.OpenWrite(filePath))
				{
					exporter.Export(plotView.Model, stream);
				}

				exceptionOnFailure = null;
				return (true);
			}
			catch (Exception ex)
			{
				exceptionOnFailure = ex;
				return (false);
			}
		}

		private void ResetAxes()
		{
			var model = plotView.Model;
			if (model != null)
			{
				model.ResetAllAxes();
				model.InvalidatePlot(false);
			}
		}

		private void ToggleShowLegend()
		{
			ApplicationSettings.RoamingUserSettings.Plot.ShowLegend = !ApplicationSettings.RoamingUserSettings.Plot.ShowLegend;
			ApplicationSettings.SaveRoamingUserSettings();

			ApplyShowLegend();
		}

		private void ApplyShowLegend()
		{
			var showLegend = ApplicationSettings.RoamingUserSettings.Plot.ShowLegend;

			this.isSettingControls.Enter();
			try
			{
				checkBox_ShowLegend.Checked = showLegend;
			}
			finally
			{
				this.isSettingControls.Leave();
			}

			var model = plotView.Model;
			if (model != null)
			{
				model.IsLegendVisible = showLegend;

				model.LegendPlacement   = OxyPlot.LegendPlacement.Outside;
				model.LegendPosition    = OxyPlot.LegendPosition.BottomLeft;
				model.LegendOrientation = OxyPlot.LegendOrientation.Horizontal;

				model.InvalidatePlot(false);
			}
		}

		private void Clear()
		{
			lock (this.terminal.AutoActionPlotModelSyncObj) // Guarantee consistency with ongoing
				this.terminal.AutoActionPlotModel.Clear();  // automatic action requests.
		}

		private void OnSuspendResume()
		{
			if (this.terminal.AutoActionPlotModel.IsActive)
				OnSuspendAutoAction(EventArgs.Empty);
			else
				OnResumeAutoAction(EventArgs.Empty);

			ApplySuspendResume();
		}

		private void ApplySuspendResume()
		{
			this.isSettingControls.Enter();
			try
			{
				if (this.terminal.AutoActionPlotModel.IsActive)
					button_SuspendResume.Text = "S&uspend";
				else
					button_SuspendResume.Text = "Res&ume";
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnPlotAreaBackColorChanged(EventArgs e)
		{
			EventHelper.RaiseSync(PlotAreaBackColorChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSuspendAutoAction(EventArgs e)
		{
			EventHelper.RaiseSync(SuspendAutoAction, this, e);
		}

		/// <summary></summary>
		protected virtual void OnResumeAutoAction(EventArgs e)
		{
			EventHelper.RaiseSync(ResumeAutoAction, this, e);
		}

		/// <summary></summary>
		protected virtual void OnDeactivateAutoAction(EventArgs e)
		{
			EventHelper.RaiseSync(DeactivateAutoAction, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
