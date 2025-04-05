namespace MediPulseWeb.Models.DTOs
{
    public class DoctorResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Speciality { get; set; } = string.Empty;
        public string CertificateUrl { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DoctorCertificate> Certificates { get; set; } = new();

        // Properties for view compatibility
        public string Name => FullName;
        public string Specialty => Speciality;
        public DateTime RegistrationDate => CreatedAt;
        public string ApprovalStatus => IsApproved ? "Approved" : IsRejected ? "Rejected" : "Pending";
    }

    public class DoctorCertificate
    {
        public string Name { get; set; } = string.Empty;
        public string IssuingAuthority { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public string FileUrl { get; set; } = string.Empty;
    }

    public class DoctorStatusResponse
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Speciality { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Properties for view compatibility
        public string Name => FullName;
        public string Specialty => Speciality;
    }

    public class UpdateDoctorProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Speciality { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public IFormFile? Certificate { get; set; }
        public List<DoctorCertificateRequest> Certificates { get; set; } = new();

        // Properties for view compatibility
        public string Name => FullName;
        public string Specialty => Speciality;
    }

    public class DoctorCertificateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string IssuingAuthority { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
    }
} 