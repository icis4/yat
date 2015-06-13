//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

// \remind MKY 2013-05-25 (related to feature request #163)
// Ideally, the two properties 'HorizontalAutoScroll' and 'VerticalAutoScroll' and the corresponding
// automatism would be supported by this ListBox extension. However, no feasible implementation has
// been found. Therefore, it was decided to skip the automatism and simply provide the necessary
// methods that allow the user of this control to trigger the auto scroll.

// Enable to continue working/testing with an automatic horizontally scrolling list box:
//#define ENABLE_HORIZONTAL_AUTO_SCROLL

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using MKY.Win32;

#endregion

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a list box that extends <see cref="ListBox"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	[DesignerCategory("Windows Forms")]
	public class ListBoxEx : ListBox
	{
#if (ENABLE_HORIZONTAL_AUTO_SCROLL)
	
		#region General
		//==========================================================================================
		// General
		//==========================================================================================

		private bool IsLeftToRight
		{
			get { return (RightToLeft == RightToLeft.No); }
		}

		#endregion

#endif

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

#if (ENABLE_HORIZONTAL_AUTO_SCROLL)

		#region Scroll > Events
		//------------------------------------------------------------------------------------------
		// Scroll > Events
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Occurs when horizontally scrolled.
		/// </summary>
		[Category("Action")]
		public event ScrollEventHandler HorizontalScrolled;

		/// <summary>
		/// Occurs when vertically scrolled.
		/// </summary>
		[Category("Action")]
		public event ScrollEventHandler VerticalScrolled;

		#endregion

#endif

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
					DialogResult dr = DialogResult.OK;
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
					VerticalScroll();
				}
				else
				{
					this.verticalAutoScroll = false;
				}
			}
		}
#endif
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
#endif
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

#if (ENABLE_HORIZONTAL_AUTO_SCROLL)

		#region Scroll > Overridden Methods
		//------------------------------------------------------------------------------------------
		// Scroll > Overridden Methods
		//------------------------------------------------------------------------------------------

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

						// Fire event:
						if (m.Msg == Window.NativeConstants.WM_HSCROLL)
							OnHorizontalScrolledAsync(new ScrollEventArgs(ScrollEventType.EndScroll, si.nPos, ScrollOrientation.HorizontalScroll));
						else
							OnVerticalScrolledAsync(new ScrollEventArgs(ScrollEventType.EndScroll, si.nPos, ScrollOrientation.VerticalScroll));
					}
				}
			}

			base.WndProc(ref m);
		}

		#endregion

		#region Scroll > Private Methods
		//------------------------------------------------------------------------------------------
		// Scroll > Private Methods
		//------------------------------------------------------------------------------------------

		private void GetScrollInfo(int sb, ref Window.NativeTypes.SCROLLINFO si)
		{
			si.cbSize = Convert.ToUInt32(Marshal.SizeOf(si));
			si.fMask = Window.NativeConstants.SIF_ALL;

			if (!Window.NativeMethods.GetScrollInfo(Handle, sb, ref si))
			{
				StringBuilder message = new StringBuilder();
				message.AppendLine("Unable to retrieve scroll info from system:");
				message.AppendLine();
				message.AppendLine(WinError.GetLastError());
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
				StringBuilder message = new StringBuilder();
				message.AppendLine("Unable to retrieve scroll info from system:");
				message.AppendLine();
				message.AppendLine(WinError.GetLastError());
				throw (new InvalidOperationException(message.ToString()));
			}
		}

		#endregion

		#region Scroll > Event Invoking
		//------------------------------------------------------------------------------------------
		// Scroll > Event Invoking
		//------------------------------------------------------------------------------------------

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

		#endregion

#endif

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
