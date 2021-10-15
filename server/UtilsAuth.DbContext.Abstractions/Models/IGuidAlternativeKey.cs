using System;

namespace UtilsAuth.DbContext.Models
{
    public interface IGuidAlternativeKey
    {
        public Guid IdGuid { get; set; }
    }
}