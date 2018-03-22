using System;

[Serializable]
public class Card
{
    public string name;
    public string mana_cost;
    public Image_Uris image_uris;
}

[Serializable]
public class Image_Uris
{
    public string normal;
}