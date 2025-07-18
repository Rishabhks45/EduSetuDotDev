using System.ComponentModel.DataAnnotations;
using EduSetu.Domain.Enums;

namespace EduSetu.Domain.Entities;

public abstract class BaseEntity
{
 [Key]
public Guid Id { get; set; } = Guid.NewGuid();
public DateTime CreatedDate { get; set; } = DateTime.Now;
public DateTime? LastModifiedDate { get; set; }
public Guid CreatedBy { get; set; }
public Guid LastModifiedBy { get; set; }
public RowStatus RowStatus { get; set; } = RowStatus.Inactive;
} 