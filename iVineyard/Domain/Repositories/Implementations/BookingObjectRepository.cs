using Domain.Repositories.Interfaces;
using Model.Configurations;
using Model.Entities.Bookingobjects;

namespace Domain.Repositories.Implementations;

public class BookingObjectRepository(ApplicationDbContext context)
    : ARepository<BookingObject>(context), IBookingObjectRepository
{
    
}