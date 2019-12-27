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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// This class wraps a size limited <see cref="Queue{T}"/> holding <see cref="RawChunk"/> items.
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
				foreach (RawChunk chunk in this.queue.ToArray())
					to.AddRange(chunk.Content);
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
			return (QueueToString());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public virtual string QueueToString()
		{
			var sb = new StringBuilder();

			bool isFirst = true;
			foreach (var re in ToChunks())
			{
				if (isFirst)
					isFirst = false;
				else
					sb.Append(" ");

				sb.Append(re.ToString());
			}

			return (sb.ToString());
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string ToExtendedDiagnosticsString(string indent = "")
		{
			return (indent + "> Capacity: " + this.capacity + Environment.NewLine +
					indent + "> Queue: " + Environment.NewLine +
					QueueToExtendedDiagnosticsString(indent + "   "));
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="QueueToString()"/> method which can be used for trace/debug.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual string QueueToExtendedDiagnosticsString(string indent = "")
		{
			var sb = new StringBuilder();

			int i = 0;
			foreach (var re in ToChunks())
			{
				sb.AppendLine(indent + "> RawChunk#" + (i++) + ":");
				sb.Append(re.ToExtendedDiagnosticsString(indent + "   "));
			}

			if (i == 0)
				sb.AppendLine(indent + "<None>");

			return (sb.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
