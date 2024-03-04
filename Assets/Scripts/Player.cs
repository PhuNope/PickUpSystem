using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    [SerializeField]
    private LayerMask pickableLayerMask;

    [SerializeField]
    private Transform playerCameratransform;

    [SerializeField]
    private GameObject pickUpUI;

    [SerializeField]
    [Min(1)]
    private float hitRange = 3;

    [SerializeField]
    private Transform pickUpParent;

    [SerializeField]
    private GameObject inHandItem;

    [SerializeField]
    private InputActionReference interactionInput, dropInput, useInput;

    private RaycastHit hit;

    [SerializeField]
    private AudioSource pickUpSource;

    private void Start() {
        interactionInput.action.performed += PickUp;
        dropInput.action.performed += Drop;
        useInput.action.performed += Use;
    }

    private void Use(InputAction.CallbackContext obj) {
        if (inHandItem != null) {
            IUseable useable = inHandItem.GetComponent<IUseable>();
            if (useable != null) {
                useable.Use(this.gameObject);
            }
        }
    }

    private void Drop(InputAction.CallbackContext obj) {
        if (inHandItem != null) {
            inHandItem.transform.SetParent(null);
            inHandItem = null;
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = false;
            }
        }
    }

    private void PickUp(InputAction.CallbackContext obj) {
        if (hit.collider != null && inHandItem == null) {
            IPickable pickableItem = hit.collider.GetComponent<IPickable>();
            if (pickableItem != null) {
                pickUpSource.Play();
                inHandItem = pickableItem.PickUp();
                inHandItem.transform.SetParent(pickUpParent.transform, pickableItem.KeepWorldPosition);
            }
        }

        //if (hit.collider != null) {
        //    Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
        //    if (hit.collider.GetComponent<Food>()) {
        //        inHandItem = hit.collider.gameObject;
        //        inHandItem.transform.position = Vector3.zero;
        //        inHandItem.transform.rotation = Quaternion.identity;
        //        inHandItem.transform.SetParent(pickUpParent.transform, false);
        //        if (rb != null) {
        //            rb.isKinematic = true;
        //        }
        //        return;
        //    }

        //    if (hit.collider.GetComponent<Item>()) {
        //        inHandItem = hit.collider.gameObject;
        //        inHandItem.transform.position = Vector3.zero;
        //        inHandItem.transform.rotation = Quaternion.identity;
        //        inHandItem.transform.SetParent(pickUpParent.transform, false);
        //        if (rb != null) {
        //            rb.isKinematic = true;
        //        }
        //        return;
        //    }
        //}
    }

    private void Update() {
        if (hit.collider != null && inHandItem == null) {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
            pickUpUI.SetActive(false);
        }

        if (inHandItem != null) {
            return;
        }

        if (Physics.Raycast(playerCameratransform.position, playerCameratransform.forward, out hit, hitRange, pickableLayerMask)) {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            pickUpUI.SetActive(true);
        }
    }

    public void AddHealth(int healthBoost) {

    }
}
