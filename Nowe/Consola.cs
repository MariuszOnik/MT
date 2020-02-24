using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events; 
using UnityEngine;

public class Consola : MonoBehaviour
{
    //Przyjmijmy ze Consola to Maszyna Do Testow


    // To jest cel jaki aktualnie trzeba wcisnąć, musi miec zmienna enum Button3DNameEnum
    //to do tej zmiennej beda porownywane aktualnie klikniete przyciski
    GameObject curentTarget;

    public GameObject startTestButton3D;
    public GameObject stopTestButton3D; 
    public GameObject[] my3dButtons;
    //public int curentSet3DbuttonLength; 

    public GameObject targetPosition;

    public AudioClip[] countDownSounds;

    //czy test juz trwa ? 
    

    //Przycisk uruchamiajacy test, musi posiadac skrypt Button3D
     
    //czas odliczania przed startem
    public int countDownTimeBeforeStart;

    [HideInInspector]
    public int score; 
    //dzwięki odtwarzane podczas odliczania do startu
   

    //referencje do wszystkich przyciskow w maszynie testowej
    //W inspektorze nalezy je dodac recznie, alternatywnie mozna kazdemu przypisac
    //"TAG" i automatycznie pobrac w skrypcie za pomoca metody GameObject.FindObjectsWithTag("TAG")
    
    
    
    
    //
    [HideInInspector]
    public GameObject[] curentSetOf3Dbuttons;
    
    //tutaj tworzymy uniwersalna reakcje na klikniecie, kazda metoda o tej sygnaturze moze byc przekazana do metody "AddToAllButton3DListener"  jako argument
    public  UnityAction<Button3DName> response;

    //Wszystkie klipy audio , dodac w inspektorze
    public AudioClip[] button3DSounds;

    //odtwarzacz, jezeli nie dodano w isnpektorze, w Awake() sprawdzamy czy istnieje a jezeli nie sami go tworzymy
    public AudioSource audioSource; 
    
    //Kazdy przycisk po kliknieciu wyzwala swoj Event, tutaj podpinamy odpowiedź maszyny testowej na ten event do kazdego przycisku 3D
    //Jako parametr podaj metode ktora ma byc wywołana jako odpowiedź, metoda ta musi miec tą samą sygnature jak  
    //np. public void OdpowiedzNaKlikniecie(Button3DName _name){ ...zrob cos... };
    public void AddToAllButton3DListener(UnityAction<Button3DName> action)
    {
        if (my3dButtons.Length > 0)
        {
            foreach (var item in my3dButtons)
            {
                item.GetComponent<Button3D>().button3DClickEvent.AddListener(action);
                

            }
        }
        else
        {
            Debug.LogError("Nie przypisano zadnego przycisku do maszyny w inspektorze, pole My 3D Buttons");
        }
    }


    //Jeżeli przed startem tesu ma być zoobrazowane odliczanie tutaj dodac obiekty ktore maja się pokazac podczas odliczania. 
    public GameObject[] counterBeforeStartVisualizationObjects;

    public bool testIsRunNow = false;




    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        countDownTimeBeforeStart = countDownTimeBeforeStart == 0 ? 3 : countDownTimeBeforeStart; 
        audioSource = CheckIfAduioSourceExistInsceneAndIfNotCreate();
        CheckIfEverythingIsNeededToRunConsole();
        
        //podpinamy metode start do przycisku start
        if(startTestButton3D != null)
        {
            startTestButton3D.GetComponent<Button3D>().button3DClickEvent.AddListener(StartCountDown); 
        }
        if (stopTestButton3D != null)
        {
            stopTestButton3D.GetComponent<Button3D>().button3DClickEvent.AddListener(StopTest);
        }

        AddToAllButton3DListener(ResponseToClick);

