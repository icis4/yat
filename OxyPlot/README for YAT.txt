
OxyPlot for YAT
***************


Background
----------
Initially, OxyPlot 1.0.0 code was used, as that is the last version to support .NET 4.0, based on source code.
Then, with upgrade to .NET 4.8, upgraded to OxyPlot 2.0.0, now based on binaries.
    But, OxyPlot 2.0.0 no longer supports Visual Studio 2015 projects, thus excluded from solution,
    i.e. source code debugging is no longer possible. Still, the source is committed for reference.


Future
------
Upgrade to OxyPlot 2.1.0+ is tracked in YAT FR#420.


HowTo Upgrade
-------------
 1. Download release including extensions, documentation and source code.
 2. Use NuGet (e.g. via "<LocalFolder>\NuGet\DummySolution") to get binaries.
 3. Archive the release in ".\!-Packages".
 4. Copy "OxyPlot*.*" to ".\Binaries".
 5. In a temporary location, extract source (oxyplot-<version>.zip).
 6. Copy *CAPITAL* from "<oxyplot-<version>>" to ".\".
 7. Update "\.Icons" as required.
 8. Delete all files and folders and(except "\.vs") of ".\Source".
 9. Copy the required files from "<oxyplot-<version>>" to ".\Source".
10. 'SVN:Add|Rename|Delete' as required.
11. Open the YAT solution.
N/A Build the "Debug Test with Binaries Source" configuration for "YAT".
12. Build the "Debug Test" configuration.
13. Update the YAT release notes:
    > Update the OxyPlot version.
    > Check whether bug fixes or change requests have been fully fixed.
    > Check whether limitations can be removed from the release notes.
14. Commit changes to SVN.


is tied to .NET 4.5, issue #1452 "Again support .NET 4.0 for WinForms and WPF" is pending for OxyPlot 2.1.0.


 > Modifications were needed to several of the '*_NET40' projects (see previous revisions of this README).
 > Minor project/solution fixes were applied.


Modifications
-------------
1. Unnecessary references (PresentationCore, PresentationFramework, System.Xaml, WindowsBase) removed from:
    > \Source\OxyPlot\OxyPlot_NET40.csproj
2. x64/x86 configurations added to:
    > \Source\OxyPlot\OxyPlot_NET40.csproj                                     => required for integrating projects into YAT
    > \Source\OxyPlot.WindowsForms\OxyPlot.WindowsForms_NET40.csproj           => required for integrating projects into YAT
    > \Source\Examples\ExampleLibrary\ExampleLibrary_NET40.csproj              => for verification purposes only
    > \Source\Examples\WindowsForms\ExampleBrowser\ExampleBrowser_NET40.csproj => for verification purposes only
    > \Source\Examples\WindowsForms\WindowsFormsDemo\WindowsFormsDemo.csproj   => for verification purposes only
    > \Source\OxyPlot.WindowsForms.sln                                         => for verification purposes only
3. Minor project/solution fixes.


Integration
-----------
YAT directly uses the projects (doesn't use the released DLLs) for...
...not having the undesired references mentioned above.
...explicitly using x64/x86 builds.

Disadvantage:
 > OxyPlot version has to be incremented for each YAT release (same as for ALAZ,...).


HowTo Use Source
----------------
 1. In 'YAT.Model' and 'YAT.View.Forms', replace the references to 'OxyPlot*.dll' by 'OxyPlot*' projects.
 2. Debugging can then be done using the [Debug Test with Binaries Source] configuration.
    (A separate solution configuration is required to get the 'OxyPlot*' projects built.)
 3. Don't forget to restore the references after debugging.


Hosting
-------
 > Project hosted at "https://github.com/oxyplot/oxyplot".
   Online help hosted at "https://oxyplot.readthedocs.io/en/latest/".


Release Management
------------------
 > Use "https://github.com/oxyplot/oxyplot/releases.atom" to get release notifiations
   with e.g. "https://support.mozilla.org/en-US/kb/how-subscribe-news-feeds-and-blogs"


Issue Management
----------------
 > "https://github.com/oxyplot/oxyplot/issues".
 > Issue #1452 created by mky -aka- maettu_this.
 > Issue has been archived to "\Issues" for future reference.


----------------
2020-11-15 / MKY
2020-01-14 / MKY
