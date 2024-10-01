using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField]
    float speed = 5.0f; // 이동 속도

    void Update()
    {
        // x축 방향으로 일정하게 이동
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
    }
}
