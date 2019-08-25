﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY;
using MKY.Text;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class TextTerminalSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public static readonly string DefaultEol = (string)EolEx.Parse(Environment.NewLine);

		/// <summary></summary>
		public static readonly int DefaultEncoding = (EncodingEx)(System.Text.Encoding.Default);

		private bool              separateTxRxEol;
		private string            txEol;
		private string            rxEol;
		private int               encoding;
		private bool              showEol;
		private TextLineSendDelay lineSendDelay;
		private WaitForResponse   waitForResponse;
		private CharSubstitution  charSubstitution;
		private bool              skipEolComments;
		private List<string>      eolCommentIndicators;

		/// <summary></summary>
		public TextTerminalSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public TextTerminalSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TextTerminalSettings(TextTerminalSettings rhs)
			: base(rhs)
		{
			SeparateTxRxEol      = rhs.SeparateTxRxEol;
			TxEol                = rhs.TxEol;
			RxEol                = rhs.RxEol;
			Encoding             = rhs.Encoding;
			ShowEol              = rhs.ShowEol;
			LineSendDelay        = rhs.LineSendDelay;
			WaitForResponse      = rhs.WaitForResponse;
			CharSubstitution     = rhs.CharSubstitution;
			SkipEolComments      = rhs.SkipEolComments;
			EolCommentIndicators = new List<string>(rhs.EolCommentIndicators);

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
			SkipEolComments  = false;

			List<string> l = new List<string>();
			l.Add("//");
			l.Add("REM");
			EolCommentIndicators = l;
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
				if (value != this.separateTxRxEol)
				{
					this.separateTxRxEol = value;
					SetChanged();
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
				if (value != this.txEol)
				{
					this.txEol = value;
					SetChanged();
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
				if (value != this.rxEol)
				{
					this.rxEol = value;
					SetChanged();
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
				if (value != this.encoding)
				{
					this.encoding = value;
					SetChanged();
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
				if (value != this.showEol)
				{
					this.showEol = value;
					SetChanged();
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
				if (value != this.lineSendDelay)
				{
					this.lineSendDelay = value;
					SetChanged();
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
				if (value != this.waitForResponse)
				{
					this.waitForResponse = value;
					SetChanged();
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
				if (value != this.charSubstitution)
				{
					this.charSubstitution = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SkipEolComments")]
		public virtual bool SkipEolComments
		{
			get { return (this.skipEolComments); }
			set
			{
				if (value != this.skipEolComments)
				{
					this.skipEolComments = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("EolCommentIndicators")]
		public List<string> EolCommentIndicators
		{
			get { return (this.eolCommentIndicators); }
			set
			{
				if (value != this.eolCommentIndicators)
				{
					this.eolCommentIndicators = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			TextTerminalSettings other = (TextTerminalSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.separateTxRxEol      == other.separateTxRxEol) &&
				(this.txEol                == other.txEol) &&
				(this.rxEol                == other.rxEol) &&
				(this.encoding             == other.encoding) &&
				(this.showEol              == other.showEol) &&
				(this.lineSendDelay        == other.lineSendDelay) &&
				(this.waitForResponse      == other.waitForResponse) &&
				(this.charSubstitution     == other.charSubstitution) &&
				(this.skipEolComments      == other.skipEolComments) &&
				(this.eolCommentIndicators == other.eolCommentIndicators)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.separateTxRxEol     .GetHashCode() ^
				this.txEol               .GetHashCode() ^
				this.rxEol               .GetHashCode() ^
				this.encoding            .GetHashCode() ^
				this.showEol             .GetHashCode() ^
				this.lineSendDelay       .GetHashCode() ^
				this.waitForResponse     .GetHashCode() ^
				this.charSubstitution    .GetHashCode() ^
				this.skipEolComments     .GetHashCode() ^
				this.eolCommentIndicators.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================