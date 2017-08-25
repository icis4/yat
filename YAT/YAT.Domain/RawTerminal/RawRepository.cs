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
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// RawRepository is a fixed-sized Queue holding RawElements.
	/// </summary>
	public class RawRepository
	{
		private int capacity;
		private Queue<RawChunk> queue;

		/// <summary></summary>
		public RawRepository(int capacity)
		{
			this.capacity = capacity;
			this.queue = new Queue<RawChunk>(this.capacity);
		}

		/// <summary></summary>
		public RawRepository(RawRepository rhs)
		{
			this.capacity = rhs.capacity;
			this.queue = new Queue<RawChunk>(rhs.queue);
		}

	#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~RawRepository()
		{
			MKY.Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

	#endif // DEBUG

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual int Capacity
		{
			get { return (this.capacity); }
			set
			{
				if (value == this.queue.Count)
					return;

				if (value < this.queue.Count)
				{
					lock (this.queue)
					{
						while (this.queue.Count > value)
							this.queue.Dequeue();

						this.queue.TrimExcess();

						this.capacity = value;
					}
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Enqueue(RawChunk chunk)
		{
			lock (this.queue)
			{
				while (this.queue.Count >= this.capacity)
					this.queue.Dequeue();

				// Do not TrimExcess() as queue length/count is limited.

				this.queue.Enqueue(chunk);
			}
		}

		/// <summary></summary>
		public virtual void Clear()
		{
			lock (this.queue)
			{
				this.queue.Clear();
			}
		}

		/// <summary></summary>
		public virtual byte[] ToByteArray()
		{
			List<byte> to;
			lock (this.queue)
			{
				to = new List<byte>(this.queue.Count);
				foreach (RawChunk re in this.queue.ToArray())
					to.AddRange(re.Data);
			}
			return (to.ToArray());
		}

		/// <summary></summary>
		public virtual List<RawChunk> ToChunks()
		{
			List<RawChunk> to;
			lock (this.queue)
			{
				to = new List<RawChunk>(this.queue);
			}
			return (to);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (ToString(""));
		}

		/// <summary></summary>
		public virtual string ToString(string indent)
		{
			return (QueueToString(indent));
		}

		/// <summary></summary>
		public virtual string ToDetailedString()
		{
			return (ToDetailedString(""));
		}

		/// <summary></summary>
		public virtual string ToDetailedString(string indent)
		{
			return (indent + "> Capacity: " + this.capacity + Environment.NewLine +
					indent + "> Queue: " + Environment.NewLine +
					QueueToDetailedString(indent + "   "));
		}

		/// <summary></summary>
		public virtual string QueueToString()
		{
			return (QueueToString(""));
		}

		/// <summary></summary>
		public virtual string QueueToString(string indent)
		{
			using (var sw = new StringWriter(CultureInfo.InvariantCulture))
			{
				foreach (var re in ToChunks())
					sw.Write(re.ToString(indent));

				return (sw.ToString());
			}
		}

		/// <summary></summary>
		public virtual string QueueToDetailedString()
		{
			return (QueueToDetailedString(""));
		}

		/// <summary></summary>
		public virtual string QueueToDetailedString(string indent)
		{
			var sb = new StringBuilder();

			int i = 0;
			foreach (RawChunk re in ToChunks())
			{
				sb.Append(indent + "> RawChunk#" + (i++) + ":" + Environment.NewLine);
				sb.Append(re.ToDetailedString(indent + "   "));
			}

			if (i == 0)
				sb.AppendLine(indent + "<NONE>");

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
