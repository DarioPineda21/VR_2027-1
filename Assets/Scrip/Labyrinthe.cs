using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;

public class Labyrinthe : MonoBehaviour
{
    //Objetos en escena
    public GameObject player;
    public Transform entrance;
    public Transform exit;
    public GameObject WinCanvas;



    // Variables de configuración
    public float detectionRange = 1f;
    public float exitRange = 1f;
    public float minDistanceFromEntrance = 3f;

    //Agentes de IA
    List<NavMeshAgent> agents = new ();
    NavMeshTriangulation triangulation;
    Vector3 entrancePos;

    //Variables para el algoritmo
    float detectionRangeSqr;
    float exitRangeSqr;
    float minDistanceSqr;

    bool gameWon = false;

    //Concurrencia
    System.Random random = new();

    void Start()
    {
        entrancePos = entrance.position;
        triangulation = NavMesh.CalculateTriangulation();

        detectionRangeSqr = detectionRange * detectionRange;
        exitRangeSqr = exitRange * exitRange;
        minDistanceSqr = minDistanceFromEntrance * minDistanceFromEntrance;

        WinCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Regresar Entrada
    void TeleportPlayerToEntrance() 
        { 
            var cc = player.GetComponent<NavMeshAgent>();
            if (cc != null) 
            {
            cc.enabled = false;
            
        
            }
            player.transform.position = entrancePos;
            if (cc != null) 
            {
            cc.enabled = false;


            }
        Debug.Log("Teport a: " + entrancePos);


    }

    //Objetos canvas de ganar

    public void WinGame() 
        { 
            gameWon = true;

        foreach (var agent in agents) 
            {
            agent.isStopped = true;

            
            }
        WinCanvas.SetActive(true);
    }
        
    //Encontrar todos los enemigos

    public void FinAllEnemies() 
        { 
        
        agents.Clear();

        foreach (var agent in FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None)) 
            { 
        
            if (agent.CompareTag("Enemy")) 
                {

                agents.Add(agent);

                }
        }
        
        }


}
