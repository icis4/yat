using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Generic
{
	/// <summary>
	/// Variance workaround, allows casts of generics (e.g. List of device to List of object)
	/// </summary>
	/// <remarks>
	/// Taken from "C# Programming Guide" topic "Variance in Generic Types"
	/// </remarks>
	public static class VarianceWorkaround
	{
		/// <summary>
		/// Simple variance for single method, variance in one direction only 
		/// </summary>
		public static void Add<S, D>(List<S> source, List<D> destination)
			where S : D
		{
			foreach (S sourceElement in source)
			{
				destination.Add(sourceElement);
			}
		}

		/// <summary>
		/// Variance for enummerator, variance in one direction only so type expressinos are natural
		/// </summary>
		public static IEnumerable<D> Convert<S, D>(IEnumerable<S> source)
			where S : D
		{
			return (new EnumerableWrapper<S, D>(source));
		}

		/// <summary>
		/// Wrapper for enummerator variance
		/// </summary>
		public class EnumerableWrapper<S, D> : IEnumerable<D>
			where S : D
		{
			private IEnumerable<S> _source;

			/// <summary></summary>
			public EnumerableWrapper(IEnumerable<S> source)
			{
				_source = source;
			}

			/// <summary></summary>
			public IEnumerator<D> GetEnumerator()
			{
				return (new EnumeratorWrapper(_source.GetEnumerator()));
			}

			/// <summary></summary>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return (this.GetEnumerator());
			}

			private class EnumeratorWrapper : IEnumerator<D>
			{
				private IEnumerator<S> _source;

				public EnumeratorWrapper(IEnumerator<S> source)
				{
					_source = source;
				}

				public D Current
				{
					get { return (_source.Current); }
				}

				public void Dispose()
				{
					_source.Dispose();
				}

				object System.Collections.IEnumerator.Current
				{
					get { return (_source.Current); }
				}

				public bool MoveNext()
				{
					return (_source.MoveNext());
				}

				public void Reset()
				{
					_source.Reset();
				}
			}
		}

		/// <summary>
		/// Variance for collection, variance in both directions, causes issues similar to existing array variance
		/// </summary>
		public static ICollection<D> Convert<S, D>(ICollection<S> source)
			where S : D
		{
			return (new CollectionWrapper<S, D>(source));
		}

		/// <summary>
		/// Wrapper for collection variance
		/// </summary>
		public class CollectionWrapper<S, D> : EnumerableWrapper<S, D>, ICollection<D>
			where S : D
		{
			private ICollection<S> _source;

			/// <summary></summary>
			public CollectionWrapper(ICollection<S> source)
				: base(source)
			{
				_source = source;
			}

			/// <summary>
			/// variance going the wrong way ... 
			/// ... can yield exceptions at runtime 
			/// </summary>
			public void Add(D item)
			{
				if (item is S)
				{
					_source.Add((S)item);
				}
				else
				{
					throw (new Exception("Type mismatch due to type hole introduced by variance!"));
				}
			}

			/// <summary></summary>
			public void Clear()
			{
				_source.Clear();
			}

			/// <summary>
			/// variance going the wrong way ... 
			/// ... but the semantics of the method yields reasonable semantics
			/// </summary>
			public bool Contains(D item)
			{
				if (item is S)
				{
					return (_source.Contains((S)item));
				}
				else
				{
					return (false);
				}
			}

			/// <summary>
			/// variance going the right way ... 
			/// </summary>
			public void CopyTo(D[] array, int arrayIndex)
			{
				foreach (S src in _source)
				{
					array[arrayIndex++] = src;
				}
			}

			/// <summary></summary>
			public int Count
			{
				get { return (_source.Count); }
			}

			/// <summary></summary>
			public bool IsReadOnly
			{
				get { return (_source.IsReadOnly); }
			}

			/// <summary>
			/// variance going the wrong way ... 
			/// ... but the semantics of the method yields reasonable semantics
			/// </summary>
			public bool Remove(D item)
			{
				if (item is S)
				{
					return (_source.Remove((S)item));
				}
				else
				{
					return (false);
				}
			}
		}

		/// <summary>
		/// Variance for collection, variance in both directions, causes issues similar to existing array variance
		/// </summary>
		public static IList<D> Convert<S, D>(IList<S> source)
			where S : D
		{
			return (new ListWrapper<S, D>(source));
		}

		/// <summary>
		/// Wrapper for list variance
		/// </summary>
		public class ListWrapper<S, D> : CollectionWrapper<S, D>, IList<D>
			where S : D
		{
			private IList<S> _source;

			/// <summary></summary>
			public ListWrapper(IList<S> source)
				: base(source)
			{
				_source = source;
			}

			/// <summary></summary>
			public int IndexOf(D item)
			{
				if (item is S)
				{
					return (_source.IndexOf((S)item));
				}
				else
				{
					return (-1);
				}
			}

			/// <summary>
			/// variance the wrong way ...
			/// ... can throw exceptions at runtime
			/// </summary>
			public void Insert(int index, D item)
			{
				if (item is S)
				{
					_source.Insert(index, (S)item);
				}
				else
				{
					throw (new Exception("Invalid type!"));
				}
			}

			/// <summary></summary>
			public void RemoveAt(int index)
			{
				_source.RemoveAt(index);
			}

			/// <summary></summary>
			public D this[int index]
			{
				get
				{
					return (_source[index]);
				}
				set
				{
					if (value is S)
						_source[index] = (S)value;
					else
						throw (new Exception("Invalid type!"));
				}
			}
		}
	}
}
