using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Linq.Expressions;


public class FlightThreadNosinc : MonoBehaviour {
    // Variable de clase
    public float speed = 50f;
public float rotationSpeed = 100f;
public Transform cameraTransform;
public Vector2 movementInput;

//Control de iteraciones
public int turbulenceIterations = 1000000;

//Lista de vectores de posición calculados
private List<Vector3> turbulenceForces = new List<Vector3>();

//Variables para manipular el hilo secundario
private Thread turbulenceThread;
private bool isTurbulenceRunning = false;
private bool stopTurbulencethread = false;
private float captureTime;

    //Banderas de control sobre lectura
    public bool read = false;
    string filepath;

//Metodo para leer la entradad del teclado
public void OnMovement(InputValue value) {
    {
        movementInput = value.Get<Vector2>();
    }

    void Start() {
            filepath = Application.dataPath + "/TurbulenceData.txt";
            Debug.Log("Ruta al archivo: " + filepath);
    }

    // Update is called once per frame
    void Update() {
        if (cameraTransform == null) {
            Debug.LogError("No hay camara asignada");
            return;
        }

    }

    //tiempo transcurrido
    captureTime = Time.time;

    //Proceso de consumo de recursos
    if (!isTurbulenceRunning) {
        isTurbulenceRunning = true;
        stopTurbulencethread = false;

        turbulenceThread = new Thread(() => SimulateTurbulence(captureTime));
        turbulenceThread.Start();
    }

    //Mover la consumo de recursos
    Vector3 moveDirection = cameraTransform.forward * movementInput.y * speed * Time.deltaTime;

    this.transform.position += moveDirection;

    //Mover la nave en rotacion
    float yaw = movementInput.x * rotationSpeed * Time.deltaTime;
    transform.Rotate(0, yaw, 0);

        //mETODO PARA LECTURA DEL ARC
        TryReadFile();

}

//Metodo para simular turbulencias

public void SimulateTurbulence(float time) {
    turbulenceForces.Clear();

    //Repeticiones
    for (int i = 0; i < turbulenceIterations; i++) {
        //Verificar si se debe detener el hilo
        if (stopTurbulencethread) {
            break;
        }
        Vector3 force = new Vector3
        (
            Mathf.PerlinNoise(i * 0.001f, time) * 2 - 1,
            Mathf.PerlinNoise(i * 0.002f, time) * 2 - 1,
            Mathf.PerlinNoise(i * 0.003f, time) * 2 - 1
            );
        turbulenceForces.Add(force);
    }
    //Señal en consola de inicio del hilo
    Debug.Log("Iniciando simulacioens de turbulencia");

    //escritura del archivo 

        using (StreamWriter writer = new StreamWriter(filepath, false))
            {
            foreach(var force in turbulenceForces) 
                { 
                writer .WriteLine(force.ToString());
                }
            writer.Flush();
        }

        Debug.Log("Archivo escrito");



    //Simulacion completada
    isTurbulenceRunning = false;

}


    public void TryReadFile() 
        {
        try 
            {
            string content = File.ReadAllLines(filepath);
            Debug.Log("Archivo leido:"+ content);
            }
        catch (IOException ex) 
        {
            Debug.LogError("Error en acceso al archivo: " + ex.Message);
            }
    }



private void OnDestroy() {
    //Indicar el cierra del hilo secundario
    stopTurbulencethread = true;

    //Verificar si el hilo existe y se esta ejecutando
    if (turbulenceThread != null && turbulenceThread.IsAlive) {
        //Unir al hilo principal y centar ejecucion
        turbulenceThread.Join();
    }
}
}

