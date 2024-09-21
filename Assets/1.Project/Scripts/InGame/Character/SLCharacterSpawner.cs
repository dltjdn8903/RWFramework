using System;
using System.Collections.Generic;
using Unity.Tutorials.Core.Editor;
using UnityEngine;

public enum controllerType
{
    None,
    Player,
    Enemy,
    NPC,

}

[Serializable]
public class CharacterSpawnData
{
    public controllerType controllerType;
    public string battleForm;
    public string view;
}

public class SLCharacterSpawner : MonoBehaviour
{
    public SphereCollider spawnCollider = null;

    public float spawnRadius = 2f;

    public List<CharacterSpawnData> spawnList = new List<CharacterSpawnData>();
    public List<GameObject> spawnedList = new List<GameObject>();

    private const string viewPath = "Prefab/Character/View";

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            spawnedList.Add(child.gameObject);
        }
    }


#if UNITY_EDITOR
    void OnValidate()
    {
        var tempArray = new GameObject[transform.childCount];

        for (int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = transform.GetChild(i).gameObject;
        }

        foreach (var child in tempArray)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (Application.isPlaying == true)
                    return;

                child.SetActive(false);
                SmartDestroy(child);
            };
        }

        int sequence = 0;

        foreach (var spawnData in spawnList)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (Application.isPlaying == true)
                    return;

                if (spawnData.view.IsNullOrEmpty() == false)
                {
                    switch (spawnData.controllerType)
                    {
                        case controllerType.Player:
                            {

                            }
                            break;
                        case controllerType.Enemy:
                            {
                                var resource = Resources.Load<GameObject>($"Prefab/Character/CharacterPresenterAI");
                                var instance = Instantiate(resource, transform);
                                instance.transform.localPosition = Vector3.zero;
                                instance.transform.localScale = Vector3.one;
                                SpawnObjectsAtPolygonVertices(instance.gameObject, spawnList.Count, sequence);

                                var ai = instance.GetComponent<SFCharacterAI>();
                                ai.InitCharacter(spawnData.view);
                            }
                            break;
                        case controllerType.NPC:
                            {

                            }
                            break;
                        default:
                            break;
                    }

                    //var resource = Resources.Load<GameObject>($"{viewPath}/{spawnData.view}");
                    //var instance = Instantiate(resource, transform);
                    //instance.transform.localPosition = Vector3.zero;
                    //SpawnObjectsAtPolygonVertices(instance, spawnList.Count, sequence);
                    //instance.transform.localScale = Vector3.one;
                    //spawnedList.Add(instance);
                }

                sequence++;
            };
        }
    }
#endif

    public static void SmartDestroy(UnityEngine.Object obj)
    {
        if (obj == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject.DestroyImmediate(obj);
            obj = null;
        }
        else
#endif
        {
            GameObject.Destroy(obj);
            obj = null;
        }
    }

    void SpawnObjectsAtPolygonVertices(GameObject character, int count, int sequence)
    {
        if (count == 1)
        {
            character.transform.localPosition = Vector3.zero;
            return;
        }

        float angleStep = 360f / count;
        float xPos = Mathf.Cos((angleStep * sequence) * Mathf.Deg2Rad) * spawnRadius;
        float zPos = Mathf.Sin((angleStep * sequence) * Mathf.Deg2Rad) * spawnRadius;
        Vector3 spawnPos = new Vector3(xPos, 0, zPos);
        character.transform.localPosition = spawnPos;
    }
}
