using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomUpdateManager : Singleton<CustomUpdateManager>
{
    public const int size = 10000;
    public const int sizeThreshold = (int)(size * 0.01f);

    public const int InitialIndex = -1;
    public const int Error = -2;

    // for으로 실행할 CustomUpdateBehaviour 함수 배열
    CustomUpdateBehaviour[] _customUpdateObjs = new CustomUpdateBehaviour[size];

    // CustomUpdateBehaviour에게 부여된 인덱스 저장
    // CustomUpdateBehaviour가 가지고 있는 CustomUpdateIndex와 이 딕셔너리의 value값이 다르다면 수정해야 함
    Dictionary<CustomUpdateBehaviour, int> _customUpdateObjIndexes = new Dictionary<CustomUpdateBehaviour, int>();

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
        for (int i = 0; i < size; i++)
        {
            _indexQ.Enqueue(i);
            _indexPool.Add(i);
        }
    }

    /// <summary>
    /// Register to CustomUpdate pool.
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="System.Exception"></exception>
    public void Register(CustomUpdateBehaviour obj)
    {
        // 최대 용량의 1% 미만으로 남게되면 예외처리
        if (_indexQ.Count < sizeThreshold)
            throw new System.Exception("Index Queue has no spare indexes. Please resize its size!");

        // 이미 등록돼 있다면
        if (obj.CustomUpdateIndex != -1)
        {
            // 사용 중인 풀의 인덱스와 가지고 있는 인덱스 번호가 일치하면 종료 
            if (_customUpdateObjs[obj.CustomUpdateIndex].Equals(obj)) return;

            // 해당 컴포넌트를 찾음
            var foundIndex = _FindIndex(obj);
            // 해당 컴포넌트가 있으면
            if (foundIndex != -2)
            {
                // 매개변수의 값을 수정
                obj.CustomUpdateIndex = foundIndex;
                return;
            }

            // 해당 컴포넌트가 없다면 밑으로 내려가 등록
        }
        
        var index = _indexQ.Dequeue();
        _indexPool.Remove(index);

        obj.CustomUpdateIndex = index;
        _customUpdateObjs[index] = obj;
    }

    /// <summary>
    /// Deregister to CustomUpdate pool.
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="System.Exception"></exception>
    public void Deregister(CustomUpdateBehaviour obj)
    {
        var index = obj.CustomUpdateIndex;

        if (index < 0 || index >= size)
        {
            var foundIndex = _FindIndex(obj);

            // 풀에 존재한다면
            if (foundIndex != Error)
            {
                _customUpdateObjs[foundIndex] = null;
            }
        }

        if (_indexPool.Contains(index))
            throw new System.Exception($"{index} is already in the pool!");

        obj.CustomUpdateIndex = -1;
        _customUpdateObjs[index] = null;

        _indexPool.Add(index);
        _indexQ.Enqueue(index);
    }

    int _FindIndex(CustomUpdateBehaviour obj)
    {
        for (int i = 0; i < size; i++)
            if (_customUpdateObjs[i].Equals(obj))
                return i;
        return Error;
    }

    void Update()
    {
        for (int i = 0; i < size; i++)
            if (_customUpdateObjs[i] != null)
                _customUpdateObjs[i].CustomUpdate();
    }
}
