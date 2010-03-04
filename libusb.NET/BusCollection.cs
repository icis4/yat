// BusCollection.cs
// Copyright (C) 2004 Mike Krueger
// 
// This program is free software. It is dual licensed under GNU GPL and GNU LGPL.
// See COPYING_GPL.txt and COPYING_LGPL.txt for details.
//

using System;
using System.Collections.Generic;

namespace libusb.NET
{
	/// <summary>
	/// A collection that stores <see cref='Bus'/> objects.
	/// </summary>
    /// <remarks>
    /// This class used to derive from <see cref="CollectionBase"/>. Since .NET 2.0, templates
    /// have been available. For backward compatibility this class wasn't renamed. It simply
    /// derives from <see cref="T:List`1"/> instead.
    /// </remarks>
    [Serializable()]
    public class BusCollection : List<Bus>
    {
	}
}
