namespace MediPulseWeb.Models.DTOs
{
    public class PatientResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<MedicalCondition> MedicalHistory { get; set; } = new();

        // Properties for view compatibility
        public string Name => FullName;
        public DateTime RegistrationDate => CreatedAt;
    }

    public class MedicalCondition
    {
        public string ConditionName { get; set; } = string.Empty;
        public DateTime DiagnosedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdatePatientProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<MedicalCondition> MedicalHistory { get; set; } = new();
    }
} 