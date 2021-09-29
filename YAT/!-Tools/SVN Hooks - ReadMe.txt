
The SVN hooks save and restore the original time stamp of non-built and 3rd party files.
The SVN hook approach has been chosen after considering various approaches (see further below).

Hooks in use:
 > Start Commit [SVN Hook - StartCommit.exe]
 > Complete Checkout [SVN Hook - CompleteCheckout.exe]

Hooks are .NET 4.8 AnyCPU console applications copied here from the ..\..\YATInfra\SVN projects.
Hooks are [Release] builds. For debugging, configure SVN client to the corresponding \bin folder.

Note that build artefact related files are touched/stamped using pre-build steps according to:
 > ..\YAT\YAT\YAT.csproj.ReadMe.txt
 > ..\YAT\YATConsole\YATConsole.csproj.ReadMe.txt
This results in having the same time stamp as the build artefacts. While this technically isn't
fully accurate (files got created some other time in the past) it seems the most logical approach
from a user's perspective (files are time-tied to build artefacts).

Potential approaches:
 A) \!-Scripts\RefreshFilesToRedistribute.cmd
     + Simple
     + Single update for all files
     - Required after each major checkout
    -- Too complicated to support e.g. handling NuGet packages
 B) Use of SVN client side hooks
     + Works for everybody using TortoiseSVN
     - Requires configuration of TortoiseSVN
     - Active for all projects using TortoiseSVN
     - Always active, i.e. more time consuming
    B1) Use of SVN revision
         + Inherently available
         - Not representing the creation timestamp
    B2) Use of SVN properties
         + No need for additional files
         - Not obvious without looking into SVN
    B3) Use of .timestamp files
         + Diffing files is more obvious than properties
        ++ Automatic update of timestamp possible
       +/- Significant efforts but also learning something
        -- TortoiseSVN specific

Considerations:
 > Maintainability/Transparency/Comprehensibility
 > Speed

Decided to use B3) implemented with console applications written in C#.
(C# code is preferred over fiddeling with JavaScript.)

Hints:
 > https://putridparrot.com/blog/creating-a-pre-commit-hook-for-tortoisesvn/
 > https://www.codeproject.com/Articles/528302/TortoiseSVN-pre-commit-hook-in-Csharp-Save-yoursel
