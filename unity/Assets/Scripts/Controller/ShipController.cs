using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField]
    float speed = 5.0f; // �̵� �ӵ�

    void Update()
    {
        // x�� �������� �����ϰ� �̵�
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
    }
}
