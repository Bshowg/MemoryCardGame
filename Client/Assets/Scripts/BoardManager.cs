using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    private static readonly int SPRITE_NUM = 34;
    private static readonly string LOAD_PREFIX = "unit_";

    private static readonly int num_x=7;
    private static readonly int num_y = 4;

    private static readonly int num_triples = (int)Mathf.Floor((num_x * num_y) / 3);


    private Sprite[] locked_unit= new Sprite[SPRITE_NUM];

    private Sprite[] unit=new Sprite[SPRITE_NUM];

    private GameObject[] cards = new GameObject[num_x * num_y];

    [SerializeField]
    private GameObject cardPrefab;

    private List<Card> selected_cards = new List<Card>();

    private int current_triplets = 0;

    [SerializeField]
    private InterfaceManager im;

    private GameObject cardRoot;


    // Start is called before the first frame update
    void Start()
    {
        //LOAD RESOURCES
        for(int i=0; i< SPRITE_NUM; i++)
        {
            cardRoot = this.transform.GetChild(0).gameObject;
            unit[i]=Resources.Load<Sprite>("unit_cards/" + LOAD_PREFIX + (i+1).ToString());
            locked_unit[i]=Resources.Load<Sprite>("unit_cards_lock/" + LOAD_PREFIX + (i + 1).ToString());
        }

        //INIT BOARD
        for(int x = 0; x < num_x; x++)
        {
            for(int y=0; y<num_y; y++)
            {
                
                cards[(x*num_y)+y]=Instantiate(cardPrefab, this.transform.GetChild(0));
                cards[(x*num_y)+y].transform.position = new Vector3(x*1.5f, y*2f, 0);
                
            }
        }

        //INIT CARDS
        
        List<GameObject> tmp = new List<GameObject>();
        tmp = cards.ToList();
        Shuffle(tmp);
        List<int> extracted = new List<int>();

        makeTriplets(tmp,extracted);

    }

    private void makeTriplets(List<GameObject> tmp,List<int> extracted)
    {
        
        //actual triples
        for (int i = 0; i < num_triples*3; i = i + 3)
        {
            int random = newUniqueIndex(extracted);
            tmp[i].GetComponent<Card>().setCard(random, unit[random]);
            tmp[i + 1].GetComponent<Card>().setCard(random, unit[random]);
            tmp[i + 2].GetComponent<Card>().setCard(random, unit[random]);
        }
        //remaining cards
        for (int i = (num_triples*3); i <= tmp.Count-1; i++)
        {
            int random = newUniqueIndex(extracted);
            tmp[i].GetComponent<Card>().setCard(random, unit[random]);
        }
    }

    private int newUniqueIndex(List<int> extracted)
    {
        int random = Random.Range(0, 34);
        while (extracted.Contains(random))
        {
            random = Random.Range(0, 34);
        }
        extracted.Add(random);
        return random;
    }

    // Method to shuffle a Deck using Fisher-Yates shuffle.
    public static void Shuffle(List<GameObject> list)
    {
        System.Random random = new System.Random();

        for (int i = 0; i < list.Count; i++)
        {
            int j = random.Next(i, list.Count);
            GameObject temporary = list[i];
            list[i] = list[j];
            list[j] = temporary;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (selected_cards.Count == 3)
        {
            var same = compare();
            if (same)
            {
                im.success(selected_cards[0].getSP().sprite);
                current_triplets++;
                foreach (Card card in selected_cards)
                {
                    
                    
                    card.getSP().sprite = locked_unit[card.getType()];
                    //card.setFlipped(true);
                    card.setGuessed();
                    
                }
            }
            else
            {
                im.failure();
                foreach (Card card in selected_cards)
                {
                    card.rotate();
                    //card.setFlipped(false);
                    
                }
                
            }
            
            selected_cards.Clear();
            if (current_triplets == num_triples)
            {
                Destroy(this.gameObject.transform.GetChild(0).gameObject);
                im.endGame();
            }
        }
    }


    public void addSelectedCard(Card card)
    {
        
        this.selected_cards.Add(card);
        
    }

    private bool compare()
    {
        if (selected_cards[0].getType() == selected_cards[1].getType())
        {
            if(selected_cards[2].getType() == selected_cards[1].getType())
            {
                return true;
            }
        }
        return false;
    }

    public void GameView()
    {
        cardRoot.SetActive(true);
        im.GameView();
    }
    public void Leaderboard()
    {
        selected_cards.Clear();
        cardRoot.SetActive(false);
        im.Leaderboard();
    }


}
