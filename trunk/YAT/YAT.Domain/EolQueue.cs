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
// YAT 2.0 Gamma 2 Development Version 1.99.35
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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace YAT.Domain
{
	/// <summary>
	/// EolQueue evaluates whether a sequence of bytes matches the given end-of-line sequence.
	/// </summary>
	/// <remarks>
	/// Implementation used to evaluate match on request only. However, when adding
	/// <see cref="IsPartlyMatch"/>, evaluation was moved to <see cref="Enqueue"/> and enqueue
	/// was optimized. This implementation is far better performing if properties are regularly
	/// read.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "This class indeed implements a queue, but not using inheritance.")]
	public class EolQueue
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum State
		{
			Inactive,
			Armed,
			PartlyMatch,
			CompleteMatch
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private ReadOnlyCollection<byte> eolSequence;
		private Queue<byte> queue;
		private State state;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public EolQueue(byte[] eolSequence)
		{
			this.eolSequence = new ReadOnlyCollection<byte>(eolSequence);
			this.queue = new Queue<byte>(this.eolSequence.Count);
			Evaluate();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual ReadOnlyCollection<byte> EolSequence
		{
			get { return (this.eolSequence); }
		}

		/// <summary></summary>
		public virtual bool IsPartlyMatch
		{
			get { return ( this.state == State.PartlyMatch || this.state == State.CompleteMatch ); }
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
				case State.Armed:       // EOL not started yet
				{
					if (b == this.eolSequence[0])   // Start of EOL detected
					{
						this.queue.Enqueue(b);
						Evaluate();
					}
					break;
				}

				case State.PartlyMatch: // EOL already started
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
			if (this.eolSequence.Count <= 0) // Empty EOL => Inactive.
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
			// - EOL >= 1
			// - Queue >= 1

			// \fixme (2010-04-01 / MKY):
			// Weird InvalidOperationException when receiving large chunks of data.
			try
			{
				// Evaluate EOL until there is either a match or no match.
				// Covers cases like <CR><CR><LF>.
				State evaluatedState = State.Armed;
				while ((evaluatedState == State.Armed) && (this.queue.Count > 0))
				{
					byte[] queue = this.queue.ToArray();
					for (int i = 0; i < queue.Length; i++)
					{
						if (queue[i] == this.eolSequence[i])
						{
							if (i < (this.eolSequence.Count - 1))
								evaluatedState = State.PartlyMatch;
							else
								evaluatedState = State.CompleteMatch;
						}
						else
						{
							this.queue.Dequeue(); // dequeue one element, then retry
							evaluatedState = State.Armed;
							break;
						}
					}
				}
				this.state = evaluatedState;
			}
			catch (InvalidOperationException ex)
			{
				MKY.Diagnostics.DebugEx.WriteException(GetType(), ex, "Queue.Count = " + this.queue.Count);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
