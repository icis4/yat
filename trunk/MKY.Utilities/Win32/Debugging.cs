//==================================================================================================
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Serial/Enums.cs $
// $Author: maettu_this $
// $Date: 2010-02-23 00:18:29 +0100 (Di, 23 Feb 2010) $
// $Revision: 254 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
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
using System.Runtime.InteropServices;

#endregion

namespace MKY.Utilities.Win32
{
    /// <summary>
    /// Encapsulates parts of the Win32 debugging API.
    /// </summary>
    /// <remarks>
    /// This class is partly based on GenericHid of Jan Axelson's Lakeview Research. Visit GenericHid
    /// on http://www.lvr.com/hidpage.htm for details.
    /// MKY.Utilities.Win32 needs to modify the structure and contents of GenericHid due to the
    /// following reasons:
    /// - Suboptimal structure of the original GenericHid project
    /// - Missing features required for YAT
    /// - Potential reuse of this class for other services relying on the Win32 API
    /// </remarks>
    public static class Debugging
    {
        #region Constants
        //==========================================================================================
        // Constants
        //==========================================================================================

        private const Int16 FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;

        #endregion

        #region External Function Declaration
        //==========================================================================================
        // External Function Declaration
        //==========================================================================================

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 FormatMessage(Int32 dwFlags, ref Int64 lpSource, Int32 dwMessageId, Int32 dwLanguageZId, String lpBuffer, Int32 nSize, Int32 Arguments);

        #endregion

        #region Methods
        //==========================================================================================
        // Methods
        //==========================================================================================

        /// <summary>
        /// Get text that describes the result of an API call.
        /// </summary>
        /// <param name="functionName"> the name of the API function.</param>
        /// <returns>The resulting text.</returns>
        public static string ResultOfAPICall(string functionName)
        {
            Int32 bytes = 0;
            Int32 resultCode = 0;
            String resultString = "";

            resultString = new String(Convert.ToChar(0), 129);

            // Get the result code for the last API call.
            resultCode = System.Runtime.InteropServices.Marshal.GetLastWin32Error();

            // Get the result message that corresponds to the code.
            Int64 temp = 0;
            bytes = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, ref temp, resultCode, 0, resultString, 128, 0);

            // Subtract two characters from the message to strip EOL.
            int eolLength = Environment.NewLine.Length;
            if (bytes > eolLength)
                resultString = resultString.Remove(bytes - eolLength, eolLength);

            // Create the String to return.
            resultString = Environment.NewLine +
                "Win32 API function = " + functionName + Environment.NewLine +
                "Win32 API result = "   + resultString + Environment.NewLine;

            return (resultString);
        }

        #endregion
    }
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Usb/UsbDeviceCollection.cs $
//==================================================================================================
