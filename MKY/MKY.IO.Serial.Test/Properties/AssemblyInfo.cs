//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.7
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MKY.IO.Serial.Test")]
[assembly: AssemblyDescription("System.IO extension test")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("MKY")]
[assembly: AssemblyProduct("MKY.IO.Serial.Test")]
[assembly: AssemblyCopyright("Copyright © 2007-2011 Matthias Kläy. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.
[assembly: Guid("de62a24c-6df4-4ba2-aa70-9b46eaec10b9")]

// Assembly versions are defined by linked-in MKY.Version.cs.

// CLS compliance.
[assembly: CLSCompliant(true)]

// Assembly-level FxCop suppressions.
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.IO.Serial.Test", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.IO.Serial.Test.SerialPort", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.IO.Serial.Test.Socket", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.IO.Serial.Test.Usb", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.IO.Serial.Test.SerialPort", Justification = "Namespace for orthogonality of test and testee namespaces.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.IO.Serial.Test.Usb", Justification = "Namespace for orthogonality of test and testee namespaces.")]

//==================================================================================================
// End of
// $URL$
//==================================================================================================
