using Domain.Repositories.Implementations;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Model.Configurations;
using Model.Entities.Bookingobjects;
using Models;
using Services.Interfaces;

namespace Services.Implementations;

public class RegisterService(
    IUserStore<ApplicationUser> userStore,
    UserManager<ApplicationUser> userManager,
    ILogger<RegisterService> logger,
    SignInManager<ApplicationUser> signInManager,
    NavigationManager navigationManager,
    IBookingObjectRepository bookingObjectRepository)
    : IRegisterService
{
    private IEnumerable<IdentityError>? identityErrors;
    private IBookingObjectRepository? _bookingObjectRepository = bookingObjectRepository;
    private readonly IUserStore<ApplicationUser>? _userStore = userStore;
    private readonly UserManager<ApplicationUser>? _userManager = userManager;
    private readonly ILogger<RegisterService>? _logger = logger;
    private readonly SignInManager<ApplicationUser>? _signInManager = signInManager;
    private readonly NavigationManager? _navigationManager = navigationManager;

    public async Task RegisterUser(EditContext editContext, string returnUrl)
    {
        var registerModel = editContext.Model as RegisterModel;
        var user = await CreateUser();

        await _userStore.SetUserNameAsync(user, registerModel.Email, CancellationToken.None);
        var emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(user, registerModel.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, registerModel.Password);

        if (!result.Succeeded)
        {
            identityErrors = result.Errors;
            return;
        }
        
        _logger.LogInformation("User created a new Account with password.");
        
        await _userManager.AddToRoleAsync(user, "Admin");

        await _signInManager.SignInAsync(user, true);
        _navigationManager.NavigateTo(returnUrl);
    }

    public async Task<BookingObject> CreateBookingObject()
    {
        var bookingObject = await _bookingObjectRepository.CreateAsync(new BookingObject());
        return bookingObject;
    }

    public async Task<ApplicationUser> CreateUser()
    {
        try
        {
            var bookingObject = await CreateBookingObject();
            var user = Activator.CreateInstance<ApplicationUser>();
            user.BookingObjectId = bookingObject.Id;
            return user;
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                                                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
        }
    }

    public IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }

        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
}
