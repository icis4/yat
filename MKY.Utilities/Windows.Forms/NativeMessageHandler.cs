//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Windows.Forms;

namespace MKY.Utilities.Windows.Forms
{
	/// <summary>
	/// Native message handler delegate.
	/// </summary>
	/// <param name="m">
	/// A System.Windows.Forms.Message that is associated with the current Windows message.
	/// </param>
	public delegate void NativeMessageHandlerCallback(ref Message m);

	/// <remarks>
	/// Utility class that allows to process native messages.
	/// </remarks>
	public class NativeMessageHandler : NativeWindow
	{
		private NativeMessageHandlerCallback messageHandlerCallback;

		/// <summary></summary>
		public NativeMessageHandler(NativeMessageHandlerCallback messageHandlerCallback)
		{
			this.messageHandlerCallback = messageHandlerCallback;
		}

		/// <summary>
		/// Gets the handle for this window.
		/// </summary>
		/// <remarks>
		/// If no handle is associated, a new is create once upon first call of this property.
		/// Attention:
		/// Such handle must not be created within the constructor because that would lead
		/// to exceptions in <see cref="WndProc"/>.
		/// </remarks>
		/// <returns>
		/// If successful, an System.IntPtr representing the handle to the associated
		/// native Win32 window; otherwise, 0 if no handle is associated with the window.
		/// </returns>
		public new IntPtr Handle
		{
			get
			{
				if (base.Handle == IntPtr.Zero)
					CreateHandle(new CreateParams());

				return (base.Handle);
			}
		}

		/// <summary>
		/// Invokes the default window procedure associated with this window.
		/// </summary>
		/// <param name="m">
		/// A System.Windows.Forms.Message that is associated with the current Windows message.
		/// </param>
		protected override void WndProc(ref Message m)
		{
			try
			{
				this.messageHandlerCallback(ref m);
				base.WndProc(ref m);
			}
			catch (Exception ex)
			{
				Diagnostics.XDebug.WriteException(this, ex);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
