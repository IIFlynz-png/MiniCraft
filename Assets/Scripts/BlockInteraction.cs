using UnityEngine;
using System.Collections;

public class BlockInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public float maxDistance = 4f;
    public Material outlineMaterial; // Materiale per il contorno
    private Material originalMaterial; // Materiale originale del blocco
    private Transform selectedBlock; // Blocco attualmente selezionato
    public GameObject blockPrefab; // Prefab per piazzare blocchi

    private bool isBreaking = false; // Controlla se il giocatore sta rompendo un blocco
    private Coroutine breakCoroutine; // Coroutine per gestire la distruzione

    void Update()
    {
        HandleBlockSelection();
        HandleBlockDestruction();
        HandleBlockPlacement();
    }

    void HandleBlockSelection()
    {
        if (selectedBlock != null)
        {
            ResetBlockMaterial();
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Block"))
            {
                selectedBlock = hit.transform;
                Renderer renderer = selectedBlock.GetComponent<Renderer>();

                if (renderer != null)
                {
                    originalMaterial = renderer.material;
                    renderer.material = outlineMaterial; // Applica il materiale del contorno
                }
            }
        }
        else
        {
            selectedBlock = null;
        }
    }

    void HandleBlockDestruction()
    {
        if (Input.GetMouseButtonDown(0) && selectedBlock != null && selectedBlock.tag != "Bedrock") 
        {
            if (!isBreaking) // Se non sta già distruggendo un blocco
            {
                breakCoroutine = StartCoroutine(BreakBlock(selectedBlock));
            }
        }

        if (Input.GetMouseButtonUp(0) && isBreaking)
        {
            StopCoroutine(breakCoroutine); // Interrompe la distruzione se il tasto viene rilasciato
            isBreaking = false;
            Debug.Log("Distruzione annullata.");
        }
    }

    IEnumerator BreakBlock(Transform block)
    {
        isBreaking = true;
        BlockData blockData = block.GetComponent<BlockData>();
        float breakTime = blockData != null ? blockData.breakTime : 1f; // Tempo di distruzione del blocco

        Debug.Log($"Distruzione avviata, tempo richiesto: {breakTime}s");

        yield return new WaitForSeconds(breakTime); // Aspetta il tempo necessario per la distruzione

        Debug.Log("Blocco distrutto!");
        Destroy(block.gameObject);
        isBreaking = false;
    }

    void HandleBlockPlacement()
    {
        if (Input.GetMouseButtonDown(1)) // Aggiungere un blocco
        {
            print("Piazzo blocco");
            if (blockPrefab != null)
            {
                Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, maxDistance))
                {
                    Vector3 placePosition = hit.point + hit.normal * 0.5f;
                    placePosition = new Vector3(Mathf.Round(placePosition.x), Mathf.Round(placePosition.y), Mathf.Round(placePosition.z));

                    if (!Physics.CheckSphere(placePosition, 0.1f))
                    {
                        Instantiate(blockPrefab, placePosition, Quaternion.identity);
                    }
                    else
                    {
                        Debug.Log("Posizione occupata, non posso piazzare il blocco.");
                    }
                }
                else
                {
                    Debug.Log("Nessuna superficie colpita. Non posso piazzare il blocco.");
                }
            }
            else
            {
                Debug.Log("Nessun prefab di blocco assegnato. Assegna un prefab nell'Inspector.");
            }
        }
    }

    void ResetBlockMaterial()
    {
        if (selectedBlock != null && originalMaterial != null)
        {
            selectedBlock.GetComponent<Renderer>().material = originalMaterial;
        }
    }

    void OnGUI()
    {
        float crosshairSize = 10f;
        float x = (Screen.width - crosshairSize) / 2;
        float y = (Screen.height - crosshairSize) / 2;
        
        GUI.Label(new Rect(x, y, crosshairSize, crosshairSize), "+");
    }
}
