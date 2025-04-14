using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public char Letter { get; private set; }
    public Node Node;
    public Block MergingBlock;
    public bool Merging;
    public Vector2 Pos => transform.position;

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _text;

    public void Init(BlockType type)
    {
        Letter = type.Letter;
        _renderer.color = type.Color;
        _text.text = Letter.ToString();
    }

    public void SetBlock(Node node)
    {
        if (Node != null) Node.OccupiedBlock = null;
        Node = node;
        Node.OccupiedBlock = this;
    }

    public void MergeBlock(Block blockToMergeWith)
    {
        MergingBlock = blockToMergeWith;
        Node.OccupiedBlock = null;
        blockToMergeWith.Merging = true;
    }

    public bool CanMerge(char otherLetter)
    {
        return Letter == otherLetter && !Merging && MergingBlock == null && Letter < 'Z';
    }
}
