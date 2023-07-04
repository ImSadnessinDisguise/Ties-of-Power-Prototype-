using UnityEngine;

[CreateAssetMenu(fileName = "Music Track", menuName = "Scriptable Objects/Sounds/Music Track")]
public class MusicTrackSO : ScriptableObject
{
    #region Header
    [Space(10)]
    [Header("Music Track Details")]
    #endregion

    #region Tooltip
    [Tooltip("The name of the music track")]
    #endregion
    public string musicName;

    #region Tooltip
    [Tooltip("The audio clip of the music track")]
    #endregion
    public AudioClip musicClip;

    #region Tooltip
    [Tooltip("The volume of the music track")]
    #endregion
    [Range(0, 1)]
    public float musicVolume = 1f;

    #region 
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(musicName), musicName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicClip), musicClip);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(musicVolume), musicVolume, true);
    }
#endif
    #endregion


}
