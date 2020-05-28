using System.ComponentModel.DataAnnotations;

namespace SubscriptionService.Models { 
public class Category
{
    public int Category_Id { get; set; }

    [StringLength(50)]

    public string Category_Name { get; set; }
}
}