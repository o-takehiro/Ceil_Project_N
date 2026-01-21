using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_EnemyDecisionData : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public int EnemyID;
		public float ClosePlayerDistance;
		public float FarPlayerDistance;
		public float MinCoolTime;
		public float MaxCoolTime;
		public int[] PlayerActiveMagic;
	}
}

