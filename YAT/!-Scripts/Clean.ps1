#===================================================================================================
# YAT - Yet Another Terminal.
# Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
# Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
# -------------------------------------------------------------------------------------------------
# $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/!-Scripts/CountLOC.ps1 $
# $Revision: 3823 $
# $Date: 2021-05-15 22:40:22 +0200 (Sa., 15 Mai 2021) $
# $Author: maettu_this $
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
Cleans directories and files in the YAT workspace.

.DESCRIPTION
This PowerShell script is the base for the accompanying batch scripts. Both result in the same.
The batch scripts exists for convenience as they can directly be invoked.

Notes:
 > This file is named according to batch naming conventions, not PowerShell (e.g. "Count-LOC").
 > If one day renaming this file, it shall be considered to rename the batch files as well.

.PARAMETER SelectAnalyzer
This is a switch parameter. Enable to select cleaning of analyzer artefacts.

.PARAMETER SelectOptions
This is a switch parameter. Enable to select cleaning of solution/project options.

.PARAMETER SelectSetup
This is a switch parameter. Enable to select cleaning of setup/installer project artefacts.

.PARAMETER SelectTemporaries
This is a switch parameter. Enable to select cleaning of project temporaries.

.PARAMETER SelectAll
This is a switch parameter. Enable to select cleaning of all of the possible selections.
#>

[CmdletBinding()]

#---------------------------------------------------------------------------------------------------
# Input parameters
#---------------------------------------------------------------------------------------------------

param
(
	[Parameter()]
	[Alias("Analyzer")]
	[Switch]
	$SelectAnalyzer,

	[Parameter()]
	[Alias("Options")]
	[Switch]
	$SelectOptions,

	[Parameter()]
	[Alias("Setup")]
	[Switch]
	$SelectSetup,

	[Parameter()]
	[Alias("Temporaries", "Temp")]
	[Switch]
	$SelectTemporaries,

	[Parameter()]
	[Alias("All")]
	[Switch]
	$SelectAll
)

# Read the 'Verbose' and 'Debug' parameters into variables for easier processing:
$verbose = $null
if (!$PSBoundParameters.TryGetValue('Verbose', [ref]$verbose)) { $verbose = $false }
$debug = $null
if (!$PSBoundParameters.TryGetValue('Debug', [ref]$debug)) { $debug = $false }


#===================================================================================================
# Local function implementation
#===================================================================================================

#---------------------------------------------------------------------------------------------------
# Clean-Analyzer
#---------------------------------------------------------------------------------------------------
function Clean-Analyzer()
{
	Write-Verbose "Retrieving StyleCop cache files..."
	$filePaths = Get-ChildItem "StyleCop.Cache" -File -Recurse # Items are returned as string[]
	if (@($filePaths).Count -gt 0) {
		foreach ($filePath in $filePaths) {
			Write-Verbose "...removing ""$filePath"""
			Remove-Item "$filePath" -Confirm:$false
		}
		Write-Verbose "...done"
	}
	else {
		Write-Verbose "...found none"
	}
}


#---------------------------------------------------------------------------------------------------
# Clean-Options
#---------------------------------------------------------------------------------------------------
function Clean-Options()
{
	$vsDirectory = ".\.vs\"
	if (Test-Path -Path $vsDirectory -PathType Container) {
		Write-Verbose "Removing solution user option directory..."
		Remove-Item "$vsDirectory" -Recurse -Confirm:$false  -Force # Force is needed for hidden files.
		Write-Verbose "...done"
	}

	Write-Verbose "Retrieving project user options..."
	$filePaths = Get-ChildItem "*.csproj.user" -File -Recurse # Items are returned as string[]
	if (@($filePaths).Count -gt 0) {
		foreach ($filePath in $filePaths) {
			Write-Verbose "...removing ""$filePath""..."
			Remove-Item "$filePath" -Confirm:$false

			# So far, no '.csproj.user' settings are required. If any every gets required, this script
			# will delete it => visible in source code management => this script will have to be adapted.
		}
		Write-Verbose "...done"
	}
	else {
		Write-Verbose "...found none"
	}
}


