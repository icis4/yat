//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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
	/// Queue evaluates whether a sequence of bytes matches the given sequence. Can be used to e.g.
	/// detect end-of-line sequences.
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

		private ReadOnlyCollection<byte> sequence;
		private Queue<byte> queue;
		private State state;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SequenceQueue(byte[] sequence)
			: this(new ReadOnlyCollection<byte>(sequence))
		{
		}

		/// <summary></summary>
		public SequenceQueue(ReadOnlyCollection<byte> sequence)
		{
			this.sequence = sequence;
			this.queue = new Queue<byte>(this.sequence.Count);
			Evaluate();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual ReadOnlyCollection<byte> Sequence
		{
			get { return (this.sequence); }
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
		public virtual bool IsCompleteMatch
		{
			get { return (this.state == State.CompleteMatch); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Enqueue(byte b)
		{
			// Reset queue if it previously matched
			if (this.state == State.CompleteMatch)
				Reset();

			// Enqueue incoming byte according to state
			switch (this.state)
			{
				case State.Armed:       // Sequence not started yet
				{
					if (b == this.sequence[0])   // Start of sequence detected
					{
						this.queue.Enqueue(b);
						Evaluate();
					}
					break;
				}

				case State.PartlyMatchBeginning: // Sequence already started
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

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void Evaluate()
		{
			if (this.sequence.Count <= 0) // Empty sequence => Inactive.
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
				for (int i = 0; ((i < queue.Length) &&  (i < this.sequence.Count)); i++)
				{
					if (queue[i] == this.sequence[i])
					{
						if (this.sequence.Count == 1)
						{
							evaluatedState = State.CompleteMatch;
						}
						else // this.sequence.Count >= 2
						{
							if (i == 0)
								evaluatedState = State.PartlyMatchBeginning;
							else if (i < (this.sequence.Count - 1))
								evaluatedState = State.PartlyMatchContinued;
							else // i == (this.sequence.Count - 1)
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
