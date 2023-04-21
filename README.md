# vertical-slice-sample

Este codigo es un ejemplo de como emplear el concepto del patron de arquitectura "vertical slices" en codigo C#.

## Preludio a la explicación

Antes que nada, primero deberiamos comenzar de apoco. Una forma tipica de ordenar codigo, es emplear una arquitectura de 3 capas, parecida a esta:

<img 
  alt="Vertical slices - Arquitectura 3 capas" 
  src="https://user-images.githubusercontent.com/51165159/233527136-d95c220d-c819-449f-8db3-92ff0aa3a23d.png" 
  width="360px" />
  
Esta arquitectura se leeria de esta forma: la capa de "Presentación" conoce y depende de la capa de "Lógica de negocio", y esta ultima, conoce y depende de la capa de "Acceso de datos".

Una de las primeras mejoras que alguien suele agregar, es invertir la dependencia entre la capa de "Lógica de negocio" y "Acceso a datos". Esto quedaría así: 

<img 
  alt="Vertical slices - Arquitectura 3 capas, invertida" 
  src="https://user-images.githubusercontent.com/51165159/233527581-ba878cb5-2a73-4c55-8d8a-6a23ce6eda9e.png" 
  width="360px" />

El solo hacer esta inversión, cambia mucho el juego de como se codifica, ya que, ahora que la capa de "Lógica de negocio" no conoce como se accede a los datos ¿Como hace para poder persistir la información? Sencillo, interfaces.

La capa de "Lógica", establece unas interfaces y como la capa de "Acceso a datos", conoce y depende de esta, puede acceder a ellas, de este modo, se genera una clase que implementa los metodos de la interface, estableciendo ahí el como se accede a los datos. Si diagramamos esto, quedaría así: 

<img 
  alt="Vertical slices - Componentes de 3 capas, invertida" 
  src="https://user-images.githubusercontent.com/51165159/233530202-0d589b65-7d09-4c65-915d-5c2887f51dd7.png" 
  width="720px" />
  
De este modo, el servicio de "Logica de negocio" solo emplea la interface que esta definido en el mismo paquete, y el como se accede a los datos, se deja aparte en "Acceso a datos" (Este patrón se llama _"repository"_). 


Esta es la base en la que funcionan la Arquitectura Hexagonal, y Clean arquitecture, el principio de inversión del control (La I en SOLID).

De hecho, podriamos dar un paso más allá y centralizar los objetos con los que trabajaremos (Por ejemplo, con Domain Driven Development) en una capa de nombre "Dominio" y se tendría el diagrama de "Clean Architecture":

<img
  alt="Vertical slices - Clean architecture" 
  src="https://user-images.githubusercontent.com/51165159/233529549-e84eda6f-7fdd-406f-a3ea-390735b13f51.png" 
  width="480px" />

Se sigue la misma regla de lectura, "Presentación" conoce y depende de "Lógica", "Lógica" conoce y depende de "Dominio", pero, cualquier abstracción (Acceso a datos, subida de archivos, validación, etc.) se representa con una interface que existe en "Logica" o "Dominio", pero esta implementada en "Infrastructura" (la nueva capa)

## ¿Cual es el sentido del patrón _Vertical slices_?

Implementando todo lo anterior, tendriamos un sistema bastante robusto, escalable, testeable, etc. etc.; pero al ser un sistema hecho en capas, es inevitable que existan cruces de métodos.

Por decir algo, en la interface de "IQuestionRepository" tendriamos un método "GetQuestion", que retorna una pregunta persistida en la base de datos, y este metodo sería compartido entre los casos de uso de "RemoveQuestion", "GetOneQuestion" y "UpdateQuestion"; si bien esto no es malo, este caso de uso de metodos cruzados puede darse en contextos donde si puede ser peligroso.

Pero, en realidad, el problema del uso cruzado de métodos no es tanto si pones el código en manos de un desarrollador experimentado. 

Y es acá donde realmente inicia el problema, un desarrollador junior no podria desarrollar en un sistema hecho con clear arquitecture de manera cómoda y si esta trabajando en el caso de uso "GetQuestion", corre el riesgo de que toque las funcionalides "UpdateQuestion" y "RemoveQuestion".

Teniendo ese problema ¿Como podriamos mejorar esto para hagamos un sistema robusto, escalable, testeable y que aún así, un desarrollador junior podria trabajar en el? ¿Conoces el concepto de "silo"?

Sí, el mismo en donde se guarda granos de trigo o bueno, da igual eso. El punto, es que cada silo, por si mismo, esta aislado del resto, si hay un problema con uno, no tenemos porque revisar que el resto este bien, porque estan aislados. En nuestro caso, no guardamos granos de trigo, pero desarrollamos casos de uso.

¿Y sí mejor, dejamos que cada caso de uso, actue por si mismo de forma aislada, como un silo? El concepto de "vertical slices" es eso, que cada caso de uso, actue como un silo, y que en el mismo caso de uso, se vea como se presenta, como es su logica interna, como se accede/persiste información, etc. si lo hicieramos, quedaría así.

## Implementando Vertical slices

Si hacemos aplicamos el concepto de silo en lo que ya tenemos, quedaría un diagrama así.

<img
  alt="Vertical slices - Arquitectura vertical slices" 
  src="https://user-images.githubusercontent.com/51165159/233533413-cd500962-da6b-46de-8ccf-c90c2e27d567.png" 
  width="720px" />

