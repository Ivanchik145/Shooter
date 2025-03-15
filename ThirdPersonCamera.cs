using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour 
{        
    [SerializeField] GameObject player; // Указываем персонажа

    [SerializeField][Range(0.5f, 2f)] // Чувствительность мыши
    float mouseSense = 1; 
    [SerializeField][Range(-20, -10)] //  камера вверх и вниз
    int lookUp = -15;
    [SerializeField][Range(15, 25)]
    //булева переменная, для определения текущего состояния игрока
    public bool isSpectator;
    
    int lookDown = 20;
    //скорость полета свободной камеры
    [SerializeField] float speed = 50f;
    [SerializeField] Vector3 cameraFirstPerson;
    [SerializeField] Vector3 cameraThirdPerson;
    
    private void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked; //Эта строчка блокирует курсор
    }
    void Update()
    {     
        float rotateX = Input.GetAxis("Mouse X") * mouseSense; // Вращение в право 
        float rotateY = Input.GetAxis("Mouse Y") * mouseSense; //  и в лево
        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            //если курсор заблокирован, то..
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                //разблокируем курсор
                Cursor.lockState = CursorLockMode.None;
            }
            //иначе..
            else
            {
                //блокируем курсор
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
        
        if (!isSpectator)
        {
            Vector3 rotCamera = transform.rotation.eulerAngles; // поворот камеры 
            Vector3 rotPlayer = player.transform.rotation.eulerAngles; // поворот пресонажа

            rotCamera.x = (rotCamera.x > 180) ? rotCamera.x - 360 : rotCamera.x;
            rotCamera.x = Mathf.Clamp(rotCamera.x, lookUp, lookDown);// ограничения для камеры вверх и вниз 
            rotCamera.x -= rotateY;               
            
            rotCamera.z = 0;
            rotPlayer.y += rotateX; // лево право 

            transform.rotation = Quaternion.Euler(rotCamera);
            player.transform.rotation = Quaternion.Euler(rotPlayer);
        }
        else
        {
            //получаем текущий поворот камеры
            Vector3 rotCamera = transform.rotation.eulerAngles;
            //меняем поворот камеры в зависимости от значения движения мыши
            rotCamera.x -= rotateY;
            rotCamera.z = 0;
            rotCamera.y += rotateX;
            transform.rotation = Quaternion.Euler(rotCamera);
            //считываем значение клавиш WASD
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            
            //Задаем вектор направления полета камеры
            Vector3 dir = transform.right * x + transform.forward * z;
            //меняем позицию камеры
            transform.position += dir * speed * Time.deltaTime;
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(transform.localPosition == cameraThirdPerson)
            {
                transform.localPosition = cameraFirstPerson;
            }
            else
            {
                transform.localPosition = cameraThirdPerson;
            }

        }     

    }
}