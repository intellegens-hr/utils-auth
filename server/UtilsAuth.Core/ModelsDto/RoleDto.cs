using System;

namespace UtilsAuth.Demo.ModelsDto
{
    public class RoleDto : IRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}