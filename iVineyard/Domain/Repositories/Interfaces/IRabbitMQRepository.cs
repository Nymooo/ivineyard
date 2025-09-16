namespace Domain.Repositories.Interfaces;

public interface IRabbitMQRepository {
    public Task<List<string>> GetMessagesAsync();
}