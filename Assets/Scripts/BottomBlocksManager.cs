using UnityEngine;

public class BottomBlocksManager : MonoBehaviour
{
    // Assuming we have a Block component that controls each block's behavior.
    public Block[] bottomBlocks;

    public void ActivateBlockAt(int index)
    {
        if (index >= 0 && index < bottomBlocks.Length)
        {
            bottomBlocks[index].Activate();
        }
        else
        {
            Debug.LogWarning("Attempted to activate a block out of range: " + index);
        }
    }
}