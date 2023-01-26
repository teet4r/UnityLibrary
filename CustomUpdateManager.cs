using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomUpdateManager : Singleton<CustomUpdateManager>
{
    int _size = 10;
    int _sizeThreshold; // size의 1% 기준

    // for으로 실행할 CustomUpdateBehaviour 함수 배열
    CustomUpdateBehaviour[] _customUpdateObjs;

    // CustomUpdateBehaviour에게 부여된 인덱스 저장
    Dictionary<CustomUpdateBehaviour, int> _customUpdateObjIndexes = new Dictionary<CustomUpdateBehaviour, int>();

    // 사용 가능한 번호표 큐
    // 사용할 번호표 발급 및 사용한 번호표 회수할 큐
    Queue<int> _indexQ = new Queue<int>();

    // 뽑은 번호표는 해당 풀에서 삭제, 반환된 번호표는 해당 풀에 저장
    HashSet<int> _indexPool = new HashSet<int>();

    protected override void Awake()
    {
        base.Awake();

        _Initialize();
    }

    void _Initialize()
    {
        _sizeThreshold = (int)(_size * 0.01f);
        _customUpdateObjs = new CustomUpdateBehaviour[_size];

        for (int i = 0; i < _size; i++)
        {
            _indexQ.Enqueue(i);
            _indexPool.Add(i);
        }
    }

    /// <summary>
    /// Register to CustomUpdate pool.
    /// </summary>
    /// <param name="obj"></param>
    public void Register(CustomUpdateBehaviour obj)
    {
        if (_indexQ.Count <= _sizeThreshold)
            _Resize();

        // 이미 등록돼 있다면 종료
        if (_customUpdateObjIndexes.TryGetValue(obj, out int index))
            return;
        // 없다면 새로 등록
        else
        {
            index = _indexQ.Dequeue();
            _indexPool.Remove(index);
            _customUpdateObjIndexes.Add(obj, index);
            _customUpdateObjs[index] = obj;
        }

        Debug.Log($"남은 인덱스: {_indexQ.Count}");
    }

    /// <summary>
    /// Deregister to CustomUpdate pool.
    /// </summary>
    /// <param name="obj"></param>
    public void Deregister(CustomUpdateBehaviour obj)
    {
        // 풀에 존재한다면 삭제
        if (_customUpdateObjIndexes.TryGetValue(obj, out int index))
        {
            _customUpdateObjs[index] = null;
            _indexPool.Add(index);
            _indexQ.Enqueue(index);
        }
        Debug.Log($"남은 인덱스: {_indexQ.Count}");
    }

    void _Resize()
    {
        int newSize = _size * 2;
        CustomUpdateBehaviour[] _newCustomUpdateObjs = new CustomUpdateBehaviour[newSize];
        for (int i = 0; i < _size; i++)
            _newCustomUpdateObjs[i] = _customUpdateObjs[i];
        for (int i = _size; i < newSize; i++)
        {
            _indexQ.Enqueue(i);
            _indexPool.Add(i);
        }

        _size = newSize;
        _sizeThreshold = (int)(_size * 0.01f);
        _customUpdateObjs = _newCustomUpdateObjs;
    }

    void Update()
    {
        for (int i = 0; i < _size; i++)
            if (_customUpdateObjs[i] != null)
                _customUpdateObjs[i].CustomUpdate();
    }
}
