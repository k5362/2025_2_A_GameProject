using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainShoot : MonoBehaviour
{
    [SerializeField] float refreshRate = 0.1f;
    [SerializeField][Range(1, 10)] int maximumEnemiesInChain = 3;
    [SerializeField] float delayBetweenEachChain = 0.5f;
    [SerializeField] Transform playerFirePoint;
    [SerializeField] EmenyDetecter playerEmenyDetector;
    [SerializeField] GameObject IinRendererPrefab;

    bool shooting;
    bool shot;
    float counter = 1;
    GameObject currentClosestEnemy;

    List<GameObject> activeEffects = new List<GameObject>();
    List<GameObject> spawnedLineRanderers = new List<GameObject>();
    List<GameObject> enemiesInChain = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if(playerEmenyDetector.GetEnemiesInRange().Count > 0)
            {
                 if (!shooting)
                {
                    StartShooting();
                }
            }
            else
            {
                if (shooting)
                {
                    StopShooting();
                }
            }

            if(Input.GetButtonUp("Fire1"))
            {
                if (shooting)
                {
                    StopShooting();
                }
            }
        }
    }

    IEnumerator ChainReaction(GameObject closestEnemy)
    {
        yield return new WaitForSeconds(delayBetweenEachChain);

        if (counter == maximumEnemiesInChain)
        {
            yield return null;
        }
        else
        {
            if (shooting)
            {
                counter++;
                enemiesInChain.Add(closestEnemy);
                if (!enemiesInChain.Contains(closestEnemy.GetComponent<EmenyDetecter>().GetClosestEnemy()))
                {
                    NewLineRanderer(closestEnemy.transform, closestEnemy.GetComponent<EmenyDetecter>().GetClosestEnemy().transform);
                    StartCoroutine(ChainReaction(closestEnemy.GetComponent<EmenyDetecter>().GetClosestEnemy()));
                }
            }
        }
    }

    IEnumerator UpdateLineRenderers(GameObject IineR, Transform startPos, Transform endPos, bool getClosestEnmeyToPlayer = false)
    {
        if(shooting && shot && IineR != null)
        {
            IineR.GetComponent<LineRandererController>().SetPosition(startPos, endPos);
            yield return new WaitForSeconds(refreshRate);

            if (getClosestEnmeyToPlayer)
            {
                StartCoroutine(UpdateLineRenderers(IineR, startPos, playerEmenyDetector.GetClosestEnemy().transform, true));
                if (currentClosestEnemy != playerEmenyDetector.GetClosestEnemy())
                {
                    StopShooting();
                    StartShooting();
                }
            }
            else
            {
                StartCoroutine(UpdateLineRenderers(IineR, startPos, endPos));
            }
        }
    }

    void NewLineRanderer(Transform startPos, Transform endPos, bool getClosestEnmeyToPlayer = false)
    {
        GameObject IineR = Instantiate(IinRendererPrefab);
        spawnedLineRanderers.Add(IineR);
        StartCoroutine(UpdateLineRenderers(IineR, startPos, endPos, getClosestEnmeyToPlayer));
    }

    void StartShooting()
    {
        shooting = true;

        if (playerEmenyDetector != null && playerFirePoint != null && IinRendererPrefab != null)
        {
            if (!shot)
            {
                shot = true;
                currentClosestEnemy = playerEmenyDetector.GetClosestEnemy();
                NewLineRanderer(playerFirePoint, playerEmenyDetector.GetClosestEnemy().transform, true);

                if (maximumEnemiesInChain > 1)
                {
                    StartCoroutine(ChainReaction(playerEmenyDetector.GetClosestEnemy()));
                }
            }

        }

    }

    void StopShooting()
    {
        shooting = false;
        shot = false;
        
        for (int i = 0; i < spawnedLineRanderers.Count; i++)
        {
            Destroy(spawnedLineRanderers[i]);
        }

        spawnedLineRanderers.Clear();
        enemiesInChain.Clear();

        for (int i = 0; i < activeEffects.Count; i++)
        {
            Destroy(activeEffects[i]);
        }

        activeEffects.Clear();
    }

}