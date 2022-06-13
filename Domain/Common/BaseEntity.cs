namespace Domain.Common;
public class BaseEntity
{
    public int Id { get; set; }
    public int CreatedBy { get; set; }
    public DateTime DateCreated { get; set; }
    public int? ModifiedBy { get; set; }
    public DateTime? DateModified { get; set; }
}
