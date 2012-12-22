//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
[assembly: AssemblyTitle("MKY")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("MKY")]
[assembly: AssemblyProduct("MKY")]
[assembly: AssemblyCopyright("Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil. Copyright © 2003-2012 Matthias Kläy. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.
[assembly: Guid("e352407e-68f9-41d0-8306-d3ba2779dd8d")]

// Assembly versions are defined by linked-in MKY.Version.cs.

// CLS compliance.
[assembly: CLSCompliant(true)]

// Assembly-level FxCop suppressions.
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Contracts", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Collections", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Collections.Generic", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.CommandLine", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Data", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Drawing", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Event", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Globalization", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Recent", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Runtime.InteropServices", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Text", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Time", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Usb", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Xml", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Xml.Schema", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "namespace", Target = "MKY.Xml.Serialization", MessageId = "MKY", Justification = "MKY is a name.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Contracts", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Collections", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Collections.Generic", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.CommandLine", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Data", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Drawing", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Event", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Globalization", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Recent", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Runtime.InteropServices", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Text", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Time", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Usb", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Xml", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Xml.Schema", Justification = "Name of namespaces is given by the .NET framework.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MKY.Xml.Serialization", Justification = "Name of namespaces is given by the .NET framework.")]

//==================================================================================================
// End of
// $URL$
//==================================================================================================
