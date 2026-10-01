using System;
using UnityEngine;

public class PlayerMovementAlt : MonoBehaviour
{
    float moveSpeed;
    private CharacterController chController;

    void Start()
    {
        chController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
        
    }

    private void Move()
    {
        // 1
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(horizontal, 0, vertical);

        //6
        chController.Move(input * Time.deltaTime);
        throw new NotImplementedException();
    }
}


// 1. Recoger el input
// 2. Obtener las coords o direcciones de l acamara
// 3. Calcular la direccion del movimiento
// 4. Calculos de la velocidad
// 5. Gamefeel: Aceleración, frenos, cambios de direccion
// 6. Mover al personaje