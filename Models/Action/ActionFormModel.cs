using Microsoft.AspNetCore.Mvc;

namespace AMZN.Models.Action;

public class ActionFormModel
{
    [FromForm(Name = "action-name")]
    public string Name { get; set; } = null!;
    [FromForm(Name = "action-description")]
    public string? Description { get; set; } = null!;
    [FromForm(Name = "apply-to")]
    public string ApplyTo { get; set; } = null!;
    [FromForm(Name = "product-title")]
    public string ProductTitle { get; set; } = null!;
    [FromForm(Name = "action-amount")]
    public int Amount { get; set; }
    [FromForm(Name = "action-start-date")]
    public DateTime StartDate { get; set; }
    [FromForm(Name = "action-end-date")]
    public DateTime EndDate { get; set; }
}