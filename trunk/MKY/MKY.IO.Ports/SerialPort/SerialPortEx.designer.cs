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

		private bool isDisposed;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			// Modified version of the designer generated Dispose() method:
			if (!this.isDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					try
					{
						// This fixes the deadlock issue described in the header of this class.
						// Attention, the base stream is only available if the port is open!
						if ((this.IsOpen) && (this.BaseStream != null))
						{
							this.BaseStream.Flush();
							this.BaseStream.Close();

							// Attention, do not call BaseStream.Dispose() !!!
							// It can throw after a call to Close() !!!
						}
					}
					catch (ObjectDisposedException) { }     // May happen on BaseStream.Close().
					catch (UnauthorizedAccessException) { } // May happen on BaseStream.Close().
				}

				// Dispose designer generated managed resources if requested:
				if (disposing && (components != null))
				{
					components.Dispose();
				}

				// Set state to disposed:
				this.isDisposed = true;
			}

			try
			{
				// This fixes a result of the deadlock issue described in the header of this class.
				base.Dispose(disposing);
			}
			catch (UnauthorizedAccessException) { }
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

		/// <summary>
		/// Returns whether the object has already been disposed.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}
	}
}
