using UnityEngine;

public class DestroyWithTarget : MonoBehaviour
{
    [Header("Target Object")]
    [Tooltip("The GameObject this object should follow. If this target is destroyed, this object will also be destroyed.")]
    public GameObject targetObject;

    void Update()
    {
        // In Unity, when a GameObject is destroyed, any variable referencing it becomes null.
        // We check every frame if our target has become null.
        if (targetObject == null)
        {
            // If the target is null, it was either destroyed or never assigned.
            // In either case, we destroy this GameObject.
            Destroy(this.gameObject);
        }
    }
}