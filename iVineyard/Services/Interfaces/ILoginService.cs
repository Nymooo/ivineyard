using Microsoft.AspNetCore.Components.Forms;

namespace Services.Interfaces;

public interface ILoginService
{
    public Task LoginUser(EditContext editContext, string returnUrl);
}