using System;
using System.ComponentModel;

namespace MKY.IO.Ports
{
	partial class SerialPortDotNet
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private bool _isDisposed;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				try
				{
					if (disposing && (components != null))
					{
						components.Dispose();
					}
					_isDisposed = true;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
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
		/// Returns whether object has already been disposed.
		/// </summary>
		[Browsable(false)]
		public bool IsDisposed
		{
			get { return (_isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (_isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}
	}
}
