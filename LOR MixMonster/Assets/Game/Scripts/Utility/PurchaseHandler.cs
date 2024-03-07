namespace GameUtility
{
    public class PurchaseHandler
    {
        public static bool Purchase(int price)
        {
            if (DataManagement.DataManager.Instance.userData.inventory.Cash >= price)
            {
                DataManagement.DataManager.Instance.userData.inventory.cash -= price;
                DataManagement.DataManager.Instance.Save();

                Sound.SoundData soundData = Sound.Controller.Instance.soundData;
                Sound.Controller.Instance.PlayOneShot(soundData.purchaseSFX);
                return true;
            }
            return false;
        }
    }
}