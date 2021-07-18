
These scripts are a response to YAT feature request #429 'docklight file type' at
https://sourceforge.net/p/y-a-terminal/feature-requests/429/.

The scripts convert a Docklight settings file to .yat format, in best-effort manner.


Precondition
------------
YAT binaries of version 2.4.0 or above are available.


Usage
-----
1. Download the 5 files of this directory to a local folder.
    > !-ReadMe.txt                              -> This 'ReadMe'.
    > Convert-DocklightToYAT.cs                 -> A CS-Script based conversion script.
    > Convert-DocklightToYAT.ps1                -> The main script initiating the conversion.
    > cscs.exe                                  -> CS-Script required by the above two scripts.
    > cscs.exe corresponds to v3.30.5.2 release -> CS-Script related information.

   Or 'SVN > Checkout' the whole YAT repository to a local folder.

2. Place the Docklight settings file(s) (.ptp) into the local or an adjacent folder.
3. Open PowerShell on the local folder.
4. Run .\Convert-DocklightToYAT.ps1 with defaults or according to the script's documentation.
5. Retrieve the resulting YAT terminal file(s) (.yat).


Alternative usage (limited to a single file)
--------------------------------------------
.\cscs.exe -dir:"C:\Program Files\YAT" .\Convert-DocklightToYAT.cs ".\MyFile.ptp" ".\MyFile.yat"
