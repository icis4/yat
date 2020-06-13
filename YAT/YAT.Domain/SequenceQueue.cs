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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace YAT.Domain
{
	/// <summary>
	/// Queue evaluates whether a sequence of bytes matches the given sequence.
	/// Can be used to e.g. detect EOL (end-of-line) sequences.
	/// </summary>
	/// <remarks>
	/// Implementation used to evaluate match on request only. However, when adding 'IsPartlyMatch'
	/// (predecessor of <see cref="IsPartlyMatchBeginning"/> and <see cref="IsPartlyMatchContinued"/>,
	/// evaluation was moved to <see cref="Enqueue"/> and enqueue was optimized. This implementation
	/// is far better performing if properties are regularly read.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This class indeed implements a queue, but not using inheritance.")]
	public class SequenceQueue
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum State
		{
			Inactive,
			Armed,
			PartlyMatchBeginning,
			PartlyMatchContinued,
			CompleteMatch
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <remarks>
		/// "Guidelines for Collections": "Do use byte arrays instead of collections of bytes."
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private byte[] sequence;

		private Queue<byte> queue;
		private State state;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SequenceQueue(byte[] sequence)
		{
			this.sequence = sequence;
			this.queue = new Queue<byte>(this.sequence.Length);
			Evaluate();
		}

	#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~SequenceQueue()
		{
			MKY.Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

	#endif // DEBUG

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public virtual byte[] Sequence
		{
			get { return (this.sequence); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public virtual byte[] QueueAsArray()
		{
			return (this.queue.ToArray());
		}

		/// <summary></summary>
		public virtual bool IsPartlyMatchBeginning
		{
			get { return (this.state == State.PartlyMatchBeginning); }
		}

		/// <summary></summary>
		public virtual bool IsPartlyMatchContinued
		{
			get { return (this.state == State.PartlyMatchContinued); }
		}

		/// <summary></summary>
		public virtual bool IsPartlyMatch
		{
			get
			{
				switch (this.state)
				{
					case State.PartlyMatchBeginning:
					case State.PartlyMatchContinued:
						return (true);

					case State.Armed:
					case State.Inactive:
					case State.CompleteMatch:
					default:
						return (false);
				}
			}
		}

		/// <summary></summary>
		public virtual bool IsCompleteMatch
		{
			get { return (this.state == State.CompleteMatch); }
		}

		/// <summary></summary>
		public virtual bool IsAnyMatch
		{
			get
			{
				switch (this.state)
				{
					case State.PartlyMatchBeginning:
					case State.PartlyMatchContinued:
					case State.CompleteMatch:
						return (true);

					case State.Armed:
					case State.Inactive:
					default:
						return (false);
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		public virtual void Enqueue(byte b)
		{
			// Reset queue if it previously matched:
			if (this.state == State.CompleteMatch)
				Reset();

			// Enqueue incoming byte according to state:
			switch (this.state)
			{
				case State.Armed: // Sequence not started yet.
				{
					if (b == this.sequence[0]) // Start of sequence detected.
					{
						this.queue.Enqueue(b);
						Evaluate();
					}
					break;
				}

				case State.PartlyMatchBeginning: // Sequence already started.
				case State.PartlyMatchContinued:
				{
					this.queue.Enqueue(b);
					Evaluate();
					break;
				}
			}
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			this.queue.Clear();
			Evaluate();
		}

		/// <summary>
		/// Creates and returns a new object that is a deep-copy of this instance.
		/// </summary>
		public virtual SequenceQueue Clone()
		{
			SequenceQueue clone = new SequenceQueue(this.sequence);

			clone.queue = new Queue<byte>(this.queue);
			clone.state = this.state;

			return (clone);
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void Evaluate()
		{
			if (this.sequence.Length <= 0) // Empty sequence => Inactive.
			{
				this.state = State.Inactive;
				return;
			}

			if (this.queue.Count <= 0) // Empty queue => Armed.
			{
				this.state = State.Armed;
				return;
			}

			// Precondition:
			//  > Sequence >= 1
			//  > Queue >= 1
			//
			// Evaluate sequence until there is either a match or no match.
			// Covers cases like <LF>, <LF><LF>, <CR><CR><LF>,...
			State evaluatedState = State.Armed;
			while ((evaluatedState == State.Armed) && (this.queue.Count > 0))
			{
				byte[] queue = this.queue.ToArray();
				for (int i = 0; ((i < queue.Length) &&  (i < this.sequence.Length)); i++)
				{
					if (queue[i] == this.sequence[i])
					{
						if (this.sequence.Length == 1)
						{
							evaluatedState = State.CompleteMatch;
						}
						else // this.sequence.Count >= 2
						{
							if (i == 0)
								evaluatedState = State.PartlyMatchBeginning;
							else if (i < (this.sequence.Length - 1))
								evaluatedState = State.PartlyMatchContinued;
							else // i == (this.sequence.Length - 1)
								evaluatedState = State.CompleteMatch;
						}
					}
					else
					{
						this.queue.Dequeue(); // Dequeue one element, then retry.
						evaluatedState = State.Armed;
						break;
					}
				}
			}
			this.state = evaluatedState;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
