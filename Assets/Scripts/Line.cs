using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tile Line effect.
/// </summary>
public class Line : MonoBehaviour {
    // line order
    public int idx = 0;

    // tile list in line
    public Tile[] items;

    // sort time order in line
    public void SortCells()
    {
        List<Tile> tlist = new List<Tile>();
        int y = 0, t = items.Length;
        for (int i = 0; i < items.Length; i++)
            if (!items[i].isMove)
            {
                tlist.Add(items[i]);
                items[i].idx = y++;
            }
        for (int i = 0; i < items.Length; i++)
            if (items[i].isMove)
            {
                tlist.Add(items[i]);
                items[i].idx = y++;
                items[i].MoveTo(t++);
                items[i].SetTileType(Random.Range(0, 5) % 5);
            }
        items = tlist.ToArray();
        for (int i = 0; i < items.Length; i++) items[i].TweenMoveTo(i);
    }
}
