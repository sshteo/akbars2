namespace akbars.ViewModels
{
    public class UpdateRoleForm
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }
    }

    public class AddPriorityForm
    {
        public string Name { get; set; } = string.Empty;

        public int SlaHours { get; set; }
    }

    public class AddStatusForm
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }

    public class AddTypeForm
    {
        public string Name { get; set; } = string.Empty;
    }
}
