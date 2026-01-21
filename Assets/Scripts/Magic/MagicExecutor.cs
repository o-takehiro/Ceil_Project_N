using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static MagicUtility;
using static PlayerMagicAttack;
using static CharacterUtility;

public class MagicExecutor {
    // 発動する魔法
    private List<List<Action<MagicObject>>> _activeMagic = null;
    // 発動中の魔法ID
    private List<List<int>> _activeMagicIDList = null;
    //private eMagicType activeEnemyMagicID = eMagicType.Invalid;
    // コピーした魔法
    private List<eMagicType> _copyMagicList = null;
    

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="sideTypeMax"></param>
    public void Initialize(int sideTypeMax) {
        // 魔法の種類分のリストを生成しておく
        int magicTypeMax = (int)eMagicType.Max;
        _copyMagicList = new List<eMagicType>(magicTypeMax);

        // 発動中の魔法リストを魔法の種類分生成
        _activeMagicIDList = new List<List<int>>(sideTypeMax);
        for (int i = 0; i < sideTypeMax; i++) {
            _activeMagicIDList.Add(new List<int>(magicTypeMax));
            for (int magicCount = 0; magicCount < magicTypeMax; magicCount++) {
                // 未使用状態にしておく
                _activeMagicIDList[i].Add(-1);
            }
        }

        // 発動する魔法リストをある程度生成
        _activeMagic = new List<List<Action<MagicObject>>>(sideTypeMax);
        for (int i = 0; i < sideTypeMax; i++) {
            _activeMagic.Add(new List<Action<MagicObject>>(magicTypeMax));
            for (int magicCount = 0; magicCount < magicTypeMax; magicCount++) {
                // 未使用状態にしておく
                _activeMagic[i].Add(null);
            }
        }
    }

    /// <summary>
    /// 魔法実行処理
    /// </summary>
    public void MagicExecute() {
        if (_activeMagic == null) return;

        // 関数とIDが入っていれば関数実行
        for (int sideCount = 0; sideCount < (int)eSideType.Max - 1; sideCount++) {
            for (int i = 0, max = _activeMagicIDList[sideCount].Count; i < max; i++) {
                if (_activeMagic[sideCount][i] == null || _activeMagicIDList[sideCount][i] < 0) continue;
                _activeMagic[sideCount][i](GetMagicObject(_activeMagicIDList[sideCount][i]));
            }
        }
    }

    /// <summary>
    /// 発動魔法のIDの取得
    /// </summary>
    /// <param name="side"></param>
    /// <param name="magic"></param>
    /// <returns></returns>
    public int GetActiveMagicID(int side, int magic) {
        return _activeMagicIDList[side][magic];
    }

    /// <summary>
    /// 発動魔法のIDセット
    /// </summary>
    /// <param name="side"></param>
    /// <param name="magic"></param>
    /// <param name="setID"></param>
    public void SetActiveMagicID(int side, int magic, int setID) {
        _activeMagicIDList[side][magic] = setID;
    }

    /// <summary>
    /// 実行したい魔法関数を保存
    /// </summary>
    /// <param name="side"></param>
    /// <param name="magic"></param>
    /// <param name="action"></param>
    public void SetActiveMagic(int side, int magic, Action<MagicObject> action) {
        _activeMagic[side][magic] = action;
    }

    /// <summary>
    /// 解析魔法実行
    /// </summary>
    public void AnalysisMagicExecute() {
        UniTask task = EffectManager.Instance.PlayEffect(eEffectType.Analysis, GetEnemyCenterPosition());
        _copyMagicList = GetMagicStorageSlot();
        // SE再生
        SoundManager.Instance.PlaySE((int)eSEType.Analysis);
        int enemy = (int)eSideType.EnemySide;
        // 発動中の魔法を探す
        for (int magic = 0, magicMax = _activeMagicIDList[enemy].Count; magic < magicMax; magic++)
        {
            // 魔法発動中かつ、コピー済みでなければセット
            if (!GetMagicActive(enemy, magic) || GetMagicCopied(magic)) continue;
            SetMagicStorageSlot((eMagicType)magic);
            // SE再生
            SoundManager.Instance.PlaySE((int)eSEType.GetMagic);
            return;
        }
    }

    /// <summary>
	/// 特定の魔法を既にコピーしているかどうか
	/// </summary>
	/// <returns></returns>
	private bool GetMagicCopied(int magicID) {
        for (int i = 0, max = _copyMagicList.Count; i < max; i++) {
            if ((int)_copyMagicList[i] == magicID) return true;
        }
        return false;
    }
}
