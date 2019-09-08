﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vegetable : MonoBehaviour
{
    public enum VegetableType       //Vegetable Types
    {
        A,
        B,
        C,
        D,
        E,
        F
    }

    public enum STATE
    {
        IDLE,
        PICKED,
        TOBECHOPPED,
        CHOPPED,
        READY
    }

    [SerializeField]
    private STATE m_VegetableState;
    [SerializeField]
    private VegetableType m_VegetableType;
    private PlayerController.PlayerIndex m_OwnerPlayerIndex;
    private PlayerController m_OwnerPlayerController;
    private ChoppingBoard m_ChoppingBoardOwner;
    private List<PlayerController.PlayerIndex> m_PlayerInZone = new List<PlayerController.PlayerIndex>();
    public static Action<VegetableType> AddvegetableToSalad;
    private void Start()
    {
        PlayerController.TriggerInput += SubscribeInput;
    }

    private void SubscribeInput(PlayerController playerController, PlayerController.PlayerIndex playerIndex)
    {
        if (!m_PlayerInZone.Contains(playerIndex))
            return;

        switch (m_VegetableState)
        {
            case STATE.IDLE:
                if (playerController.OrderOfCollection.Count == 2)
                    return;
                gameObject.layer = m_OwnerPlayerController.gameObject.layer;
                playerController.OrderOfCollection.Add(m_VegetableType);
                m_OwnerPlayerController.TextStatus.text = "Picked " + m_VegetableType.ToString();
                m_VegetableState = STATE.PICKED;
                this.transform.SetParent(playerController.transform);
                break;
            case STATE.PICKED:
                if (playerController.OrderOfCollection[0] == m_VegetableType)
                {
                    GetComponent<Collider>().enabled = false;
                    this.transform.SetParent(m_ChoppingBoardOwner.transform);
                    this.transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.2f,0.2f), 0, UnityEngine.Random.Range(-0.2f, 0.2f));
                    m_OwnerPlayerController.TextStatus.text = "Placed " + m_VegetableType.ToString() + " On Board";
                    m_VegetableState = STATE.TOBECHOPPED;
                }
                break;
            case STATE.TOBECHOPPED:
                if (!m_ChoppingBoardOwner.m_IsBeingChopped && m_ChoppingBoardOwner.m_IsPlayerInArea)
                {
                    m_OwnerPlayerController.ChangeSpeed(0);
                    m_ChoppingBoardOwner.CurrentVegetableType = m_VegetableType;
                    m_ChoppingBoardOwner.m_IsBeingChopped = true;
                    m_VegetableState = STATE.CHOPPED;
                }
                break;
            case STATE.CHOPPED:
                if (!m_ChoppingBoardOwner.m_IsBeingChopped && m_ChoppingBoardOwner.m_IsPlayerInArea)
                {
                    m_OwnerPlayerController.ChangeSpeed(4);
                    this.transform.SetParent(m_ChoppingBoardOwner.Plate.transform);
                    this.transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.027f, 0.027f), 19f, UnityEngine.Random.Range(-0.16f, 0.2f));
                    this.transform.localScale = new Vector3(.5f, .5f, .5f);
                    m_OwnerPlayerController.TextStatus.text = m_VegetableType.ToString() + " Placed On Plate";
                    if (AddvegetableToSalad != null)
                        AddvegetableToSalad(m_VegetableType);
                    m_OwnerPlayerController.OrderOfCollection.RemoveAt(0);
                    m_VegetableState = STATE.READY;
                }
                break;
            case STATE.READY:
                return;
            default:
                break;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            m_PlayerInZone.Add(other.GetComponent<PlayerController>().PlayerIndexValue);
            m_OwnerPlayerController = other.GetComponent<PlayerController>();
            m_OwnerPlayerIndex = other.GetComponent<PlayerController>().PlayerIndexValue;
        }
        if (other.GetComponent<ChoppingBoard>())
        {
            m_ChoppingBoardOwner = other.GetComponent<ChoppingBoard>();
            if (!m_PlayerInZone.Contains(m_OwnerPlayerIndex))
                m_PlayerInZone.Add(m_OwnerPlayerIndex);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            m_PlayerInZone.Remove(other.GetComponent<PlayerController>().PlayerIndexValue);
        }
        if (other.GetComponent<ChoppingBoard>())
        {
            m_PlayerInZone.Remove(m_OwnerPlayerIndex);
        }
    }

    private void OnDestroy()
    {
        PlayerController.TriggerInput -= SubscribeInput;
    }
}
