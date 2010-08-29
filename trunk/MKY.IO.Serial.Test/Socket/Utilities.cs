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
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NUnit.Framework;

using MKY.IO.Serial;

namespace MKY.IO.Serial.Test.Socket
{
	/// <summary></summary>
	public static class Utilities
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const int Interval = 100;
		private const int Timeout = 10000;

		#endregion

		#region Wait
		//==========================================================================================
		// Wait
		//==========================================================================================

		internal static void WaitForStart(IO.Serial.IIOProvider io, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(Interval);
				timeout += Interval;

				if (timeout >= Timeout)
					Assert.Fail(message);
			}
			while (!io.IsStarted);
		}

		internal static void WaitForConnect(IO.Serial.IIOProvider ioA, IO.Serial.IIOProvider ioB, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(Interval);
				timeout += Interval;

				if (timeout >= Timeout)
					Assert.Fail(message);
			}
			while (!ioA.IsConnected && !ioB.IsConnected);
		}

		internal static void WaitForDisconnect(IO.Serial.IIOProvider ioA, IO.Serial.IIOProvider ioB, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(Interval);
				timeout += Interval;

				if (timeout >= Timeout)
					Assert.Fail(message);
			}
			while (ioA.IsConnected || ioB.IsConnected);
		}

		internal static void WaitForStop(IO.Serial.IIOProvider io, string message)
		{
			int timeout = 0;
			do
			{
				Thread.Sleep(Interval);
				timeout += Interval;

				if (timeout >= Timeout)
					Assert.Fail(message);
			}
			while (io.IsStarted);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
