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
// MKY Version 1.0.20
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

// Enable to continue working/testing an automatically vertically scrolling list box:
//#define ENABLE_VERTICAL_AUTO_SCROLL

// \todo MKY 2016-09-19 (related to upgrade of ExSim)
// When enabling 'ENABLE_HORIZONTAL_AUTO_SCROLL', a reference to 'MKY.Win32' and thus Win32 API
// calls is required. This is undesirable, as there shall be no references to the Win32 API from
// within true .NET assemblies. As a consequence, when enabling 'ENABLE_HORIZONTAL_AUTO_SCROLL' for
// e.g. testing or analysis, the reference to 'MKY.Win32' must manually be added. And, when finally
// introducing 'ENABLE_HORIZONTAL_AUTO_SCROLL', a better solution must be found to deal with any
// dependencies to the Win32 API.

// Enable to continue working/testing an automatically horizontally scrolling list box:
//#define ENABLE_HORIZONTAL_AUTO_SCROLL

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

#if (ENABLE_HORIZONTAL_AUTO_SCROLL)
using System;
using System.Runtime.InteropServices;
using System.Text;

using MKY.Win32;
#endif

#endregion

namespace MKY.Windows.Forms
{
	/// <summary>
	/// An improved <see cref="ListBox"/> that additionally provides:
	/// <list type="bullet">
	/// <item><description>The <see cref="SelectAllIndices"/> method.</description></item>
	/// <item><description>Several "VerticalScroll...()" methods.</description></item>
	/// </list>
	/// </summary>
	/// <remarks>
	/// Ideally, the two properties 'HorizontalAutoScroll' and 'VerticalAutoScroll' and the
	/// corresponding automatism would be supported by this ListBox extension. However, no feasible
	/// implementation has been found. Thus, it was decided to skip the automatism and simply
	/// provide the necessary methods that allow the control's parent to trigger scrolling.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public class ListBoxEx : ListBox
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

	#if (ENABLE_HORIZONTAL_AUTO_SCROLL)
		private const bool HorizontalAutoScrollDefault = false;
	#endif

	#if (ENABLE_VERTICAL_AUTO_SCROLL)
		private const bool VerticalAutoScrollDefault = false;
	#endif

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

	#if (ENABLE_HORIZONTAL_AUTO_SCROLL)
		private bool horizontalAutoScroll = HorizontalAutoScrollDefault;
	#endif

	#if (ENABLE_VERTICAL_AUTO_SCROLL)
		private bool verticalAutoScroll = VerticalAutoScrollDefault;
	#endif

		#endregion

		#region General
		//==========================================================================================
		// General
		//==========================================================================================

	#if (ENABLE_HORIZONTAL_AUTO_SCROLL)

		private bool IsLeftToRight
		{
			get { return (RightToLeft == RightToLeft.No); }
		}

	#endif

		#endregion

		#region Selection
		//==========================================================================================
		// Selection
		//==========================================================================================

		/// <summary>
		/// Select all indices within the list box.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
		public void SelectAllIndices()
		{
			for (int i = 0; i < Items.Count; i++)
				SetSelected(i, true);
		}

		#endregion

		#region Scroll
		//==========================================================================================
		// Scroll
		//==========================================================================================

		#region Scroll > Events
		//------------------------------------------------------------------------------------------
		// Scroll > Events
		//------------------------------------------------------------------------------------------

	#if (ENABLE_HORIZONTAL_AUTO_SCROLL)

		/// <summary>
		/// Occurs when horizontally scrolled.
		/// </summary>
		[Category("Action")]
		public event ScrollEventHandler HorizontalScrolled;

	#endif

	#if (ENABLE_VERTICAL_AUTO_SCROLL)

		/// <summary>
		/// Occurs when vertically scrolled.
		/// </summary>
		[Category("Action")]
		public event ScrollEventHandler VerticalScrolled;

	#endif

		#endregion

		#region Scroll > Properties
		//------------------------------------------------------------------------------------------
		// Scroll > Properties
		//------------------------------------------------------------------------------------------

	#if (ENABLE_HORIZONTAL_AUTO_SCROLL)

		/// <summary></summary>
		[Category("Scroll")]
		[Description("Enables or disables horizontal auto scroll.")]
		[DefaultValue(HorizontalAutoScrollDefault)]
		public virtual bool HorizontalAutoScroll
		{
			get { return (this.horizontalAutoScroll); }
			set
			{
				if (!this.horizontalAutoScroll && value)
				{
					var dr = DialogResult.OK;
					if (DesignMode)
					{
						dr = MessageBoxEx.Show
						(
							"Note that horizontal auto scroll is limited to environments which implement the Win32 API.",
							"Warning",
							MessageBoxButtons.OKCancel,
							MessageBoxIcon.Warning
						);
					}
					if (dr == DialogResult.OK)
					{
						this.horizontalAutoScroll = true;
						HorizontalScroll();
					}
				}
				else
				{
					this.horizontalAutoScroll = false;
				}
			}
		}

	#endif // ENABLE_HORIZONTAL_AUTO_SCROLL

