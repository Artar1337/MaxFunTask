using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "Difficulty")]
public class ScriptableDifficulty : ScriptableObject
{
    // if in easy bot mode - true, else - false
    public bool _easyMode;
    // difficulty name (eng)
    public string _difficultyName;
    // planets count
    [Range(2,40)]
    public int _planetsCount;
    // bot reaction time
    public float _reactionTime;
    // background music
    public AudioClip _clip;
}
