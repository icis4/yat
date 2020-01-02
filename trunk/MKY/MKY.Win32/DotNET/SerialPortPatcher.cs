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

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Ports;

using Microsoft.Win32.SafeHandles;

using MKY.Diagnostics;

namespace MKY.Win32.DotNet
{
	/// <summary>
	/// Implements a workaround to the <see cref="IOException"/> issue in <see cref="SerialPort"/>
	/// based on http://zachsaw.blogspot.ch/2010/07/serialport-ioexception-workaround-in-c.html
	/// by Zach Saw.
	///
	/// Advantages:
	/// Prevents the exception (lookahead) instead of handling it (lookbehind),
	/// the implementation of the workaround is properly encapsulated.
	///
	/// Big disadvantage:
	/// Requires 'Microsoft.Win32.SafeHandles' and several Win32 APIs, thus
	/// introduces platform dependency.
	///
	/// The <see cref="IOException"/> issue can also be solved by actively managing the
	/// <see cref="SerialPort.BaseStream"/>. The implementation in 'MKY.IO.Ports.SerialPort'
	/// uses this approach, in order to not depend upon Win32 specifics.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Patcher", Justification = "What's wrong with 'Patcher'?")]
	public class SerialPortPatcher : IDisposable, IDisposableEx
	{
		/// <summary>
		/// Applies the fix to the given serial port.
		/// </summary>
		public static void ApplyTo(string portName)
		{
			using (new SerialPortPatcher(portName))
			{
			}
		}

		private SafeFileHandle handle;

		private SerialPortPatcher(string portName)
		{
			if (string.IsNullOrEmpty(portName) || !StringEx.StartsWithOrdinalIgnoreCase(portName, "COM"))
				throw (new ArgumentException(@"Invalid serial port name, must be ""COM...""!", "portName"));

			SafeFileHandle h = FileIO.NativeMethods.CreateFile
			(
				@"\\.\" + portName,
				FileIO.NativeTypes.Access.GENERIC_READ_WRITE,
				FileIO.NativeTypes.ShareMode.SHARE_NONE,
				IntPtr.Zero,
				FileIO.NativeTypes.CreationDisposition.OPEN_EXISTING,
				FileIO.NativeTypes.AttributesAndFlags.FLAG_OVERLAPPED,
				IntPtr.Zero
			);

			if (h.IsInvalid)
				throw (WinError.LastErrorToIOException());

			try
			{
				var fileType = FileApi.NativeMethods.GetFileType_(h);
				switch (fileType)
				{
					case FileApi.NativeTypes.FileType.FILE_TYPE_CHAR:
					case FileApi.NativeTypes.FileType.FILE_TYPE_UNKNOWN:
					{
						InitializeDCB(h);
						this.handle = h;
						break;
					}

					default:
					{
						throw (new ArgumentException("Invalid serial port handle", "portName"));
					}
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex, "Failed to initialize serial port handle!");

				h.Close();
				this.handle = null;
				throw;
			}
		}

		private static void InitializeDCB(SafeFileHandle handle)
		{
			WinBase.NativeTypes.DCB dcb;
			WinBase.GetCommState(handle, out dcb);
			dcb.Flags.AbortOnError = false;
			WinBase.SetCommState(handle, ref dcb);
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					if (this.handle != null)
					{
						this.handle.Close();
						this.handle = null;
					}
				}

				// Set state to disposed:
				IsDisposed = true;
			}
		}

	#if (DEBUG)
		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		///
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		///
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~SerialPortPatcher()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}
	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
