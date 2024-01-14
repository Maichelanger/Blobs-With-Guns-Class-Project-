using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

       

    // Declaración de Variables
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject multiplayerMenuUI;
    [SerializeField] private GameObject newLobbyMenuUI;


    private bool isLocalPlayerReady;
    
   

    private void Awake()
    {
        Instance = this;

        isLocalPlayerReady = false;
    }


    // Establecemos la configuración inicial de los menús del juego y los eventos de inicio de partida
    void Start()
    {
        // Activamos los elementos iniciales
        mainMenuUI.SetActive(true);


        // Desactivamos el resto de menús
        multiplayerMenuUI.SetActive(false);
        newLobbyMenuUI.SetActive(false);

    }


    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public void SetPlayerReady()
    {
        isLocalPlayerReady=true;
    }

   
}
