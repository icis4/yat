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
// MKY Version 1.0.30
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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

// Choose whether to write input and output documents and schemas to files:
// - Uncomment to write files
// - Comment out for normal operation
//#define WRITE_DOCUMENTS_TO_FILES
//#define WRITE_SCHEMAS_TO_FILES

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

using MKY.Xml.Schema;

#endregion

namespace MKY.Xml.Serialization
{
	/// <summary>
	/// Provides methods to treat XML serialization more tolerantly than <see cref="XmlSerializer"/>.
	/// </summary>
	public class TolerantXmlSerializer
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Type type;
		private XmlDocument defaultDocument;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TolerantXmlSerializer(Type type)
		{
			this.type = type;
			this.defaultDocument = XmlDocumentEx.CreateDefaultDocument(type, XmlSchemaEx.GuidSchema); // GUID extension, for details see "GuidSchema".

		#if (WRITE_SCHEMAS_TO_FILES)
			WriteSchemasToFiles(this.defaultDocument.Schemas, "DefaultSchema");
		#endif
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Deserializes the XML document from by the specified input stream. This implementation
		/// is more tolerant regarding the XML structure than
		/// <see cref="XmlSerializer.Deserialize(Stream)"/>.
		/// </summary>
		/// <remarks>
		/// For example, it allows that XML elements in the input have a different schema type than
		/// the required output type. This is pretty handy if a settings file slightly changes over
		/// time, e.g. an enumerated settings value is replaced by an integer value. In such case,
		/// <see cref="XmlSerializer.Deserialize(Stream)"/> throws an exception. In contrast, this
		/// implementation handles the mismatch by simply setting the new value to its default.
		/// </remarks>
		public object Deserialize(XmlReader reader)
		{
			// Read input stream.
			XmlDocument inputDocument = XmlDocumentEx.LoadFromReader(reader);

		#if (WRITE_DOCUMENTS_TO_FILES)
			WriteDocumentToFile(inputDocument, "InputDocument");
		#endif

			// Retrieve and activate schema within document.
			using (var sr = new StringReader(inputDocument.OuterXml))
			{
				using (var innerReader = XmlReader.Create(sr))
				{
					var inference = new XmlSchemaInference();
					inputDocument.Schemas = inference.InferSchema(innerReader);
					inputDocument.Validate(null);
				}
			}

		#if (WRITE_SCHEMAS_TO_FILES)
			WriteSchemasToFiles(inputDocument.Schemas, "InputSchema");
		#endif

			// Create output document from default.
			var outputDocument = new XmlDocument();
			outputDocument.LoadXml(this.defaultDocument.InnerXml);
			outputDocument.Schemas = this.defaultDocument.Schemas;
			outputDocument.Validate(null);

		#if (WRITE_SCHEMAS_TO_FILES)
			WriteSchemasToFiles(outputDocument.Schemas, "OutputSchema");
		#endif

			// Recursively traverse documents node-by-node and copy compatible nodes.
			CopyDocumentTolerantly(inputDocument, outputDocument);

		#if (WRITE_DOCUMENTS_TO_FILES)
			WriteDocumentToFile(outputDocument, "OutputDocument");
		#endif

			// Create object tree from output document.
			return (XmlDocumentEx.SaveToObjectTree(outputDocument, this.type));
		}

		#endregion

		#region Protected Methods
		//==========================================================================================
		// Protected Methods
		//==========================================================================================

		/// <summary>
		/// Tries to match a given input attribute to the given output attribute.
		/// </summary>
		/// <remarks>
		/// <see cref="CopyTolerantly"/> on why input must be matched to output and not vice-versa.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Required for recursion.")]
		protected virtual bool TryToMatchAttribute(XPathNavigator inputNavigator, ref XPathNavigator outputNavigator)
		{
			return (outputNavigator.MoveToAttribute(inputNavigator.LocalName, inputNavigator.NamespaceURI));
		}

		/// <summary>
		/// Tries to match a given input child to the given output child.
		/// </summary>
		/// <remarks>
		/// <see cref="CopyTolerantly"/> on why input must be matched to output and not vice-versa.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Required for recursion.")]
		protected virtual bool TryToMatchChild(XPathNavigator inputNavigator, ref XPathNavigator outputNavigator)
		{
			return (outputNavigator.MoveToChild(inputNavigator.LocalName, inputNavigator.NamespaceURI));
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <summary>
		/// Recursively traverses documents node-by-node and copies compatible nodes.
		/// </summary>
		private void CopyDocumentTolerantly(XmlDocument inputDocument, XmlDocument outputDocument)
		{
			// Retrieve and reset navigator.
			XPathNavigator inputNavigator = inputDocument.CreateNavigator();
			inputNavigator.MoveToRoot();

			// Retrieve and reset navigator.
			XPathNavigator outputNavigator = outputDocument.CreateNavigator();
			outputNavigator.MoveToRoot();

			// Copy recursivly.
			CopyTolerantly(inputNavigator, outputNavigator);
		}

		/// <summary>
		/// Recursively traverses nodes and tries to copy elements that have the same name.
		/// </summary>
		/// <remarks>
		/// This method assumes that both navigators are pointing to an XML node with the same name.
		///
		/// Important:
		/// Input navigator must be used as the primary navigator to ensure that all elements of
		/// the input are traversed, including those that are not available/filled on an empty
		/// XML tree. This is i.e. the case for arrays which are empty by default.
		/// </remarks>
		private void CopyTolerantly(XPathNavigator inputNavigator, XPathNavigator outputNavigator)
		{
			if (inputNavigator.IsNode)
			{
				switch (inputNavigator.NodeType)
				{
					case XPathNodeType.Attribute:
					{
						// Attributes are handled by TryToCopyValue().
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'Attribute' must be handled by TryToCopyValue()!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}

					case XPathNodeType.Root:
					case XPathNodeType.Element:
					{
						// Process attributes of the element.
						if (inputNavigator.HasAttributes)
						{
							if (outputNavigator.HasAttributes)
							{
								if (inputNavigator.MoveToFirstAttribute())
								{
									do
									{
										// Clone output navigator to keep current node level.
										XPathNavigator outputNavigatorClone = outputNavigator.Clone();

										// In case of an attribute with the same name, try to copy.
										if (TryToMatchAttribute(inputNavigator, ref outputNavigatorClone))
										{
											if (TryToCopyValue(inputNavigator, outputNavigatorClone))
												break; // Immediately break on successful copy.
										}
									}
									while (inputNavigator.MoveToNextAttribute());
								}
							}
							else
							{
								// In case of <c>null</c> values and sequences, input may contain
								// attributes that output doesn't, then try to copy complete node.
								TryToCopyNode(inputNavigator.Clone(), outputNavigator.Clone());
							}
						}

						// Process children of the element.
						if (inputNavigator.HasChildren)
						{
							if (outputNavigator.HasChildren)
							{
								if (inputNavigator.MoveToFirstChild())
								{
									// Check whether this node ends here.
									if (inputNavigator.LocalName.Length == 0)
									{
										// In case both nods end here, copy the value.
										if (outputNavigator.MoveToFirstChild())
										{
											if (outputNavigator.LocalName.Length == 0)
												CopyTolerantly(inputNavigator.Clone(), outputNavigator.Clone());
										}
									}
									else
									{
										do
										{
											// Clone output navigator to keep current node level.
											XPathNavigator outputNavigatorClone = outputNavigator.Clone();

											// In case of a child with the same name, recurse.
											if (TryToMatchChild(inputNavigator, ref outputNavigatorClone))
												CopyTolerantly(inputNavigator.Clone(), outputNavigatorClone);
										}
										while (inputNavigator.MoveToNext());
									}
								}
							}
							else
							{
								// In case of <c>null</c> values and sequences, input may contain
								// children that output doesn't, then try to copy complete node.
								TryToCopyNode(inputNavigator, outputNavigator);
							}
						}

						break;
					} // Root || Element.

					case XPathNodeType.Namespace:
					case XPathNodeType.ProcessingInstruction:
					{
						// Don't care.
						break;
					}

					default: // Covers comments, text and whitespaces.
					{
						TryToCopyValue(inputNavigator, outputNavigator);
						break;
					}
				} // switch (NodeType)
			} // if (IsNode)
		}

		/// <summary>
		/// Tries to copy the value if both navigators are pointing to an element with a compatible type.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private static bool TryToCopyValue(XPathNavigator inputNavigator, XPathNavigator outputNavigator)
		{
			// Navigate to parents to set typed value.
			XPathNavigator outputParentNavigator;
			outputParentNavigator = outputNavigator.Clone();
			outputParentNavigator.MoveToParent();

			XPathNavigator inputParentNavigator;
			inputParentNavigator = inputNavigator.Clone();
			inputParentNavigator.MoveToParent();

			try
			{
				outputParentNavigator.SetTypedValue(inputParentNavigator.TypedValue);
				return (true);
			}
			catch
			{
				return (false);
			}
		}

		/// <summary>
		/// Tries to copy a complete node.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private static bool TryToCopyNode(XPathNavigator inputNavigator, XPathNavigator outputNavigator)
		{
			try
			{
				using (XmlReader reader = inputNavigator.ReadSubtree())
				{
					outputNavigator.ReplaceSelf(reader);
					return (true);
				}
			}
			catch
			{
				return (false);
			}
		}

	#if (WRITE_SCHEMAS_TO_FILES)
		private void WriteSchemasToFiles(XmlSchemaSet schemas, string name)
		{
			=> Migrate to XmlSchemaEx.WriteToFile() when using this code the next time.

			int n = schemas.Schemas().Count;
			int i = 0;
			foreach (var schema in schemas.Schemas())
			{
				string filePath;
				if (n <= 1)
					filePath = MKY.IO.Temp.MakeTempFilePath(this.type, name, ".xsd");
				else
					filePath = MKY.IO.Temp.MakeTempFilePath(this.type, name + "-" + i, ".xsd";

				using (var sw = new StreamWriter(filePath, false, EncodingEx.EnvironmentRecommendedUTF8Encoding))
				{
					schema.Write(sw);
				}
				System.Diagnostics.Trace.WriteLine
				(
					"For development purposes, schema written to" + Environment.NewLine +
					@"""" + filePath + @""""
				);
				i++;
			}
		}
	#endif

	#if (WRITE_DOCUMENTS_TO_FILES)
		private void WriteDocumentToFile(XmlDocument document, string name)
		{
			=> Migrate to XmlDocumentEx.WriteToFile() when using this code the next time.
			=> Attention! No way found to preserve whitespace in XML content when writing a type-unspecified XML document! See XmlDocumentEx.WriteToFile() for details!

			string filePath = MKY.IO.Temp.MakeTempFilePath(this.type, name, ".xml");
			using (var sw = new StreamWriter(filePath, false, EncodingEx.EnvironmentRecommendedUTF8Encoding))
			{
				document.Save(sw);
			}
			System.Diagnostics.Trace.WriteLine
			(
				"For development purposes, document written to" + Environment.NewLine +
				@"""" + filePath + @""""
			);
		}
	#endif

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
