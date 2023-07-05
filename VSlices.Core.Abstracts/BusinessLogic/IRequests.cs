using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.BusinessLogic;

public interface IRequest<TResponse> { }

public interface IRequest : IRequest<Success> { }

public interface ICommand<TResponse> : IRequest<TResponse> { }

public interface ICommand : ICommand<Success> { }

public interface IQuery<TResponse> : IRequest<TResponse> { }
