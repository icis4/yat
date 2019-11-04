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
// YAT Version 2.1.1 Development
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

using MKY;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// This partial class implements the repositories part of <see cref="Terminal"/>.
	/// </remarks>
	public abstract partial class Terminal
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private DisplayRepository txRepository;
		private DisplayRepository bidirRepository;
		private DisplayRepository rxRepository;
		private object repositorySyncObj = new object();

		#endregion

		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		#region Repository Access
		//------------------------------------------------------------------------------------------
		// Repository Access
		//------------------------------------------------------------------------------------------

		/// <remarks>See remarks in <see cref="RefreshRepositories"/> below.</remarks>
		public virtual bool ClearRepository(RepositoryType repositoryType)
		{
			AssertNotDisposed();
			            //// Only try for some time, otherwise ignore. Prevents deadlocks among main thread (view) and large amounts of incoming data.
			if (Monitor.TryEnter(this.clearAndRefreshSyncObj, ClearAndRefreshTimeout))
			{
				try
				{
					this.rawTerminal.ClearRepository(repositoryType);
				}
				finally
				{
					Monitor.Exit(this.clearAndRefreshSyncObj);
				}

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <remarks>See remarks in <see cref="RefreshRepositories"/> below.</remarks>
		public virtual bool ClearRepositories()
		{
			AssertNotDisposed();
			            //// Only try for some time, otherwise ignore. Prevents deadlocks among main thread (view) and large amounts of incoming data.
			if (Monitor.TryEnter(this.clearAndRefreshSyncObj, ClearAndRefreshTimeout))
			{
				try
				{
					this.rawTerminal.ClearRepositories();
				}
				finally
				{
					Monitor.Exit(this.clearAndRefreshSyncObj);
				}

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <remarks>See remarks in <see cref="RefreshRepositories"/> below.</remarks>
		public virtual bool RefreshRepository(RepositoryType repositoryType)
		{
			AssertNotDisposed();
			            //// Only try for some time, otherwise ignore. Prevents deadlocks among main thread (view) and large amounts of incoming data.
			if (Monitor.TryEnter(this.clearAndRefreshSyncObj, ClearAndRefreshTimeout))
			{
				try
				{
					// Reset processing:
					ResetProcess(repositoryType);

					// Clear repository:
					ClearMyRepository(repositoryType);

					// Reload repository:
					this.isReloading = true;
					foreach (var raw in this.rawTerminal.RepositoryToChunks(repositoryType))
					{
						ProcessRawChunk(raw, LineChunkAttribute.None); // Attributes are not (yet) supported on reloading => bug #211.
					}
					this.isReloading = false;
					ReloadMyRepository(repositoryType);
				}
				finally
				{
					Monitor.Exit(this.clearAndRefreshSyncObj);
				}

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <remarks>
		/// Alternatively, clear/refresh operations could be implemented asynchronously.
		/// Advantages:
		///  > No deadlock possible below.
		/// Disadvantages:
		///  > User does not get immediate feedback that a time consuming operation is taking place.
		///  > User actually cannot trigger any other operation.
		///  > Other synchronization issues?
		/// Therefore, decided to keep the implementation synchronous until new issues pop up.
		/// </remarks>
		public virtual bool RefreshRepositories()
		{
			AssertNotDisposed();
			            //// Only try for some time, otherwise ignore. Prevents deadlocks among main thread (view) and large amounts of incoming data.
			if (Monitor.TryEnter(this.clearAndRefreshSyncObj, ClearAndRefreshTimeout))
			{
				try
				{
					// Reset processing:
					ResetProcess(RepositoryType.Tx);
					ResetProcess(RepositoryType.Bidir);
					ResetProcess(RepositoryType.Rx);

					// Clear repositories:
					ClearMyRepository(RepositoryType.Tx);
					ClearMyRepository(RepositoryType.Bidir);
					ClearMyRepository(RepositoryType.Rx);

					// Reload repositories:
					this.isReloading = true;
					foreach (var raw in this.rawTerminal.RepositoryToChunks(RepositoryType.Bidir))
					{
						ProcessRawChunk(raw, LineChunkAttribute.None); // Attributes are not (yet) supported on reloading => bug #211.
					}
					this.isReloading = false;
					ReloadMyRepository(RepositoryType.Tx);
					ReloadMyRepository(RepositoryType.Bidir);
					ReloadMyRepository(RepositoryType.Rx);
				}
				finally
				{
					Monitor.Exit(this.clearAndRefreshSyncObj);
				}

				return (true);
			}
			else
			{
				return (false);
			}
		}

		/// <remarks>
		/// Note that value reflects the byte count of the elements contained in the repository,
		/// i.e. the byte count of the elements shown. The value thus not necessarily reflects the
		/// total byte count of a sent or received sequence, a hidden EOL is e.g. not reflected.
		/// </remarks>
		public virtual int GetRepositoryByteCount(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    return (this.txRepository   .ByteCount);
					case RepositoryType.Bidir: return (this.bidirRepository.ByteCount);
					case RepositoryType.Rx:    return (this.rxRepository   .ByteCount);

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual int GetRepositoryLineCount(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    return (this.txRepository   .Count);
					case RepositoryType.Bidir: return (this.bidirRepository.Count);
					case RepositoryType.Rx:    return (this.rxRepository   .Count);

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual DisplayElementCollection RepositoryToDisplayElements(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    return (this.txRepository   .ToElements());
					case RepositoryType.Bidir: return (this.bidirRepository.ToElements());
					case RepositoryType.Rx:    return (this.rxRepository   .ToElements());

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual DisplayLineCollection RepositoryToDisplayLines(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    return (this.txRepository.   ToLines());
					case RepositoryType.Bidir: return (this.bidirRepository.ToLines());
					case RepositoryType.Rx:    return (this.rxRepository   .ToLines());

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual DisplayLine LastDisplayLineAuxiliary(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    return (this.txRepository.   LastLineAuxiliary());
					case RepositoryType.Bidir: return (this.bidirRepository.LastLineAuxiliary());
					case RepositoryType.Rx:    return (this.rxRepository   .LastLineAuxiliary());

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual void ClearLastDisplayLineAuxiliary(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    this.txRepository.   ClearLastLineAuxiliary(); break;
					case RepositoryType.Bidir: this.bidirRepository.ClearLastLineAuxiliary(); break;
					case RepositoryType.Rx:    this.rxRepository   .ClearLastLineAuxiliary(); break;

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual string RepositoryToExtendedDiagnosticsString(RepositoryType repositoryType)
		{
			return (RepositoryToExtendedDiagnosticsString(repositoryType, ""));
		}

		/// <summary></summary>
		public virtual string RepositoryToExtendedDiagnosticsString(RepositoryType repositoryType, string indent)
		{
			AssertNotDisposed();

			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    return (this.txRepository   .ToExtendedDiagnosticsString(indent));
					case RepositoryType.Bidir: return (this.bidirRepository.ToExtendedDiagnosticsString(indent));
					case RepositoryType.Rx:    return (this.rxRepository   .ToExtendedDiagnosticsString(indent));

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		public virtual List<RawChunk> RepositoryToRawChunks(RepositoryType repositoryType)
		{
			AssertNotDisposed();

			return (this.rawTerminal.RepositoryToChunks(repositoryType));
		}

		private void DisposeRepositories()
		{
			lock (this.repositorySyncObj)
			{
				if (this.txRepository != null)
				{
					this.txRepository.Clear();
					this.txRepository = null;
				}

				if (this.bidirRepository != null)
				{
					this.bidirRepository.Clear();
					this.bidirRepository = null;
				}

				if (this.rxRepository != null)
				{
					this.rxRepository.Clear();
					this.rxRepository = null;
				}
			}
		}

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Repositories
		//------------------------------------------------------------------------------------------
		// Repositories
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual void CreateRepositories(Settings.TerminalSettings settings)
		{
			this.txRepository    = new DisplayRepository(settings.Display.MaxLineCount);
			this.bidirRepository = new DisplayRepository(settings.Display.MaxLineCount);
			this.rxRepository    = new DisplayRepository(settings.Display.MaxLineCount);
		}

		/// <summary></summary>
		protected virtual void CreateRepositories(Terminal terminal)
		{
			this.txRepository    = new DisplayRepository(terminal.txRepository);
			this.bidirRepository = new DisplayRepository(terminal.bidirRepository);
			this.rxRepository    = new DisplayRepository(terminal.rxRepository);
		}

		/// <summary></summary>
		protected virtual void AddDisplayElement(IODirection direction, DisplayElement element)
		{
			switch (direction)
			{
				case IODirection.Tx:
				{
					AddDisplayElement(RepositoryType.Tx,    element.Clone()); // Clone elements as they are needed again below.
					AddDisplayElement(RepositoryType.Bidir, element);         // No clone needed as elements are not needed again.
					break;
				}

				case IODirection.Rx:
				{
					AddDisplayElement(RepositoryType.Bidir, element.Clone()); // Clone elements as they are needed again below.
					AddDisplayElement(RepositoryType.Rx,    element);         // No clone needed as elements are not needed again.
					break;
				}

				case IODirection.None:
				{
					throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		protected virtual void AddDisplayElement(RepositoryType repositoryType, DisplayElement element)
		{
			var elements = new DisplayElementCollection(1); // Preset the required capacity to improve memory management.
			elements.Add(element); // No clone needed as the element must be created when calling this event method.
			AddDisplayElements(repositoryType, elements);
		}

		/// <summary></summary>
		protected virtual void AddDisplayElements(RepositoryType repositoryType, DisplayElementCollection elements)
		{
			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    this.txRepository   .Enqueue(elements.Clone()); break; // Clone elements as they are needed again below.
					case RepositoryType.Bidir: this.bidirRepository.Enqueue(elements.Clone()); break; // Clone elements as they are needed again below.
					case RepositoryType.Rx:    this.rxRepository   .Enqueue(elements.Clone()); break; // Clone elements as they are needed again below.

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				OnDisplayElementsAdded(repositoryType, new DisplayElementsEventArgs(elements));
		}

		/// <summary></summary>
		protected virtual void ReplaceCurrentDisplayLine(RepositoryType repositoryType, DisplayElementCollection currentLineElements)
		{
			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    this.txRepository   .ReplaceCurrentLine(currentLineElements.Clone()); break; // Clone elements as they are needed again below.
					case RepositoryType.Bidir: this.bidirRepository.ReplaceCurrentLine(currentLineElements.Clone()); break; // Clone elements as they are needed again below.
					case RepositoryType.Rx:    this.rxRepository   .ReplaceCurrentLine(currentLineElements.Clone()); break; // Clone elements as they are needed again below.

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				OnCurrentDisplayLineReplaced(repositoryType, new DisplayElementsEventArgs(currentLineElements));
		}

		/// <summary></summary>
		protected virtual void ClearCurrentDisplayLine(RepositoryType repositoryType)
		{
			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    this.txRepository   .ClearCurrentLine(); break;
					case RepositoryType.Bidir: this.bidirRepository.ClearCurrentLine(); break;
					case RepositoryType.Rx:    this.rxRepository   .ClearCurrentLine(); break;

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				OnCurrentDisplayLineCleared(repositoryType, EventArgs.Empty);
		}

		/// <summary></summary>
		protected virtual void AddDisplayLine(RepositoryType repositoryType, DisplayLine line)
		{
			var lines = new DisplayLineCollection(1); // Preset the required capacity to improve memory management.
			lines.Add(line); // No clone needed as the element must be created when calling this event method.
			AddDisplayLines(repositoryType, lines);
		}

		/// <summary></summary>
		protected virtual void AddDisplayLines(RepositoryType repositoryType, DisplayLineCollection lines)
		{
			// Nothing to add/enqueue to the repositories, lines are created by enqueuing elements.

			if (!this.isReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				OnDisplayLinesAdded(repositoryType, new DisplayLinesEventArgs(lines));
		}

		/// <remarks>Named 'My' for distinction with <see cref="ClearRepository(RepositoryType)"/>.</remarks>
		protected virtual void ClearMyRepository(RepositoryType repositoryType)
		{
			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    this.txRepository   .Clear(); break;
					case RepositoryType.Bidir: this.bidirRepository.Clear(); break;
					case RepositoryType.Rx:    this.rxRepository   .Clear(); break;

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			OnRepositoryCleared(repositoryType, EventArgs.Empty); // Raise event outside the lock.
		}

		/// <remarks>Named 'My' for consistency with <see cref="ClearMyRepository(RepositoryType)"/>.</remarks>
		protected virtual void ReloadMyRepository(RepositoryType repositoryType)
		{
		////lock (this.repositorySyncObj)
		////{
		////	switch (repositoryType)
		////	{
		////		case RepositoryType.Tx:    /* Nothing to do */ break;
		////		case RepositoryType.Bidir: /* Nothing to do */ break;
		////		case RepositoryType.Rx:    /* Nothing to do */ break;
		////
		////		case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		////		default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		////	}
		////}

			OnRepositoryReloaded(repositoryType, EventArgs.Empty); // Raise event outside the lock.
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
