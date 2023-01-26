using UnityEngine;

public abstract class CustomUpdateBehaviour : MonoBehaviour
{
    public int CustomUpdateIndex
    {
        get { return _customUpdateIndex; }
        set
        {
            if (value < -1 || value >= CustomUpdateManager.size)
                _customUpdateIndex = Random.Range(0, CustomUpdateManager.size);
            else
                _customUpdateIndex = value;
        }
    }

    // Do not modify this variable;
    int _customUpdateIndex = -1;

    void OnEnable()
    {
        
    }

    public abstract void CustomUpdate();
}
