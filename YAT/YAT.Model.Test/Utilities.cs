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
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
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
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Collections.Generic;
using MKY.IO.Serial.SerialPort;
using MKY.IO.Serial.Usb;
using MKY.Net;

using NUnit.Framework;

using YAT.Settings.Terminal;

#endregion

namespace YAT.Model.Test
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public static class Utilities
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		/// <typeparam name="T">The (simple) settings type.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasizing the 'utility' nature of this delegate.")]
		[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Why not?")]
		public delegate TerminalSettingsRoot TerminalSettingsDelegate<T>(T arg);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasizing the 'utility' nature of this class.")]
		public static class TransmissionSettings
		{
			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
			public static IEnumerable<Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>> SerialPortLoopbackPairs
			{
				get
				{
					foreach (MKY.IO.Ports.Test.SerialPortPairConfigurationElement ce in MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackPairs)
					{
						TerminalSettingsDelegate<string> tsm = new TerminalSettingsDelegate<string>(GetStartedTextSerialPortSettings);
						Pair<TerminalSettingsDelegate<string>, string> pA = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.PortA);
						Pair<TerminalSettingsDelegate<string>, string> pB = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.PortB);
						string name = "SerialPortLoopbackPairs_" + ce.PortA + "_" + ce.PortB;
						string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackPairsAreAvailable };
						yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>(pA, pB, name, cats));
					}
				}
			}

			/// <summary></summary>
			[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
			[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
			public static IEnumerable<Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>> SerialPortLoopbackSelfs
			{
				get
				{
					foreach (MKY.IO.Ports.Test.SerialPortConfigurationElement ce in MKY.IO.Ports.Test.ConfigurationProvider.Configuration.LoopbackSelfs)
					{
						TerminalSettingsDelegate<string> tsm = new TerminalSettingsDelegate<string>(GetStartedTextSerialPortSettings);
						Pair<TerminalSettingsDelegate<string>, string> pA = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.Port);
						Pair<TerminalSettingsDelegate<string>, string> pB = new Pair<TerminalSettingsDelegate<string>, string>(null, null);
						string name = "SerialPortLoopbackSelf_" + ce.Port;
						string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackSelfsAreAvailable };
						yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>(pA, pB, name, cats));
					}
				}
			}

			/// <summary></summary>
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to verify.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to verify.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to verify.")]
			[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
			public static IEnumerable<Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>> IPLoopbacks
			{
				get
				{
					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings, null),
					              "IPv4Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings, null),
					              "IPv6Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv4SpecificInterfaceSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv4SpecificInterfaceSettings, null),
					              "IPv4SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv6SpecificInterfaceSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTextTcpAutoSocketOnIPv6SpecificInterfaceSettings, null),
					              "IPv6SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable }));
				}
			}
		}

		/// <remarks>
		/// \todo:
		/// This test set struct should be improved such that it can also handle expectations on the
		/// sender side (i.e. terminal A). Rationale: Testing of \!(Clear) behavior.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This struct really belongs to these test utilities only.")]
		public struct TestSet : IEquatable<TestSet>
		{
			private Types.Command command;
			private int   expectedLineCount;
			private int[] expectedElementCounts;
			private int[] expectedDataCounts;

			/// <remarks>Using 'A' instead of 'Tx' as some tests perform two-way-transmission.</remarks>
			private bool  expectedAlsoApplyToA;

			/// <summary></summary>
			/// <param name="command">The test command.</param>
			public TestSet(Types.Command command)
			{
				this.command = command;
				this.expectedLineCount = command.CommandLines.Length;

				this.expectedElementCounts = new int[this.expectedLineCount];
				this.expectedDataCounts    = new int[this.expectedLineCount];
				for (int i = 0; i < this.expectedLineCount; i++)
				{
					this.expectedElementCounts[i] = 3; // LineStart + 1 data element + LineBreak.
					this.expectedDataCounts[i]    = command.CommandLines[i].Length;
				}

				this.expectedAlsoApplyToA = true;
			}

			/// <summary></summary>
			/// <param name="command">The test command.</param>
			/// <param name="expectedLineCount">The expected number of lines as returned by <see cref="Terminal.RxLineCount"/> and <see cref="Terminal.TxLineCount"/>.</param>
			/// <param name="expectedElementCounts">The expected number of display elements per display line.</param>
			/// <param name="expectedDataCounts">The expected number of raw data bytes per display line.</param>
			/// <param name="expectedAlsoApplyToA">Flag indicating that expected values not only apply to B but also A.</param>
			public TestSet(Types.Command command, int expectedLineCount, int[] expectedElementCounts, int[] expectedDataCounts, bool expectedAlsoApplyToA)
			{
				this.command = command;
				this.expectedLineCount     = expectedLineCount;
				this.expectedElementCounts = expectedElementCounts;
				this.expectedDataCounts    = expectedDataCounts;
				this.expectedAlsoApplyToA  = expectedAlsoApplyToA;
			}

			/// <summary>The test command.</summary>
			public Types.Command Command
			{
				get { return (this.command); }
			}

			/// <summary>The expected number of lines as returned by <see cref="Terminal.RxLineCount"/> and <see cref="Terminal.TxLineCount"/>.</summary>
			public int ExpectedLineCount
			{
				get { return (this.expectedLineCount); }
			}

			/// <summary>The expected number of display elements per display line.</summary>
			[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
			public int[] ExpectedElementCounts
			{
				get { return (this.expectedElementCounts); }
			}

			/// <summary>The expected number of raw data bytes per display line.</summary>
			[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
			public int[] ExpectedDataCounts
			{
				get { return (this.expectedDataCounts); }
			}

			/// <summary>Flag indicating that expected values not only apply to B but also A.</summary>
			/// <remarks>Using 'A' and 'B' instead of 'Tx' and 'Rx' as some tests perform two-way-transmission.</remarks>
			public bool ExpectedAlsoApplyToA
			{
				get { return (this.expectedAlsoApplyToA); }
			}

			#region Object Members
			//======================================================================================
			// Object Members
			//======================================================================================

			/// <summary>
			/// Serves as a hash function for a particular type.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public override int GetHashCode()
			{
				unchecked
				{
					int hashCode = Command.GetHashCode();

					hashCode = (hashCode * 397) ^  ExpectedLineCount    .GetHashCode();
					hashCode = (hashCode * 397) ^  ExpectedElementCounts.GetHashCode();
					hashCode = (hashCode * 397) ^  ExpectedDataCounts   .GetHashCode();
					hashCode = (hashCode * 397) ^  ExpectedAlsoApplyToA .GetHashCode();

					return (hashCode);
				}
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			public override bool Equals(object obj)
			{
				if (obj is TestSet)
					return (Equals((TestSet)obj));
				else
					return (false);
			}

			/// <summary>
			/// Determines whether this instance and the specified object have value equality.
			/// </summary>
			/// <remarks>
			/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
			/// properties, i.e. properties with some logic, are also properly handled.
			/// </remarks>
			public bool Equals(TestSet other)
			{
				return
				(
					ObjectEx            .Equals(Command,               other.Command) &&
					ExpectedLineCount   .Equals(                       other.ExpectedLineCount) &&
					ArrayEx     .ElementsEqual( ExpectedElementCounts, other.ExpectedElementCounts) &&
					ArrayEx     .ElementsEqual( ExpectedDataCounts,    other.ExpectedDataCounts) &&
					ExpectedAlsoApplyToA.Equals(                       other.ExpectedAlsoApplyToA)
				);
			}

			/// <summary>
			/// Determines whether the two specified objects have reference or value equality.
			/// </summary>
			public static bool operator ==(TestSet lhs, TestSet rhs)
			{
				return (lhs.Equals(rhs));
			}

			/// <summary>
			/// Determines whether the two specified objects have reference and value inequality.
			/// </summary>
			public static bool operator !=(TestSet lhs, TestSet rhs)
			{
				return (!(lhs == rhs));
			}

			#endregion
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int WaitTimeoutForConnectionChange = Domain.Test.Utilities.WaitTimeoutForConnectionChange;
		private const int WaitTimeoutForLineTransmission = Domain.Test.Utilities.WaitTimeoutForLineTransmission;
		private const int WaitInterval                   = Domain.Test.Utilities.WaitInterval;

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		#region Settings > SerialPort
		//------------------------------------------------------------------------------------------
		// Settings > SerialPort
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedTextSerialPortSettings(string portId)
		{
			return (GetStartedTextSerialPortSettings((MKY.IO.Ports.SerialPortId)portId));
		}

		internal static TerminalSettingsRoot GetStartedTextSerialPortSettings(MKY.IO.Ports.SerialPortId portId)
		{
			TerminalSettingsRoot settings = new TerminalSettingsRoot();
			settings.TerminalType = Domain.TerminalType.Text;
			settings.Terminal.IO.IOType = Domain.IOType.SerialPort;
			settings.Terminal.IO.SerialPort.PortId = portId;
			settings.TerminalIsStarted = true;
			return (settings);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedTextSerialPortASettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortAIsAvailable)
				return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortA));

			Assert.Ignore("'PortA' is not available, therefore this test is excluded. Ensure that 'PortA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedTextSerialPortASettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextSerialPortASettings());
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedTextSerialPortBSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortBIsAvailable)
				return (GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortB));

			Assert.Ignore("'PortB' is not available, therefore this test is excluded. Ensure that 'PortB' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedTextSerialPortBSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextSerialPortBSettings());
		}

		internal static TerminalSettingsRoot GetStartedTextSerialPortMTSicsDeviceASettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
			{
				TerminalSettingsRoot settings = GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceA);

				// MT-SICS devices use XOn/XOff by default:
				settings.Terminal.IO.SerialPort.Communication.FlowControl = SerialFlowControl.Software;

				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextSerialPortMTSicsDeviceASettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextSerialPortMTSicsDeviceASettings());
		}

		internal static TerminalSettingsRoot GetStartedTextSerialPortMTSicsDeviceBSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected)
			{
				TerminalSettingsRoot settings = GetStartedTextSerialPortSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceB);

				// MT-SICS devices use XOn/XOff by default:
				settings.Terminal.IO.SerialPort.Communication.FlowControl = SerialFlowControl.Software;

				return (settings);
			}
		
			Assert.Ignore("'MTSicsDeviceB' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextSerialPortMTSicsDeviceBSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextSerialPortMTSicsDeviceBSettings());
		}

		#endregion

		#region Settings > Socket
		//------------------------------------------------------------------------------------------
		// Settings > Socket
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketSettings(string networkInterface)
		{
			return (GetStartedTextTcpAutoSocketSettings((IPNetworkInterfaceEx)networkInterface));
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketSettings(IPNetworkInterfaceEx networkInterface)
		{
			TerminalSettingsRoot settings = new TerminalSettingsRoot();
			settings.TerminalType = Domain.TerminalType.Text;
			settings.Terminal.IO.IOType = Domain.IOType.TcpAutoSocket;
			settings.Terminal.IO.Socket.LocalInterface = networkInterface;
			settings.TerminalIsStarted = true;
			return (settings);
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings()
		{
			return (GetStartedTextTcpAutoSocketSettings(IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings());
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings()
		{
			return (GetStartedTextTcpAutoSocketSettings(IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextTcpAutoSocketOnIPv6LoopbackSettings());
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv4SpecificInterfaceSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetStartedTextTcpAutoSocketSettings(MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv4SpecificInterfaceSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextTcpAutoSocketOnIPv4SpecificInterfaceSettings());
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv6SpecificInterfaceSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetStartedTextTcpAutoSocketSettings(MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketOnIPv6SpecificInterfaceSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextTcpAutoSocketOnIPv6SpecificInterfaceSettings());
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketMTSicsDeviceSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.MTSicsDeviceIsAvailable)
			{
				int port = MKY.Net.Test.ConfigurationProvider.Configuration.MTSicsDeviceTcpPortAsInt;

				TerminalSettingsRoot settings = GetStartedTextTcpAutoSocketOnIPv4LoopbackSettings();
				settings.IO.Socket.LocalTcpPort = port;
				settings.IO.Socket.RemoteTcpPort = port;
				return (settings);
			}

			Assert.Ignore("'MTSicsDevice' is not available, therefore this test is excluded. Ensure that 'MTSicsDevice' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextTcpAutoSocketMTSicsDeviceSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextTcpAutoSocketMTSicsDeviceSettings());
		}

		#endregion

		#region Settings > USB Ser/HID
		//------------------------------------------------------------------------------------------
		// Settings > USB Ser/HID
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedTextUsbSerialHidSettings(string deviceInfo)
		{
			return (GetStartedTextUsbSerialHidSettings((MKY.IO.Usb.DeviceInfo)deviceInfo));
		}

		internal static TerminalSettingsRoot GetStartedTextUsbSerialHidSettings(MKY.IO.Usb.DeviceInfo deviceInfo)
		{
			TerminalSettingsRoot settings = new TerminalSettingsRoot();
			settings.TerminalType = Domain.TerminalType.Text;
			settings.Terminal.IO.IOType = Domain.IOType.UsbSerialHid;
			settings.Terminal.IO.UsbSerialHidDevice.DeviceInfo = deviceInfo;
			settings.TerminalIsStarted = true;
			return (settings);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedTextUsbSerialHidDeviceASettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.DeviceAIsAvailable)
				return (GetStartedTextUsbSerialHidSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.DeviceA));

			Assert.Ignore("'DeviceA' is not available, therefore this test is excluded. Ensure that 'DeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedTextUsbSerialHidDeviceASettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextUsbSerialHidDeviceASettings());
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedTextUsbSerialHidDeviceBSettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.DeviceBIsAvailable)
				return (GetStartedTextUsbSerialHidSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.DeviceB));

			Assert.Ignore("'DeviceB' is not available, therefore this test is excluded. Ensure that 'DeviceB' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedTextUsbSerialHidDeviceBSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextUsbSerialHidDeviceBSettings());
		}

		internal static TerminalSettingsRoot GetStartedTextUsbSerialHidMTSicsDeviceASettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
			{
				TerminalSettingsRoot settings = GetStartedTextUsbSerialHidSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceA);

				// MT-SICS devices use XOn/XOff by default:
				settings.Terminal.IO.UsbSerialHidDevice.FlowControl = SerialHidFlowControl.Software;

				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextUsbSerialHidMTSicsDeviceASettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextUsbSerialHidMTSicsDeviceASettings());
		}

		internal static TerminalSettingsRoot GetStartedTextUsbSerialHidMTSicsDeviceBSettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected)
			{
				TerminalSettingsRoot settings = GetStartedTextUsbSerialHidSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceB);
	
				// MT-SICS devices use XOn/XOff by default:
				settings.Terminal.IO.UsbSerialHidDevice.FlowControl = SerialHidFlowControl.Software;

				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceB' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTextUsbSerialHidMTSicsDeviceBSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTextUsbSerialHidMTSicsDeviceBSettings());
		}

		#endregion

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForStart(Terminal terminal)
		{
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for start, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForConnectionChange + " ms...");

				if (waitTime >= WaitTimeoutForConnectionChange)
					Assert.Fail("Start timeout!");
			}
			while (!terminal.IsStarted);

			Console.Out.WriteLine("...done");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForOpen(Terminal terminal)
		{
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for open, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForConnectionChange + " ms...");

				if (waitTime >= WaitTimeoutForConnectionChange)
					Assert.Fail("Open timeout!");
			}
			while (!terminal.IsOpen);

			Console.Out.WriteLine("...done");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static void WaitForConnection(Terminal terminal)
		{
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForConnectionChange + " ms...");

				if (waitTime >= WaitTimeoutForConnectionChange)
					Assert.Fail("Connect timeout!");
			}
			while (!terminal.IsConnected);

			Console.Out.WriteLine("...done");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForConnection(Terminal terminalA, Terminal terminalB)
		{
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForConnectionChange + " ms...");

				if (waitTime >= WaitTimeoutForConnectionChange)
					Assert.Fail("Connect timeout!");
			}
			while (!terminalA.IsConnected && !terminalB.IsConnected);

			Console.Out.WriteLine("...done");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForClose(Terminal terminal)
		{
			int waitTime = 0;
			while (terminal.IsOpen)
			{
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for close, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForConnectionChange + " ms...");

				if (waitTime >= WaitTimeoutForConnectionChange)
					Assert.Fail("Close timeout!");
			}

			Console.Out.WriteLine("...done");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static void WaitForDisconnection(Terminal terminal)
		{
			int waitTime = 0;
			while (terminal.IsConnected)
			{
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for disconnection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForConnectionChange + " ms...");

				if (waitTime >= WaitTimeoutForConnectionChange)
					Assert.Fail("Disconnect timeout!");
			}

			Console.Out.WriteLine("...done");
		}

		internal static void WaitForTransmission(Terminal terminalTx, Terminal terminalRx, TestSet testSet)
		{
			WaitForTransmission(terminalTx, terminalRx, testSet.ExpectedLineCount, 1); // Single cycle.
		}

		internal static void WaitForTransmission(Terminal terminalTx, Terminal terminalRx, int expectedPerCycleLineCount)
		{
			WaitForTransmission(terminalTx, terminalRx, expectedPerCycleLineCount, 1); // Single cycle.
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForTransmission(Terminal terminalTx, Terminal terminalRx, int expectedPerCycleLineCount, int cycle)
		{
			// Calculate total expected line count at the receiver side:
			int expectedTotalLineCount = (expectedPerCycleLineCount * cycle);

			// Calculate timeout factor per line, taking cases with 0 lines into account:
			int timeoutFactorPerLine = ((expectedPerCycleLineCount > 0) ? expectedPerCycleLineCount : 1);

			// Calculate timeout:
			int timeout = (WaitTimeoutForLineTransmission * timeoutFactorPerLine);

			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for transmission, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout)
					Assert.Fail("Transmission timeout! Not enough lines received within expected interval.");

				if (terminalRx.RxLineCount > expectedTotalLineCount) // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
						" Number of received lines = " + terminalRx.RxLineCount +
						" mismatches expected = " + expectedTotalLineCount + ".");
			}
			while ((terminalRx.RxLineCount != expectedTotalLineCount) ||
			       (terminalRx.RxLineCount != terminalTx.TxLineCount) ||
			       (terminalRx.RxByteCount != terminalTx.TxByteCount));

			// Attention: Terminal line count is not always equal to display line count!
			//  > Terminal line count = number of *completed* lines in terminal
			//  > Display line count = number of lines in view
			// This function uses terminal line count for verification!

			Console.Out.WriteLine("...done");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForReceiving(Terminal terminalRx, int expectedTotalLineCount, int expectedTotalByteCount)
		{
			// Calculate timeout:
			int timeout = (WaitTimeoutForLineTransmission * expectedTotalLineCount);

			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitInterval);
				waitTime += WaitInterval;

				Console.Out.WriteLine("Waiting for transmission, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout)
					Assert.Fail("Transmission timeout! Not enough lines received within expected interval.");

				if ((terminalRx.RxLineCount > expectedTotalLineCount) ||
					(terminalRx.RxByteCount > expectedTotalByteCount)) // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
						" Number of received lines = " + terminalRx.RxLineCount + " / bytes = " + terminalRx.RxByteCount +
						" mismatches expected = " + expectedTotalLineCount + " / " + expectedTotalByteCount + ".");
			}
			while ((terminalRx.RxLineCount != expectedTotalLineCount) ||
			       (terminalRx.RxByteCount != expectedTotalByteCount));

			// Attention: Terminal line count is not always equal to display line count!
			//  > Terminal line count = number of *completed* lines in terminal
			//  > Display line count = number of lines in view
			// This function uses terminal line count for verification!

			Console.Out.WriteLine("...done");
		}

		#endregion

		#region Verifications
		//==========================================================================================
		// Verifications
		//==========================================================================================

		/// <remarks>Using 'A' and 'B' instead of 'Tx' and 'Rx' as some tests perform two-way-transmission.</remarks>
		internal static void VerifyLines(List<Domain.DisplayLine> displayLinesA, List<Domain.DisplayLine> displayLinesB, TestSet testSet)
		{
			VerifyLines(displayLinesA, displayLinesB, testSet, 1); // Single cycle.
		}

		/// <remarks>Using 'A' and 'B' instead of 'Tx' and 'Rx' as some tests perform two-way-transmission.</remarks>
		internal static void VerifyLines(List<Domain.DisplayLine> displayLinesA, List<Domain.DisplayLine> displayLinesB, TestSet testSet, int cycle)
		{
			// Attention: Display line count is not always equal to terminal line count!
			//  > Display line count = number of lines in view
			//  > Terminal line count = number of *completed* lines in terminal
			// This function uses display line count for verification!

			// Calculate total expected display line count at the receiver side:
			int expectedTotalDisplayLineCountB = 0;
			if (testSet.ExpectedElementCounts != null)
				expectedTotalDisplayLineCountB = (testSet.ExpectedElementCounts.Length * cycle);

			// Compare the expected line count at the receiver side:
			if (displayLinesB.Count != expectedTotalDisplayLineCountB)
			{
				StringBuilder sbB = new StringBuilder();
				foreach (Domain.DisplayLine displayLineB in displayLinesB)
					sbB.Append(ArrayEx.ElementsToString(displayLineB.ToArray()));

				Console.Error.Write
				(
					"B:" + Environment.NewLine + sbB + Environment.NewLine
				);

				Assert.Fail
				(
					"Line count mismatches: " + Environment.NewLine +
					"Expected = " + expectedTotalDisplayLineCountB + " line(s), " +
					"B = " + displayLinesB.Count + " line(s)." + Environment.NewLine +
					@"See ""Output"" for details."
				);
			}

			// If both sides are expected to show the same line count, compare the counts,
			// otherwise, ignore the comparision:
			if (testSet.ExpectedAlsoApplyToA)
			{
				if (displayLinesB.Count == displayLinesA.Count)
				{
					for (int i = 0; i < displayLinesA.Count; i++)
					{
						int index                = i % testSet.ExpectedElementCounts.Length;
						int expectedElementCount =     testSet.ExpectedElementCounts[index];
						int expectedDataCount    =     testSet.ExpectedDataCounts[index];

						Domain.DisplayLine displayLineA = displayLinesA[i];
						Domain.DisplayLine displayLineB = displayLinesB[i];

						if ((displayLineB.Count     == displayLineA.Count) &&
							(displayLineB.Count     == expectedElementCount) &&
							(displayLineB.DataCount == displayLineA.DataCount) &&
							(displayLineB.DataCount == expectedDataCount))
						{
							for (int j = 0; j < displayLineA.Count; j++)
								Assert.That(displayLineB[j].Text, Is.EqualTo(displayLineA[j].Text));
						}
						else
						{
							string strA = ArrayEx.ElementsToString(displayLineA.ToArray());
							string strB = ArrayEx.ElementsToString(displayLineB.ToArray());

							Console.Error.Write
							(
								"A:" + Environment.NewLine + strA + Environment.NewLine +
								"B:" + Environment.NewLine + strB + Environment.NewLine
							);

							Assert.Fail
							(
								"Length of line " + i + " mismatches:" + Environment.NewLine +
								"Expected = " + expectedElementCount + " element(s), " +
								"A = " + displayLineA.Count + " element(s), " +
								"B = " + displayLineB.Count + " element(s)," + Environment.NewLine +
								"Expected = " + expectedDataCount + " data, " +
								"A = " + displayLineA.DataCount + " data, " +
								"B = " + displayLineB.DataCount + " data." + Environment.NewLine +
								@"See ""Output"" for details."
							);
						}
					}
				}
				else
				{
					StringBuilder sbA = new StringBuilder();
					foreach (Domain.DisplayLine displayLineA in displayLinesA)
						sbA.Append(ArrayEx.ElementsToString(displayLineA.ToArray()));

					StringBuilder sbB = new StringBuilder();
					foreach (Domain.DisplayLine displayLineB in displayLinesB)
						sbB.Append(ArrayEx.ElementsToString(displayLineB.ToArray()));

					Console.Error.Write
					(
						"A:" + Environment.NewLine + sbA + Environment.NewLine +
						"B:" + Environment.NewLine + sbB + Environment.NewLine
					);

					Assert.Fail
					(
						"Line count mismatches: " + Environment.NewLine +
						"Expected = " + expectedTotalDisplayLineCountB + " line(s), " +
						"A = " + displayLinesA.Count + " line(s), " +
						"B = " + displayLinesB.Count + " line(s)." + Environment.NewLine +
						@"See ""Output"" for details."
					);
				}
			}
		}

		#endregion

		#region Helpers
		//==========================================================================================
		// Helpers
		//==========================================================================================

		private static bool staticTerminalMessageInputRequestResultsInExclude = false;
		private static string staticTerminalMessageInputRequestResultsInExcludeText = "";

		/// <summary></summary>
		public static bool TerminalMessageInputRequestResultsInExclude
		{
			get { return (staticTerminalMessageInputRequestResultsInExclude); }
		}

		/// <summary></summary>
		public static string TerminalMessageInputRequestResultsInExcludeText
		{
			get { return (staticTerminalMessageInputRequestResultsInExcludeText); }
		}

		/// <summary></summary>
		public static void TerminalMessageInputRequest(object sender, MessageInputEventArgs e)
		{
			// No assertion = exception can be invoked here as it might be handled by the calling event handler.
			// Therefore, simply confirm...
			e.Result = DialogResult.OK;

			// ...and signal exclusion via a flag:
			if (e.Text.StartsWith("Unable to start terminal", StringComparison.Ordinal)) // 'Ordinal' since YAT is all-English.
			{
				staticTerminalMessageInputRequestResultsInExclude = true;
				staticTerminalMessageInputRequestResultsInExcludeText = e.Text;
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
