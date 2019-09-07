﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChoppingBoard : MonoBehaviour
{
    public bool m_IsBeingChopped;
    public bool m_IsPlayerInArea;
    private PlayerController m_PlayerController;
    private Vegetable.VegetableType m_CurrentVegetableType;
    [SerializeField]
    private Slider m_Slider;
    private float m_CurrentLerpTime;

    public Slider Slider
    {
        get { return m_Slider; }
    }

    public Vegetable.VegetableType CurrentVegetableType
    {
        get {return m_CurrentVegetableType; }
        set { m_CurrentVegetableType = value; }
    }

    void Start()
    {

    }

    void Update()
    {
        if (m_IsBeingChopped)
        {
            m_Slider.value = Mathf.Lerp(m_Slider.value, 1, Time.deltaTime *1.5f);
            m_PlayerController.TextStatus.text = "Chopping " + m_CurrentVegetableType.ToString();
            if (m_Slider.value>=0.9f)
            {
                m_Slider.value = 0;
                m_IsBeingChopped = false;
                m_PlayerController.TextStatus.text = "Chopped " + m_CurrentVegetableType.ToString();
                m_PlayerController.OrderOfCollection.RemoveAt(0);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            m_IsPlayerInArea = true;
            m_PlayerController = other.GetComponent<PlayerController>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            m_IsPlayerInArea = false;
        }
    }
}
