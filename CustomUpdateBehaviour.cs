using UnityEngine;

public abstract class CustomUpdateBehaviour : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        CustomUpdateManager.Instance.Register(this);
    }
    protected virtual void OnDisable()
    {
        CustomUpdateManager.Instance.Deregister(this);
    }

    public abstract void CustomUpdate();
}
