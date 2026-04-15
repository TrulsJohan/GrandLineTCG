namespace GrandLineTCG;

public enum ListingType
{
    Pokemon,
    YuGiOh,
    MagicTheGathering,
    OnePiece,
    FleshAndBlood,
    Lorcana,
    DragonBallSuper,
    Other
}

public enum TournamentType
{
    Official,
    Casual
}

public enum BookingStatus
{
    Available,
    FullyBooked,
    Closed
}

public enum PrizeType
{
    Cash,
    Products,
    Honor,
    Mixed
}

public enum Ruleset
{
    Regular,
    Unlimited,
    Expanded,
    Limited
}

public enum EventStatus
{
    Upcoming,
    InProgress,
    Completed,
    Cancelled,
    Postponed
}

public enum MaxParticipants
{
    _8 = 8,
    _16 = 16,
    _32 = 32,
    _64 = 64,
    _128 = 128
}