using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class Card : MonoBehaviour
{
    private int type = 0;
    private SpriteRenderer spriteRenderer;
    private bool isFlipped = false;
    private bool isGuessed = false;
    private Quaternion original_rotation;

    
    private BoardManager board;
    // Start is called before the first frame update
    void Awake()
    {
        original_rotation = this.transform.rotation;
        spriteRenderer = this.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        board = GameObject.FindGameObjectWithTag("GameController").GetComponent<BoardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        if (!isFlipped) {
            setFlipped(true);
            StartCoroutine(RotateAndAddCard(1f));
        }
        Debug.Log(isFlipped);
    }

    private void OnEnable()
    {
        if (!isGuessed)
        {
            this.setFlipped(false);
            this.transform.rotation = original_rotation;
        }
    }
    public void setCard(int t,Sprite s)
    {
        
        this.type = t;
        this.spriteRenderer.sprite = s;
    }

    IEnumerator RotateAndAddCard(float duration)
    {
        this.setFlipped(true);
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 180.0f;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            yield return null;
        }
        board.addSelectedCard(this);
        yield return null;
    }

    IEnumerator Rotate(float duration)
    {
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation - 180.0f;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            yield return null;
        }
        this.setFlipped(false);
        yield return null;
    }

    public int getType()
    {
        return type;
    }
    public void setFlipped(bool val)
    {
        this.isFlipped = val;
    }

    public void setGuessed()
    {
        this.isGuessed = true;
    }

    public void rotate()
    {
        StartCoroutine(Rotate(1f));
    }

    public SpriteRenderer getSP()
    {
        return spriteRenderer;
    }
}
