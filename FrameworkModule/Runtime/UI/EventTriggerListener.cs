﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CenturyGame.Framework.UI
{
    public class EventTriggerListener : EventTrigger
    {

        #region event
        public delegate void PointerEventDelegate(GameObject go, PointerEventData eventData);
        public delegate void BaseEventDelegate(GameObject go, BaseEventData eventData);
        public delegate void AxisEventDelegate(GameObject go, AxisEventData eventData);

        public event PointerEventDelegate onPointerEnter;
        public event PointerEventDelegate onPointerExit;
        public event PointerEventDelegate onPointerDown;
        public event PointerEventDelegate onPointerUp;
        public event PointerEventDelegate onPointerClick;
        public event PointerEventDelegate onInitializePotentialDrag;
        public event PointerEventDelegate onBeginDrag;
        public event PointerEventDelegate onDrag;
        public event PointerEventDelegate onEndDrag;
        public event PointerEventDelegate onDrop;
        public event PointerEventDelegate onScroll;
        public event BaseEventDelegate onUpdateSelected;
        public event BaseEventDelegate onSelect;
        public event BaseEventDelegate onDeselect;
        public event AxisEventDelegate onMove;
        public event BaseEventDelegate onSubmit;
        public event BaseEventDelegate onCancel;
        public Action onLongPress;

        public float durationPress = 1.0f;
        private bool isPointerDown = false;
        private bool longPressTriggered = false;
        private float timePressStarted;
        #endregion

        public static EventTriggerListener GetListener(GameObject go)
        {
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null)
                listener = go.AddComponent<EventTriggerListener>();
            return listener;
        }

        private void Update()
        {
            if (isPointerDown && !longPressTriggered)
            {
                if (Time.time - timePressStarted > durationPress)
                {
                    longPressTriggered = true;
                    if (onLongPress != null)
                        onLongPress();
                }
            }
        }

        #region 方法  
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onPointerEnter != null) onPointerEnter(gameObject, eventData);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onPointerExit != null) onPointerExit(gameObject, eventData);
            isPointerDown = false;
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onPointerDown != null) onPointerDown(gameObject, eventData);
            timePressStarted = Time.time;
            isPointerDown = true;
            longPressTriggered = false;
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onPointerUp != null) onPointerUp(gameObject, eventData);
            isPointerDown = false;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onPointerClick != null && !longPressTriggered) onPointerClick(gameObject, eventData);
        }
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (onInitializePotentialDrag != null) onInitializePotentialDrag(gameObject, eventData);
        }
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (onBeginDrag != null) onBeginDrag(gameObject, eventData);
        }
        public override void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null) onDrag(gameObject, eventData);
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (onEndDrag != null) onEndDrag(gameObject, eventData);
        }
        public override void OnDrop(PointerEventData eventData)
        {
            if (onDrop != null) onDrop(gameObject, eventData);
        }
        public override void OnScroll(PointerEventData eventData)
        {
            if (onScroll != null) onScroll(gameObject, eventData);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (onUpdateSelected != null) onUpdateSelected(gameObject, eventData);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null) onSelect(gameObject, eventData);
        }
        public override void OnDeselect(BaseEventData eventData)
        {
            if (onDeselect != null) onDeselect(gameObject, eventData);
        }
        public override void OnMove(AxisEventData eventData)
        {
            if (onMove != null) onMove(gameObject, eventData);
        }
        public override void OnSubmit(BaseEventData eventData)
        {
            if (onSubmit != null) onSubmit(gameObject, eventData);
        }
        public override void OnCancel(BaseEventData eventData)
        {
            if (onCancel != null) onCancel(gameObject, eventData);
        }
        #endregion

    }
}