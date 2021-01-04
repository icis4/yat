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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

// Ideally, the two properties 'HorizontalAutoScroll' and 'VerticalAutoScroll' and the corresponding
// automatism would be supported by this ListBox extension. However, no feasible implementation has
// been found. Thus, it was decided to skip the automatism and simply provide the necessary methods
// that allow the control's parent to trigger scrolling.

// Enable to continue working/testing an automatically vertically scrolling list box:
//#define ENABLE_VERTICAL_AUTO_SCROLL

// \todo MKY 2016-09-19 (related to upgrade of ExSim)
// When enabling 'ENABLE_HORIZONTAL_AUTO_SCROLL', a reference to 'MKY.Win32' and thus Win32 API
// calls is required. This is undesirable, as there shall be no references to the Win32 API from
// within true .NET assemblies. As a consequence, when enabling 'ENABLE_HORIZONTAL_AUTO_SCROLL' for
// e.g. testing or analysis, the reference to 'MKY.Win32' must manually be added.

// Enable to continue working/testing an automatically horizontally scrolling list box:
//#define ENABLE_HORIZONTAL_AUTO_SCROLL

#if (DEBUG)

	// Enable debugging of general stuff:
////#define DEBUG_CLIENT_AREA               // The 'DebugEnabled' property must also be set!

	// Attention, requires to be enabled in multiple files!
////#define DEBUG_COUNT_AND_INDICES         // The 'DebugEnabled' property must also be set!

	// Enable debugging of vertical semi-auto scrolling:
////#define DEBUG_VERTICAL_SEMI_AUTO_SCROLL // The 'DebugEnabled' property must also be set!

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
#if (ENABLE_HORIZONTAL_AUTO_SCROLL)
using System.Runtime.InteropServices;
using System.Text;
#endif
using System.Text.RegularExpressions;
using System.Windows.Forms;

#if (ENABLE_HORIZONTAL_AUTO_SCROLL)
using MKY.Win32;
#endif

#endregion

namespace MKY.Windows.Forms
{
	/// <summary>
	/// An improved <see cref="ListBox"/> that additionally provides:
	/// <list type="bullet">
	/// <item><description>The <see cref="SelectAll"/> method.</description></item>
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

////#if (DEBUG) must not be active, configuration must always be available for designer.

		/// <remarks>Public for use in parent controls.</remarks>
		public const bool DebugEnabledDefault = false;

////#endif

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
		private int fullyVisibleClientItemCapacity; // = 0
		private int totalVisibleClientItemCapacity; // = 0

		private int previousTopIndex; // = 0;
		private bool userIsScrolling; // = false;

		private int lastSelectedIndex = ControlEx.InvalidIndex;

////#if (DEBUG) must not be active, configuration must always be available for designer.
		private bool debugEnabled = DebugEnabledDefault;
////#endif

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

		#region Overriden Methods
		//==========================================================================================
		// Overriden Methods
		//==========================================================================================

		/// <summary>
		/// Raises the <see cref="E:DrawItem" /> event.
		/// </summary>
		/// <param name="e">The <see cref="DrawItemEventArgs"/> instance containing the event data.</param>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (!this.userIsScrolling)
				EvaluateWhetherUserIsScrolling();

			base.OnDrawItem(e);

