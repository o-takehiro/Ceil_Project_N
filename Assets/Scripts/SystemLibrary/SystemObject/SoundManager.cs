using Cysharp.Threading.Tasks;
using UnityEngine;

using static CommonModule;

/// <summary>
/// サウンドの管理
/// </summary>
public class SoundManager : SystemObject {

    // BGM再生用オーディオソース
    [SerializeField]
    private AudioSource _bgmAudioSource = null;
    // SE再生用コンポーネント
    [SerializeField]
    private AudioSource[] _seAudioSource = null;

    // BGMのリスト
    [SerializeField]
    private BGMAssign _bgmAssign = null;
    // SEのリスト
    [SerializeField]
    private SEAssign _seAssign = null;

    /// <summary>
    /// 自身への参照
    /// </summary>
    public static SoundManager Instance { get; private set; } = null;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override async UniTask Initialize() {
        Instance = this;
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="bgmID"></param>
    public void PlayBGM(int bgmID) {
        if (!IsEnableIndex(_bgmAssign.bgmArray, bgmID)) return;

        _bgmAudioSource.clip = _bgmAssign.bgmArray[bgmID];
        _bgmAudioSource.Play();
    }

    /// <summary>
    /// BGM停止
    /// </summary>
    public void StopBGM() {
        _bgmAudioSource.Stop();
    }


    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="seID"></param>
    public void PlaySE(int seID) {
        if (!IsEnableIndex(_seAssign.seArray, seID)) return;

        // 再生中ではないオーディオソースを探してそれで再生
        for (int i = 0, max = _seAudioSource.Length; i < max; i++) {
            AudioSource audioSource = _seAudioSource[i];
            if (audioSource == null ||
                audioSource.isPlaying) continue;
            // 再生中でないオーディオソースが見つかったので再生
            audioSource.clip = _seAssign.seArray[seID];
            audioSource.Play();
            // SEの終了待ち
            //while (audioSource.isPlaying) await UniTask.DelayFrame(1);

            return;
        }

    }
}
