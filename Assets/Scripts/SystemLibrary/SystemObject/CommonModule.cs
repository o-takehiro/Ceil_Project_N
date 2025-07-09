using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 汎用処理クラス
/// </summary>
public class CommonModule {

    /// <summary>
    /// リストが空か判定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    /// <returns></returns>
    public static bool IsEmpty<T>(List<T> _list) {
        return _list == null || _list.Count == 0;
    }

    public static bool IsEmpty<T>(T[] _array) {
        return _array == null || _array.Length <= 0;
    }


    /// <summary>
    /// リストに対して有効なインデックスか判定
    /// </summary>
    /// <returns></returns>
    public static bool IsEnableIndex<T>(List<T> _list, int _index) {
        if (IsEmpty(_list)) return false;

        return _index >= 0 && _list.Count > _index;
    }

    public static bool IsEnableIndex<T>(T[] _array, int _index) {
        if (IsEmpty(_array)) return false;
        return _index >= 0 && _array.Length > _index;
    }

    /// <summary>
    /// リストを初期化する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    /// <param name="_capacity"></param>
    public static void InitializeList<T>(ref List<T> _list, int _capacity = -1) {
        if (_list == null) {
            if (_capacity < 1) {
                _list = new List<T>();
            }
            else {
                _list = new List<T>(_capacity);
            }
        }
        else {
            if (_list.Capacity < _capacity) _list.Capacity = _capacity;
            _list.Clear();
        }
    }

    /// <summary>
    /// リストを重複なしでマージ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_main"></param>
    /// <param name="_sub"></param>
    public static void MeargeList<T>(ref List<T> _main, List<T> _sub) {
        if (IsEmpty(_sub)) return;

        int meargeCount = _sub.Count;
        if (_main == null) _main = new List<T>(meargeCount);

        for (int i = 0; i < meargeCount; i++) {
            // 重複した要素は追加しない
            if (_main.Exists(mainElem => mainElem.Equals(_sub[i]))) continue;
            _main.Add(_sub[i]);
        }

    }


    /// <summary>
    /// /// 複数のタスクの終了を待つ
    /// </summary>
    /// <param name="_taskList"></param>
    /// <returns></returns>
    public static async UniTask WaitTask(List<UniTask> _taskList) {
        // 終了したらタスクを知るとから除き、リストが空になるまで待つ
        while (!IsEmpty(_taskList)) {
            // 途中で要素が抜ける可能性があるので末尾から走査
            for (int i = _taskList.Count - 1; i >= 0; i--) {
                if (!_taskList[i].Status.IsCompleted()) continue;
                // タスクが終了していたらリストから抜く
                _taskList.RemoveAt(i);
            }
            await UniTask.DelayFrame(1);
        }
    }


    /// <summary>
    /// /// 複数のタスクの終了を待つ
    /// </summary>
    /// <param name="_taskList"></param>
    /// <returns></returns>
    public static async UniTask WaitTask(List<UniTask> _taskList, CancellationToken _token) {
        // 終了したらタスクを知るとから除き、リストが空になるまで待つ
        while (!IsEmpty(_taskList)) {
            // 途中で要素が抜ける可能性があるので末尾から走査
            for (int i = _taskList.Count - 1; i >= 0; i--) {
                if (!_taskList[i].Status.IsCompleted()) continue;
                // タスクが終了していたらリストから抜く
                _taskList.RemoveAt(i);
            }
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _token);
        }
    }


}
