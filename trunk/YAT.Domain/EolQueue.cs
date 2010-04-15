//==================================================================================================
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
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// EolQueue evaluates whether a sequence of bytes matches the given end-of-line sequence.
	/// </summary>
	/// <remarks>
	/// Implementation used to evaluate match on request only. However, when adding
	/// <see cref="IsPartlyMatch"/>, evaluation was moved to <see cref="Enqueue"/> and enqueue
	/// was optimized. This implementation is far more performant if properties are regularly
	/// read.
	/// </remarks>
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

		private byte[] eol;
		private Queue<byte> queue;
		private State state;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public EolQueue(byte[] eol)
		{
			this.eol = eol;
			this.queue = new Queue<byte>(this.eol.Length);
			Evaluate();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual byte[] Eol
		{
			get { return (this.eol); }
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
					if (b == this.eol[0])   // Start of EOL detected
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
			if (this.eol.Length <= 0)       // Empty EOL => Inactive
			{
				this.state = State.Inactive;
				return;
			}
			
			if (this.queue.Count <= 0)     // Empty queue => Armed
			{
				this.state = State.Armed;
				return;
			}

			// Precondition:
			// - EOL >= 1
			// - Queue >= 1

			// \fixme 2010-04-01 / mky
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
						if (queue[i] == this.eol[i])
						{
							if (i < (this.eol.Length - 1))
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
			catch (Exception ex)
			{
				MKY.Utilities.Diagnostics.XDebug.WriteException(this, ex);
				System.Diagnostics.Debug.WriteLine("Queue.Count = " + this.queue.Count);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
