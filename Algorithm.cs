using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Algorithm
{
    public class Navigation
    {
        public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance)
        {
            // center를 중심으로 반지름이 maxDistance인 구 안에서의 랜덤한 위치 하나를 저장
            // Random.insideUnitSphere는 반지름이 1인 구 안에서의 랜덤한 한 점을 반환하는 프로퍼티
            Vector3 randomPos = center + Random.insideUnitSphere * distance;

            // distance 반경 안에서, randomPos에 가장 가까운 내비메시 위의 한 점을 찾음
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, distance, NavMesh.AllAreas))
                // 찾은 점 반환
                return hit.position;

            throw new System.Exception("There is no Navigation!");
        }
    }
}
