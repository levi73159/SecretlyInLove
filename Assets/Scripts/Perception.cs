using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(GameplayInput))]
public class Perception : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Image overlay;
    [SerializeField] private float opacityChangeSpeed;
    private bool _isActive = false;
    private float _transparency;

    private GameplayInput _input;

    private ThirdPersonController _controller;

    private float _defaultSpeed;
    private GameObject[] _hidden;

    // Start is called before the first frame update
    private void Awake()
    {
        _transparency = overlay.color.a;
        overlay.enabled = true;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
    }

    private void Start()
    {
        _hidden = GameObject.FindGameObjectsWithTag("Hidden");
        _controller = GetComponent<ThirdPersonController>();
        _input = GetComponent<GameplayInput>();
        _defaultSpeed = _controller.MoveSpeed;
    }

    /// <summary>
    /// Set all the hidden object to visible or non visiable
    /// <see cref="_hidden"/>
    /// </summary>
    private void UpdateHidden()
    {
        foreach (var obj in _hidden)
        {
            if (obj.TryGetComponent<TMPro.TextMeshPro>(out var text))
            {
                Color color = text.color;
                color.a = Mathf.Clamp(overlay.color.a / _transparency, 0, 255);
                text.color = color;
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }
    }

    // gets called by the PlayerInput
    public void OnPerceptionView(InputValue value)
    {
        if (!value.isPressed) return;

        SetActive(!_isActive);
    }

    public void SetActive(bool active) {
        _isActive = active;
        cam.enabled = active;
        _controller.MoveSpeed = active ? _defaultSpeed / 2f : _defaultSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_input.Sprint) SetActive(false); // no sprinting durrint perception
        if (_isActive && overlay.color.a < _transparency)
        {
            Color color = overlay.color;
            color.a += opacityChangeSpeed * Time.deltaTime;
            overlay.color = color;
        }
        else if (!_isActive && overlay.color.a > 0)
        {
            Color color = overlay.color;
            color.a -= opacityChangeSpeed * Time.deltaTime;
            overlay.color = color;
        }
        UpdateHidden();
    }
}
