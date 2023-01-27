public interface ICustomUpdate
{
    // 상속하는 컴포넌트의 OnEnable()에서 호출.
    // void Register() { CustomUpdateManager.Instance.Register(this); }
    void RegisterUpdate();

    // 상속하는 컴포넌트의 OnDisable()에서 호출.
    // void Register() { CustomUpdateManager.Instance.Deregister(this); }
    void DeregisterUpdate();

    void CustomUpdate();
}