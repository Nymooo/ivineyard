using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Model.Configurations;
using Models;
using Services.Interfaces;

namespace Services.Implementations;

public class LoginService : ILoginService
{
    private readonly SignInManager<ApplicationUser>? _signInManager;
    private readonly LoginModel _loginModel = new();
    private readonly ILogger<LoginService> _logger;
    private readonly NavigationManager _navigationManager;

    public LoginService(SignInManager<ApplicationUser>? signInManager, ILogger<LoginService> logger, NavigationManager navigationManager)
    {
        _signInManager = signInManager;
        _logger = logger;
        _navigationManager = navigationManager;
    }

    public async Task LoginUser(EditContext editContext, string returnUrl)
    {
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        var loginModel = editContext.Model as LoginModel;
        
        var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in.");
            _navigationManager.NavigateTo(returnUrl);
        }
        else if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            _navigationManager.NavigateTo("Account/Lockout");
        }
    }
}