namespace users.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }

        public string DisplayName => $"{AccountNumber} - {Balance:C}";
    }

}