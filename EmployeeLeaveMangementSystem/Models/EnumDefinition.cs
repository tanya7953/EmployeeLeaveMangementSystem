namespace EmployeeLeaveMangementSystem.Models
{
    public class EnumDefinition
    {
        public enum LeaveType
        {
            SickLeave,
            VacationLeave,
            MaternityLeave
        }

        public enum LeaveStatus
        {
            Pending,
            Approved,
            Rejected
        }
    }
}
