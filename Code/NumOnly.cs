using Godot;
using System;

public partial class NumOnly : LineEdit
{
    private RegEx LineEditRegEx = new RegEx();
    private String oldText;
    public override void _Ready()
    {   
        LineEditRegEx.Compile("^[0-9.]*$");
		base.TextChanged += (newText) => text_changed(newText);
    }

    private void text_changed(String newText)
    {
        if(LineEditRegEx.Search(newText) != null)
		    oldText = newText;
	    else
		    base.Text = oldText;
            base.CaretColumn = base.Text.Length;
    }
}
