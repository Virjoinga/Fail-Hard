using System.Collections.Generic;
using System.Reflection;

namespace Game.Util
{
	public class Debugger
	{
		public class ObjectInspectionView
		{
			public string Type { get; private set; }

			public Dictionary<string, object> Fields { get; private set; }

			public ObjectInspectionView(string type, Dictionary<string, object> fields)
			{
				Type = type;
				Fields = fields;
			}
		}

		private static List<object> s_objects = new List<object>();

		public static void AddObject(object target)
		{
			if (!s_objects.Contains(target))
			{
				s_objects.Add(target);
			}
		}

		public static void Clear()
		{
			s_objects.Clear();
		}

		public static bool HasObject()
		{
			return s_objects.Count > 0;
		}

		public static IEnumerable<ObjectInspectionView> GetInspectorView()
		{
			for (int i = 0; i < s_objects.Count; i++)
			{
				object obj = s_objects[i];
				string type = obj.GetType().ToString();
				FieldInfo[] fis = obj.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				Dictionary<string, object> fields = new Dictionary<string, object>();
				FieldInfo[] array = fis;
				foreach (FieldInfo fi in array)
				{
					fields.Add(fi.Name, fi.GetValue(obj));
				}
				yield return new ObjectInspectionView(type, fields);
			}
		}
	}
}