        RandomCurentSetButtons(my3dButtons.Length); 
        curentTarget = my3dButtons[0]; 
       

        

    }

    // Update is called once per frame
    void Update()
    {
        if(testIsRunNow == true)
        {

           // Debug.Log("TrwaTest"); 
        }
    }

    public AudioSource CheckIfAduioSourceExistInsceneAndIfNotCreate()
    {
        AudioSource _audioSource = FindObjectOfType<AudioSource>();
        if (audioSource == null)
        {
            GameObject ConsolaAudioSource = new GameObject("ConsolaAudioSource");
            ConsolaAudioSource.AddComponent<AudioSource>();
            ConsolaAudioSource.transform.parent = this.transform;
            _audioSource = ConsolaAudioSource.GetComponent<AudioSource>();
            return _audioSource; 
        }
        else
        {
            return _audioSource; 
        }
    }

    public void CheckIfEverythingIsNeededToRunConsole()
    {
        if(my3dButtons.Length == 0)
        {
            Debug.LogError("Nie przypisano przyciskow do maszyny Testowej");
        }
        if(audioSource == null)
        {
            Debug.LogError("Brak Komponentu AudioSource na Scenie");
        }
        if(curentTarget == null)
        {
            //Debug.LogError("Musisz dodac obiekt pokazujacy aktualny cel"); 
        }
        if(curentTarget != null)
        {
            var t = curentTarget.GetComponent<Button3D>(); 
            if (t == null){
                Debug.LogError("Obiekt curentTarget nie posiada zmiennej typu Button3DName "); 
            }
        }
        
    }


    IEnumerator CountDownBeforeStart()
    {
        
        startTestButton3D.GetComponent<Button3D>().isActivNow = false; 
        if(testIsRunNow == false)
        {
           
            for (int i = 0; i < countDownTimeBeforeStart; i++)
            {
                if (i < counterBeforeStartVisualizationObjects.Length)
                {
                    if (counterBeforeStartVisualizationObjects[i] != null)
                    {
                        counterBeforeStartVisualizationObjects[i].SetActive(true);
                    }
                }
                
                if (i < countDownSounds.Length)
                {
                    if (countDownSounds[i] != null)
                    {
                      
                        audioSource.PlayOneShot(countDownSounds[i]);
                    }
                }

                yield return new WaitForSeconds(1);
            }


            if (counterBeforeStartVisualizationObjects.Length > 0)
            {
                foreach (var item in counterBeforeStartVisualizationObjects)
                {
                    if (item.activeInHierarchy == true)
                        item.SetActive(false);
                }
            }
            testIsRunNow = true;
            RandomCurentSetButtons(my3dButtons.Length);
            
        }
}

    public void StartCountDown(Button3DName button3DName)
    {
        StartCoroutine("CountDownBeforeStart"); 
    }

    public void RandomCurentSetButtons(int ile)
    {

        List<GameObject> _all = new List<GameObject>(my3dButtons);

        List<GameObject> _current = new List<GameObject>();


        for (int i = 0; i < ile; i++)
        {

            var curentRandom = Random.Range(0, _all.Count);
            _current.Add(_all[curentRandom]);
            _all.Remove(_all[curentRandom]);
        }

        curentSetOf3Dbuttons = _current.ToArray();
        /*for (int i = 0; i < _current.Count; i++)
        {
            curentSetOf3Dbuttons[i] = _current[i]; 
            

            //buttons[i].GetComponent<znakButtonScript>().SetCurentSprite(_newSprite[i]);
        }*/
        if (testIsRunNow)
        {
            curentTarget = curentSetOf3Dbuttons[0];

        GameObject temp = Instantiate(curentTarget);
        temp.transform.name = "temp"; 
        temp.transform.position = targetPosition.transform.position; 
        }
        
        
        // curentSetToTest = _newSprite.ToArray();
    }


    public void ResponseToClick(Button3DName _name)
    {
        if (button3DSounds.Length > 0)
        {
            audioSource.PlayOneShot(button3DSounds[0]);
        }
        audioSource.PlayOneShot(button3DSounds[0]); 
        int nameIndex = EnumToInt(_name);
        int curentTargetIndex = EnumToInt(curentTarget.GetComponent<Button3D>().button3DName); 
        if(nameIndex == curentTargetIndex)
        {
            if (testIsRunNow)
            {
                score += 1; 
                RandomNewSprite();
            }
             
        }
    }

    public void RandomNewSprite()
    {
        var _newCurentButton3D = curentSetOf3Dbuttons[Random.Range(0, my3dButtons.Length)];

        int _oldIndex =EnumToInt(curentTarget.GetComponent<Button3D>().button3DName);
        int _newIndex = EnumToInt(_newCurentButton3D.GetComponent<Button3D>().button3DName); 


        if ( _newIndex== _oldIndex)
        {
            RandomNewSprite();
        }
        else
        {
            curentTarget = _newCurentButton3D; 
            DestroyImmediate(GameObject.Find("temp"));
            GameObject temp = Instantiate(curentTarget);
            temp.transform.name = "temp";
            temp.transform.position = targetPosition.transform.position;
        }
    }

    public int EnumToInt(Button3DName _name)
    {
        return (int)_name; 
    }

    public void StopTest(Button3DName _name)
    {
        testIsRunNow = false;
        StopAllCoroutines();
        
        var target = GameObject.Find("temp");
        if(target != null)
        {
            DestroyImmediate(target); 
        }

        startTestButton3D.GetComponent<Button3D>().isActivNow = true;

        SaveTestScore();

        score = 0; 

    }

    public void SaveTestScore()
    {
        
        Debug.Log("Zapisuje wynik testu , wynik : " + score); 
    }


}
