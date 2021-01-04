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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Windows.Forms;

using MKY.Diagnostics;

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

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

		/// <summary>
		/// Returns whether a message source has previously been registered and this class is ready
		/// to create handlers.
		/// </summary>
		public static bool MessageSourceIsRegistered
		{
			get { return (staticMainForm != null); }
		}

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <remarks>
		/// Using term 'MainForm' same as the argument of the <see cref="Application.Run(Form)"/>
		/// method. Obviously, Windows.Forms uses the concept of a 'MainForm'.
		/// </remarks>
		public static void RegisterMainForm(Form mainForm)
		{
			UnregisterMainForm();
			{
				Debug.WriteLine(@"Registering main form """ + mainForm.ToString() + @""" as native message source.");
				staticMainForm = mainForm;

				foreach (NativeMessageHandler handler in staticMessageHandlers)
					handler.Register(mainForm);
			}
		}

		/// <remarks>
		/// Using term 'MainForm' same as the argument of the <see cref="Application.Run(Form)"/>
		/// method. Obviously, Windows.Forms uses the concept of a 'MainForm'.
		/// </remarks>
		public static void UnregisterMainForm()
		{
			if (staticMainForm != null)
			{
				foreach (NativeMessageHandler handler in staticMessageHandlers)
					handler.Unregister();

				staticMainForm = null;
				Debug.WriteLine("Main form unregistered as native message source.");
			}
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

		/// <remarks>
		/// Ensure <see cref="MessageSourceIsRegistered"/> returns <c>true</c> before calling this
		/// constructor; <c>InvalidOperationException</c> will be thrown otherwise.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// Thrown if no message source has previously been registered.
		/// </exception>
		public NativeMessageHandler(NativeMessageCallback callback)
		{
			if (staticMainForm != null)
			{
				this.messageCallback = callback;
				Register(staticMainForm);

				Debug.WriteLine(@"Native message handler registered at """ + staticMainForm.ToString() + @""".");
			}
			else
			{
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "no message source to register at!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <remarks>
		/// Using term 'mainForm' same as the argument of the <see cref="Application.Run(Form)"/>
		/// method. Obviously, Windows.Forms uses the concept of a 'MainForm'.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="mainForm"/> is <c>null</c>.
		/// </exception>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Just relax in the 'free'd' world...")]
		protected void Register(Form mainForm)
		{
			if (mainForm == null)
				throw (new ArgumentNullException("mainForm", MessageHelper.InvalidExecutionPreamble + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			this.mainForm = mainForm;

			if (this.mainForm.InvokeRequired)
			{
				var methodToSynchronize = new Action<Form>(RegisterMainFormSynchronized);
				object[] args = { this.mainForm };
				this.mainForm.Invoke(methodToSynchronize, args);
			}
			else
			{
				RegisterMainFormSynchronized(this.mainForm);
			}

			staticMessageHandlers.Add(this);
		}

		/// <summary></summary>
		protected void Unregister()
		{
			staticMessageHandlers.Remove(this);

			if (this.mainForm != null)
			{
				if (this.mainForm.InvokeRequired)
				{
					var methodToSynchronize = new Action<Form>(UnregisterMainFormSynchronized);
					object[] args = { mainForm };
					this.mainForm.Invoke(methodToSynchronize, args);
				}
				else
				{
					UnregisterMainFormSynchronized(this.mainForm);
				}

				this.mainForm = null;
			}

			ReleaseHandle();
		}

		private void RegisterMainFormSynchronized(Form mainForm)
		{
			if (mainForm.IsHandleCreated)
				AssignHandle(mainForm.Handle);

			mainForm.HandleCreated   += mainForm_HandleCreated;
			mainForm.HandleDestroyed += mainForm_HandleDestroyed;
		}

		private void UnregisterMainFormSynchronized(Form mainForm)
		{
			mainForm.HandleCreated   -= mainForm_HandleCreated;
			mainForm.HandleDestroyed -= mainForm_HandleDestroyed;
		}

		/// <remarks>
		/// Intentionally using the term 'Close' opposed to 'Dispose', to indicate that nothing gets
		/// free'd but rather this handler gets closed.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Just relax in the 'free'd' world...")]
		public void Close()
		{
			Unregister();
			this.messageCallback = null;

			Debug.WriteLine("Native message handler unregistered.");
		}

		/// <summary>
		/// Invokes the default window procedure associated with this window.
		/// </summary>
		/// <param name="m">
		/// A System.Windows.Forms.Message that is associated with the current Windows message.
		/// </param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			try
			{
				messageCallback(ref m);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Exception during message callback!");
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
