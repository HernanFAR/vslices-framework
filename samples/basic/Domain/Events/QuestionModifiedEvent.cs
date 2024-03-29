﻿using VSlices.Core.Abstracts.BusinessLogic;

namespace Domain.Events;

public enum ModificationType
{
    Creation, Modification, Deletion
}

public record QuestionModifiedEvent(ModificationType ModificationType, string QuestionId, string NewField, string NewContent) : EventBase;
