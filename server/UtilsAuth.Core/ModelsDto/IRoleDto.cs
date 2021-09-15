using System;

namespace UtilsAuth.Demo.ModelsDto
{
    public interface IRoleDto
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}