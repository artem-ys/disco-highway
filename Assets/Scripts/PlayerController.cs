using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class PlayerController : MonoBehaviour
{
    // Reference to the BottomBlocksManager which controls the blocks at the bottom.
    private BottomBlocksManager _bottomBlocksManager;

    // Define activation keys for the bottom blocks.
    private readonly KeyCode[] _activationKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };

    [Inject]
    public void InjectDependencies(BottomBlocksManager bottomBlocksManager)
    {
        this._bottomBlocksManager = bottomBlocksManager;
    }
    
    void Update()
    {
        for (int i = 0; i < _activationKeys.Length; i++)
        {
            if (Input.GetKeyDown(_activationKeys[i]))
            {
                // When the player presses one of the defined keys, activate the corresponding block.
                _bottomBlocksManager.ActivateBlockAt(i);
            }
        }
    }
}