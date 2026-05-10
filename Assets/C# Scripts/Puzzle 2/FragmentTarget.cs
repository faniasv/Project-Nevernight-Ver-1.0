using UnityEngine;
using UnityEngine.EventSystems;

public class FragmentTarget : MonoBehaviour, IDropHandler // Hanya perlu mendeteksi Drop
{
    [Header("Konfigurasi Target")]
    [Tooltip("ID kepingan yang benar untuk slot ini")]
    [SerializeField] private string correctFragmentID;
    [Tooltip("Sudut rotasi Z yang benar untuk kepingan")]
    [SerializeField] private float correctRotationZ = 0f;
    [Tooltip("Toleransi sudut (derajat) agar dianggap pas")]
    [SerializeField] private float rotationTolerance = 10f;

    private VasePuzzleSet vaseSetManager; // Referensi ke manajer set
    private bool isCorrectlyFilled = false; // Status slot

    public void SetupTarget(VasePuzzleSet manager)
    {
        vaseSetManager = manager;
        Debug.Log($"Target {gameObject.name} (mengharapkan {correctFragmentID}) telah di-setup.");
    }

    // 3. BEHAVIOR: Menerima Drop dan Mengecek
    public void OnDrop(PointerEventData eventData)
    {
        if (isCorrectlyFilled) return; // Jika sudah terisi, abaikan

        Debug.Log($"Item di-drop di atas target {correctFragmentID}");
        GameObject droppedObject = eventData.pointerDrag;
        FragmentPuzzle fragment = droppedObject.GetComponent<FragmentPuzzle>();

        // Jika yang di-drop adalah kepingan puzzle
        if (fragment != null)
        {
            // Cek 1: Apakah ID-nya cocok?
            if (fragment.fragmentID == correctFragmentID)
            {
                Debug.Log("ID Kepingan COCOK.");
                // Cek 2: Apakah rotasinya cocok?
                float fragmentRotation = fragment.transform.eulerAngles.z;
                float angleDifference = Mathf.Abs(Mathf.DeltaAngle(fragmentRotation, correctRotationZ));

                if (angleDifference <= rotationTolerance)
                {
                    // Sesuai brief: "Bisa narik puzzle snap"
                    Debug.Log("Rotasi BENAR. Kepingan terkunci!");
                    isCorrectlyFilled = true;

                    // Lakukan Snap
                    fragment.transform.position = this.transform.position;
                    fragment.transform.rotation = this.transform.rotation;

                    // Kunci kepingan
                    fragment.LockInPlace();

                    // Beri tahu VasePuzzleSet (manager set) bahwa 1 target selesai
                    if (vaseSetManager != null)
                    {
                        vaseSetManager.NotifyTargetCompleted();
                    } else 
                    {
                        Debug.LogWarning("Setup not called yet - finding VasePuzzleSet manually");
                        VasePuzzleSet puzzleSet = GetComponentInParent<VasePuzzleSet>();
                        if (puzzleSet != null)
                        {
                            puzzleSet.NotifyTargetCompleted();
                        }
                    }
                }
                else
                {
                    Debug.Log($"Rotasi SALAH. (Target: {correctRotationZ}, Aktual: {fragmentRotation})");
                }
            }
            else
            {
                Debug.Log($"ID Kepingan SALAH. (Target: {correctFragmentID}, Aktual: {fragment.fragmentID})");
            }
        }
    }
}