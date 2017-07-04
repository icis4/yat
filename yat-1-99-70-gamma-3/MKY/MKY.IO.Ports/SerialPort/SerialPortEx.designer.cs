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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		protected override void Dispose(bool disposing)
		{
			// Modified version of the designer generated Dispose() method:
			if (!IsDisposed)
			{
				this.eventHelper.DiscardAllEventsAndExceptions();

				// Dispose of managed resources if requested:
				if (disposing)
				{
					try
					{
						// This fixes the deadlock issue described in the header of this class.
						// Attention, the base stream is only available if the port is open!
						if ((this.IsOpen) && (this.BaseStream != null))
						{
							// Attention, do not call BaseStream.Flush() !!!
							// It will block if the device is no longer available !!!

							this.BaseStream.Close();

							// Attention, do not call BaseStream.Dispose() !!!
							// It can throw after a call to Close() !!!
						}
					}
					catch (Exception) { } // May be 'IOException' or 'ObjectDisposedException' or ...
				}

				// Dispose designer generated managed resources if requested:
				if (disposing && (components != null))
				{
					components.Dispose();
				}

				// Set state to disposed:
				IsDisposed = true;
			}

			try
			{
				// This fixes a result of the deadlock issue described in the header of this class.
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
