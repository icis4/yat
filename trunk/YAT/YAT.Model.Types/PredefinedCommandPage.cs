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
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
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

		private string pageName;
		private List<Command> commands;

		/// <summary></summary>
		public PredefinedCommandPage()
		{
			this.pageName = "";
			this.commands = new List<Command>();
		}

		/// <summary></summary>
		public PredefinedCommandPage(string pageName)
		{
			this.pageName = pageName;
			this.commands = new List<Command>();
		}

		/// <summary></summary>
		public PredefinedCommandPage(PredefinedCommandPage rhs)
		{
			this.pageName = rhs.pageName;

			// Clone all commands:
			this.commands = new List<Command>(rhs.commands.Count); // Preset the required capacity to improve memory management.
			foreach (Command c in rhs.commands)
				this.commands.Add(new Command(c));
		}

		/// <summary></summary>
		public PredefinedCommandPage(int capacity, string pageName)
		{
			this.pageName = pageName;
			this.commands = new List<Command>(capacity);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("PageName")]
		public virtual string PageName
		{
			get { return (this.pageName); }
			set { this.pageName = value; }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("Commands")]
		public virtual List<Command> Commands
		{
			get { return (this.commands); }
			set { this.commands = value; }
		}

		/// <summary></summary>
		[XmlIgnore]
		public int DefinedCommandCount
		{
			get
			{
				int n = 0;

				foreach (var c in Commands)
				{
					if ((c != null) && (c.IsDefined))
						n++;
				}

				return (n);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <param name="index">Command index 0..(<see cref="MaxCommandCapacityPerPage"/> - 1).</param>
		/// <param name="command">Command to be set.</param>
		public virtual void SetCommand(int index, Command command)
		{
			if ((index < 0) || (index >= MaxCommandCapacityPerPage))
				throw (new ArgumentOutOfRangeException("index", index, MessageHelper.InvalidExecutionPreamble + "'" + index + "' is an invalid command index!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if ((command != null) && (command.IsDefined)) // Prevent non-defined commands from being added to the list
			{                                             // and potentially leading to adding even more non-defineds.
				if (index < this.commands.Count)
				{
					// Replace command:
					this.commands[index] = new Command(command); // Clone command to ensure decoupling.
				}
				else
				{
					// Fill-in empty commands:
					while (this.commands.Count < (index))
						this.commands.Add(new Command());

					// Add command:
					this.commands.Add(new Command(command));
				}
			}
			else
			{
				ClearCommand(index);
			}
		}

		/// <param name="index">Command index 0..(<see cref="MaxCommandCapacityPerPage"/> - 1).</param>
		public virtual void ClearCommand(int index)
		{
			if ((index < 0) || (index >= MaxCommandCapacityPerPage))
				throw (new ArgumentOutOfRangeException("index", index, MessageHelper.InvalidExecutionPreamble + "'" + index + "' is an invalid command index!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if (index < this.commands.Count)
			{
				if (index != (this.commands.Count - 1))
				{
					this.commands[index].Clear();
				}
				else // 'index' is/was the last command:
				{
					// Remove command:
					this.commands.RemoveAt(index);

					// Remove trailing command(s):
					for (int i = (index - 1); i >= 0; i--)
					{
						if ((this.commands[i] == null) || (!this.commands[i].IsDefined))
							this.commands.RemoveAt(i);
						else
							break;
					}
				}
			}
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

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + subpageId.ToString(CultureInfo.InvariantCulture) + "' is a subpage ID that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
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

				foreach (Command c in Commands)
					hashCode = (hashCode * 397) ^ c.GetHashCode();

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

			// Compare page name, i.e. header of page:
			if (!StringEx.EqualsOrdinal(PageName, other.PageName))
				return (false);

			// Compare commands, i.e. contents of page:
			if (!Commands.Count.Equals(other.Commands.Count))
				return (false);

			return (IEnumerableEx.ItemsEqual(Commands, other.Commands));
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
				return (string.Compare(this.pageName, other.pageName, StringComparison.CurrentCulture));
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
