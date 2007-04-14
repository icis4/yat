using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Settings.Document
{
	public struct SaveInfo
	{
		[XmlElement("TimeStamp")]
		public DateTime TimeStamp;

		[XmlElement("UserName")]
		public string UserName;

		public SaveInfo(DateTime timeStamp, string userName)
		{
			TimeStamp = timeStamp;
			UserName = userName;
		}
	}
}
