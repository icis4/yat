using System;
using System.ComponentModel;

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
						DisposeBaseStream_SerialPortBugFix();
					}
					catch { }
				}

				// Dispose designer generated managed resources if requested:
				if (disposing && (components != null))
				{
					components.Dispose();
				}

				// Set state to disposed:
				this.isDisposed = true;
			}

			base.Dispose(disposing);
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
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}
	}
}
