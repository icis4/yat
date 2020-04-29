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

			this.eventHelper.DiscardAllEventsAndExceptions();

			// Dispose of managed resources:
			if (disposing)
			{
				// Try to patch some of the issues described in the ".\!-Doc\*.txt" files:
				TryToApplyEventLoopHandlerPatchAndCloseBaseStreamSafely(this.baseStreamReferenceForCloseSafely);
				this.baseStreamReferenceForCloseSafely = null;
			}

			// Dispose of designer generated managed resources:
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			try
			{
				// Try to patch some of the issues described in the ".\!-Doc\*.txt" files:
				base.Dispose(disposing);
			}
			catch (Exception) { } // May be 'UnauthorizedAccessException' or ...
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
