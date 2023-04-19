using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyDemoProject
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UIObjects _UIObjects;

        private ISelectable currentSelection;

        private void OnEnable()
        {
            EventManager.OnSelectableObjectPicked += OnSelectableObjectPicked;
            EventManager.OnPlaceableObjectUnpicked += OnPlaceableObjectUnpicked;
            EventManager.OnSelectableObjectDestroyed += OnSelectableObjectDestroyed;
        }

        private void OnDisable()
        {
            EventManager.OnSelectableObjectPicked -= OnSelectableObjectPicked;
            EventManager.OnPlaceableObjectUnpicked -= OnPlaceableObjectUnpicked;
            EventManager.OnSelectableObjectDestroyed -= OnSelectableObjectDestroyed;
        }

        private void OnSelectableObjectPicked(ISelectable selectableObject, bool hasProduct)
        {
            if (selectableObject == null)
            {
                if (_UIObjects.InformationPanel.activeInHierarchy)
                    _UIObjects.InformationPanel.SetActive(false);

                return;
            }

            currentSelection = selectableObject;


            if (!_UIObjects.InformationPanel.activeInHierarchy)
                _UIObjects.InformationPanel.SetActive(true);

            _UIObjects.SelectedObjectImage.sprite = selectableObject.PlaceableDataSO.ImageSprite;

            BuildingDataSO buildingDataSO = selectableObject.PlaceableDataSO as BuildingDataSO;

            _UIObjects.SelectedObjectNameText.text = buildingDataSO.BuildingType.ToString();

            if (!hasProduct)
            {
                CloseProductionPanel();
                return;
            }

            OpenProductionPanel();

            BarrackDataSO barrackDataSO = selectableObject.PlaceableDataSO as BarrackDataSO;

            _UIObjects.SelectedObjectProductImage.sprite = barrackDataSO.ProductionData.ImageSprite;
            _UIObjects.SelectedObjectProductNameText.text = barrackDataSO.ProductionData.UnitType.ToString();

            _UIObjects.SelectableObjectProductButton.ChangeUnitProducer(selectableObject);
        }

        private void OnPlaceableObjectUnpicked()
        {
            if (_UIObjects.InformationPanel.activeInHierarchy)
                _UIObjects.InformationPanel.SetActive(false);
        }

        private void CloseProductionPanel()
        {
            _UIObjects.SelectedObjectProductImage.gameObject.SetActive(false);
            _UIObjects.SelectedObjectProductNameText.gameObject.SetActive(false);
        }

        private void OpenProductionPanel()
        {
            _UIObjects.SelectedObjectProductImage.gameObject.SetActive(true);
            _UIObjects.SelectedObjectProductNameText.gameObject.SetActive(true);
        }

        private void OnSelectableObjectDestroyed(ISelectable selectable)
        {
            if (selectable == currentSelection)
            {
                if (_UIObjects.InformationPanel.activeInHierarchy)
                    _UIObjects.InformationPanel.SetActive(false);
            }
        }
    }
}
