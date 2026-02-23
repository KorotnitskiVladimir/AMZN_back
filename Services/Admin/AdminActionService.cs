using AMZN.Models;
using AMZN.Repositories.Actions;

namespace AMZN.Services.Admin;

public class AdminActionService
{
    private readonly IActionRepository _actions;
    private readonly FormsValidators _formsValidator;

    public AdminActionService(IActionRepository actions,
        FormsValidators formsValidator)
    {
        _actions = actions;
        _formsValidator = formsValidator;
    }
}