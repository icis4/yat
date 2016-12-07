//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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

using YAT/*.<TODO>*/;

#endregion

namespace YAT/*.<TODO>*/
{
	public class TODO
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

		private bool isDisposed;

		private int todo;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Release unmanaged resources:

				// Set state to disposed:
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~TODO()
		{
			Dispose(false);

			MKY.Diagnostics.DisposalHelper.DebugNotifyFinalizerInsteadOfDispose(this);
		}

		/// <summary></summary>
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Release unmanaged resources:

				// Set state to disposed:

			}

			base.Dispose(disposing);
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
				// Do not call AssertNotDisposed() in a simple get-property.

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

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		#region Private Methods > Item
		//------------------------------------------------------------------------------------------
		// Private Methods > Item
		//------------------------------------------------------------------------------------------

		#endregion

		#endregion

		#region Event Handling
		//==========================================================================================
		// Event Handling
		//==========================================================================================

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
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
			return ("TODO");
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

	public class TODO : IEquatable<TODO>
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
				int hashCode = base.GetHashCode(); // !!! If derived from other than 'Object', otherwise remove !!!

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
		/// Determines whether this instance and the specified object have value equality.
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
		/// Determines whether this instance and the specified object have value equality.
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
				// Do not call 'base.Equals(other)' when deriving from 'object'.

				TODO.Equals(other.TODO)
			//	!!! For value types only !!!
			//	Property1.Equals(other.Property1) &&
			//	...
			//	PropertyN.Equals(other.PropertyN) &&
			//	...
			//	!!! For reference types !!!
			//	StringEx     .EqualsOrdinal( StringProperty,        other.StringProperty)      &&
			//	ArrayEx      .ElementsEqual( ArrayProperty,         other.ArrayProperty)       &&
			//	IEnumerableEx.ElementsEqual( IEnumerableProperty,   other.IEnumerableProperty) &&
			//	ObjectEx             .Equals(ReferenceTypeProperty, other.ReferenceTypeProperty)
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
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TODO lhs, TODO rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);  // !!! Remove for value types !!!
			if (ReferenceEquals(lhs, null)) return (false); // !!! Remove for value types !!!
			if (ReferenceEquals(rhs, null)) return (false); // !!! Remove for value types !!!

		//	!!! Remove for value types !!!
			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that the virtual <Derived>.Equals() is called.
		//	!!! For value types only !!!
		//	return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(TODO lhs, TODO rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
