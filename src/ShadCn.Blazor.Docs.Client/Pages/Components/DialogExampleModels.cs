namespace ShadCn.Blazor.Docs.Client.Pages.Components;

public class UserProfile
{
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

public class DialogResult
{
    public bool Success { get; set; }
    public UserProfile? UpdatedProfile { get; set; }
}

public class NestedDialogModel
{
    public int Level { get; set; } = 1;
}