#---------------------------------------------------------------------------------------------------
# Clean-Setup
#---------------------------------------------------------------------------------------------------
function Clean-Setup()
{
	Write-Verbose "Retrieving setup output directories..."
	$directoryPaths = (Get-ChildItem -Directory -Recurse | where { $_.FullName -like "*\YAT\YAT.Setup\Setup.*" }).FullName # Items are returned as string[]
	if (@($directoryPaths).Count -gt 0) {
		foreach ($directoryPath in $directoryPaths) {
			Write-Verbose "...removing ""$directoryPath""..."
			Remove-Item "$directoryPath" -Recurse -Confirm:$false
		}
		Write-Verbose "...done"
	}
	else {
		Write-Verbose "...found none"
	}
}


#---------------------------------------------------------------------------------------------------
# Clean-Temporaries
#---------------------------------------------------------------------------------------------------
function Clean-Temporaries()
{
	Write-Verbose "Retrieving project files/directories..."
	$projectFilePaths = Get-ChildItem "*.csproj" -File -Recurse # Items are returned as string[]
	if (@($projectFilePaths).Count -gt 0) {
		foreach ($projectFilePath in $projectFilePaths) {
			$projectDirectoryPath = (Get-Item $projectFilePath).Directory.FullName

			Write-Verbose "...cleaning ""$projectDirectoryPath""..."

			$binDirectoryPath = "$projectDirectoryPath\bin"
			if (Test-Path -Path $binDirectoryPath -PathType Container) {
				Write-Verbose "...removing ""$binDirectoryPath""..."
				Remove-Item $binDirectoryPath -Recurse -Confirm:$false
			}

			$objDirectoryPath = "$projectDirectoryPath\obj"
			if (Test-Path -Path $objDirectoryPath -PathType Container) {
				Write-Verbose "...removing ""$objDirectoryPath""..."
				Remove-Item $objDirectoryPath -Recurse -Confirm:$false
			}
		}
		Write-Verbose "...done"
	}
	else {
		Write-Verbose "...found none"
	}

	# Additionally for ALAZ, in case the script is called from within the root directory:
	$alazDirectoryName = ".\ALAZ\"
	if (Test-Path -Path $alazDirectoryName -PathType Container) {
		Push-Location

		Write-Verbose "Removing ALAZ *.tmp files..."
		Set-Location $alazDirectoryName
		Remove-Item "*.tmp" -Recurse -Confirm:$false
		Write-Verbose "...done"

		Pop-Location
	}
}


#===================================================================================================
# Main
#===================================================================================================

#---------------------------------------------------------------------------------------------------
# Info
#---------------------------------------------------------------------------------------------------

Write-Verbose "This script cleans the selected items in the YAT solution directory."


#---------------------------------------------------------------------------------------------------
# Preparation
#---------------------------------------------------------------------------------------------------

# Retrieve name and root:
$MY_NAME = $MyInvocation.MyCommand.Name
$MY_PATH = [System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Path)

# Save the initial location:
$START_DIRECTORY = Get-Location
Write-Verbose "Script has been started in..."
Write-Verbose "...""$START_DIRECTORY"""

# Go to solution root:
Set-Location ..
$ROOT_DIRECTORY = Get-Location
Write-Verbose "Current directory changed to..."
Write-Verbose "...""$ROOT_DIRECTORY"""


#---------------------------------------------------------------------------------------------------
# Processing
#---------------------------------------------------------------------------------------------------

if ($SelectAll) {
	$SelectAnalyzer    = $true
	$SelectOptions     = $true
	$SelectSetup       = $true
	$SelectTemporaries = $true
}

if ($SelectAnalyzer) {
	Clean-Analyzer
}

if ($SelectOptions) {
	Clean-Options
}

if ($SelectSetup) {
	Clean-Setup
}

if ($SelectTemporaries) {
	Clean-Temporaries
}


#---------------------------------------------------------------------------------------------------
# Finishing
#---------------------------------------------------------------------------------------------------

# Restore the initial location:
Set-Location $START_DIRECTORY
Write-Verbose "Current directory restored to..."
Write-Verbose "...""$START_DIRECTORY"""


#===================================================================================================
# End of
# $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/!-Scripts/CountLOC.ps1 $
#===================================================================================================
