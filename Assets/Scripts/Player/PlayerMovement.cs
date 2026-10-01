using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb; // Los métodos, clases y funciones en PascalCase || Las variables (rb, movementDirection), nunca empiezan con mayúscula. camelCase
    [SerializeField] Vector3 movementDirection; // SeralizeField hace que la variable siga siendo privada (no pueden editar el valor otros scripts) pero aparece en el editor
    [Header("Movement Properties")]
    [SerializeField] private float horizontalMovement;
    [SerializeField] private float forwardMovement;
    [SerializeField] private float movementVelocity = 15;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Inicializa la variable
        movementDirection = new Vector3(horizontalMovement, 0, forwardMovement);
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalMovement = Input.GetAxis("Horizontal");
        forwardMovement = Input.GetAxis("Vertical");
        movementDirection = new Vector3(horizontalMovement, 0, forwardMovement);
    }

    private void Awake() // Sirve, por ejemplo, para instanciar GO (Gameobjects) cuando inicia el juego
    {
        
    }

    private void FixedUpdate() // Es un update pero se ejecuta SI O SI 100 veces por segundo, ya que es la frecuencia con la que se actualiza el sistema de físicas.
    {
        // Time.deltaTime; // Realmente cumple la misma función que el fixed update ya que multiplica el valor de algo x el intervalo entre frames.
        rb.AddForce(movementDirection*movementVelocity, ForceMode.Acceleration);

        /*
        ForceMode.Force; Añade una fuerza -- Le da un empujón
        ForceMode.Impulse; Da un solo impulso. Acelera y frena -- Si me empujan en el hielo
        ForceMode.VelocityChange; Cambia la velocidad del rb -- Me establece una velocidad concreta
        ForceMode.Acceleration; Aceleración constante -- Un coche
        */
    }

}
