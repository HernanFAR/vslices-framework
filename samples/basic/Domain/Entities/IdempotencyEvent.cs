using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSlices.Domain;

namespace Domain.Entities;

public class IdempotencyEvent : Entity
{
    public Guid EventId { get; private set; }
    public Guid HandlerId { get; private set; }

    public IdempotencyEvent(Guid eventId, Guid handlerId)
    {
        EventId = eventId;
        HandlerId = handlerId;
    }

    public override object[] GetKeys()
    {
        return new object[] { EventId, HandlerId };
    }

    internal static class Database
    {
        public const string TableName = "IdempotencyEvents";
    }
}
