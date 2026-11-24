using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputRebind : MonoBehaviour
{
    public string actionPath;
    public string labelName;

    public TextMeshProUGUI actionLabelText;
    public Button rebindButton;
    public TextMeshProUGUI keycapText;
    public string waitingText = "...";
    private bool isRebinding;

    private void Awake() {
        if(rebindButton != null)
            rebindButton.onClick.AddListener(StartRebind);
    }
    
    public void RefreshUI()
    {
        if(actionLabelText != null && !string.IsNullOrEmpty(labelName))
            actionLabelText.text = labelName;

        if(keycapText != null)
        {
            string binding = InputManager.instance.GetBinding(actionPath);
            keycapText.text = string.IsNullOrEmpty(binding) ? "?" : binding;
        }
    }
    
    public void StartRebind()
    {
        if(isRebinding)
            return;
        isRebinding = true;
        string previousText = keycapText != null ? keycapText.text : "";

        if(keycapText != null)
            keycapText.text = waitingText;

        InputManager.instance.RebindAction(actionPath, newBinding =>
        {
           isRebinding = false;
           if(keycapText != null)
                keycapText.text = string.IsNullOrEmpty(newBinding) ? previousText :  newBinding;
        });
    }
}