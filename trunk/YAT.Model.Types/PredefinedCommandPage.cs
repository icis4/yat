//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Model.Types
{
	/// <summary></summary>
	[Serializable]
	public class PredefinedCommandPage : IEquatable<PredefinedCommandPage>, IComparable
	{
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

			// clone all commands
			this.commands = new List<Command>();
			foreach (Command c in rhs.commands)
			{
				this.commands.Add(new Command(c));
			}
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
		[XmlElement("Commands")]
		public virtual List<Command> Commands
		{
			get { return (this.commands); }
			set { this.commands = value; }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		/// <param name="selectedCommand">Index 0..max-1.</param>
		/// <param name="command">Command to be set.</param>
		public virtual void SetCommand(int selectedCommand, Command command)
		{
			if (selectedCommand >= 0)
			{
				if (selectedCommand < this.commands.Count)
				{
					this.commands[selectedCommand] = new Command(command);
				}
				else
				{
					while (this.commands.Count < (selectedCommand))
						this.commands.Add(new Command());

					this.commands.Add(new Command(command));
				}
			}
		}

		#endregion

		#region Object Members
		//------------------------------------------------------------------------------------------
		// Object Members
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return (false);

			PredefinedCommandPage casted = obj as PredefinedCommandPage;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(PredefinedCommandPage casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			// Compare page name, i.e. header of page.
			if (this.pageName != casted.pageName)
				return (false);

			// Compare commands, i.e. contents of page.
			for (int i = 0; i < this.commands.Count; i++)
			{
				if (this.commands[i] != casted.commands[i])
					return (false);
			}
			return (true);
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region IComparable Members
		//------------------------------------------------------------------------------------------
		// IComparable Members
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Compares this instance to a specified object and returns an indication
		/// of their relative values.
		/// </summary>
		public virtual int CompareTo(object obj)
		{
			if (obj == null) return (1);
			if (obj is PredefinedCommandPage)
			{
				PredefinedCommandPage p = (PredefinedCommandPage)obj;
				return (this.pageName.CompareTo(p.pageName));
			}
			throw (new ArgumentException("Object is not a PredefinedCommandPage"));
		}

		#endregion

		#region Comparison Methods

		/// <summary></summary>
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB)) return (0);
			if (objA is PredefinedCommandPage)
			{
				PredefinedCommandPage casted = (PredefinedCommandPage)objA;
				return (casted.CompareTo(objB));
			}
			return (-1);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(PredefinedCommandPage lhs, PredefinedCommandPage rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));

			return (false);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(PredefinedCommandPage lhs, PredefinedCommandPage rhs)
		{
			return (!(lhs == rhs));
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
