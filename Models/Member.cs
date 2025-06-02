namespace users.Models
{
    public class Member
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;

    public string DisplayName => $"{MemberId} - {Name}";
}

}