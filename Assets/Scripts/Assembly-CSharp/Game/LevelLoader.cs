using System;
using System.Collections.Generic;
using System.Reflection;
using Game.IO;
using Game.Util;
using UnityEngine;

namespace Game
{
	public class LevelLoader
	{
		private class InstanceData
		{
			public GameObject InstanceObject { get; set; }

			public LevelBlock Block { get; set; }
		}

		public const LevelFormat LevelLoadFormat = LevelFormat.bytes;

		public const LevelFormat LevelSaveFormat = LevelFormat.bytes;

		public const string LevelRootObjectName = "Level:root";

		private IPrefabInstantiator m_instantiator;

		private List<InstanceData> m_intatiatedObjects;

		private Dictionary<int, GameObject> m_oldIdToInstance;

		public LevelLoader(IPrefabInstantiator instantiator)
		{
			m_instantiator = instantiator;
		}

		public void LoadLevel(LevelParameters info, LevelFormat type)
		{
			ILevelReader levelReader = LevelReaderFactory.Construct(type);
			TextAsset data = (TextAsset)Resources.Load("SceneObjects/" + info.RootPath, typeof(TextAsset));
			LevelRootBlock root = levelReader.ReadRoot(data);
			if (info == null)
			{
				Debug.LogError("Null returned!!");
				return;
			}
			m_oldIdToInstance = new Dictionary<int, GameObject>();
			m_intatiatedObjects = new List<InstanceData>();
			GameObject gameObject = GameObject.Find("Level:root");
			if (!gameObject)
			{
				Logger.Log("Could not find level root!");
				return;
			}
			InstantiateLevelBlocks(gameObject, root);
			RestoreComponentDataForAll();
		}

		private void InstantiateLevelBlocks(GameObject baseObject, ILevelItem root)
		{
			GameObject gameObject = null;
			if (root.GetType() == typeof(LevelBlock))
			{
				LevelBlock levelBlock = (LevelBlock)root;
				string text = levelBlock.Resource.ToString();
				if (text.Length > 0)
				{
					if (text.Contains("."))
					{
						text = text.Substring(0, text.LastIndexOf("."));
					}
					UnityEngine.Object @object = Resources.Load(text, typeof(GameObject));
					if ((bool)@object)
					{
						gameObject = m_instantiator.Instantiate(@object);
						m_intatiatedObjects.Add(new InstanceData
						{
							InstanceObject = gameObject,
							Block = levelBlock
						});
					}
					else
					{
						Logger.Error("Could not find resource: " + text + " for instantiation!!");
					}
				}
				else
				{
					gameObject = new GameObject();
				}
				if ((bool)gameObject)
				{
					Transform transform = gameObject.transform;
					transform.parent = baseObject.transform;
					transform.localPosition = levelBlock.Position;
					transform.localRotation = levelBlock.Rotation;
					transform.localScale = levelBlock.Scale;
					gameObject.name = levelBlock.Resource.Name;
					m_oldIdToInstance.Add(levelBlock.Resource.TransformInstanceId, gameObject);
				}
				{
					foreach (LevelBlock child in root.Children)
					{
						InstantiateLevelBlocks(gameObject, child);
					}
					return;
				}
			}
			foreach (LevelBlock child2 in root.Children)
			{
				InstantiateLevelBlocks(baseObject, child2);
			}
		}

		private void RestoreComponentDataForAll()
		{
			foreach (InstanceData intatiatedObject in m_intatiatedObjects)
			{
				GameObject go = intatiatedObject.InstanceObject;
				LevelBlock block = intatiatedObject.Block;
				RestoreSupportedComponentData(ref go, ref block);
			}
			m_intatiatedObjects.Clear();
		}

		private void RestoreSupportedComponentData(ref GameObject go, ref LevelBlock block)
		{
			Component[] components = go.GetComponents<Component>();
			List<PBComponent> list = new List<PBComponent>(block.PBComponents);
			Component[] array = components;
			foreach (Component component in array)
			{
				MemberInfo type = component.GetType();
				List<object> list2 = new List<object>(type.GetCustomAttributes(true));
				PBSerialize serializeAttribute = list2.Find((object x) => x is PBSerialize) as PBSerialize;
				if (serializeAttribute != null)
				{
					PBComponent pBComponent = list.Find((PBComponent x) => x.Type == serializeAttribute.Name);
					if (pBComponent != null)
					{
						DeserializeComponent(component, pBComponent);
					}
				}
			}
			foreach (LevelBlockComponent component3 in block.Components)
			{
				if (component3.Type == TargetZoneComponent.TYPE)
				{
					TargetZone component2 = go.GetComponent<TargetZone>();
					if (component3.Data.ContainsKey(TargetZoneComponent.Property_ZoneId))
					{
						component2.zoneId = component3.Data[TargetZoneComponent.Property_ZoneId];
					}
					if (!component3.Data.ContainsKey(TargetZoneComponent.Property_RequiresStay))
					{
					}
				}
			}
		}

