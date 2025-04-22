using UnityEngine;

public class LockpickDoor : Door
{
    Item lockpick;
    public LockpickSystem lockpickSystem;

    public override void Interact(NetworkPlayerController owner)
    {
        if(isSealed){
            if(Inventory.Instance.GetMainItemOut(owner, out lockpick) && lockpick.itemName == ItemList.lvl4lockpick){
           
                lockpickSystem.Interact(owner);
            }
            else if(lockpickSystem.hasLockpickBeenInstalled){
                 lockpickSystem.Interact(owner);
            }
            else{
                 base.Interact(owner);
            }
        }
        else{
             base.Interact(owner);
        }
        
       
    }
}
