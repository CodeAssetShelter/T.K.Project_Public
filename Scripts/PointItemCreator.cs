using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointItemCreator : MonoBehaviour
{
    public GameObject m_PointItemPrefab;

    [Header("- Options")]
    public Vector2 m_SpawnTime;

    [Header("- SpawnPos")]
    public Vector2Int m_LeftBottom;
    public Vector2Int m_RightTop;

    public List<Vector2> m_SpawnPosList;

    public void Start()
    {
        m_SpawnPosList = new List<Vector2>();
        BuildSpawnablePoints();
    }

    void BuildSpawnablePoints()
    {
        int m_LayerMask =
        (1 << LayerMask.NameToLayer("Character")) |
        (1 << LayerMask.NameToLayer("Map")) |
        (1 << LayerMask.NameToLayer("Tile Booker"));

        Vector2Int curr_pos = m_LeftBottom;

        int cc = 0;
        while (curr_pos.y < m_RightTop.y + 1)
        {
            if (cc > 10000)
            {
                Debug.LogError("COUNT BREAK");
            }

            Ray2D ray = new Ray2D(curr_pos, Vector3.forward);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 10f);

            var hit = Physics2D.Raycast(ray.origin, ray.direction, 100f, m_LayerMask);
            if (hit.transform != null)
            {
                Debug.Log($"{hit.transform.gameObject.layer} : {curr_pos}");
            }
            else
            {
                Vector2 newVec = new Vector2(curr_pos.x, curr_pos.y);
                m_SpawnPosList.Add(newVec);
                //Instantiate(m_PointItemPrefab, newVec, Quaternion.identity, transform);
            }

            curr_pos.x++;
            cc++;
            if (curr_pos.x > m_RightTop.x)
            {
                curr_pos.y++;
                curr_pos.x = m_LeftBottom.x;
            }
        }

        StartCoroutine(CorItemSpawn());
    }

    IEnumerator CorItemSpawn()
    {
        var data = csvReader.Instance.m_UserLists;

        int total_donation = 0;
        foreach (var item in data)
        {
            total_donation += item.Value.cash;
        }


        if (total_donation < 5000)
            yield break;


        int m_LayerMask =
        (1 << LayerMask.NameToLayer("Character")) |
        (1 << LayerMask.NameToLayer("Map")) |
        (1 << LayerMask.NameToLayer("Point Item")) |
        (1 << LayerMask.NameToLayer("Tile Booker"));


        yield return new WaitUntil(() => Player_Pixel.g_PlayBlock == false);
        yield return new WaitForSeconds(10f);


        while (true)
        {
            if (Player_Pixel.g_PlayBlock)
            {
                yield break;
            }

            Vector2 curr_pos = m_SpawnPosList[Random.Range(0, m_SpawnPosList.Count)];
            Ray2D ray = new Ray2D(curr_pos, Vector3.forward);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 10f);

            var hit = Physics2D.Raycast(ray.origin, ray.direction, 100f, m_LayerMask);
            if (hit.transform != null)
            {
                Debug.LogError("겹침 : " + hit.transform.name + " // " + curr_pos);
                yield return null;
            }
            else
            {
                Instantiate(m_PointItemPrefab, curr_pos, Quaternion.identity, transform);
                yield return new WaitForSeconds(Random.Range(m_SpawnTime.x, m_SpawnTime.y));
            }
        }
    }
}
