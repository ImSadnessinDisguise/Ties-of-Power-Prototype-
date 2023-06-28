using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header OBJECT REFERENCE
    [Space(10)]
    [Header("OBJECT REFERENCE")]
    #endregion

    #region Tooltip
    [Space(10)]
    [Tooltip("Populate with the box collider component on the DoorCollider gameObject")]
    [SerializeField] private BoxCollider2D doorCollider;
    #endregion

    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator animator;

    private void Awake()
    {
        //disable door collider by default
        doorCollider.enabled = false;

        //load component
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        //when parent gameobject is disabled(when player moves far enough away from the room)
        //the animator state gets reset, therefore reset it
        animator.SetBool(Settings.open, isOpen);
    }

    /// <summary>
    /// Open the door
    /// </summary>
    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            //Set open parameters in animator
            animator.SetBool(Settings.open, true);
        }
    }

    /// <summary>
    /// Lock the door
    /// </summary>
    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        // set open to false to close
        animator.SetBool(Settings.open, false);
    }

    /// <summary>
    /// Unlock the door
    /// </summary>
    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (previouslyOpened == true)
        {
            isOpen = false;
            OpenDoor();
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif
    #endregion

}
