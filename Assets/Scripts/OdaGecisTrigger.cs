using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public GameManager gameManager;
    public int girilenOdaIndeksi; // Inspector'dan elle girilecek (Örn: Oturma odasý için 2 yaz)

    private bool tetiklendi = false;

    private void OnTriggerEnter(Collider other)
    {
        // Sadece bir kere çalýþsýn ve sadece Player tetiklesin
        if (!tetiklendi && other.CompareTag("Player"))
        {
            tetiklendi = true;
            gameManager.YeniOdayaGirisYapildi(girilenOdaIndeksi);

            // Trigger görevini yaptý, artýk kendini kapatabilir
            gameObject.SetActive(false);
        }
    }
}