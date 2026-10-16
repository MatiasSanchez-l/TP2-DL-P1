/*
 * Created on Tue Feb 18 2025
 *
 * Script Name: HandController.cs
 * Description: Controlador para arrastrar objeto(mano) con el mouse y detectar agitadas para llenar barra.
 * Cambiar el valor de "wavesToFill" para ajustar la cantidad de agitadas necesarias para llenar la barra.
 * Cambiar el valor de "minSwing" para ajustar la distancia mínima que debe moverse la mano para contar como una agitada.
 * Cambiar el valor de "deadZone" para ajustar la sensibilidad del movimiento de la mano.
 * En UpdateBar() se puede agregar la lógica para lo que sucede cuando la barra se llena(se gano el minijuego).
 */
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : Minijuego
{
    [SerializeField] private Camera cam;

    [Header("Deteccion de Agitadas")]
    [SerializeField] private float minSwing = 1.5f;
    [SerializeField] private float deadZone = 0.02f;

    [Header("Barra de Progreso")]
    [SerializeField] private Transform barFill;
    [SerializeField] private int wavesToFill = 10;

    private bool dragging;
    private Vector3 offset;


    private float lastX;
    private float travel;
    private int lastDir;
    private int waves;

    private Vector3 fullScale;
    private bool completed;

    private void Awake()
    {
        if(cam == null) cam = BuscarCamaraDeLaEscena();

        fullScale = barFill.localScale;
        UpdateBar();
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        
        if(mouse == null) return;

        Vector3 mouseWorld = GetMouseWorld(mouse);

        if(mouse.leftButton.wasPressedThisFrame)
        {
            Collider2D hit = Physics2D.OverlapPoint(mouseWorld);

            if(hit != null && hit.gameObject == gameObject)
            {
                dragging = true;
                offset = transform.position - mouseWorld;

                lastX = transform.position.x;
                travel = 0f;
                lastDir = 0;
            }
        }

        if(dragging)
        {
            transform.position = mouseWorld + offset;
            DetectWaves();
        }

        if(mouse.leftButton.wasReleasedThisFrame)
        {
            dragging = false;
        }
    }

    private void DetectWaves()
    {
        if(completed) return;

        float deltaX = transform.position.x - lastX;
        lastX = transform.position.x;

        if(Mathf.Abs(deltaX) < deadZone) return;

        int dir = deltaX > 0 ? 1 : -1;

        if(dir != lastDir)
        {
            if(Mathf.Abs(travel) >= minSwing)
            {
                waves++;
                Debug.Log($"Agitadas: {waves}");
                UpdateBar();
            }

            travel = 0f;
            lastDir = dir;
        }

        travel += deltaX;
    }

    private void UpdateBar()
    {
        float progress = Mathf.Clamp01((float)waves / wavesToFill);

        Vector3 scale = fullScale;
        scale.x = fullScale.x * progress;
        barFill.localScale = scale;

        if(progress >= 1f && !completed)
        {
            completed = true;
            Debug.Log("Barra Llena");
            Invoke(nameof(Ganar), 0.5f);
        }
    }

    private Camera BuscarCamaraDeLaEscena()
    {
        foreach(GameObject raiz in gameObject.scene.GetRootGameObjects())
        {
            Camera c = raiz.GetComponentInChildren<Camera>();
            if(c != null) return c;
        }
        return Camera.main;
    }

    private Vector3 GetMouseWorld(Mouse mouse)
    {
        Vector3 screenPos = mouse.position.ReadValue();
        screenPos.z = -cam.transform.position.z;

        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        return worldPos;
    }
}
