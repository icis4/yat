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
// YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("YAT.Domain.Test")]
[assembly: AssemblyDescription("YAT domain components test")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("YAT")]
[assembly: AssemblyProduct("YAT")]
[assembly: AssemblyCopyright("Copyright © 2007-2013 Matthias Kläy. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.
[assembly: Guid("bff64daf-924c-40cb-a2ea-73c1cb4f5457")]

// Assembly versions are defined by linked-in YAT.Version.cs.

// CLS compliance.
[assembly: CLSCompliant(true)]

// Assembly-level FxCop suppressions.
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "YAT.Domain.Test.Parser", Justification = "Namespace structure of test assembly is defined by testee assembly.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "YAT.Domain.Test.TextTerminal", Justification = "Namespace structure of test assembly is defined by testee assembly.")]

//==================================================================================================
// End of
// $URL$
//==================================================================================================
