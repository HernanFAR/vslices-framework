using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.BusinessLogic;

/// <summary>
/// Represents the start point of any business logic
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IBaseRequest<TResponse> {}

/// <summary>
/// Represents the start point of a use case, with a specific response type
/// </summary>
/// <typeparam name="TResponse">The expected response of this request</typeparam>
public interface IRequest<TResponse> : IBaseRequest<TResponse> { }

/// <summary>
/// Represents the start point of a use case, with a success response
/// </summary>
public interface IRequest : IRequest<Success> { }

/// <summary>
/// Represents the start point of a use case that mutates state, with a specific response type
/// </summary>
/// <typeparam name="TResponse">The expected response of this request</typeparam>
public interface ICommand<TResponse> : IRequest<TResponse> { }

/// <summary>
/// Represents the start point of a use case that mutates state, with a success response
/// </summary>
public interface ICommand : ICommand<Success> { }

/// <summary>
/// Represents the start point of a use case that queries data, with a specific response type
/// </summary>
/// <typeparam name="TResponse">The expected response of this request</typeparam>
public interface IQuery<TResponse> : IRequest<TResponse> { }

/// <summary>
/// Represents the start point of a side effect of a use case
/// </summary>
public interface IEvent : IBaseRequest<Success> { }
