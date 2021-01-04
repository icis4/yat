#===================================================================================================
# YAT - Yet Another Terminal.
# Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
# Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
# -------------------------------------------------------------------------------------------------
# $URL$
# $Revision$
# $Date$
# $Author$
# -------------------------------------------------------------------------------------------------
# See release notes for product version details.
# See SVN change log for file revision details.
# Author(s): Matthias Klaey
# -------------------------------------------------------------------------------------------------
# Copyright © 2003-2021 Matthias Kläy.
# All rights reserved.
# -------------------------------------------------------------------------------------------------
# This source code is licensed under the GNU LGPL.
# See http://www.gnu.org/licenses/lgpl.html for license details.
#===================================================================================================


#===================================================================================================
# Global interface declaration
#===================================================================================================

<#
.SYNOPSIS
Counts the number of lines-of-code of the whole YAT project.

.DESCRIPTION
Counted are LOC (physical), SLOC (source only) and CLOC (comments only). Evaluation takes various
scopes into account:
 > 3rd party code.
 > Generated code (e.g. Visual Studio generated .designer files).
 > Example code.
 > Test code.

The implementation is based on the simple PowerShell command line...
(Get-ChildItem -Include *.cs -Recurse | Select-String "^(\s*)//" -NotMatch | Select-String "^(\s*)$" -NotMatch).Count
...as documented at https://www.limilabs.com/blog/source-lines-of-code-count-using-powershell.

To use this approach on any other project, it can be used as e.g.
 > $files = Get-ChildItem -File -Include @("*.h","*.hpp","*.c","*.cpp") -Recurse
 > $fileCount = ($files).Count
 > $locCount  = ($files | Get-Content | Measure-Object -Line).Lines
 > $slocCount = ($files | Select-String "^(\s*)//" -NotMatch | Select-String "^(\s*)$" -NotMatch).Count
 > $clocCount = ($files | Select-String "//").Count
 > $message  = "$fileCount files`n"
 > $message += "$locCount LOC`n"
 > $message += "$slocCount SLOC`n"
 > $message += "$clocCount CLOC"
 > Write-Host $message

Alternatives:
 > https://github.com/AlDanial/cloc (quite powerful, but way more complex than needed for YAT).
 > Others as listed at https://github.com/AlDanial/cloc#Other_Counters (not evaluated).

.OUTPUTS
Returns a hash table of the counts of the whole YAT project, including 3rd party and generated.
#>

[CmdletBinding()]

#---------------------------------------------------------------------------------------------------
# Input parameters
#---------------------------------------------------------------------------------------------------

param
(
)
# If the script has no parameters, still keep "param()"!
# Otherwise, 'Verbose' and 'Debug' will not be available!

# Read the 'Verbose' and 'Debug' parameters into variables for easier processing:
$verbose = $null
if (!$PSBoundParameters.TryGetValue('Verbose', [ref]$verbose)) { $verbose = $false }
$debug = $null
if (!$PSBoundParameters.TryGetValue('Debug', [ref]$debug)) { $debug = $false }


#===================================================================================================
# Local function implementation
#===================================================================================================

#---------------------------------------------------------------------------------------------------
# Create-Counts
#---------------------------------------------------------------------------------------------------
<#
.SYNOPSIS
Creates a hash table usable to count LOC (physical), SLOC (source only) and CLOC (comments only).

.OUTPUTS
Returns a new hash table with all counts initialized to 0.
#>
function Create-Counts()
{
	$result = @{}
	$result.Add("Files", 0)
	$result.Add("LOC",   0)
	$result.Add("SLOC",  0)
	$result.Add("CLOC",  0)

	Write-Output $result
}


#---------------------------------------------------------------------------------------------------
# Add-Counts
#---------------------------------------------------------------------------------------------------
<#
.SYNOPSIS
Add the counts of the given tables together.

.OUTPUTS
Returns the summed up hash table.
#>
function Add-Counts([HashTable]$tableA, [HashTable]$tableB)
{
	$tableA["Files"] += $tableB["Files"]
	$tableA["LOC"]   += $tableB["LOC"]
	$tableA["SLOC"]  += $tableB["SLOC"]
	$tableA["CLOC"]  += $tableB["CLOC"]

	Write-Output $tableA
}


#---------------------------------------------------------------------------------------------------
# Count-LOC
#---------------------------------------------------------------------------------------------------
<#
.SYNOPSIS
Counts LOC (physical), SLOC (source only) and CLOC (comments only) for the current path.

.PARAMETER SomeMandatoryParam
This is a mandatory value parameter.

.OUTPUTS
Returns a hash table of the counts.
#>
function Count-LOC([Switch]$GeneratedOnly)
{
	# Count:
	$Patterns = @("*.Designer.cs", "*.Generated.cs")
	if ($GeneratedOnly) {
		$files = Get-ChildItem -File -Include $Patterns -Recurse
	}
	else {
		$files = Get-ChildItem -File -Include *.cs -Exclude $Patterns -Recurse
	}

	$fileCount = ($files).Count
	$locCount  = ($files | Get-Content | Measure-Object -Line).Lines
	$slocCount = ($files | Select-String "^(\s*)//" -NotMatch | Select-String "^(\s*)$" -NotMatch).Count
	$clocCount = ($files | Select-String "//").Count

	# Compose the result:
	$result = Create-Counts
	$result["Files"] = $fileCount
	$result["LOC"]   = $locCount
	$result["SLOC"]  = $slocCount
	$result["CLOC"]  = $clocCount

	if ($verbose) {
		$message = $result.GetEnumerator() | Sort Value | Out-String
		$message = $message -Replace "`n", "" -Replace "`r", ""
		Write-Verbose $message
	}

	# Return the result:
	Write-Output $result
}


