using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
        
public class BotController : MonoBehaviour
{
    private Vector2 YPlus = new Vector2(0, 50);
    private Vector2 YMinus = new Vector2(0, -50);
    private Vector2 XPlus = new Vector2(50, 0);
    private Vector2 XMinus = new Vector2(-50, 0);

    private Boolean _wasUsed;
    private Camera _camera;
    private Launcher _launcher;
    private GameObject _buttonWasHitted;
    [SerializeField] private List<Vector2> _positionToCheck = new List<Vector2>();
    [SerializeField] private List<Vector2> _positionToStep = new List<Vector2>();
    private Vector2 _positionRemembered; 

    private void Start()
    {
        _camera = Camera.main;
        _launcher = _camera.GetComponent<Launcher>();
        }
    
    private void Update()   
    {
        if (!_launcher.IsGameStared)
        {
            if (_launcher.IsFirstPlayerChoised)
            {
                PrepareGame();
            }
        }

        if (!_launcher.IsGameStared) return;

        if (!_launcher.IsFirstPlayerChoised)
        {
            AddPositionToCheck();
            
            
            if (_positionToStep.Count != 0)
            {
                foreach (var position in _positionToStep)
                {
                    foreach (var button in _launcher.buttonsShipsOne)
                    {
                        if (button.GetComponent<RectTransform>().anchoredPosition == position)
                        {
                            if (button.GetComponent<ClickDetection>()._wasChosen)
                            {
                                _positionToStep.Remove(position);
                                return;                 
                            }
                            
                            button.GetComponent<ClickDetection>().OnClick();

                            if (button.GetComponent<ClickDetection>()._isHitted)
                            {
                                _positionToStep.Add(position + (position - _positionRemembered));
                                _positionRemembered = position;
                            }
                            return;
                        }
                    }   
                }           
            }    
            

            if (_positionToCheck.Count != 0)
            {
                foreach (var position in _positionToCheck)
                {
                    foreach (var button in _launcher.buttonsShipsOne)
                    {
                        if (button.GetComponent<RectTransform>().anchoredPosition == position)
                        {
                            button.GetComponent<ClickDetection>().OnClick();

                            if (button.GetComponent<ClickDetection>()._isHitted)
                            {
                                _positionToStep.Add(position + (position - _positionRemembered));
                                _positionRemembered = position;

                                foreach (var position1 in _positionToCheck)
                                {       
                                    if (position1 - _positionRemembered == YPlus || position1 - _positionRemembered == YMinus)
                                    {
                                        _positionToCheck.Remove(position1 + XMinus);
                                        _positionToCheck.Remove(position1 + XPlus);
                                    }
                                    else if (position - _positionRemembered == XPlus || position - _positionRemembered == XMinus)
                                    {
                                        _positionToCheck.Remove(position1 + YMinus);
                                        _positionToCheck.Remove(position1 + YPlus);   
                                    }
                                }
                            }
                            
                            _positionToCheck.Remove(position);
                            return;
                        }
                    }
                }
            }

            if (_positionToStep.Count != 0 && _positionToCheck.Count != 0) return;

            var attempts = 0;       
            var choosed = false;
            
            while (!choosed && attempts < 10000)        
            {
                var randomButton = _launcher.buttonsShipsOne[Random.Range(0, _launcher.buttonsShipsOne.Count)].GetComponent<ClickDetection>();
                
                if (!randomButton._wasChosen)
                {
                    randomButton.OnClick();

                    if (randomButton._isHitted)
                    {
                        _buttonWasHitted = randomButton.gameObject;
                    }
                    choosed = true;
                }   

                attempts++;
            }
        }
    }


    private void AddPositionToCheck()
    {
        if (_buttonWasHitted != null)
        {
            Vector2 position = _buttonWasHitted.GetComponent<RectTransform>().anchoredPosition;

            foreach (var button in _launcher.buttonsShipsOne)
            {
                if (!button.GetComponent<ClickDetection>()._wasChosen)
                {
                    if (position + XPlus == button.GetComponent<RectTransform>().anchoredPosition)
                    {
                        _positionToCheck.Add(position + XPlus); 
                        _positionRemembered = position;
                    }                    
                    
                    if (position + XMinus == button.GetComponent<RectTransform>().anchoredPosition)
                    {
                        _positionToCheck.Add(position + XMinus); 
                        _positionRemembered = position;
                    }                    
                    
                    if (position + YPlus == button.GetComponent<RectTransform>().anchoredPosition)
                    {
                        _positionToCheck.Add(position + YPlus); 
                        _positionRemembered = position;
                    }                    
                    
                    if (position + YMinus == button.GetComponent<RectTransform>().anchoredPosition)
                    {
                        _positionToCheck.Add(position + YMinus); 
                        _positionRemembered = position;
                    }
                }                
            }

            _buttonWasHitted = null;
        }
    }

    private void PrepareGame()
    { 
        foreach (var ship in _launcher.player2Ships)
            ship.transform.GetChild(0).gameObject.SetActive(false);
    
        _launcher.RandomShipsPositions("player2Ships");
            
        _launcher.StartGame();   
    }
}
