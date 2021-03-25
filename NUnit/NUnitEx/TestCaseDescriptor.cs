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
// NUnit Version 1.0.22
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace NUnitEx
{
	/// <summary>
	/// Encapsulates information to initialize <see cref="TestCaseData"/> objects.
	/// </summary>
	/// <remarks>
	/// Useful to encapsulate information where <see cref="TestCaseData"/> objects are
	/// not suitable or suboptimal, e.g. where named and typed arguments shall be used rather than
	/// just an array of <see cref="object"/> named "arg1", "arg2",...
	/// </remarks>
	/// <remarks>
	/// Simply derive from this class and add arguments as needed.
	/// </remarks>
	/// <remarks>
	/// Named 'TestCaseDescriptor' rather than e.g. 'TestCaseDataHelper' for better distinguishing
	/// from <see cref="TestCaseData"/>.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'argI' just happens to be an 'arg'...")]
	public class TestCaseDescriptor
	{
		/// <summary>
		/// The name for setting <see cref="TestCaseData.TestName"/>.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The categories for setting <see cref="TestCaseData.Categories"/>.
		/// </summary>
		public IEnumerable<string> Categories { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TestCaseDescriptor"/> class.
		/// </summary>
		public TestCaseDescriptor(string name, IEnumerable<string> categories)
		{
			Name = name;
			Categories = categories;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
