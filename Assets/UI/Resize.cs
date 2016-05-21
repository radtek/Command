﻿// --------------------------------------------------------------
// Resize.cs is part of the VLAB project.
// Copyright (c) 2016 All Rights Reserved
// Li Alex Zhang fff008@gmail.com
// 5-21-2016
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace VLab
{
    public class Resize : MonoBehaviour, IPointerDownHandler, IDragHandler,IEndDragHandler
    {
        public Vector2 minSize;
        public Vector2 maxSize;

        private RectTransform parentRectTransform;
        private Vector2 originalLocalPointerPosition;
        private Vector2 originalSizeDelta;

        void Awake()
        {
            var rootcanvassize = (GameObject.Find("Canvas").transform as RectTransform).rect.size;
            minSize = rootcanvassize * 0.2f;
            maxSize = rootcanvassize * 0.9f;
            parentRectTransform = transform.parent as RectTransform;
        }

        public virtual void OnPointerDown(PointerEventData data)
        {
            parentRectTransform.SetAsLastSibling();
            originalSizeDelta = parentRectTransform.sizeDelta;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
        }

        public virtual void OnDrag(PointerEventData data)
        {
            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, data.position, data.pressEventCamera, out localPointerPosition);
            var offsetToOriginal = localPointerPosition - originalLocalPointerPosition;

            parentRectTransform.sizeDelta = new Vector2(
                Mathf.Clamp(originalSizeDelta.x + offsetToOriginal.x, minSize.x, maxSize.x),
                Mathf.Clamp(originalSizeDelta.y - offsetToOriginal.y, minSize.y, maxSize.y)
            );
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
        }
    }
}