﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckCombinationWithOrder : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private float m_WrongOrderScore;
    [SerializeField]
    private float m_CorrectOrderScore;
    [SerializeField]
    private float m_QuickDeliveryRewardAmount;
    private List<PlayerController.PlayerIndex> m_PlayerInZone = new List<PlayerController.PlayerIndex>();
    private PlayerController m_OwnerPlayerController;
    private PlayerController m_AngryPenalizablePlayer;
    private RandomOrderCombination m_RandomCombination;
    private List<Vegetable.VegetableType> m_PlayerCookedCombination;
    private Customer m_CustomerScript;
    public bool m_IsCorrectCombination;
    public static Action<PlayerController> RewardPlayer;
    #endregion

    #region Properties
    public PlayerController AngryPenalizablePlayer
    {
        get { return m_AngryPenalizablePlayer; }
    }
    #endregion

    #region Unity callbacks
    private void Start()
    {
        PlayerController.TriggerInput += MatchCombination;
        m_RandomCombination = GetComponent<RandomOrderCombination>();
        m_CustomerScript = GetComponent<Customer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInChildren<Plate>())
        {
            m_OwnerPlayerController = other.GetComponentInParent<PlayerController>();
            m_PlayerCookedCombination = m_OwnerPlayerController.GetComponentInChildren<Plate>().SaladCombination;
            if (!m_PlayerInZone.Contains(m_OwnerPlayerController.PlayerIndexValue))
                m_PlayerInZone.Add(m_OwnerPlayerController.PlayerIndexValue);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Plate>())
        {
            m_PlayerInZone.Remove(other.GetComponentInParent<PlayerController>().PlayerIndexValue);
        }
    }

    private void OnDestroy()
    {
        PlayerController.TriggerInput -= MatchCombination;
    }
    #endregion

    #region Class Functions
    private void MatchCombination(PlayerController playerController)          //Check Customer Order Combination with Player Made Combination
    {
        if (!m_PlayerInZone.Contains(playerController.PlayerIndexValue))
            return;
        m_IsCorrectCombination = CheckMatch();
        if(m_IsCorrectCombination)
        {
            m_OwnerPlayerController = playerController;
            float checkForQuickDelivery = 0.7f * m_CustomerScript.WaitingTime;
            if (m_CustomerScript.CurrentTime < checkForQuickDelivery)
            {
                if (RewardPlayer != null)
                    RewardPlayer(playerController);
                ResetCustomerForCorrectOrder(m_QuickDeliveryRewardAmount);
                m_PlayerInZone.Clear();
                m_OwnerPlayerController = null;
                m_IsCorrectCombination = false;
                return;
            }
            ResetCustomerForCorrectOrder(m_CorrectOrderScore);
        }
        else
        {
            m_OwnerPlayerController.TextStatus.text = "Wrong Order";
            m_AngryPenalizablePlayer = m_OwnerPlayerController;
            m_CustomerScript.CustomerStateEnum = Customer.CustomerState.ANGRY;
            m_CustomerScript.m_GotWrongOrder = true;
            foreach (Transform child in m_OwnerPlayerController.transform)
            {
                Destroy(child.gameObject);
            }
            m_OwnerPlayerController.UpdateScoreForPlayer(m_WrongOrderScore);
        }
        m_PlayerInZone.Clear();
        m_OwnerPlayerController = null;
        m_IsCorrectCombination = false;
    }

    private bool CheckMatch()
    {
        if (m_RandomCombination.CustomerOrderCombination.Count != m_PlayerCookedCombination.Count)
            return false;
        for (int i = 0; i < m_RandomCombination.CustomerOrderCombination.Count; i++)
        {
            if (!m_RandomCombination.CustomerOrderCombination.Contains(m_PlayerCookedCombination[i]))
                return false;
        }
        return true;
    }

    private void ResetCustomerForCorrectOrder(float Score)                   //If its a correct order thenn add score to player and bring new customer i.e Reset Customer Demand
    {
        m_CustomerScript.NewCustomerOrder();
        m_OwnerPlayerController.UpdateScoreForPlayer(Score);
        m_OwnerPlayerController.TextStatus.text = "Correct Order";
        m_CustomerScript.m_GotWrongOrder = false;
        m_CustomerScript.GetComponent<MeshRenderer>().material.color = Color.gray;
        foreach (Transform child in m_OwnerPlayerController.transform)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion
}
