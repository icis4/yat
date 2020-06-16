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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2020 Matthias Kläy.
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
using System.Diagnostics;
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

using YAT.Settings.Model;

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
						var portA = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.PortA);
						var portB = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.PortB);
						string name = "SerialPortLoopbackPairs_" + ce.PortA + "_" + ce.PortB;
						string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackPairsAreAvailable };
						yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>(portA, portB, name, cats));
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
						var portA = new Pair<TerminalSettingsDelegate<string>, string>(tsm, ce.Port);
						var portB = new Pair<TerminalSettingsDelegate<string>, string>(null, null);
						string name = "SerialPortLoopbackSelf_" + ce.Port;
						string[] cats = { MKY.IO.Ports.Test.ConfigurationCategoryStrings.LoopbackSelfsAreAvailable };
						yield return (new Quadruple<Pair<TerminalSettingsDelegate<string>, string>, Pair<TerminalSettingsDelegate<string>, string>, string, string[]>(portA, portB, name, cats));
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
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
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
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Too many values to verify.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Too many values to verify.")]
			[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "Too many values to verify.")]
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

			/// <summary>The expected number of completed lines as returned by <see cref="Terminal.RxLineCount"/> and <see cref="Terminal.TxLineCount"/>.</summary>
			public int ExpectedLineCountCompleted { get; }

			/// <summary>The expected number of display elements per display line, including incomplete lines.</summary>
			[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
			public int[] ExpectedElementCounts { get; }

			/// <summary>The expected number of shown characters per display line, ASCII mnemonics (e.g. &lt;CR&gt;) are considered a single shown character.</summary>
			[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward test implementation.")]
			public int[] ExpectedCharCounts { get; }

			/// <summary>The expected number of raw byte content per display line, without hidden EOL or control bytes.</summary>
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

				ExpectedLineCountCompleted = command.TextLines.Length;

				ExpectedElementCounts = new int[ExpectedLineCountCompleted];
				ExpectedCharCounts    = new int[ExpectedLineCountCompleted];
				ExpectedByteCounts    = new int[ExpectedLineCountCompleted];
				for (int i = 0; i < ExpectedLineCountCompleted; i++)
				{
					ExpectedElementCounts[i] = 4; // LineStart + Data + EOL + LineBreak.
					ExpectedCharCounts[i]    = command.TextLines[i].Length + 2; // Content + EOL.
					ExpectedByteCounts[i]    = command.TextLines[i].Length + 2; // Content + EOL.
				}

				ExpectedAlsoApplyToA = true;
				ClearedIsExpectedInTheEnd = false;
			}

			/// <summary></summary>
			/// <param name="command">The test command.</param>
			/// <param name="expectedLineCount">The expected number of completed lines as returned by <see cref="Terminal.RxLineCount"/> and <see cref="Terminal.TxLineCount"/>.</param>
			/// <param name="expectedElementCounts">The expected number of display elements per display line, including incomplete lines.</param>
			/// <param name="expectedCharAndByteCounts">
			/// The expected number of shown characters per display line, ASCII mnemonics (e.g. &lt;CR&gt;) are considered a single shown character,
			/// which equals the expected number of raw byte content per display line, without hidden EOL or control bytes.
			/// </param>
			/// <param name="expectedAlsoApplyToA">Flag indicating that expected values not only apply to B but also A.</param>
			/// <param name="clearedIsExpectedInTheEnd">Flag indicating that cleared terminals are expected in the end.</param>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public TestSet(Types.Command command, int expectedLineCount, int[] expectedElementCounts, int[] expectedCharAndByteCounts, bool expectedAlsoApplyToA, bool clearedIsExpectedInTheEnd = false)
				: this(command, expectedLineCount, expectedElementCounts, expectedCharAndByteCounts, expectedCharAndByteCounts, expectedAlsoApplyToA, clearedIsExpectedInTheEnd)
			{
			}

			/// <summary></summary>
			/// <param name="command">The test command.</param>
			/// <param name="expectedLineCount">The expected number of completed lines as returned by <see cref="Terminal.RxLineCount"/> and <see cref="Terminal.TxLineCount"/>.</param>
			/// <param name="expectedElementCounts">The expected number of display elements per display line, including incomplete lines.</param>
			/// <param name="expectedCharCounts">The expected number of shown characters per display line, ASCII mnemonics (e.g. &lt;CR&gt;) are considered a single shown character.</param>
			/// <param name="expectedByteCounts">The expected number of raw byte content per display line, without hidden EOL or control bytes.</param>
			/// <param name="expectedAlsoApplyToA">Flag indicating that expected values not only apply to B but also A.</param>
			/// <param name="clearedIsExpectedInTheEnd">Flag indicating that cleared terminals are expected in the end.</param>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public TestSet(Types.Command command, int expectedLineCount, int[] expectedElementCounts, int[] expectedCharCounts, int[] expectedByteCounts, bool expectedAlsoApplyToA, bool clearedIsExpectedInTheEnd = false)
			{
				Command = command;

				ExpectedLineCountCompleted = expectedLineCount;
				ExpectedElementCounts      = expectedElementCounts;
				ExpectedCharCounts         = expectedCharCounts;
				ExpectedByteCounts         = expectedByteCounts;
				ExpectedAlsoApplyToA       = expectedAlsoApplyToA;
				ClearedIsExpectedInTheEnd  = clearedIsExpectedInTheEnd;
			}

			/// <summary>The expected number of lines in the display, including incomplete lines.</summary>
			public int ExpectedLineCountDisplayed
			{
				get
				{
					return (ExpectedElementCounts.Length);
				}
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

			/// <summary>The expected number of shown characters in total.</summary>
			public int ExpectedTotalCharCount
			{
				get
				{
					int totalCount = 0;
					foreach (int count in ExpectedCharCounts)
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

					hashCode = (hashCode * 397) ^  ExpectedLineCountCompleted                           .GetHashCode();
					hashCode = (hashCode * 397) ^ (ExpectedElementCounts != null ? ExpectedElementCounts.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (ExpectedCharCounts    != null ? ExpectedCharCounts   .GetHashCode() : 0);
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
					ObjectEx                  .Equals(Command,               other.Command) &&
					ExpectedLineCountCompleted.Equals(                       other.ExpectedLineCountCompleted) &&
					ArrayEx             .ValuesEqual( ExpectedElementCounts, other.ExpectedElementCounts) &&
					ArrayEx             .ValuesEqual( ExpectedCharCounts,    other.ExpectedCharCounts) &&
					ArrayEx             .ValuesEqual( ExpectedByteCounts,    other.ExpectedByteCounts) &&
					ExpectedAlsoApplyToA      .Equals(                       other.ExpectedAlsoApplyToA)
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

		                      /// <remarks><see cref="Domain.Test.Utilities.WaitTimeoutForStateChange"/>.</remarks>
		public const int WaitTimeoutForStateChange  = Domain.Test.Utilities.WaitTimeoutForStateChange;

		                      /// <remarks><see cref="Domain.Test.Utilities.WaitIntervalForStateChange"/>.</remarks>
		public const int WaitIntervalForStateChange = Domain.Test.Utilities.WaitIntervalForStateChange;

		                           /// <remarks><see cref="Domain.Test.Utilities.WaitTimeoutForLineTransmission"/>.</remarks>
		public const int WaitTimeoutForLineTransmission  = Domain.Test.Utilities.WaitTimeoutForLineTransmission;

		                       /// <remarks><see cref="Domain.Test.Utilities.WaitIntervalForTransmission"/>.</remarks>
		public const int WaitIntervalForTransmission = Domain.Test.Utilities.WaitIntervalForTransmission;

		#endregion

		#region Devices
		//==========================================================================================
		// Devices
		//==========================================================================================

		internal static IEnumerable<Triple<Pair<TerminalSettingsDelegate<string>, string>, string, string>> MTSicsDevices
		{
			get
			{
				foreach (var dev in MTSicsSerialPortDevices)
					yield return (dev);

				foreach (var dev in MTSicsTcpAutoSocketDevices)
					yield return (dev);

				foreach (var dev in MTSicsUsbDevices)
					yield return (dev);
			}
		}

		internal static IEnumerable<Triple<Pair<TerminalSettingsDelegate<string>, string>, string, string>> MTSicsSerialPortDevices
		{
			get
			{
				if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable ||
					!MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable) // Add 'A' if neither device is available => 'Ignore' is issued in that case.
				{
					var settingsDelegate = new Pair<TerminalSettingsDelegate<string>, string>(GetStartedSerialPortMTSicsDeviceATextSettings, null);
					yield return (new Triple<Pair<TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Ports.Test.ConfigurationCategoryStrings.MTSicsDeviceAIsAvailable, "SerialPort_MTSicsDeviceA"));
				}

				if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable)
				{
					var settingsDelegate = new Pair<TerminalSettingsDelegate<string>, string>(GetStartedSerialPortMTSicsDeviceBTextSettings, null);
					yield return (new Triple<Pair<TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Ports.Test.ConfigurationCategoryStrings.MTSicsDeviceBIsAvailable, "SerialPort_MTSicsDeviceB"));
				}
			}
		}

		internal static IEnumerable<Triple<Pair<TerminalSettingsDelegate<string>, string>, string, string>> MTSicsTcpAutoSocketDevices
		{
			get
			{
				// Add device in any case => 'Ignore' is issued if device is not available.
				{
					var settingsDelegate = new Pair<TerminalSettingsDelegate<string>, string>(GetStartedTcpAutoSocketMTSicsDeviceTextSettings, null);
					yield return (new Triple<Pair<TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.Net.Test.ConfigurationCategoryStrings.MTSicsDeviceIsAvailable, "TcpAutoSocket_MTSicsDevice"));
				}
			}
		}

		internal static IEnumerable<Triple<Pair<TerminalSettingsDelegate<string>, string>, string, string>> MTSicsUsbDevices
		{
			get
			{
				if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable ||
					!MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable) // Add 'A' if neither device is available => 'Ignore' is issued in that case.
				{
					var settingsDelegate = new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUsbSerialHidMTSicsDeviceATextSettings, null);
					yield return (new Triple<Pair<TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Usb.Test.ConfigurationCategoryStrings.MTSicsDeviceAIsAvailable, "UsbSerialHid_MTSicsDeviceA"));
				}

				if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable)
				{
					var settingsDelegate = new Pair<TerminalSettingsDelegate<string>, string>(GetStartedUsbSerialHidMTSicsDeviceBTextSettings, null);
					yield return (new Triple<Pair<TerminalSettingsDelegate<string>, string>, string, string>(settingsDelegate, MKY.IO.Usb.Test.ConfigurationCategoryStrings.MTSicsDeviceBIsAvailable, "UsbSerialHid_MTSicsDeviceB"));
				}
			}
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		internal static TerminalSettingsRoot GetTextSettings()
		{
			var settings = new TerminalSettingsRoot();
			settings.TerminalType = Domain.TerminalType.Text;
			settings.Terminal.TextTerminal.ShowEol = true; // Required for easier test verification (char/byte count).
			settings.Terminal.UpdateTerminalTypeDependentSettings(); // Consider moving to each test instead.
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
			settings.Terminal.UpdateIOTypeDependentSettings();
			settings.Terminal.UpdateIOSettingsDependentSettings();
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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedSerialPortBTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedSerialPortMTSicsDeviceATextSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable)
			{
				var settings = GetStartedSerialPortTextSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceA);
				ConfigureMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceA' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedSerialPortMTSicsDeviceATextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedSerialPortMTSicsDeviceATextSettings());
		}

		internal static TerminalSettingsRoot GetStartedSerialPortMTSicsDeviceBTextSettings()
		{
			if (MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable)
			{
				var settings = GetStartedSerialPortTextSettings(MKY.IO.Ports.Test.ConfigurationProvider.Configuration.MTSicsDeviceB);
				ConfigureMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceB' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedSerialPortMTSicsDeviceBTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			settings.Terminal.UpdateIOTypeDependentSettings();
			settings.Terminal.UpdateIOSettingsDependentSettings();
			settings.TerminalIsStarted = true;
			return (settings);
		}

		#region Settings > Socket > TCP/IP Client
		//------------------------------------------------------------------------------------------
		// Settings > Socket > TCP/IP Client
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedTcpClientOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpClientOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedTcpClientOnIPv6SpecificInterfaceTextSettings());
		}

		#endregion

		#region Settings > Socket > TCP/IP Server
		//------------------------------------------------------------------------------------------
		// Settings > Socket > TCP/IP Server
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedTcpServerOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpServerOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedTcpServerOnIPv6SpecificInterfaceTextSettings());
		}

		#endregion

		#region Settings > Socket > TCP/IP AutoSocket
		//------------------------------------------------------------------------------------------
		// Settings > Socket > TCP/IP AutoSocket
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpAutoSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedTcpAutoSocketOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.TcpAutoSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedTcpAutoSocketOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedTcpAutoSocketMTSicsDeviceTextSettings());
		}

		#endregion

		#region Settings > Socket > UDP/IP Client
		//------------------------------------------------------------------------------------------
		// Settings > Socket > UDP/IP Client
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedUdpClientOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpClient, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpClientOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedUdpClientOnIPv6SpecificInterfaceTextSettings());
		}

		#endregion

		#region Settings > Socket > UDP/IP Server
		//------------------------------------------------------------------------------------------
		// Settings > Socket > UDP/IP Server
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedUdpServerOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpServer, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpServerOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedUdpServerOnIPv6SpecificInterfaceTextSettings());
		}

		#endregion

		#region Settings > Socket > UDP/IP PairSocket
		//------------------------------------------------------------------------------------------
		// Settings > Socket > UDP/IP PairSocket
		//------------------------------------------------------------------------------------------

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv4LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpPairSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv4Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv4LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedUdpPairSocketOnIPv4LoopbackTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv6LoopbackTextSettings()
		{
			return (GetStartedTextSettings(Domain.IOType.UdpPairSocket, (IPNetworkInterfaceEx)IPNetworkInterface.IPv6Loopback));
		}

		internal static TerminalSettingsRoot GetStartedUdpPairSocketOnIPv6LoopbackTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			return (GetStartedUsbSerialHidTextSettings((MKY.IO.Usb.HidDeviceInfo)deviceInfo));
		}

		internal static TerminalSettingsRoot GetStartedUsbSerialHidTextSettings(MKY.IO.Usb.HidDeviceInfo deviceInfo)
		{
			var settings = GetTextSettings();
			settings.Terminal.IO.IOType = Domain.IOType.UsbSerialHid;
			settings.Terminal.IO.UsbSerialHidDevice.DeviceInfo = deviceInfo;
			settings.Terminal.UpdateIOTypeDependentSettings();
			settings.Terminal.UpdateIOSettingsDependentSettings();
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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedUsbSerialHidDeviceBTextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUsbSerialHidMTSicsDeviceATextSettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceAIsAvailable)
			{
				var settings = GetStartedUsbSerialHidTextSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceA);
				ConfigureMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceA' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceA' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedUsbSerialHidMTSicsDeviceATextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

			return (GetStartedUsbSerialHidMTSicsDeviceATextSettings());
		}

		internal static TerminalSettingsRoot GetStartedUsbSerialHidMTSicsDeviceBTextSettings()
		{
			if (MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceBIsAvailable)
			{
				var settings = GetStartedUsbSerialHidTextSettings(MKY.IO.Usb.Test.ConfigurationProvider.Configuration.MTSicsDeviceB);
				ConfigureMTSicsSettings(settings);
				return (settings);
			}

			Assert.Ignore("'MTSicsDeviceB' is not available, therefore this test is excluded. Ensure that 'MTSicsDeviceB' is properly configured and available if passing this test is required.");
		//// Using Ignore() instead of Inconclusive() to get a yellow bar, not just a yellow question mark.

			return (null);
		}

		internal static TerminalSettingsRoot GetStartedUsbSerialHidMTSicsDeviceBTextSettings(string dummy)
		{
			UnusedArg.PreventAnalysisWarning(dummy, "Argument is required to provide signature of common type TerminalSettingsDelegate<string>.");

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
			settings.Terminal.IO.UsbSerialHidDevice      .FlowControl = SerialHidFlowControl.Software;
			settings.Terminal.UpdateIOSettingsDependentSettings();

			// Set required USB Ser/HID format incl. dependent settings:
			var presetEx = new MKY.IO.Usb.SerialHidDeviceSettingsPresetEx(MKY.IO.Usb.SerialHidDeviceSettingsPreset.MT_SerHid);
			settings.Terminal.IO.UsbSerialHidDevice.Preset        = presetEx;
			settings.Terminal.IO.UsbSerialHidDevice.ReportFormat  = presetEx.ToReportFormat();
			settings.Terminal.IO.UsbSerialHidDevice.RxFilterUsage = presetEx.ToRxFilterUsage();
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

				Trace.WriteLine("Waiting for start, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Start timeout!");
				}
			}
			while (!terminal.IsStarted);

			Trace.WriteLine("...done, started");
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

				Trace.WriteLine("Waiting for open, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Open timeout!");
				}
			}
			while (!terminal.IsOpen);

			Trace.WriteLine("...done, opened");
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

				Trace.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Connect timeout!");
				}
			}
			while (!terminal.IsConnected);

			Trace.WriteLine("...done, connected");
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

				Trace.WriteLine("Waiting for connection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Connect timeout!");
				}
			}
			while (!terminalA.IsConnected && !terminalB.IsConnected);

			Trace.WriteLine("...done, connected");
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

				Trace.WriteLine("Waiting for close, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Close timeout!");
				}
			}

			Trace.WriteLine("...done, closed");
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

				Trace.WriteLine("Waiting for disconnection, " + waitTime + " ms have passed, timeout is " + WaitTimeoutForStateChange + " ms...");

				if (waitTime >= WaitTimeoutForStateChange) {
					Assert.Fail("Disconnect timeout!");
				}
			}

			Trace.WriteLine("...done, disconnected");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines, *and* the number of complete lines!
		/// </remarks>
		internal static void WaitForReceivingAndVerifyCounts(Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			WaitForReceivingAndVerifyCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCount, expectedTotalLineCount);
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		internal static void WaitForReceivingAndVerifyCounts(Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalLineCountCompleted, int timeout = WaitTimeoutForLineTransmission)
		{
			int rxByteCount = 0;
			int rxLineCount = 0;
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForTransmission);
				waitTime += WaitIntervalForTransmission;

				Trace.WriteLine("Waiting for receiving, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				rxByteCount = terminalRx.GetRepositoryByteCount(Domain.RepositoryType.Rx);
				if (rxByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of received bytes = " + rxByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				rxLineCount = terminalRx.GetRepositoryLineCount(Domain.RepositoryType.Rx);
				if (rxLineCount > expectedTotalLineCountDisplayed) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of received lines = " + rxLineCount +
					            " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
				}

				if (waitTime >= timeout) {
					var sb = new StringBuilder("Timeout!");

					if (rxByteCount < expectedTotalByteCount) {
						sb.Append(" Number of received bytes = " + rxByteCount +
						          " mismatches expected = " + expectedTotalByteCount + ".");
					}

					if (rxLineCount < expectedTotalLineCountDisplayed) {
						sb.Append(" Number of received lines = " + rxLineCount +
						          " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
					}

					Assert.Fail(sb.ToString());
				}
			}
			while ((rxByteCount != expectedTotalByteCount) || (rxLineCount != expectedTotalLineCountDisplayed));

			Debug.WriteLine("Rx of " + rxByteCount + " bytes / " + rxLineCount + " lines completed");

			// Also assert count properties:
			Assert.That(terminalRx.RxByteCount, Is.EqualTo(expectedTotalByteCount));
			Assert.That(terminalRx.RxLineCount, Is.EqualTo(expectedTotalLineCountCompleted));

			Trace.WriteLine("...done, received and verified");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		internal static void WaitForReceivingCycleAndVerifyCounts(Terminal terminalRx, TestSet testSet, int cycle)
		{
			// Calculate total expected counts at the receiver side:
			int expectedTotalByteCount          = (testSet.ExpectedTotalByteCount     * cycle);
			int expectedTotalLineCountDisplayed = (testSet.ExpectedLineCountDisplayed * cycle);
			int expectedTotalLineCountCompleted = (testSet.ExpectedLineCountCompleted * cycle);

			// Calculate timeout:
			int timeoutFactorPerLine = ((testSet.ExpectedLineCountCompleted > 0) ? (testSet.ExpectedLineCountCompleted) : (1)); // Take cases with 0 lines into account!
			int timeout = (WaitTimeoutForLineTransmission * timeoutFactorPerLine);

			WaitForReceivingAndVerifyCounts(terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountCompleted, timeout);
		}

		internal static void WaitForTransmissionAndVerifyCounts(Terminal terminalTx, Terminal terminalRx, TestSet testSet)
		{
			WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, testSet.ExpectedTotalByteCount, testSet.ExpectedLineCountDisplayed, testSet.ExpectedLineCountCompleted);
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		/// <remarks>
		/// 'expectedTotalLineCount' will be compared against the number of lines in the view,
		/// i.e. complete as well as incomplete lines, *and* the number of complete lines!
		/// </remarks>
		internal static void WaitForTransmissionAndVerifyCounts(Terminal terminalTx, Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCount)
		{
			WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCount, expectedTotalLineCount);
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		internal static void WaitForTransmissionAndVerifyCounts(Terminal terminalTx, Terminal terminalRx, int expectedTotalByteCount, int expectedTotalLineCountDisplayed, int expectedTotalLineCountCompleted, int timeout = WaitTimeoutForLineTransmission)
		{
			// Attention:
			// Similar code exists in Domain.Test.Utilities.WaitForTransmissionAndVerifyCounts().
			// Changes here may have to be applied there too.

			int txByteCount = 0;
			int txLineCount = 0;
			int rxByteCount = 0;
			int rxLineCount = 0;
			int waitTime = 0;
			do                         // Initially wait to allow async send,
			{                          //   therefore, use do-while.
				Thread.Sleep(WaitIntervalForTransmission);
				waitTime += WaitIntervalForTransmission;

				Trace.WriteLine("Waiting for transmission, " + waitTime + " ms have passed, timeout is " + timeout + " ms...");

				txByteCount = terminalTx.GetRepositoryByteCount(Domain.RepositoryType.Tx);
				if (txByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of sent bytes = " + txByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				txLineCount = terminalTx.GetRepositoryLineCount(Domain.RepositoryType.Tx);
				if (txLineCount > expectedTotalLineCountDisplayed) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of sent lines = " + txLineCount +
					            " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
				}

				rxByteCount = terminalRx.GetRepositoryByteCount(Domain.RepositoryType.Rx);
				if (rxByteCount > expectedTotalByteCount) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of received bytes = " + rxByteCount +
					            " mismatches expected = " + expectedTotalByteCount + ".");
				}

				rxLineCount = terminalRx.GetRepositoryLineCount(Domain.RepositoryType.Rx);
				if (rxLineCount > expectedTotalLineCountDisplayed) { // Break in case of too much data to improve speed of test.
					Assert.Fail("Number of received lines = " + rxLineCount +
					            " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
				}

				if (waitTime >= timeout) {
					var sb = new StringBuilder("Timeout!");

					if (txByteCount < expectedTotalByteCount) {
						sb.Append(" Number of sent bytes = " + txByteCount +
						          " mismatches expected = " + expectedTotalByteCount + ".");
					}

					if (txLineCount < expectedTotalLineCountDisplayed) {
						sb.Append(" Number of sent lines = " + txLineCount +
						          " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
					}

					if (rxByteCount < expectedTotalByteCount) {
						sb.Append(" Number of received bytes = " + rxByteCount +
						          " mismatches expected = " + expectedTotalByteCount + ".");
					}

					if (rxLineCount < expectedTotalLineCountDisplayed) {
						sb.Append(" Number of received lines = " + rxLineCount +
						          " mismatches expected = " + expectedTotalLineCountDisplayed + ".");
					}

					Assert.Fail(sb.ToString());
				}
			}
			while ((txByteCount != expectedTotalByteCount) || (txLineCount != expectedTotalLineCountDisplayed) ||
			       (rxByteCount != expectedTotalByteCount) || (rxLineCount != expectedTotalLineCountDisplayed));

			Debug.WriteLine("Tx of " + txByteCount + " bytes / " + txLineCount + " lines completed");
			Debug.WriteLine("Rx of " + rxByteCount + " bytes / " + rxLineCount + " lines completed");

			// Also assert count properties:
			Assert.That(terminalTx.TxByteCount, Is.EqualTo(expectedTotalByteCount));
			Assert.That(terminalTx.TxLineCount, Is.EqualTo(expectedTotalLineCountCompleted));
			Assert.That(terminalRx.RxByteCount, Is.EqualTo(expectedTotalByteCount));
			Assert.That(terminalRx.RxLineCount, Is.EqualTo(expectedTotalLineCountCompleted));

			Trace.WriteLine("...done, transmitted and verified");
		}

		/// <remarks>
		/// There are similar utility methods in <see cref="Domain.Test.Utilities"/>.
		/// Changes here may have to be applied there too.
		/// </remarks>
		/// <remarks>
		/// 'expectedPerCycleCharCount' does not need to be considered, since bytes are transmitted.
		/// </remarks>
		internal static void WaitForTransmissionCycleAndVerifyCounts(Terminal terminalTx, Terminal terminalRx, TestSet testSet, int cycle)
		{
			// Calculate total expected counts at the receiver side:
			int expectedTotalByteCount          = (testSet.ExpectedTotalByteCount     * cycle);
			int expectedTotalLineCountDisplayed = (testSet.ExpectedLineCountDisplayed * cycle);
			int expectedTotalLineCountCompleted = (testSet.ExpectedLineCountCompleted * cycle);

			// Calculate timeout:
			int timeoutFactorPerLine = ((testSet.ExpectedLineCountCompleted > 0) ? (testSet.ExpectedLineCountCompleted) : (1)); // Take cases with 0 lines into account!
			int timeout = (WaitTimeoutForLineTransmission * timeoutFactorPerLine);

			WaitForTransmissionAndVerifyCounts(terminalTx, terminalRx, expectedTotalByteCount, expectedTotalLineCountDisplayed, expectedTotalLineCountCompleted, timeout);
		}

		#endregion

		#region Verifications
		//==========================================================================================
		// Verifications
		//==========================================================================================

		/// <remarks>Using 'A' and 'B' instead of 'Tx' and 'Rx' as some tests perform two-way-transmission.</remarks>
		internal static void VerifyLines(Domain.DisplayLineCollection displayLinesA, Domain.DisplayLineCollection displayLinesB, TestSet testSet, int cycle = 1)
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
					sbB.Append(ArrayEx.ValuesToString(displayLineB.ToArray()));

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
						int expectedCharCount    =     testSet.ExpectedCharCounts[index];
						int expectedByteCount    =     testSet.ExpectedByteCounts[index];

						var displayLineA = displayLinesA[i];
						var displayLineB = displayLinesB[i];

						if ((displayLineB.Count     == displayLineA.Count)     &&
							(displayLineB.Count     == expectedElementCount)   &&
							(displayLineB.CharCount == displayLineA.CharCount) &&
							(displayLineB.CharCount == expectedCharCount)      &&
							(displayLineB.ByteCount == displayLineA.ByteCount) &&
							(displayLineB.ByteCount == expectedByteCount))
						{
							for (int j = 0; j < displayLineA.Count; j++)
								Assert.That(displayLineB[j].Text, Is.EqualTo(displayLineA[j].Text));
						}
						else
						{
							string strA = ArrayEx.ValuesToString(displayLineA.ToArray());
							string strB = ArrayEx.ValuesToString(displayLineB.ToArray());

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
								"Expected = " + expectedCharCount + " char(s), " +
								"A = " + displayLineA.CharCount + " char(s), " +
								"B = " + displayLineB.CharCount + " char(s)." + Environment.NewLine +
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
						sbA.Append(ArrayEx.ValuesToString(displayLineA.ToArray()));

					var sbB = new StringBuilder();
					foreach (Domain.DisplayLine displayLineB in displayLinesB)
						sbB.Append(ArrayEx.ValuesToString(displayLineB.ToArray()));

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
			if (e.Text.StartsWith("Unable to start terminal", StringComparison.Ordinal)) // 'Ordinal' since YAT is all-English and test is passable with this strict comparison.
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
