using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicObject : MonoBehaviour {
	// ユニークのID
	public int ID { get; private set; } = -1;

	public void Setup(int setID) {
		ID = setID;
		// 対応した子オブジェクトを表示
	}

	/// <summary>
	/// 片付け
	/// </summary>
	public void Teardown() {

	}

}