			DebugCountAndIndices("TopIndex is going to be retrieved");
			this.previousTopIndex = TopIndex;
		}

		/// <summary>
		/// Raises the <see cref="E:SelectedIndexChanged" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			if (SelectedIndices.Count > 0)
				this.lastSelectedIndex = SelectedIndices[0];

			base.OnSelectedIndexChanged(e);
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
		protected virtual void EvaluateClientItemCapacity()
		{
			if (DrawMode != DrawMode.OwnerDrawVariable)
			{
				this.clientItemCapacity = (float)ClientSize.Height / (float)ItemHeight;

				this.fullyVisibleClientItemCapacity = (int)(Math.Floor(this.clientItemCapacity));
				this.totalVisibleClientItemCapacity = (int)(Math.Ceiling(this.clientItemCapacity));

				DebugClientArea("ClientItemCapacity evaluated");
			}
			else
			{
				DebugClientArea("ClientItemCapacity cannot be evaluated for 'DrawMode.OwnerDrawVariable'!");
			}
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
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual float ClientItemCapacity
		{
			get { return (this.clientItemCapacity); }
		}

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
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int FullyVisibleClientItemCapacity
		{
			get { return (this.fullyVisibleClientItemCapacity); }
		}

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
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int TotalVisibleClientItemCapacity
		{
			get { return (this.totalVisibleClientItemCapacity); }
		}

		#endregion

		#region Items
		//==========================================================================================
		// Items
		//==========================================================================================

		/// <summary>
		/// The zero-based index of the first item in the control.
		/// </summary>
		/// <remarks>
		/// If no items are available, this property returns <see cref="ControlEx.InvalidIndex"/>.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int FirstIndex
		{
			get
			{
				int result = Math.Max(ControlEx.InvalidIndex, 0);
				return (result);
			}
		}

		/// <summary>
		/// The zero-based index of the last item in the control.
		/// </summary>
		/// <remarks>
		/// If no items are available, this property returns <see cref="ControlEx.InvalidIndex"/>.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int LastIndex
		{
			get
			{
				int result = Math.Max(ControlEx.InvalidIndex, (Items.Count - 1));
				return (result);
			}
		}

		/// <summary>
		/// The first item in the control.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object FirstItem
		{
			get
			{
				if (Items.Count > 0)
					return (Items[0]);
				else
					return (null);
			}
		}

		/// <summary>
		/// The  last item in the control.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object LastItem
		{
			get
			{
				if (Items.Count > 0)
					return (Items[LastIndex]);
				else
					return (null);
			}
		}

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
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int FullyVisibleItemCount
		{
			get { return (Math.Min(FullyVisibleClientItemCapacity, Items.Count)); }
		}

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
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int TotalVisibleItemCount
		{
			get { return (Math.Min(TotalVisibleClientItemCapacity, Items.Count)); }
		}

		/// <summary>
		/// The zero-based index of the last visible item in the control.
		/// If the last item is only partially visible, that index is returned.
		/// </summary>
		/// <remarks>
		/// Same as <see cref="ListBox.TopIndex"/>, this property initially returns zero (0).
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int BottomIndex
		{
			get
			{
				// Debug output...
				// ...is not required here, it is already done at all places where 'BottomIndex' is retrieved.
				// ...and must not be done here, it would result in recursion => stack overflow!
			////DebugCountAndIndices("TopIndex is going to be retrieved")

				int unsafeResult = (TopIndex + (TotalVisibleItemCount - 1));
				int   safeResult = Int32Ex.Limit(unsafeResult, 0, Math.Max((Items.Count - 1), 0)); // 'max' must be 0 or above.
				return (safeResult);
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
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool ItemIsSelected
		{
			get { return (SelectedItems.Count > 0); }
		}

		/// <remarks>
		/// Note that items gets deselected if another control gets the focus.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool VisibleItemIsSelected
		{
			get
			{
				foreach (int i in SelectedIndices)
				{
					DebugCountAndIndices("Indices are going to be retrieved");

					if ((i >= TopIndex) && (i <= BottomIndex))
						return (true);
				}

				return (false);
			}
		}

		/// <remarks>
		/// Note that items gets deselected if another control gets the focus.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
		/// Gets the index of the last selected item.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int LastSelectedIndex
		{
			get { return (this.lastSelectedIndex); }
		}

		/// <summary>
		/// Select all items within the list box.
		/// </summary>
		public void SelectAll()
		{
			for (int i = 0; i < Items.Count; i++)
				SetSelected(i, true);
		}

		#endregion

		#region Find
		//==========================================================================================
		// Find
		//==========================================================================================

		/// <summary>
		/// Finds the next item in the <see cref="ListBox"/> that matches the given text and/or regex.
		/// </summary>
		/// <remarks>
		/// The <see cref="ListBox.FindString(string, int)"/> method seems promising at first,
		/// but there are severe limitations:
		/// <list type="bullet">
		/// <item><description>Only searches simple case-sensitive matches.</description></item>
		/// <item><description>Only searches down.</description></item>
		/// <item><description>...when reaches the bottom...it continues searching from the top...</description></item>
		/// <item><description>...first item...that starts with the specified string...</description></item>
		/// </list>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The <paramref name="startIndex"/> parameter is less than zero or greater than or equal
		/// to the value of the <see cref="ListBox.ObjectCollection.Count"/> property.
		/// </exception>
		public virtual int FindNext(string text, bool textCaseSensitive, bool textWholeWord, Regex regex, int startIndex)
		{
			if (startIndex < NoMatches)
				throw (new ArgumentOutOfRangeException("startIndex", startIndex, "The start index is less than 'ListBox.NoMatches'!")); // Do not decorate with 'InvalidExecutionPreamble/SubmitBug' as this exception is eligible during normal execution.

			if (startIndex >= Items.Count)
				throw (new ArgumentOutOfRangeException("startIndex", startIndex, "The start index is greater or equal 'Item.Count'!")); // Do not decorate with 'InvalidExecutionPreamble/SubmitBug' as this exception is eligible during normal execution.

			if (Items.Count > 0)
			{
				if (startIndex == NoMatches)
					startIndex = 0; // Same behavior as 'ListBox.FindString(string, int)' method.

				for (int i = startIndex; i < Items.Count; i++)
				{
					var str = Items[i].ToString();

					if (!string.IsNullOrEmpty(text))
					{
						if (TryFind(str, text, textCaseSensitive, textWholeWord))
							return (i);
					}

					if (regex != null)
					{
						if (regex.IsMatch(str))
							return (i);
					}
				}
			}

			return (NoMatches);
		}

		/// <summary>
		/// Finds the previous item in the <see cref="ListBox"/> that matches the given text and/or regex.
		/// </summary>
		/// <remarks>
		/// The <see cref="ListBox.FindString(string, int)"/> method seems promising at first,
		/// but there are severe limitations:
		/// <list type="bullet">
		/// <item><description>Only searches simple case-sensitive matches.</description></item>
		/// <item><description>Only searches down.</description></item>
		/// <item><description>...when reaches the bottom...it continues searching from the top...</description></item>
		/// <item><description>...first item...that starts with the specified string...</description></item>
		/// </list>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The <paramref name="startIndex"/> parameter is less than zero or greater than or equal
		/// to the value of the <see cref="ListBox.ObjectCollection.Count"/> property.
		/// </exception>
		public virtual int FindPrevious(string text, bool textCaseSensitive, bool textWholeWord, Regex regex, int startIndex)
		{
			if (startIndex < NoMatches)
				throw (new ArgumentOutOfRangeException("startIndex", startIndex, "The start index is less than 'ListBox.NoMatches'!")); // Do not decorate with 'InvalidExecutionPreamble/SubmitBug' as this exception is eligible during normal execution.

			if (startIndex >= Items.Count)
				throw (new ArgumentOutOfRangeException("startIndex", startIndex, "The start index is greater or equal 'Item.Count'!")); // Do not decorate with 'InvalidExecutionPreamble/SubmitBug' as this exception is eligible during normal execution.

			if (Items.Count > 0)
			{
				if (startIndex == NoMatches)
					startIndex = 0; // Same behavior as 'ListBox.FindString(string, int)' method.

				for (int i = startIndex; i >= 0; i--)
				{
					var str = Items[i].ToString();

					if (!string.IsNullOrEmpty(text))
					{
						if (TryFind(str, text, textCaseSensitive, textWholeWord))
							return (i);
					}

					if (regex != null)
					{
						if (regex.IsMatch(str))
							return (i);
					}
				}
			}

			return (NoMatches);
		}

		/// <summary></summary>
		protected virtual bool TryFind(string str, string findText, bool caseSensitive, bool wholeWord)
		{
			StringComparison comparisonType;
			if (caseSensitive)
				comparisonType = StringComparison.CurrentCulture;
			else
				comparisonType = StringComparison.CurrentCultureIgnoreCase;

			if (wholeWord)
				return (StringEx.IndexOfWholeWord(str, findText, comparisonType) >= 0);
			else
				return (str.IndexOf(findText, comparisonType) >= 0); // Using string.IndexOf() because string.Contains()
		}                                                            // does not allow controlling culture and case.

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
						VerticalScrollToBottomIfNoItemIsSelected();
				}
				else
				{
					this.verticalAutoScroll = false;
				}
			}
		}

	#endif // ENABLE_VERTICAL_AUTO_SCROLL

		/// <summary></summary>
		protected virtual void EvaluateWhetherUserIsScrolling()
		{
			DebugCountAndIndices("TopIndex is going to be retrieved");

			if (!this.userIsScrolling && (this.previousTopIndex > TopIndex))
			{
				this.userIsScrolling = true;
				DebugVerticalSemiAutoScroll("User has started scrolling.......");
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool UserIsScrolling
		{
			get { return (this.userIsScrolling); }
		}

		/// <remarks>
		/// "NearBottom" means at bottom or at least half the visible items close to it.
		/// This margin accounts for two effects:
		///  > When an item is added, the item count is already incremented while the top index is still lower.
		///  > When the user wants to reactivate vertical auto scroll while a lot of data is being received, the margin "glues" scrolling.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool VerticalScrollBarIsNearBottom
		{
			get
			{
				int glueIndex = ((Items.Count - FullyVisibleItemCount) - (FullyVisibleItemCount / 2));

				DebugCountAndIndices("TopIndex is going to be retrieved");

				return (TopIndex >= glueIndex);
			}
		}

////#if (DEBUG) must not be active, configuration must always be available for designer.

		/// <remarks>
		/// Flag in a addition to configuration items to allow selective debugging of just a single
		/// list box, in order to reduce debug output.
		/// </remarks>
		[Category("Behavior")]
		[Description("Enables or disables debugging.")]
		[DefaultValue(DebugEnabledDefault)]
		public virtual bool DebugEnabled
		{
			get { return (this.debugEnabled); }
			set { this.debugEnabled = value;  }
		}

////#endif

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
		/// Notifies the control that the items got cleared.
		/// </summary>
		/// <remarks>
		/// Required to reset <see cref="UserIsScrolling"/>.
		/// </remarks>
		public void NotifyCleared()
		{
			this.userIsScrolling = false;
			this.previousTopIndex = 0;
		}

		/// <summary>
		/// Vertically scroll the list to the bottom.
		/// </summary>
		public void VerticalScrollToBottom()
		{
			if (this.userIsScrolling)
			{
				this.userIsScrolling = false;
				DebugVerticalSemiAutoScroll("User has ended scrolling.........");
			}

			int intendedTopIndex = (Items.Count - FullyVisibleItemCount);

			DebugCountAndIndices("TopIndex is going to be retrieved");

			if (TopIndex != intendedTopIndex)
			{
				DebugVerticalSemiAutoScroll(string.Format("VerticalScrollToBottom() is about to scroll to intended 'TopIndex' of {0}", intendedTopIndex));
				DebugCountAndIndices("TopIndex is going to be changed..");

				TopIndex = intendedTopIndex;
			}
			else
			{
				DebugVerticalSemiAutoScroll(string.Format("VerticalScrollToBottom() has been skipped since current 'TopIndex' already is at intended 'TopIndex' of {0}", intendedTopIndex));
			}

			DebugCountAndIndices("TopIndex is going to be retrieved");

			this.previousTopIndex = TopIndex;
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
				DebugVerticalSemiAutoScroll("VerticalScrollToBottomIfNoItemIsSelected() is being done...");

				VerticalScrollToBottom();
				return (true);
			}
			else
			{
				DebugVerticalSemiAutoScroll("VerticalScrollToBottomIfNoItemIsSelected() has been skipped since no item is selected");
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
				DebugVerticalSemiAutoScroll("VerticalScrollToBottomIfNoVisibleItemIsSelected() is being done...");

				if (SelectedIndices.Count > 0)
					SelectedIndices.Clear(); // Clear selection to ensure that scrolling continues.

				VerticalScrollToBottom();
				return (true);
			}
			else
			{
				DebugVerticalSemiAutoScroll("VerticalScrollToBottomIfNoVisibleItemIsSelected() has been skipped since no visible item is selected");
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
		/// Vertically scroll the list to the bottom if only one of the last items is selected.
		/// </summary>
		public bool VerticalScrollToBottomIfOnlyOneOfTheLastItemsIsSelected()
		{
			if (OnlyOneOfTheLastItemsIsSelected) // Note that items gets deselected if another control gets the focus.
			{
				DebugVerticalSemiAutoScroll("VerticalScrollToBottomIfOnlyOneOfTheLastItemsIsSelected() is being done...");

				SelectedIndices.Clear(); // Clear selection to ensure that scrolling continues.
				VerticalScrollToBottom();
				return (true);
			}
			else
			{
				DebugVerticalSemiAutoScroll("VerticalScrollToBottomIfOnlyOneOfTheLastItemsIsSelected() has been skipped since more than the last items is selected");
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

		#region Scroll > Event Raising
		//------------------------------------------------------------------------------------------
		// Scroll > Event Raising
		//------------------------------------------------------------------------------------------

	#if (ENABLE_HORIZONTAL_AUTO_SCROLL)

		/// <summary></summary>
		protected virtual void OnHorizontalScrolledAsync(ScrollEventArgs e)
		{
			EventHelper.RaiseAsync<ScrollEventArgs>(HorizontalScrolled, this, e);
		}

		/// <summary></summary>
		protected virtual void OnVerticalScrolledAsync(ScrollEventArgs e)
		{
			EventHelper.RaiseAsync<ScrollEventArgs>(VerticalScrolled, this, e);
		}

	#endif // ENABLE_HORIZONTAL_AUTO_SCROLL

		#endregion

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_CLIENT_AREA")]
		private void DebugClientArea(string leadMessage)
		{
			if (DebugEnabled)
			{
				Debug.WriteLine
				(
					string.Format
					(
						CultureInfo.CurrentCulture,
						"{0} : ClientHeight = {1} | ClientItemCapacity = {2} | FullyVisibleClientItemCapacity = {3} | TotalVisibleClientItemCapacity = {4}",
						leadMessage,
						ClientSize.Height,
						ClientItemCapacity,
						FullyVisibleClientItemCapacity,
						TotalVisibleClientItemCapacity
					)
				);
			}
		}

		/// <remarks>
		/// Attention, requires that <see cref="ConditionalAttribute"/> is activated in in multiple files!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[Conditional("DEBUG_COUNT_AND_INDICES")]
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
		protected virtual void DebugCountAndIndices(string leadMessage)
		{
			DebugCountAndIndices(leadMessage, BottomIndex);
		}

		/// <remarks>
		/// Attention, requires that <see cref="ConditionalAttribute"/> is activated in in multiple files!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[Conditional("DEBUG_COUNT_AND_INDICES")]
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
		protected virtual void DebugCountAndIndices(string leadMessage, int bottomIndex)
		{
			if (DebugEnabled)
			{
				Debug.WriteLine
				(
					string.Format
					(
						CultureInfo.CurrentCulture,
						"{0} : ItemCount = {1} | FullyVisibleItemCount = {2} | TotalVisibleItemCount = {3} | TopIndex = {4} | BottomIndex = {5}",
						leadMessage,
						Items.Count,
						FullyVisibleItemCount,
						TotalVisibleItemCount,
						TopIndex,
						bottomIndex
					)
				);
			}
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_VERTICAL_SEMI_AUTO_SCROLL")]
		private void DebugVerticalSemiAutoScroll(string message)
		{
			if (DebugEnabled)
			{
				Debug.WriteLine(message);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
