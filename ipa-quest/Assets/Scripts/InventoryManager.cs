using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] Image heartPiece;
    [SerializeField] GameObject upCast, sideCast, downCast;
    [SerializeField] GameObject dash, varJump, wallJump;

    private void OnEnable()
    {
        heartPiece.fillAmount = PlayerController.Instance.heartPieces * 0.25f;

        //explosion
        if (PlayerController.Instance.unlockedUpCast)
        {
            upCast.SetActive(true);
        }
        else
        {
            upCast.SetActive(false);
        }
        // fireball and heal
        if (PlayerController.Instance.unlockedSideCast)
        {
            sideCast.SetActive(true);
        }
        else
        {
            sideCast.SetActive(false);
        }
        // divebomb
        if (PlayerController.Instance.unlockedDownCast)
        {
            downCast.SetActive(true);
        }
        else
        {
            downCast.SetActive(false);
        }
        // dash
        if (PlayerController.Instance.unlockedDash)
        {
            dash.SetActive(true);
        }
        else
        {
            dash.SetActive(false);
        }
        // double jump
        if (PlayerController.Instance.unlockedVarJump)
        {
            varJump.SetActive(true);
        }
        else
        {
            varJump.SetActive(false);
        }
        // wall jump
        if (PlayerController.Instance.unlockedWallJump)
        {
            wallJump.SetActive(true);
        }
        else
        {
            wallJump.SetActive(false);
        }
    }
}
