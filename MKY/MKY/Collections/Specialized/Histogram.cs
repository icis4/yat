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
// MKY Version 1.0.28 Development
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

using System;
using System.Collections.Generic;

namespace MKY.Collections.Specialized
{
	/// <summary>
	/// Collection of values of a histogram.
	/// </summary>
	/// <typeparam name="T">The type of the values of the histogram</typeparam>
	public abstract class Histogram<T>
	{
		/// <summary></summary>
		public const int DefaultBinCount = 100;

		/// <summary></summary>
		public readonly bool AutoResize; // = false;

		/// <summary></summary>
		public readonly int MaxBinCount;

		private List<long> values = new List<long>(); // No preset needed, the default behavior is good enough.
		private List<long> bins; // = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Histogram{T}"/> class with a <see cref="BinCount"/>
		/// of <see cref="DefaultBinCount"/>.
		/// </summary>
		public Histogram()
			: this(DefaultBinCount)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Histogram{T}"/> class.
		/// </summary>
		public Histogram(int binCount)
		{
			AutoResize = false;
			MaxBinCount = binCount;
			InitializeBins(binCount);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Histogram{T}"/> class,
		/// automatically resizing up to <paramref name="maxBinCount"/>.
		/// </summary>
		public Histogram(int initialBinCount, int maxBinCount)
		{
			AutoResize = true;
			MaxBinCount = maxBinCount;
			InitializeBins(initialBinCount);
		}

		private void InitializeBins(int binCount)
		{
			this.bins = new List<long>(binCount);
			for (int i = 0; i < binCount; i++)
				this.bins.Add(0);
		}

		/// <summary>
		/// Gets the current bin count.
		/// </summary>
		public int BinCount
		{
			get { return (this.bins.Count); }
		}

		/// <summary>
		/// Adds an item to the histogram.
		/// </summary>
		public abstract void Add(T item);

		/// <summary>
		/// Resizes the collection up to <see cref="MaxBinCount"/>.
		/// Needed when <see cref="AutoResize"/> is <c>true</c>.
		/// </summary>
		protected abstract void Resize();
	}

	/// <summary>
	/// Collection of values of type <see cref="double"/> of a histogram.
	/// </summary>
	public class HistogramDouble : Histogram<double>
	{
		public override void Add(double item)
		{
			throw new NotImplementedException();
		}

		protected override void Resize()
		{
			throw new NotImplementedException();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
