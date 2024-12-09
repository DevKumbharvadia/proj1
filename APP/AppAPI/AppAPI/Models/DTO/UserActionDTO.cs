namespace AppAPI.Models.DTO
{
    public class UserActionDTO
    {
        public Guid UserActionId { get; set; } // Primary Key
        public string Action { get; set; } = null!; // Action name
        public DateTime TimeOfAction { get; set; } // UTC timestamp of the action
    }

}
