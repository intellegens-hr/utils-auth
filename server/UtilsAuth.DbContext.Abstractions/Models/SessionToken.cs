using System;
using System.ComponentModel.DataAnnotations;

namespace UtilsAuth.DbContext.Models
{
    public class SessionToken
    {
        public const string PayloadName = "sessiontoken";

        [Key]
        public int Id { get; set; }

        public bool IsInvalidated { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TimeCreated { get; set; }

        [Required, StringLength(64)]
        public string Token { get; set; }

        public int UserId { get; set; }
    }
}