	#if (ENABLE_VERTICAL_AUTO_SCROLL)

		/// <summary></summary>
		[Category("Scroll")]
		[Description("Enables or disables vertical auto scroll.")]
		[DefaultValue(VerticalAutoScrollDefault)]
		public virtual bool VerticalAutoScroll
		{
			get { return (this.verticalAutoScroll); }
			set
			{
				if (!this.verticalAutoScroll && value)
				{
					this.verticalAutoScroll = true;

					if (!DesignMode)
						VerticalScrollToBottomIfNoItemsAreSelected();
				}
				else
				{
					this.verticalAutoScroll = false;
				}
			}
		}

	#endif // ENABLE_VERTICAL_AUTO_SCROLL

		#endregion

		#region Scroll > Methods
		//------------------------------------------------------------------------------------------
		// Scroll > Methods
		//------------------------------------------------------------------------------------------

	#if (ENABLE_HORIZONTAL_AUTO_SCROLL)

		/// <summary>
		/// Horizontally scroll the list to the beginning of the scroll extent, taking
		/// <see cref="RightToLeft"/> into account.
		/// </summary>
		public void HorizontalScrollToBegin()
		{
			// Only extend the Windows.Forms.ListBox if we are in a Microsoft Windows environment!
			// Otherwise, calling native Win32 methods will fail!
			if (EnvironmentEx.IsWindows)
			{
				Window.NativeTypes.SCROLLINFO si = new Window.NativeTypes.SCROLLINFO();

				// Get scroll info (errors will be handled there):
				GetScrollInfo(Window.NativeConstants.SB_HORZ, ref si);

				// Get delta from scroll info:
				int dx;
				if (IsLeftToRight)
					dx = (si.nPos - si.nMin);
				else
					dx = (si.nMax - si.nPos);

				// Set delta:
				ScrollWindowHorizontalDelta(dx);
			}
		}

		/// <summary>
		/// Horizontally scroll the list to the end of the scroll extent, taking
		/// <see cref="RightToLeft"/> into account.
		/// </summary>
		public void HorizontalScrollToEnd()
		{
			// Only extend the Windows.Forms.ListBox if we are in a Microsoft Windows environment!
			// Otherwise, calling native Win32 methods will fail!
			if (EnvironmentEx.IsWindows)
			{
				Window.NativeTypes.SCROLLINFO si = new Window.NativeTypes.SCROLLINFO();

				// Get scroll info (errors will be handled there):
				GetScrollInfo(Window.NativeConstants.SB_HORZ, ref si);

				// Get delta from scroll info:
				int dx;
				if (IsLeftToRight)
					dx = (si.nMax - si.nPos);
				else
					dx = (si.nPos - si.nMin);

				// Set delta:
				ScrollWindowHorizontalDelta(dx);
			}
		}

		/// <summary>
		/// Horizontally scroll the list to the given position, taking <see cref="RightToLeft"/>
		/// into account.
		/// </summary>
		public void HorizontalScrollToPosition(int position)
		{
			// Only extend the Windows.Forms.ListBox if we are in a Microsoft Windows environment!
			// Otherwise, calling native Win32 methods will fail!
			if (EnvironmentEx.IsWindows)
			{
				Window.NativeTypes.SCROLLINFO si = new Window.NativeTypes.SCROLLINFO();

				// Get scroll info (errors will be handled there):
				GetScrollInfo(Window.NativeConstants.SB_HORZ, ref si);

				// Get delta from scroll info:
				int dx;
				if (IsLeftToRight)
					dx = (position - si.nPos);
				else
					dx = (si.nPos - position);

				// Set delta:
				ScrollWindowHorizontalDelta(position);
			}
		}

	#endif // ENABLE_HORIZONTAL_AUTO_SCROLL

		/// <summary>
		/// Vertically scroll the list to the bottom.
		/// </summary>
		public void VerticalScrollToBottom()
		{
			TopIndex = (Items.Count - 1);
		}

		/// <summary>
		/// Vertically scroll the list to the bottom if no items are selected.
		/// </summary>
		public bool VerticalScrollToBottomIfNoItemsAreSelected()
		{
			if ((SelectedItems.Count == 0) && (Items.Count > 0))
			{
				VerticalScrollToBottom();
				return (true);
			}

			return (false);
		}

		/// <summary>
		/// Vertically scroll the list to the bottom if no items are selected, except the last.
		/// </summary>
		public bool VerticalScrollToBottomIfNoItemButTheLastIsSelected()
		{
			if (VerticalScrollToBottomIfNoItemsAreSelected())
				return (true);

			if (SelectedIndices.Count == 1)
			{
				if (SelectedIndices[0] == (Items.Count - 1))
				{
					SelectedIndices.Clear(); // Clear selection to ensure that scrolling continues.
					VerticalScrollToBottom();
					return (true);
				}

				if (SelectedIndices[0] == (Items.Count - 2))
				{
					SelectedIndices.Clear(); // Clear selection to ensure that scrolling continues.
					VerticalScrollToBottom();
					return (true);
				}
			}

			return (false);
		}

