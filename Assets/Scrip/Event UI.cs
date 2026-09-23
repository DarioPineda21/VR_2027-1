using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EventUI : MonoBehaviour
{
    public List<GameObject> objects; //Lista de Objetos
    public List<string> messages; // Lista de mensajes a mostrar
    public int currentIndex = 0;
    public TextMeshProUGUI textMeshPro; //Componente de texto en el objeto

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateVisibility();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Metodo para el ciclo de objetos
    public void CycleObjects() 
        { 
            currentIndex = (currentIndex +1)%objects.Count;

        //Actualizar la visibilidad de los objetos 
        UpdateText();
    }

    private void UpdateVisibility() 
        { 
          for (int i=0; i<objects.Count;i++) 
            { 
             // Solo el objeto en el indice actual es visible
             objects [i].SetActive(i==currentIndex);
            }
        
        }
    //Metodo para ciclo de textos
    public void CycleText() 
        { 
            //Incrementa el indice y vuelve al principio si es necesario
            currentIndex = (currentIndex +1) % messages.Count;
            //Actualizar el texto actual

        }
    private void UpdateText() 
    {
        if (messages.Count > 0 && textMeshPro != null) 
            {
            textMeshPro.text = messages[currentIndex];
        }
     
    }


    //cambiar de escena por Nombre
    public void ChangeSceneByName(string sceneName) {
       SceneManager.LoadScene(sceneName);
    }
    public void ChangeSceneByName(int sceneIndex)   
    {
      SceneManager.LoadScene(sceneIndex);
    }

    public void ReloadCurrentScene() 
        { 
        
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        }

    public void Exit() 
     { 
        Application.Quit();
        Debug.Log("Salio del Juego");

    }

}
