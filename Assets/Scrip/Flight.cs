using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Threading;

public class Flight : MonoBehaviour {
    // Variables de clase
    public float speed = 50f;
    public float rotationSpeed = 100f;
    public Transform cameraTransform;
    public Vector2 movementInput;

    // Control de iteraciones
    public int turbulenceIterations = 1000000;

    // true = la turbulencia se calcula en un hilo secundario
    // false = se calcula en el hilo principal (bloquea el juego, útil para comparar)
    public bool useSecondaryThread = true;

    // Lista de fuerzas de turbulencia calculadas
    private List<Vector3> turbulenceForces = new List<Vector3>();
    private readonly object forcesLock = new object();

    // Variables para manipular el hilo secundario
    private Thread turbulenceThread;
    private volatile bool stopTurbulenceThread = false;

    // Tiempo capturado en el hilo principal (Time.time no se puede usar en otro hilo)
    private volatile float captureTime;

    // Método para leer la entrada del teclado (PlayerInput -> Send Messages)
    public void OnMovement(InputValue value) {
        movementInput = value.Get<Vector2>();
    }

    void Update() {
        if (cameraTransform == null) {
            Debug.LogError("No hay cámara asignada");
            return;
        }

        // Tiempo transcurrido (se lee aquí, en el hilo principal)
        captureTime = Time.time;

        // Proceso de consumo de recursos
        if (useSecondaryThread) {
            if (turbulenceThread == null || !turbulenceThread.IsAlive) {
                StartTurbulenceThread();
            }
        } else {
            if (turbulenceThread != null) {
                StopTurbulenceThread();
            }
            SimulateTurbulence();
        }

        // Mover la nave linealmente
        Vector3 moverDireccion = cameraTransform.forward * movementInput.y * speed * Time.deltaTime;
        transform.position += moverDireccion;

        // Mover la nave en rotación
        float yaw = movementInput.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, yaw, 0);
    }

    // Método para simular turbulencias
    public void SimulateTurbulence() {
        float t = captureTime;
        List<Vector3> result = new List<Vector3>(turbulenceIterations);

        for (int i = 0; i < turbulenceIterations; i++) {
            if (stopTurbulenceThread) return;

            Vector3 force = new Vector3(
                Mathf.PerlinNoise(i * 0.001f, t) * 2 - 1,
                Mathf.PerlinNoise(i * 0.002f, t) * 2 - 1,
                Mathf.PerlinNoise(i * 0.003f, t) * 2 - 1
            );
            result.Add(force);
        }

        // Se reemplaza la lista completa de forma segura
        lock (forcesLock) {
            turbulenceForces = result;
        }
    }

    // Bucle del hilo secundario
    private void TurbulenceLoop() {
        while (!stopTurbulenceThread) {
            SimulateTurbulence();
        }
    }

    private void StartTurbulenceThread() {
        stopTurbulenceThread = false;
        turbulenceThread = new Thread(TurbulenceLoop);
        turbulenceThread.IsBackground = true;
        turbulenceThread.Start();
    }

    private void StopTurbulenceThread() {
        stopTurbulenceThread = true;
        if (turbulenceThread != null && turbulenceThread.IsAlive) {
            turbulenceThread.Join();
        }
        turbulenceThread = null;
        stopTurbulenceThread = false;
    }

    void OnDisable() {
        StopTurbulenceThread();
    }
}