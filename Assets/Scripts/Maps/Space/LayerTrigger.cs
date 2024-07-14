using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    //when object exit the trigger, put it to the assigned layer and sorting layers
    //used in the stair objects for player to travel between layers
    public class LayerTrigger : MonoBehaviour
    {
        public string layer;
        public string sortingLayer;
        public int orderInLayer;

        private void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log("Triggered");
            other.gameObject.layer = LayerMask.NameToLayer(layer);

            other.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer;
            other.gameObject.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
            SpriteRenderer[] srs = other.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach ( SpriteRenderer sr in srs)
            {
                sr.sortingLayerName = sortingLayer;
                sr.sortingOrder = orderInLayer;
            }
        }

    }
}
