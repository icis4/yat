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

#if (DEBUG)

	// Enable debugging of vertical auto scrolling:
////#define DEBUG_VERTICAL_AUTO_SCROLL

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

#if (ENABLE_HORIZONTAL_AUTO_SCROLL)
using System;
using System.ComponentModel;
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

		private float clientItemCapacity; // = 0.0

		private int previousTopIndex; // = 0;
		private bool userIsScrolling; // = false;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="ListBoxEx"/> class.
		/// </summary>
		public ListBoxEx()
		{
			this.Resize += new EventHandler(this.ListBoxEx_Resize);

			EvaluateClientItemCapacity();
		}

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

		private void ListBoxEx_Resize(object sender, EventArgs e)
		{
			EvaluateClientItemCapacity();
		}

		/// <summary>
		/// Sets the number of items that the client area can show.
		/// </summary>
		protected virtual void EvaluateClientItemCapacity()
		{
			this.clientItemCapacity = (float)ClientSize.Height / (float)ItemHeight;

			DebugVerticalAutoScroll("ClientItemCapacity evaluated");
		}

		/// <summary>
		/// Gets the number of items that the client area can show.
		/// </summary>
		/// <remarks>
		/// If <see cref="ListBox.IntegralHeight"/> is set to <c>false</c>, the top or bottom most
		/// item may only be partially shown and the resulting value will fractional.
		/// </remarks>
		/// <remarks>
		/// If the <see cref="DrawMode"/> property is set to <see cref="DrawMode.OwnerDrawFixed"/>,
		/// all items have the same height. When the <see cref="DrawMode"/> property is set to
		/// <see cref="DrawMode.OwnerDrawVariable"/>, the <see cref="ListBox.ItemHeight"/> property
		/// specifies the height of each item added to the <see cref="ListBox"/>. Because each item
		/// in an owner-drawn list can have a different height, you can use the <see cref="ListBox.GetItemHeight"/>
		/// method to get the height of a specific item in the <see cref="ListBox"/>. If you use the
		/// <see cref="ListBox.ItemHeight"/> property on a <see cref="ListBox"/> with items of
		/// variable height, this property returns the height of the first item in the control.
		/// </remarks>
		public virtual float ClientItemCapacity
		{
			get { return (this.clientItemCapacity); }
		}

		#endregion

		#region Items
		//==========================================================================================
		// Items
		//==========================================================================================

		/// <summary></summary>
		public virtual int FullyVisibleItemCount
		{
			get
			{
				int result = Math.Min((int)Math.Floor(ClientItemCapacity), Items.Count);
				return (result);             // Floor() excludes a partially visible top or bottom most item.
			}
		}

		/// <summary></summary>
		public virtual int BottomIndex
		{
			get
			{
				int result = Math.Min(TopIndex + (int)Math.Ceiling(ClientItemCapacity), (Items.Count - 1));
				return (result);                        // Ceiling() includes a partially visible bottom most item.
			}
		}

		#endregion

		#region Selection
		//==========================================================================================
		// Selection
		//==========================================================================================

		/// <remarks>
		/// Note that items gets deselected if another control gets the focus.
		/// </remarks>
		public virtual bool ItemIsSelected
		{
			get { return (SelectedItems.Count > 0); }
		}

		/// <remarks>
		/// Note that items gets deselected if another control gets the focus.
		/// </remarks>
		public virtual bool VisibleItemIsSelected
		{
			get
			{
				foreach (int i in SelectedIndices)
				{
					if ((i >= TopIndex) && (i <= BottomIndex))
						return (true);
				}

				return (false);
			}
		}

		/// <remarks>
		/// Note that items gets deselected if another control gets the focus.
		/// </remarks>
		public virtual bool OnlyOneOfTheLastItemsIsSelected
		{
			get
			{
				if (SelectedIndices.Count == 1)
				{
					if (SelectedIndices[0] == (Items.Count - 1))
						return (true);

					if (SelectedIndices[0] == (Items.Count - 2))
						return (true);
				}

				return (false);
			}
		}

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

		/// <summary></summary>
		public virtual bool UserIsScrolling
		{
			get
			{
				if (this.previousTopIndex > TopIndex)
				{
					this.userIsScrolling = true;

					DebugVerticalAutoScroll("User has started scrolling");
				}

				this.previousTopIndex = TopIndex; // Update.

				return (this.userIsScrolling);
			}
		}

		/// <remarks>
		/// "NearBottom" means at bottom or at least half the visible items close to it.
		/// This margin accounts for two effects:
		///  > When an item is added, the item count is already incremented while the top index is still lower.
		///  > When the user want to reactivate vertical auto scroll while a lot of data is being received, the margin "glues" scrolling.
		/// </remarks>
		public virtual bool VerticalScrollBarIsNearBottom
		{
			get { return (TopIndex >= ((Items.Count - FullyVisibleItemCount) - (FullyVisibleItemCount / 2))); }
		}

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
			DebugVerticalAutoScroll("Doing VerticalScrollToBottom()...");

			TopIndex = (Items.Count - FullyVisibleItemCount);

			if (this.userIsScrolling)
			{
				this.userIsScrolling = false;

				DebugVerticalAutoScroll("User has ended scrolling");
			}

			this.previousTopIndex = TopIndex; // Update.

			DebugVerticalAutoScroll("...VerticalScrollToBottom() done!");
		}

		/// <summary>
		/// Vertically scroll the list to the bottom if no items are selected.
		/// </summary>
		/// <remarks>
		/// Note that items gets deselected if another control gets the focus.
		/// </remarks>
		public bool VerticalScrollToBottomIfNoItemIsSelected()
		{
			if (!ItemIsSelected) // Note that items gets deselected if another control gets the focus.
			{
				VerticalScrollToBottom();
				return (true);
			}

			return (false);
		}

		/// <summary>
		/// Vertically scroll the list to the bottom if no visible items are selected.
		/// </summary>
		/// <remarks>
		/// Note that items gets deselected if another control gets the focus.
		/// </remarks>
		public bool VerticalScrollToBottomIfNoVisibleItemIsSelected()
		{
			if (!VisibleItemIsSelected) // Note that items gets deselected if another control gets the focus.
			{
				SelectedIndices.Clear(); // Clear selection to ensure that scrolling continues.
				VerticalScrollToBottom();
				return (true);
			}

			return (false);
		}

		/// <summary>
		/// Vertically scroll the list to the bottom if no visible items are selected, except for the last.
		/// </summary>
		public bool VerticalScrollToBottomIfNoVisibleItemOrOnlyOneOfTheLastItemsIsSelected()
		{
			if (VerticalScrollToBottomIfNoVisibleItemIsSelected())
				return (true);

			// There are visible items!

			if (VerticalScrollToBottomIfOnlyOneOfTheLastItemsIsSelected())
				return (true);

			return (false);
		}

		/// <summary>
		/// Vertically scroll the list to the bottom if no visible items are selected, except for the last.
		/// </summary>
		public bool VerticalScrollToBottomIfOnlyOneOfTheLastItemsIsSelected()
		{
			if (OnlyOneOfTheLastItemsIsSelected) // Note that items gets deselected if another control gets the focus.
			{
				SelectedIndices.Clear(); // Clear selection to ensure that scrolling continues.
				VerticalScrollToBottom();
				return (true);
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

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG_VERTICAL_AUTO_SCROLL")]
		protected virtual void DebugVerticalAutoScroll(string leadMessage)
		{
			Debug.WriteLine(string.Format("{0} : ClientHeight = {1} | ClientItemCapacity = {2} | ItemCount = {3} | FullyVisibleItemCount = {4} | TopIndex = {5} | BottomIndex = {6}", leadMessage, ClientSize.Height, ClientItemCapacity, Items.Count, FullyVisibleItemCount, TopIndex, BottomIndex));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
