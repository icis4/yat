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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Collections.Generic
{
	/// <summary>
	/// Variance workaround, allows casts of generics (e.g. List of device to List of object)
	/// </summary>
	/// <remarks>
	/// Taken from "C# Programming Guide" topic "Variance in Generic Types".
	/// </remarks>
	public static class VarianceWorkaround
	{
		/// <summary>
		/// Simple variance for single method, variance in one direction only.
		/// </summary>
		/// <typeparam name="S">IList source.</typeparam>
		/// <typeparam name="D">IList destination.</typeparam>
		public static void Add<S, D>(IList<S> source, IList<D> destination)
			where S : D
		{
			foreach (S sourceElement in source)
			{
				destination.Add(sourceElement);
			}
		}

		/// <summary>
		/// Variance for enummerator, variance in one direction only so type expressinos are natural.
		/// </summary>
		/// <typeparam name="S">IEnumerable source.</typeparam>
		/// <typeparam name="D">IEnumerable destination.</typeparam>
		public static IEnumerable<D> Convert<S, D>(IEnumerable<S> source)
			where S : D
		{
			return (new EnumerableWrapper<S, D>(source));
		}

		/// <summary>
		/// Wrapper for enummerator variance.
		/// </summary>
		/// <typeparam name="S">IEnumerable source.</typeparam>
		/// <typeparam name="D">IEnumerable destination.</typeparam>
		public class EnumerableWrapper<S, D> : IEnumerable<D>
			where S : D
		{
			private IEnumerable<S> source;

			/// <summary></summary>
			public EnumerableWrapper(IEnumerable<S> source)
			{
				this.source = source;
			}

			/// <summary></summary>
			public IEnumerator<D> GetEnumerator()
			{
				return (new EnumeratorWrapper(this.source.GetEnumerator()));
			}

			/// <summary></summary>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return (this.GetEnumerator());
			}

			private class EnumeratorWrapper : IEnumerator<D>
			{
				private IEnumerator<S> source;

				public EnumeratorWrapper(IEnumerator<S> source)
				{
					this.source = source;
				}

				public D Current
				{
					get { return (this.source.Current); }
				}

				public void Dispose()
				{
					this.source.Dispose();
				}

				object System.Collections.IEnumerator.Current
				{
					get { return (this.source.Current); }
				}

				public virtual bool MoveNext()
				{
					return (this.source.MoveNext());
				}

				public virtual void Reset()
				{
					this.source.Reset();
				}
			}
		}

		/// <summary>
		/// Variance for collection, variance in both directions, causes issues similar to existing array variance.
		/// </summary>
		/// <typeparam name="S">ICollection source.</typeparam>
		/// <typeparam name="D">ICollection destination.</typeparam>
		public static ICollection<D> Convert<S, D>(ICollection<S> source)
			where S : D
		{
			return (new CollectionWrapper<S, D>(source));
		}

		/// <summary>
		/// Wrapper for collection variance.
		/// </summary>
		/// <typeparam name="S">ICollection source.</typeparam>
		/// <typeparam name="D">ICollection destination.</typeparam>
		public class CollectionWrapper<S, D> : EnumerableWrapper<S, D>, ICollection<D>
			where S : D
		{
			private ICollection<S> source;

			/// <summary></summary>
			public CollectionWrapper(ICollection<S> source)
				: base(source)
			{
				this.source = source;
			}

			/// <summary>
			/// Variance going the wrong way...
			/// ...can yield exceptions at runtime.
			/// </summary>
			public virtual void Add(D item)
			{
				if (item is S)
					this.source.Add((S)item);
				else
					throw (new Exception("Type mismatch due to type hole introduced by variance!"));
			}

			/// <summary></summary>
			public virtual void Clear()
			{
				this.source.Clear();
			}

			/// <summary>
			/// Variance going the wrong way...
			/// ...but the semantics of the method yields reasonable semantics.
			/// </summary>
			public virtual bool Contains(D item)
			{
				if (item is S)
				{
					return (this.source.Contains((S)item));
				}
				else
				{
					return (false);
				}
			}

			/// <summary>
			/// Variance going the right way...
			/// </summary>
			public virtual void CopyTo(D[] array, int arrayIndex)
			{
				foreach (S src in this.source)
				{
					array[arrayIndex++] = src;
				}
			}

			/// <summary></summary>
			public virtual int Count
			{
				get { return (this.source.Count); }
			}

			/// <summary></summary>
			public virtual bool IsReadOnly
			{
				get { return (this.source.IsReadOnly); }
			}

			/// <summary>
			/// Variance going the wrong way...
			/// ...but the semantics of the method yields reasonable semantics.
			/// </summary>
			public virtual bool Remove(D item)
			{
				if (item is S)
				{
					return (this.source.Remove((S)item));
				}
				else
				{
					return (false);
				}
			}
		}

		/// <summary>
		/// Variance for collection, variance in both directions, causes issues similar to existing array variance.
		/// </summary>
		/// <typeparam name="S">IList source.</typeparam>
		/// <typeparam name="D">IList destination.</typeparam>
		public static IList<D> Convert<S, D>(IList<S> source)
			where S : D
		{
			return (new ListWrapper<S, D>(source));
		}

		/// <summary>
		/// Wrapper for list variance.
		/// </summary>
		/// <typeparam name="S">IList source.</typeparam>
		/// <typeparam name="D">IList destination.</typeparam>
		public class ListWrapper<S, D> : CollectionWrapper<S, D>, IList<D>
			where S : D
		{
			private IList<S> source;

			/// <summary></summary>
			public ListWrapper(IList<S> source)
				: base(source)
			{
				this.source = source;
			}

			/// <summary></summary>
			public virtual int IndexOf(D item)
			{
				if (item is S)
				{
					return (this.source.IndexOf((S)item));
				}
				else
				{
					return (-1);
				}
			}

			/// <summary>
			/// Variance the wrong way...
			/// ...can throw exceptions at runtime.
			/// </summary>
			public virtual void Insert(int index, D item)
			{
				if (item is S)
				{
					this.source.Insert(index, (S)item);
				}
				else
				{
					throw (new Exception("Invalid type!"));
				}
			}

			/// <summary></summary>
			public virtual void RemoveAt(int index)
			{
				this.source.RemoveAt(index);
			}

			/// <summary></summary>
			public D this[int index]
			{
				get
				{
					return (this.source[index]);
				}
				set
				{
					if (value is S)
						this.source[index] = (S)value;
					else
						throw (new Exception("Invalid type!"));
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
