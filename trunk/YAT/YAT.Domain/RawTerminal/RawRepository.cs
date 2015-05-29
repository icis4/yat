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
// YAT 2.0 Gamma 1 Version 1.99.32
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
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
		private Queue<RawElement> queue;

		/// <summary></summary>
		public RawRepository(int capacity)
		{
			this.capacity = capacity;
			this.queue = new Queue<RawElement>(this.capacity);
		}

		/// <summary></summary>
		public RawRepository(RawRepository rhs)
		{
			this.capacity = rhs.capacity;
			this.queue = new Queue<RawElement>(rhs.queue);
		}

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

						this.capacity = value;
					}
				}
			}
		}

		/// <summary></summary>
		public virtual void Enqueue(RawElement re)
		{
			lock (this.queue)
			{
				while (this.queue.Count >= this.capacity)
					this.queue.Dequeue();

				this.queue.Enqueue(re);
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
				foreach (RawElement re in this.queue.ToArray())
					to.AddRange(re.Data);
			}
			return (to.ToArray());
		}

		/// <summary></summary>
		public virtual List<RawElement> ToElements()
		{
			List<RawElement> to;
			lock (this.queue)
			{
				to = new List<RawElement>(this.queue);
			}
			return (to);
		}

		/// <summary></summary>
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
			StringWriter to = new StringWriter(CultureInfo.InvariantCulture);
			foreach (RawElement re in ToElements())
			{
				to.Write(re.ToString(indent));
			}
			return (to.ToString());
		}

		/// <summary></summary>
		public virtual string QueueToDetailedString()
		{
			return (QueueToDetailedString(""));
		}

		/// <summary></summary>
		public virtual string QueueToDetailedString(string indent)
		{
			StringBuilder sb = new StringBuilder();

			int i = 0;
			foreach (RawElement re in ToElements())
			{
				sb.Append(indent + "> RawElement#" + (i++) + ":" + Environment.NewLine);
				sb.Append(re.ToDetailedString(indent + "   "));
			}

			if (i == 0)
				sb.AppendLine(indent + "<NONE>");

			return (sb.ToString());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
