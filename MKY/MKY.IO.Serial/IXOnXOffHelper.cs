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
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace MKY.IO.Serial
{
	/// <summary></summary>
	public class IXOnXOffHelper
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary>
		/// Input XOn/XOff reflects the XOn/XOff state of this serial port itself, i.e. this computer.
		/// </summary>
		private bool inputIsXOn;
		private object inputIsXOnSyncObj = new object();

		/// <summary>
		/// Output XOn/XOff reflects the XOn/XOff state of the communication counterpart, i.e. a device.
		/// </summary>
		private bool outputIsXOn;
		private object outputIsXOnSyncObj = new object();

		/// <remarks>
		/// In case of manual XOn/XOff, input is initialized to XOn.
		/// </remarks>
		private bool manualInputWasXOn = true;
		private object manualInputWasXOnSyncObj = new object();

		private int sentXOnCount;
		private int sentXOffCount;
		private int receivedXOnCount;
		private int receivedXOffCount;

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Gets the input XOn/XOff state.
		/// </summary>
		public virtual bool InputIsXOn
		{
			get
			{
				lock (this.inputIsXOnSyncObj)
					return (this.inputIsXOn);
			}
		}

		/// <summary>
		/// Gets or sets the output XOn/XOff state.
		/// </summary>
		public virtual bool OutputIsXOn
		{
			get
			{
				lock (this.outputIsXOnSyncObj)
					return (this.outputIsXOn);
			}
			set
			{
				lock (this.outputIsXOnSyncObj)
					this.outputIsXOn = value;
			}
		}

		/// <summary>
		/// Gets the manual input XOn/XOff state.
		/// </summary>
		public virtual bool ManualInputWasXOn
		{
			get
			{
				lock (this.manualInputWasXOnSyncObj)
					return (this.manualInputWasXOn);
			}
		}

		/// <summary>
		/// Returns the number of sent XOn bytes, i.e. the count of input XOn/XOff signaling.
		/// </summary>
		public virtual int SentXOnCount
		{
			get { return (this.sentXOnCount); }
		}

		/// <summary>
		/// Returns the number of sent XOff bytes, i.e. the count of input XOn/XOff signaling.
		/// </summary>
		public virtual int SentXOffCount
		{
			get { return (this.sentXOffCount); }
		}

		/// <summary>
		/// Returns the number of received XOn bytes, i.e. the count of output XOn/XOff signaling.
		/// </summary>
		public virtual int ReceivedXOnCount
		{
			get { return (this.receivedXOnCount); }
		}

		/// <summary>
		/// Returns the number of received XOff bytes, i.e. the count of output XOn/XOff signaling.
		/// </summary>
		public virtual int ReceivedXOffCount
		{
			get { return (this.receivedXOffCount); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Notify that an XOn or XOff byte has been sent.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if XOn/XOff state has changed, <c>false</c> if state remains.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and obvious.")]
		public virtual bool XOnOrXOffSent(byte b)
		{
			if (b == XOnXOff.XOnByte)
				return (XOnSent());

			if (b == XOnXOff.XOffByte)
				return (XOffSent());

			return (true);
		}

		/// <summary>
		/// Notify that an XOn byte has been sent.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if XOn/XOff state has changed, <c>false</c> if state remains.
		/// </returns>
		public virtual bool XOnSent()
		{
			bool hasChanged = false;

			lock (this.inputIsXOnSyncObj)
			{
				if (BooleanEx.SetIfCleared(ref this.inputIsXOn))
					hasChanged = true;

				lock (this.manualInputWasXOnSyncObj)
					this.manualInputWasXOn = true;
			}

			Interlocked.Increment(ref this.sentXOnCount);

			return (hasChanged);
		}

		/// <summary>
		/// Notify that an XOff byte has been sent.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if XOn/XOff state has changed, <c>false</c> if state remains.
		/// </returns>
		public virtual bool XOffSent()
		{
			bool hasChanged = false;

			lock (this.inputIsXOnSyncObj)
			{
				if (BooleanEx.ClearIfSet(ref this.inputIsXOn))
					hasChanged = true;

				lock (this.manualInputWasXOnSyncObj)
					this.manualInputWasXOn = false;
			}

			Interlocked.Increment(ref this.sentXOffCount);

			return (hasChanged);
		}

		/// <summary>
		/// Notify that an XOn or XOff byte has been received.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if XOn/XOff state has changed, <c>false</c> if state remains.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and obvious.")]
		public virtual bool XOnOrXOffReceived(byte b)
		{
			if (b == XOnXOff.XOnByte)
				return (XOnReceived());

			if (b == XOnXOff.XOffByte)
				return (XOffReceived());

			return (true);
		}

		/// <summary>
		/// Notify that an XOn byte has been received.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if XOn/XOff state has changed, <c>false</c> if state remains.
		/// </returns>
		public virtual bool XOnReceived()
		{
			bool hasChanged = false;

			lock (this.outputIsXOnSyncObj)
			{
				if (BooleanEx.SetIfCleared(ref this.outputIsXOn))
					hasChanged = true;
			}

			Interlocked.Increment(ref this.receivedXOnCount);

			return (hasChanged);
		}

		/// <summary>
		/// Notify that an XOff byte has been received.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if XOn/XOff state has changed, <c>false</c> if state remains.
		/// </returns>
		public virtual bool XOffReceived()
		{
			bool hasChanged = false;

			lock (this.outputIsXOnSyncObj)
			{
				if (BooleanEx.ClearIfSet(ref this.outputIsXOn))
					hasChanged = true;
			}

			Interlocked.Increment(ref this.receivedXOffCount);

			return (hasChanged);
		}

		/// <summary>
		/// Resets the XOn/XOff signaling count.
		/// </summary>
		public virtual void ResetCounts()
		{
			Interlocked.Exchange(ref this.sentXOnCount, 0);
			Interlocked.Exchange(ref this.sentXOffCount, 0);
			Interlocked.Exchange(ref this.receivedXOnCount, 0);
			Interlocked.Exchange(ref this.receivedXOffCount, 0);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
