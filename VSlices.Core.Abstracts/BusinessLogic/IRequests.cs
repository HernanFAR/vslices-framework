using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.BusinessLogic;

/// <summary>
/// A non objetive-specific request that expects a <see cref="Response{TResponse}" />
/// </summary>
/// <typeparam name="TResponse">The expected response of this request</typeparam>
public interface IRequest<TResponse> { }

/// <summary>
/// A non objetive-specific request that expects a <see cref="Response{Success}" />
/// </summary>
public interface IRequest : IRequest<Success> { }

/// <summary>
/// A request that mutates state and expects a <see cref="Response{TResponse}" /> 
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface ICommand<TResponse> : IRequest<TResponse> { }

/// <summary>
/// A request that mutates state and expects a <see cref="Response{Success}" />
/// </summary>
public interface ICommand : ICommand<Success> { }

/// <summary>
/// A request that queries data and expects a <see cref="Response{TResponse}" />
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IQuery<TResponse> : IRequest<TResponse> { }
