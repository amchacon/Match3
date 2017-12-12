using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PuzzleArea : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerExitHandler
{
    public GameManager gameManager;

    public void OnPointerExit(PointerEventData eventData)
    {
        gameManager.OnPointerExit(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        gameManager.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        gameManager.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        gameManager.OnEndDrag(eventData);
    }
}