#===================================================================================================
# Main
#===================================================================================================

#---------------------------------------------------------------------------------------------------
# Info
#---------------------------------------------------------------------------------------------------

Write-Verbose "This script counts the number of lines-of-code of the whole YAT project."


#---------------------------------------------------------------------------------------------------
# Preparation
#---------------------------------------------------------------------------------------------------

# Retrieve name and root:
$MY_NAME = $MyInvocation.MyCommand.Name
$MY_PATH = [System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Path)

# Save the initial location:
$START_DIRECTORY = Get-Location # 'Get-Location' is the PowerShell equivalent to 'pwd'.
Write-Verbose "Script has been started in..."
Write-Verbose "...""$START_DIRECTORY"""

# Go to project root:
Set-Location ..
$ROOT_DIRECTORY = Get-Location
Write-Verbose "Current directory changed to..."
Write-Verbose "...""$ROOT_DIRECTORY"""


#---------------------------------------------------------------------------------------------------
# Processing
#---------------------------------------------------------------------------------------------------

$thirdPartyPaths = @("ALAZ\Source", "CSScript\Source", "netrtfwriter\RtfWriter", "OxyPlot\Source")
$proprietaryPaths = @("MKY", "NUnit", "YAT")

Write-Host ""
Write-Host "3rd Party            (ALAZ, CSScript"
Write-Host "              netrtfwriter, OxyPlot)"
Write-Host "===================================="

$thirdPartyCounts = Create-Counts

foreach ($thirdPartyPath in $thirdPartyPaths) {
	Set-Location "$ROOT_DIRECTORY\$thirdPartyPath"
	$currentDirectory = Get-Location
	Write-Verbose "Current directory changed to..."
	Write-Verbose "...""$currentDirectory"""

	$currentCounts = Count-LOC
	$thirdPartyCounts = Add-Counts $thirdPartyCounts $currentCounts
}

$message = $thirdPartyCounts.GetEnumerator() | Sort Value | Out-String
Write-Host $message

Write-Host "Proprietary        (MKY, NUnit, YAT)"
Write-Host "===================================="

$proprietaryGeneratedCounts = Create-Counts
$proprietaryWrittenCounts   = Create-Counts
$proprietaryTestCounts      = Create-Counts

foreach ($proprietaryPath in $proprietaryPaths) {
	Set-Location "$ROOT_DIRECTORY\$proprietaryPath"
	$currentDirectory = Get-Location
	Write-Verbose "Current directory changed to..."
	Write-Verbose "...""$currentDirectory"""

	foreach ($currentPath in Get-ChildItem -Directory) {
		if ($currentPath.ToString().StartsWith("!-")) {
			continue
		}

		Set-Location "$ROOT_DIRECTORY\$proprietaryPath\$currentPath"
		$currentDirectory = Get-Location
		Write-Verbose "Current directory changed to..."
		Write-Verbose "...""$currentDirectory"""

		if ($currentPath.ToString().EndsWith(".Test")) {
			$currentCounts = Count-LOC
			$proprietaryTestCounts = Add-Counts $proprietaryTestCounts $currentCounts
		}
		else {
			$currentCounts = Count-LOC
			$proprietaryWrittenCounts = Add-Counts $proprietaryWrittenCounts $currentCounts

			$currentCounts = Count-LOC -GeneratedOnly
			$proprietaryGeneratedCounts = Add-Counts $proprietaryGeneratedCounts $currentCounts
		}
	}
}

Write-Host ""
Write-Host "Generated  (*.Designer|Generated.cs)"
Write-Host "------------------------------------"
$message = $proprietaryGeneratedCounts.GetEnumerator() | Sort Value | Out-String
Write-Host $message

Write-Host "Written                       (*.cs)"
Write-Host "------------------------------------"
$message = $proprietaryWrittenCounts.GetEnumerator() | Sort Value | Out-String
Write-Host $message

Write-Host "Test                (<Project>.Test)"
Write-Host "------------------------------------"
$message = $proprietaryTestCounts.GetEnumerator() | Sort Value | Out-String
Write-Host $message

$result = Create-Counts
$result = Add-Counts $result $thirdPartyCounts
$result = Add-Counts $result $proprietaryGeneratedCounts
$result = Add-Counts $result $proprietaryWrittenCounts
$result = Add-Counts $result $proprietaryTestCounts


#---------------------------------------------------------------------------------------------------
# Finishing
#---------------------------------------------------------------------------------------------------

# Restore the initial location:
Set-Location $START_DIRECTORY # 'Set-Location' is the PowerShell equivalent to 'cd'.
Write-Verbose "Current directory restored to..."
Write-Verbose "...""$START_DIRECTORY"""


#---------------------------------------------------------------------------------------------------
# Return
#---------------------------------------------------------------------------------------------------

if ($verbose) {
	$message = $result.GetEnumerator() | Sort Value | Out-String
	$message = $message -Replace "`n","" -Replace "`r",""
	Write-Verbose $message
}

Write-Host "           S U M M A R Y            "
Write-Host "===================================="

Write-Output $result


#===================================================================================================
# End of
# $URL$
#===================================================================================================
