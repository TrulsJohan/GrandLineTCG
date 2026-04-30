namespace GrandLineTCG.interfaces;

public interface IEvent
{
    Guid Id { get; }
    User Host { get; }
    string Title { get; }
    string Description { get; }
    string Location { get; }
    decimal Price { get; }
    DateTime EventDate { get; }
    EventStatus Status { get; set; }
    int ParticipantsCount { get; }
    bool IsFull { get; }
    int MaxCapacity { get; } 
    SlotStatus SlotStatus { get; }
    bool AddParticipant(User user);
    void RemoveParticipant(User user);
    void UpdateStatus();
}