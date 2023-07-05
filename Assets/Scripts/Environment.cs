using UnityEngine;

[DisallowMultipleComponent]
public class Environment : MonoBehaviour
{
    // Attach this class to environment game objects whose lighting gets faded in

    #region Header References
    [Space(10)]
    [Header("References")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the Mesh Renderer component on the prefab")]
    #endregion

    public MeshRenderer meshRenderer;

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(meshRenderer), meshRenderer);
    }

#endif

    #endregion Validation

}
