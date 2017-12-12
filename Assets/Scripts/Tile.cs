using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

/// <summary>
/// Tile cell effect and touch/click event.
/// </summary>
public class Tile : MonoBehaviour
{
    public GameManager gameManager;

    // tile order
    public int idx = 0;

    public float height = 1f;

    // tile type
    int _type = 0;

    Transform tf;

    public Sprite[] sprites;
    // for changing tile color & shape
    Image shapeRenderer;
    Image tileRenderer;
    // for choice display
    Image choiceRenderer;

    // Save color
    Color color = Color.white;

    // Tile Line Component
    public Line lineScript;

    // Condition when tile is moving
    public bool isMove = false;

    // Tile Color Set
    public Color[] colors = new Color[]{
    	Color.red
		, Color.green
		, Color.blue
		, Color.white
		, Color.yellow
		, Color.cyan
		, Color.magenta
		, Color.gray
		, Color.black
    };

    void Awake()
    {
        tf = transform;
        choiceRenderer = tf.Find("Choice").GetComponent<Image>();
        shapeRenderer = tf.Find("Shape").GetComponent<Image>();
        tileRenderer = tf.Find("Bg").GetComponent<Image>();
        UnSetChoice();
    }

    // Setup Tile Type.
    public void SetTileType(int type)
    {
        _type = type;
        shapeRenderer.sprite = sprites[type];
        SetColor(colors[type]);
    }

    // Get Tile Type.
    public int GetTileType()
    {
        return _type;
    }

    // Click Down Event
    public void OnClickDown()
    {
        gameManager.ChoiceCell(this, lineScript.idx, idx, _type, isMove);
    }

    // Move To Order Position
    public void MoveTo(int seq)
    {
        tf.localPosition = Vector3.up * (seq * height);
    }

    // Set Choice Tile
    public void SetChoice()
    {
        choiceRenderer.enabled = true;
    }
    // Unset Choice Tile
    public void UnSetChoice()
    {
        choiceRenderer.enabled = false;
    }


    // Move with Tweening Animation
    public void TweenMoveTo(int seq)
    {
        TweenMove(tf, tf.localPosition, Vector3.up * (seq * height));
    }

    // Move with Tweening Animation
    void TweenMove(Transform tr, Vector3 pos1, Vector3 pos2)
    {
        tr.localPosition = pos1;
        choiceRenderer.enabled = false;
        if (isMove)
        {
            tileRenderer.enabled = false;
            shapeRenderer.enabled = false;
        }
        StartCoroutine(DelayAction(0.5f, () =>
        {
            if (isMove)
            {
                tileRenderer.enabled = true;
                shapeRenderer.enabled = true;
                isMove = false;
            }
            TweenParms parms = new TweenParms().Prop("localPosition", pos2).Ease(EaseType.EaseOutBounce);
            HOTween.To(tr, 0.4f, parms);
            //The lines below cause the pieces to fit into the positions left empty by the previous match (Without Tween).
            //while (Vector3.Distance(tr.transform.localPosition, pos2) >= 0.1f)
            //    tr.transform.localPosition = Vector3.Lerp(tr.transform.localPosition, pos2, 0.4f);
        }));
    }

    // Reset Tile Color
    public void ResetColor()
    {
        tileRenderer.color = color;
    }

    // Set Tile Color
    public void SetColor(Color c)
    {
        tileRenderer.color = c;
    }

    // Coroutine Delay Method
    IEnumerator DelayAction(float dTime, System.Action callback)
    {
        yield return new WaitForSeconds(dTime);
        callback();
    }
}
