using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Core.Models
{
  [Table("RefreshTokens")]
  public class RefreshToken
  {
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    public User User { get; set; }

    [Required]
    public string AccessToken { get; set; }

    [Required]
    [StringLength(255)]
    public string Token { get; set; }

    public DateTime CreatedAt { get; set; }
  }
}