		private void DeserializeComponent(Component gameComponent, PBComponent serializedComponent, bool forcePublicDeserialization = false)
		{
			Dictionary<Type, Action<MemberInfo, object, PBAbstractValue>> dictionary = DeserializeFieldActions();
			List<MemberInfo> list = new List<MemberInfo>(gameComponent.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public));
			list.AddRange(gameComponent.GetType().GetFields());
			foreach (MemberInfo item in list)
			{
				List<object> list2 = new List<object>(item.GetCustomAttributes(true));
				if (!forcePublicDeserialization && !list2.Exists((object x) => x is PBSerializeField))
				{
					continue;
				}
				PBAbstractValue value = null;
				serializedComponent.Fields.TryGetValue(item.Name, out value);
				if (value == null)
				{
					continue;
				}
				if (MemberType(item) == typeof(Component))
				{
					PBValue<Component_Proxy> pBValue = value as PBValue<Component_Proxy>;
					object memberValue = GetMemberValue<object>(item, gameComponent);
					DeserializeComponent((Component)memberValue, pBValue.Value.PBData, true);
					continue;
				}
				Action<MemberInfo, object, PBAbstractValue> value2 = null;
				if (dictionary.TryGetValue(MemberType(item), out value2))
				{
					value2(item, gameComponent, value);
				}
				else
				{
					Debug.LogError("No matching type found from serialized data:" + item.Name);
				}
			}
		}

		private Dictionary<Type, Action<MemberInfo, object, PBAbstractValue>> DeserializeFieldActions()
		{
			Dictionary<Type, Action<MemberInfo, object, PBAbstractValue>> dictionary = new Dictionary<Type, Action<MemberInfo, object, PBAbstractValue>>();
			dictionary.Add(typeof(string), delegate(MemberInfo memberInfo, object targetObject, PBAbstractValue abstractValue)
			{
				PBValue<string> pBValue8 = abstractValue as PBValue<string>;
				SetMemberValue(memberInfo, targetObject, pBValue8.Value);
			});
			dictionary.Add(typeof(int), delegate(MemberInfo memberInfo, object targetObject, PBAbstractValue abstractValue)
			{
				PBValue<int> pBValue7 = abstractValue as PBValue<int>;
				SetMemberValue(memberInfo, targetObject, pBValue7.Value);
			});
			dictionary.Add(typeof(bool), delegate(MemberInfo memberInfo, object targetObject, PBAbstractValue abstractValue)
			{
				PBValue<bool> pBValue6 = abstractValue as PBValue<bool>;
				SetMemberValue(memberInfo, targetObject, pBValue6.Value);
			});
			dictionary.Add(typeof(float), delegate(MemberInfo memberInfo, object targetObject, PBAbstractValue abstractValue)
			{
				PBValue<float> pBValue5 = abstractValue as PBValue<float>;
				SetMemberValue(memberInfo, targetObject, pBValue5.Value);
			});
			dictionary.Add(typeof(Vector3), delegate(MemberInfo memberInfo, object targetObject, PBAbstractValue abstractValue)
			{
				PBValue<LevelBlock.Data_Vector3> pBValue4 = abstractValue as PBValue<LevelBlock.Data_Vector3>;
				SetMemberValue(memberInfo, targetObject, (Vector3)pBValue4.Value);
			});
			dictionary.Add(typeof(GameObjectId), delegate(MemberInfo memberInfo, object targetObject, PBAbstractValue abstractValue)
			{
				PBValue<int> pBValue3 = abstractValue as PBValue<int>;
				GameObject gameObject = GameObjectId.Find(pBValue3.Value);
				SetMemberValue(memberInfo, targetObject, gameObject.GetComponent<GameObjectId>());
			});
			dictionary.Add(typeof(Transform), delegate(MemberInfo memberInfo, object targetObject, PBAbstractValue abstractValue)
			{
				PBValue<Transform_Proxy> pBValue2 = abstractValue as PBValue<Transform_Proxy>;
				Transform_Proxy value2 = pBValue2.Value;
				GameObject value3 = null;
				m_oldIdToInstance.TryGetValue(value2.Id, out value3);
				if ((bool)value3)
				{
					SetMemberValue(memberInfo, targetObject, value3.transform);
				}
			});
			dictionary.Add(typeof(List<Vector3>), delegate(MemberInfo memberInfo, object targetObject, PBAbstractValue abstractValue)
			{
				PBValue<PBList<LevelBlock.Data_Vector3>> pBValue = abstractValue as PBValue<PBList<LevelBlock.Data_Vector3>>;
				PBList<LevelBlock.Data_Vector3> value = pBValue.Value;
				List<Vector3> list = new List<Vector3>();
				foreach (LevelBlock.Data_Vector3 item in value.List)
				{
					list.Add(item);
				}
				SetMemberValue(memberInfo, targetObject, list);
			});
			return dictionary;
		}

		private Type MemberType(MemberInfo mi)
		{
			if (mi is FieldInfo)
			{
				FieldInfo fieldInfo = mi as FieldInfo;
				return fieldInfo.FieldType;
			}
			if (mi is PropertyInfo)
			{
				PropertyInfo propertyInfo = mi as PropertyInfo;
				return propertyInfo.PropertyType;
			}
			return null;
		}

		private T GetMemberValue<T>(MemberInfo mi, object source)
		{
			if (mi is FieldInfo)
			{
				FieldInfo fieldInfo = mi as FieldInfo;
				return (T)fieldInfo.GetValue(source);
			}
			if (mi is PropertyInfo)
			{
				PropertyInfo propertyInfo = mi as PropertyInfo;
				return (T)propertyInfo.GetValue(source, null);
			}
			return default(T);
		}

		private void SetMemberValue(MemberInfo mi, object target, object value)
		{
			if (mi is FieldInfo)
			{
				FieldInfo fieldInfo = mi as FieldInfo;
				fieldInfo.SetValue(target, value);
			}
			else if (mi is PropertyInfo)
			{
				PropertyInfo propertyInfo = mi as PropertyInfo;
				propertyInfo.SetValue(target, value, null);
			}
		}
	}
}
