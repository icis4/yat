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
// YAT Version 2.4.0
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using MKY;
using MKY.Collections;

#endregion

namespace YAT.Model.Types
{
	/// <summary></summary>
	[Serializable]
	public class PredefinedCommandPage : IEquatable<PredefinedCommandPage>, IComparable
	{
		/// <remarks>Commands and pages are numbered 1..max, 0 indicates none/invalid.</remarks>
		public const int NoCommandId = 0;

		/// <remarks>Commands and pages are numbered 1..max, 0 indicates none/invalid.</remarks>
		public const int FirstCommandIdPerPage = 1;

		/// <remarks>Limited to a) keep view simple for most use cases and b) ease implementation.</remarks>
		public const int CommandCapacityPerSubpage = 12;

		/// <remarks>F1..F12 are available, F13..F24 don't seem supported by Windows (any longer).</remarks>
		public const int CommandCapacityWithShortcut = 12;

		/// <remarks>Possible capacities are 12, 24, 36, 48, 72 or 108.</remarks>
		public const int MaxCommandCapacityPerPage = 108;

		/// <remarks>Subpages are numbered 1..9, 0 indicates none/invalid.</remarks>
		public const int NoSubpageId = 0;

		/// <remarks>Subpages are numbered 1..9, 0 indicates none/invalid.</remarks>
		public const int FirstSubpageId = 1;

		/// <remarks>Subpages are numbered 1..9, 0 indicates none/invalid.</remarks>
		public const int LastSubpageId = MaxSubpageCount;

		/// <remarks>Subpages are numbered 1..9, 0 indicates none/invalid.</remarks>
		public const int MaxSubpageCount = 9;

		private string nameIntegrated;
		private string nameLinked;

		private List<Command> commandsIntegrated;
		private List<Command> commandsLinked;

		private string linkFilePath;

		/// <summary></summary>
		public PredefinedCommandPage()
			: this (PredefinedCommandPageCollection.FirstPageNameDefault)
		{
		}

		/// <summary></summary>
		public PredefinedCommandPage(string pageName)
		{
			this.nameIntegrated = pageName;
		////this.nameLinked = null;

			this.commandsIntegrated = new List<Command>();
			this.commandsLinked     = new List<Command>(); // Even though typically not needed, same behavior as 'commandsIntegrated'.

		////this.linkFilePath = null;
		}

		/// <summary></summary>
		public PredefinedCommandPage(PredefinedCommandPage rhs)
		{
			this.nameIntegrated = rhs.nameIntegrated;
			this.nameLinked     = rhs.nameLinked;

			this.commandsIntegrated = new List<Command>(rhs.commandsIntegrated.Count); // Preset the required capacity to improve memory management.
			foreach (var c in rhs.commandsIntegrated)
				this.commandsIntegrated.Add(new Command(c)); // Clone to ensure decoupling.

			this.commandsLinked = new List<Command>(rhs.commandsLinked.Count); // Preset the required capacity to improve memory management.
			foreach (var c in rhs.commandsLinked)
				this.commandsLinked.Add(new Command(c)); // Clone to ensure decoupling.

			this.linkFilePath = rhs.linkFilePath;
		}

		/// <summary></summary>
		public PredefinedCommandPage(int capacity)
			: this (capacity, PredefinedCommandPageCollection.FirstPageNameDefault)
		{
		}

