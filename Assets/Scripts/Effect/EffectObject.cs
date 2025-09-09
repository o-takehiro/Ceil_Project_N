using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour {
	// 使用中オブジェクトの親オブジェクト
	[SerializeField]
	private Transform _useObjectRoot = null;
	// 未使用オブジェクトの親オブジェクト
	[SerializeField]
	private Transform _unuseObjectRoot = null;

	// 魔法用のオブジェクトの生成数
	public const int _GENERATE_OBJECTS_MAX = 16;

	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize() {
		// 各魔法オブジェクトの準備
		//defenseObject = Instantiate(originDefense, _unuseObjectRoot);
		//miniBulletObjects = new List<GameObject>(_GENERATE_OBJECTS_MAX);
		//for (int i = 0, max = _GENERATE_OBJECTS_MAX; i < max; i++) {
		//	miniBulletObjects.Add(Instantiate(originMiniBullet, _unuseMagicRoot));
		//}
	}
}
