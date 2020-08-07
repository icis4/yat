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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

namespace NUnitEx
{
	/// <summary>
	/// Encapsulates information to initialize <see cref="Framework.TestCaseData"/> objects.
	/// </summary>
	/// <remarks>
	/// Useful to encapsulate information where <see cref="Framework.TestCaseData"/> objects are
	/// not suitable or suboptimal, e.g. where named and typed arguments shall be used rather than
	/// just an array of <see cref="object"/> named "arg1", "arg2",...
	/// </remarks>
	/// <remarks>
	/// Simply derive from this class and add arguments as needed.
	/// </remarks>
	/// <remarks>
	/// Name "TestCaseDescriptor" rather than e.g. "TestCaseDataHelper" for better distinguishing
	/// from <see cref="Framework.TestCaseData"/>.
	/// </remarks>
	public class TestCaseDescriptor
	{
		/// <summary>
		/// The name for setting <see cref="Framework.TestCaseData.TestName"/>.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The categories for setting <see cref="Framework.TestCaseData.Categories"/>.
		/// </summary>
		public string[] Categories { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TestCaseDescriptor"/> class.
		/// </summary>
		public TestCaseDescriptor(string name, string[] categories)
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
