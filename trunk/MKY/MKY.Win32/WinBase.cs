﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.20
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

#endregion

#region Module-level StyleCop suppressions
//==================================================================================================
// Module-level StyleCop suppressions
//==================================================================================================

[module: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1404:CodeAnalysisSuppressionMustHaveJustification", Justification = "Large blocks of module-level FxCop suppressions which were copy-pasted out of FxCop.")]

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates parts of the Win32 error API.
	/// </summary>
	public static class WinBase
	{
		#region Native
		//==========================================================================================
		// Native
		//==========================================================================================

		#region Native > Types
		//------------------------------------------------------------------------------------------
		// Native > Types
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using exact native parameter names.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Using exact native parameter names.")]
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native items are nested on purpose, to emphasize their native nature.")]
		public static class NativeTypes
		{
			// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
			// warnings for each undocumented member below. Documenting each member makes little sense
			// since they pretty much tell their purpose and documentation tags between the members
			// makes the code less readable.
			#pragma warning disable 1591

			/// <summary>
			/// Flags bitfield for the <see cref="COMSTAT"/> structure.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Type name is given by the Win32 API.")]
			[CLSCompliant(false)]
			public struct COMSTATFlags
			{
				private BitVector32 flags;

				public bool CtsHold  { get { return (this.flags[0]); } set { this.flags[0] = value; } }
				public bool DsrHold  { get { return (this.flags[1]); } set { this.flags[1] = value; } }
				public bool RlsdHold { get { return (this.flags[2]); } set { this.flags[2] = value; } }
				public bool XoffHold { get { return (this.flags[3]); } set { this.flags[3] = value; } }
				public bool XoffSent { get { return (this.flags[4]); } set { this.flags[4] = value; } }
				public bool Eof      { get { return (this.flags[5]); } set { this.flags[5] = value; } }
				public bool Txim     { get { return (this.flags[6]); } set { this.flags[6] = value; } }
				public uint Reserved                             // [7]...[32] (25 bits)
				{
					get { throw (new NotImplementedException("Access to 'Reserved' bits not implemented (yet)")); }
					set { throw (new NotImplementedException("Access to 'Reserved' bits not implemented (yet)")); }
				}
			}

			/// <summary>
			/// Contains information about a communications device.
			/// This structure is filled by the <see cref="NativeMethods.ClearCommError"/> function.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct COMSTAT
			{
				public COMSTATFlags Flags;
				public UInt32       cbInQue;
				public UInt32       cbOutQue;
			}

			/// <summary>
			/// Flags bitfield for the <see cref="DCB"/> structure.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Type name is given by the Win32 API.")]
			[CLSCompliant(false)]
			public struct DCBFlags
			{
				private BitVector32 flags;

				public bool Binary           { get { return (this.flags[ 0]); } set { this.flags[ 0] = value; } }
				public bool Parity           { get { return (this.flags[ 1]); } set { this.flags[ 1] = value; } }
				public bool OutxCtsFlow      { get { return (this.flags[ 2]); } set { this.flags[ 2] = value; } }
				public bool OutxDsrFlow      { get { return (this.flags[ 3]); } set { this.flags[ 3] = value; } }
				public uint DtrControl                                        // [4] + [5]
				{
					get { return (BitConverterEx.ToUInt32(      this.flags,        4, 5)); }
					set {         BitConverterEx.FromUInt32(ref this.flags, value, 4, 5);  }
				}
				public bool DsrSensitivity   { get { return (this.flags[ 6]); } set { this.flags[ 6] = value; } }
				public bool TXContinueOnXoff { get { return (this.flags[ 7]); } set { this.flags[ 7] = value; } }
				public bool OutX             { get { return (this.flags[ 8]); } set { this.flags[ 8] = value; } }
				public bool InX              { get { return (this.flags[ 9]); } set { this.flags[ 9] = value; } }
				public bool ErrorChar        { get { return (this.flags[10]); } set { this.flags[10] = value; } }
				public bool Null             { get { return (this.flags[11]); } set { this.flags[11] = value; } }
				public uint RtsControl                                   // [12] + [13]
				{
					get { return (BitConverterEx.ToUInt32(      this.flags,        12, 13)); }
					set {         BitConverterEx.FromUInt32(ref this.flags, value, 12, 13);  }
				}
				public bool AbortOnError     { get { return (this.flags[14]); } set { this.flags[14] = value; } }
				public uint Dummy2                                       // [15]...[31] (17 bits)
				{
					get { throw (new NotImplementedException("Access to 'Dummy2' bits not implemented (yet)")); }
					set { throw (new NotImplementedException("Access to 'Dummy2' bits not implemented (yet)")); }
				}
			}

			/// <summary>
			/// Defines the control setting for a serial communications device.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "All native types are nested on purpose, to emphasize their native nature.")]
			[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Dont' care.")]
			[CLSCompliant(false)]
			[StructLayout(LayoutKind.Sequential)]
			public struct DCB
			{
				public UInt32   DCBlength;
				public UInt32   BaudRate;
				public DCBFlags Flags;
				public UInt16   wReserved;
				public UInt16   XonLim;
				public UInt16   XoffLim;
				public Byte     ByteSize;
				public Byte     Parity;
				public Byte     StopBits;
				public Byte     XonChar;
				public Byte     XoffChar;
				public Byte     ErrorChar;
				public Byte     EofChar;
				public Byte     EvtChar;
				public UInt16   wReserved1;
			}

			#pragma warning restore 1591
		}
	
		#endregion

		#region Native > External Functions
		//------------------------------------------------------------------------------------------
		// Native > External Functions
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		private static class NativeMethods
		{
			private const string KERNEL_DLL = "kernel32.dll";

			/// <summary>
			/// Retrieves the current control settings for a specified communications device.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Function signature is given by the Win32 API.")]
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean GetCommState([In] SafeFileHandle hFile, [Out] out NativeTypes.DCB lpDCB);

			/// <summary>
			/// Configures a communications device according to the specifications in a device-control block (a <see cref="NativeTypes.DCB"/> structure).
			/// The function reinitializes all hardware and control settings, but it does not empty output or input queues.
			/// </summary>
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean SetCommState([In] SafeFileHandle hFile, [In] ref NativeTypes.DCB lpDCB);

			/// <summary>
			/// Retrieves information about a communications error and reports the current status of a communications device.
			/// The function is called when a communications error occurs, and it clears the device's error flag to enable additional input and output (I/O) operations.
			/// </summary>
			[DllImport(KERNEL_DLL, CharSet = CharSet.Auto, SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern Boolean ClearCommError([In] SafeFileHandle hFile, [Out] out UInt32 lpErrors, [Out] out NativeTypes.COMSTAT lpStat);
		}

		#endregion

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int CommStateRetries = 10;

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary></summary>
		[CLSCompliant(false)]
		public static void GetCommState(SafeFileHandle handle, out NativeTypes.DCB dcb)
		{
			dcb = new NativeTypes.DCB();

			for (int i = 0; i < CommStateRetries; i++)
			{
				uint errors;
				NativeTypes.COMSTAT stat;
				if (!NativeMethods.ClearCommError(handle, out errors, out stat))
					break;

				if (NativeMethods.GetCommState(handle, out dcb))
					return;
			}

			throw (WinError.LastErrorToIOException());
		}
 
		/// <summary></summary>
		[CLSCompliant(false)]
		public static void SetCommState(SafeFileHandle handle, ref NativeTypes.DCB lpDCB)
		{
			for (int i = 0; i < CommStateRetries; i++)
			{
				uint errors;
				NativeTypes.COMSTAT stat;
				if (!NativeMethods.ClearCommError(handle, out errors, out stat))
					break;

				if (NativeMethods.SetCommState(handle, ref lpDCB))
					return;
			}

			throw (WinError.LastErrorToIOException());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
