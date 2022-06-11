using UnityEngine;
using System.Collections;

namespace atomtwist.AudioNodes
{
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		private static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null)
					_instance = GameObject.FindObjectOfType<T>();
			
				return _instance;
			}
			set
			{
				if (_instance != null)
				{
					_instance = value;
				}
			}
		}
	
		void Awake()
		{
			_instance = GameObject.FindObjectOfType<T>();
		}
	
	}
}
