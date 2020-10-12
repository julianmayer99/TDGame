using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryRangeControler : MonoBehaviour
{
    public GameObject selected;
    private float selectedDistance;
    private float dist = Mathf.Infinity;

    private List<GameObject> ressourceItems = new List<GameObject>();
    // Start is called before the first frame update
    void Awake()
    {
        selected = null;
        selectedDistance = Mathf.Infinity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject oldSelected = selected;
        foreach(GameObject go in ressourceItems)
        {
            //Calculate Distance
            float dist = Vector3.Distance(go.transform.position, transform.position);
            //If distance is less than selected Distance
            if (dist < selectedDistance)
            {
                selectedDistance = dist;
                selected = go;
            }
        }

        //Make Selections
        if (oldSelected == null && selected != null)
        {
            //SelectNew
            selected.GetComponent<RessourceItemControler>().Select();
        }
        else if (oldSelected != selected)
        {
            //SelectNew
            selected.GetComponent<RessourceItemControler>().Select();
            //DeselectOld
            oldSelected.GetComponent<RessourceItemControler>().Deselect();
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RessourceItem")
        {
            //If the Item is not already in the List
            if (!ressourceItems.Contains(other.gameObject))
            {
                //Add it
                ressourceItems.Add(other.gameObject);
                Debug.Log("Item entered, ItemCount: " + ressourceItems.Count);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RessourceItem")
        {
            //If the Item is in the List
            if (ressourceItems.Contains(other.gameObject))
            {
                //Remove it
                ressourceItems.Remove(other.gameObject);
                Debug.Log("Item left");
                //Reset Selected if Nescessary
                if (other.gameObject == selected)
                {
                    selected.GetComponent<RessourceItemControler>().Deselect();
                    selected = null;
                    selectedDistance = Mathf.Infinity;
                }
            }
        }
    }
}