		/// <summary></summary>
		public PredefinedCommandPage(int capacity, string name)
		{
			this.nameIntegrated = name;
		////this.nameLinked = null;

			this.commandsIntegrated = new List<Command>(capacity);
			this.commandsLinked     = new List<Command>(capacity); // Even though typically not needed, same behavior as 'commandsIntegrated'.

		////this.linkFilePath = null;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlIgnore]
		public virtual int Capacity
		{
			get { return (Commands.Capacity); }
		}

		/// <remarks>
		/// \remind (2019-08-13..15 / MKY)
		/// Property was renamed from "PageName" to "Name" on 2019-08-13 for no longer replicating
		/// the class context in the the property name. However, detected on 2019-08-14 that this
		/// change is not properly handled by the XML deserialization infrastructure, thus reverted.
		/// On 2019-08-15 split into <see cref="NameIntegrated"/> and <see cref="NameLinked"/>.
		/// </remarks>
		[XmlElement("PageName")] // Backward compatibility! To be renamed when fixing bug #246.
		public virtual string NameIntegrated
		{
			get { return (this.nameIntegrated); }
			set { this.nameIntegrated = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string NameLinked
		{
			get { return (this.nameLinked); }
			set { this.nameLinked = value;  }
		}

		/// <summary>
		/// The name effectively in use, either <see cref="NameIntegrated"/>, or
		/// <see cref="NameLinked"/> in case <see cref="LinkFilePath"/> is defined.
		/// </summary>
		[XmlIgnore]
		public virtual string Name
		{
			get
			{
				if (IsLinkedToFilePath)
					return (this.nameLinked);
				else
					return (this.nameIntegrated);
			}
			set
			{
				if (IsLinkedToFilePath)
					this.nameLinked = value;
				else
					this.nameIntegrated = value;
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string Caption
		{
			get
			{
				// Attention:
				// Similar code exists in CaptionFallback() further below.
				// Changes here may have to be applied there too.

				if (IsLinkedToFilePath)
				{
					var sb = new StringBuilder(Name);

					sb.Append(" (linked to ");
					sb.Append(Path.GetFileName(LinkFilePath));
					sb.Append(")");

					return (sb.ToString());
				}
				else
				{
					return (Name);
				}
			}
		}

		/// <remarks>XML is named 'Commands' for simplicity/comprehensibility as well as backward compatibility.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("Commands")]
		public virtual List<Command> CommandsIntegrated
		{
			get { return (this.commandsIntegrated); }
			set { this.commandsIntegrated = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlIgnore]
		public virtual List<Command> CommandsLinked
		{
			get { return (this.commandsLinked); }
			set { this.commandsLinked = value;  }
		}

		/// <summary>
		/// The commands effectively in use, either <see cref="CommandsIntegrated"/>, or
		/// <see cref="CommandsLinked"/> in case <see cref="LinkFilePath"/> is defined.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlIgnore]
		public virtual List<Command> Commands
		{
			get
			{
				if (IsLinkedToFilePath)
					return (this.commandsLinked);
				else
					return (this.commandsIntegrated);
			}
			set
			{
				if (IsLinkedToFilePath)
					this.commandsLinked = value;
				else
					this.commandsIntegrated = value;
			}
		}

		/// <summary></summary>
		[XmlElement("LinkFilePath")]
		public virtual string LinkFilePath
		{
			get { return (this.linkFilePath); }
			set { this.linkFilePath = value;  }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsLinkedToFilePath
		{
			get { return (!string.IsNullOrEmpty(LinkFilePath)); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public int DefinedCommandCount
		{
			get
			{
				int n = 0;

				if (Commands != null)
				{
					foreach (var c in Commands)
					{
						if ((c != null) && (c.IsDefined))
							n++;
					}
				}

				return (n);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public static string CaptionFallback(PredefinedCommandPage page, int id)
		{
			// Attention:
			// Similar code exists in Caption{get} further above.
			// Changes here may have to be applied there too.

			var sb = new StringBuilder(PredefinedCommandPageCollection.StandardPageNamePrefix);
			sb.Append(id);

			if (page.IsLinkedToFilePath)
			{
				sb.Append(" (linked to ");
				sb.Append(Path.GetFileName(page.LinkFilePath));
				sb.Append(")");
			}

			return (sb.ToString());
		}

		/// <summary></summary>
		public static string CaptionOrFallback(PredefinedCommandPage page, int id)
		{
			if (!string.IsNullOrEmpty(page.Name)) // Name must be compared! Caption could be " (linked to ...)"!
				return (page.Caption);
			else
				return (CaptionFallback(page, id));
		}

		/// <param name="subpageId">Subpage ID <see cref="FirstSubpageId"/>..<see cref="LastSubpageId"/>.</param>
		public static string SubpageIdToString(int subpageId)
		{
			switch (subpageId)
			{
				case 1: return ( "1..12" );
				case 2: return ("13..24" );
				case 3: return ("25..36" );
				case 4: return ("37..48" );
				case 5: return ("49..64" );
				case 6: return ("65..72" );
				case 7: return ("73..84" );
				case 8: return ("85..96" );
				case 9: return ("97..108");

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + subpageId.ToString(CultureInfo.InvariantCulture) + "' is a subpage ID that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// Modifies the commands effectively in use, i.e. either <see cref="CommandsIntegrated"/>,
		/// or <see cref="CommandsLinked"/> in case <see cref="LinkFilePath"/> is defined.
		/// </remarks>
		/// <param name="index">Command index 0..(<see cref="MaxCommandCapacityPerPage"/> - 1).</param>
		/// <param name="command">Command to be set.</param>
		public virtual void SetCommand(int index, Command command)
		{
			if ((index < 0) || (index >= MaxCommandCapacityPerPage))
				throw (new ArgumentOutOfRangeException("index", index, MessageHelper.InvalidExecutionPreamble + "'" + index + "' is an invalid command index!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if ((command != null) && (command.IsDefinedOrHasDescription)) // Prevent dummy commands from being added to the list.
			{
				if (index < Commands.Count)
				{
					// Replace command:
					Commands[index] = new Command(command); // Clone to ensure decoupling.
				}
				else
				{
					// Fill-in empty commands:
					while (Commands.Count < (index))
						Commands.Add(new Command());

					// Add command:
					Commands.Add(new Command(command));
				}
			}
			else
			{
				ClearCommand(index);
			}
		}

		/// <remarks>
		/// Modifies the commands effectively in use, i.e. either <see cref="CommandsIntegrated"/>,
		/// or <see cref="CommandsLinked"/> in case <see cref="LinkFilePath"/> is defined.
		/// </remarks>
		/// <param name="index">Command index 0..(<see cref="MaxCommandCapacityPerPage"/> - 1).</param>
		public virtual void ClearCommand(int index)
		{
			if ((index < 0) || (index >= MaxCommandCapacityPerPage))
				throw (new ArgumentOutOfRangeException("index", index, MessageHelper.InvalidExecutionPreamble + "'" + index + "' is an invalid command index!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if (index < Commands.Count)
			{
				if (index != (Commands.Count - 1))
				{
					Commands[index].Clear();
				}
				else // 'index' is/was the last command:
				{
					Commands.RemoveAt(index);
					RemoveTrailingCommands();
				}
			}
		}

		/// <remarks>
		/// Modifies the commands effectively in use, i.e. either <see cref="CommandsIntegrated"/>,
		/// or <see cref="CommandsLinked"/> in case <see cref="LinkFilePath"/> is defined.
		/// </remarks>
		public virtual void RemoveTrailingCommands()
		{
			for (int i = (Commands.Count - 1); i >= 0; i--)
			{
				if ((Commands[i] == null) || (!Commands[i].IsDefinedOrHasDescription))
					Commands.RemoveAt(i);
				else
					break;
			}
		}

		/// <summary>
		/// Activates the link to a file.
		/// </summary>
		public virtual void Link(string filePath, string nameLinked, List<Command> commandsLinked)
		{
			NameLinked = nameLinked;
			CommandsLinked = commandsLinked;
			LinkFilePath = filePath;
		}

		/// <summary>
		/// Updates the items effectively in use, i.e. <see cref="Name"/> and <see cref="Commands"/>.
		/// </summary>
		public virtual void UpdateEffectivelyInUse(PredefinedCommandPage other)
		{
			Name = other.Name;

			Commands = new List<Command>(other.Commands.Count); // Preset the required capacity to improve memory management.
			foreach (var c in other.Commands)
				Commands.Add(new Command(c)); // Clone to ensure decoupling.

		////LinkFilePath = other.LinkFilePath shall not be modified as that determines what effectively is in use.
		}

		/// <summary>
		/// Clears the link to a file.
		/// </summary>
		public virtual void Unlink()
		{
			NameLinked = null;
			CommandsLinked = new List<Command>();
			LinkFilePath = null;
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = 0;
				                               //// Just comparing the name effectively in use.
				hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);

				if (Commands != null)
				{
					foreach (var c in Commands) // Just hashing the commands effectively in use.
						hashCode = (hashCode * 397) ^ c.GetHashCode();
				}

				hashCode = (hashCode * 397) ^ (LinkFilePath != null ? LinkFilePath.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as PredefinedCommandPage));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(PredefinedCommandPage other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
			////base.Equals(other) is not required when deriving from 'object'.

				StringEx          .EqualsOrdinal(Name,         other.Name)     && // Just comparing the name effectively in use.
				IEnumerableEx.ItemsEqual(        Commands,     other.Commands) && // Just comparing the commands effectively in use.
				StringEx          .EqualsOrdinal(LinkFilePath, other.LinkFilePath)
			);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality
		/// on the items effectively in use, i.e. <see cref="Name"/> and <see cref="Commands"/>.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool EqualsEffectivelyInUse(PredefinedCommandPage other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
			////base.Equals(other) is not required when deriving from 'object'.

				StringEx          .EqualsOrdinal(Name,         other.Name)     && // The name effectively in use.
				IEnumerableEx.ItemsEqual(        Commands,     other.Commands)    // The commands effectively in use.
			////StringEx          .EqualsOrdinal(LinkFilePath, other.LinkFilePath)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(PredefinedCommandPage lhs, PredefinedCommandPage rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(PredefinedCommandPage lhs, PredefinedCommandPage rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region IComparable Members / Comparison Methods and Operators
		//==========================================================================================
		// IComparable Members / Comparison Methods and Operators
		//==========================================================================================

		/// <summary>
		/// Compares this instance to a specified object and returns an indication
		/// of their relative values.
		/// </summary>
		public virtual int CompareTo(object obj)
		{
			var other = (obj as PredefinedCommandPage);
			if (other != null)
				return (string.Compare(this.Name, other.Name, StringComparison.CurrentCulture));
			else
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'" + obj.ToString() + "' does not specify a 'PredefinedCommandPage'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "obj"));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB))
				return (0);

			var casted = (objA as PredefinedCommandPage);
			if (casted != null)
				return (casted.CompareTo(objB));

			return (ObjectEx.InvalidComparisonResult);
		}

		/// <summary></summary>
		public static bool operator <(PredefinedCommandPage lhs, PredefinedCommandPage rhs)
		{
			return (Compare(lhs, rhs) < 0);
		}

		/// <summary></summary>
		public static bool operator >(PredefinedCommandPage lhs, PredefinedCommandPage rhs)
		{
			return (Compare(lhs, rhs) > 0);
		}

		/// <summary></summary>
		public static bool operator <=(PredefinedCommandPage lhs, PredefinedCommandPage rhs)
		{
			return (Compare(lhs, rhs) <= 0);
		}

		/// <summary></summary>
		public static bool operator >=(PredefinedCommandPage lhs, PredefinedCommandPage rhs)
		{
			return (Compare(lhs, rhs) >= 0);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
