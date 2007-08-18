using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Settings
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
