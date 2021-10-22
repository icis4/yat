
The SVN hooks save and restore the original time stamp of non-built and 3rd party files.
The SVN hook approach has been chosen after considering various approaches (see further below).

Hooks in use:
 > Start-Commit [SVN Hook - StartCommit.exe] version 1.0.1.0 (2021-10-22)
 > Post-Update [SVN Hook - PostUpdate.exe] version 1.0.0.1 (2021-10-22)

Hooks are .NET 4.8 AnyCPU console applications copied here from the ..\..\YATInfra\SVN projects.
Hooks are [Release] builds. For debugging, activate DEBUGGER_BREAK_HOOK and build [Debug].

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
     + Active only for configured projects / working copies
     - Always active for project / working copy, i.e. more time consuming
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
         + Git can be configured similarly (see 3rd link below)
    Hints:
     > https://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-dug-settings.html
     > https://putridparrot.com/blog/creating-a-pre-commit-hook-for-tortoisesvn/
     > https://www.codeproject.com/Articles/528302/TortoiseSVN-pre-commit-hook-in-Csharp-Save-yoursel
     > https://stackoverflow.com/questions/40953545/how-do-i-save-the-original-creation-time-in-git

 C) Use of SVN [use-commit-times]
     + Comes out of the box
   --- Preserves time stamp of commit, not original time stamps!
    Hints:
     > https://stackoverflow.com/questions/2171939/how-can-i-keep-the-original-file-commit-timestamp-on-subversion

Considerations:
 > Maintainability/Transparency/Comprehensibility
 > Speed

Decided to use B3) implemented with console applications written in C#.
(C# code is preferred over fiddeling with JavaScript.)
http://svn.code.sf.net/p/tortoisesvn/code/trunk/contrib/hook-scripts/
