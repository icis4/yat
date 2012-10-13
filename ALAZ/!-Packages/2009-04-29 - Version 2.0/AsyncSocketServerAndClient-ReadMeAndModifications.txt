
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
2007 this indeed looked like an active project. Well, yet another open-source-dead-end...


Modifications to ALAZ
---------------------
Four kinds of modifications have been made:
 > Repository
 > Project settings
 > Additional exception handling in source code
 > Minor modifications to the behaviour of the source code

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
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\ALAZLibSN.snk" moved to "\Properties"
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\Properties\AssemblyInfo.cs" modified

Additional exception handling in source code have led to the following file diffs:
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\SocketsEx\BaseSocketConnection.cs"
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\SocketsEx\BaseSocketConnectionHost.cs"
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\SocketsEx\SocketListener.cs"

Minor modifications to the behaviour of the source code have led to the following file diffs:
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\SocketsEx\BaseSocketConnection.cs"
 "\ALAZ\Source\ALAZ.SystemEx.NetEx\SocketsEx\BaseSocketConnectionHost.cs"
   > BaseSocketConnectionHost.StopConnections() has been modified to be non-blocking

Modifications to all files are also stored as unified diffs.


-----------------------
2012-10 / Matthias Kläy
