using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Model.Configurations;
using Model.Entities.Bookingobjects;
using Models;

namespace Services.Interfaces;

public interface IRegisterService
{ 
    Task RegisterUser(EditContext editContext, string returnUrl);
    Task<ApplicationUser> CreateUser();
    Task<BookingObject> CreateBookingObject();
    IUserEmailStore<ApplicationUser> GetEmailStore();
}