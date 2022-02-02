using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TextAsset conversationsFile;
    [Header("UI")]
    public TMP_Text creditsText;
    public TMP_Text rationsText;
    public TMP_Text ammoText;
    public TMP_Text oresText;
    public TMP_Text playerNameText;
    public GameObject playerDialogue;
    public TMP_Text NPCNameText;
    public TMP_Text NPCDialogueText;

    private struct convo
    {
        public string NPCDialogue;
        public List<string> responses;
        public int[,] inventoryChanges;
    }
    private List<convo> conversations;
    private int conversationNum;

    private int credits, rations, ammo;
    private float ores;
    [HideInInspector]
    public GameObject homePlanet;
    private GameObject starshipObject;
    private GameObject playerUI, NPCUI;
    private List<GameObject> planets;
    private bool isOpen;

    private string playerName;
    private string[] NPCNamePrefixes = {
        "Gor",
        "Blez",
        "Zyhaph",
        "Bob",
        "Eg",
        "Will",
        "Suz",
        "Lill",
        "Jos",
        "Dave",
        "Orm",
        "Zeggl",
        "Son",
        "Yog"
    };
    private string[] NPCNameSuffixes = {
        "go",
        "omat",
        "omel",
        "bert",
        "iam",
        "anna",
        "ith",
        "iah",
        "onath",
        "etor",
        "ia",
        "othor"
    };

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

        conversations = new List<convo>();
        string[] split = conversationsFile.text.Split('\n');

        for (int i = 0; i < split.Length; i++)
        {
            if (split[i].Trim().Equals("---"))
            {
                convo newConvo;
                newConvo.NPCDialogue = split[i + 1];
                newConvo.responses = new List<string>();

                int j = i + 2;
                while (!split[j].Trim().Equals("===")) {
                    newConvo.responses.Add(split[j]);
                    j++;
                }

                j++;

                newConvo.inventoryChanges = new int[3, 4];

                for (int k = 0; k < 3; k++)
                {
                    string[] tempSplit = split[j + k].Split(' ');

                    for (int l = 0; l < 4; l++)
                    {
                        newConvo.inventoryChanges[k, l] = int.Parse(tempSplit[l]);
                    }
                }

                conversations.Add(newConvo);
            }
        }

        isOpen = true;

        playerName = "Player Name";
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
        playerNameText.text = playerName;
        NPCUI.SetActive(true);
        NPCNameText.text = NPCNamePrefixes[Random.Range(0, NPCNamePrefixes.Length)] + NPCNameSuffixes[Random.Range(0, NPCNameSuffixes.Length)];

        NPCDialogueText.text = conversations[conversationNum].NPCDialogue;
        GameObject choiceObject = Resources.Load<GameObject>("Choice");

        for (int i = 0; i < conversations[conversationNum].responses.Count; i++)
        {
            GameObject tempChoice = Instantiate(choiceObject, playerDialogue.transform);
            tempChoice.GetComponentInChildren<TMP_Text>().text = conversations[conversationNum].responses[i];
        }

        credits += conversations[conversationNum].inventoryChanges[0, 0];
        rations += conversations[conversationNum].inventoryChanges[0, 1];
        ammo += conversations[conversationNum].inventoryChanges[0, 2];
        ores += conversations[conversationNum].inventoryChanges[0, 3];

        updateInventory();

        Invoke("endConversation", 1);
    }

    private void endConversation()
    {
        isOpen = true;

        playerUI.SetActive(false);
        NPCUI.SetActive(false);

        NPCDialogueText.text = "NPC Dialogue";

        foreach (Transform i in playerDialogue.transform)
        {
            Destroy(i.gameObject);
        }
    }

    private void updateInventory()
    {
        creditsText.text = colourString("Credits ", StringColours.Yellow) + "$" + credits;
        rationsText.text = colourString("Rations ", StringColours.Pink) + rations + " cases";
        ammoText.text = colourString("Ammo ", StringColours.Orange) + ammo + " rounds";
        oresText.text = colourString("Ores ", StringColours.Green) + ores + "kg";
    }

    private enum StringColours { Yellow, Pink, Orange, Green };
    private string colourString(string message, StringColours colour)
    {
        string code = "";

        switch (colour)
        {
            case StringColours.Yellow:
                code = "FFEA80";
                break;
            case StringColours.Pink:
                code = "EA80FF";
                break;
            case StringColours.Orange:
                code = "FFAA80";
                break;
            case StringColours.Green:
                code = "80FFD5";
                break;
        }

        return "<b><color=#" + code + ">" + message + "</color></b>";
    }
}
