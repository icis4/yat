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
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using MKY.Xml;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing XML writer functionality for YAT.
	/// </summary>
	public static class XmlHelper
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static void SchemaToFile(Type type, string directory, string fileName)
		{
			var document = XmlDocumentEx.CreateDefaultDocument(type);
			int n = document.Schemas.Schemas().Count;
			int i = 0;
			foreach (XmlSchema schema in document.Schemas.Schemas())
			{
				string filePath;
				if (n <= 1)
					filePath = directory + Path.DirectorySeparatorChar + fileName + ".xsd";
				else
					filePath = directory + Path.DirectorySeparatorChar + fileName + "-" + i + ".xsd";

				using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
				{
					schema.Write(sw);
				}

				i++;
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
