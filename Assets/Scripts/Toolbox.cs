public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox() { } // guarantee this will be always a singleton only - can't use the constructor!

    //public InputHandler InputHandler;

    public float Speed;
    public int Status = 0;
    /*// Use this for initialization
    void Start()
    {

    }*/

    /*// Update is called once per frame
    void Update()
    {

    }*/
}