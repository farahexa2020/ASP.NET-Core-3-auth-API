using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp1.Controllers.Resources
{
  public class CreateRoleResource
  {
    public string Id { get; set; }

    [Required]
    public string RoleName { get; set; }
  }
}