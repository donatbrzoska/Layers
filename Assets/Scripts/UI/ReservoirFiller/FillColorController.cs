using System;
using System.Collections.Generic;
using UnityEngine;

public class FillColorController : DropdownController
{
    new public void Awake()
    {
        base.Awake();
        // Using sprites needs setup: https://forum.unity.com/threads/sprites-in-dropdown-list.374405/
        // "The dropdown supports an image for each option when the Caption Image and
        //  Item Image properties are both setup. These are not setup by default.
        //  - The Caption Image is the Image component to hold the image for the currently selected option.
        //    It is typically a child to the Dropdown GameObject.
        //  - The Item Image is the Image component to hold the image for each option.
        //    It is typically a child to the Item GameObject.
        //  So I set the "Item Image" property of my dropdown to the "Item Background" underneath Template.
        //  For the "Caption Image", I added a new UI.Image object as a child of the Dropdown and then set the
        //  "Caption Image" property of my dropdown to this new Image object.
        //  (I then set the Rect Transform of this new Image to stretch horizontally within the dropdown)."

        List<TMPro.TMP_Dropdown.OptionData> options = new List<TMPro.TMP_Dropdown.OptionData>();
        foreach (Color_ color_ in Enum.GetValues(typeof(Color_)))
        {
            // create text
            string name = Colors.GetName(color_);

            // create sprite
            Vector3 rgb = Colors.GetColor(color_);
            Color color = new Color(rgb.x, rgb.y, rgb.z);

            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, color);
            t.Apply();

            Sprite s = Sprite.Create(t, new Rect(0, 0, 1, 1), new Vector2(0, 0));

            // create and add option
            options.Add(new TMPro.TMP_Dropdown.OptionData(name, s));
        }
        Dropdown.AddOptions(options);
    }

    public void Start()
    {
        Dropdown.SetValueWithoutNotify((int)OilPaintEngine.Config.FillConfig.Color);
    }

    override public void OnValueChanged(int value)
    {
        OilPaintEngine.UpdateFillColor((Color_)value);
    }
}
