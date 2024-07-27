using System.ComponentModel.DataAnnotations;

namespace BankManagementSystem.Models
{
    public class CustomerAccountHybridModel
    {
        //public int CustomerId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }

        [Phone]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string PanNo { get; set; }
        [Required]
        public string AadharNo { get; set; }
        [Required]
        public string BranchLocation { get; set; }
        [Required]
        public string AccountType { get; set; } = string.Empty;
        [Required]
        public decimal Balance { get; set; }

        public CustomerAccountHybridModel()
        {
            
        }


        //public CustomerModel(int cid, string name, string mobile, string email, string gender, DateTime dob, string panno, string aadhar, string branchLoc)
        //{
        //    CustomerId = cid;
        //    Name = name;
        //    Mobile = mobile;
        //    Email = email;
        //    Gender = gender;
        //    DateOfBirth = dob;
        //    PanNo = panno;
        //    AadharNo = aadhar;
        //    BranchLocation = branchLoc;
        //}
    }
}
