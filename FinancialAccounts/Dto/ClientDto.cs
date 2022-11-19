using System.ComponentModel.DataAnnotations;
using FinancialAccounts.Models;

namespace FinancialAccounts.Dto;

public class ClientDto
{
    /// <summary>
    /// Id
    /// </summary>
    public long? Id { get; set; }
    
    /// <summary>
    /// First name
    /// </summary>
    [Required]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid first name length")]
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Last name
    /// </summary>
    [Required]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid last name length")]
    public string? LastName { get; set; }
    
    /// <summary>
    /// Patronymic
    /// </summary>
    [Required]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid patronymic length")]
    public string? Patronymic { get; set; }
    
    /// <summary>
    /// Birthdate
    /// </summary>
    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/0:MM/yyyy}")]
    public DateTime Birthdate { get; set; }
}
