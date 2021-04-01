using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

sealed class PlayByInputAction : MonoBehaviour
{
    [SerializeField] public InputAction _action = null;
    [SerializeField] Text buttonText;

    public InputAction action;

    RDAudioScript AudioScript;
    float lastFeed, lastDv;
    AudioPeer audioPeer;
    bool checkForButton;

    InputActionRebindingExtensions.RebindingOperation rebindOperation;

    void Start()
    {
        audioPeer = GetComponent<AudioPeer>();
        AudioScript = FindObjectOfType<RDAudioScript>();
        AudioScript.RDUpdateShader.SetFloat("_Feed", 0.03f);
        AudioScript.RDUpdateShader.SetFloat("_Dv", 0.3f);
    }

    private void Update()
    {
        if (checkForButton)
        {

        }
    }

    void OnEnable()
    {
        _action.performed += OnPerformed;
        _action.Enable();
    }

    void OnDisable()
    {
        Debug.Log("stop");
        AudioScript.RDUpdateShader.SetFloat("_Feed", lastFeed);
        AudioScript.RDUpdateShader.SetFloat("_Dv", lastDv);
        _action.performed -= OnPerformed;
        _action.Disable();
    }

    void OnPerformed(InputAction.CallbackContext context)
      => SetShaderCircleRadius(context);

    public void SetShaderCircleRadius(InputAction.CallbackContext ctx)
    {
        AudioScript.RDUpdateShader.SetFloat("_Feed", ctx.ReadValue<float>() * 0.04f + 0.03f);
        lastFeed = ctx.ReadValue<float>() * 0.04f + 0.03f;
        AudioScript.RDUpdateShader.SetFloat("_Dv", ctx.ReadValue<float>() * 0.2f + 0.3f);
        lastDv = ctx.ReadValue<float>() * 0.2f + 0.3f;
        AudioScript.lerp = ctx.ReadValue<float>();
    }

    public void StartInteractiveRebind()
    {
        OnDisable();
        rebindOperation = _action.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted());
    }

    void RebindCompleted()
    {
        rebindOperation.Dispose();
    }

    public void RemapButtonClicked()
    {
        OnDisable();

        var rebindOperation = _action.PerformInteractiveRebinding()
                    // To avoid accidental input from mouse motion
                    .WithControlsExcluding("Mouse")
                    .OnMatchWaitForAnother(0.1f)
                    .Start();
    }
}
