using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIDemo.BAL.Entity
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int c_userid { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string c_email { get; set; }

        [Required]
        [StringLength(256)]
        public string c_passwordhash { get; set; }

        [Required]
        [StringLength(50)]
        public string c_firstname { get; set; }

        [Required]
        [StringLength(50)]
        public string c_lastname { get; set; }

        [StringLength(15)]
        public string c_contactnumber { get; set; }

        [Required]
        public int c_roleid { get; set; }

        [Required]
        public int c_createdby { get; set; }

        [Required]
        public DateTime? c_createdat { get; set; }

        [Required]
        public int c_updatedby { get; set; }

        public DateTime? c_updatedat { get; set; }

        [Required]
        public bool c_isactive { get; set; } = true;
        public string c_rolename { get; set; }
        public string c_createdbyemail { get; set; }
        public string c_updatedbyemail { get; set; }
    }
}
