using UnityEngine;

[RequireComponent(typeof(DestroyedEvent))]
[DisallowMultipleComponent]
public class Destroyed : MonoBehaviour
{
    private DestroyedEvent destroyedEvent;
    

    private void Awake()
    {
        destroyedEvent = GetComponent<DestroyedEvent>();
    }


    private void OnEnable()
    {
        //Subscribe to destoyed Event
        destroyedEvent.OnDestroyed += DestroyedEvent_OnDestroyed;
    }

    private void OnDisable()
    {
        //Unsubscribe
        destroyedEvent.OnDestroyed -= DestroyedEvent_OnDestroyed;
    }

    private void DestroyedEvent_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        if (destroyedEventArgs.playerDied)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }


}
