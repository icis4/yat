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
// YAT 2.0 Epsilon Version 1.99.90
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2018 Matthias Kläy.
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
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize the 'utility' nature of this delegate.")]
		[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Why not?")]
		public delegate TerminalSettingsRoot TerminalSettingsDelegate<T>(T arg);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize the 'utility' nature of this class.")]
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
						var tsm = new TerminalSettingsDelegate<string>(GetStartedSerialPortTextSettings);
						var pA = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.PortA);
						var pB = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.PortB);
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
						var tsm = new TerminalSettingsDelegate<string>(GetStartedSerialPortTextSettings);
						var pA = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.Port);
						var pB = new Pair<TerminalSettingsDelegate<string>, string>(null, null);
						string name = "SerialPortLoopbackSelf_" + ce.Port;
						string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackSelfsAreAvailable };
						yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>(pA, pB, name, cats));
					}
				}
			}

			/// <summary>
			/// Returns settings tuples for TCP/IP and UDP/IP Client/Server as well as AutoSocket.
			/// </summary>
			/// <remarks>
			/// TCP/IP combinations Server/AutoSocket and AutoSocket/Client are skipped as they don't really offer additional test coverage.
			/// UPD/IP PairSocket is also skipped as that would require additional settings with different ports, and they are tested further below anyway.
			/// </remarks>
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
			[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
			public static IEnumerable<Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>> IPLoopbackPairs
			{
				get
				{
					// TCP/IP Client/Server

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpClientOnIPv4LoopbackTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpServerOnIPv4LoopbackTextSettings, null),
					              "TcpClientServer_IPv4Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpClientOnIPv6LoopbackTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpServerOnIPv6LoopbackTextSettings, null),
					              "TcpClientServer_IPv6Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpClientOnIPv4SpecificInterfaceTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpServerOnIPv4SpecificInterfaceTextSettings, null),
					              "TcpClientServer_IPv4SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpClientOnIPv6SpecificInterfaceTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpServerOnIPv6SpecificInterfaceTextSettings, null),
					              "TcpClientServer_IPv6SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable }));

					// TCP/IP AutoSocket

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings, null),
					              "TcpAutoSocket_IPv4Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpAutoSocketOnIPv6LoopbackTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpAutoSocketOnIPv6LoopbackTextSettings, null),
					              "TcpAutoSocket_IPv6Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpAutoSocketOnIPv4SpecificInterfaceTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpAutoSocketOnIPv4SpecificInterfaceTextSettings, null),
					              "TcpAutoSocket_IPv4SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpAutoSocketOnIPv6SpecificInterfaceTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpAutoSocketOnIPv6SpecificInterfaceTextSettings, null),
					              "TcpAutoSocket_IPv6SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable }));

					// UDP/IP Client/Server

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpClientOnIPv4LoopbackTextSettings, null), // Client must be the first as that is used for sending first.
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpServerOnIPv4LoopbackTextSettings, null),
					              "UdpClientServer_IPv4Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpClientOnIPv6LoopbackTextSettings, null), // Client must be the first as that is used for sending first.
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpServerOnIPv6LoopbackTextSettings, null),
					              "UdpClientServer_IPv6Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpClientOnIPv4SpecificInterfaceTextSettings, null), // Client must be the first as that is used for sending first.
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpServerOnIPv4SpecificInterfaceTextSettings, null),
					              "UdpClientServer_IPv4SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpClientOnIPv6SpecificInterfaceTextSettings, null), // Client must be the first as that is used for sending first.
					              new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpServerOnIPv6SpecificInterfaceTextSettings, null),
					              "UdpClientServer_IPv6SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable }));
				}
			}

			/// <summary></summary>
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Too many values to verify.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "Too many values to verify.")]
			[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
			[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Selfs", Justification = "Multiple items, same as 'Pairs'.")]
			public static IEnumerable<Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>> IPLoopbackSelfs
			{
				get
				{
					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpPairSocketOnIPv4LoopbackTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(null, null),
					              "UdpPairSocket_IPv4Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpPairSocketOnIPv6LoopbackTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(null, null),
					              "UdpPairSocket_IPv6Loopback", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6LoopbackIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpPairSocketOnIPv4SpecificInterfaceTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(null, null),
					              "UdpPairSocket_IPv4SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv4SpecificInterfaceIsAvailable }));

					yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>
					             (new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUdpPairSocketOnIPv6SpecificInterfaceTextSettings, null),
					              new Pair<TerminalSettingsDelegate<string>, string>(null, null),
					              "UdpPairSocket_IPv6SpecificInterface", new string[] { MKY.Net.Test.ConfigurationCategoryStrings.IPv6SpecificInterfaceIsAvailable }));
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
			/// <summary>The test command.</summary>
			public Types.Command Command { get; }

			/// <summary>The expected number of lines as returned by <see cref="Terminal.RxLineCount"/> and <see cref="Terminal.TxLineCount"/>.</summary>
			public int ExpectedLineCount { get; }

			/// <summary>The expected number of display elements per display line.</summary>
			[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
			public int[] ExpectedElementCounts { get; }

			/// <summary>The expected number of raw byte content per display line, including potentially hidden EOL or control bytes.</summary>
			[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
			public int[] ExpectedByteCounts { get; }

			/// <summary>Flag indicating that expected values not only apply to B but also A.</summary>
			/// <remarks>Using 'A' and 'B' instead of 'Tx' and 'Rx' as some tests perform two-way-transmission.</remarks>
			public bool ExpectedAlsoApplyToA { get; }

			/// <summary>Flag indicating that cleared terminals are expected in the end.</summary>
			public bool ClearedIsExpectedInTheEnd { get; }

			/// <summary></summary>
			/// <param name="command">The test command.</param>
			public TestSet(Types.Command command)
			{
				Command = command;

				ExpectedLineCount = command.TextLines.Length;

				ExpectedElementCounts = new int[ExpectedLineCount];
				ExpectedByteCounts    = new int[ExpectedLineCount];
				for (int i = 0; i < ExpectedLineCount; i++)
				{
					ExpectedElementCounts[i] = 4; // LineStart + Data + EOL + LineBreak.
					ExpectedByteCounts[i]    = command.TextLines[i].Length + 2; // Content + EOL.
				}

				ExpectedAlsoApplyToA = true;
				ClearedIsExpectedInTheEnd = false;
			}

			/// <summary></summary>
			/// <param name="command">The test command.</param>
			/// <param name="expectedLineCount">The expected number of lines as returned by <see cref="Terminal.RxLineCount"/> and <see cref="Terminal.TxLineCount"/>.</param>
			/// <param name="expectedElementCounts">The expected number of display elements per display line.</param>
			/// <param name="expectedByteCounts">The expected number of raw byte content per display line, including potentially hidden EOL or control bytes.</param>
			/// <param name="expectedAlsoApplyToA">Flag indicating that expected values not only apply to B but also A.</param>
			/// <param name="clearedIsExpectedInTheEnd">Flag indicating that cleared terminals are expected in the end.</param>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public TestSet(Types.Command command, int expectedLineCount, int[] expectedElementCounts, int[] expectedByteCounts, bool expectedAlsoApplyToA, bool clearedIsExpectedInTheEnd = false)
			{
				Command = command;

				ExpectedLineCount         = expectedLineCount;
				ExpectedElementCounts     = expectedElementCounts;
				ExpectedByteCounts        = expectedByteCounts;
				ExpectedAlsoApplyToA      = expectedAlsoApplyToA;
				ClearedIsExpectedInTheEnd = clearedIsExpectedInTheEnd;
			}

			/// <summary>The expected number of display elements in total.</summary>
			public int ExpectedTotalElementCount
			{
				get
				{
					int totalCount = 0;
					foreach (int count in ExpectedElementCounts)
						totalCount += count;

					return (totalCount);
				}
			}

			/// <summary>The expected number of raw byte content in total.</summary>
			public int ExpectedTotalByteCount
			{
				get
				{
					int totalCount = 0;
					foreach (int count in ExpectedByteCounts)
						totalCount += count;

					return (totalCount);
				}
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
					int hashCode = (Command != null ? Command.GetHashCode() : 0);

					hashCode = (hashCode * 397) ^  ExpectedLineCount                                    .GetHashCode();
					hashCode = (hashCode * 397) ^ (ExpectedElementCounts != null ? ExpectedElementCounts.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (ExpectedByteCounts    != null ? ExpectedByteCounts   .GetHashCode() : 0);
					hashCode = (hashCode * 397) ^  ExpectedAlsoApplyToA                                 .GetHashCode();

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
					ArrayEx     .ElementsEqual( ExpectedByteCounts,    other.ExpectedByteCounts) &&
					ExpectedAlsoApplyToA.Equals(                       other.ExpectedAlsoApplyToA)
				);
			}

			/// <summary>
			/// Determines whether the two specified objects have value equality.
			/// </summary>
			public static bool operator ==(TestSet lhs, TestSet rhs)
			{
				return (lhs.Equals(rhs));
			}

			/// <summary>
			/// Determines whether the two specified objects have value inequality.
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

		private const int WaitTimeoutForStateChange  = Domain.Test.Utilities.WaitTimeoutForStateChange;
		private const int WaitIntervalForStateChange = Domain.Test.Utilities.WaitIntervalForStateChange;

		private const int WaitTimeoutForLineTransmission  = Domain.Test.Utilities.WaitTimeoutForLineTransmission;
		private const int WaitIntervalForLineTransmission = Domain.Test.Utilities.WaitIntervalForLineTransmission;

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		internal static TerminalSettingsRoot GetTextSettings()
		{
			var settings = new TerminalSettingsRoot();
			settings.TerminalType = Domain.TerminalType.Text;
			settings.Terminal.TextTerminal.ShowEol = true; // Required for easier test verification (byte count).
			return (settings);
		}

		#region Settings > SerialPort
		//------------------------------------------------------------------------------------------
		// Settings > SerialPort
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedSerialPortTextSettings(MKY.IO.Ports.SerialPortId portId)
		{
			var settings = GetTextSettings();
			settings.Terminal.IO.IOType = Domain.IOType.SerialPort;
			settings.Terminal.IO.SerialPort.PortId = portId;
			settings.TerminalIsStarted = true;
			return (settings);
		}

		internal static TerminalSettingsRoot GetStartedSerialPortTextSettings(string portId)
		{
			return (GetStartedSerialPortTextSettings((MKY.IO.Ports.SerialPortId)portId));
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedSerialPortATextSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortAIsAvailable)
				return (GetStartedSerialPortTextSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortA));

			Assert.Ignore("'PortA' is not available, therefore this test is excluded. Ensure that 'PortA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedSerialPortATextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedSerialPortATextSettings());
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedSerialPortBTextSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortBIsAvailable)
				return (GetStartedSerialPortTextSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.PortB));

			Assert.Ignore("'PortB' is not available, therefore this test is excluded. Ensure that 'PortB' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedSerialPortBTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedSerialPortBTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedSerialPortMTSicsDeviceATextSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
			{
				var settings = GetStartedSerialPortTextSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceA);
				ConfigureMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedSerialPortMTSicsDeviceATextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedSerialPortMTSicsDeviceATextSettings());
		}

		internal static TerminalSettingsRoot GetStartedSerialPortMTSicsDeviceBTextSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected)
			{
				var settings = GetStartedSerialPortTextSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceB);
				ConfigureMTSicsSettings(settings);
				return (settings);
			}
		
			Assert.Ignore("'MTSicsDeviceB' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedSerialPortMTSicsDeviceBTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedSerialPortMTSicsDeviceBTextSettings());
		}

		#endregion

		#region Settings > Socket
		//------------------------------------------------------------------------------------------
		// Settings > Socket
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedTextSettings(Domain.IOType type, string networkInterface)
		{
			return (GetStartedTextSettings(type, (IPNetworkInterfaceEx)networkInterface));
		}

		internal static TerminalSettingsRoot GetStartedTextSettings(Domain.IOType type, IPNetworkInterfaceEx networkInterface)
		{
			var settings = GetTextSettings();
			settings.Terminal.IO.IOType = type;
			settings.Terminal.IO.Socket.LocalInterface = networkInterface;
			settings.TerminalIsStarted = true;
			return (settings);
		}

		#region Settings > Socket > TCP/IP Client
		//------------------------------------------------------------------------------------------
		// Settings > Socket > TCP/IP Client
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpClient, IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpClientOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpClient, IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpClientOnIPv6LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv4SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.TcpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv4SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpClientOnIPv4SpecificInterfaceTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv6SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.TcpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv6SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpClientOnIPv6SpecificInterfaceTextSettings());
		}

		#endregion

		#region Settings > Socket > TCP/IP Server
		//------------------------------------------------------------------------------------------
		// Settings > Socket > TCP/IP Server
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpServer, IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpServerOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpServer, IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpServerOnIPv6LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv4SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.TcpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv4SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpServerOnIPv4SpecificInterfaceTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv6SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.TcpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv6SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpServerOnIPv6SpecificInterfaceTextSettings());
		}

		#endregion

		#region Settings > Socket > TCP/IP AutoSocket
		//------------------------------------------------------------------------------------------
		// Settings > Socket > TCP/IP AutoSocket
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpAutoSocket, IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpAutoSocket, IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpAutoSocketOnIPv6LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv4SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.TcpAutoSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv4SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpAutoSocketOnIPv4SpecificInterfaceTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv6SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.TcpAutoSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv6SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpAutoSocketOnIPv6SpecificInterfaceTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketMTSicsDeviceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.MTSicsDeviceIsAvailable)
			{
				int port = MKY.Net.Test.ConfigurationProvider.Configuration.MTSicsDeviceTcpPortAsInt;

				var settings = GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings();
				settings.IO.Socket.LocalTcpPort = port;
				settings.IO.Socket.RemoteTcpPort = port;
				return (settings);
			}

			Assert.Ignore("'MTSicsDevice' is not available, therefore this test is excluded. Ensure that 'MTSicsDevice' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketMTSicsDeviceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedTcpAutoSocketMTSicsDeviceTextSettings());
		}

		#endregion

		#region Settings > Socket > UDP/IP Client
		//------------------------------------------------------------------------------------------
		// Settings > Socket > UDP/IP Client
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpClient, IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpClientOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpClient, IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpClientOnIPv6LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv4SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.UdpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv4SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpClientOnIPv4SpecificInterfaceTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv6SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.UdpClient, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv6SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpClientOnIPv6SpecificInterfaceTextSettings());
		}

		#endregion

		#region Settings > Socket > UDP/IP Server
		//------------------------------------------------------------------------------------------
		// Settings > Socket > UDP/IP Server
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpServer, IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpServerOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpServer, IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpServerOnIPv6LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv4SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.UdpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv4SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpServerOnIPv4SpecificInterfaceTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv6SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.UdpServer, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv6SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpServerOnIPv6SpecificInterfaceTextSettings());
		}

		#endregion

		#region Settings > Socket > UDP/IP PairSocket
		//------------------------------------------------------------------------------------------
		// Settings > Socket > UDP/IP PairSocket
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpPairSocket, IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpPairSocketOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpPairSocket, IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpPairSocketOnIPv6LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv4SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.UdpPairSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv4SpecificInterface));

			Assert.Ignore("'IPv4SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv4SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv4SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpPairSocketOnIPv4SpecificInterfaceTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv6SpecificInterfaceTextSettings()
		{
			if (MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterfaceIsAvailable)
				return (GetStartedTextSettings(Domain.IOType.UdpPairSocket, MKY.Net.Test.ConfigurationProvider.Configuration.IPv6SpecificInterface));

			Assert.Ignore("'IPv6SpecificInterface' is not available, therefore this test is excluded. Ensure that 'IPv6SpecificInterface' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv6SpecificInterfaceTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUdpPairSocketOnIPv6SpecificInterfaceTextSettings());
		}

		#endregion

		#endregion

		#region Settings > USB Ser/HID
		//------------------------------------------------------------------------------------------
		// Settings > USB Ser/HID
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedUsbSerialHidTextSettings(string deviceInfo)
		{
			return (GetStartedUsbSerialHidTextSettings((MKY.IO.Usb.DeviceInfo)deviceInfo));
		}

		internal static TerminalSettingsRoot GetStartedUsbSerialHidTextSettings(MKY.IO.Usb.DeviceInfo deviceInfo)
		{
			var settings = GetTextSettings();
			settings.Terminal.IO.IOType = Domain.IOType.UsbSerialHid;
			settings.Terminal.IO.UsbSerialHidDevice.DeviceInfo = deviceInfo;
			settings.TerminalIsStarted = true;
			return (settings);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedUsbSerialHidDeviceATextSettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.DeviceAIsAvailable)
				return (GetStartedUsbSerialHidTextSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.DeviceA));

			Assert.Ignore("'DeviceA' is not available, therefore this test is excluded. Ensure that 'DeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedUsbSerialHidDeviceATextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUsbSerialHidDeviceATextSettings());
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedUsbSerialHidDeviceBTextSettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.DeviceBIsAvailable)
				return (GetStartedUsbSerialHidTextSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.DeviceB));

			Assert.Ignore("'DeviceB' is not available, therefore this test is excluded. Ensure that 'DeviceB' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Prepared for future use.")]
		internal static TerminalSettingsRoot GetStartedUsbSerialHidDeviceBTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUsbSerialHidDeviceBTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUsbSerialHidMTSicsDeviceATextSettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsConnected)
			{
				var settings = GetStartedUsbSerialHidTextSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceA);
				ConfigureMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceA' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedUsbSerialHidMTSicsDeviceATextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUsbSerialHidMTSicsDeviceATextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUsbSerialHidMTSicsDeviceBTextSettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsConnected)
			{
				var settings = GetStartedUsbSerialHidTextSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceB);
				ConfigureMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceB' is not connected, therefore this test is excluded. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
			//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedUsbSerialHidMTSicsDeviceBTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy); // Dummy required to provide signature of common type TerminalSettingsDelegate<string>.
			return (GetStartedUsbSerialHidMTSicsDeviceBTextSettings());
		}

		#endregion

		#region Settings > MT-SICS
		//------------------------------------------------------------------------------------------
		// Settings > MT-SICS
		//------------------------------------------------------------------------------------------

		private static void ConfigureMTSicsSettings(TerminalSettingsRoot settings)
		{
			// MT-SICS devices use XOn/XOff by default:
			settings.Terminal.IO.SerialPort.Communication.FlowControl = SerialFlowControl.Software;
			settings.Terminal.IO.UsbSerialHidDevice.FlowControl = SerialHidFlowControl.Software;

			// Set required USB Ser/HID format:
			var deviceFormat = (MKY.IO.Usb.SerialHidReportFormatPresetEx)MKY.IO.Usb.SerialHidReportFormatPreset.MT_SerHid;
			settings.Terminal.IO.UsbSerialHidDevice.ReportFormat = deviceFormat.ToReportFormat();
			settings.Terminal.IO.UsbSerialHidDevice.RxFilterUsage = deviceFormat.ToRxFilterUsage();
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
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Console.Out.WriteLine("Waiting for start, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Start timeout!");
				}
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
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Console.Out.WriteLine("Waiting for open, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Open timeout!");
				}
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
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Console.Out.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Connect timeout!");
				}
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
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Console.Out.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Connect timeout!");
				}
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
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Console.Out.WriteLine("Waiting for close, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Close timeout!");
				}
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
				Thread.Sleep(WaitIntervalForStateChange);
				waitTime += WaitIntervalForStateChange;

				Console.Out.WriteLine("Waiting for disconnection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Disconnect timeout!");
				}
			}

			Console.Out.WriteLine("...done");
		}

		internal static void WaitForTransmission(Terminal terminalTx, Terminal terminalRx, TestSet testSet)
		{
			WaitForTransmission(terminalTx, terminalRx, testSet.ExpectedTotalByteCount, testSet.ExpectedLineCount, 1); // Single cycle.
		}

		internal static void WaitForTransmission(Terminal terminalTx, Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			WaitForTransmission(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, 1); // Single cycle.
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForTransmission(Terminal terminalTx, Terminal terminalRx, int expectedPerCycleByteCount, int expectedPerCycleLineCount, int cycle)
		{
			// Calculate total expected counts at the receiver side:
			int expectedTotalByteCount = (expectedPerCycleByteCount * cycle);
			int expectedTotalLineCount = (expectedPerCycleLineCount * cycle);

			// Calculate timeout factor per line, taking cases with 0 lines into account:
			int timeoutFactorPerLine = ((expectedPerCycleLineCount > 0) ? (expectedPerCycleLineCount) : (1));

			// Calculate timeout:
			int timeout = (WaitTimeoutForLineTransmission * timeoutFactorPerLine);

			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForLineTransmission);
				waitTime += WaitIntervalForLineTransmission;

				Console.Out.WriteLine("Waiting for transmission, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout) {
					Assert.Fail("Transmission timeout! Not enough data received within expected interval.");
				}

				if (terminalTx.TxByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of sent bytes = " + terminalTx.TxByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				if (terminalTx.TxLineCount > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of sent lines = " + terminalTx.TxLineCount +
					            " mismatches expected = " + expectedTotalLineCount + ".");
				}

				if (terminalRx.RxByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of received bytes = " + terminalRx.RxByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				if (terminalRx.RxLineCount > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of received lines = " + terminalRx.RxLineCount +
					            " mismatches expected = " + expectedTotalLineCount + ".");
				}
			}
			while ((terminalRx.RxByteCount != expectedTotalByteCount) || (terminalRx.RxLineCount != expectedTotalLineCount));

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
		internal static void WaitForReceiving(Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			// Calculate timeout:
			int timeout = (WaitTimeoutForLineTransmission * expectedTotalLineCount);

			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForLineTransmission);
				waitTime += WaitIntervalForLineTransmission;

				Console.Out.WriteLine("Waiting for transmission, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				if (waitTime >= timeout) {
					Assert.Fail("Transmission timeout! Not enough data received within expected interval.");
				}

				if (terminalRx.RxByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of received bytes = " + terminalRx.RxByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				if (terminalRx.RxLineCount > expectedTotalLineCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Transmission error!" +
					            " Number of received lines = " + terminalRx.RxLineCount +
					            " mismatches expected = " + expectedTotalLineCount + ".");
				}
			}
			while ((terminalRx.RxByteCount != expectedTotalByteCount) || (terminalRx.RxLineCount != expectedTotalLineCount));

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
				var sbB = new StringBuilder();
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
			if (testSet.ExpectedAlsoApplyToA && !testSet.ClearedIsExpectedInTheEnd)
			{
				if (displayLinesB.Count == displayLinesA.Count)
				{
					for (int i = 0; i < displayLinesA.Count; i++)
					{
						int index                = i % testSet.ExpectedElementCounts.Length;
						int expectedElementCount =     testSet.ExpectedElementCounts[index];
						int expectedByteCount    =     testSet.ExpectedByteCounts[index];

						var displayLineA = displayLinesA[i];
						var displayLineB = displayLinesB[i];

						if ((displayLineB.Count     == displayLineA.Count) &&
							(displayLineB.Count     == expectedElementCount) &&
							(displayLineB.ByteCount == displayLineA.ByteCount) &&
							(displayLineB.ByteCount == expectedByteCount))
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
								"Expected = " + expectedByteCount + " byte(s), " +
								"A = " + displayLineA.ByteCount + " byte(s), " +
								"B = " + displayLineB.ByteCount + " byte(s)." + Environment.NewLine +
								@"See ""Output"" for details."
							);
						}
					}
				}
				else
				{
					var sbA = new StringBuilder();
					foreach (Domain.DisplayLine displayLineA in displayLinesA)
						sbA.Append(ArrayEx.ElementsToString(displayLineA.ToArray()));

					var sbB = new StringBuilder();
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