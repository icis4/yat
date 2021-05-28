
ALAZ
====

Over the years I found a few issues with ALAZ. These issues required to make minor modifications
to ALAZ. These modifications are described below in more detail. I also fed back the issues to
Andre by email. But my feedback was never responded, so I asked online whether any updates or
improvementes of ALAZ can be expected in the future:
 "The purpose of this library is show how Sockets can be development on .NET; it cannot be called
  a "project" with upgrades/new versions. Maybe I'll write some update using .NET 4.5 but, till
  there, you can check SuperSocket Library hosted on codeplex."
After all, 6 releases (1.0 through 1.5) were published between May 2006 and September 2007. So in
2007 this indeed looked like an active project. And in 2009 version 2.0 was released. But then,
well, yet another open-source-dead-end...


Modifications to ALAZ
---------------------
Four kinds of modifications have been made:
 > Repository
 > Project settings
 > Additional exception handling in source code
 > Modifications to the behaviour of the source code

The following contents of the original distribution were not added to the YAT respoitory:
 "\ALAZ\*.cache"
 "\ALAZ\*.TMP"
 "\ALAZ\*.user"
 "\ALAZ\Demos\Echo\EchoConsoleServer\Service References"

Modifications to the project settings have led to the following file diffs:
 "\ALAZ\Source\ALAZ.Version.cs" added
 "\ALAZ\Source\ALAZ.SystemEx\ALAZ.SystemEx.csproj" modified
 "\ALAZ\Source\ALAZ.SystemEx\Properties\AssemblyInfo.cs" modified
 "\ALAZ\Source\ALAZ.SystemEx\RuntTimeEx" renamed to RunTimeEx
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\ALAZ.SystemEx.NetEx.csproj" modified
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\ALAZLibSN.snk" added to "\Properties"
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\Properties\AssemblyInfo.cs" modified

Additional exception handling in source code have led to the following file diffs:
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\SocketsEx\BaseSocketConnection.cs"
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\SocketsEx\BaseSocketConnectionHost.cs"
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\SocketsEx\SocketListener.cs"

Mdifications to the behaviour of the source code have led to the following file diffs:
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\SocketsEx\BaseSocketConnection.cs"
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\SocketsEx\BaseSocketConnectionHost.cs"

Modifications to all files are also stored as unified diffs.


Details on modifications
------------------------
Some details about the modifications or work-arounds which impact YAT:

 > 'ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.StopConnections()' is blocking:
    > If Stop() is called from a GUI/main thread and the GUI/main is attached to the Disconnected
      event, a dead-lock happens:
       - StopConnections() blocks the GUI/main thread
       - FireOnDisconnected() is blocked when trying to synchronize Invoke() onto the GUI/main thread
 > 'ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection.Active.get()' was also blocking but has been
    modified to be non-blocking:
    > In case of YAT with the original ALAZ implementation, AutoSockets created a deadlock on
      shutdown in case of two AutoSockets that were interconnected with each other:
       - Active.get() blocks the GUI/main thread
       - FireOnDisconnected is blocked when trying to synchronize Invoke() onto the GUI/main thread
 > These two issues could be solved by modifying 'ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnection.Active.get()'
   to be non-blocking, by calling Stop() asynchronously and by suppressing the 'OnDisconnected' and
   'OnException' events while stopping.

 > 'ALAZ.SystemEx.NetEx.SocketsEx.SocketClient/Server.Dispose()' doesn't seem to properly stop the
    socket, i.e. Stop() must be called prior to Dispose() to ensure that stopping is properly done.

 > 'ALAZ.SystemEx.NetEx.SocketsEx.BaseSocketConnectionHost.FireOnSent()' now takes an additional
    argument 'buffer' to forward the raw data buffer to the event handler.


Issues but no modifications
---------------------------
Potential issues which don't have any impact on YAT, so no modifications were done in these cases:
 > There are 10 occurrences of "Encoding.GetEncoding(1252)"
    i.e. some code is fixed to Windows code page 1252


-----------------------
2012-10 / Matthias Kläy
