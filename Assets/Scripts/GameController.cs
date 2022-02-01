using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TMP_Text creditsText, rationsText, ammoText, oresText;

    private int credits, rations, ammo;
    private float ores;
    [HideInInspector]
    public GameObject homePlanet;
    private GameObject starshipObject;
    private GameObject playerUI, NPCUI;
    private List<GameObject> planets;
    private bool isOpen;

    private void Start()
    {
        credits = 100;
        rations = 200;
        ammo = 300;
        ores = 400;

        starshipObject = Resources.Load<GameObject>("Starship");

        playerUI = GameObject.FindGameObjectWithTag("Player");
        playerUI.SetActive(false);
        NPCUI = GameObject.FindGameObjectWithTag("NPC");
        NPCUI.SetActive(false);

        isOpen = true;
    }

    private void FixedUpdate()
    {
        if (isOpen && Random.Range(0, 200) <= 1)
        {
            createStarship();
        }
    }

    public void loadHome()
    {
        planets = GetComponent<SpaceSpawner>().planets;

        float minDistance = Mathf.Infinity;
        GameObject minPlanet = null;

        foreach (GameObject i in planets)
        {
            float tempDistance = Vector3.Distance(i.transform.position, Vector3.zero);

            if (tempDistance < minDistance)
            {
                minPlanet = i;
                minDistance = tempDistance;
            }
        }

        planets.Remove(minPlanet);
        homePlanet = minPlanet;

        GameObject homeMarker = Instantiate(Resources.Load<GameObject>("Home"), homePlanet.transform);
    }

    private void createStarship()
    {
        isOpen = false;

        GameObject newShip = Instantiate(starshipObject);
        newShip.transform.position = planets[Random.Range(0, planets.Count)].transform.position + (Vector3.up * 0.2f);
        newShip.transform.LookAt(homePlanet.transform);
    }

    public void startCoversation()
    {
        playerUI.SetActive(true);
        NPCUI.SetActive(true);

        updateInventory();

        Invoke("endConversation", 2);
    }

    private void endConversation()
    {
        isOpen = true;

        playerUI.SetActive(false);
        NPCUI.SetActive(false);
    }

    private void updateInventory()
    {
        creditsText.text = "<b><color=#FFEA80>Credits</color></b> $" + credits;
        rationsText.text = "<b><color=#EA80FF>Rations</color></b> " + rations + " cases";
        ammoText.text = "<b><color=#FFAA80>Ammo</color></b> " + ammo + " rounds";
        oresText.text = "<b><color=#80FFD5>Ores</color></b> " + ores + "kg";
    }
}
