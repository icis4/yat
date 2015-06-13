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
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Windows.Forms;

#endregion

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Native message handler delegate.
	/// </summary>
	/// <param name="m">
	/// A System.Windows.Forms.Message that is associated with the current Windows message.
	/// </param>
	[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "This design is given by the Windows.Forms message handling design.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "m", Justification = "Naming according to parameter 'm' of NativeWindow methods.")]
	public delegate void NativeMessageCallback(ref Message m);

	/// <summary>
	/// Utility class that simplifies processing native messages. The class acts as an interface
	/// between consumers of native messages and the main form. The main form is needed to provide
	/// the window handle. Without this handle, consumers cannot properly register callbacks.
	/// </summary>
	/// <remarks>
	/// This class helps to reduce the coupling among the main form and other parts of the system.
	/// The main form and the consumer need to access this handler, but they don't need to know of
	/// each other. Thus, this class implements a simple form of the mediator pattern.
	/// </remarks>
	public class NativeMessageHandler : NativeWindow
	{
		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		private static Form staticMainForm;
		private static List<NativeMessageHandler> staticMessageHandlers = new List<NativeMessageHandler>();

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <remarks>
		/// Using the term 'MainForm' as used for the argument of the <see>Application.Run</see>
		/// method. Obviously, Windows.Forms uses the concept of a 'MainForm'.
		/// </remarks>
		public static void RegisterMainForm(Form mainForm)
		{
			UnregisterMainForm();

			staticMainForm = mainForm;

			foreach (NativeMessageHandler handler in staticMessageHandlers)
				handler.Register(mainForm);
		}

		/// <remarks>
		/// Using the term 'MainForm' as used for the argument of the <see>Application.Run</see>
		/// method. Obviously, Windows.Forms uses the concept of a 'MainForm'.
		/// </remarks>
		public static void UnregisterMainForm()
		{
			foreach (NativeMessageHandler handler in staticMessageHandlers)
				handler.Unregister();
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Form mainForm;
		private NativeMessageCallback messageCallback;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public NativeMessageHandler(NativeMessageCallback callback)
		{
			this.messageCallback = callback;
			Register(staticMainForm);
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <remarks>
		/// Intentionally using the term 'Close' opposed to 'Dispose', to indicate that nothing gets
		/// free'd but rather this handler gets closed.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Just relax in the 'free'd' world...")]
		protected void Register(Form mainForm)
		{
			if (mainForm.IsHandleCreated)
				AssignHandle(mainForm.Handle);

			this.mainForm = mainForm;
			this.mainForm.HandleCreated   += mainForm_HandleCreated;
			this.mainForm.HandleDestroyed += mainForm_HandleDestroyed;

			staticMessageHandlers.Add(this);
		}

		/// <remarks>
		/// Intentionally using the term 'Close' opposed to 'Dispose', to indicate that nothing gets
		/// free'd but rather this handler gets closed.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Just relax in the 'free'd' world...")]
		protected void Unregister()
		{
			staticMessageHandlers.Remove(this);

			if (this.mainForm != null)
			{
				this.mainForm.HandleCreated   -= mainForm_HandleCreated;
				this.mainForm.HandleDestroyed -= mainForm_HandleDestroyed;
				this.mainForm = null;
			}

			ReleaseHandle();
		}

		/// <remarks>
		/// Intentionally using the term 'Close' opposed to 'Dispose', to indicate that nothing gets
		/// free'd but rather this handler gets closed.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Just relax in the 'free'd' world...")]
		protected void Close()
		{
			Unregister();
			this.messageCallback = null;
		}

		/// <summary>
		/// Invokes the default window procedure associated with this window.
		/// </summary>
		/// <param name="m">
		/// A System.Windows.Forms.Message that is associated with the current Windows message.
		/// </param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			try
			{
				messageCallback(ref m);
			}
			catch (Exception ex)
			{
				Diagnostics.DebugEx.WriteException(GetType(), ex);
			}

			base.WndProc(ref m);
		}

		#endregion

		#region Event Handlers
		//==========================================================================================
		// Event Handlers
		//==========================================================================================

		private void mainForm_HandleCreated(object sender, EventArgs e)
		{
			AssignHandle(((Form)sender).Handle);
		}

		private void mainForm_HandleDestroyed(object sender, EventArgs e)
		{
			ReleaseHandle();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