		/// <summary>
		/// Vertically scroll the list to the given index.
		/// </summary>
		public void VerticalScrollToIndex(int index)
		{
			TopIndex = index;
		}

		#endregion

		#region Scroll > Overridden Methods
		//------------------------------------------------------------------------------------------
		// Scroll > Overridden Methods
		//------------------------------------------------------------------------------------------

	#if (ENABLE_HORIZONTAL_AUTO_SCROLL)

		/// <summary>
		/// The list's window procedure.
		/// </summary>
		/// <param name="m">A Windows message object.</param>
		protected override void WndProc(ref Message m)
		{
			// Only extend the Windows.Forms.ListBox if we are in a Microsoft Windows environment!
			// Otherwise, calling native Win32 methods will fail!
			if (EnvironmentEx.IsWindows)
			{
				if ((m.Msg == Window.NativeConstants.WM_HSCROLL) || (m.Msg == Window.NativeConstants.WM_VSCROLL))
				{
					if (m.WParam.ToInt32() == Window.NativeConstants.SB_ENDSCROLL)
					{
						int sb;
						if (m.Msg == Window.NativeConstants.WM_HSCROLL)
							sb = Window.NativeConstants.SB_HORZ;
						else
							sb = Window.NativeConstants.SB_VERT;

						Window.NativeTypes.SCROLLINFO si = new Window.NativeTypes.SCROLLINFO();

						// Get scroll info (errors will be handled there):
						GetScrollInfo(sb, ref si);

						// Raise event:
						if (m.Msg == Window.NativeConstants.WM_HSCROLL)
							OnHorizontalScrolledAsync(new ScrollEventArgs(ScrollEventType.EndScroll, si.nPos, ScrollOrientation.HorizontalScroll));
						else
							OnVerticalScrolledAsync(new ScrollEventArgs(ScrollEventType.EndScroll, si.nPos, ScrollOrientation.VerticalScroll));
					}
				}
			}

			base.WndProc(ref m);
		}

	#endif // ENABLE_HORIZONTAL_AUTO_SCROLL

		#endregion

		#region Scroll > Private Methods
		//------------------------------------------------------------------------------------------
		// Scroll > Private Methods
		//------------------------------------------------------------------------------------------

	#if (ENABLE_HORIZONTAL_AUTO_SCROLL)

		private void GetScrollInfo(int sb, ref Window.NativeTypes.SCROLLINFO si)
		{
			si.cbSize = Convert.ToUInt32(Marshal.SizeOf(si));
			si.fMask = Window.NativeConstants.SIF_ALL;

			if (!Window.NativeMethods.GetScrollInfo(Handle, sb, ref si))
			{
				var message = new StringBuilder();
				message.AppendLine("Unable to retrieve scroll info from system:");
				message.AppendLine();
				message.AppendLine(WinError.GetLastError());

				+ Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug

				throw (new InvalidOperationException(message.ToString()));
			}
		}

		private void SetScrollInfo(int sb, ref Window.NativeTypes.SCROLLINFO si)
		{
			Window.NativeMethods.SetScrollInfo(Handle, sb, ref si, true);
		}

		private void ScrollWindowHorizontalDelta(int dx)
		{
			Window.NativeTypes.RECT prcScroll;
			Window.NativeTypes.RECT prcClip;
			Window.NativeTypes.RECT prcUpdate;

			prcScroll.left   = ClientRectangle.Left;
			prcScroll.top    = ClientRectangle.Top;
			prcScroll.right  = ClientRectangle.Right;
			prcScroll.bottom = ClientRectangle.Bottom;

			prcClip.left   = ClientRectangle.Left;
			prcClip.top    = ClientRectangle.Top;
			prcClip.right  = ClientRectangle.Right;
			prcClip.bottom = ClientRectangle.Bottom;

			int result = Window.NativeMethods.ScrollWindowEx(Handle, dx, 0, ref prcScroll, ref prcClip, IntPtr.Zero, out prcUpdate, Window.NativeConstants.SW_INVALIDATE);
			if (result != WinError.Success)
			{
				var message = new StringBuilder();
				message.AppendLine("Unable to retrieve scroll info from system:");
				message.AppendLine();
				message.AppendLine(WinError.GetLastError());

				+ Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug

				throw (new InvalidOperationException(message.ToString()));
			}
		}

	#endif // ENABLE_HORIZONTAL_AUTO_SCROLL

		#endregion

		#region Scroll > Event Invoking
		//------------------------------------------------------------------------------------------
		// Scroll > Event Invoking
		//------------------------------------------------------------------------------------------

	#if (ENABLE_HORIZONTAL_AUTO_SCROLL)

		/// <summary></summary>
		protected virtual void OnHorizontalScrolledAsync(ScrollEventArgs e)
		{
			EventHelper.FireAsync<ScrollEventArgs>(HorizontalScrolled, this, e);
		}

		/// <summary></summary>
		protected virtual void OnVerticalScrolledAsync(ScrollEventArgs e)
		{
			EventHelper.FireAsync<ScrollEventArgs>(VerticalScrolled, this, e);
		}

	#endif // ENABLE_HORIZONTAL_AUTO_SCROLL

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
