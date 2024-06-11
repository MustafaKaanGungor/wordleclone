using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Tile : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] public Image tileImage;
    [SerializeField] public char letter {get; private set;}

    public Colours currentColour = Colours.EMPTY;

    private Color originalColour;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        tileImage = GetComponent<Image>();
        originalColour = tileImage.color;
    }

    public void SetLetter(char letter) {
        this.letter = letter;
        text.text = letter.ToString();
    }

    public void SetColour(Colours colour) {
        if(colour == Colours.GREEN) {
            tileImage.color = Color.green;
            currentColour = Colours.GREEN;
        } else if( colour == Colours.YELLOW) {
            tileImage.color = Color.yellow;
            currentColour = Colours.YELLOW;

        } else if(colour == Colours.BLACK) {
            currentColour = Colours.BLACK;        
        } else {
            currentColour = Colours.EMPTY;
            tileImage.color = originalColour;
        }
    }
}

public enum Colours {
    GREEN,
    YELLOW,
    BLACK,
    EMPTY
}