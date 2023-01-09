using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentaCamera : MonoBehaviour
{
    Vector3 offSet;
    public GameObject jogador;

    // Start is called before the first frame update
    void Start()
    {
        offSet = transform.position - jogador.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = jogador.transform.position + offSet;
    }
}
