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
// NUnit Version 1.0.22 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using NUnit.Framework;

namespace NUnitEx
{
	/// <summary></summary>
	[TestFixture]
	public class StandardCategoriesDummyInstance
	{
		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Second1]
		public virtual void Second1DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Seconds4]
		public virtual void Second4DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Seconds15]
		public virtual void Second15DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Minute1]
		public virtual void Minute1DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Minutes4]
		public virtual void Minutes4DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Seconds15]
		public virtual void Minutes15DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Hour1]
		public virtual void Hour1DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Hours5]
		public virtual void Hours5DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Hours24]
		public virtual void Hours24DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Day1]
		public virtual void Day1DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Days7]
		public virtual void Days7DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Days30]
		public virtual void Days30DummyInstance()
		{
			// Dummy only, nothing to do.
		}

		/// <remarks>
		/// Dummy instance to ensure every standard instance is instantiated and can thus be
		/// activated or excluded in the NUnit test runner.
		/// </remarks>
		[Test, StandardDurationCategory.Infinite]
		public virtual void InfiniteDummyInstance()
		{
			// Dummy only, nothing to do.
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
