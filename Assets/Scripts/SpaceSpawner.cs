using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SpaceSpawner : MonoBehaviour
{
    public Vector2 spaceBounds;
    public Material[] starColours;
    public Material[] planetColours;
    public float orbitFactor = 1;

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
    [HideInInspector] public List<GameObject> smallStars, midStars, bigStars, planets;

    private void Start()
    {
        smallStars = new List<GameObject>();
        midStars = new List<GameObject>();
        bigStars = new List<GameObject>();

        starObject = Resources.Load<GameObject>("Star");
        planetObject = Resources.Load<GameObject>("Planet");

        createStarBatch(bigStarCount, bigStarSize);
        createStarBatch(midStarCount, midStarSize);
        createStarBatch(smallStarCount, smallStarSize);

        createPlanetBatch(planetCount);
    }

    private void createStarBatch(int count, float size)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 tempPosition = new Vector3(Random.Range(-spaceBounds.x, spaceBounds.x), 0, Random.Range(-spaceBounds.y, spaceBounds.y));

            int tempIndex = 2;

            string tempTag = "";

            List<GameObject> tempList = null;

            if (count == smallStarCount)
            {
                tempIndex = 2;
                tempTag = "Small";
                tempList = smallStars;
            }
            else
            {
                if (count == midStarCount)
                {
                    tempIndex = Random.Range(0, 2);
                    tempTag = "Mid";
                    tempList = midStars;
                }
                else if (count == bigStarCount)
                {
                    tempIndex = Random.Range(3, 5);
                    tempTag = "Big";
                    tempList = bigStars;
                }
            }

            createStar(tempPosition, size, tempIndex, tempTag, tempList);
        }
    }

    private void createStar(Vector3 starPosition, float starSize, int colourIndex, string starTag, List<GameObject> starList)
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

        if (!tag.Equals(""))
        {
            newStar.tag = starTag;
        }

        List<GameObject> tempList = null;
        float tempSpeedMin = 0.0001f;
        float tempSpeedMax = 0.005f;

        if (starList != null)
        {
            starList.Add(newStar);

            if (!starList.Equals(bigStars))
            {
                tempList = bigStars;

                TrailRenderer trail = newStar.GetComponent<TrailRenderer>();
                trail.material = starColours[colourIndex];

                if (starList.Equals(smallStars))
                {
                    trail.startWidth = 0.01f;
                    trail.endWidth = 0;
                    tempSpeedMin = 0.0001f;
                    tempSpeedMax = 0.005f;
                }
                else
                {
                    trail.startWidth = 0.05f;
                    trail.endWidth = 0;
                    tempSpeedMin = 0.01f;
                    tempSpeedMax = 0.02f;
                }
            }
        }

        if (tempList != null)
        {
            Transform closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject i in tempList)
            {
                float tempDistance = Vector3.Distance(newStar.transform.position, i.transform.position);

                if (tempDistance < closestDistance)
                {
                    closest = i.transform;
                    closestDistance = tempDistance;
                }
            }

            if (closest != null)
            {
                Orbit tempOrbit = newStar.GetComponent<Orbit>();
                tempOrbit.parent = closest;
                tempOrbit.speed = Random.Range(tempSpeedMin, tempSpeedMax);

                if (Random.Range(0, 2) == 0)
                {
                    tempOrbit.speed *= -1;
                }
            }
        }
    }

    private void createPlanetBatch(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 tempPosition = new Vector3(Random.Range(-spaceBounds.x, spaceBounds.x), 0, Random.Range(-spaceBounds.y, spaceBounds.y));

            GameObject newPlanet = Instantiate(planetObject, transform);
            newPlanet.transform.localPosition = tempPosition;

            newPlanet.GetComponent<MeshRenderer>().material = planetColours[Random.Range(0, planetColours.Length)];

            Transform closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject j in bigStars)
            {
                float tempDistance = Vector3.Distance(newPlanet.transform.position, j.transform.position);

                if (tempDistance < closestDistance)
                {
                    closest = j.transform;
                    closestDistance = tempDistance;
                }
            }

            if (closest != null)
            {
                Orbit tempOrbit = newPlanet.GetComponent<Orbit>();
                tempOrbit.parent = closest;
                tempOrbit.speed = Random.Range(0.01f, 0.02f);

                if (Random.Range(0, 2) == 0)
                {
                    tempOrbit.speed *= -1;
                }
            }

            planets.Add(newPlanet);
        }

        if (GetComponent<GameController>() != null)
        {
            GetComponent<GameController>().loadHome();
        }
    }
}