using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomUnityMessageManager : Singleton<CustomUnityMessageManager>
{
    int _size = 100;
    int _sizeThreshold; // size의 10% 기준

    // for으로 실행할 CustomUpdateBehaviour 함수 배열
    ICustomUpdate[] _customUpdateObjs;

    // CustomUpdateBehaviour에게 부여된 인덱스 저장
    Dictionary<ICustomUpdate, int> _customUpdateObjIndexes = new Dictionary<ICustomUpdate, int>();

    // 사용 가능한 번호표 큐
    // 사용할 번호표 발급 및 사용한 번호표 회수할 큐
    Queue<int> _indexQ = new Queue<int>();

    // 뽑은 번호표는 해당 풀에서 삭제, 반환된 번호표는 해당 풀에 저장
    HashSet<int> _indexPool = new HashSet<int>();

    void Awake()
    {
        _Initialize();
    }

    void Update()
    {
        for (int i = 0; i < _size; i++)
            if (_customUpdateObjs[i] != null)
                _customUpdateObjs[i].CustomUpdate();
    }

    void _Initialize()
    {
        _sizeThreshold = (int)(_size * 0.1f);
        _customUpdateObjs = new ICustomUpdate[_size];

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
    public void Register(ICustomUpdate obj)
    {
        if (_indexQ.Count <= _sizeThreshold)
            _ResizeUpdatePool();

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
    }

    /// <summary>
    /// Deregister to CustomUpdate pool.
    /// </summary>
    /// <param name="obj"></param>
    public void Deregister(ICustomUpdate obj)
    {
        // 풀에 존재한다면 삭제
        if (_customUpdateObjIndexes.TryGetValue(obj, out int index))
        {
            _customUpdateObjs[index] = null;
            _indexPool.Add(index);
            _indexQ.Enqueue(index);
            _customUpdateObjIndexes.Remove(obj);
        }
    }

    void _ResizeUpdatePool()
    {
        int newSize = _size * 2;
        ICustomUpdate[] _newCustomUpdateObjs = new ICustomUpdate[newSize];
        for (int i = 0; i < _size; i++)
            _newCustomUpdateObjs[i] = _customUpdateObjs[i];
        for (int i = _size; i < newSize; i++)
        {
            _indexQ.Enqueue(i);
            _indexPool.Add(i);
        }

        _size = newSize;
        _sizeThreshold = (int)(_size * 0.1f);
        _customUpdateObjs = _newCustomUpdateObjs;
    }
}
