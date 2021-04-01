using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

sealed class PlayByInputAction : MonoBehaviour
{
    //[SerializeField] InputAction _action = null;
    [SerializeField] InputActionAsset actionAsset = null;
    [SerializeField] Text buttonText;

    RDAudioScript AudioScript;
    float lastFeed, lastDv, lastLerp;
    AudioPeer audioPeer;
    bool update;

    InputActionRebindingExtensions.RebindingOperation rebindOperation;

    void Start()
    {
        audioPeer = GetComponent<AudioPeer>();
        AudioScript = FindObjectOfType<RDAudioScript>();
        AudioScript.RDUpdateShader.SetFloat("_Feed", 0.03f);
        AudioScript.RDUpdateShader.SetFloat("_Dv", 0.3f);
        actionAsset.FindAction("Control").Enable();
    }

    private void Update()
    {
        if (!update) { return; }
        AudioScript.RDUpdateShader.SetFloat("_Feed", lastFeed);
        AudioScript.RDUpdateShader.SetFloat("_Dv", lastDv);
        AudioScript.lerp = lastLerp;
    }

    public void BindKey()
    {
        StartInteractiveRebind();
    }

    public void SetShaderCircleRadius(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            update = false;
            AudioScript.RDUpdateShader.SetFloat("_Feed", ctx.ReadValue<float>() * 0.04f + 0.03f);
            AudioScript.RDUpdateShader.SetFloat("_Dv", ctx.ReadValue<float>() * 0.2f + 0.3f);
            AudioScript.lerp = ctx.ReadValue<float>();

            lastFeed = ctx.ReadValue<float>() * 0.04f + 0.03f;
            lastDv = ctx.ReadValue<float>() * 0.2f + 0.3f;
            lastLerp = ctx.ReadValue<float>();
        }
        else
        {
            update = true;
        }
    }

    void StartInteractiveRebind()
    {
        actionAsset.FindAction("Control").Disable();
        InputAction inputAction = actionAsset.FindAction("Control");
        rebindOperation = inputAction.PerformInteractiveRebinding().OnComplete(operation => RebindCompleted());
        rebindOperation.Start();
    }

    void RebindCompleted()
    {
        buttonText.text = rebindOperation.selectedControl.ToString();
        rebindOperation.Dispose();
        actionAsset.FindAction("Control").Enable();

        //Apply UI Changes (IE: New Binding Icon)
    }
}
