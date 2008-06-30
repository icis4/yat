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

			// create an empty object tree of the type to be able to serialize it afterwards
			object obj = _type.GetConstructor(new System.Type[] { }).Invoke(new object[] { });

			// serialize the empty object tree into a string
			// unlike file serialization, this string serialization will be UTF-16 encoded
			StringBuilder sb = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(sb);
			XmlSerializer serializer = new XmlSerializer(type);
			serializer.Serialize(writer, obj);

			// load that string into an XML document that serves as base for new documents
			_defaultDocument = new XmlDocument();
			_defaultDocument.LoadXml(sb.ToString());
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
			// reads input stream
			XmlDocument inputDocument = CreateDocumentFromInput(reader);

			// create output document from default
			XmlDocument outputDocument = new XmlDocument();
			outputDocument.LoadXml(_defaultDocument.InnerXml);

			// recursively traverse documents node-by-node and copy compatible nodes
			CopyDocumentTolerantly(inputDocument, outputDocument);

			// create object tree from output document
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
		/// Recursively traverses documents node-by-node and copies compatible nodes.
		/// </summary>
		private void CopyDocumentTolerantly(XmlDocument inputDocument, XmlDocument outputDocument)
		{
			// retrieve and activate schema within document
			XmlSchema inputSchema = InferSchemaFromXml(inputDocument.OuterXml);
			inputDocument.Schemas.Add(inputSchema);
			inputDocument.Validate(null);

			#region Disabled Debug Output
			//--------------------------------------------------------------------------------------
			#if false
			using (StreamWriter sw = new StreamWriter(@"c:\" + GetType() + ".CopyDocumentTolerantly-InputSchema.xsd"))
			{
				inputSchema.Write(sw);
			}
			#endif
			//--------------------------------------------------------------------------------------
			#endregion

			// retrieve and reset navigator
			XPathNavigator inputNavigator = inputDocument.CreateNavigator();
			inputNavigator.MoveToRoot();

			// retrieve and activate schema within document
			XmlSchema outputSchema = InferSchemaFromXml(outputDocument.OuterXml);
			outputDocument.Schemas.Add(outputSchema);
			outputDocument.Validate(null);

			#region Disabled Debug Output
			//--------------------------------------------------------------------------------------
			#if false
			using (StreamWriter sw = new StreamWriter(@"c:\" + GetType() + ".CopyDocumentTolerantly-OutputSchema.xsd"))
			{
				outputSchema.Write(sw);
			}
			#endif
			//--------------------------------------------------------------------------------------
			#endregion

			// retrieve and reset navigator
			XPathNavigator outputNavigator = outputDocument.CreateNavigator();
			outputNavigator.MoveToRoot();

			// copy recursivly
			CopyTolerantly(inputNavigator, outputNavigator);
		}

		private XmlSchema InferSchemaFromXml(string xmlString)
		{
			XmlSchemaSet schemaSet = new XmlSchemaSet();
			XmlSchemaInference inference = new XmlSchemaInference();
			using (StringReader sr = new StringReader(xmlString))
			{
				XmlReader reader = XmlReader.Create(sr);
				schemaSet = inference.InferSchema(reader);
			}

			// return first schema
			foreach (XmlSchema s in schemaSet.Schemas())
				return (s);

			return (null);
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
		/// Tries to copy the value if both navigators are pointing to an element with the same type.
		/// </summary>
		private bool TryToCopyValue(XPathNavigator inputNavigator, XPathNavigator outputNavigator)
		{
			XmlSchemaType inputType = inputNavigator.SchemaInfo.SchemaType;
			XmlSchemaType outputType = outputNavigator.SchemaInfo.SchemaType;

			// if necessary, navigate to parent to retrieve type
			// must always be done on both to ensure comparing apples with apples
			if ((inputType == null) || (outputType == null))
			{
				XPathNavigator parentNavigator;

				parentNavigator = inputNavigator.Clone();
				parentNavigator.MoveToParent();
				inputType = parentNavigator.SchemaInfo.SchemaType;

				parentNavigator = outputNavigator.Clone();
				parentNavigator.MoveToParent();
				outputType = parentNavigator.SchemaInfo.SchemaType;
			}

			// copy value if type fits
			if (inputType == outputType)
			{
				outputNavigator.SetValue(inputNavigator.Value);
				return (true);
			}
			return (false);
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

		/// <summary>
		/// Creates and returns object tree from document.
		/// </summary>
		private object CreateObjectTreeFromDocument(XmlDocument document)
		{
			// save the resulting document into a string
			// unlike file serialization, this string serialization will be UTF-16 encoded
			StringBuilder sb = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(sb);
			document.Save(writer);

			// deserialize that string into an object tree
			StringReader sr = new StringReader(sb.ToString());
			XmlSerializer serializer = new XmlSerializer(_type);
			return (serializer.Deserialize(sr));
		}

		#endregion
	}
}
