using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Valinta
{
	public static class VUtils
	{
		public static T Deserialize<T>(string xml)
		{
			T result = default(T);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			using (TextReader textReader = new StringReader(xml))
			{
				try
				{
					result = (T)xmlSerializer.Deserialize(textReader);
				}
				catch (FormatException ex)
				{
					Debug.Log("EXC: " + ex.StackTrace.ToString());
				}
			}
			return result;
		}
	}
}
