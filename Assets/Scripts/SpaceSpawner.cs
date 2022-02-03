using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SpaceSpawner : MonoBehaviour
{
    public Vector2 spaceBounds;
    public Material[] starColours;
    public Material[] planetColours;

    [Header("Small Stars")]
    public int smallStarCount = 800;
    public float smallStarSize = 0.03f;
    [Header("Medium Stars")]
    public int midStarCount = 70;
    public float midStarSize = 0.1f;
    [Header("Big Stars")]
    public int bigStarCount = 7;
    public float bigStarSize = 0.3f;
    [Header("Planets")]
    public int planetCount = 10;

    private GameObject starObject, planetObject;
    [HideInInspector]
    public List<GameObject> stars, planets;

    private void Start()
    {
        starObject = Resources.Load<GameObject>("Star");
        planetObject = Resources.Load<GameObject>("Planet");

        createStarBatch(smallStarCount, smallStarSize);
        createStarBatch(midStarCount, midStarSize);
        createStarBatch(bigStarCount, bigStarSize);

        createPlanetBatch(planetCount);
    }

    private void createStarBatch(int count, float size)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 tempPosition = new Vector3(Random.Range(-spaceBounds.x, spaceBounds.x), 0, Random.Range(-spaceBounds.y, spaceBounds.y));

            int tempIndex = 2;

            if (count == smallStarCount)
            {
                tempIndex = 2;
            }
            else
            {
                if (count == midStarCount)
                {
                    tempIndex = Random.Range(0, 2);
                }
                else if (count == bigStarCount)
                {
                    tempIndex = Random.Range(3, 5);
                }
            }

            createStar(tempPosition, size, tempIndex);
        }
    }

    private void createStar(Vector3 starPosition, float starSize, int colourIndex)
    {
        GameObject newStar = Instantiate(starObject, transform);
        newStar.transform.localPosition = starPosition;
        newStar.transform.localScale = Vector3.one * starSize;

        if (starSize == bigStarSize)
        {
            Light tempLight = newStar.GetComponent<Light>();
            tempLight.enabled = true;
            tempLight.intensity = 2 * starSize;
        }

        newStar.GetComponent<MeshRenderer>().material = starColours[colourIndex];

        stars.Add(newStar);
    }

    private void createPlanetBatch(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 tempPosition = new Vector3(Random.Range(-spaceBounds.x, spaceBounds.x), 0, Random.Range(-spaceBounds.y, spaceBounds.y));

            GameObject newPlanet = Instantiate(planetObject, transform);
            newPlanet.transform.localPosition = tempPosition;

            newPlanet.GetComponent<MeshRenderer>().material = planetColours[Random.Range(0, planetColours.Length)];

            planets.Add(newPlanet);
        }

        if (GetComponent<GameController>() != null)
        {
            GetComponent<GameController>().loadHome();
        }
    }
}