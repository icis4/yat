using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MKY.IO.Ports
{
	partial class SerialPortEx
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		protected override void Dispose(bool disposing)
		{
			// Modified version of the designer generated Dispose() method:

			if (this.eventHelper != null) // Possible when called by finalizer (non-deterministic).
				this.eventHelper.DiscardAllEventsAndExceptions();

			// Dispose of managed resources:
			if (disposing)
			{
				// --------------------------------------------------------------------------------
				// Begin of patches to the issues described in ".\!-Doc\*.txt".
				// --------------------------------------------------------------------------------
				// With .NET 4.7.1 and above the .NET Framework itself contains workarounds for the
				// documented 'ObjectDisposedException' and 'UnauthorizedAccssException' issues.
				// However, as those workarounds do not really solve the issues, only workaround
				// them and the patches applied by this 'SerialPortEx' class should not interfere
				// with those workarounds, it has been decided to keep the patches for the moment.
				// --------------------------------------------------------------------------------

				TryToApplyEventLoopHandlerPatchAndCloseBaseStreamSafely(this.baseStreamReferenceForCloseSafely);
				this.baseStreamReferenceForCloseSafely = null;

				// --------------------------------------------------------------------------------
				// End of patches to the issues described in ".\!-Doc\*.txt".
				// --------------------------------------------------------------------------------
			}

			// Dispose of designer generated managed resources:
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			// ------------------------------------------------------------------------------------
			// Begin of patches to the issues described in ".\!-Doc\*.txt".
			// ------------------------------------------------------------------------------------
			// With .NET 4.7.1 and above the .NET Framework itself contains workarounds for the
			// documented 'ObjectDisposedException' and 'UnauthorizedAccssException' issues.
			// However, as those workarounds do not really solve the issues, only workaround
			// them and the patches applied by this 'SerialPortEx' class should not interfere
			// with those workarounds, it has been decided to keep the patches for the moment.
			// ------------------------------------------------------------------------------------

			try
			{
				// Try to patch some of the issues described in the ".\!-Doc\*.txt" files:
				base.Dispose(disposing);
			}
			catch // May be 'UnauthorizedAccessException' or ...
			{
			}

			// ------------------------------------------------------------------------------------
			// End of patches to the issues described in ".\!-Doc\*.txt".
			// ------------------------------------------------------------------------------------
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}

		#endregion
	}
}
