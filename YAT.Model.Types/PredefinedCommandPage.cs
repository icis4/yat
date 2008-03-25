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
		private string _pageName;
		private List<Command> _commands;

		/// <summary></summary>
		public PredefinedCommandPage()
		{
			_pageName = "";
			_commands = new List<Command>();
		}

		/// <summary></summary>
		public PredefinedCommandPage(string pageName)
		{
			_pageName = pageName;
			_commands = new List<Command>();
		}

		/// <summary></summary>
		public PredefinedCommandPage(PredefinedCommandPage rhs)
		{
			_pageName = rhs._pageName;

			// clone all commands
			_commands = new List<Command>();
			foreach (Command c in rhs._commands)
			{
				_commands.Add(new Command(c));
			}
		}

		/// <summary></summary>
		public PredefinedCommandPage(int capacity, string pageName)
		{
			_pageName = pageName;
			_commands = new List<Command>(capacity);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("PageName")]
		public string PageName
		{
			get { return (_pageName); }
			set { _pageName = value; }
		}

		/// <summary></summary>
		[XmlElement("Commands")]
		public List<Command> Commands
		{
			get { return (_commands); }
			set { _commands = value; }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		/// <param name="selectedCommand">Index 0..max-1.</param>
		/// <param name="command">Command to be set.</param>
		public void SetCommand(int selectedCommand, Command command)
		{
			if (selectedCommand >= 0)
			{
				if (selectedCommand < _commands.Count)
				{
					_commands[selectedCommand] = new Command(command);
				}
				else
				{
					while (_commands.Count < (selectedCommand))
						_commands.Add(new Command());

					_commands.Add(new Command(command));
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
			if (obj is PredefinedCommandPage)
				return (Equals((PredefinedCommandPage)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(PredefinedCommandPage value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				if (!_pageName.Equals(value._pageName))
					return (false);

				for (int i = 0; i < _commands.Count; i++)
				{
					if (!_commands[i].Equals(value._commands[i]))
						return (false);
				}
				return (true);
			}
			return (false);
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
				return (_pageName.CompareTo(p._pageName));
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
