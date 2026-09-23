using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class UISelection : MonoBehaviour {
    public static bool gazedAT;
    public float fillTime = 5f;
    public Image radialImage;
    public UnityEvent onFillComplete; //Evento generico cuando termine

    private Coroutine fillCoroutine; //Proceso concurrente
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        gazedAT = false;
        radialImage.fillAmount = 0f;
    }
    public void OnPointerEnter() {
        gazedAT = true;
        if (fillCoroutine != null) {
            StopCoroutine(fillCoroutine); //Detener cualquier corutina anterior
        }
        fillCoroutine = StartCoroutine(FillRadial()); //Iniciar el llenado
    }

    public void OnPointerExit() {
        gazedAT = false;
        if (fillCoroutine != null) {
            StopCoroutine(fillCoroutine); //Detener cualquier corutina anterior
            fillCoroutine = null;
        }
        radialImage.fillAmount = 0f;
    }

    private IEnumerator FillRadial() {
        float elapsedTime = 0f;

        while (elapsedTime < fillTime) {
            if (!gazedAT) //Si deja de ser observado
            {
                yield break; //Salid de la corutina
            }
            elapsedTime += Time.deltaTime;
            radialImage.fillAmount = Mathf.Clamp01(elapsedTime / fillTime);

            yield return null;
        }

        //Ejecuta el evento onFillComplete cuando se completa el llenado
        onFillComplete?.Invoke();
    }
    // Update is called once per frame
    void Update() {

    }
}