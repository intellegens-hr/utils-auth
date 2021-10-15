using System;
using System.ComponentModel.DataAnnotations;

namespace UtilsAuth.DbContext.Models
{
    public class RefreshToken
    {
        [DataType(DataType.DateTime)]
        public int HoursValid { get; set; }

        [Key]
        public int Id { get; set; }

        public bool IsInvalidated { get; set; }
        public bool IsUsed { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TimeCreated { get; set; }

        [Required, StringLength(64)]
        public string Token { get; set; }

        public int UserId { get; set; }
    }
}