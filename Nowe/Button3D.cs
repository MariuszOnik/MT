using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events; 
using UnityEngine;




public class Button3D : MonoBehaviour
{
    //czy przycisk jest aktywny
    public bool isActivNow; 
    
    //Typ przycisku, mozna go zmienic w skrypcie "Button3DName", to zwykły enum, ale potrzebny do porownywania przyciskow podczas tastow
    //mozna zamiast Button3DName.znak_1 wstawic cokolwiek innego
    public Button3DName button3DName;

    //Deklarujemy Even wywoływany po kliknieciu na przycisk
    public Button3DClickEvent button3DClickEvent;
    
    //dystans na jaki przycisk ma sie poruszyc w dol po kliknieciu
    public float distanceClickVizualization = 0.1f;

    //szybkosc powrotu do pozycji docelowej
    public float returnPositionSpeed = 1f; 

    
        
    //zapamietuje pierwotna pozycje
    public Vector3 defaultPosition;

    //jeżeli przycisk 3d ma nietypowy kształt , nalezy uzyc MeshColider, jezeli wystarczy jakikolwiek podstawowy colider ustawic zmienna na false
    public bool shouldIUseMeshColider = false; 


    //Musimy utworzyc wlasna klase, aby moc przekazac parametr typu "Button3DName"  do Eventu 
    [System.Serializable]
    public class Button3DClickEvent : UnityEvent<Button3DName>
    {

    }

    private void Awake()
    {
        //Czy przycisk ma nietypowy kształt 3d ? Jezeli tak nalezy uzuc meshColidera
         if(shouldIUseMeshColider == true)
         {
            UseMeshColider(); 
         }

        isActivNow = true; 
        if (distanceClickVizualization == 0)
        {
            distanceClickVizualization = 0.1f; 
        }
        
        
    }
    void Start()
    {
        //zapisujemy pierotna pozycje 
        defaultPosition = transform.position;
        //sprawdzamy czy jest dołączony Collider
        IhaveAnyColiderTest(); 

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        //jezeli jest acktywny
        if (isActivNow)
        {
            //przycisk idzie w dol po kliknieciu 
            transform.position = new Vector3(defaultPosition.x, defaultPosition.y - distanceClickVizualization, defaultPosition.z);

            //wywolujemy Event
            if (button3DClickEvent != null)
            {
                button3DClickEvent.Invoke(button3DName);
            }
            //startujemy z powrotem do docelowej pozycji 
            StartCoroutine("ReturnToDefaultPosition");
        }
        
    }

    //wraca przycisk na docelowa pozycje
    IEnumerator ReturnToDefaultPosition()
    {
        if (distanceClickVizualization <= 0)
        {

            Debug.LogError("Odległość najaką przesuwa się przycisk3D o nazwie " + gameObject.name+ " nie może być ujemna, zmień ustawienia w inspektorze , pozycja distanceClickVizualization ");
        }

         
        while(transform.position.y < defaultPosition.y)
        {
            
            transform.position = new Vector3(transform.position.x, transform.position.y + returnPositionSpeed * Time.deltaTime, transform.position.z);
            yield return null; 
        }
        StopCoroutine("ReturnToDefaultPosition"); 
    }

    //sprawdź czy są inne colidery i usuń je , dodaj mesh colider
    public void UseMeshColider()
    {
            
            var meshcolider = gameObject.GetComponent<MeshCollider>();
            var boxColider = gameObject.GetComponent<BoxCollider>();
            var sphereColider = gameObject.GetComponent<SphereCollider>();
            var capsuleColider = gameObject.GetComponent<CapsuleCollider>();

            
            if (boxColider != null)
            {
                Destroy(boxColider);
            }
            if (sphereColider != null)
            {
                Destroy(sphereColider);
            }
            if (capsuleColider != null)
            {
                Destroy(capsuleColider);
            }

            if (meshcolider == null)
            {
                this.gameObject.AddComponent<MeshCollider>();
                gameObject.GetComponent<MeshCollider>().convex = true;
            }
            else
            {
                meshcolider.convex = true;
            }
        
    }

    public void IhaveAnyColiderTest()
    {
        int test = 0;
        var meshcolider = gameObject.GetComponent<MeshCollider>();
        var boxColider = gameObject.GetComponent<BoxCollider>();
        var sphereColider = gameObject.GetComponent<SphereCollider>();
        var capsuleColider = gameObject.GetComponent<CapsuleCollider>();

        if (boxColider != null)
        {
            test += 1; 
        }
        if (sphereColider != null)
        {
            test += 1;
        }
        if (capsuleColider != null)
        {
            test += 1;
        }

        if (meshcolider != null)
        {
            test += 1;
        }

        if(test == 0)
        {
            Debug.LogError("przycisk3D o nazwie " + gameObject.name + " nie posiada żadnego Collidera musisz go dodać  ");
        }

    }
}
