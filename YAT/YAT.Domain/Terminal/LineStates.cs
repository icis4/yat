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
// Copyright © 2007-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	public class LineState
	{
		/// <summary></summary>
		public LinePosition             Position  { get; set; }

		/// <summary></summary>
		public DisplayElementCollection Elements  { get; set; }

		/// <summary></summary>
		public DateTime                 TimeStamp { get; set; }

		/// <summary></summary>
		public string                   Device    { get; set; }

		/// <summary></summary>
		public LineState()
		{
			Reset();
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			Position  = LinePosition.Begin;
			Elements  = new DisplayElementCollection(DisplayElementCollection.TypicalNumberOfElementsPerLine); // Preset the typical capacity to improve memory management.
			TimeStamp = DateTime.Now;
			Device    = null;
		}
	}

	/// <summary></summary>
	public class DeviceAndDirectionLineState
	{
		/// <summary></summary>
		public bool        IsFirstChunk { get; set; }

		/// <summary></summary>
		public string      Device       { get; set; }

		/// <summary></summary>
		public IODirection Direction    { get; set; }

		/// <summary></summary>
		public DeviceAndDirectionLineState()
		{
			Reset();
		}

		/// <summary></summary>
		public void Reset()
		{
			IsFirstChunk = true;
			Device       = null;
			Direction    = IODirection.None;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
