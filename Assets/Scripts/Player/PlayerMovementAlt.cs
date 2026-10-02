using System;
using UnityEngine;

public class PlayerMovementAlt : MonoBehaviour
{
    [SerializeField] float moveSpeed;
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

        // 4
        Vector3 targetVelocity = input * moveSpeed;

        input = Vector3.ClampMagnitude(input, 1.0f);

        // 6
        chController.Move(targetVelocity * Time.deltaTime);
        throw new NotImplementedException();
    }
}


// 1. Recoger el input
// 2. Obtener las coords o direcciones de la camara
// 3. Calcular la direccion del movimiento
// 4. Calculos de la velocidad
// 5. Gamefeel: Aceleración, frenos, cambios de direccion
// 6. Mover al personaje