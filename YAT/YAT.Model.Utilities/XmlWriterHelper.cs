﻿//==================================================================================================
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using MKY.Xml;
using MKY.Xml.Serialization;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing XML writer functionality for YAT.
	/// </summary>
	public static class XmlWriterHelper
	{
		/// <summary></summary>
		public static void LinesToFile(List<Domain.DisplayLine> lines, string filePath, bool addSchema)
		{
			Type type = typeof(List<Domain.DisplayLine>);
			XmlSerializerEx.SerializeToFile(filePath, type, lines);

			if (addSchema)
				SchemaToFile(type, Path.GetFullPath(filePath), Path.GetFileNameWithoutExtension(filePath));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static void SchemaToFile(Type type, string path, string fileName)
		{
			XmlDocument document = XmlDocumentEx.CreateDefaultDocument(type);
			int n = document.Schemas.Schemas().Count;
			int i = 0;
			foreach (XmlSchema schema in document.Schemas.Schemas())
			{
				string filePath;
				if (n <= 1)
					filePath = path + fileName + ".xsd";
				else
					filePath = path + fileName + "-" + i + ".xsd";

				using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
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
