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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
[assembly: AssemblyTitle("MKY.IO.Serial.Usb.Test")]
[assembly: AssemblyDescription("System.IO extension test")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("MKY")]
[assembly: AssemblyProduct("MKY.IO.Serial.Usb.Test")]
[assembly: AssemblyCopyright("Copyright © 2010-2021 Matthias Kläy. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components. If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.
[assembly: Guid("fb02fe23-d8c9-47cc-b3e8-29fb02810f6e")]

// Assembly versions are defined by linked-in MKY.Version.cs.

// CLS compliance.
[assembly: CLSCompliant(true)]

// Assembly-level FxCop suppressions.
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.IO.Serial.Usb.Test", Justification = "Namespace structure of test assembly is defined by testee assembly.")]
[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Scope = "assembly", Justification = "Intentionally not signing test assemblies, as there will be a warning when accidentally referencing it from a 'normal' assembly.")]

//==================================================================================================
// End of
// $URL$
//==================================================================================================