Aún que al inicio pueda parecer muy diferente de como era el diagrama anterior, centremonos primero en lo que conocemos para ir entendiendo.

Dentro de "Core" (la caja gris) tenemos dos "capas" horizontales, "Presentación" y "Lógica" y en la misma sección en vertical tenemos los casos de uso. 

Tal como se dijo antes, cada caso de uso es el responsable de definir como es su presentación y su lógica, por lo que, dentro de ellos, veremos un par de clases dedicadas a esto, junto con una inteface que habla de las funciones de repositorio (acceso a datos) que necesita.

Siguiendo la misma linea de inversión de control, la lógica va a usar dicha interface y no la implementación, ya que estas, estarán en "Infrastructura". Todos lo anterior, depende del dominio.

Lo que agrega el patrón, es la capa superior "Integrador", ya que, como cada caso de uso puede tener diferente forma de presentarse, pudiendo ser una RestApi, un gRPC, lambdas de AWS, Functions de Azure, etc. debemos tener uno o varios integradores cuya unica función sería exponer al publico la forma de presentar cada caso de uso.

## ¿Cómo sería una implementación en codigo de vertical slices?

¡Para esto esta el código de este repositorio! Dentro de esta solución de C# .net, tenemos 4 paquetes, "Integrator", "Core", "Infrastructure" y "Domain"

![image](https://user-images.githubusercontent.com/51165159/233534886-be8c1f41-8e21-4966-a152-5233ff7b6dca.png)

Dentro de "Core" tenemos el grueso calibre del codigo, los casos de uso, mostraré un ejemplo acá:

```c#
// Archivo: Core/UseCases/CreateQuestion.cs

namespace Core.UseCases;

// Presentación
public record CreateQuestionContract(string Name);

public class CreateQuestionEndpoint : IEndpointDefinition
{
    public const string Uri = "/api/question";

    public void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/question", CreateQuestionAsync)
            .WithSwaggerOperationInfo("Crea una pregunta", "Crea una pregunta, en base al body")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status422UnprocessableEntity);

    }

    public static async Task<IResult> CreateQuestionAsync(
        [FromBody] CreateQuestionContract createQuestionContract,
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateQuestionCommand(
            createQuestionContract.Name,
            contextAccessor.GetIdentityId());

        var response = await sender.Send(command, cancellationToken);

        return response
            .Match<IResult>(
                e => TypedResults.Created($"/api/question/{e}"),
                e => TypedResults.UnprocessableEntity(e.Value));
    }

}

// Lógica
public record CreateQuestionCommand(string Name, Guid CreatedBy) : IRequest<OneOf<Guid, Error<string[]>>>;

public class CreateQuestionHandler : IRequestHandler<CreateQuestionCommand, OneOf<Guid, Error<string[]>>>
{
    private readonly ICreateQuestionRepository _repository;
    private readonly IValidator<CreateQuestionCommand> _contractValidator;
    private readonly IValidator<Question> _domainValidator;

    public CreateQuestionHandler(ICreateQuestionRepository repository,
        IValidator<CreateQuestionCommand> contractValidator,
        IValidator<Question> domainValidator)
    {
        _repository = repository;
        _contractValidator = contractValidator;
        _domainValidator = domainValidator;
    }

    public async Task<OneOf<Guid, Error<string[]>>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var contractValidationResult = await _contractValidator.ValidateAsync(request, cancellationToken);

        if (!contractValidationResult.IsValid)
        {
            return new Error<string[]>(contractValidationResult
                .Errors.Select(e => e.ErrorMessage)
                .ToArray());
        }

        var question = new Question(request.Name, request.CreatedBy);

        var domainValidationResult = await _domainValidator.ValidateAsync(question, cancellationToken);

        if (!domainValidationResult.IsValid)
        {
            return new Error<string[]>(domainValidationResult
                .Errors.Select(e => e.ErrorMessage)
                .ToArray());
        }

        await _repository.SaveQuestion(question, cancellationToken);

        return question.Id;
    }
}

public class CreateQuestionValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty();

        RuleFor(e => e.CreatedBy)
            .NotEmpty();

    }
}

public interface ICreateQuestionRepository
{
    Task SaveQuestion(Question question, CancellationToken cancellationToken = default);

}


```

Cada archivo tiene dos partes diferenciadas por un comentario: Presentación y Lógica, donde cada tiene codigo que tiene dicha responsabilidad. 

En este caso, el codigo emplea Minimal Apis para la presentación, con MediatR para la logica de negocio y una interface para las necesidades de acceso a datos (Cuya implementación esta en "Infrastructure/UseCases/CreateQuestion.cs).

De este modo, se consigue un codigo bastante consiso, legible y mantenible (¡Con tan solo 100 lineas!), en el cual, cualquier cambio que se haga, solo afecta a este caso de uso, ya que al ser un silo, aislado del resto, no hay código compartido con los otros casos.

La idea, es que si hay nuevos casos de uso, solo se crea un nuevo archivo, y se repite el patrón: se define la presentación, se define como se abarcará la lógica, se define la interface, se agrega la implementación y se realiza el testing.

¡Invito a ver el codigo a fondo! Esta perfectamente ejecutable tal como esta ahora mismo :)

## Información extra

Explicación dinamica de las arquitecturas mencionadas: https://www.youtube.com/watch?v=JubdZIdLQ4M

Explicación del concepto de silo de vertical slices: https://www.youtube.com/watch?v=S0zC0u5tIx8
