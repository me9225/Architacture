using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.Models
{
    public enum Role
    {
        Manager,
        User
    }
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        [EmailAddress]
        public string EMail { get; set; }
        public string Address { get; set; }
        public List<Card> CardsList { get; set; }
        public List<Basket> BasketList { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
