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
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Linq;
using System.Threading;

using MKY;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
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

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <remarks>See remarks in <see cref="RefreshRepositories"/> below.</remarks>
		public virtual bool ClearRepository(RepositoryType repositoryType)
		{
			AssertUndisposed();
			            //// Only try for some time, otherwise ignore. Prevents deadlocks among main thread (view) and large amounts of incoming data.
			if (Monitor.TryEnter(ClearRefreshEmptySyncObj, ClearRefreshEmptyTimeout))
			{
				try
				{
					this.rawTerminal.ClearRepository(repositoryType);
				}
				finally
				{
					Monitor.Exit(ClearRefreshEmptySyncObj);
				}

				return (true);
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("ClearRepository() monitor has timed out, rejecting this request!");

				return (false);
			}
		}

		/// <remarks>See remarks in <see cref="RefreshRepositories"/> below.</remarks>
		public virtual bool ClearRepositories()
		{
			AssertUndisposed();
			            //// Only try for some time, otherwise ignore. Prevents deadlocks among main thread (view) and large amounts of incoming data.
			if (Monitor.TryEnter(ClearRefreshEmptySyncObj, ClearRefreshEmptyTimeout))
			{
				try
				{
					this.rawTerminal.ClearRepositories();
				}
				finally
				{
					Monitor.Exit(ClearRefreshEmptySyncObj);
				}

				return (true);
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("ClearRepositories() monitor has timed out, rejecting this request!");

				return (false);
			}
		}

		/// <remarks>See remarks in <see cref="RefreshRepositories"/> below.</remarks>
		public virtual bool RefreshRepository(RepositoryType repositoryType)
		{
			AssertUndisposed();
			            //// Only try for some time, otherwise ignore. Prevents deadlocks among main thread (view) and large amounts of incoming data.
			if (Monitor.TryEnter(ClearRefreshEmptySyncObj, ClearRefreshEmptyTimeout))
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
						ProcessChunk(repositoryType, raw);
					}
					this.isReloading = false;
					FinishReload(repositoryType);
				}
				finally
				{
					Monitor.Exit(ClearRefreshEmptySyncObj);
				}

				return (true);
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("RefreshRepository() monitor has timed out, rejecting this request!");

				return (false);
			}
		}

		/// <remarks>
		/// Alternatively, clear/refresh/empty operations could be implemented asynchronously.
		/// Advantages:
		///  > Already synchronized onto main thread.
		///  > No deadlock possible below.
		///  > No deadlock possible with async process such as e.g. timed line breaks or glue timeout.
		///    (Currently mitigated by doing separate handling 'OnReload'.)
		/// Disadvantages:
		///  > User does not get immediate feedback that a time consuming operation is taking place.
		///  > User actually cannot trigger any other operation.
		///  > Other synchronization issues?
		/// Therefore, decided to keep the implementation synchronous until new issues pop up.
		/// </remarks>
		public virtual bool RefreshRepositories()
		{
			AssertUndisposed();
			            //// Only try for some time, otherwise ignore. Prevents deadlocks among main thread (view) and large amounts of incoming data.
			if (Monitor.TryEnter(ClearRefreshEmptySyncObj, ClearRefreshEmptyTimeout))
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
					foreach (var chunk in this.rawTerminal.RepositoryToChunks(RepositoryType.Tx))
					{                                           // Separate Tx/Bidir/Rx processing is required...
						ProcessChunk(RepositoryType.Tx, chunk); // ...for selectively clearing repositories...
					}                                           // ...with ClearRepository(repository).
					foreach (var chunk in this.rawTerminal.RepositoryToChunks(RepositoryType.Bidir))
					{
						ProcessChunk(RepositoryType.Bidir, chunk);
					}
					foreach (var chunk in this.rawTerminal.RepositoryToChunks(RepositoryType.Rx))
					{                                           // Synchronized Tx/Bidir/Rx processing is not...
						ProcessChunk(RepositoryType.Rx, chunk); // ...useful for synchronized incremental monitor...
					}                                           // ...refresh because monitors are only refreshed...
					this.isReloading = false;                   // ...by the subsequent 'ReloadMyRepository' calls.
					FinishReload(RepositoryType.Tx);
					FinishReload(RepositoryType.Bidir);
					FinishReload(RepositoryType.Rx);
				}
				finally
				{
					Monitor.Exit(ClearRefreshEmptySyncObj);
				}

				return (true);
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("RefreshRepositories() monitor has timed out, rejecting this request!");

				return (false);
			}
		}

		/// <remarks>See remarks in <see cref="RefreshRepositories"/> above.</remarks>
		public virtual bool EmptyRepositories()
		{
			AssertUndisposed();
			            //// Only try for some time, otherwise ignore. Prevents deadlocks among main thread (view) and large amounts of incoming data.
			if (Monitor.TryEnter(ClearRefreshEmptySyncObj, ClearRefreshEmptyTimeout))
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
				}
				finally
				{
					Monitor.Exit(ClearRefreshEmptySyncObj);
				}

				return (true);
			}
			else // Monitor.TryEnter()
			{
				DebugMessage("EmptyRepositories() monitor has timed out, rejecting this request!");

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
			AssertUndisposed();

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
			AssertUndisposed();

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
			AssertUndisposed();

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
			AssertUndisposed();

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
			AssertUndisposed();

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
			AssertUndisposed();

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
			AssertUndisposed();

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
			AssertUndisposed();

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

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

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
			{                                                                     // Clone element as it is needed right again.
				case IODirection.Tx:    AddDisplayElement(RepositoryType.Tx, element.Clone()); AddDisplayElement(RepositoryType.Bidir, element);                                                        break;
				case IODirection.Bidir: AddDisplayElement(RepositoryType.Tx, element.Clone()); AddDisplayElement(RepositoryType.Bidir, element.Clone()); AddDisplayElement(RepositoryType.Rx, element); break;
				case IODirection.Rx:                                                           AddDisplayElement(RepositoryType.Bidir, element.Clone()); AddDisplayElement(RepositoryType.Rx, element); break;
				                                                                                                                          //// Clone element as it is needed right again.
				case IODirection.None:  throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void AddDisplayElement(RepositoryType repositoryType, DisplayElement element)
		{
			var elements = new DisplayElementCollection(1); // Preset the required capacity to improve memory management.
			elements.Add(element); // No clone needed as the element must be created when calling this event method.
			AddDisplayElements(repositoryType, elements);
		}

		/// <remarks>
		/// Using <see cref="DisplayElementCollection"/> instead of <see cref="IEnumerable{T}"/> of
		/// <see cref="DisplayElement.InlineElement "/> for <paramref name="elements"/> in order to
		/// ease cloning.
		/// </remarks>
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

			if (!IsReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
				OnDisplayElementsAdded(repositoryType, new DisplayElementsEventArgs(elements));
		}

		/// <summary>
		/// Inlines the given element into the data/control content of the related repositories.
		/// </summary>
		/// <remarks>
		/// Opposed to the 'AddDisplayElement...' methods, the 'InlineDisplayElement...' methods
		/// automatically insert a space if necessary.
		/// </remarks>
		/// <remarks>
		/// Explicit <paramref name="direction"/> because <see cref="DisplayElement.Direction"/>
		/// may be <see cref="Direction.None"/>.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Inline' is a correct English term in programming.")]
		protected virtual void InlineDisplayElement(IODirection direction, DisplayElement.InlineElement element)
		{
			switch (direction)
			{                                                                                   // Clone element as it is needed right again.
				case IODirection.Tx:    InlineDisplayElement(RepositoryType.Tx, direction, element.Clone()); InlineDisplayElement(RepositoryType.Bidir, direction, element);                                                                      break;
				case IODirection.Bidir: InlineDisplayElement(RepositoryType.Tx, direction, element.Clone()); InlineDisplayElement(RepositoryType.Bidir, direction, element.Clone()); InlineDisplayElement(RepositoryType.Rx, direction, element); break;
				case IODirection.Rx:                                                                         InlineDisplayElement(RepositoryType.Bidir, direction, element.Clone()); InlineDisplayElement(RepositoryType.Rx, direction, element); break;
				                                                                                                                                                      //// Clone element as it is needed right again.
				default:                throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Inlines the given element into the data/control content of the related repositories.
		/// </summary>
		/// <remarks>
		/// Opposed to the 'AddDisplayElement...' methods, the 'InlineDisplayElement...' methods
		/// automatically insert a space if necessary.
		/// </remarks>
		/// <remarks>
		/// Explicit <paramref name="direction"/> because <see cref="DisplayElement.Direction"/>
		/// may be <see cref="Direction.None"/>.
		/// </remarks>
		/// <remarks>
		/// Using <see cref="DisplayElementCollection"/> instead of <see cref="IEnumerable{T}"/> of
		/// <see cref="DisplayElement.InlineElement "/> for <paramref name="elements"/> in order to
		/// ease cloning.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Inline' is a correct English term in programming.")]
		protected virtual void InlineDisplayElements(IODirection direction, DisplayElementCollection elements)
		{
			switch (direction)
			{                                                                                     // Clone elements as they are needed right again.
				case IODirection.Tx:    InlineDisplayElements(RepositoryType.Tx, direction, elements.Clone()); InlineDisplayElements(RepositoryType.Bidir, direction, elements);                                                                        break;
				case IODirection.Bidir: InlineDisplayElements(RepositoryType.Tx, direction, elements.Clone()); InlineDisplayElements(RepositoryType.Bidir, direction, elements.Clone()); InlineDisplayElements(RepositoryType.Rx, direction, elements); break;
				case IODirection.Rx:                                                                           InlineDisplayElements(RepositoryType.Bidir, direction, elements.Clone()); InlineDisplayElements(RepositoryType.Rx, direction, elements); break;
				                                                                                                                                                          //// Clone elements as they are needed right again
				default:                throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// Helper method to ease handling direction/repositories.
		/// </remarks>
		/// <remarks>
		/// Opposed to the 'AddDisplayElement...' methods, the 'InlineDisplayElement...' methods
		/// automatically insert a space if necessary.
		/// </remarks>
		private void InlineDisplayElement(RepositoryType repositoryType, IODirection direction, DisplayElement.InlineElement element)
		{
			var lp = new DisplayElementCollection(1 + 1); // Preset the required capacity to improve memory management.
			var lineState = GetLineState(repositoryType);

			// Add space if necessary:
			if (ElementsAreSeparate(lineState.Direction))
				AddContentSeparatorIfNecessary(lineState, direction, lp, element);

			// Add element:
			lp.Add(element);

			AddDisplayElements(repositoryType, lp);

			// Note that simply adding leads to a suboptimality (bug #352):
			//
			// ABC<CR><LF>[Warning: 5 bytes not sent anymore due to break.]
			//
			// ABCD<CR><LF>
			// ABCD<CR><LF>[Warning: CTS inactive, retaining data...] <= Warning appears on already completed line
			//
			// [Error: EOL keyword is not supported for binary terminals!]
			// 41h 41h 42h
			// 41h 42h 43h[Error: EOL keyword is not supported for binary terminals!]
			//
			// => Simply adding may result in inlining at suboptimal locations.
			//
			// However, the current approach also has advantages over moving message to a separate line:
			//  > In the first example, the number of shown lines relates to true number of lines.
			//  > Separate line would require additional LineStart/End elements, somewhat inconsistent.
			//  > Separate line would also be inconsistent in general, e.g. length would be useless.
			//
			// => Insert into RawRepository? Same as for retaining e.g. "RX PARITY ERROR"?
			// => Significant refactoring of the raw terminal repository content would be required to
			//    solve this, e.g. changing the raw terminal from byte to 'IOElement'.
			//
			// https://sourceforge.net/p/y-a-terminal/bugs/352/ (Terminal warnings are not shown on separate line)
			// https://sourceforge.net/p/y-a-terminal/bugs/211/ (RX PARITY ERROR indications disappear)
		}

		/// <remarks>
		/// Helper method to ease handling direction/repositories.
		/// </remarks>
		/// <remarks>
		/// Opposed to the 'AddDisplayElement...' methods, the 'InlineDisplayElement...' methods
		/// automatically insert a space if necessary.
		/// </remarks>
		/// <remarks>
		/// Using <see cref="DisplayElementCollection"/> instead of <see cref="IEnumerable{T}"/> of
		/// <see cref="DisplayElement.InlineElement "/> for <paramref name="elements"/> in order to
		/// ease cloning.
		/// </remarks>
		private void InlineDisplayElements(RepositoryType repositoryType, IODirection direction, DisplayElementCollection elements)
		{
			var lp = new DisplayElementCollection(1 + elements.Count); // Preset the required capacity to improve memory management.
			var lineState = GetLineState(repositoryType);

			// Add space if necessary:
			if (ElementsAreSeparate(lineState.Direction))
				AddContentSeparatorIfNecessary(lineState, direction, lp, elements.First());

			// Add elements:
			lp.AddRange(elements);

			AddDisplayElements(repositoryType, lp);

			// Consider note at InlineDisplayElement() above.
		}

		/// <remarks>
		/// Using <see cref="DisplayElementCollection"/> instead of <see cref="IEnumerable{T}"/> of
		/// <see cref="DisplayElement.InlineElement "/> for <paramref name="currentLineElements"/>
		/// in order to ease cloning.
		/// </remarks>
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

			if (!IsReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
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

			if (!IsReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
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

			if (!IsReloading) // For performance reasons, skip 'normal' events during reloading, a 'RepositoryReloaded' event will be raised after completion.
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

		/// <summary></summary>
		protected virtual void FinishReload(RepositoryType repositoryType)
		{
			lock (this.repositorySyncObj)
			{
				switch (repositoryType)
				{
					case RepositoryType.Tx:    DoFinishReload(repositoryType); break;
					case RepositoryType.Bidir: DoFinishReload(repositoryType); break;
					case RepositoryType.Rx:    DoFinishReload(repositoryType); break;

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			OnRepositoryReloaded(repositoryType, new DisplayLinesEventArgs(RepositoryToDisplayLines(repositoryType))); // Raise event outside the lock.
		}

		/// <summary></summary>
		protected virtual void DoFinishReload(RepositoryType repositoryType)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    ProcessAndSignalTimedLineBreakOnReloadIfNeeded(repositoryType, DateTime.MaxValue, IODirection.Tx); break;
				case RepositoryType.Bidir: ProcessAndSignalTimedLineBreakOnReloadIfNeeded(repositoryType, DateTime.MaxValue, IODirection.Tx);
				                           ProcessAndSignalTimedLineBreakOnReloadIfNeeded(repositoryType, DateTime.MaxValue, IODirection.Rx); break;
				case RepositoryType.Rx:    ProcessAndSignalTimedLineBreakOnReloadIfNeeded(repositoryType, DateTime.MaxValue, IODirection.Rx); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
