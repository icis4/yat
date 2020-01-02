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
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY
{
	/// <summary>
	/// Wrapper class that wraps a 'MarshalByValObject' type to <see cref="MarshalByRefObject"/>.
	/// </summary>
	/// <remarks>
	/// Based on https://stackoverflow.com/questions/22766549/cross-appdomain-cancelable-event.
	/// </remarks>
	public class MarshalByRefWrapper<T> : MarshalByRefObject
	{
		/// <summary>
		/// Gets the object that has been wrapped.
		/// </summary>
		public T Obj { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MarshalByRefWrapper{T}"/> class.
		/// </summary>
		public MarshalByRefWrapper()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MarshalByRefWrapper{T}"/> class.
		/// </summary>
		/// <param name="obj">The object to be wrapped.</param>
		public MarshalByRefWrapper(T obj)
		{
			Obj = obj;
		}

		/// <summary>
		/// Performs an implicit conversion from <typeparamref name="T"/> to <see cref="MarshalByRefWrapper{T}"/>.
		/// </summary>
		/// <param name="obj">The object to be converted.</param>
		public static implicit operator MarshalByRefWrapper<T>(T obj)
		{
			return (new MarshalByRefWrapper<T>(obj));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
