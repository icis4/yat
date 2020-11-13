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
// YAT Version 2.1.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of...:
////#define DEBUG_<...>

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System/*.<TODO>*/;

using MKY/*.<TODO>*/;

using YAT/*.<TODO>*/;

#endregion

namespace YAT/*.<TODO>*/
{
	public /*static*/ class TODO
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		#endregion

		#region Static Fields
		//==========================================================================================
		// Static Fields
		//==========================================================================================

		#endregion

		#region Static Events
		//==========================================================================================
		// Static Events
		//==========================================================================================

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		#endregion

		#region Static Properties
		//==========================================================================================
		// Static Properties
		//==========================================================================================

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int todo;
		private IDisposable toDo;

		#endregion

		#region Auto-Properties
		//==========================================================================================
		// Auto-Properties
		//==========================================================================================

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public int TODO
		{
			get
			{
				return (this.todo);
			}
			set
			{
				this.todo = value;
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#endregion

		#region Form/Control Special Keys
		//==========================================================================================
		// Form/Control Special Keys
		//==========================================================================================

		#endregion

		#region Form/Control Event Handlers
		//==========================================================================================
		// Form/Control Event Handlers
		//==========================================================================================

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		#region Controls Event Handlers > Item
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Item
		//------------------------------------------------------------------------------------------

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Non-Public Methods > Item
		//------------------------------------------------------------------------------------------
		// Non-Public Methods > Item
		//------------------------------------------------------------------------------------------

		#endregion

		#endregion

		#region Event Handling
		//==========================================================================================
		// Event Handling
		//==========================================================================================

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			if (IsUndisposed) // AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging.
				return (ToDo);
			else
				return (base.ToString());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		#endregion
	}

	public class /* or struct */ TODO : IEquatable<TODO>
	{
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
				int hashCode = base.GetHashCode(); // !!! If derived from other than 'Object', otherwise replace by = 0; !!!

			//	hashCode =                     IntegerProperty;
			//	hashCode = (hashCode * 397) ^  IntegerProperty;
			//	hashCode = (hashCode * 397) ^  BooleanOrNumericOrEnumOrStructProperty               .GetHashCode();
			//	hashCode = (hashCode * 397) ^ (ArrayProperty         != null ? ArrayProperty        .GetHashCode() : 0);
			//	hashCode = (hashCode * 397) ^ (StringProperty        != null ? StringProperty       .GetHashCode() : 0);
			//	hashCode = (hashCode * 397) ^ (ReferenceTypeProperty != null ? ReferenceTypeProperty.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality. !!! For reference types !!!
		/// Determines whether this instance and the specified object have value equality.              !!! For value types !!!
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as TODO)); // !!! For reference types !!!

		//	if (obj is TODO)              // !!! For value types !!!
		//		return (Equals((TODO)obj));
		//	else
		//		return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality. !!! For reference types !!!
		/// Determines whether this instance and the specified object have value equality.              !!! For value types !!!
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TODO other)
		{
			if (ReferenceEquals(other, null)) return (false); // !!! Remove for value types !!!
			if (ReferenceEquals(this, other)) return (true);  // !!! Remove for value types !!!
			if (GetType() != other.GetType()) return (false); // !!! Remove for value types !!!

			return
			(
			//	!!! Remove for value types !!!
				base.Equals(other) && // !!! If derived from other than 'object', otherwise replace by comment below !!!
			////base.Equals(other) is not required when deriving from 'object'.

				TODO.Equals(other.TODO)
			//	!!! For value types only !!!
			//	Property1.Equals(other.Property1) &&
			//	...
			//	PropertyN.Equals(other.PropertyN) &&
			//	...
			//	!!! For reference types !!!
			//	StringEx          .EqualsOrdinal(StringProperty,        other.StringProperty)      &&
			//	ArrayEx     .ValuesEqual(        ArrayProperty,         other.ArrayProperty)       &&
			//	IEnumerableEx.ItemsEqual(        IEnumerableProperty,   other.IEnumerableProperty) &&
			//	ObjectEx          .Equals(       ReferenceTypeProperty, other.ReferenceTypeProperty)
			);
		}

		// ----------------------------------------------------------------------------------------
		// Background information and recommentations (to be removed for effective code)
		// ----------------------------------------------------------------------------------------
		// Section "Equality Operators" of the Microsoft "Framework Design Guidelines":
		//  > "DO overload the equality operators on value types, if equality is meaningful."
		//  > "AVOID overloading equality operators on mutable reference types.
		//     Many languages have built-in equality operators for reference types. The built-in
		//     operators usually implement the reference equality, and many developers are
		//     surprised when the default behavior is changed to the value equality."
		//       => Remove when value equality is not desired for a reference type.
		//       => Keep when value equality is desired for a reference type.
		//  > "DO ensure that Equals() and the equality operators have exactly the same semantics
		//     and similar performance characteristics."
		// ----------------------------------------------------------------------------------------
		/// <summary>
		/// Determines whether the two specified objects have reference or value equality. !!! For reference types !!!
		/// Determines whether the two specified objects have value equality.              !!! For value types !!!
		/// </summary>
		public static bool operator ==(TODO lhs, TODO rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);  // !!! Remove for value types !!!
			if (ReferenceEquals(lhs, null)) return (false); // !!! Remove for value types !!!
			if (ReferenceEquals(rhs, null)) return (false); // !!! Remove for value types !!!

		//	!!! Remove for value types !!!
			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		//	!!! For value types only !!!
		//	return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality. !!! For reference types !!!
		/// Determines whether the two specified objects have value inequality.               !!! For value types !!!
		/// </summary>
		public static bool operator !=(TODO lhs, TODO rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

//	!!! For classes not yet deriving !!!
	public class TODO : DisposableBase
//	!!! When deriving from classes already implementing IDisposable, either implement the auxiliary interface by adapting or copying/pasting code from DisposableBase !!!
//	public class TODO : IDisposableEx
//	!!! When deriving from other classes, implement the two interfaces by copying/pasting code from DisposableBase !!!
//	public class TODO : IDisposable, IDisposableEx
	{
		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			// Dispose of managed resources:
			if (disposing)
			{
				if (this.toDo != null)
				{
					this.toDo.Dispose();
					this.toDo = null;
				}
			}

			// Release of unmanaged resources:
			this.moreToDo = null;

		//	!!! When deriving from a class that implements DisposableBase, otherwise remove and use commented out hint instead !!!
			base.Dispose(disposing);
		////base.Dispose(disposing) of 'DisposableBase' doesn't need and cannot be called since abstract.
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		public int TODO
		{
			get
			{
			////AssertUndisposed() shall not be called from this simple get-property.

				return (this.todo);
			}
			set
			{
				this.todo = value;
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
