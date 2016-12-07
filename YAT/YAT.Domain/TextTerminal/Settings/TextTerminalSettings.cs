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

using System;
using System.Xml.Serialization;

using MKY;
using MKY.Text;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class TextTerminalSettings : MKY.Settings.SettingsItem, IEquatable<TextTerminalSettings>
	{
		/// <summary></summary>
		public static readonly string DefaultEol = EolEx.Parse(Environment.NewLine);

		/// <summary></summary>
		public static readonly int DefaultEncoding = (EncodingEx)System.Text.Encoding.Default;

		private bool               separateTxRxEol;
		private string             txEol;
		private string             rxEol;
		private int                encoding;
		private bool               showEol;
		private TextLineSendDelay  lineSendDelay;
		private WaitForResponse    waitForResponse;
		private CharSubstitution   charSubstitution;
		private EolCommentSettings eolComment;

		/// <summary></summary>
		public TextTerminalSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public TextTerminalSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			EolComment = new EolCommentSettings(SettingsType);

			SetNodeDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TextTerminalSettings(TextTerminalSettings rhs)
			: base(rhs)
		{
			SeparateTxRxEol  = rhs.SeparateTxRxEol;
			TxEol            = rhs.TxEol;
			RxEol            = rhs.RxEol;
			Encoding         = rhs.Encoding;
			ShowEol          = rhs.ShowEol;
			LineSendDelay    = rhs.LineSendDelay;
			WaitForResponse  = rhs.WaitForResponse;
			CharSubstitution = rhs.CharSubstitution;

			EolComment = new EolCommentSettings(rhs.EolComment);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			SeparateTxRxEol  = false;
			TxEol            = DefaultEol;
			RxEol            = DefaultEol;
			Encoding         = DefaultEncoding;
			ShowEol          = false;
			LineSendDelay    = new TextLineSendDelay(false, 500, 1);
			WaitForResponse  = new WaitForResponse(false, 500);
			CharSubstitution = CharSubstitution.None;
		}

		/// <remarks>
		/// No other way has yet been found to properly set defaults of a list of settings. Before
		/// this solution, the issue #3581368 "EOL comment indicators always contain the defaults"
		/// existed. With this solution, the underlying node itself does no longer know any default,
		/// and default deserialization properly deserializes the settings into the underlying node.
		/// </remarks>
		protected override void SetNodeDefaults()
		{
			base.SetNodeDefaults();

			EolComment.Indicators.Clear();
			EolComment.Indicators.Add("//");
			EolComment.Indicators.Add("REM");
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("SeparateTxRxEol")]
		public virtual bool SeparateTxRxEol
		{
			get { return (this.separateTxRxEol); }
			set
			{
				if (this.separateTxRxEol != value)
				{
					this.separateTxRxEol = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxEol")]
		public virtual string TxEol
		{
			get { return (this.txEol); }
			set
			{
				if (this.txEol != value)
				{
					this.txEol = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxEol")]
		public virtual string RxEol
		{
			get
			{
				if (this.separateTxRxEol)
					return (this.rxEol);
				else
					return (this.txEol);
			}
			set
			{
				if (this.rxEol != value)
				{
					this.rxEol = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Encoding")]
		public virtual int Encoding
		{
			get { return (this.encoding); }
			set
			{
				if (this.encoding != value)
				{
					this.encoding = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowEol")]
		public virtual bool ShowEol
		{
			get { return (this.showEol); }
			set
			{
				if (this.showEol != value)
				{
					this.showEol = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("LineSendDelay")]
		public virtual TextLineSendDelay LineSendDelay
		{
			get { return (this.lineSendDelay); }
			set
			{
				if (this.lineSendDelay != value)
				{
					this.lineSendDelay = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("WaitForResponse")]
		public virtual WaitForResponse WaitForResponse
		{
			get { return (this.waitForResponse); }
			set
			{
				if (this.waitForResponse != value)
				{
					this.waitForResponse = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CharSubstitution")]
		public virtual CharSubstitution CharSubstitution
		{
			get { return (this.charSubstitution); }
			set
			{
				if (this.charSubstitution != value)
				{
					this.charSubstitution = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("EolComment")]
		public virtual EolCommentSettings EolComment
		{
			get { return (this.eolComment); }
			set
			{
				if (this.eolComment != value)
				{
					var oldNode = this.eolComment;
					this.eolComment = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
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
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^  SeparateTxRxEol      .GetHashCode();
				hashCode = (hashCode * 397) ^ (TxEol != null ? TxEol.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RxEol != null ? RxEol.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  Encoding             .GetHashCode();
				hashCode = (hashCode * 397) ^  ShowEol              .GetHashCode();
				hashCode = (hashCode * 397) ^  LineSendDelay        .GetHashCode();
				hashCode = (hashCode * 397) ^  WaitForResponse      .GetHashCode();
				hashCode = (hashCode * 397) ^  CharSubstitution     .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as TextTerminalSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TextTerminalSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				SeparateTxRxEol       .Equals(other.SeparateTxRxEol) &&
				StringEx.EqualsOrdinal(TxEol, other.TxEol)           &&
				StringEx.EqualsOrdinal(RxEol, other.RxEol)           &&
				Encoding              .Equals(other.Encoding)        &&
				ShowEol               .Equals(other.ShowEol)         &&
				LineSendDelay         .Equals(other.LineSendDelay)   &&
				WaitForResponse       .Equals(other.WaitForResponse) &&
				CharSubstitution      .Equals(other.CharSubstitution)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TextTerminalSettings lhs, TextTerminalSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(TextTerminalSettings lhs, TextTerminalSettings rhs)
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
