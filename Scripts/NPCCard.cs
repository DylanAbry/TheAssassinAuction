using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCCard : MonoBehaviour, IPointerClickHandler
{
    public MemoryGameHandler gameHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        gameHandler.NPCCardHit(gameObject);
    }
}
