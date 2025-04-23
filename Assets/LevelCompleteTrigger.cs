using UnityEngine;
using UnityEngine.SceneManagement; // ต้องเพิ่มบรรทัดนี้เพื่อใช้งาน SceneManager

public class LevelCompleteTrigger : MonoBehaviour // ตั้งชื่อสคริปต์ให้สื่อความหมาย (เช่น LevelCompleteTrigger)
{
    // ตัวแปรเก็บ Build Index ของฉากถัดไปที่จะโหลด
    public int nextSceneLoad;

    // Start is called before the first frame update
    void Start()
    {
        nextSceneLoad = SceneManager.GetActiveScene().buildIndex + 1;
    }
    public void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(nextSceneLoad);
            if(nextSceneLoad > PlayerPrefs.GetInt("levelAt", 0))
            {
                
                PlayerPrefs.SetInt("levelAt", nextSceneLoad);
              
                PlayerPrefs.Save();
            }
        }
    }

    
}