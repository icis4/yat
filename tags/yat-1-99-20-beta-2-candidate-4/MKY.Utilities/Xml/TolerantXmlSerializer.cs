// Enable to write input and output documents and schemas to files
//#define WRITE_DOCUMENTS_TO_FILES
//#define WRITE_SCHEMAS_TO_FILES

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace MKY.Utilities.Xml
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

		private Type _type;
		private XmlDocument _defaultDocument;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TolerantXmlSerializer(Type type)
		{
			_type = type;

			// Create an empty object tree of the type to be able to serialize it afterwards
			object obj = _type.GetConstructor(new System.Type[] { }).Invoke(new object[] { });

			// Serialize the empty object tree into a string
			// Unlike file serialization, this string serialization will be UTF-16 encoded
			StringBuilder sb = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(sb);
			XmlSerializer serializer = new XmlSerializer(type);
			serializer.Serialize(writer, obj);

			// Load that string into an XML document that serves as base for new documents
			_defaultDocument = new XmlDocument();
			_defaultDocument.LoadXml(sb.ToString());

			// Retrieve default schema
			XmlReflectionImporter reflectionImporter = new XmlReflectionImporter();
			XmlTypeMapping typeMapping = reflectionImporter.ImportTypeMapping(type);
			XmlSchemas schemas = new XmlSchemas();
			XmlSchemaExporter schemaExporter = new XmlSchemaExporter(schemas);
			schemaExporter.ExportTypeMapping(typeMapping);

			// Set and compile default schema
			_defaultDocument.Schemas.Add(schemas[0]);
			_defaultDocument.Schemas.Compile();
			_defaultDocument.Validate(null);

		#if WRITE_SCHEMAS_TO_FILES
			WriteSchemasToFiles(_defaultDocument.Schemas, "DefaultSchema");
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
		/// time, e.g. an enummerated settings value is replaced by an integer value. In such case,
		/// <see cref="XmlSerializer.Deserialize(Stream)"/> throws an exception. In contrast, this
		/// implementation handles the mismatch by simply setting the new value to its default.
		/// </remarks>
		public object Deserialize(TextReader reader)
		{
			// Read input stream
			XmlDocument inputDocument = CreateDocumentFromInput(reader);

		#if WRITE_DOCUMENTS_TO_FILES
			WriteDocumentToFile(inputDocument, "InputDocument");
		#endif

			// Retrieve and activate schema within document
			inputDocument.Schemas = InferSchemaFromXml(inputDocument.OuterXml);
			inputDocument.Validate(null);

		#if WRITE_SCHEMAS_TO_FILES
			WriteSchemasToFiles(inputDocument.Schemas, "InputSchema");
		#endif

			// Create output document from default
			XmlDocument outputDocument = new XmlDocument();
			outputDocument.LoadXml(_defaultDocument.InnerXml);
			outputDocument.Schemas = _defaultDocument.Schemas;
			outputDocument.Validate(null);

		#if WRITE_SCHEMAS_TO_FILES
			WriteSchemasToFiles(outputDocument.Schemas, "OutputSchema");
		#endif

			// Recursively traverse documents node-by-node and copy compatible nodes
			CopyDocumentTolerantly(inputDocument, outputDocument);

		#if WRITE_DOCUMENTS_TO_FILES
			WriteDocumentToFile(outputDocument, "OutputDocument");
		#endif

			// Create object tree from output document
			return (CreateObjectTreeFromDocument(outputDocument));
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
		virtual protected bool TryToMatchAttribute(XPathNavigator inputNavigator, ref XPathNavigator outputNavigator)
		{
			return (outputNavigator.MoveToAttribute(inputNavigator.LocalName, inputNavigator.NamespaceURI));
		}

		/// <summary>
		/// Tries to match a given input child to the given output child.
		/// </summary>
		/// <remarks>
		/// <see cref="CopyTolerantly"/> on why input must be matched to output and not vice-versa.
		/// </remarks>
		virtual protected bool TryToMatchChild(XPathNavigator inputNavigator, ref XPathNavigator outputNavigator)
		{
			return (outputNavigator.MoveToChild(inputNavigator.LocalName, inputNavigator.NamespaceURI));
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		/// <summary>
		/// Reads XML input stream into a document.
		/// </summary>
		private XmlDocument CreateDocumentFromInput(TextReader inputReader)
		{
			XmlDocument document = new XmlDocument();
			using (XmlReader reader = XmlReader.Create(inputReader))
			{
				document.Load(reader);
			}
			return (document);
		}

		/// <summary>
		/// Creates and returns object tree from document.
		/// </summary>
		private object CreateObjectTreeFromDocument(XmlDocument document)
		{
			// Save the resulting document into a string
			// Unlike file serialization, this string serialization will be UTF-16 encoded
			StringBuilder sb = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(sb);
			document.Save(writer);

			// Deserialize that string into an object tree
			StringReader sr = new StringReader(sb.ToString());
			XmlSerializer serializer = new XmlSerializer(_type);
			return (serializer.Deserialize(sr));
		}

		/// <summary>
		/// Recursively traverses documents node-by-node and copies compatible nodes.
		/// </summary>
		private void CopyDocumentTolerantly(XmlDocument inputDocument, XmlDocument outputDocument)
		{
			// Retrieve and reset navigator
			XPathNavigator inputNavigator = inputDocument.CreateNavigator();
			inputNavigator.MoveToRoot();

			// Retrieve and reset navigator
			XPathNavigator outputNavigator = outputDocument.CreateNavigator();
			outputNavigator.MoveToRoot();

			// Copy recursivly
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
						// attributes are handled by TryToCopyValue()
						throw (new InvalidOperationException("Programm execution must never get here"));
					}

					case XPathNodeType.Root:
					case XPathNodeType.Element:
					{
						// process attributes of the element
						if (inputNavigator.HasAttributes)
						{
							if (outputNavigator.HasAttributes)
							{
								if (inputNavigator.MoveToFirstAttribute())
								{
									do
									{
										// clone output navigator to keep current node level
										XPathNavigator outputNavigatorClone = outputNavigator.Clone();

										// in case of an attribute with the same name, try to copy
										if (TryToMatchAttribute(inputNavigator, ref outputNavigatorClone))
										{
											if (TryToCopyValue(inputNavigator, outputNavigatorClone))
												break; // immediately break on successful copy
										}
									}
									while (inputNavigator.MoveToNextAttribute());
								}
							}
							// in case of null values and sequences, input may contain attributes
							// that output doesn't, then try to copy complete node
							else
							{
								TryToCopyNode(inputNavigator.Clone(), outputNavigator.Clone());
							}
						}

						// process children of the element
						if (inputNavigator.HasChildren)
						{
							if (outputNavigator.HasChildren)
							{
								if (inputNavigator.MoveToFirstChild())
								{
									// check whether this node ends here
									if (inputNavigator.LocalName == "")
									{
										// in case both nods end here, copy the value
										if (outputNavigator.MoveToFirstChild())
										{
											if (outputNavigator.LocalName == "")
												CopyTolerantly(inputNavigator.Clone(), outputNavigator.Clone());
										}
									}
									else
									{
										do
										{
											// clone output navigator to keep current node level
											XPathNavigator outputNavigatorClone = outputNavigator.Clone();

											// in case of a child with the same name, recurse
											if (TryToMatchChild(inputNavigator, ref outputNavigatorClone))
												CopyTolerantly(inputNavigator.Clone(), outputNavigatorClone);
										}
										while (inputNavigator.MoveToNext());
									}
								}
							}
							// in case of null values and sequences, input may contain children
							// that output doesn't, then try to copy complete node
							else
							{
								TryToCopyNode(inputNavigator, outputNavigator);
							}
						}

						break;
					} // Root || Element

					case XPathNodeType.Namespace:
					case XPathNodeType.ProcessingInstruction:
					{
						// don't care
						break;
					}

					default: // covers comments, text and white spaces
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
		private bool TryToCopyValue(XPathNavigator inputNavigator, XPathNavigator outputNavigator)
		{
			// Navigate to parents to set typed value
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
		/// <remarks>
		/// </remarks>
		private bool TryToCopyNode(XPathNavigator inputNavigator, XPathNavigator outputNavigator)
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

		private XmlSchemaSet InferSchemaFromXml(string xmlString)
		{
			using (StringReader sr = new StringReader(xmlString))
			{
				XmlReader reader = XmlReader.Create(sr);
				XmlSchemaInference inference = new XmlSchemaInference();
				return (inference.InferSchema(reader));
			}
		}

	#if WRITE_SCHEMAS_TO_FILES
		private void WriteSchemasToFiles(XmlSchemaSet schemas, string label)
		{
			int i = 0;
			foreach (XmlSchema schema in schemas.Schemas())
			{
				string filePath = @"C:\" + GetType() + "." + label + "-" + i + ".xsd";
				using (StreamWriter sw = new StreamWriter(filePath))
				{
					schema.Write(sw);
				}
				Console.WriteLine
				(
					"For development purposes, schema written to" + Environment.NewLine +
					@"""" + filePath + @""""
				);
				i++;
			}
		}
	#endif

	#if WRITE_DOCUMENTS_TO_FILES
		private void WriteDocumentToFile(XmlDocument document, string label)
		{
			string filePath = @"C:\" + GetType() + "." + label + ".xml";
			using (StreamWriter sw = new StreamWriter(filePath))
			{
				document.Save(sw);
			}
			Console.WriteLine
			(
				"For development purposes, document written to" + Environment.NewLine +
				@"""" + filePath + @""""
			);
		}
	#endif

		#endregion
	}
}
