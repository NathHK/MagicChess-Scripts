using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public Manager manager;
    public GameObject selector;
    public GameObject gameOver;
    public GameObject parent;
    public Image menu;
    public Button openMenu;
    public Button closeMenu;
    public Button normal;
    public Button hard;
    public Button simulation;

    // Start is called before the first frame update
    void Start()
    {
        openMenu.onClick.AddListener(OpenMenu);
        closeMenu.onClick.AddListener(CloseMenu);
        normal.onClick.AddListener(NormalGame);
        hard.onClick.AddListener(HardGame);
        simulation.onClick.AddListener(SimulatedGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenMenu()
    {
        parent.SetActive(true);
        gameOver.SetActive(false);

    }

    void CloseMenu()
    {
        parent.SetActive(false);
    }

    void NormalGame()
    {
        manager.NewGame("normal");
        parent.SetActive(false);
    }

    void HardGame()
    {
        manager.NewGame("hard");
        parent.SetActive(false);
    }

    void SimulatedGame()
    {
        manager.NewGame("sim");
        parent.SetActive(false);
    }

}
